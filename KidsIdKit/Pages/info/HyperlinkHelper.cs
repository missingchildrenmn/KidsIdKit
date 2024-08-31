using Microsoft.AspNetCore.Components;

namespace KidsIdKit.Pages.info
{
    public static class HyperlinkHelper
    {
        public static MarkupString SetEmailLink(string emailAddress)
        {
            return (MarkupString)$"<a href='mailto:{emailAddress}'>{emailAddress}</a>";
        }

        public class PhoneNumberHelper
        {
            public static MarkupString GetPhoneLink(string cpiapStateDepartment)
            {
                var callablePhoneNumber = CallablePhoneNumber(cpiapStateDepartment);
                var phoneLink = $"<a href='tel:{callablePhoneNumber}'>{cpiapStateDepartment}</a>";
                return (MarkupString)phoneLink;
            }

            private static string CallablePhoneNumber(string input)
            {
                return "+1" + new string(input.Where(char.IsDigit).ToArray());
            }
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
}
