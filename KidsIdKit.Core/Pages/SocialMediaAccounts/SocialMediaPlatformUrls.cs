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

        return platform switch
        {
            "Facebook" => $"https://www.facebook.com/{encoded}",
            "Instagram" => $"https://www.instagram.com/{encoded}",
            "Snapchat" => $"https://www.snapchat.com/add/{encoded}",
            "TikTok" => $"https://www.tiktok.com/@{encoded}",
            "YouTube" => $"https://www.youtube.com/@{encoded}",
            "X (Twitter)" => $"https://x.com/{encoded}",
            "Reddit" => $"https://www.reddit.com/user/{encoded}",
            "Pinterest" => $"https://www.pinterest.com/{encoded}",
            "LinkedIn" => $"https://www.linkedin.com/in/{encoded}",
            "Twitch" => $"https://www.twitch.tv/{encoded}",
            "Tumblr" => $"https://{encoded}.tumblr.com",
            "Threads" => $"https://www.threads.net/@{encoded}",
            "BeReal" => $"https://bere.al/{encoded}",
            "Roblox" => $"https://www.roblox.com/users/profile?username={encoded}",
            "Telegram" => $"https://t.me/{encoded}",
            "WhatsApp" => BuildWhatsAppUrl(handle),
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
