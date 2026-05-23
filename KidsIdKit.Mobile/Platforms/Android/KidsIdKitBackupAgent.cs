using Android.App;
using Android.App.Backup;
using Android.OS;
using Android.Runtime;
using Java.IO;
using KidsIdKit.Core.Services;
using KidsIdKit.Mobile.Data;
using KidsIdKit.Mobile.Services;
using System.Globalization;
using AndroidUtil = Android.Util;

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


    public override void OnCreate()
    {
        base.OnCreate();
        AndroidUtil.Log.Info("KidsIdKit", "BackupAgent.OnCreate called");
        System.Diagnostics.Debug.WriteLine("KidsIdKit: BackupAgent.OnCreate called");

        // Backup all files in the app's files directory
        var fileBackupHelper = new FileBackupHelper(this, GetFilesToBackup());
        AddHelper(FilesBackupKey, fileBackupHelper);
    }

    public override void OnBackup(ParcelFileDescriptor? oldState, BackupDataOutput? data, ParcelFileDescriptor? newState)
    {
        System.Diagnostics.Debug.WriteLine("KidsIdKit: BackupAgent.OnBackup called");
        AndroidUtil.Log.Info("KidsIdKit", "BackupAgent.OnBackup called");
        // Only perform backup if cloud backup is enabled
        if (IsCloudBackupEnabled())
        {
            System.Diagnostics.Debug.WriteLine("KidsIdKit: Performing backup");
            AndroidUtil.Log.Info("KidsIdKit", "Performing backup");
            base.OnBackup(oldState, data, newState);
        }
        else
        {
            AndroidUtil.Log.Info("KidsIdKit", "Backup disabled, writing empty state");
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

    public override void OnRestoreFile(ParcelFileDescriptor? data, long size, Java.IO.File? destination, [GeneratedEnum] BackupFileType type, long mode, long mtime)
    {
        AndroidUtil.Log.Info("KidsIdKit", "BackupAgent.OnRestoreFile called");
        base.OnRestoreFile(data, size, destination, type, mode, mtime);
    }

    public override void OnRestore(BackupDataInput? data, int appVersionCode, ParcelFileDescriptor? newState)
    {
        AndroidUtil.Log.Info("KidsIdKit", "BackupAgent.OnRestore called");
        base.OnRestore(data, appVersionCode, newState);
    }
    public override void OnRestoreFinished()
    {
        base.OnRestoreFinished();

        // Log completion (in production, you might want to notify the app)
        System.Diagnostics.Debug.WriteLine("KidsIdKit: Restore operation finished");
        AndroidUtil.Log.Info("KidsIdKit", "Restore operation finished");
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
        string[] filesToBackup = new string[5];
        filesToBackup[0]= "profileInstalled";
        filesToBackup[1]= PinService.SaltKey;
        filesToBackup[2]= PinService.TokenKey;
        filesToBackup[3]= CloudBackupService.CloudBackupEnabledKey;
        filesToBackup[4]= $"{DataAccessService.ProjectName}.zip";
        return filesToBackup;
    }

    public override void OnQuotaExceeded(long backupDataBytes, long quotaBytes)
    {
        AndroidUtil.Log.Info("KidsIdKit", $"Data quota exceeded");

        base.OnQuotaExceeded(backupDataBytes, quotaBytes);

        // Log quota exceeded (in production, you might want to notify the user)
        
        System.Diagnostics.Debug.WriteLine(
            $"KidsIdKit: Backup quota exceeded. Data size: {backupDataBytes} bytes, Quota: {quotaBytes} bytes");
    }
}
