using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

using KidsIdKit.Shared.Data;

namespace KidsIdKit.Shared.Pages;

public partial class Kids
{
    private IQueryable<Data.Child>? data;
    private DateTime LastUpdatedDateTime;                   // TODO: rename to '...Any...'

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

    private void NavigateToEdit(Data.Child child)
    {
        if (DataStore.Family != null)
        {
            var index = DataStore.Family.Children.IndexOf(child);
            NavigationManager.NavigateTo($"/Child/{index}");
        }
    }

    private void NavigateToRemove(Data.Child child, MouseEventArgs e)
    {
        if (DataStore.Family != null)
        {
            var index = DataStore.Family.Children.IndexOf(child);
            NavigationManager.NavigateTo($"/ChildRemove/{index}");
        }
    }
}
