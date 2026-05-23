using KidsIdKit.Core.Services;

namespace KidsIdKit.Web.Services;

/// <summary>
/// Manages cloud backup preferences using secure storage.
/// </summary>
public class CloudBackupService(
    IStorageService storageService,
    ILogger<CloudBackupService> logger) : ICloudBackupService
{
    public async Task<bool> IsCloudBackupEnabledAsync()
    {
        return await storageService.IsCloudBackupEnabledAsync();
    }

    public bool IsCloudBackupSupported()
    {
        return false;
    }

    public async Task EnableCloudBackupAsync()
    {
        logger.LogInformation("EnableCloudBackupAsync not supported on Web platform");
        throw new NotImplementedException();
    }

    public async Task DisableCloudBackupAsync()
    {
        logger.LogInformation("EnableCloudBackupAsync not supported on Web platform");
        throw new NotImplementedException();
    }

    public string GetBackupLocation()
    {
        return "Not Supported";
    }
}
