namespace KidsIdKit.Shared.Services;

public interface IFileSaverService
{
    Task<bool> SaveFileAsync(string filename, string content);
}
