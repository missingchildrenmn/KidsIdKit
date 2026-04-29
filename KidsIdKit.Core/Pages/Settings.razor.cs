using KidsIdKit.Core.Services;
using KidsIdKit.Core.SharedComponents;

namespace KidsIdKit.Core.Pages;

public partial class Settings
{
    private bool EnableBiometrics { get; set; } = false;
    private bool DarkMode { get; set; } = false;
    private bool IsBiometricAvailable { get; set; } = false;
    private bool ShowBiometricWarningAlert { get; set; } = false;

    protected override async Task OnInitializedAsync()
    {
        IsBiometricAvailable = await BiometricService.IsAvailableAsync();
        if (IsBiometricAvailable)
        {
            EnableBiometrics = await PinService.IsBiometricEnabledAsync();
        }

    }

    private async Task OnEnableBiometricsChanged(bool value)
    {
        EnableBiometrics = value;

        if (EnableBiometrics)
        {
            ShowBiometricWarningAlert = true;
        }
        else
        {
            await PinService.DisableBiometricAsync();
        }
    }

    private async Task OnBiometricWarningAlertClosed((McmAlert.AlertAction action, string stateInformation) result)
    {
        ShowBiometricWarningAlert = false;

        if (result.action == McmAlert.AlertAction.Cancel)
        {
            EnableBiometrics = false;
        }
        else if (result.action == McmAlert.AlertAction.Confirm)
        {
            if (!await BiometricService.IsAvailableAsync())
            {
                EnableBiometrics = false;
                return;
            }

            try
            {
                await PinService.EnableBiometricAsync();
            }
            catch
            {
                EnableBiometrics = false;
            }
        }

    }
}
