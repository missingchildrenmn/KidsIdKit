using System.Text.Json;
using KidsIdKit.Core.SharedComponents;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace KidsIdKit.Core.Pages.Child;

public partial class ChildMedicalNotes
{
    [Parameter]
    public int Id { get; set; }
    public override string MenuBarTitle { get; protected set; } = "Medical Notes";

    private Data.ChildDetails? CurrentChild;
    private Data.MedicalNotes? MedicalNotes;
    private EditContext? EditContext;
    private bool ShowPendingChangesAlert;
    private string? originalMedicalNotesSnapshot;
    private int? snapshotChildId;

    protected override void OnParametersSet()
    {
        var child = FamilyState.GetChild(Id);
        if (child == null)
        {
            CurrentChild = null;
            MedicalNotes = null;
            originalMedicalNotesSnapshot = null;
            snapshotChildId = null;
            ShowPendingChangesAlert = false;
            return;
        }

        CurrentChild = child.ChildDetails;
        MedicalNotes = child.MedicalNotes;

        if (snapshotChildId != Id && MedicalNotes != null)
        {
            originalMedicalNotesSnapshot = SerializeMedicalNotes(MedicalNotes);
            snapshotChildId = Id;
            ShowPendingChangesAlert = false;
        }
    }

    protected override async Task OnBackButtonClicked()
    {
        if (!HasPendingChanges())
        {
            RemoveEmptyNewChild();
            await NavigateBack();
            return;
        }

        ShowPendingChangesAlert = true;
    }

    private async Task SaveData() => await InternalSaveData();

    private async Task OnPendingChangesAlertClosed((McmAlert.AlertAction action, string stateInformation) result)
    {
        ShowPendingChangesAlert = false;

        if (result.action == McmAlert.AlertAction.Confirm)
        {
            // Validate before saving
            if (EditContext != null && EditContext.Validate())
            {
                await SaveData();
            }
            else
            {
                // Validation failed, show the alert again
                ShowPendingChangesAlert = true;
            }
            return;
        }

        RestoreOriginalMedicalNotes();
        await NavigateBack();
    }

    private bool HasPendingChanges() =>
        MedicalNotes != null &&
        !string.IsNullOrWhiteSpace(originalMedicalNotesSnapshot) &&
        SerializeMedicalNotes(MedicalNotes) != originalMedicalNotesSnapshot;

    private void RestoreOriginalMedicalNotes()
    {
        if (string.IsNullOrWhiteSpace(originalMedicalNotesSnapshot))
        {
            return;
        }

        var originalMedicalNotes = JsonSerializer.Deserialize<Data.MedicalNotes>(originalMedicalNotesSnapshot);
        if (originalMedicalNotes == null)
        {
            return;
        }

        var child = FamilyState.GetChild(Id);
        if (child == null)
        {
            MedicalNotes = originalMedicalNotes;
            return;
        }

        child.MedicalNotes = originalMedicalNotes;
        MedicalNotes = child.MedicalNotes;

        // If this is a newly created child (empty GivenName) and it's still in the collection,
        // remove it since the user is discarding changes without entering a name
        if (string.IsNullOrWhiteSpace(child.ChildDetails.GivenName) && FamilyState.Family != null)
        {
            FamilyState.Family.Children.Remove(child);
        }

        originalMedicalNotesSnapshot = SerializeMedicalNotes(MedicalNotes);
    }

    private void RemoveEmptyNewChild()
    {
        var child = FamilyState.GetChild(Id);
        if (child != null && string.IsNullOrWhiteSpace(child.ChildDetails.GivenName) && FamilyState.Family != null)
        {
            FamilyState.Family.Children.Remove(child);
        }
    }

    public void SetEditContext(EditContext context)
    {
        EditContext = context;
    }

    private static string SerializeMedicalNotes(Data.MedicalNotes medicalNotes) => JsonSerializer.Serialize(medicalNotes);
}
