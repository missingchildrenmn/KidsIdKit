using KidsIdKit.Core.Data;
using KidsIdKit.Core.SharedComponents;
using Microsoft.AspNetCore.Components;

namespace KidsIdKit.Core.Pages.FamilyMembers;

public partial class ChildFamilyMemberDetails : EditablePageBase<Data.FamilyMember>
{
    [Parameter] public int ChildId { get; set; }
    [Parameter] public int FamilyId { get; set; }

    Data.ChildDetails? CurrentChild;
    private string? messageText;
    public override string MenuBarTitle { get; protected set; } = "Family Member";
    
    protected override void OnInitialized()
    {
        var child = FamilyState.GetChild(ChildId);
        if (child != null)
        {
            CurrentChild = child.ChildDetails;

            if (FamilyId == -1)
            {
                EditingObject = new FamilyMember();
                EditingObject!.Id = child.FamilyMembers.Count == 0 ? 0 : child.FamilyMembers.Max(r => r.Id) + 1;
            }
            else if (FamilyId >= 0)
            {
                var index = child.FamilyMembers.FindIndex(f => f.Id == FamilyId);
                if (index >= 0)
                {
                    EditingObject = child.FamilyMembers[index];
                }
                else
                {
                    Console.WriteLine($"Family member with an ID of {FamilyId} was not found");
                }
            }
            originalSnapshot = SerializeObject(EditingObject!);
        }
        ShowPendingChangesAlert = false;
    }

    protected override FamilyMember ResetUnalteredObject(FamilyMember unalteredObject)
    {
        var child = FamilyState.GetChild(ChildId);
        if (child == null)
        {
            return unalteredObject;
        }

        if (child.FamilyMembers.Any(f => f.Id == FamilyId))
        {
            var index = child.FamilyMembers.FindIndex(f => f.Id == FamilyId);
            if (index >= 0)
            {
                child.FamilyMembers[index] = unalteredObject;
            }
            else
            {
                Console.WriteLine($"Family member with an ID of {FamilyId} was not found");
            }
        }
        return unalteredObject;
    }

    protected override async Task SaveData()
    {
        messageText = string.Empty;
        if (ValidateChangesForSave())
        {
            try
            {
                var child = FamilyState.GetChild(ChildId);
                if (child != null && EditingObject is not null)
                {
                    if (FamilyId == -1)
                    {
                        child.FamilyMembers.Add(EditingObject);
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
