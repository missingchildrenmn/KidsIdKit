using KidsIdKit.Core.Data;
using KidsIdKit.Core.Services;
using Microsoft.AspNetCore.Components;

namespace KidsIdKit.Core.Pages.DistinguishingFeatures;

public partial class ChildDistinguishingFeatureDetails
{
    [Inject] private IFamilyStateService FamilyState { get; set; } = default!;
    [Inject] private NavigationManager NavigationManager { get; set; } = default!;

    [Parameter] public int ChildId { get; set; }
    [Parameter] public int FeatureId { get; set; }

    readonly string PageTitle = "Distinguishing Feature (birthmark, scar, etc.)";
    KidsIdKit.Core.Data.ChildDetails? CurrentChild;
    DistinguishingFeature? DistinguishingFeature;
    private bool SelectingImage;
    private string? messageText;

    protected override void OnInitialized()
    {
        var child = FamilyState.GetChild(ChildId);
        if (child != null)
        {
            CurrentChild = child.ChildDetails;

            if (FeatureId == -1)
            {
                DistinguishingFeature = new DistinguishingFeature();
                DistinguishingFeature.Id = child.DistinguishingFeatures.Count == 0
                    ? 0
                    : child.DistinguishingFeatures.Max(r => r.Id) + 1;
            }
            else if (FeatureId >= 0 && FeatureId < child.DistinguishingFeatures.Count)
            {
                DistinguishingFeature = child.DistinguishingFeatures[FeatureId];
            }
        }
    }

    private async Task SaveData()
    {
        messageText = string.Empty;
        try
        {
            var child = FamilyState.GetChild(ChildId);
            if (child != null && DistinguishingFeature is not null)
            {
                if (FeatureId == -1)
                {
                    child.DistinguishingFeatures.Add(DistinguishingFeature);
                }
                await FamilyState.SaveAsync();
            }
            NavigationManager.NavigateTo($"/childDistinguishingFeatures/{ChildId}");
        }
        catch (Exception e)
        {
            messageText = e.Message;
        }
    }
}
