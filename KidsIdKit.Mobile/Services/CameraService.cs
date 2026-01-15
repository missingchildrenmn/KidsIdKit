using KidsIdKit.Core.Services;

namespace KidsIdKit.Mobile.Services;

/// <summary>
/// Camera service using MAUI MediaPicker for photo capture.
/// </summary>
public class CameraService : ICameraService
{
    public async Task<byte[]?> TakePhotoAsync()
    {
        var photo = await CapturePhotoAsync();
        if (photo == null)
        {
            return null;
        }

        using var stream = await photo.OpenReadAsync();
        using var memoryStream = new MemoryStream();
        await stream.CopyToAsync(memoryStream);
        return memoryStream.ToArray();
    }

    private async Task<FileResult?> CapturePhotoAsync()
    {
        if (!MediaPicker.Default.IsCaptureSupported)
        {
            return null;
        }

        try
        {
            return await MediaPicker.Default.CapturePhotoAsync(new MediaPickerOptions
            {
                Title = $"photo_{DateTime.Now:yyyyMMdd_HHmmss}.jpg"
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error capturing photo: {ex.Message}");
            return null;
        }
    }
}
