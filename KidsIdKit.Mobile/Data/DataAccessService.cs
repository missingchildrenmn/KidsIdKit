using System.Text.Json;

namespace KidsIdKit.Data
{
    public class DataAccessService : IDataAccess
    {
        private static readonly string localApplicationDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + Path.DirectorySeparatorChar;
        private readonly string fileName = localApplicationDataFolder + "kidsidkitdata.dat";
        private readonly string backupFileName = localApplicationDataFolder + "kidsidkitdata.bak";

        public Task<Family?> GetDataAsync()
        {
            try
            {
                if (File.Exists(fileName))
                {
                    var json = File.ReadAllText(fileName);
                    // TODO: add decryption
                    return Task.FromResult(JsonSerializer.Deserialize<Family>(json));
                }
                else
                {
                    return Task.FromResult<Family?>(new Family());
                }
            }
            catch
            {
                return Task.FromResult<Family?>(new Family());
            }
        }

        public Task SaveDataAsync(Family data)
        {
            if (File.Exists(fileName))
                File.Copy(fileName, backupFileName, true);
            var json = JsonSerializer.Serialize(data);
            // TODO: add encryption
            File.WriteAllText(fileName, json);
            return Task.CompletedTask;
        }
    }
}
