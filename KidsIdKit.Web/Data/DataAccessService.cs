using Blazored.LocalStorage;
using ICSharpCode.SharpZipLib.Zip;
using KidsIdKit.Shared.Data;
using System.Text.Json;

namespace KidsIdKit.Web.Data
{
    public class DataAccessService(ILocalStorageService localStorage) : IDataAccess
    {
        private const string ZipKey = "familyZip";
        private const string EntryName = "family.json";

        public async Task<Family?> GetDataAsync()
        {
            try
            {
                var zipBytes = await localStorage.GetItemAsync<byte[]>(ZipKey);
                if (zipBytes != null && zipBytes.Length > 0)
                {
                    using var zipStream = new MemoryStream(zipBytes);
                    using var zipFile = new ZipFile(zipStream);
                    var entry = zipFile.GetEntry(EntryName);
                    if (entry != null)
                    {
                        using var entryStream = zipFile.GetInputStream(entry);
                        using var streamReader = new StreamReader(entryStream);
                        var json = await streamReader.ReadToEndAsync();
                        // TODO: add decryption
                        return JsonSerializer.Deserialize<Family>(json);
                    }
                }
                return new Family();
            }
            catch
            {
                return new Family();
            }
        }

        public async Task SaveDataAsync(Family data)
        {
            var json = JsonSerializer.Serialize(data);
            // TODO: add encryption

            using var memStream = new MemoryStream();
            using (var zipStream = new ZipOutputStream(memStream))
            {
                var entry = new ZipEntry(EntryName);
                zipStream.PutNextEntry(entry);
                var jsonBytes = System.Text.Encoding.UTF8.GetBytes(json);
                await zipStream.WriteAsync(jsonBytes, 0, jsonBytes.Length);
                zipStream.CloseEntry();
                zipStream.IsStreamOwner = false;
                zipStream.Finish();
            }
            await localStorage.SetItemAsync(ZipKey, memStream.ToArray());

            await localStorage.SetItemAsync("familyZipLastUpdated", DateTime.UtcNow);
        }

        public async Task<DateTime?> GetLastUpdatedDateTimeAsync()
        {
            try
            {
                var lastUpdated = await localStorage.GetItemAsync<DateTime>("familyZipLastUpdated");
                return lastUpdated;
            }
            catch
            {
                return null; // Return null if not found or error occurs
            }
        }
    }
}
