using System.Text.Json;
using KidsIdKit.Core.SharedComponents;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace KidsIdKit.Core.Pages.Child;

public partial class ChildPhysicalDetails
{
    [Parameter]
    public int Id { get; set; }
    public override string MenuBarTitle { get; protected set; } = "Physical Details";

    private Data.ChildDetails? CurrentChild;
    private Data.PhysicalDetails? Details;
    private EditContext? EditContext;
    private bool ShowPendingChangesAlert;
    private string? originalPhysicalDetailsSnapshot;
    private int? snapshotChildId;

    protected override void OnParametersSet()
    {
        var child = FamilyState.GetChild(Id);
        if (child == null)
        {
            CurrentChild = null;
            Details = null;
            originalPhysicalDetailsSnapshot = null;
            snapshotChildId = null;
            ShowPendingChangesAlert = false;
            return;
        }

        CurrentChild = child.ChildDetails;
        Details = child.PhysicalDetails;

        if (snapshotChildId != Id && Details != null)
        {
            originalPhysicalDetailsSnapshot = SerializePhysicalDetails(Details);
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

        RestoreOriginalPhysicalDetails();
        await NavigateBack();
    }

    private bool HasPendingChanges() =>
        Details != null &&
        !string.IsNullOrWhiteSpace(originalPhysicalDetailsSnapshot) &&
        SerializePhysicalDetails(Details) != originalPhysicalDetailsSnapshot;

    private void RestoreOriginalPhysicalDetails()
    {
        if (string.IsNullOrWhiteSpace(originalPhysicalDetailsSnapshot))
        {
            return;
        }

        var originalPhysicalDetails = JsonSerializer.Deserialize<Data.PhysicalDetails>(originalPhysicalDetailsSnapshot);
        if (originalPhysicalDetails == null)
        {
            return;
        }

        var child = FamilyState.GetChild(Id);
        if (child == null)
        {
            Details = originalPhysicalDetails;
            return;
        }

        child.PhysicalDetails = originalPhysicalDetails;
        Details = child.PhysicalDetails;

        // If this is a newly created child (empty GivenName) and it's still in the collection,
        // remove it since the user is discarding changes without entering a name
        if (string.IsNullOrWhiteSpace(child.ChildDetails.GivenName) && FamilyState.Family != null)
        {
            FamilyState.Family.Children.Remove(child);
        }

        originalPhysicalDetailsSnapshot = SerializePhysicalDetails(Details);
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

    private static string SerializePhysicalDetails(Data.PhysicalDetails physicalDetails) => JsonSerializer.Serialize(physicalDetails);
}
