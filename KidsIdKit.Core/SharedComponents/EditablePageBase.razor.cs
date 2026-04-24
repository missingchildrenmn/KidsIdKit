using Microsoft.AspNetCore.Components.Forms;
using System.Text.Json;

namespace KidsIdKit.Core.SharedComponents;

public abstract partial class EditablePageBase<T>: PageBase where T : class
{
    protected bool ShowPendingChangesAlert = false;
    protected bool CannotSaveChangesAlert = false;
    protected EditContext? EditContext { get; set; }
    protected string? originalSnapshot;
    protected T? EditingObject { get; set; }

    protected override async Task OnBackButtonClicked()
    {
        if (!HasPendingChanges())
        {
            RemoveAnyEmptyObjects();
            await NavigateBack();
            return;
        }

        ShowPendingChangesAlert = true;
    }

    protected virtual async Task OnPendingChangesAlertClosed((McmAlert.AlertAction action, string stateInformation) result)
    {
        ShowPendingChangesAlert = false;

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
        bool isValid = false;
        if (EditContext == null)
            isValid = true;
        else if (EditContext.Validate())
        {
            isValid = true;
        }
        CannotSaveChangesAlert = true;
        return isValid;
    }

    protected virtual async Task OnCannotSaveAlertClosed((McmAlert.AlertAction action, string stateInformation) result)
    {
        CannotSaveChangesAlert = false;
    }

    protected virtual void RestoreOriginalObject()
    {
        if (string.IsNullOrWhiteSpace(originalSnapshot))
        {
            return;
        }

        var unalteredObject = JsonSerializer.Deserialize<T>(originalSnapshot);
        if (unalteredObject == null)
        {
            return;
        }

        EditingObject = ResetUnalteredObject(unalteredObject);

        if (EditingObject != null)
        {
            originalSnapshot = SerializeObject(EditingObject);
        }
    }


    protected virtual bool HasPendingChanges() =>
        EditingObject != null &&
        !string.IsNullOrWhiteSpace(originalSnapshot) &&
        SerializeObject(EditingObject) != originalSnapshot;

    protected virtual void RemoveAnyEmptyObjects() { }

    protected abstract T ResetUnalteredObject(T unalteredObject);

    protected abstract Task SaveData();

    public void SetEditContext(EditContext context)
    {
        EditContext = context;
    }

    protected static string SerializeObject(T targetObject) => JsonSerializer.Serialize(targetObject);
}
