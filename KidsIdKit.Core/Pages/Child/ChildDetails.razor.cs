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

    protected override void OnParametersSet()
    {
        var child = FamilyState.GetChild(Id);
        EditingObject = child?.ChildDetails;
        MenuBarTitle = GetMenuBarTitle();

        if (EditingObject == null)
        {
            originalSnapshot = null;
            snapshotChildId = null;
            ShowPendingChangesAlert = false;
            return;
        }

        if (snapshotChildId != Id)
        {
            originalSnapshot = SerializeObject(EditingObject);
            snapshotChildId = Id;
            ShowPendingChangesAlert = false;
        }
    }

    private void RestoreOriginalChildDetails()
    {
        if (string.IsNullOrWhiteSpace(originalSnapshot))
        {
            return;
        }

        var originalChildDetails = JsonSerializer.Deserialize<Data.ChildDetails>(originalSnapshot);
        if (originalChildDetails == null)
        {
            return;
        }

        var child = FamilyState.GetChild(Id);
        if (child == null)
        {
            EditingObject = originalChildDetails;
        }
        else
        {
            child.ChildDetails = originalChildDetails;
            EditingObject = child.ChildDetails;

            // If this is a newly created child (empty GivenName) and it's still in the collection,
            // remove it since the user is discarding changes without entering a name
            if (string.IsNullOrWhiteSpace(child.ChildDetails.GivenName) && FamilyState.Family != null)
            {
                FamilyState.Family.Children.Remove(child);
            }
        }

        originalSnapshot = SerializeObject(EditingObject);
        MenuBarTitle = GetMenuBarTitle();
        SelectingImage = false;
    }

    protected override void RemoveAnyEmptyObjects()
    {
        if (EditingObject == null || !string.IsNullOrWhiteSpace(EditingObject!.GivenName) || FamilyState.Family == null)
        {
            return;
        }

        var child = FamilyState.GetChild(Id);
        if (child != null && string.IsNullOrWhiteSpace(child.ChildDetails.GivenName))
        {
            FamilyState.Family.Children.Remove(child);
        }
    }

    private string GetMenuBarTitle() =>
        EditingObject == null ? PageTitle : string.IsNullOrWhiteSpace(EditingObject!.GivenName) ? "New Child" : PageTitle;

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
