namespace KidsIdKit.Core.Services;

/// <summary>
/// Represents a photo captured or picked from the device camera/gallery.
/// Carries both the raw image bytes and the content type reported by the platform,
/// allowing callers to produce correct data URIs and file extensions for any format
/// the device returns (JPEG, PNG, HEIC, etc.).
/// </summary>
public record CameraPhoto(byte[] Bytes, string ContentType);
