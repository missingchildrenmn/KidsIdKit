using KidsIdKit.Core.Data;
using KidsIdKit.Core.SharedComponents;
using Microsoft.AspNetCore.Components;

namespace KidsIdKit.Core.Pages.FamilyMembers;

public partial class ChildFamilyMembers
{
    [Parameter] public int Id { get; set; }

    ChildDetails? CurrentChild;
    IEnumerable<FamilyMember>? Family;

    public override string MenuBarTitle { get; protected set; } = "Family Members";

    private bool AlertShow = false;
    private string AlertTitle = string.Empty;
    private string AlertMessage = "Are you sure you want to remove this family member?";
    private string AlertStateInformation = string.Empty;


    protected override void OnParametersSet()
    {
        var child = FamilyState.GetChild(Id);
        if (child != null)
        {
            CurrentChild = child.ChildDetails;
            Family = child.FamilyMembers;
        }
    }

    private void NavigateToFamilyMemberEdit(int childId, int personId)
    {
        NavigationManager.NavigateTo($"/Family/{childId}/{personId}");
    }

    public async Task DeleteResponse(string stateInformation, McmAlert.AlertAction result)
    {
        AlertShow = false;
        int familyMemberId = int.Parse(stateInformation);
        if (result == McmAlert.AlertAction.Confirm)
        {
            var child = FamilyState.GetChild(Id);
            if (child != null && familyMemberId >= 0)
            {
                var familyMember = child.FamilyMembers.FirstOrDefault((p) => p.Id == familyMemberId);
                if (familyMember is not null)
                {
                    child.FamilyMembers.Remove(familyMember);
                    await FamilyState.SaveAsync();
                    Family = child.FamilyMembers;
                }
            }
        }
    }

    public void ShowAlert(FamilyMember familyMember)
    {
        AlertTitle = $"Remove family member {familyMember.GivenName} {familyMember.FamilyName} ?";
        AlertStateInformation = familyMember.Id.ToString();
        AlertShow = true;
    }
}
