using KidsIdKit.Core.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;

namespace KidsIdKit.Core.SharedComponents;

public partial class PinEntry
{
    [Parameter] public bool IsSetupMode { get; set; }
    [Parameter] public bool HasLegacyData { get; set; }
    [Parameter] public EventCallback OnUnlocked { get; set; }
    [Parameter] public EventCallback OnSkipToInfo { get; set; }

    private readonly string[] pinDigits = new string[6];
    private readonly ElementReference[] inputRefs = new ElementReference[6];
    private int focusedIndex = 0;
    private string? pinErrorMessage;
    private string? biometricErrorMessage;
    private bool isProcessing;

    private string Title => IsSetupMode ? "Create Your PIN" : "Enter Your PIN";
    private string Subtitle => IsSetupMode
      ? (HasLegacyData ? "Set a PIN to secure your existing data" : "Set a PIN to protect your children's information")
      : "Enter your PIN to unlock";
    private string ButtonText => IsSetupMode ? "Set PIN" : "Unlock";

    private string CurrentPin => string.Join(String.Empty, pinDigits.Where(d => !string.IsNullOrEmpty(d)));

    private bool biometricAvailable;
    private bool biometricEnabled;

    private bool ShowImportBackupAlert = false;
    private bool ShowImportMessageAlert = false;
    private string ImportMessage = string.Empty;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            if (!IsSetupMode)
            {
                biometricAvailable = await BiometricService.IsAvailableAsync();
                if (biometricAvailable)
                {
                    biometricEnabled = await PinService.IsBiometricEnabledAsync();
                }
                StateHasChanged();
            }

            // Small delay to ensure DOM is fully ready for focus
            await Task.Delay(100);
            await FocusInput(0);
        }
    }

    private void OnFocus(int index)
    {
        focusedIndex = index;
    }

    private async Task OnInput(ChangeEventArgs e, int index)
    {
        var value = e.Value?.ToString() ?? string.Empty;

        // Extract only digit characters
        var digits = new string(value.Where(char.IsDigit).ToArray());

        if (digits.Length == 0)
        {
            pinDigits[index] = string.Empty;
            return;
        }

        if (digits.Length > 1)
        {
            // Paste scenario: distribute digits across fields starting at current index
            for (int i = 0; i < digits.Length && index + i < pinDigits.Length; i++)
            {
                pinDigits[index + i] = digits[i].ToString();
            }

            var lastFilledIndex = Math.Min(index + digits.Length - 1, pinDigits.Length - 1);
            if (lastFilledIndex < 5)
            {
                await FocusInput(lastFilledIndex + 1);
            }
            else if (lastFilledIndex == 5)
            {
                await SubmitPin();
            }
        }
        else
        {
            // Single digit entry
            pinDigits[index] = digits;

            if (index < 5)
            {
                await FocusInput(index + 1);
            }
            else if (index == 5)
            {
                // Auto-submit when 6th digit is entered
                await SubmitPin();
            }
        }
    }

    private async Task OnKeyDown(KeyboardEventArgs e, int index)
    {
        if (e.Key == "Backspace" && string.IsNullOrEmpty(pinDigits[index]) && index > 0)
        {
            await FocusInput(index - 1);
        }
        else if (e.Key == "Enter" && CurrentPin.Length >= 4)
        {
            await SubmitPin();
        }
    }

    private async Task FocusInput(int index)
    {
        if (index >= 0 && index < inputRefs.Length)
        {
            focusedIndex = index;
            try
            {
                await inputRefs[index].FocusAsync();
            }
            catch
            {
                // Focus may fail on some platforms
            }
        }
    }

    private async Task SubmitPin()
    {
        var pin = CurrentPin;
        if (pin.Length < 4)
        {
            pinErrorMessage = "PIN must be at least 4 digits";
            return;
        }

        if (pin.Length > 6)
        {
            pinErrorMessage = "PIN must be at most 6 digits";
            return;
        }

        isProcessing = true;
        pinErrorMessage = null;
        StateHasChanged();

        try
        {
            if (IsSetupMode)
            {
                if (HasLegacyData)
                {
                    await PinService.MigrateLegacyDataAsync(pin);
                }
                else
                {
                    await PinService.SetPinAsync(pin);
                }

                if (await BiometricService.IsAvailableAsync())
                {
                    await PinService.EnableBiometricAsync();
                }

                await OnUnlocked.InvokeAsync();
            }
            else
            {
                var isValid = await PinService.ValidatePinAsync(pin);
                if (isValid)
                {
                    if (await BiometricService.IsAvailableAsync())
                    {
                        await PinService.EnableBiometricAsync();
                    }

                    await OnUnlocked.InvokeAsync();
                }
                else
                {
                    pinErrorMessage = "Incorrect PIN. Please try again.";
                    ClearPin();
                }
            }
        }
        catch (Exception ex)
        {
            pinErrorMessage = $"An error occurred: {ex.Message}";
            ClearPin();
        }
        finally
        {
            isProcessing = false;
            StateHasChanged();
        }
    }

    private async Task AuthenticateWithBiometric()
    {
        isProcessing = true;
        biometricErrorMessage = null;
        StateHasChanged();

        try
        {
            var success = await PinService.ValidateBiometricAsync();
            if (success)
            {
                await OnUnlocked.InvokeAsync();
            }
            else
            {
                biometricErrorMessage = "Fingerprint authentication failed. Please use your PIN.";
            }
        }
        catch (Exception ex)
        {
            biometricErrorMessage = $"An error occurred: {ex.Message}";
        }
        finally
        {
            isProcessing = false;
            StateHasChanged();
        }
    }

    private void ClearPin()
    {
        for (int i = 0; i < pinDigits.Length; i++)
        {
            pinDigits[i] = string.Empty;
        }
        _ = FocusInput(0);
    }

    private async Task ImportBackup()
    {
        ShowImportBackupAlert = true;
    }

    protected virtual async Task OnImportBackupAlertClosed((McmAlert.AlertAction action, string stateInformation) result)
    {
        ShowImportBackupAlert = false;
        if (result.action == McmAlert.AlertAction.Confirm)
        {
            try
            {
                isProcessing = true;
                var fileContent = await ImportService.SelectFile();
                if (fileContent != null)
                {
                    IImportService.XmlImportResult? importResult;
                    var xml = await ImportService.LoadXmlFromContentAsync(fileContent!);
                    if (xml != null)
                    {
                        importResult = await ImportService.ImportXml(xml);
                        isProcessing = false;
                        if (importResult == IImportService.XmlImportResult.Success)
                        {
                            ImportMessage = "Backup imported successfully! You can now log in using the PIN from the backed up system.";
                            ShowImportMessageAlert = true;
                            IsSetupMode = false;
                        }
                        else if (importResult == IImportService.XmlImportResult.InvalidVersion)
                        {
                            ImportMessage = "Failed to import backup. The backup file version is not compatible with this app version.";
                            ShowImportMessageAlert = true;
                        }
                        else if (importResult == IImportService.XmlImportResult.InvalidVersion)
                        {
                            ImportMessage = "This xml file is not appear to be a version that can be imported.";
                            ShowImportMessageAlert = true;
                        }
                        else
                        {
                            ImportMessage = "Failed to import backup. The structure of the XML file is invalid.";
                            ShowImportMessageAlert = true;
                        }
                    }
                    else
                    {
                        ImportMessage = "Failed to read the backup file, it does not appear to be a valid xml file.";
                        ShowImportMessageAlert = true;
                    }
                }
            }
            catch (Exception ex)
            {
                ImportMessage = $"An error occurred during import: {ex.Message}";
                ShowImportMessageAlert = true;
            }
            finally
            {
                isProcessing = false;
                StateHasChanged();
            }
        }
    }

    protected virtual async Task OnImportAlertClosed((McmAlert.AlertAction action, string stateInformation) result)
    {
        ShowImportMessageAlert = false;
    }
}