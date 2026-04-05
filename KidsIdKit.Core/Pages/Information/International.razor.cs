using Microsoft.AspNetCore.Components;

namespace KidsIdKit.Core.Pages.Information
{
    public partial class International
    {
        private string cpiapStateDepartmentPhoneNumber = "(888) 407-4747";
        private MarkupString? cpiapStateDepartmentPhoneNumberLink;
        public override string MenuBarTitle { get; protected set; } = "International Abductions";

        protected override void OnInitialized()
        {
            cpiapStateDepartmentPhoneNumberLink = HyperlinkHelper.PhoneNumberHelper.GetPhoneLink(cpiapStateDepartmentPhoneNumber);
        }
    }
}
