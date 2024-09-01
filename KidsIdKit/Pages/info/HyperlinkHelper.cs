using Microsoft.AspNetCore.Components;

namespace KidsIdKit.Pages.info
{
    public static class HyperlinkHelper
    {
        static string minnesotaLegislatureOfficeOfTheRevisorOfStatutesUrl = "https://www.revisor.mn.gov/statutes/";

        public static MarkupString SetEmailLink(string emailAddress)
        {
            return (MarkupString)$"<a href='mailto:{emailAddress}'>{emailAddress}</a>";
        }

        public class PhoneNumberHelper
        {
            public static MarkupString GetPhoneLink(string userFriendlyPhoneNumber)
            {
                var callablePhoneNumber = CallablePhoneNumber(userFriendlyPhoneNumber);
                var phoneLink = $"<a href='tel:{callablePhoneNumber}'>{userFriendlyPhoneNumber}</a>";
                return (MarkupString)phoneLink;
            }

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

            public static string Link(string text, string url)
            {
                return Link(String.Empty, text, url);
            }

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
