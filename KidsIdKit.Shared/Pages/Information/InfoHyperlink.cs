﻿using Microsoft.AspNetCore.Components;

namespace KidsIdKit.Shared.Pages.Information
{
    public class InfoHyperlink
    {
        public static MarkupString ContactTheNationalCenterForMissingAndExploitedChildren()
        {
            var userFriendlyPhoneLink = GetPhoneLink("800-843-5678");
            return (MarkupString)$"Contact the National Center for Missing and Exploited Children (NCMEC) at 1-800-THE-LOST ({userFriendlyPhoneLink}) to register your child";
        }

        public static MarkupString InfoLink(string page, string text)
        {
            var doubleQuote = InfoConstants.DOUBLE_QUOTE;
            return (MarkupString)$"<div><a class={doubleQuote}link-primary{doubleQuote} href={doubleQuote}/Information/{page}{doubleQuote}>{text}</a></div>";
        }

        public static MarkupString MissingChildrenMinnesotaWebsite()
        {
            return @HyperlinkHelper.LinkHelper.HtmlLink("Missing Children Minnesota", "https://missingchildrenmn.com/contact/");
        }

        public static MarkupString RunawayPhoneNumber()
        {
            var userFriendlyPhoneLink = GetPhoneLink("800-786-2929");         // TODO - why is the area code different than the one on the next line?
            return (MarkupString)$"1-888-RUNAWAY ({userFriendlyPhoneLink})";
        }
        // Also: https://www.1800runaway.org/

        public static MarkupString RunYellPhoneNumber()
        {
            MarkupString userFriendly800PhoneLink = GetPhoneLink("888-786-9355");
            return (MarkupString)$"1-888-RUN-YELL ({userFriendly800PhoneLink})";
        }

        #region Helper method for other methods in this class
        private static MarkupString GetPhoneLink(string phoneNumber)
        {
            return HyperlinkHelper.PhoneNumberHelper.GetPhoneLink(phoneNumber);
        }
        #endregion
    }
}
