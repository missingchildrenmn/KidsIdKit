using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace KidsIdKit.Shared.Pages;

public partial class Kids
{
    private IQueryable<Data.Child>? data;

    protected override async Task OnInitializedAsync()
    {
        DataStore.Family = await dal.GetDataAsync();
        if (DataStore.Family is not null)
        {
            data = DataStore.Family.Children.AsQueryable();
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