using System.Text.Json;
using KidsIdKit.Core.Data;
using Microsoft.Extensions.Logging;

namespace KidsIdKit.Core.Services;

/// <summary>
/// Base class for data access services with shared JSON serialization,
/// encryption, and error handling logic.
/// </summary>
public abstract class DataAccessServiceBase : IDataAccess
{
    private readonly ICompressionService _compressionService;
    private readonly IStorageService _storageService;
    private readonly IEncryptionKeyProvider _encryptionKeyProvider;
    private readonly ILogger _logger;

    protected abstract string StorageKey { get; }
    protected abstract string BackupKey { get; }
    protected abstract string EntryName { get; }
    protected virtual bool SupportsBackup => false;

    protected DataAccessServiceBase(
        ICompressionService compressionService,
        IStorageService storageService,
        IEncryptionKeyProvider encryptionKeyProvider,
        ILogger logger)
    {
        _compressionService = compressionService;
        _storageService = storageService;
        _encryptionKeyProvider = encryptionKeyProvider;
        _logger = logger;
    }

    public async Task<Family?> GetDataAsync()
    {
        try
        {
            var compressedData = await _storageService.ReadAsync(StorageKey);
            if (compressedData == null || compressedData.Length == 0)
            {
                return new Family();
            }

            var encryptedJson = await _compressionService.DecompressAsync(compressedData, EntryName);
            if (encryptedJson == null)
            {
                _logger.LogWarning("Archive exists but entry '{EntryName}' not found", EntryName);
                return new Family();
            }

            // Decrypt the data
            string json;
            try
            {
                var key = await _encryptionKeyProvider.GetOrCreateKeyAsync();
                json = EncryptionHelper.Decrypt(encryptedJson, key);
            }
            catch (Exception ex)
            {
                // If decryption fails, try reading as plain JSON (for migration from unencrypted data)
                _logger.LogWarning(ex, "Decryption failed, attempting to read as plain JSON for migration");
                json = encryptedJson;
            }

            var family = JsonSerializer.Deserialize<Family>(json);
            return family ?? new Family();
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Failed to deserialize family data - data may be corrupted");
            throw new DataAccessException(
                DataAccessErrorType.CorruptedData,
                SupportsBackup
                    ? "Your saved data appears to be corrupted. You may be able to restore from backup."
                    : "Your saved data appears to be corrupted. Please contact support if this persists.",
                ex);
        }
        catch (DataAccessException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error reading family data");
            throw new DataAccessException(
                DataAccessErrorType.ReadFailure,
                "Unable to load your data. Please try again.",
                ex);
        }
    }

    public async Task SaveDataAsync(Family familyData)
    {
        try
        {
            // Create backup before saving if supported
            if (SupportsBackup && await _storageService.ExistsAsync(StorageKey))
            {
                await _storageService.BackupAsync(StorageKey, BackupKey);
                _logger.LogDebug("Backup created at {BackupKey}", BackupKey);
            }

            if (familyData.Children.Count == 0)
            {
                await _storageService.DeleteAsync(StorageKey);
                return;
            }

            familyData.LastDateTimeAnyChildWasUpdated = DateTime.Now;
            var json = JsonSerializer.Serialize(familyData);

            // Encrypt the data
            var key = await _encryptionKeyProvider.GetOrCreateKeyAsync();
            var encryptedJson = EncryptionHelper.Encrypt(json, key);

            // Compress and save
            var compressedData = await _compressionService.CompressAsync(encryptedJson, EntryName);
            await _storageService.WriteAsync(StorageKey, compressedData);

            _logger.LogDebug("Family data saved successfully");
        }
        catch (Exception ex) when (IsStorageFullException(ex))
        {
            _logger.LogError(ex, "Storage full while saving family data");
            throw new DataAccessException(
                DataAccessErrorType.StorageFull,
                SupportsBackup
                    ? "Device storage is full. Please free up space to save your data."
                    : "Storage is full. Please delete some data or photos to continue.",
                ex);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to save family data");
            throw new DataAccessException(
                DataAccessErrorType.WriteFailure,
                "Unable to save your data. Please try again.",
                ex);
        }
    }

    private static bool IsStorageFullException(Exception ex)
    {
        var message = ex.Message.ToLowerInvariant();
        return message.Contains("quota") ||
               message.Contains("space") ||
               message.Contains("full");
    }
}
