using System;
using System.Text.RegularExpressions;

namespace KidsIdKit.Core.Pages.SocialMediaAccounts;

/// <summary>
/// Builds the public profile URL for a social media account from its platform
/// name and username, so the platform icon can link straight to the child's
/// profile. Shared by the Social Media Accounts list page and the exported PDF
/// so both link to the same place. Returns <c>null</c> when a profile URL can't
/// be built from a username (unknown/free-text platform, a missing username, or
/// a platform such as Discord that has no username-based public web profile), in
/// which case no link should be shown.
/// </summary>
public static class SocialMediaPlatformUrls
{
    public static string? GetProfileUrl(string? platform, string? userName)
    {
        if (string.IsNullOrWhiteSpace(platform) || string.IsNullOrWhiteSpace(userName))
        {
            return null;
        }

        // Users often store the handle with a leading '@'; strip it so it isn't
        // doubled in URLs that already include one.
        var handle = userName.Trim().TrimStart('@');
        if (handle.Length == 0)
        {
            return null;
        }

        // Usernames are almost always URL-safe, but encode as a safety net so an
        // unusual character can't break the resulting path.
        var encoded = Uri.EscapeDataString(handle);
        var normalizedPlatform = platform.Trim().ToUpperInvariant();

        return normalizedPlatform switch
        {
            "FACEBOOK" => $"https://www.facebook.com/{encoded}",
            "INSTAGRAM" => $"https://www.instagram.com/{encoded}",
            "SNAPCHAT" => $"https://www.snapchat.com/add/{encoded}",
            "TIKTOK" => $"https://www.tiktok.com/@{encoded}",
            "YOUTUBE" => $"https://www.youtube.com/@{encoded}",
            "X (TWITTER)" => $"https://x.com/{encoded}",
            "REDDIT" => $"https://www.reddit.com/user/{encoded}",
            "PINTEREST" => $"https://www.pinterest.com/{encoded}",
            "LINKEDIN" => $"https://www.linkedin.com/in/{encoded}",
            "TWITCH" => $"https://www.twitch.tv/{encoded}",
            "TUMBLR" => $"https://{encoded}.tumblr.com",
            "THREADS" => $"https://www.threads.net/@{encoded}",
            "BEREAL" => $"https://bere.al/{encoded}",
            "ROBLOX" => $"https://www.roblox.com/users/profile?username={encoded}",
            "TELEGRAM" => $"https://t.me/{encoded}",
            "WHATSAPP" => BuildWhatsAppUrl(handle),
            // Discord identifies profiles by numeric user id, not username, so
            // there is no reliable username-based web profile URL to build.
            _ => null
        };
    }

    // WhatsApp's click-to-chat link (wa.me) expects an international phone number
    // in digits only, so distill whatever the username holds down to its digits.
    private static string? BuildWhatsAppUrl(string handle)
    {
        var digits = Regex.Replace(handle, "[^0-9]", string.Empty);
        return digits.Length == 0 ? null : $"https://wa.me/{digits}";
    }
}
