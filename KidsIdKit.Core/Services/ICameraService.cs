namespace KidsIdKit.Core.Services;

public interface ICameraService
{
    Task<CameraPhoto?> TakePhotoAsync();
    Task<CameraPhoto?> PickPhotoAsync();
}