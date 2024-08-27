using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
