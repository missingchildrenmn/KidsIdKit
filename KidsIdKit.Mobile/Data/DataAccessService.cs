using System.Text.Json;

namespace KidsIdKit.Data
{
    public class DataAccessService : IDataAccess
    {
        // TODO: Could use reflection to derive the name of the project
        private const string projectName = "kidsidkitdata";

        //-----------------------------------------------------------------------------------------------------------------------------------
        // An enumeration member in the.NET framework's System.Environment class. It represents a special folder on the user's system
        // specifically designated for storing application-specific data that is local to the current user and not intended to roam
        // between different machines or profiles.
        //
        // The directory that serves as a common repository for application-specific data for the current, non-roaming user.
        private const Environment.SpecialFolder appSpecificDataDirForCurrentNonRoamingUser = Environment.SpecialFolder.LocalApplicationData;
        //-----------------------------------------------------------------------------------------------------------------------------------
        private static readonly string localApplicationDataFolder = Environment.GetFolderPath(appSpecificDataDirForCurrentNonRoamingUser) +
                                                                    Path.DirectorySeparatorChar;

        private readonly string fileName = localApplicationDataFolder + projectName + ".dat";
        private readonly string backupFileName = localApplicationDataFolder + projectName + ".bak";

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
                    return NewFamily();
                }
            }
            catch
            {
                return NewFamily();
            }

            static Task<Family?> NewFamily()
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