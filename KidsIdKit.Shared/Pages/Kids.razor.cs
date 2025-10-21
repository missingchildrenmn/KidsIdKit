using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

using KidsIdKit.Shared.Data;

namespace KidsIdKit.Shared.Pages;

public partial class Kids
{
    private IQueryable<Data.Child>? data;
    private DateTime LastUpdatedDateTime;                   // TODO: rename to '...Any...'
    bool UserNeedsToUpdateInfo = false;

    protected override async Task OnInitializedAsync()
    {
        DataStore.Family = await dal.GetDataAsync();
        if (DataStore.Family is not null)
        {
            data = DataStore.Family.Children.AsQueryable();
        }

        var lastUpdated = await dal.GetLastUpdatedDateTimeAsync();
        if (lastUpdated != null)
        {
            LastUpdatedDateTime = lastUpdated ?? DateTime.MinValue;     // Theoretically, LastUpdatedDateTime should never equal DateTime.MinValue
            // LastUpdatedDateTime = LastUpdatedDateTime.AddDays(-100);   // Temporary code to test 'needs to update' logic
            DateTime today = DateTime.Today;
            DateTime dateNumberOfDaysAgo = today.AddDays(-30);

            UserNeedsToUpdateInfo = dateNumberOfDaysAgo > LastUpdatedDateTime;
        }
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
