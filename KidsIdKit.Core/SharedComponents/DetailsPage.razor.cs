using KidsIdKit.Core.Data;
using KidsIdKit.Core.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace KidsIdKit.Core.SharedComponents;

public abstract partial class DetailsPage<T> : EditablePageBase<T> where T : class
{
    [Inject]
    protected NavigationManager NavigationManager { get; set; } = default!;
    [Inject]
    protected IFamilyStateService FamilyState { get; set; } = default!;
    [Inject] 
    protected IJSRuntime JSRuntime { get; set; } = default!;

    protected const string MessageTextState = "MessageText";
    protected const string IsErrorState = "IsError";

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        PageState.InitStateItem<string?>(MessageTextState, string.Empty);
        PageState.InitStateItem(IsErrorState, false);
    }

    protected virtual async Task InternalSaveData()
    {
        BusyMessage = "Saving...";
        ShowBusyIndicator = true;
        await InvokeAsync(StateHasChanged);
        await Task.Run(async () => {
            if (FamilyState.Family == null)
            {
                PageState.SetStateItem(IsErrorState, true);
                PageState.SetStateItem(MessageTextState, "No data to save.");
                return;
            }

            PageState.SetStateItem<string?>(MessageTextState, string.Empty);
            PageState.SetStateItem<bool>(IsErrorState, false);

            try
            {
                await FamilyState.SaveAsync();
                await JSRuntime.InvokeVoidAsync("history.back");
            }
            catch (DataAccessException ex)
            {
                PageState.SetStateItem<bool>(IsErrorState, true);
                PageState.SetStateItem<string?>(MessageTextState, ex.Message);
            }
            catch (Exception ex)
            {
                PageState.SetStateItem<bool>(IsErrorState, true);
                PageState.SetStateItem<string?>(MessageTextState, $"An unexpected error occurred: {ex.Message}");
            }
        });
        ShowBusyIndicator = false;
        await InvokeAsync(StateHasChanged);
    }
}
