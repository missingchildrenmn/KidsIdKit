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

    public Task BackupAsync(string key, string backupKey)
    {
        var sourcePath = GetFilePath(key);
        var backupPath = GetFilePath(backupKey);

        if (File.Exists(sourcePath))
        {
            File.Copy(sourcePath, backupPath, overwrite: true);
            logger.LogDebug("Backup created: {SourcePath} -> {BackupPath}", sourcePath, backupPath);
        }

        return Task.CompletedTask;
    }

    private static string GetFilePath(string key)
    {
        return BaseDirectory + key;
    }
}
