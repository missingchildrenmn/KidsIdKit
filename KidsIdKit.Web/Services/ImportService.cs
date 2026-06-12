using KidsIdKit.Core.Data;
using KidsIdKit.Core.Services;
using Microsoft.JSInterop;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace KidsIdKit.Web.Services;

public class ImportService(
    IJSRuntime jsRuntime,
    IPinService pinService,
    IDataAccess dataAccessService,
    IFamilyStateService familyStateService)
    : ImportServiceBase(pinService, dataAccessService, familyStateService)
{
    public override async Task<string?> SelectFile()
    {
        try
        {
            // Use JavaScript interop to show file picker dialog
            var result = await jsRuntime.InvokeAsync<string>("fileImportInterop.selectFile");

            if (string.IsNullOrEmpty(result))
            {
                return null;
            }

            try
            {
                // Parse the JSON response which contains filename and content
                var fileData = JsonSerializer.Deserialize<FileImportData>(result);
                if (fileData != null && !string.IsNullOrEmpty(fileData.Content))
                {
                    return fileData.Content;
                }
            }
            catch (JsonException ex)
            {
                Console.WriteLine($"ImportService: Error parsing JSON response: {ex.Message}");
            }

            return null;
        }
        catch (JSException jsEx)
        {
            Console.WriteLine($"ImportService: JavaScript error while selecting file: {jsEx.Message}");
            return null;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"ImportService: Error selecting file: {ex.Message}");
            return null;
        }
    }
}

internal class FileImportData
{
    [JsonPropertyName("fileName")]
    public string? FileName { get; set; }
    [JsonPropertyName("content")]
    public string? Content { get; set; }
}

