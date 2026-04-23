using System.Text.Json;
using KidsIdKit.Core.SharedComponents;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace KidsIdKit.Core.Pages.Child;

public partial class ChildDetails
{
    [Parameter] public int Id { get; set; }

    private Data.ChildDetails? CurrentChild { get; set; }
    private EditContext? EditContext { get; set; }

    public override string MenuBarTitle { get; protected set; } = string.Empty;

    private readonly string PageTitle = "Child Details";
    private bool SelectingImage;
    private bool ShowPendingChangesAlert;
    private string? originalChildSnapshot;
    private int? snapshotChildId;

    protected override void OnParametersSet()
    {
        var child = FamilyState.GetChild(Id);
        CurrentChild = child?.ChildDetails;
        MenuBarTitle = GetMenuBarTitle();

        if (CurrentChild == null)
        {
            originalChildSnapshot = null;
            snapshotChildId = null;
            ShowPendingChangesAlert = false;
            return;
        }

        if (snapshotChildId != Id)
        {
            originalChildSnapshot = SerializeChildDetails(CurrentChild);
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

        RestoreOriginalChildDetails();
        await NavigateBack();
    }

    private bool HasPendingChanges() =>
        CurrentChild != null &&
        !string.IsNullOrWhiteSpace(originalChildSnapshot) &&
        SerializeChildDetails(CurrentChild) != originalChildSnapshot;

    private void RestoreOriginalChildDetails()
    {
        if (string.IsNullOrWhiteSpace(originalChildSnapshot))
        {
            return;
        }

        var originalChildDetails = JsonSerializer.Deserialize<Data.ChildDetails>(originalChildSnapshot);
        if (originalChildDetails == null)
        {
            return;
        }

        var child = FamilyState.GetChild(Id);
        if (child == null)
        {
            CurrentChild = originalChildDetails;
        }
        else
        {
            child.ChildDetails = originalChildDetails;
            CurrentChild = child.ChildDetails;

            // If this is a newly created child (empty GivenName) and it's still in the collection,
            // remove it since the user is discarding changes without entering a name
            if (string.IsNullOrWhiteSpace(child.ChildDetails.GivenName) && FamilyState.Family != null)
            {
                FamilyState.Family.Children.Remove(child);
            }
        }

        originalChildSnapshot = SerializeChildDetails(CurrentChild);
        MenuBarTitle = GetMenuBarTitle();
        SelectingImage = false;
    }

    private void RemoveEmptyNewChild()
    {
        if (CurrentChild == null || !string.IsNullOrWhiteSpace(CurrentChild.GivenName) || FamilyState.Family == null)
        {
            return;
        }

        var child = FamilyState.GetChild(Id);
        if (child != null && string.IsNullOrWhiteSpace(child.ChildDetails.GivenName))
        {
            FamilyState.Family.Children.Remove(child);
        }
    }

    public void SetEditContext(EditContext context)
    {
        EditContext = context;
    }

    private string GetMenuBarTitle() =>
        CurrentChild == null ? PageTitle : string.IsNullOrWhiteSpace(CurrentChild.GivenName) ? "New Child" : PageTitle;

    private static string SerializeChildDetails(Data.ChildDetails childDetails) => JsonSerializer.Serialize(childDetails);
}
