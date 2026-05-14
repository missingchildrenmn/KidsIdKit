using Humanizer;
using KidsIdKit.Core.Data;
using KidsIdKit.Core.SharedComponents;
using Microsoft.AspNetCore.Components;

namespace KidsIdKit.Core.Pages;

public partial class Kids
{
    private IQueryable<Data.Child>? data;
    private string ExistingChildText = string.Empty;
    private string EditExistingChildText = string.Empty;
    private string? errorMessage;
    private const string AlertShowState = "AlertShow";
    private const string AlertTitleState = "AlertTitle";
    private const string AlertMessage = "Are you sure you want to remove this child from the app?";
    private const string AlertStateInformationState = "AlertStateInformation";
    private bool ShowBusyIndicator = false;
    private string BusyMessage = string.Empty;

    #region Properties for reminding the user to update their child's information
    public int SelectedNumberOfDaysToRemind;
    private DateTime LastDateTimeAnyChildWasUpdatedAsync = DateTime.MinValue;
    private bool UserNeedsToUpdateInfo = false;
    #endregion

    protected override Task OnInitializedAsync()
    {
        if (!PageState.AppSuspended)
        {
            PageState.ClearStateItems();
        }

        PageState.AppSuspended = false;

        PageState.InitStateItem<bool>(AlertShowState, false);
        PageState.InitStateItem<string>(AlertTitleState, string.Empty);
        PageState.InitStateItem<string>(AlertStateInformationState, string.Empty);

        return Task.CompletedTask;
    }

    protected override bool ShouldRender()
    {
        if (FamilyState.Family is null)
        {
            BusyMessage = "Loading...";
            ShowBusyIndicator = true;
        }
        return base.ShouldRender();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        try
        {
            if (firstRender)
            {
                await InvokeAsync(StateHasChanged);
                await Task.Run(async () => { 
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
                });
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
        finally
        {
            BusyMessage = string.Empty;
            ShowBusyIndicator = false;
            await InvokeAsync(StateHasChanged);
        }
        await base.OnAfterRenderAsync(firstRender);
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
        PageState.SetStateItem<bool>(AlertShowState, false);
        int childId = int.Parse(stateInformation);
        if (result == McmAlert.AlertAction.Confirm)
        {
            BusyMessage = "Removing child...";
            ShowBusyIndicator = true;
            await InvokeAsync(StateHasChanged);
            await Task.Run(async () =>
            {
                var child = FamilyState.Family!.Children.FirstOrDefault(c => c.Id == childId);
                if (child != null)
                {
                    FamilyState.Family.Children.Remove(child);
                    await FamilyState.SaveAsync();
                    data = FamilyState.Family.Children.AsQueryable();
                }
            });
            ShowBusyIndicator = false;
            BusyMessage = string.Empty;
            await InvokeAsync(StateHasChanged);
        }
    }

    public void ShowAlert(Data.Child child)
    {
        PageState.SetStateItem<string>(AlertStateInformationState, child.Id.ToString());
        PageState.SetStateItem<string>(AlertTitleState, $"Remove {child.ChildDetails.GivenName} {child.ChildDetails.FamilyName} ?");
        PageState.SetStateItem<bool>(AlertShowState, true);
    }
}
