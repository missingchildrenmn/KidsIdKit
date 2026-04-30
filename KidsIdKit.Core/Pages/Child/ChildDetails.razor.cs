using System.Text.Json;
using KidsIdKit.Core.SharedComponents;
using Microsoft.AspNetCore.Components;

namespace KidsIdKit.Core.Pages.Child;

public partial class ChildDetails: DetailsPage<Data.ChildDetails>
{
    [Parameter] public int Id { get; set; }

    public override string MenuBarTitle { get; protected set; } = string.Empty;

    private readonly string PageTitle = "Child Details";
    private bool SelectingImage;
    private int? snapshotChildId;

    protected override Task OnInitializedAsync()
    {
        return base.OnInitializedAsync();
    }

    protected override void OnParametersSet()
    {
        var child = FamilyState.GetChild(Id);
        PageState.InitStateItem(EditingObjectState, child?.ChildDetails);

        MenuBarTitle = GetMenuBarTitle();

        if (PageState.GetStateItem<Data.ChildDetails>(EditingObjectState).Value == null)
        {
            PageState.InitStateItem<string?>(OriginalSnapshotState, null);
            snapshotChildId = null;
            return;
        }

        if (snapshotChildId != Id)
        {
            var editingObject = PageState.GetStateItem<Data.ChildDetails>(EditingObjectState).Value;
            PageState.InitStateItem<string?>(OriginalSnapshotState, SerializeObject(editingObject));
            snapshotChildId = Id;
        }
    }

    protected override void RemoveAnyEmptyObjects()
    {
        var editingObject = PageState.GetStateItem<Data.ChildDetails>(EditingObjectState).Value;
        if (editingObject == null || !string.IsNullOrWhiteSpace(editingObject!.GivenName) || FamilyState.Family == null)
        {
            return;
        }

        var child = FamilyState.GetChild(Id);
        if (child != null && string.IsNullOrWhiteSpace(child.ChildDetails.GivenName))
        {
            FamilyState.Family.Children.Remove(child);
        }
    }

    private string GetMenuBarTitle()
    {
        var editingObject = PageState.GetStateItem<Data.ChildDetails>(EditingObjectState).Value;
        return editingObject == null ? PageTitle : string.IsNullOrWhiteSpace(editingObject!.GivenName) ? "New Child" : PageTitle;
    }

    protected override Data.ChildDetails ResetUnalteredObject(Data.ChildDetails unalteredObject)
    {
        var child = FamilyState.GetChild(Id);
        if (child == null)
        {
            return unalteredObject;
        }
        else
        {
            child.ChildDetails = unalteredObject;

            // If this is a newly created child (empty GivenName) and it's still in the collection,
            // remove it since the user is discarding changes without entering a name
            if (string.IsNullOrWhiteSpace(child.ChildDetails.GivenName) && FamilyState.Family != null)
            {
                FamilyState.Family.Children.Remove(child);
            }

            return child.ChildDetails;
        }
    }

    protected override async Task SaveData()
    {
        if (ValidateChangesForSave())
        {
            await InternalSaveData();
        }
    }
}
