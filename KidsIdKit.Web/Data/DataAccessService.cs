using Blazored.LocalStorage;
using ICSharpCode.SharpZipLib.Zip;
using KidsIdKit.Shared.Data;
using System.Text.Json;

namespace KidsIdKit.Web.Data
{
    public class DataAccessService(ILocalStorageService localStorage) : IDataAccess
    {
        private const string ZipKey = "FamilyZip";
        private const string EntryName = "Family.json";

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

        public async Task SaveDataAsync(Family familyData)
        {
            if (familyData.Children.Count == 0)
            {
                await localStorage.RemoveItemAsync(ZipKey);
            }
            else
            {
                familyData.LastDateTimeAnyChildWasUpdated = DateTime.Now;
                var json = JsonSerializer.Serialize(familyData);
                // TODO: add encryption

                using var memoryStream = new MemoryStream();
                using (var zipStream = new ZipOutputStream(memoryStream))
                {
                    var entry = new ZipEntry(EntryName);
                    zipStream.PutNextEntry(entry);
                    var jsonBytes = System.Text.Encoding.UTF8.GetBytes(json);
                    await zipStream.WriteAsync(jsonBytes, 0, jsonBytes.Length);
                    zipStream.CloseEntry();
                    zipStream.IsStreamOwner = false;
                    zipStream.Finish();
                }
                await localStorage.SetItemAsync(ZipKey, memoryStream.ToArray());
            }
        }
    }
}
