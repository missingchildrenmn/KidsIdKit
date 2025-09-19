using KidsIdKit.Shared.Services;
using Microsoft.JSInterop;

namespace KidsIdKit.Web.Services;

public class WebFileSharerService : IFileSharerService
{
    private readonly IJSRuntime _jsRuntime;

    public WebFileSharerService(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    public async Task ShareFileAsync(string filename)
    {
        try
        {
            Console.WriteLine($"WebFileSharerService: Attempting to share file '{filename}'");
            
            // Check if the Web Share API is available
            var canShare = await _jsRuntime.InvokeAsync<bool>("canShareFiles");
            
            if (canShare)
            {
                // Use the Web Share API to share the file information
                await _jsRuntime.InvokeVoidAsync("shareFile", filename);
                Console.WriteLine($"WebFileSharerService: Shared file '{filename}' using Web Share API");
            }
            else
            {
                // Fallback: Show a simple alert or notification
                await _jsRuntime.InvokeVoidAsync("alert", $"File '{filename}' has been downloaded. Check your Downloads folder.");
                Console.WriteLine($"WebFileSharerService: Notified user about downloaded file '{filename}'");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"WebFileSharerService: Error sharing file '{filename}': {ex.Message}");
            // Fallback to simple alert
            try
            {
                await _jsRuntime.InvokeVoidAsync("alert", $"File '{filename}' has been downloaded.");
            }
            catch
            {
                // If even alert fails, just log
                Console.WriteLine($"WebFileSharerService: Could not show notification for file '{filename}'");
            }
        }
    }
}