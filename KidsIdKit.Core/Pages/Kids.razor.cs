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
            await FamilyState.LoadAsync();
            if (FamilyState.Family is not null)
            {
                data = FamilyState.Family.Children.AsQueryable();

                ExistingChildText = $"existing child{(FamilyState.Family.Children.Count > 1 ? "ren" : string.Empty)}";
                EditExistingChildText = $"Edit {ExistingChildText}";

                LastDateTimeAnyChildWasUpdatedAsync = FamilyState.Family.LastDateTimeAnyChildWasUpdated;
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
            data = Enumerable.Empty<Data.Child>().AsQueryable();
        }
        catch (Exception ex)
        {
            errorMessage = $"Unable to load your data: {ex.Message}";
            data = Enumerable.Empty<Data.Child>().AsQueryable();
        }
    }

    private void NavigateToEdit(Data.Child child)
    {
        if (FamilyState.Family != null)
        {
            var index = FamilyState.Family.Children.IndexOf(child);
            NavigationManager.NavigateTo($"/Child/{index}");
        }
    }

    private void NavigateToRemove(Data.Child child, MouseEventArgs e)
    {
        if (FamilyState.Family != null)
        {
            var index = FamilyState.Family.Children.IndexOf(child);
            NavigationManager.NavigateTo($"/ChildRemove/{index}");
        }
    }
}
