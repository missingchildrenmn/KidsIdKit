using KidsIdKit.Shared.Data;
using Microsoft.AspNetCore.Components;

namespace KidsIdKit.Shared.Pages.SocialMediaAccounts;

public partial class SocialMediaAccounts
{
    [Parameter] public int Id { get; set; }

    ChildDetails? CurrentChild { get; set; }
    IEnumerable<SocialMediaAccount>? SocialMediaAccountObjects;
    readonly string PageTitle = "Social Media Accounts";
    private readonly PasswordVisibilityManager passwordVisibilityManager = new();

    protected override void OnParametersSet()
    {
        if (DataStore.Family is not null)
        {
            CurrentChild = DataStore.Family.Children[Id].ChildDetails;
            SocialMediaAccountObjects = DataStore.Family.Children[Id].SocialMediaAccounts;
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
}
