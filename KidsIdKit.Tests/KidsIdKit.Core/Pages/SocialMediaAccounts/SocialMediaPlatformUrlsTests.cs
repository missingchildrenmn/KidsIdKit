using KidsIdKit.Core.Pages.SocialMediaAccounts;

namespace KidsIdKit.Tests.KidsIdKit.Core.Pages.SocialMediaAccounts;

public class SocialMediaPlatformUrlsTests
{
    [Theory]
    [InlineData("Facebook", "https://www.facebook.com/123")]
    [InlineData("Instagram", "https://www.instagram.com/123")]
    [InlineData("Snapchat", "https://www.snapchat.com/add/123")]
    [InlineData("TikTok", "https://www.tiktok.com/@123")]
    [InlineData("YouTube", "https://www.youtube.com/@123")]
    [InlineData("X (Twitter)", "https://x.com/123")]
    [InlineData("Reddit", "https://www.reddit.com/user/123")]
    [InlineData("Pinterest", "https://www.pinterest.com/123")]
    [InlineData("LinkedIn", "https://www.linkedin.com/in/123")]
    [InlineData("Twitch", "https://www.twitch.tv/123")]
    [InlineData("Tumblr", "https://123.tumblr.com")]
    [InlineData("Threads", "https://www.threads.net/@123")]
    [InlineData("BeReal", "https://bere.al/123")]
    [InlineData("Roblox", "https://www.roblox.com/users/profile?username=123")]
    [InlineData("Telegram", "https://t.me/123")]
    [InlineData("WhatsApp", "https://wa.me/123")]
    public void GetProfileUrl_KnownPlatform_IgnoresCaseAndSurroundingWhitespace(string platform, string expected)
    {
        Assert.Equal(expected, SocialMediaPlatformUrls.GetProfileUrl(platform, "123"));
        Assert.Equal(expected, SocialMediaPlatformUrls.GetProfileUrl(platform.ToLowerInvariant(), "123"));
        Assert.Equal(expected, SocialMediaPlatformUrls.GetProfileUrl(platform.ToUpperInvariant(), "123"));
        Assert.Equal(expected, SocialMediaPlatformUrls.GetProfileUrl($" \t{platform.ToUpperInvariant()}\r\n", "123"));
    }

    [Theory]
    [InlineData(null, "pat")]
    [InlineData(" \t", "pat")]
    [InlineData(" Unknown ", "pat")]
    [InlineData(" DISCORD ", "pat")]
    [InlineData(" facebook ", null)]
    [InlineData(" facebook ", " \t")]
    [InlineData(" facebook ", "@@")]
    [InlineData(" whatsapp ", "pat")]
    public void GetProfileUrl_UnsupportedPlatformOrMissingHandle_ReturnsNull(string? platform, string? userName)
    {
        Assert.Null(SocialMediaPlatformUrls.GetProfileUrl(platform, userName));
    }

    [Theory]
    [InlineData(" FaCeBoOk ", " @pat name ", "https://www.facebook.com/pat%20name")]
    [InlineData(" WhAtSaPp ", "+1 (234) 567-8900", "https://wa.me/12345678900")]
    public void GetProfileUrl_NormalizedPlatform_PreservesHandleFormatting(string platform, string userName, string expected)
    {
        Assert.Equal(expected, SocialMediaPlatformUrls.GetProfileUrl(platform, userName));
    }
}
