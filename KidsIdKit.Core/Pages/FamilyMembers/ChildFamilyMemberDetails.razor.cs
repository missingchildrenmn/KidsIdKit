using KidsIdKit.Core.Data;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Text;

namespace KidsIdKit.Core.Pages.FamilyMembers;

public partial class ChildFamilyMemberDetails
{
    [Parameter] public int childId { get; set; }
    [Parameter] public int familyId { get; set; }

    ChildDetails? CurrentChild;
    FamilyMember? Family;
    private string? messageText;
    public override string MenuBarTitle { get; protected set; } = "Family Member";
    
    protected override void OnInitialized()
    {
        var child = FamilyState.GetChild(childId);
        if (child != null)
        {
            CurrentChild = child.ChildDetails;

            if (familyId == -1)
            {
                Family = new FamilyMember();
                Family.Id = child.FamilyMembers.Count == 0 ? 0 : child.FamilyMembers.Max(r => r.Id) + 1;
            }
            else if (familyId >= 0 && familyId < child.FamilyMembers.Count)
            {
                Family = child.FamilyMembers[familyId];
            }
        }
    }

    private async Task SaveData()
    {
        messageText = string.Empty;
        try
        {
            var child = FamilyState.GetChild(childId);
            if (child != null && Family is not null)
            {
                if (familyId == -1)
                {
                    child.FamilyMembers.Add(Family);
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
