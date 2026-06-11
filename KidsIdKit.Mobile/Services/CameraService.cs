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

#if ANDROID
    private Android.Graphics.Bitmap RotateIfRequired(Android.Graphics.Bitmap bitmap, Stream imageStream)
    {
        var ei = new Android.Media.ExifInterface(imageStream);
        var orientation = ei.GetAttributeInt(Android.Media.ExifInterface.TagOrientation, (int)Android.Media.Orientation.Undefined);

        return orientation switch
        {
            (int)Android.Media.Orientation.Rotate90 => Rotate(bitmap, 90),
            (int)Android.Media.Orientation.Rotate180 => Rotate(bitmap, 180),
            (int)Android.Media.Orientation.Rotate270 => Rotate(bitmap, 270),
            _ => bitmap,
        };
    }

    private Android.Graphics.Bitmap Rotate(Android.Graphics.Bitmap bitmap, int angle)
    {
        var matrix = new Android.Graphics.Matrix();
        matrix.PostRotate(angle);

        return Android.Graphics.Bitmap.CreateBitmap(bitmap, 0, 0, bitmap.Width, bitmap.Height, matrix, true);
    }

    private Task<MemoryStream> BitmapToStreamAsync(Android.Graphics.Bitmap finalImage, string contentType)
    {
        var tcs = new TaskCompletionSource<MemoryStream>();
        MemoryStream bos = new MemoryStream();
        finalImage.Compress(ContentTypeToAndroidCompressFormat(contentType)!, 100, bos);
        tcs.SetResult(bos);
        return tcs.Task;
    }

    private Android.Graphics.Bitmap.CompressFormat? ContentTypeToAndroidCompressFormat(string contentType)
    {
        return contentType switch
        {
            "image/jpeg" => Android.Graphics.Bitmap.CompressFormat.Jpeg,
            "image/jpg" => Android.Graphics.Bitmap.CompressFormat.Jpeg,
            "image/png" => Android.Graphics.Bitmap.CompressFormat.Png,
            _ => Android.Graphics.Bitmap.CompressFormat.Png
        };
    }
#endif

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
