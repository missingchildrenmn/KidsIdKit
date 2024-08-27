using Microsoft.AspNetCore.Components;

namespace KidsIdKit.Pages.info
{
    public static class HyperlinkHelper
    {
        public static MarkupString SetEmailLink(string emailAddress)
        {
            return (MarkupString)$"<a href='mailto:{emailAddress}'>{emailAddress}</a>";
        }

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
}
