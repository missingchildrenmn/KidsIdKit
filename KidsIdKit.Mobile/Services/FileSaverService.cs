using System.Text;
using KidsIdKit.Core.Services;

namespace KidsIdKit.Mobile.Services;

public class FileSaverService : IFileSaverService
{
    public async Task<bool> SaveFileAsync(string filename, string content)
    {
        try
        {
            var path = Path.Combine(FileSystem.AppDataDirectory, filename);
            Console.WriteLine($"Saving file to: {path}");

            //-------------------------------------------------------------------------------------------------
            // Use UTF8 encoding without BOM to preserve the base64 data URI exactly (this ensures that images
            // also get included in the generated HTML file; otherwise, they are replaced with a placeholder)
            //
            // cf. ToBase64String() call in PhotoPicker.razor
            //-------------------------------------------------------------------------------------------------
            var encoding = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false);
            await File.WriteAllTextAsync(path, content, encoding);

            Console.WriteLine("File successfully saved.");
            return File.Exists(path);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error saving file: {ex}");
        }
        return false;
    }
}
