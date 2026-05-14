using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using System.Text.Json;

namespace KidsIdKit.Core.SharedComponents;

public abstract partial class EditablePageBase<T>: PageBase where T : class
{
    protected const string ShowPendingChangesAlertState = "ShowPendingChangesAlert";
    protected const string CannotSaveChangesAlertState = "CannotSaveChangesAlert";
    protected const string EditContextState = "EditContext";
    protected const string OriginalSnapshotState = "OriginalSnapshot";
    protected const string EditingObjectState = "EditingObject";
    
    protected bool ShowBusyIndicator = false;
    protected string BusyMessage = string.Empty;

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();

        PageState.InitStateItem<bool>(ShowPendingChangesAlertState, false);
        PageState.InitStateItem<bool>(CannotSaveChangesAlertState, false);
        PageState.InitStateItem<EditContext?>(EditContextState, null);
    }

    protected override async Task OnBackButtonClicked()
    {
        if (!HasPendingChanges())
        {
            RemoveAnyEmptyObjects();
            await NavigateBack();
            return;
        }

        PageState.SetStateItem<bool>(ShowPendingChangesAlertState, true);
    }

    protected virtual async Task OnPendingChangesAlertClosed((McmAlert.AlertAction action, string stateInformation) result)
    {
        PageState.SetStateItem<bool>(ShowPendingChangesAlertState, false);

        if (result.action == McmAlert.AlertAction.Confirm)
        {
            await SaveData();
            return;
        }

        RestoreOriginalObject();
        await NavigateBack();
    }

    protected virtual bool ValidateChangesForSave()
    {
        EditContext? editContext = PageState.GetStateItem<EditContext?>(EditContextState).Value;
        bool isValid = editContext == null || editContext.Validate() ? true : false;

        if (!isValid)
        {
            PageState.SetStateItem<bool>(CannotSaveChangesAlertState, true);
        }
        return isValid;
    }

    protected virtual async Task OnCannotSaveAlertClosed((McmAlert.AlertAction action, string stateInformation) result)
    {
        PageState.SetStateItem<bool>(CannotSaveChangesAlertState, false);
    }

    protected virtual void RestoreOriginalObject()
    {
        string? originalSnapshot = PageState.GetStateItem<string?>(OriginalSnapshotState).Value;
        if (string.IsNullOrWhiteSpace(originalSnapshot))
        {
            return;
        }

        var unalteredObject = JsonSerializer.Deserialize<T>(originalSnapshot);
        if (unalteredObject == null)
        {
            return;
        }

        var resetObject = ResetUnalteredObject(unalteredObject);
        PageState.SetStateItem<T?>(EditingObjectState, resetObject);

        if (resetObject != null)
        {
            PageState.SetStateItem<string?>(OriginalSnapshotState, SerializeObject(resetObject));
        }
    }


    protected virtual bool HasPendingChanges()
    {
        var editingObject = PageState.GetStateItem<T?>(EditingObjectState).Value;
        var originalSnapshot = PageState.GetStateItem<string?>(OriginalSnapshotState).Value;

        return editingObject != null &&
        !string.IsNullOrWhiteSpace(originalSnapshot) &&
        SerializeObject(editingObject) != originalSnapshot;
    }

    protected virtual void RemoveAnyEmptyObjects() { }

    protected abstract T ResetUnalteredObject(T unalteredObject);

    protected abstract Task SaveData();

    public void SetEditContext(EditContext context)
    {
        PageState.SetStateItem<EditContext?>(EditContextState, context);
    }

    protected static string SerializeObject(T targetObject) => JsonSerializer.Serialize(targetObject);
}
