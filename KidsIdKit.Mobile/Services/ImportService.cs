using KidsIdKit.Core.Data;
using KidsIdKit.Core.Services;

namespace KidsIdKit.Mobile.Services;

public class ImportService : ImportServiceBase
{
    public ImportService(
        IPinService pinService,
        IDataAccess dataAccessService) : base(pinService, dataAccessService)
    {
    }

    public override async Task<string?> SelectFile()
    {
        string? returnValue = null;
        try
        {
            var result = await FilePicker.Default.PickAsync(new PickOptions
            {
                PickerTitle = "Select the File to Import"
            });

            if (result != null)
            {
                // Process the selected file
                returnValue = await LoadXmlAsync(result.FullPath);
                Console.WriteLine($"Selected file for import: {returnValue}");
            }
        }
        catch (Exception ex)
        {
            // Handle cancellation or errors
            Console.WriteLine($"Error importing file: {ex.Message}");
        }

        return returnValue;
    }

    private async Task<string?> LoadXmlAsync(string filename)
    {
        try
        {
            if (string.IsNullOrEmpty(filename))
            {
                Console.WriteLine($"Filename cannot be null or empty: {filename}");
                return null;
            }

            if (!File.Exists(filename))
            {
                Console.WriteLine($"The file '{filename}' was not found.");
                return null;
            }

            string xmlContent = await File.ReadAllTextAsync(filename);

            return xmlContent;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"ImportService: Error loading '{filename}': {ex.Message}");
            return null;
        }
    }
}
