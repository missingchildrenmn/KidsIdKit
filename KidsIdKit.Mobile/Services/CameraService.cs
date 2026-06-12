using KidsIdKit.Core.Services;
using Microsoft.Maui.Graphics.Platform;

namespace KidsIdKit.Mobile.Services;

/// <summary>
/// Camera service using MAUI MediaPicker for photo capture and selection.
/// Suppresses session locking while native pickers are open to prevent
/// the Blazor component tree from being torn down on Android.
/// </summary>
public class CameraService() : ICameraService
{
    private const int MaxImageDimension = 1920;

    public async Task<CameraPhoto?> TakePhotoAsync()
    {
        var result = await RunOperation(() => CapturePhotoAsync());
        return await ReadFileResultAsync(result);
    }

    public async Task<CameraPhoto?> PickPhotoAsync()
    {
        var result = await RunOperation(async () =>
        {
            var results = await MediaPicker.Default.PickPhotosAsync();
            return await ReadFileResultAsync(results?.FirstOrDefault());
        });

        return result;
    }

    private async Task<T?> RunOperation<T>(Func<Task<T?>> operation) where T : class
    {
        try
        {
            return await operation();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in picker operation: {ex.Message}");
            return null;
        }
    }

    private async Task<CameraPhoto?> ReadFileResultAsync(FileResult? fileResult)
    {
        if (fileResult == null)
        {
            return null;
        }

        try
        {
            using var stream = await fileResult.OpenReadAsync();

            // Load the image to check dimensions
            var imageBytes = await ReadStreamToBytes(stream);

            // Attempt to resize if needed
            var resizedBytes = await ResizeImageIfNeededAsync(imageBytes, fileResult.ContentType);

            return new CameraPhoto(resizedBytes, fileResult.ContentType ?? "image/jpeg");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error processing image: {ex.Message}");
            return null;
        }
    }

    private async Task<byte[]> ReadStreamToBytes(Stream stream)
    {
        using var memoryStream = new MemoryStream();
        await stream.CopyToAsync(memoryStream);
        return memoryStream.ToArray();
    }

    private async Task<byte[]> ResizeImageIfNeededAsync(byte[] imageBytes, string contentType)
    {
        try
        {
            using var originalStream = new MemoryStream(imageBytes);


            var image = PlatformImage.FromStream(originalStream);

            if (image == null)
            {
                Console.WriteLine("Failed to load image, returning original bytes");
                return imageBytes;
            }

            var width = image.Width;
            var height = image.Height;

            if (width <= MaxImageDimension && height <= MaxImageDimension)
            {
                Console.WriteLine($"Image size {width}x{height} is within limits, no resize needed");
                return imageBytes;
            }

            var resizedImage = image.Downsize(MaxImageDimension, true);

            using MemoryStream outputStream = new MemoryStream();

            await resizedImage.SaveAsync(outputStream);
            
            var resizedBytes = outputStream.ToArray();

            Console.WriteLine($"Image resized from {imageBytes.Length} bytes to {resizedBytes.Length} bytes");

            return resizedBytes;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error resizing image: {ex.Message}");
            // Return original bytes if resize fails
            return imageBytes;
        }
    }

    private static async Task<FileResult?> CapturePhotoAsync()
    {
        if (!MediaPicker.Default.IsCaptureSupported)
        {
            return null;
        }

        return await MediaPicker.Default.CapturePhotoAsync(new MediaPickerOptions
        {
            Title = $"photo_{DateTime.Now:yyyyMMdd_HHmmss}.jpg"
        });
    }
}
