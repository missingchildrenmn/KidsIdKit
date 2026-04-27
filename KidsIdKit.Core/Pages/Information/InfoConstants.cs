using Microsoft.AspNetCore.Components;

namespace KidsIdKit.Core.Pages.Information
{
    public static class InfoConstants
    {
        public static readonly MarkupString APOSTROPHE = (MarkupString)"&rsquo;";   // Right single quotation mark
        public static readonly MarkupString DOUBLE_QUOTE = (MarkupString)"&quote;";
        public static readonly MarkupString EM_DASH = (MarkupString)"&mdash;";
        public static readonly MarkupString EN_DASH = (MarkupString)"&ndash;";
        public static readonly MarkupString LEFT_DOUBLE_QUOTATION_MARK = (MarkupString)"&ldquo;";
        public static readonly MarkupString REGISTERED_TRADE_MARK_SIGN = (MarkupString)"&reg;";
        public static readonly MarkupString RIGHT_DOUBLE_QUOTATION_MARK = (MarkupString)"&rdquo;";

        public static MarkupString Append_apostrophe_and_s_to(string text)
        {
            return (MarkupString)$"{text}{APOSTROPHE}s";
        }

        public static MarkupString DoubleQuote(string text)
        {
            return (MarkupString)$"{LEFT_DOUBLE_QUOTATION_MARK}{text}{RIGHT_DOUBLE_QUOTATION_MARK}";
        }
    }
}
