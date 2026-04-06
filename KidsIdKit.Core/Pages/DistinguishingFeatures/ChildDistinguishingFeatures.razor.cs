using KidsIdKit.Core.Data;
using KidsIdKit.Core.SharedComponents;
using Microsoft.AspNetCore.Components;

namespace KidsIdKit.Core.Pages.DistinguishingFeatures;

public partial class ChildDistinguishingFeatures
{
    [Parameter] public int Id { get; set; }

    ChildDetails? CurrentChild { get; set; }
    IQueryable<DistinguishingFeature>? Features { get; set; }

    public override string MenuBarTitle { get; protected set; } = "Distinguishing Features";

    private bool AlertShow = false;
    private string AlertTitle = string.Empty;
    private string AlertMessage = "Are you sure you want to remove this distinguishing feature?";
    private string AlertStateInformation = string.Empty;

    protected override void OnParametersSet()
    {
        var child = FamilyState.GetChild(Id);
        if (child != null)
        {
            CurrentChild = child.ChildDetails;
            Features = child.DistinguishingFeatures.AsQueryable();
        }
    }

    private void NavigateToEdit(DistinguishingFeature feature)
    {
        NavigationManager.NavigateTo($"/Feature/{Id}/{feature.Id}");
    }

    public async Task DeleteResponse(string stateInformation, McmAlert.AlertAction result)
    {
        AlertShow = false;
        int distinguishingFeatureId = int.Parse(stateInformation);
        if (result == McmAlert.AlertAction.Confirm)
        {
            var child = FamilyState.GetChild(Id);
            if (child != null && distinguishingFeatureId >= 0)
            {
                var distinguishingFeature = child.DistinguishingFeatures.FirstOrDefault((p) => p.Id == distinguishingFeatureId);
                if (distinguishingFeature is not null)
                {
                    child.DistinguishingFeatures.Remove(distinguishingFeature);
                    await FamilyState.SaveAsync();
                    Features = child.DistinguishingFeatures.AsQueryable();
                }
            }
        }
    }

    public void ShowAlert(DistinguishingFeature distinguishingFeature)
    {
        AlertTitle = $"Remove {distinguishingFeature.Description} ?";
        AlertStateInformation = distinguishingFeature.Id.ToString();
        AlertShow = true;
    }
}
