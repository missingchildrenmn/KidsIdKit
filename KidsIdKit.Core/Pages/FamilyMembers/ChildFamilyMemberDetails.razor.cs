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
    
    protected override Task OnInitializedAsync()
    {
        var returnValue = base.OnInitializedAsync();

        var child = FamilyState.GetChild(ChildId);
        if (child != null)
        {
            CurrentChild = child.ChildDetails;

            if (FamilyId == -1)
            {
                var newMember = new FamilyMember();
                newMember.Id = child.FamilyMembers.Count == 0 ? 0 : child.FamilyMembers.Max(r => r.Id) + 1;
                PageState.InitStateItem<Data.FamilyMember?>(EditingObjectState, newMember);
            }
            else if (FamilyId >= 0)
            {
                var index = child.FamilyMembers.FindIndex(f => f.Id == FamilyId);
                if (index >= 0)
                {
                    PageState.InitStateItem<Data.FamilyMember?>(EditingObjectState, child.FamilyMembers[index]);
                }
                else
                {
                    Console.WriteLine($"Family member with an ID of {FamilyId} was not found");
                }
            }
            var editingObject = PageState.GetStateItem<Data.FamilyMember?>(EditingObjectState).Value;
            if (editingObject != null)
            {
                PageState.InitStateItem<string?>(OriginalSnapshotState, SerializeObject(editingObject));
            }
        }

        return returnValue;

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
                var editingObject = PageState.GetStateItem<Data.FamilyMember?>(EditingObjectState).Value;
                if (child != null && editingObject is not null)
                {
                    if (FamilyId == -1)
                    {
                        child.FamilyMembers.Add(editingObject);
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
