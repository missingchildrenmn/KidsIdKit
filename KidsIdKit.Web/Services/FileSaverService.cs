using KidsIdKit.Shared.Services;
using Microsoft.JSInterop;
using System.Text;

namespace KidsIdKit.Web.Services;

public class FileSaverService : IFileSaverService
{
    private readonly IJSRuntime _jsRuntime;

    public FileSaverService(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    public async Task<bool> SaveFileAsync(string filename, string content)
    {
        try
        {
            Console.WriteLine($"WebFileSaverService: Attempting to download file '{filename}' with content length {content.Length}");
            
            // Use JavaScript interop to trigger file download with plain text content
            await _jsRuntime.InvokeVoidAsync("downloadFileFromText", filename, content);
            
            Console.WriteLine($"WebFileSaverService: Successfully triggered download for file '{filename}'");
            return true;
        }
        catch (JSException jsEx)
        {
            Console.WriteLine($"WebFileSaverService: JavaScript error while saving file '{filename}': {jsEx.Message}");
            return false;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"WebFileSaverService: Failed to save file '{filename}': {ex.Message}");
            return false;
        }
    }
}