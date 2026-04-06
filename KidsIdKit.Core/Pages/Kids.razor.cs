using KidsIdKit.Core.Data;
using KidsIdKit.Core.SharedComponents;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace KidsIdKit.Core.Pages;

public partial class Kids
{
    private IQueryable<Data.Child>? data;
    private string ExistingChildText = string.Empty;
    private string EditExistingChildText = string.Empty;
    private string? errorMessage;
    private bool AlertShow = false;
    private string AlertTitle = string.Empty;
    private string AlertMessage = "Are you sure you want to remove this child from the app?";
    private string AlertStateInformation = string.Empty;

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

    public async Task DeleteResponse(string stateInformation, McmAlert.AlertAction result)
    {
        AlertShow = false;
        int childId = int.Parse(stateInformation);
        if (result == McmAlert.AlertAction.Confirm)
        {
            var child = FamilyState.Family!.Children.FirstOrDefault(c => c.Id == childId);
            if (child != null)
            {
                FamilyState.Family.Children.Remove(child);
                await FamilyState.SaveAsync();
                data = FamilyState.Family.Children.AsQueryable();
            }
        }
    }

    public void ShowAlert(Data.Child child)
    {
        AlertTitle = $"Remove {child.ChildDetails.GivenName} {child.ChildDetails.FamilyName} ?";
        AlertStateInformation = child.Id.ToString();
        AlertShow = true;
    }
}
