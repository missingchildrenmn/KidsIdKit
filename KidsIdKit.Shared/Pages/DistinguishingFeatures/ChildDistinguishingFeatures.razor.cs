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

    private void NavigateToEdit(DistinguishingFeature feature)
    {
        NavigationManager.NavigateTo($"/Feature/{Id}/{feature.Id}");
    }

    //private void NavigateToEdit(Data.Child child)
    //{
    //    if (DataStore.Family != null)
    //    {
    //        var index = DataStore.Family.Children.IndexOf(child);
    //        //NavigationManager.NavigateTo($"/Child/{index}");
    //        NavigationManager.NavigateTo($"/Feature/@Id/@context.Id");
    //    }
    //}
}

//<a href = "/Feature/@Id/@context.Id" style="text-decoration:none"><i class="fas fa-edit"></i></a>
