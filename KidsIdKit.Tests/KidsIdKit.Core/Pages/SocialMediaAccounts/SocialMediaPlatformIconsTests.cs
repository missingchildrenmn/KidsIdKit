using KidsIdKit.Core.Pages.SocialMediaAccounts;
using KidsIdKit.Core.SharedComponents;

namespace KidsIdKit.Tests.KidsIdKit.Core.Pages.SocialMediaAccounts;

public class SocialMediaPlatformIconsTests
{
    [Theory]
    [InlineData("Facebook", "logo-facebook", "#1877F2")]
    [InlineData("Instagram", "logo-instagram", "radial-gradient(circle at 30% 107%, #fdf497 0%, #fdf497 5%, #fd5949 45%, #d6249f 60%, #285AEB 90%)")]
    [InlineData("Snapchat", "logo-snapchat", "#FFFC00", "#000000")]
    [InlineData("TikTok", "logo-tiktok", "#000000")]
    [InlineData("YouTube", "logo-youtube", "#FF0000")]
    [InlineData("X (Twitter)", "logo-twitter", "#000000")]
    [InlineData("Discord", "logo-discord", "#5865F2")]
    [InlineData("Reddit", "logo-reddit", "#FF4500")]
    [InlineData("Pinterest", "logo-pinterest", "#E60023")]
    [InlineData("LinkedIn", "logo-linkedin", "#0A66C2")]
    [InlineData("WhatsApp", "logo-whatsapp", "#25D366")]
    [InlineData("Twitch", "logo-twitch", "#9146FF")]
    [InlineData("Tumblr", "logo-tumblr", "#001935")]
    [InlineData("Threads", "at-outline", "#000000")]
    [InlineData("BeReal", "camera-outline", "#000000")]
    [InlineData("Roblox", "game-controller-outline", "#E2231A")]
    [InlineData("Telegram", "paper-plane-outline", "#26A5E4")]
    public void Get_KnownPlatform_IgnoresCaseAndSurroundingWhitespace(
        string platform, string glyph, string background, string foreground = "#ffffff")
    {
        var expected = new ComboOptionIcon(glyph, background, foreground);

        Assert.Equal(expected, SocialMediaPlatformIcons.Get(platform));
        Assert.Equal(expected, SocialMediaPlatformIcons.Get(platform.ToLowerInvariant()));
        Assert.Equal(expected, SocialMediaPlatformIcons.Get(platform.ToUpperInvariant()));
        Assert.Equal(expected, SocialMediaPlatformIcons.Get($" \t{platform}\r\n"));
        Assert.Equal(expected, SocialMediaPlatformIcons.Get($" \t{platform.ToUpperInvariant()}\r\n"));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" \t\r\n")]
    [InlineData(" Unknown ")]
    public void Get_MissingOrUnknownPlatform_ReturnsNull(string? platform)
    {
        Assert.Null(SocialMediaPlatformIcons.Get(platform));
    }

    [Fact]
    public void Get_MixedCasePlatform_ReturnsBrandIcon()
    {
        Assert.Equal(new ComboOptionIcon("logo-facebook", "#1877F2"), SocialMediaPlatformIcons.Get(" fAcEbOoK "));
    }
}
