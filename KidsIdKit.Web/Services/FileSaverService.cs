using KidsIdKit.Core.Services;
using Microsoft.JSInterop;

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

    public async Task<bool> SaveFileAsync(string filename, byte[] content)
    {
        try
        {
            Console.WriteLine($"WebFileSaverService: Attempting to download binary file '{filename}' with {content.Length} bytes");

            // Reuse the existing downloadFileFromStream JS helper which expects a base64 payload.
            var base64 = Convert.ToBase64String(content);
            await _jsRuntime.InvokeVoidAsync("downloadFileFromStream", filename, base64, "application/pdf");

            Console.WriteLine($"WebFileSaverService: Successfully triggered download for binary file '{filename}'");
            return true;
        }
        catch (JSException jsEx)
        {
            Console.WriteLine($"WebFileSaverService: JavaScript error while saving binary file '{filename}': {jsEx.Message}");
            return false;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"WebFileSaverService: Failed to save binary file '{filename}': {ex.Message}");
            return false;
        }
    }
}
