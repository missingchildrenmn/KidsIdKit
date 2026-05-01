using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace KidsIdKit.Core.SharedComponents;


public abstract partial class PageBase
{

    [Inject] protected IPageState PageState { get; set; } = default!;

    public abstract string MenuBarTitle { get; protected set; }

    protected override async Task OnInitializedAsync()
    {
        if (!PageState.AppSuspended)
        {
            PageState.ClearStateItems();
        }

        PageState.AppSuspended = false;

        await base.OnInitializedAsync();
    }

    protected virtual async Task OnBackButtonClicked()
    {
        await NavigateBack();
    }

    protected async Task NavigateBack()
    {
        await JSRuntime.InvokeVoidAsync("history.back");
    }
}
