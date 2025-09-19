using KidsIdKit.Shared.Services;

namespace KidsIdKit.Web.Services;

public class WebFileSaverService : IFileSaverService
{
    public async Task<bool> SaveFileAsync(string filename, string content)
    {
        // In a web context, we can't save files to the local file system
        // This could be extended to save to browser storage or trigger a download
        // For now, we'll just simulate success
        await Task.CompletedTask;
        Console.WriteLine($"WebFileSaverService: Would save file '{filename}' with content length {content.Length}");
        return true;
    }
}