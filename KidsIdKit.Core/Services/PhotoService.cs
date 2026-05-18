using KidsIdKit.Core.Data;
using Microsoft.AspNetCore.Components.Forms;

namespace KidsIdKit.Core.Services;

/// <summary>
/// Default <see cref="IPhotoService"/> implementation. Uses the optional
/// <see cref="ICameraService"/> when available and otherwise relies on
/// browser file input streams.
/// </summary>
public class PhotoService : IPhotoService
{
    private readonly ICameraService? _cameraService;
    private readonly ISessionService _sessionService;

    public PhotoService(IServiceProvider serviceProvider, ISessionService sessionService)
    {
        ArgumentNullException.ThrowIfNull(serviceProvider);
        _sessionService = sessionService ?? throw new ArgumentNullException(nameof(sessionService));
        _cameraService = serviceProvider.GetService(typeof(ICameraService)) as ICameraService;
    }

    public bool IsCameraAvailable => _cameraService != null;

    public Task<Photo?> PickPhotoFromCameraAsync()
        => CreatePhotoFromCameraAsync(_cameraService is null ? null : _cameraService.PickPhotoAsync);

    public Task<Photo?> TakePhotoFromCameraAsync()
        => CreatePhotoFromCameraAsync(_cameraService is null ? null : _cameraService.TakePhotoAsync);

    private async Task<Photo?> CreatePhotoFromCameraAsync(Func<Task<CameraPhoto?>>? source)
    {
        if (source == null)
        {
            return null;
        }

        // Suppress session locking around the entire camera operation, including the
        // post-capture decoding below. CameraService also suppresses internally, but it
        // releases the suppression before the captured file has been read; on Android,
        // a transient OnStop fired during that window can otherwise lock the session
        // and bounce the user to the PIN entry page.
        _sessionService.BeginSuppressLock();
        try
        {
            var cameraPhoto = await source();
            if (cameraPhoto == null)
            {
                return null;
            }

            var extension = ContentTypeToExtension(cameraPhoto.ContentType);
            return new Photo
            {
                FileName = $"photo_{DateTime.Now:yyyyMMdd_HHmmss}.{extension}",
                ImageSource = $"data:{cameraPhoto.ContentType};base64,{Convert.ToBase64String(cameraPhoto.Bytes)}",
            };
        }
        finally
        {
            _sessionService.EndSuppressLock();
        }
    }

    private static string ContentTypeToExtension(string contentType) => contentType switch
    {
        "image/png" => "png",
        "image/gif" => "gif",
        "image/webp" => "webp",
        "image/heic" => "heic",
        "image/heif" => "heif",
        _ => "jpg",
    };

    // Protected virtual so tests can override the JS-backed resize without a real browser context.
    protected virtual ValueTask<IBrowserFile> RequestImageFileAsync(IBrowserFile file, string format, int maxWidth, int maxHeight)
        => file.RequestImageFileAsync(format, maxWidth, maxHeight);

    public async Task<Photo> CreatePhotoFromBrowserFileAsync(IBrowserFile file, int maxWidth = 200, int maxHeight = 200)
    {
        ArgumentNullException.ThrowIfNull(file);

        var photoFile = await RequestImageFileAsync(file, file.ContentType, maxWidth, maxHeight);
        using var stream = photoFile.OpenReadStream();
        using var ms = new MemoryStream();
        await stream.CopyToAsync(ms);

        return new Photo
        {
            FileName = file.Name,
            ImageSource = $"data:{photoFile.ContentType};base64,{Convert.ToBase64String(ms.ToArray())}",
        };
    }
}

