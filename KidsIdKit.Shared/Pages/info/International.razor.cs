using Microsoft.AspNetCore.Components;

namespace KidsIdKit.Shared.Pages.info
{
    public partial class International : ComponentBase
    {
        private string cpiapStateDepartmentPhoneNumber = "(888) 407-4747";
        private MarkupString? cpiapStateDepartmentPhoneNumberLink;

        protected override void OnInitialized()
        {
            cpiapStateDepartmentPhoneNumberLink = HyperlinkHelper.PhoneNumberHelper.GetPhoneLink(cpiapStateDepartmentPhoneNumber);
        }
    }
}
