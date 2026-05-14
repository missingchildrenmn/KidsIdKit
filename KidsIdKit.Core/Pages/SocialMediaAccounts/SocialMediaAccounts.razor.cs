using KidsIdKit.Core.Data;
using KidsIdKit.Core.SharedComponents;
using Microsoft.AspNetCore.Components;

namespace KidsIdKit.Core.Pages.SocialMediaAccounts;

public partial class SocialMediaAccounts
{
    [Parameter] public int Id { get; set; }

    ChildDetails? CurrentChild { get; set; }
    IEnumerable<SocialMediaAccount>? SocialMediaAccountObjects;
    private readonly PasswordVisibilityManager passwordVisibilityManager = new();
    public override string MenuBarTitle { get; protected set; } = "Social Media";

    private const string AlertShowState = "AlertShow";
    private const string AlertTitleState = "AlertTitle";
    private const string AlertMessage = "Are you sure you want to remove this social media account?";
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
            SocialMediaAccountObjects = child.SocialMediaAccounts;
        }
    }

    private class PasswordVisibilityManager
    {
        private readonly HashSet<int> visiblePasswords = new();

        public void ShowPassword(int accountId) => visiblePasswords.Add(accountId);

        public void HidePassword(int accountId) => visiblePasswords.Remove(accountId);

        public bool IsPasswordVisible(int accountId) => visiblePasswords.Contains(accountId);

        public void TogglePassword(int accountId)
        {
            if (!visiblePasswords.Remove(accountId))
                visiblePasswords.Add(accountId);
        }

        public void HideAllPasswords() => visiblePasswords.Clear();

        public int VisiblePasswordCount => visiblePasswords.Count;
    }

    private void NavigateToSocialMediaAccountEdit(int childId, int socialMediaAccountId)
    {
        NavigationManager.NavigateTo($"/SocialMediaAccount/{childId}/{socialMediaAccountId}");
    }

    public async Task DeleteResponse(string stateInformation, McmAlert.AlertAction result)
    {
        PageState.SetStateItem<bool>(AlertShowState, false);
        int socialMediaAccountId = int.Parse(stateInformation);
        if (result == McmAlert.AlertAction.Confirm)
        {
            BusyMessage = "Deleting...";
            ShowBusyIndicator = true;
            await InvokeAsync(StateHasChanged);
            try
            {
                await Task.Run(async () => {
                    var child = FamilyState.GetChild(Id);
                    if (child != null && socialMediaAccountId >= 0)
                    {
                        var socialMediaAccount = child.SocialMediaAccounts.FirstOrDefault((p) => p.Id == socialMediaAccountId);
                        if (socialMediaAccount is not null)
                        {
                            child.SocialMediaAccounts.Remove(socialMediaAccount);
                            await FamilyState.SaveAsync();
                            SocialMediaAccountObjects = child.SocialMediaAccounts;
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

    public void ShowAlert(SocialMediaAccount socialMediaAccount)
    {
        PageState.SetStateItem<string>(AlertStateInformationState, socialMediaAccount.Id.ToString());
        PageState.SetStateItem<string>(AlertTitleState, $"Remove {socialMediaAccount.Platform} ?");
        PageState.SetStateItem<bool>(AlertShowState, true);
    }
}
