using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KidsIdKit.Pages.info
{
    public class InfoHyperlink
    {
        public static MarkupString RunawayPhoneNumber() {
            var userFriendlyPhoneLink = HyperlinkHelper.PhoneNumberHelper.GetPhoneLink("1-800-786-2929");
            return (MarkupString)$"1-888-RUNAWAY ({userFriendlyPhoneLink})";
        }

        public static MarkupString RunYellPhoneNumber() {
            var userFriendly800PhoneLink = HyperlinkHelper.PhoneNumberHelper.GetPhoneLink("1-888-786-9355");
            var userFriendlyPhoneLink = HyperlinkHelper.PhoneNumberHelper.GetPhoneLink("1-612-334-9449");
            return (MarkupString)$"{userFriendlyPhoneLink} or 1-888-RUN-YELL ({userFriendly800PhoneLink})";
        }
    }
}
