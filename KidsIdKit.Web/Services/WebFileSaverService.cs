using KidsIdKit.Shared.Services;
using Microsoft.JSInterop;
using System.Text;

namespace KidsIdKit.Web.Services;

public class WebFileSaverService : IFileSaverService
{
    private readonly IJSRuntime _jsRuntime;

    public WebFileSaverService(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    public async Task<bool> SaveFileAsync(string filename, string content)
    {
        try
        {
            // Use JavaScript interop to trigger file download with plain text content
            await _jsRuntime.InvokeVoidAsync("downloadFileFromText", filename, content);
            
            Console.WriteLine($"WebFileSaverService: Successfully triggered download for file '{filename}' with content length {content.Length}");
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"WebFileSaverService: Failed to save file '{filename}': {ex.Message}");
            return false;
        }
    }
}