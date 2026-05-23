using Android.App;
using Android.App.Backup;
using Android.OS;
using Java.IO;
using KidsIdKit.Core.Services;
using KidsIdKit.Mobile.Services;

namespace KidsIdKit.Mobile.Platforms.Android;

/// <summary>
/// Custom backup agent that conditionally backs up app data based on user preferences.
/// Only performs backup operations when cloud backup is enabled by the user.
/// </summary>
public class KidsIdKitBackupAgent : BackupAgentHelper
{
    private const string FilesBackupKey = "app_files";

    public KidsIdKitBackupAgent()
    {
        System.Diagnostics.Debug.WriteLine("KidsIdKit: BackupAgent constructor called");
    }

    public override void OnRestore(BackupDataInput? data, int appVersionCode, ParcelFileDescriptor? newState)
    {
        base.OnRestore(data, appVersionCode, newState);
    }


    public override void OnCreate()
    {
        base.OnCreate();

        System.Diagnostics.Debug.WriteLine("KidsIdKit: BackupAgent.OnCreate called");

        // Check if cloud backup is enabled before setting up backup helpers
        System.Diagnostics.Debug.WriteLine("KidsIdKit: Cloud backup is enabled, setting up FileBackupHelper");
        // Backup all files in the app's files directory
        var fileBackupHelper = new FileBackupHelper(this, GetFilesToBackup());
        AddHelper(FilesBackupKey, fileBackupHelper);
    }

    public override void OnBackup(ParcelFileDescriptor? oldState, BackupDataOutput? data, ParcelFileDescriptor? newState)
    {
        System.Diagnostics.Debug.WriteLine("KidsIdKit: BackupAgent.OnBackup called");

        // Only perform backup if cloud backup is enabled
        if (IsCloudBackupEnabled())
        {
            System.Diagnostics.Debug.WriteLine("KidsIdKit: Performing backup");
            base.OnBackup(oldState, data, newState);
        }
        else
        {
            System.Diagnostics.Debug.WriteLine("KidsIdKit: Backup disabled, writing empty state");
            // If backup is disabled, write empty state to indicate no backup needed
            // This prevents Android from trying to backup data
            if (newState != null)
            {
                using var newStateStream = new FileOutputStream(newState.FileDescriptor);
                // Write empty state
                newStateStream.Write(new byte[0]);
            }
        }
    }

    public override void OnRestoreFinished()
    {
        base.OnRestoreFinished();

        // Log completion (in production, you might want to notify the app)
        System.Diagnostics.Debug.WriteLine("KidsIdKit: Restore operation finished");
    }

    /// <summary>
    /// Checks if cloud backup is enabled by looking for the preference file.
    /// </summary>
    private bool IsCloudBackupEnabled()
    {
        try
        {
            if (FilesDir == null)
                return false;

            var preferencePath = Path.Combine(FilesDir.AbsolutePath, CloudBackupService.CloudBackupEnabledKey);
            return System.IO.File.Exists(preferencePath);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"KidsIdKit: Error checking backup preference: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// Gets the list of files to backup from the app's files directory.
    /// Excludes the cloud backup preference file itself.
    /// </summary>
    private string[] GetFilesToBackup()
    {
        try
        {
            if (FilesDir == null)
                return Array.Empty<string>();

            var allFiles = Directory.GetFiles(FilesDir.AbsolutePath)
                .Select(Path.GetFileName)
                .Where(f => !string.IsNullOrEmpty(f) && f != CloudBackupService.CloudBackupEnabledKey)
                .ToArray();

            System.Diagnostics.Debug.WriteLine($"KidsIdKit: Preparing to backup {allFiles.Length} files");
            return allFiles!;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"KidsIdKit: Error getting files to backup: {ex.Message}");
            return Array.Empty<string>();
        }
    }

    public override void OnQuotaExceeded(long backupDataBytes, long quotaBytes)
    {
        base.OnQuotaExceeded(backupDataBytes, quotaBytes);

        // Log quota exceeded (in production, you might want to notify the user)
        System.Diagnostics.Debug.WriteLine(
            $"KidsIdKit: Backup quota exceeded. Data size: {backupDataBytes} bytes, Quota: {quotaBytes} bytes");
    }
}
