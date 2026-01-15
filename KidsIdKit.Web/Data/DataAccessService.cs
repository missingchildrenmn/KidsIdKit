using Blazored.LocalStorage;
using ICSharpCode.SharpZipLib.Zip;
using KidsIdKit.Core.Data;
using KidsIdKit.Core.Services;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace KidsIdKit.Web.Data;

public class DataAccessService(
    ILocalStorageService localStorage,
    IEncryptionKeyProvider encryptionKeyProvider,
    ILogger<DataAccessService> logger) : IDataAccess
{
    private const string ZipKey = "FamilyZip";
    private const string EntryName = "Family.json";

    public async Task<Family?> GetDataAsync()
    {
        try
        {
            var zipBytes = await localStorage.GetItemAsync<byte[]>(ZipKey);
            if (zipBytes == null || zipBytes.Length == 0)
            {
                return new Family();
            }

            using var zipStream = new MemoryStream(zipBytes);
            using var zipFile = new ZipFile(zipStream);
            var entry = zipFile.GetEntry(EntryName);
            if (entry == null)
            {
                logger.LogWarning("ZIP archive exists but entry '{EntryName}' not found", EntryName);
                return new Family();
            }

            using var entryStream = zipFile.GetInputStream(entry);
            using var streamReader = new StreamReader(entryStream);
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
                "Your saved data appears to be corrupted. Please contact support if this persists.",
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
            if (familyData.Children.Count == 0)
            {
                await localStorage.RemoveItemAsync(ZipKey);
                return;
            }

            familyData.LastDateTimeAnyChildWasUpdated = DateTime.Now;
            var json = JsonSerializer.Serialize(familyData);

            // Encrypt the data
            var key = await encryptionKeyProvider.GetOrCreateKeyAsync();
            var encryptedJson = EncryptionHelper.Encrypt(json, key);

            using var memoryStream = new MemoryStream();
            using (var zipStream = new ZipOutputStream(memoryStream))
            {
                var entry = new ZipEntry(EntryName);
                zipStream.PutNextEntry(entry);
                var jsonBytes = System.Text.Encoding.UTF8.GetBytes(encryptedJson);
                await zipStream.WriteAsync(jsonBytes, 0, jsonBytes.Length);
                zipStream.CloseEntry();
                zipStream.IsStreamOwner = false;
                zipStream.Finish();
            }

            await localStorage.SetItemAsync(ZipKey, memoryStream.ToArray());
            logger.LogDebug("Family data saved successfully");
        }
        catch (Exception ex) when (ex.Message.Contains("quota", StringComparison.OrdinalIgnoreCase))
        {
            logger.LogError(ex, "Storage quota exceeded while saving family data");
            throw new DataAccessException(
                DataAccessErrorType.StorageFull,
                "Storage is full. Please delete some data or photos to continue.",
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
