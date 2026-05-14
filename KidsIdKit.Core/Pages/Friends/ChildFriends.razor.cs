using KidsIdKit.Core.Data;
using KidsIdKit.Core.SharedComponents;
using Microsoft.AspNetCore.Components;

namespace KidsIdKit.Core.Pages.Friends;

public partial class ChildFriends
{
    [Parameter] public int Id { get; set; }

    ChildDetails? CurrentChild;
    IEnumerable<KidsIdKit.Core.Data.Person>? Friends;

    public override string MenuBarTitle { get; protected set; } = "Child's Friends";

    private const string AlertShowState = "AlertShow";
    private const string AlertTitleState = "AlertTitle";
    private const string AlertMessage = "Are you sure you want to remove this friend?";
    private const string AlertStateInformationState = "AlertStateInformation";

    private bool ShowBusyIndicator = false;
    private string BusyMessage = string.Empty;
    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();

        PageState.InitStateItem<bool>(AlertShowState, false);
        PageState.InitStateItem<string>(AlertTitleState, string.Empty);
        PageState.InitStateItem<string>(AlertStateInformationState, string.Empty);
    }

    protected override void OnParametersSet()
    {
        var child = FamilyState.GetChild(Id);
        if (child != null)
        {
            CurrentChild = child.ChildDetails;
            Friends = child.Friends;
        }
    }

    private void NavigateToFriendEdit(int childId, int friendId)
    {
        NavigationManager.NavigateTo($"/Friend/{childId }/{friendId}");
    }

    public async Task DeleteResponse(string stateInformation, McmAlert.AlertAction result)
    {
        PageState.SetStateItem<bool>(AlertShowState, false);
        int friendId = int.Parse(stateInformation);
        if (result == McmAlert.AlertAction.Confirm)
        {
            BusyMessage = "Deleting...";
            ShowBusyIndicator = true;
            await InvokeAsync(StateHasChanged);
            try
            {
                await Task.Run(async () => {
                    var child = FamilyState.GetChild(Id);
                    if (child != null && friendId >= 0)
                    {
                        var friend = child.Friends.FirstOrDefault((p) => p.Id == friendId);
                        if (friend is not null)
                        {
                            child.Friends.Remove(friend);
                            await FamilyState.SaveAsync();
                            Friends = child.Friends;
                        }
                    }
                });
            }
            finally
            {
                BusyMessage = string.Empty;
                ShowBusyIndicator = false;
                await InvokeAsync(StateHasChanged);
            }
        }
    }

    public void ShowAlert(Person friend)
    {
        PageState.SetStateItem<string>(AlertStateInformationState, friend.Id.ToString());
        PageState.SetStateItem<string>(AlertTitleState, $"Remove {friend.GivenName} {friend.FamilyName} ?");
        PageState.SetStateItem<bool>(AlertShowState, true);
    }
}
