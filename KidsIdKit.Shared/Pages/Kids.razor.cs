using KidsIdKit.Data;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace KidsIdKit.Shared.Pages;

public partial class Kids
{
    private IQueryable<KidsIdKit.Data.Child>? data;
    private DateTime LastUpdatedDateTime;

    protected override async Task OnInitializedAsync()
    {
        DataStore.Family = await dal.GetDataAsync();
        if (DataStore.Family is not null)
        {
            data = DataStore.Family.Children.AsQueryable();
        }

        var lastUpdated = await dal.GetLastUpdatedDateTimeAsync();
        LastUpdatedDateTime = lastUpdated ?? DateTime.MinValue;
//        StateHasChanged();
    }

    private void NavigateToEdit(KidsIdKit.Data.Child child)
    {
        if (DataStore.Family != null)
        {
            var index = DataStore.Family.Children.IndexOf(child);
            NavigationManager.NavigateTo($"/Child/{index}");
        }
    }

    private void NavigateToRemove(KidsIdKit.Data.Child child, MouseEventArgs e)
    {
        if (DataStore.Family != null)
        {
            var index = DataStore.Family.Children.IndexOf(child);
            NavigationManager.NavigateTo($"/ChildRemove/{index}");
        }
    }
}