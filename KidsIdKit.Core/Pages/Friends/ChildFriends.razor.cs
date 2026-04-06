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

    private bool AlertShow = false;
    private string AlertTitle = string.Empty;
    private string AlertMessage = "Are you sure you want to remove this friend?";
    private string AlertStateInformation = string.Empty;

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
        AlertShow = false;
        int friendId = int.Parse(stateInformation);
        if (result == McmAlert.AlertAction.Confirm)
        {
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
        }
    }

    public void ShowAlert(Person friend)
    {
        AlertTitle = $"Remove {friend.GivenName} {friend.FamilyName} ?";
        AlertStateInformation = friend.Id.ToString();
        AlertShow = true;
    }
}
