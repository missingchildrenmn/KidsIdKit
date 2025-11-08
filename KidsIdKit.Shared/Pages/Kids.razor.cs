using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System.Diagnostics;

namespace KidsIdKit.Shared.Pages;

public partial class Kids
{
    private IQueryable<Data.Child>? data;
    string ExistingChildText = string.Empty;
    string EditExistingChildText = string.Empty;

    #region Properties for reminding the user to update their child's information
    // public IReadOnlyList<int> AllPossibleNumberOfDaysToRemind { get; } = new List<int> { 30, 60, 90 };
    public int SelectedNumberOfDaysToRemind;
    //private string selected = null!;
    //private int selectedNumberOfDaysToRemind;
    //public int SelectedNumberOfDaysToRemind
    //{
    //    get => selectedNumberOfDaysToRemind;
    //    set
    //    {
    //        selectedNumberOfDaysToRemind = value;
    //        Debug.WriteLine($"You selected {value}");
    //    }
    //}
    private DateTime LastDateTimeAnyChildWasUpdatedAsync = DateTime.MinValue;
    //public int SelectedNumberOfDaysToRemind
    //{
    //    get => selectedNumberOfDaysToRemind;
    //    set => selectedNumberOfDaysToRemind = value;
    //}
    bool UserNeedsToUpdateInfo = false;
    #endregion

    //private void OnSelectedNumberOfDaysToRemindChanged(int value)
    //{
    //    SelectedNumberOfDaysToRemind = value;
    //    YourCustomMethod(value);
    //}

    //private void YourCustomMethod(int selectedValue)
    //{
    //    Debug.WriteLine($"User selected: {selectedValue}");
    //    // Add your logic here
    //}

    //void SetSelected(int value)
    //{
    //    // var _ = AllPossibleNumberOfDaysToRemind.Find(e => e.Id == value);
    //    //if (_ != null)
    //    //object.apple = _;
    //    //selected = value;
    //    selectedNumberOfDaysToRemind = int.Parse(value);
    //}

    protected override async Task OnInitializedAsync()
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
                //LastDateTimeAnyChildWasUpdatedAsync = LastDateTimeAnyChildWasUpdatedAsync.AddDays(-100);   // Temporary code to test 'needs to update' logic

                DateTime today = DateTime.Today;
                DateTime dateNumberOfDaysAgo = today.AddDays(-30);

                UserNeedsToUpdateInfo = dateNumberOfDaysAgo > LastDateTimeAnyChildWasUpdatedAsync;
            }
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
