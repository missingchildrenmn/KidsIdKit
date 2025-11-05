using KidsIdKit.Shared.Data;
using Microsoft.AspNetCore.Components;

namespace KidsIdKit.Shared.Pages.SocialMediaAccounts;

public partial class SocialMediaAccounts
{
    [Parameter] public int Id { get; set; }

    ChildDetails? CurrentChild { get; set; }
    IEnumerable<SocialMediaAccount>? SocialMediaAccountObjects;

    protected override void OnParametersSet()
    {
        if (DataStore.Family is not null)
        {
            CurrentChild = DataStore.Family.Children[Id].ChildDetails;
            SocialMediaAccountObjects = DataStore.Family.Children[Id].SocialMediaAccounts;
        }
    }
}
