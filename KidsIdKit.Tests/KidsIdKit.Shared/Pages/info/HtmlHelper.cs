namespace KidsIdKit.Tests.Utilities
{
    public static class HtmlHelper
    {
        public static string GetInfoPageLink()
        {
            return $@"
                <div>
                    <a href=""/info"" class=""link-dark"">Back</a>
                </div>";
        }
    }
}
