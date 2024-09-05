using System.Text.Json;

namespace KidsIdKit.Data
{
    public class DataAccessService : IDataAccess
    {
        private static readonly string localApplicationDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + Path.DirectorySeparatorChar;
        private readonly string fileName = localApplicationDataFolder + "kidsidkitdata.dat";
        private readonly string backupFileName = localApplicationDataFolder + "kidsidkitdata.bak";

        public Family? GetData()
        {
            try
            {
                if (File.Exists(fileName))
                {
                    var json = File.ReadAllText(fileName);
                    // TODO: add decryption
                    return JsonSerializer.Deserialize<Family>(json);
                }
                else
                {
                    return new Family();
                }
            }
            catch
            {
                return new Family();
            }
        }

        public void SaveData(Family data)
        {
            if (File.Exists(fileName))
                File.Copy(fileName, backupFileName, true);
            var json = JsonSerializer.Serialize(data);
            // TODO: add encryption
            File.WriteAllText(fileName, json);
        }
    }
}
