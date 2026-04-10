using KidsIdKit.Core.Data;
using Microsoft.AspNetCore.Components;

namespace KidsIdKit.Core.Pages.Friends;

public partial class ChildFriendDetails
{
    [Parameter] public int childId { get; set; }
    [Parameter] public int friendId { get; set; }

    ChildDetails? CurrentChild;
    Person? Friend;
    private string? messageText;
    public override string MenuBarTitle { get; protected set; } = "Friend";

    protected override void OnInitialized()
    {
        var child = FamilyState.GetChild(childId);
        if (child != null)
        {
            CurrentChild = child.ChildDetails;

            if (friendId == -1)
            {
                Friend = new Person();
                Friend.Id = child.Friends.Count == 0 ? 0 : child.Friends.Max(r => r.Id) + 1;
            }
            else if (friendId >= 0 && friendId < child.Friends.Count)
            {
                Friend = child.Friends[friendId];
            }
        }
    }

    private async Task SaveData()
    {
        messageText = string.Empty;
        try
        {
            var child = FamilyState.GetChild(childId);
            if (child != null && Friend is not null)
            {
                if (friendId == -1)
                {
                    child.Friends.Add(Friend);
                }
                await FamilyState.SaveAsync();
            }
            await NavigateBack();
        }
        catch (Exception e)
        {
            messageText = e.Message;
        }
    }
}
