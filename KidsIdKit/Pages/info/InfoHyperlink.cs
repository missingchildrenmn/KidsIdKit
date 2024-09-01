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
        public static MarkupString RunYellPhoneNumber() {
            var userFriendlyPhoneLink = HyperlinkHelper.PhoneNumberHelper.GetPhoneLink("1-888-786-9355");
            return (MarkupString)$"1-888-RUN-YELL ({userFriendlyPhoneLink})";
        }
    }
}
