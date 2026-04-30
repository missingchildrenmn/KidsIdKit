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

    protected override Task OnInitializedAsync()
    {
        var returnValue = base.OnInitializedAsync();
        PageState.InitStateItem(MessageTextState, string.Empty);
        PageState.InitStateItem(IsErrorState, false);
        return returnValue;

    }

    protected virtual async Task InternalSaveData()
    {
        if (FamilyState.Family == null)
        {
            PageState.SetStateItem(IsErrorState, true);
            PageState.SetStateItem(MessageTextState, "No data to save.");
            return;
        }

        PageState.SetStateItem(MessageTextState, string.Empty);
        PageState.SetStateItem(IsErrorState, false);

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
    }
}
