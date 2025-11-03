using KidsIdKit.Shared.Data;
using Microsoft.AspNetCore.Components;

namespace KidsIdKit.Shared.Pages.DistinguishingFeatures;

public partial class ChildDistinguishingFeatureDetails
{
    [Parameter] public int ChildId { get; set; }
    [Parameter] public int FeatureId { get; set; }

    readonly string PageTitle = "Distinguishing Feature (birthmark, scar, etc.)";
    KidsIdKit.Shared.Data.ChildDetails? CurrentChild;
    DistinguishingFeature? DistinguishingFeature;
    private bool SelectingImage;
    private string? messageText;

    protected override void OnInitialized()
    {
        ArgumentNullException.ThrowIfNull(DataStore.Family);
        if (ChildId >= 0 && ChildId < DataStore.Family.Children.Count)
        {
            CurrentChild = DataStore.Family.Children[ChildId].ChildDetails;

            if (FeatureId == -1)
            {
                DistinguishingFeature = new DistinguishingFeature();
                if (DataStore.Family.Children[ChildId].DistinguishingFeatures.Count == 0)
                {
                    DistinguishingFeature.Id = 0;
                }
                else
                {
                    DistinguishingFeature.Id = DataStore.Family.Children[ChildId].DistinguishingFeatures.Max(r => r.Id) + 1;
                }
            }
            else if (FeatureId >= 0 && FeatureId < DataStore.Family.Children[ChildId].DistinguishingFeatures.Count)
            {
                DistinguishingFeature = DataStore.Family.Children[ChildId].DistinguishingFeatures[FeatureId];
            }
        }
    }

    private async Task SaveData()
    {
        messageText = string.Empty;
        try
        {
            if (DataStore.Family is not null && DistinguishingFeature is not null)
            {
                if (FeatureId == -1)
                {
                    DataStore.Family.Children[ChildId].DistinguishingFeatures.Add(DistinguishingFeature);
                }
                await dal.SaveDataAsync(DataStore.Family);
            }
            NavigationManager.NavigateTo($"/childDistinguishingFeatures/{ChildId}");
        }
        catch (Exception e)
        {
            messageText = e.Message;
        }
    }
}