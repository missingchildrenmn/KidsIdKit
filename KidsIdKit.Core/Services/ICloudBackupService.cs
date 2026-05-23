namespace KidsIdKit.Core.Services;

/// <summary>
/// Manages cloud backup preferences for the application.
/// </summary>
public interface ICloudBackupService
{
    /// <summary>
    /// Returns true if cloud backups have been enabled.
    /// </summary>
    Task<bool> IsCloudBackupEnabledAsync();

    /// <summary>
    /// Returns true if cloud backups are supported on the current platform.
    /// </summary>
    bool IsCloudBackupSupported();

    /// <summary>
    /// Enables cloud backups by storing the preference.
    /// </summary>
    Task EnableCloudBackupAsync();

    /// <summary>
    /// Disables cloud backups by removing the preference.
    /// </summary>
    Task DisableCloudBackupAsync();

    /// <summary>
    /// Gets the cloud location where files are backed up to for the current platform.
    /// </summary>
    /// <returns>The cloud backup location path for the platform.</returns>
    string GetBackupLocation();
}
