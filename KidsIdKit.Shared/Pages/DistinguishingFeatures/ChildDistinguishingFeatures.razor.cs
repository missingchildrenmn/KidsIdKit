using KidsIdKit.Shared.Data;
using Microsoft.AspNetCore.Components;

namespace KidsIdKit.Shared.Pages.DistinguishingFeatures;

public partial class ChildDistinguishingFeatures
{
    [Parameter] public int Id { get; set; }

    ChildDetails? CurrentChild { get; set; }
    IQueryable<DistinguishingFeature>? Features { get; set; }

    readonly string PageTitle = "Distinguishing Features";

    protected override void OnParametersSet()
    {
        ArgumentNullException.ThrowIfNull(DataStore.Family);
        CurrentChild = DataStore.Family.Children[Id].ChildDetails;
        Features = DataStore.Family.Children[Id].DistinguishingFeatures.AsQueryable();
    }
}