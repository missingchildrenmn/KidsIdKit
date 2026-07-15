using KidsIdKit.Core.SharedComponents;

namespace KidsIdKit.Core.Pages.SocialMediaAccounts;

/// <summary>
/// Maps a social media platform name to a brand-styled icon: its logo glyph on a
/// tile painted in the platform's real brand color/gradient, so a platform can be
/// shown the way the apps look on the web. Shared by the Social Media Accounts
/// list page and the account details combobox. Platforms with no official
/// Ionicons brand logo use a representative glyph; unknown/free-text values
/// return <c>null</c> (no icon).
/// </summary>
public static class SocialMediaPlatformIcons
{
    public static ComboOptionIcon? Get(string? platform) => platform switch
    {
        "Facebook" => new("logo-facebook", "#1877F2"),
        "Instagram" => new("logo-instagram",
            "radial-gradient(circle at 30% 107%, #fdf497 0%, #fdf497 5%, #fd5949 45%, #d6249f 60%, #285AEB 90%)"),
        "Snapchat" => new("logo-snapchat", "#FFFC00", "#000000"),
        "TikTok" => new("logo-tiktok", "#000000"),
        "YouTube" => new("logo-youtube", "#FF0000"),
        "X (Twitter)" => new("logo-twitter", "#000000"),
        "Discord" => new("logo-discord", "#5865F2"),
        "Reddit" => new("logo-reddit", "#FF4500"),
        "Pinterest" => new("logo-pinterest", "#E60023"),
        "LinkedIn" => new("logo-linkedin", "#0A66C2"),
        "WhatsApp" => new("logo-whatsapp", "#25D366"),
        "Twitch" => new("logo-twitch", "#9146FF"),
        "Tumblr" => new("logo-tumblr", "#001935"),
        // No Ionicons brand glyph exists for these; use a representative icon
        // with the platform's brand color.
        "Threads" => new("at-outline", "#000000"),
        "BeReal" => new("camera-outline", "#000000"),
        "Roblox" => new("game-controller-outline", "#E2231A"),
        "Telegram" => new("paper-plane-outline", "#26A5E4"),
        _ => null
    };
}
