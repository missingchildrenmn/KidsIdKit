using KidsIdKit.Core.Data;
using KidsIdKit.Core.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace KidsIdKit.Core.SharedComponents;

public abstract partial class DetailsPage
{
    [Inject]
    protected NavigationManager NavigationManager { get; set; } = default!;
    [Inject]
    protected IFamilyStateService FamilyState { get; set; } = default!;

    protected string? messageText;
    protected bool isError;

    protected virtual async Task InternalSaveData()
    {
        if (FamilyState.Family == null)
        {
            isError = true;
            messageText = "No data to save.";
            return;
        }

        messageText = string.Empty;
        isError = false;

        try
        {
            await FamilyState.SaveAsync();
            await JSRuntime.InvokeVoidAsync("history.back");
        }
        catch (DataAccessException ex)
        {
            isError = true;
            messageText = ex.Message;
        }
        catch (Exception ex)
        {
            isError = true;
            messageText = $"An unexpected error occurred: {ex.Message}";
        }
    }
}
