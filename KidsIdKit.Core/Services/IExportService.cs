
namespace KidsIdKit.Core.Services;

public interface IExportService
{
    Task<ExportResult> ExportData(string fileName);

    public enum ExportResult
    {
        Success = 0,
        PinDataNotFound = 1,
        FamilyDataNotFound = 2,
        UnexpectedError = 3,
    }

    public enum ImportResult
    {
        Success = 0,
    }
}
