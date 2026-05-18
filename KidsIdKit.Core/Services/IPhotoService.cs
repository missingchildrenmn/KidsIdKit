using KidsIdKit.Core.Data;
using Microsoft.AspNetCore.Components.Forms;

namespace KidsIdKit.Core.Services;

/// <summary>
/// Provides photo acquisition helpers used by photo-related UI components.
/// </summary>
public interface IPhotoService
{
    /// <summary>
    /// Indicates whether a native camera/picker (<see cref="ICameraService"/>) is available
    /// in the current host. When false, callers should fall back to an HTML file input.
    /// </summary>
    bool IsCameraAvailable { get; }

    /// <summary>
    /// Picks a photo using the registered <see cref="ICameraService"/> and returns it as a
    /// <see cref="Photo"/> with a base64-encoded data URI image source.
    /// Returns <c>null</c> if the user cancels or no camera service is available.
    /// </summary>
    Task<Photo?> PickPhotoFromCameraAsync();

    /// <summary>
    /// Captures a new photo using the device camera via the registered <see cref="ICameraService"/>
    /// and returns it as a <see cref="Photo"/> with a base64-encoded data URI image source.
    /// Returns <c>null</c> if the user cancels, capture is unsupported, or no camera service is available.
    /// </summary>
    Task<Photo?> TakePhotoFromCameraAsync();

    /// <summary>
    /// Reads the supplied browser file, optionally resizing it, and returns a <see cref="Photo"/>
    /// whose <see cref="Photo.ImageSource"/> is a base64-encoded data URI.
    /// </summary>
    Task<Photo> CreatePhotoFromBrowserFileAsync(IBrowserFile file, int maxWidth = 200, int maxHeight = 200);
}
