using KidsIdKit.Shared.Data;
using Microsoft.AspNetCore.Components;

namespace KidsIdKit.Shared.Pages.SocialMediaAccounts;

public partial class SocialMediaAccountRemove
{
    [Parameter] public int childId { get; set; }
    [Parameter] public int socialMediaAccountId { get; set; }

    ChildDetails? CurrentChild;
    SocialMediaAccount? CurrentSocialMediaAccount;

    protected override void OnParametersSet()
    {
        ArgumentNullException.ThrowIfNull(DataStore.Family);
        CurrentChild = DataStore.Family.Children[childId].ChildDetails;
        //CurrentSocialMediaAccount = CurrentChild!.SocialMediaAccounts[socialMediaAccountLId];
        CurrentSocialMediaAccount = DataStore.Family.Children[childId].SocialMediaAccounts[socialMediaAccountId];
    }

    private async Task YesRemove()
    {
        ArgumentNullException.ThrowIfNull(DataStore.Family);
        if (CurrentChild is not null && CurrentSocialMediaAccount is not null)
        {
            DataStore.Family.Children[childId].SocialMediaAccounts.Remove(CurrentSocialMediaAccount);
        }
        await dal.SaveDataAsync(DataStore.Family);
        navigationManager.NavigateTo($"/childSocialMediaAccounts/{childId}");
    }

    private void NoCancel()
    {
        navigationManager.NavigateTo($"/childSocialMediaAccounts/{childId}");
    }
}