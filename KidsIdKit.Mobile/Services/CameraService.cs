using KidsIdKit.Core.Services;
using System.Threading.Tasks;

namespace KidsIdKit.Mobile.Services;

public class CameraService : ICameraService
{
    // Service to bridge .NET MAUI and Blazor for camera access
    public async Task<byte[]> TakePhotoAsync()
    {
        //var photo = await CapturePhotoAsync();
        //if (photo != null)
        //{
        //    using var stream = await photo.OpenReadAsync();
        //    using var memoryStream = new MemoryStream();
        //    await stream.CopyToAsync(memoryStream);
        //    return memoryStream.ToArray();
        //}

        return Array.Empty<byte>();
    }

    //private async Task<FileResult?> CapturePhotoAsync()
    //{
    //    if (MediaPicker.Default.IsCaptureSupported)
    //    {
    //        FileResult photo;
    //        try
    //        {
    //            photo = await MediaPicker.Default.CapturePhotoAsync(new MediaPickerOptions
    //            {
    //                Title = $"photo_{DateTime.Now:yyyyMMdd_HHmmss}.jpg"
    //            });
    //        }
    //        catch (Exception ex)
    //        {
    //            // Handle any exceptions that may occur during photo capture
    //            Console.WriteLine($"Error capturing photo: {ex}");
    //            return null;

    //            // return photo;
    //            //if (photo == null)
    //            //    return null;
    //        }

    //        return photo;
    //    }
    //    else
    //    {
    //        // Capture not supported on this device
    //        return null;
    //    }
    //}
}
