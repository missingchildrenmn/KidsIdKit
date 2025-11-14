namespace KidsIdKit.Core.Services;

public interface IFileSharerService
{
    Task ShareFileAsync(string filename);
}
