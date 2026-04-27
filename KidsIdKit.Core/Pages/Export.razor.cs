
using KidsIdKit.Core.Services;
using KidsIdKit.Core.SharedComponents;

namespace KidsIdKit.Core.Pages;

public partial class Export
{
    private bool ShowExportErrorAlert = false;
    private string ExportErrorMessage = string.Empty;

    private async Task ExportAllData()
    {
        try
        {
            Console.WriteLine("ExportAllData: Method called");

            var filename = $"KidsIdKitExport.xml";

            var result = await ExportService.ExportData(filename);

            if (result == IExportService.ExportResult.PinDataNotFound)
            {
                ExportErrorMessage = "PIN data was not found, export cannot be performed.";
                ShowExportErrorAlert = true;
                return;
            }

            if (result == IExportService.ExportResult.FamilyDataNotFound)
            {
                ExportErrorMessage = "No family data was found to export.";
                ShowExportErrorAlert = true;
                return;
            }

            if (result == IExportService.ExportResult.UnexpectedError)
            {
                ExportErrorMessage = "An unexpected error occurred trying to export system data.";
                ShowExportErrorAlert = true;
                return;
            }

            if (result == IExportService.ExportResult.Success)
            {
                Console.WriteLine($"ExportAllData: File saved successfully, now sharing '{filename}'");
                await FileSharerService.ShareFileAsync(filename);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"ExportAllData: Error occurred: {ex.Message}");
            Console.WriteLine($"ExportAllData: Stack trace: {ex.StackTrace}");
            ExportErrorMessage = $"An unexpected error occurred trying to export system data: {ex.Message}";
            ShowExportErrorAlert = true;
        }
    }



    protected virtual Task OnExporErrorAlertClosed((McmAlert.AlertAction action, string stateInformation) result)
    {
        ShowExportErrorAlert = false;
        ExportErrorMessage = string.Empty;
        return Task.CompletedTask;
    }
}
