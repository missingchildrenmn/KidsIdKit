using Microsoft.AspNetCore.Components;

namespace KidsIdKit.Core.Pages.Information
{
    public static class HyperlinkHelper
    {
        static readonly string minnesotaLegislatureOfficeOfTheRevisorOfStatutesUrl = "https://www.revisor.mn.gov/statutes/";

        public static MarkupString SetEmailLink(string emailAddress)
        {
            string emailIcon = "<i class='fa-solid fa-envelope-open-text'></i>";
            return (MarkupString)$"{emailIcon} <a href='mailto:{emailAddress}'>{emailAddress}</a>";
        }

        public class PhoneNumberHelper
        {
            public static MarkupString GetPhoneIcon()
            {
                string phoneIcon = "<i class='fa-solid fa-phone'></i>";
                return (MarkupString)phoneIcon;
            }

            public static MarkupString GetPhoneLink(string userFriendlyPhoneNumber)
            {
                var callablePhoneNumber = CallablePhoneNumber(userFriendlyPhoneNumber);
                var phoneLink = $"{GetPhoneIcon()} <a href='tel:{callablePhoneNumber}'>{userFriendlyPhoneNumber}</a>";
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

        public static string MinnesotaStateStatuteDetailsUrl(string minnesotaStateStatute)
        {
            return $"{minnesotaLegislatureOfficeOfTheRevisorOfStatutesUrl}?id={minnesotaStateStatute}";
        }
    }
}
