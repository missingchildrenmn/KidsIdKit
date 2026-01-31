namespace KidsIdKit.Core.Services;

/// <summary>
/// Provides storage services for reading and writing data.
/// Platform-specific implementations handle different storage mechanisms.
/// </summary>
public interface IStorageService
{
    /// <summary>
    /// Reads data from storage.
    /// </summary>
    /// <param name="key">The storage key/identifier.</param>
    /// <returns>The stored data, or null if not found.</returns>
    Task<byte[]?> ReadAsync(string key);

    /// <summary>
    /// Writes data to storage.
    /// </summary>
    /// <param name="key">The storage key/identifier.</param>
    /// <param name="data">The data to store.</param>
    Task WriteAsync(string key, byte[] data);

    /// <summary>
    /// Deletes data from storage.
    /// </summary>
    /// <param name="key">The storage key/identifier.</param>
    Task DeleteAsync(string key);

    /// <summary>
    /// Checks if data exists in storage.
    /// </summary>
    /// <param name="key">The storage key/identifier.</param>
    /// <returns>True if the data exists, false otherwise.</returns>
    Task<bool> ExistsAsync(string key);

    /// <summary>
    /// Creates a backup of the data if supported by the platform.
    /// </summary>
    /// <param name="key">The storage key/identifier.</param>
    /// <param name="backupKey">The backup storage key/identifier.</param>
    Task BackupAsync(string key, string backupKey);
}
