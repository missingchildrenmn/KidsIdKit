namespace KidsIdKit.Tests.Utilities
{
    public static class HtmlHelper
    {
        public static string GetInfoPageLink()
        {
            char doubleQuote = (char)34;

            return $@"
                <div>
                    <a href={doubleQuote}/info{doubleQuote} class={doubleQuote}link-dark{doubleQuote}>Back</a>
                </div>";
        }
    }
}
