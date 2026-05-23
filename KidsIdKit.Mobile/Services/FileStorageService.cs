using KidsIdKit.Core.Services;
using Microsoft.Extensions.Logging;

namespace KidsIdKit.Mobile.Services;

/// <summary>
/// Storage service using the device file system for MAUI.
/// </summary>
public class FileStorageService(ILogger<FileStorageService> logger) : IStorageService
{
    private static readonly string BaseDirectory = FileSystem.AppDataDirectory + Path.DirectorySeparatorChar;

    public async Task<byte[]?> ReadAsync(string key)
    {
        var filePath = GetFilePath(key);
        if (!File.Exists(filePath))
        {
            return null;
        }

        return await File.ReadAllBytesAsync(filePath);
    }

    public async Task WriteAsync(string key, byte[] data)
    {
        var filePath = GetFilePath(key);
        await File.WriteAllBytesAsync(filePath, data);
        await SetFileBackupAsync(filePath);
    }

    public Task DeleteAsync(string key)
    {
        var filePath = GetFilePath(key);
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }
        return Task.CompletedTask;
    }

    public Task<bool> ExistsAsync(string key)
    {
        var filePath = GetFilePath(key);
        return Task.FromResult(File.Exists(filePath));
    }

    public async Task BackupAsync(string key, string backupKey)
    {
        var sourcePath = GetFilePath(key);
        var backupPath = GetFilePath(backupKey);

        if (File.Exists(sourcePath))
        {
            File.Copy(sourcePath, backupPath, overwrite: true);
            await SetFileBackupAsync(backupPath);
            logger.LogDebug("Backup created: {SourcePath} -> {BackupPath}", sourcePath, backupPath);
        }
    }

    private static string GetFilePath(string key)
    {
        return BaseDirectory + key;
    }

    public async Task<bool> IsCloudBackupEnabledAsync()
    {
        return await ExistsAsync(CloudBackupService.CloudBackupEnabledKey);
    }

    public async Task SetFileBackupAsync(string path)
    {
#if IOS
        try
        {
            // If cloud backup is disabled, skip iCloud backup (true means skip)
            // If cloud backup is enabled, allow iCloud backup (false means don't skip)
            bool isCloudBackupEnabled = await IsCloudBackupEnabledAsync();
            bool skipBackup = !isCloudBackupEnabled;

            Foundation.NSFileManager.SetSkipBackupAttribute(path, skipBackup);
            logger.LogDebug("Set skip backup attribute to {SkipBackup} for file: {Path}", skipBackup, path);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error setting backup attribute for file {Path}", path);
        }
#elif ANDROID
        try
        {
            bool isCloudBackupEnabled = await IsCloudBackupEnabledAsync();

            if (isCloudBackupEnabled)
            {
                // Notify Android that data has changed so it schedules a backup
                // The custom BackupAgent will handle the actual backup
                var context = Microsoft.Maui.ApplicationModel.Platform.CurrentActivity?.ApplicationContext;
                if (context != null)
                {
                    var backupManager = new Android.App.Backup.BackupManager(context);
                    backupManager.DataChanged();
                    logger.LogDebug("Android backup requested after file write: {Path}", path);
                }
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error requesting Android backup for file {Path}", path);
        }
#endif
    }
}
