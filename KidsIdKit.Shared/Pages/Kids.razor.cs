using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

using KidsIdKit.Shared.Data;

namespace KidsIdKit.Shared.Pages;

public partial class Kids
{
    private IQueryable<Data.Child>? data;
    private DateTime LastDateTimeAnyChildWasUpdatedAsync;
    bool UserNeedsToUpdateInfo = false;

    protected override async Task OnInitializedAsync()
    {
        DataStore.Family = await dal.GetDataAsync();
        if (DataStore.Family is not null)
        {
            data = DataStore.Family.Children.AsQueryable();

            LastDateTimeAnyChildWasUpdatedAsync = DataStore.Family.LastDateTimeAnyChildWasUpdated;
            if (LastDateTimeAnyChildWasUpdatedAsync != null)
            {
                //LastDateTimeAnyChildWasUpdatedAsync = LastDateTimeAnyChildWasUpdatedAsync.AddDays(-100);   // Temporary code to test 'needs to update' logic

                DateTime today = DateTime.Today;
                DateTime dateNumberOfDaysAgo = today.AddDays(-30);

                UserNeedsToUpdateInfo = dateNumberOfDaysAgo > LastDateTimeAnyChildWasUpdatedAsync;
            }
        }
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
