using Microsoft.AspNetCore.Components;

namespace KidsIdKit.Shared.Pages.Info
{
    public static class HyperlinkHelper
    {
        static readonly string minnesotaLegislatureOfficeOfTheRevisorOfStatutesUrl = "https://www.revisor.mn.gov/statutes/";

        public static MarkupString SetEmailLink(string emailAddress)
        {
            return (MarkupString)$"<a href='mailto:{emailAddress}'>{emailAddress}</a>";
        }

        public class PhoneNumberHelper
        {
            public static MarkupString GetPhoneLink(string userFriendlyPhoneNumber, string realPhoneNumber)
            {
                var callablePhoneNumber = CallablePhoneNumber(realPhoneNumber);
                var phoneLink = $"<a href='tel:{callablePhoneNumber}'>{userFriendlyPhoneNumber}</a>";
                return (MarkupString)phoneLink;
            }

            public static MarkupString GetPhoneLink(string userFriendlyPhoneNumber)
            {
                var callablePhoneNumber = CallablePhoneNumber(userFriendlyPhoneNumber);
                var phoneLink = $"<a href='tel:{callablePhoneNumber}'>{userFriendlyPhoneNumber}</a>";
                return (MarkupString)phoneLink;
                //return GetPhoneLink(userFriendlyPhoneNumber, callablePhoneNumber);
            }

            //public static MarkupString GetPhoneLink(string numericOnlyPhoneNumber, string vanityPhoneNumber) {
            //    var phoneLink = LinkHelper.Link("tel", vanityPhoneNumber, numericOnlyPhoneNumber);
            //    return (MarkupString)phoneLink;
            //}

            private static string CallablePhoneNumber(string input)
            {
                return "+1" + new string(input.Where(char.IsDigit).ToArray());
            }
        }

        public class LinkHelper
        {
            public static MarkupString HtmlLink(string text, string url)
            {
                var link = (MarkupString)Link(text, url);
                return link;
            }

            // TODO: Art note: I believe the following Link method can be removed ...
            public static string Link(string text, string url)
            {
                return Link(String.Empty, text, url);
            }

            // Art note: ... and the 2 references to "type" can be removed from the following Link() method.
            private static string Link(string type, string text, string url)
            {
                var link = $"<a href='{type}{(String.IsNullOrEmpty(type) ? String.Empty : ":")}{url}'>{text}</a>";
                return link;
            }
        }

        public static string MinnesotaStateStatuteDetailsUrl(string minnesotaStateStatute)                     // https://www.revisor.mn.gov/statutes/?id=nnn.nn[n]
        {
            return $"{minnesotaLegislatureOfficeOfTheRevisorOfStatutesUrl}?id={minnesotaStateStatute}";
        }
    }
}
