using Microsoft.AspNetCore.Components;

namespace KidsIdKit.Core.SharedComponents;

public partial class ProtectedLayout
{
    private bool isLoading = true;
    private bool isSetupMode;
    private bool hasLegacyData;

    protected override async Task OnInitializedAsync()
    {
        SessionService.OnLockStateChanged += OnLockStateChanged;

        // Check if PIN is set up
        var isPinSet = await PinService.IsPinSetAsync();
        hasLegacyData = await PinService.HasLegacyDataAsync();

        isSetupMode = !isPinSet;
        isLoading = false;
    }

    private void OnLockStateChanged()
    {
        InvokeAsync(StateHasChanged);
    }

    private async Task OnUnlocked()
    {
        // Refresh the state
        isSetupMode = false;
        hasLegacyData = false;
        await InvokeAsync(StateHasChanged);
    }

    private void OnSkipToInfo()
    {
        SessionService.EnableInfoOnlyMode();
        NavigationManager.NavigateTo("/Information");
    }

    public void Dispose()
    {
        SessionService.OnLockStateChanged -= OnLockStateChanged;
    }
}
