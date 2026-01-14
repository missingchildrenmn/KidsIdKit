using KidsIdKit.Core.Data;
using Microsoft.AspNetCore.Components;

namespace KidsIdKit.Core.Pages.SocialMediaAccounts;

public partial class SocialMediaAccountDetails
{
    [Parameter] public int childId { get; set; }
    [Parameter] public int socialMediaAccountId { get; set; }

    ChildDetails? CurrentChild;
    SocialMediaAccount? SocialMediaAccount;
    private string? messageText;
    // TODO: Extract "Social Media Account" from .razor file to a PageTitle field

    protected override void OnInitialized()
    {
        ArgumentNullException.ThrowIfNull(DataStore.Family);

        if (DataStore.Family is not null)
        {
            CurrentChild = DataStore.Family.Children[childId].ChildDetails;
        }

        if (socialMediaAccountId == -1)
        {
            SocialMediaAccount = new SocialMediaAccount();
            if (DataStore.Family is not null)
            {
                if (DataStore.Family.Children[childId].SocialMediaAccounts.Count == 0)
                {
                    SocialMediaAccount.Id = 0;
                }
                else
                {
                    SocialMediaAccount.Id = DataStore.Family.Children[childId].SocialMediaAccounts.Max(r => r.Id) + 1;
                }
            }
        }
        else
        {
            if (DataStore.Family is not null)
            {
                SocialMediaAccount = DataStore.Family.Children[childId].SocialMediaAccounts[socialMediaAccountId];
            }
        }
    }

    private async Task SaveData()
    {
        messageText = string.Empty;
        try
        {
            if (DataStore.Family is not null && SocialMediaAccount is not null)
            {
                if (socialMediaAccountId == -1)
                {
                    DataStore.Family.Children[childId].SocialMediaAccounts.Add(SocialMediaAccount);
                }
                await dal.SaveDataAsync(DataStore.Family);
            }
            NavigationManager.NavigateTo($"/childSocialMediaAccounts/{childId}");
        }
        catch (Exception e)
        {
            messageText = e.Message;
        }
    }
}
