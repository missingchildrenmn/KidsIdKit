using KidsIdKit.Core.Data;
using KidsIdKit.Core.SharedComponents;
using Microsoft.AspNetCore.Components;

namespace KidsIdKit.Core.Pages.Child;

public partial class ChildMedicalNotes : DetailsPage<Data.MedicalNotes>
{
    [Parameter]
    public int Id { get; set; }
    public override string MenuBarTitle { get; protected set; } = "Medical Notes";

    private Data.ChildDetails? CurrentChild;

    private int? snapshotChildId;

    protected override Task OnInitializedAsync()
    {
        return base.OnInitializedAsync();
    }

    protected override void OnParametersSet()
    {
        var child = FamilyState.GetChild(Id);
        if (child == null)
        {
            CurrentChild = null;
            PageState.InitStateItem<Data.MedicalNotes?>(EditingObjectState, null);
            PageState.InitStateItem<string?>(OriginalSnapshotState, null);
            snapshotChildId = null;
            return;
        }

        CurrentChild = child.ChildDetails;
        PageState.InitStateItem<Data.MedicalNotes?>(EditingObjectState, child.MedicalNotes);

        if (snapshotChildId != Id && child.MedicalNotes != null)
        {
            PageState.InitStateItem<string?>(OriginalSnapshotState, SerializeObject(child.MedicalNotes));
            snapshotChildId = Id;
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
