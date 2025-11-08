namespace KidsIdKit.Shared.Services;

public interface ICameraService
{
    Task<byte[]?> TakePhotoAsync();
}
