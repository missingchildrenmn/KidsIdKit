using KidsIdKit.Core.Data;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace KidsIdKit.Core.Pages;

public partial class Kids
{
    private IQueryable<Data.Child>? data;
    private string ExistingChildText = string.Empty;
    private string EditExistingChildText = string.Empty;
    private string? errorMessage;

    #region Properties for reminding the user to update their child's information
    public int SelectedNumberOfDaysToRemind;
    private DateTime LastDateTimeAnyChildWasUpdatedAsync = DateTime.MinValue;
    private bool UserNeedsToUpdateInfo = false;
    #endregion

    protected override async Task OnInitializedAsync()
    {
        try
        {
            DataStore.Family = await dal.GetDataAsync();
            if (DataStore.Family is not null)
            {
                data = DataStore.Family.Children.AsQueryable();

                ExistingChildText = $"existing child{(DataStore.Family.Children.Count > 1 ? "ren" : string.Empty)}";
                EditExistingChildText = $"Edit {ExistingChildText}";

                LastDateTimeAnyChildWasUpdatedAsync = DataStore.Family.LastDateTimeAnyChildWasUpdated;
                if (LastDateTimeAnyChildWasUpdatedAsync != DateTime.MinValue)
                {
                    var today = DateTime.Today;
                    var dateNumberOfDaysAgo = today.AddDays(-30);
                    UserNeedsToUpdateInfo = dateNumberOfDaysAgo > LastDateTimeAnyChildWasUpdatedAsync;
                }
            }
        }
        catch (DataAccessException ex)
        {
            errorMessage = ex.Message;
            DataStore.Family = new Family();
            data = DataStore.Family.Children.AsQueryable();
        }
        catch (Exception ex)
        {
            errorMessage = $"Unable to load your data: {ex.Message}";
            DataStore.Family = new Family();
            data = DataStore.Family.Children.AsQueryable();
        }
    }

    //// TODO: resolve why this handler method is not getting called ...
    //private void HandleSelectionChange(ChangeEventArgs changeEventArgs)
    //{
    //    Debug.WriteLine($"You selected {changeEventArgs.Value}");
    //}

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
