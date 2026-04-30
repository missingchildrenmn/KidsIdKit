using KidsIdKit.Core.SharedComponents;

namespace KidsIdKit.Core.Pages;

public partial class Settings
{
    private string EnableBiometricsState = "EnableBiometrics";
    private string IsBiometricAvailableState = "IsBiometricAvailable";
    private string ShowBiometricWarningAlertState = "ShowBiometricWarningAlert";

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
        PageState.InitStateItem<bool>(ShowBiometricWarningAlertState, false);
    }

    private async Task OnEnableBiometricsChanged(bool value)
    {
        PageState.SetStateItem<bool>(EnableBiometricsState, value);

        if (value)
        {
            PageState.SetStateItem<bool>(ShowBiometricWarningAlertState, true);
        }
        else
        {
            await PinService.DisableBiometricAsync();
        }
    }

    private async Task OnBiometricWarningAlertClosed((McmAlert.AlertAction action, string stateInformation) result)
    {
        PageState.SetStateItem<bool>(ShowBiometricWarningAlertState, false);

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
}
