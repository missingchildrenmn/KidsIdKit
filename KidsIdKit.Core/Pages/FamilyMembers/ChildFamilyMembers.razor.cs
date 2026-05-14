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

    private const string AlertShowState = "AlertShow";
    private const string AlertTitleState = "AlertTitle";
    private const string AlertMessage = "Are you sure you want to remove this family member?";
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
            Family = child.FamilyMembers;
        }
    }

    private void NavigateToFamilyMemberEdit(int childId, int personId)
    {
        NavigationManager.NavigateTo($"/Family/{childId}/{personId}");
    }

    public async Task DeleteResponse(string stateInformation, McmAlert.AlertAction result)
    {
        PageState.SetStateItem<bool>(AlertShowState, false);
        int familyMemberId = int.Parse(stateInformation);
        if (result == McmAlert.AlertAction.Confirm)
        {
            BusyMessage = "Deleting...";
            ShowBusyIndicator = true;
            await InvokeAsync(StateHasChanged);
            try
            {
                await Task.Run(async () => {
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

    public void ShowAlert(FamilyMember familyMember)
    {
        PageState.SetStateItem<string>(AlertStateInformationState, familyMember.Id.ToString());
        PageState.SetStateItem<string>(AlertTitleState, $"Remove family member {familyMember.GivenName} {familyMember.FamilyName} ?");
        PageState.SetStateItem<bool>(AlertShowState, true);
    }
}
