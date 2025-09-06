using KidsIdKit.Shared.Services;

namespace KidsIdKit.Mobile.Services;

public class FileSaverService : IFileSaverService
{
    public async Task<bool> SaveFileAsync(string filename, string content)
    {
        try
        {
            var path = Path.Combine(FileSystem.AppDataDirectory, filename);
            Console.WriteLine($"Saving file to: {path}");
            await File.WriteAllTextAsync(path, content);
            Console.WriteLine("File saved successfully.");
            return File.Exists(path);
            //Console.WriteLine($"File exists after write: {exists}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error saving file: {ex}");
        }
        return false;
    }
}