using KidsIdKit.Core.SharedComponents;
using static iText.Svg.SvgConstants;

namespace KidsIdKit.Core.Pages;

public partial class Settings
{
    private string EnableBiometricsState = "EnableBiometrics";
    private string IsBiometricAvailableState = "IsBiometricAvailable";
    private string EnableCloudBackupsState = "EnableCloudBackups";
    private string IsCloudBackupSupportedState = "IsCloudBackupSupported";

    private const string AlertShowState = "AlertShow";
    private const string AlertTitleState = "AlertTitle";
    private const string AlertStateInformationState = "AlertStateInformation";
    private const string AlertMessageState = "AlertMessage";

    private const string StateInformationBiometricWarning = "BiometricWarning";
    private const string StateInformationCloudBackupWarning = "CloudBackupWarning";

    protected override async Task OnInitializedAsync()
    {
        if (!PageState.AppSuspended)
        {
            PageState.ClearStateItems();
        }
        PageState.AppSuspended = false;
        
        var isBiometricAvailable = await BiometricService.IsAvailableAsync();
        PageState.InitStateItem<bool>(IsBiometricAvailableState, isBiometricAvailable);
        if (isBiometricAvailable)
        {
            PageState.InitStateItem<bool>(EnableBiometricsState, await PinService.IsBiometricEnabledAsync());
        }
        else
        {
            PageState.InitStateItem<bool>(EnableBiometricsState, false);
        }

        var isCloudBackupSupported = CloudBackupService.IsCloudBackupSupported();
        PageState.InitStateItem<bool>(IsCloudBackupSupportedState, isCloudBackupSupported);
        if (isCloudBackupSupported)
        {
            PageState.InitStateItem<bool>(EnableCloudBackupsState, await CloudBackupService.IsCloudBackupEnabledAsync());
        }
        else
        {
            PageState.InitStateItem<bool>(EnableCloudBackupsState, false);
        }

        PageState.InitStateItem<string>(AlertStateInformationState, string.Empty);
        PageState.InitStateItem<string>(AlertTitleState, string.Empty);
        PageState.InitStateItem<string>(AlertMessageState, string.Empty);
        PageState.InitStateItem<bool>(AlertShowState, false);
    }

    private async Task OnEnableBiometricsChanged(bool value)
    {
        PageState.SetStateItem<bool>(EnableBiometricsState, value);

        if (value)
        {
            PageState.SetStateItem<string>(AlertTitleState, "Warning!");
            PageState.SetStateItem<string>(AlertMessageState, "If you enable biometrics, anyone with biometric access to this device will be able to view application data without knowing the application PIN. Are you sure you want to continue?");
            PageState.SetStateItem<string>(AlertStateInformationState, StateInformationBiometricWarning);
            PageState.SetStateItem<bool>(AlertShowState, true);
        }
        else
        {
            await PinService.DisableBiometricAsync();
        }
    }

    private async Task OnAlertClosed((McmAlert.AlertAction action, string stateInformation) result)
    {
        PageState.SetStateItem<bool>(AlertShowState, false);

        if (result.stateInformation == StateInformationBiometricWarning)
        {
            await OnBiometricWarningAlertClosed(result);
        }
        else if (result.stateInformation == StateInformationCloudBackupWarning)
        {
            await OnCloudBackupWarningAlertClosed(result);
        }
    }

    private async Task OnBiometricWarningAlertClosed((McmAlert.AlertAction action, string stateInformation) result)
    {
        if (result.action == McmAlert.AlertAction.Cancel)
        {
            PageState.SetStateItem<bool>(EnableBiometricsState, false);
        }
        else if (result.action == McmAlert.AlertAction.Confirm)
        {
            if (!await BiometricService.IsAvailableAsync())
            {
                PageState.SetStateItem<bool>(EnableBiometricsState, false);
                return;
            }

            try
            {
                await PinService.EnableBiometricAsync();
                PageState.SetStateItem<bool>(EnableBiometricsState, true);
            }
            catch
            {
                PageState.SetStateItem<bool>(EnableBiometricsState, false);
            }
        }
    }

    private async Task OnEnableCloudBackupsChanged(bool value)
    {
        PageState.SetStateItem<bool>(EnableCloudBackupsState, value);

        if (value)
        {
            PageState.SetStateItem<string>(AlertTitleState, "Warning!");
            PageState.SetStateItem<string>(AlertMessageState, $"Cloud backups will use standard device functionality to send your app data to {CloudBackupService.GetBackupLocation()}. While the data will still be encrypted, it will no longer only be stored on this device. Are you sure you want to continue?");
            PageState.SetStateItem<string>(AlertStateInformationState, StateInformationCloudBackupWarning);
            PageState.SetStateItem<bool>(AlertShowState, true);
        }
        else
        {
            await CloudBackupService.DisableCloudBackupAsync();
        }
    }

    private async Task OnCloudBackupWarningAlertClosed((McmAlert.AlertAction action, string stateInformation) result)
    {
        if (result.action == McmAlert.AlertAction.Cancel)
        {
            PageState.SetStateItem<bool>(EnableCloudBackupsState, false);
        }
        else if (result.action == McmAlert.AlertAction.Confirm)
        {
            try
            {
                await CloudBackupService.EnableCloudBackupAsync();
                PageState.SetStateItem<bool>(EnableCloudBackupsState, true);
            }
            catch
            {
                PageState.SetStateItem<bool>(EnableCloudBackupsState, false);
            }
        }
    }
}
