namespace KidsIdKit.Tests.Utilities
{
    public static class HtmlHelper
    {
        public static string GetInfoPageLink()
        {
            return $@"
                <div>
                    <a href=""/Info"" class=""link-dark"">Back</a>
                </div>";
        }
    }
}
