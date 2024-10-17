using System.Text.Json;

namespace KidsIdKit.Data
{
    public class DataAccessService(Blazored.LocalStorage.ILocalStorageService localStorage) : IDataAccess
    {
        public async Task<Family?> GetDataAsync()
        {
            try
            {
                var json = await localStorage.GetItemAsync<string>("family");
                if (!string.IsNullOrEmpty(json))
                {
                    // TODO: add decryption
                    return JsonSerializer.Deserialize<Family>(json);
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
            await localStorage.SetItemAsync("family", json);
        }
    }
}
