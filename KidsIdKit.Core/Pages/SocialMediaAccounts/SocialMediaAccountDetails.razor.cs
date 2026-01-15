using KidsIdKit.Core.Data;
using KidsIdKit.Core.Services;
using Microsoft.AspNetCore.Components;

namespace KidsIdKit.Core.Pages.SocialMediaAccounts;

public partial class SocialMediaAccountDetails
{
    [Inject] private IFamilyStateService FamilyState { get; set; } = default!;
    [Inject] private NavigationManager NavigationManager { get; set; } = default!;

    [Parameter] public int childId { get; set; }
    [Parameter] public int socialMediaAccountId { get; set; }

    ChildDetails? CurrentChild;
    SocialMediaAccount? SocialMediaAccount;
    private string? messageText;
    // TODO: Extract "Social Media Account" from .razor file to a PageTitle field

    protected override void OnInitialized()
    {
        var child = FamilyState.GetChild(childId);
        if (child != null)
        {
            CurrentChild = child.ChildDetails;

            if (socialMediaAccountId == -1)
            {
                SocialMediaAccount = new SocialMediaAccount();
                if (child.SocialMediaAccounts.Count == 0)
                {
                    SocialMediaAccount.Id = 0;
                }
                else
                {
                    SocialMediaAccount.Id = child.SocialMediaAccounts.Max(r => r.Id) + 1;
                }
            }
            else if (socialMediaAccountId >= 0 && socialMediaAccountId < child.SocialMediaAccounts.Count)
            {
                SocialMediaAccount = child.SocialMediaAccounts[socialMediaAccountId];
            }
        }
    }

    private async Task SaveData()
    {
        messageText = string.Empty;
        try
        {
            var child = FamilyState.GetChild(childId);
            if (child != null && SocialMediaAccount is not null)
            {
                if (socialMediaAccountId == -1)
                {
                    child.SocialMediaAccounts.Add(SocialMediaAccount);
                }
                await FamilyState.SaveAsync();
            }
            NavigationManager.NavigateTo($"/childSocialMediaAccounts/{childId}");
        }
        catch (Exception e)
        {
            messageText = e.Message;
        }
    }
}
