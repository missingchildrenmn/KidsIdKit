using System.Text.Json;
using System.IO.Compression;
using KidsIdKit.Core.Data;
using KidsIdKit.Core.Services;
using Microsoft.Extensions.Logging;

namespace KidsIdKit.Mobile.Data;

public class DataAccessService(
    IEncryptionKeyProvider encryptionKeyProvider,
    ILogger<DataAccessService> logger) : IDataAccess
{
    private const string ProjectName = "KidsIdKitData";
    private static readonly string LocalAppDataFolder = FileSystem.AppDataDirectory + Path.DirectorySeparatorChar;
    private static readonly string PathAndFileName = LocalAppDataFolder + ProjectName;

    private readonly string _zipFileName = PathAndFileName + ".zip";
    private readonly string _jsonFileName = ProjectName + ".dat";
    private readonly string _backupZipFileName = PathAndFileName + ".bak.zip";

    public async Task<Family?> GetDataAsync()
    {
        try
        {
            if (!File.Exists(_zipFileName))
            {
                return new Family();
            }

            using var zipFileStream = File.OpenRead(_zipFileName);
            using var zipArchive = new ZipArchive(zipFileStream, ZipArchiveMode.Read);
            var entry = zipArchive.GetEntry(_jsonFileName);
            if (entry == null)
            {
                logger.LogWarning("ZIP archive exists but entry '{EntryName}' not found", _jsonFileName);
                return new Family();
            }

            using var stream = entry.Open();
            using var streamReader = new StreamReader(stream);
            var encryptedJson = await streamReader.ReadToEndAsync();

            // Decrypt the data
            string json;
            try
            {
                var key = await encryptionKeyProvider.GetOrCreateKeyAsync();
                json = EncryptionHelper.Decrypt(encryptedJson, key);
            }
            catch (Exception ex)
            {
                // If decryption fails, try reading as plain JSON (for migration from unencrypted data)
                logger.LogWarning(ex, "Decryption failed, attempting to read as plain JSON for migration");
                json = encryptedJson;
            }

            var family = JsonSerializer.Deserialize<Family>(json);
            return family ?? new Family();
        }
        catch (JsonException ex)
        {
            logger.LogError(ex, "Failed to deserialize family data - data may be corrupted");
            throw new DataAccessException(
                DataAccessErrorType.CorruptedData,
                "Your saved data appears to be corrupted. You may be able to restore from backup.",
                ex);
        }
        catch (DataAccessException)
        {
            throw;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error reading family data");
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
            // Create backup before saving
            if (File.Exists(_zipFileName))
            {
                File.Copy(_zipFileName, _backupZipFileName, true);
                logger.LogDebug("Backup created at {BackupPath}", _backupZipFileName);
            }

            if (familyData.Children.Count == 0)
            {
                if (File.Exists(_zipFileName))
                {
                    File.Delete(_zipFileName);
                }
                return;
            }

            familyData.LastDateTimeAnyChildWasUpdated = DateTime.Now;
            var json = JsonSerializer.Serialize(familyData);

            // Encrypt the data
            var key = await encryptionKeyProvider.GetOrCreateKeyAsync();
            var encryptedJson = EncryptionHelper.Encrypt(json, key);

            // Write to memory stream then to file
            using var memoryStream = new MemoryStream();
            using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
            {
                var zipArchiveEntry = archive.CreateEntry(_jsonFileName, CompressionLevel.Optimal);
                using var entryStream = zipArchiveEntry.Open();
                using var streamWriter = new StreamWriter(entryStream);
                await streamWriter.WriteAsync(encryptedJson);
            }

            using var zipFileStream = File.Create(_zipFileName);
            memoryStream.Seek(0, SeekOrigin.Begin);
            await memoryStream.CopyToAsync(zipFileStream);

            logger.LogDebug("Family data saved successfully");
        }
        catch (IOException ex) when (ex.Message.Contains("space", StringComparison.OrdinalIgnoreCase) ||
                                      ex.Message.Contains("full", StringComparison.OrdinalIgnoreCase))
        {
            logger.LogError(ex, "Storage full while saving family data");
            throw new DataAccessException(
                DataAccessErrorType.StorageFull,
                "Device storage is full. Please free up space to save your data.",
                ex);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to save family data");
            throw new DataAccessException(
                DataAccessErrorType.WriteFailure,
                "Unable to save your data. Please try again.",
                ex);
        }
    }
}
