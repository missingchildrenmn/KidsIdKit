using System.Text.Json;
using KidsIdKit.Core.Data;
using KidsIdKit.Core.SharedComponents;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace KidsIdKit.Core.Pages.Child;

public partial class ChildMedicalNotes : DetailsPage<Data.MedicalNotes>
{
    [Parameter]
    public int Id { get; set; }
    public override string MenuBarTitle { get; protected set; } = "Medical Notes";

    private Data.ChildDetails? CurrentChild;

    private int? snapshotChildId;

    protected override void OnParametersSet()
    {
        var child = FamilyState.GetChild(Id);
        if (child == null)
        {
            CurrentChild = null;
            EditingObject = null;
            originalSnapshot = null;
            snapshotChildId = null;
            ShowPendingChangesAlert = false;
            return;
        }

        CurrentChild = child.ChildDetails;
        EditingObject = child.MedicalNotes;

        if (snapshotChildId != Id && EditingObject != null)
        {
            originalSnapshot = SerializeObject(EditingObject);
            snapshotChildId = Id;
            ShowPendingChangesAlert = false;
        }
    }

    protected override MedicalNotes ResetUnalteredObject(MedicalNotes unalteredObject)
    {
        var child = FamilyState.GetChild(Id);
        if (child == null)
        {
            return unalteredObject;
        }

        child.MedicalNotes = unalteredObject;

        // If this is a newly created child (empty GivenName) and it's still in the collection,
        // remove it since the user is discarding changes without entering a name
        if (string.IsNullOrWhiteSpace(child.ChildDetails.GivenName) && FamilyState.Family != null)
        {
            FamilyState.Family.Children.Remove(child);
        }

        return child.MedicalNotes;
    }

    protected override async Task SaveData()
    {
        if (ValidateChangesForSave())
        {
            await InternalSaveData();
        }
    }
}
