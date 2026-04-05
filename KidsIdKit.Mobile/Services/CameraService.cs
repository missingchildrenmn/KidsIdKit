using KidsIdKit.Core.Services;

namespace KidsIdKit.Mobile.Services;

/// <summary>
/// Camera service using MAUI MediaPicker for photo capture and selection.
/// Suppresses session locking while native pickers are open to prevent
/// the Blazor component tree from being torn down on Android.
/// </summary>
public class CameraService(ISessionService sessionService) : ICameraService
{
    public async Task<byte[]?> TakePhotoAsync()
    {
        var result = await RunWithLockSuppressed(() => CapturePhotoAsync());
        return await ReadFileResultAsync(result);
    }

    public async Task<byte[]?> PickPhotoAsync()
    {
        var result = await RunWithLockSuppressed(async () =>
        {
            var results = await MediaPicker.Default.PickPhotosAsync();
            return results?.FirstOrDefault();
        });

        return await ReadFileResultAsync(result);
    }

    private async Task<T?> RunWithLockSuppressed<T>(Func<Task<T?>> operation) where T : class
    {
        sessionService.BeginSuppressLock();
        try
        {
            return await operation();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in picker operation: {ex.Message}");
            return null;
        }
        finally
        {
            sessionService.EndSuppressLock();
        }
    }

    private static async Task<byte[]?> ReadFileResultAsync(FileResult? fileResult)
    {
        if (fileResult == null)
        {
            return null;
        }

        using var stream = await fileResult.OpenReadAsync();
        using var memoryStream = new MemoryStream();
        await stream.CopyToAsync(memoryStream);
        return memoryStream.ToArray();
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
