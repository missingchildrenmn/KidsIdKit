using KidsIdKit.Core.Data;
using KidsIdKit.Core.Services;
using Microsoft.AspNetCore.Components;

namespace KidsIdKit.Core.Pages.SocialMediaAccounts;

public partial class SocialMediaAccountRemove
{
    [Inject] private IFamilyStateService FamilyState { get; set; } = default!;
    [Inject] private NavigationManager NavigationManager { get; set; } = default!;

    [Parameter] public int childId { get; set; }
    [Parameter] public int socialMediaAccountId { get; set; }

    ChildDetails? CurrentChild;
    SocialMediaAccount? CurrentSocialMediaAccount;

    protected override void OnParametersSet()
    {
        var child = FamilyState.GetChild(childId);
        if (child != null && socialMediaAccountId >= 0 && socialMediaAccountId < child.SocialMediaAccounts.Count)
        {
            CurrentChild = child.ChildDetails;
            CurrentSocialMediaAccount = child.SocialMediaAccounts[socialMediaAccountId];
        }
    }

    private async Task YesRemove()
    {
        var child = FamilyState.GetChild(childId);
        if (child != null && CurrentSocialMediaAccount is not null)
        {
            child.SocialMediaAccounts.Remove(CurrentSocialMediaAccount);
            await FamilyState.SaveAsync();
        }
        NavigationManager.NavigateTo($"/childSocialMediaAccounts/{childId}");
    }

    private void NoCancel()
    {
        NavigationManager.NavigateTo($"/childSocialMediaAccounts/{childId}");
    }
}
