using Microsoft.JSInterop;

namespace KidsIdKit.Core.SharedComponents;

public abstract partial class PageBase
{
    public abstract string MenuBarTitle { get; protected set; }

    protected virtual async Task OnBackButtonClicked()
    {
        await NavigateBack();
    }

    protected async Task NavigateBack()
    {
        await JSRuntime.InvokeVoidAsync("history.back");
    }
}
