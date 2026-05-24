using KidsIdKit.Core.Services;
using Microsoft.Extensions.Logging;

namespace KidsIdKit.Mobile.Services;

/// <summary>
/// Manages cloud backup preferences using secure storage.
/// </summary>
public class CloudBackupService(
    IStorageService storageService,
    ILogger<CloudBackupService> logger) : ICloudBackupService
{

    public const string CloudBackupEnabledKey = "KidsIdKit_CloudBackupEnabled";

    public async Task<bool> IsCloudBackupEnabledAsync()
    {
        return await storageService.IsCloudBackupEnabledAsync();
    }

    public bool IsCloudBackupSupported()
    {
        return true;
    }

    public async Task EnableCloudBackupAsync()
    {
        await storageService.WriteAsync(CloudBackupEnabledKey, new byte[] { 1 });

#if IOS
        SetSkipBackupAttributeForAllFiles(false);
#elif ANDROID
        // Request an immediate backup now that backup is enabled
        // The custom BackupAgent will check the preference and backup files
        RequestAndroidBackup();
#endif

        logger.LogInformation("Cloud backup enabled");
    }

    public async Task DisableCloudBackupAsync()
    {
        if (await storageService.ExistsAsync(CloudBackupEnabledKey))
        {
            await storageService.DeleteAsync(CloudBackupEnabledKey);

#if IOS
            SetSkipBackupAttributeForAllFiles(true);
#elif ANDROID
            // The custom BackupAgent will not backup files anymore since preference is deleted
            // Note: Existing backups in Google Drive remain until manually deleted by user
            logger.LogInformation("Cloud backup disabled. Existing Android backups remain in Google Drive.");
#endif

            logger.LogInformation("Cloud backup disabled");
        }
    }

#if IOS
    private void SetSkipBackupAttributeForAllFiles(bool skipBackup)
    {
        try
        {
            var appDataDirectory = FileSystem.AppDataDirectory;

            if (!Directory.Exists(appDataDirectory))
            {
                logger.LogWarning("App data directory does not exist: {Directory}", appDataDirectory);
                return;
            }

            var allFiles = Directory.GetFiles(appDataDirectory, "*", SearchOption.TopDirectoryOnly);

            foreach (var file in allFiles)
            {
                try
                {
                    Foundation.NSFileManager.SetSkipBackupAttribute(file, skipBackup);
                    logger.LogDebug("Set skip backup attribute to {SkipBackup} for file: {File}", skipBackup, file);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Failed to set skip backup attribute for file: {File}", file);
                }
            }

            logger.LogInformation("Set skip backup attribute to {SkipBackup} for {Count} files", skipBackup, allFiles.Length);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to set backup attributes for app data directory files");
        }
    }
#endif

#if ANDROID
    private void RequestAndroidBackup()
    {
        try
        {
            var context = Platform.CurrentActivity?.ApplicationContext;
            if (context == null)
            {
                logger.LogWarning("Android context not available to request backup");
                return;
            }

            var backupManager = new Android.App.Backup.BackupManager(context);
            backupManager.DataChanged();

            logger.LogInformation("Android backup requested via BackupManager");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to request Android backup");
        }
    }
#endif

    public string GetBackupLocation()
    {
        var returnValue = "Not supported";
#if IOS || MACCATALYST
        returnValue = "iCloud";
#elif ANDROID
        returnValue = "Google Drive";
#endif
        return returnValue;
    }
}
