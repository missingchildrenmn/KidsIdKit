namespace KidsIdKit.Core.Services;

public interface ICameraService
{
    Task<byte[]?> TakePhotoAsync();
}
