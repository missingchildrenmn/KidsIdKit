using KidsIdKit.Core.Data;
using Microsoft.AspNetCore.Components;

namespace KidsIdKit.Core.Pages.DistinguishingFeatures;

public partial class ChildDistinguishingFeatures
{
    [Parameter] public int Id { get; set; }

    ChildDetails? CurrentChild { get; set; }
    IQueryable<DistinguishingFeature>? Features { get; set; }

    readonly string PageTitle = "Distinguishing Features";

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
}
