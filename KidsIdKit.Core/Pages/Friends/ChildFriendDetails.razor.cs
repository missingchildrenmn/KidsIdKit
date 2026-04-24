using KidsIdKit.Core.Data;
using KidsIdKit.Core.Pages.SocialMediaAccounts;
using KidsIdKit.Core.SharedComponents;
using Microsoft.AspNetCore.Components;

namespace KidsIdKit.Core.Pages.Friends;

public partial class ChildFriendDetails : EditablePageBase<Data.Person>
{
    [Parameter] public int ChildId { get; set; }
    [Parameter] public int FriendId { get; set; }

    ChildDetails? CurrentChild;
    private string? messageText;
    public override string MenuBarTitle { get; protected set; } = "Friend";

    protected override void OnInitialized()
    {
        var child = FamilyState.GetChild(ChildId);
        if (child != null)
        {
            CurrentChild = child.ChildDetails;

            if (FriendId == -1)
            {
                EditingObject = new Person();
                EditingObject!.Id = child.Friends.Count == 0 ? 0 : child.Friends.Max(r => r.Id) + 1;
            }
            else if (FriendId >= 0)
            {
                var index = child.Friends.FindIndex(f => f.Id == FriendId);
                if (index >= 0)
                {
                    EditingObject = child.Friends[index];
                }
                else
                {
                    Console.WriteLine($"Friend with an ID of {FriendId} was not found.");
                }    
            }
            originalSnapshot = SerializeObject(EditingObject!);
        }
        ShowPendingChangesAlert = false;
    }

    protected override Person ResetUnalteredObject(Person unalteredObject)
    {
        var child = FamilyState.GetChild(ChildId);
        if (child == null)
        {
            return unalteredObject;
        }

        if (child.Friends.Any(f => f.Id == FriendId))
        {
            var index = child.Friends.FindIndex(f => f.Id == FriendId);
            if (index >= 0)
            {
                child.Friends[index] = unalteredObject;
            }
            else
            {
                Console.WriteLine($"Friend with an ID of {FriendId} was not found.");
            }
        }
        return unalteredObject;
    }

    protected override async Task SaveData()
    {
        messageText = string.Empty;
        // Validate before saving
        if (ValidateChangesForSave())
        {
            try
            {
                var child = FamilyState.GetChild(ChildId);
                if (child != null && EditingObject is not null)
                {
                    if (FriendId == -1)
                    {
                        child.Friends.Add(EditingObject);
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
}
