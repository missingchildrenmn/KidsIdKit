namespace KidsIdKit.Core.Services;

public interface IFileSaverService
{
    Task<bool> SaveFileAsync(string filename, string content);
    Task<bool> SaveFileAsync(string filename, byte[] content);
}
