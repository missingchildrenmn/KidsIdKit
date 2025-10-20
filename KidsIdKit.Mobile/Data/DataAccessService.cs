using System.Text.Json;
using System.IO.Compression;
using KidsIdKit.Shared.Data;

namespace KidsIdKit.Mobile.Data
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

        private readonly string zipFileName = localApplicationDataFolder + projectName + ".zip";
        private readonly string jsonFileName = projectName + ".dat";
        private readonly string backupZipFileName = localApplicationDataFolder + projectName + ".bak.zip";

        public async Task<Family?> GetDataAsync()
        {
            try
            {
                //File.Delete(zipFileName);   // DEVELOPMENT CODE ONLY (in case you need to start with a tabula rasa)

                if (File.Exists(zipFileName))
                {
                    // Unzip the file to a memory stream and read the JSON
                    using var zipFileStream = File.OpenRead(zipFileName);
                    using var zipArchiveEntry = new ZipArchive(zipFileStream, ZipArchiveMode.Read);
                    var entry = zipArchiveEntry.GetEntry(jsonFileName);
                    if (entry != null)
                    {
                        using var stream = entry.Open();
                        using var streamReader = new StreamReader(stream);
                        var jsonString = await streamReader.ReadToEndAsync();
                        // TODO: add decryption
                        return JsonSerializer.Deserialize<Family>(jsonString);
                    }
                }

                return new Family();
            }
            catch
            {
                return new Family();
            }
        }

        public Task<DateTime?> GetLastUpdatedDateTimeAsync()
        {
            try
            {
                if (File.Exists(zipFileName))
                {
                    using var zipFileStream = File.OpenRead(zipFileName);
                    using var zipArchive = new ZipArchive(zipFileStream, ZipArchiveMode.Read);
                    var entry = zipArchive.GetEntry(jsonFileName);
                    if (entry != null)
                    {
                        return Task.FromResult<DateTime?>(entry.LastWriteTime.DateTime);
                    }
                }
            }
            catch
            {
                // Log or handle exceptions as needed
            }

            return Task.FromResult<DateTime?>(null); // Return null if file or entry not found or error occurs
        }

        public async Task SaveDataAsync(Family data)
        {
            if (File.Exists(zipFileName))
            {
                File.Copy(zipFileName, backupZipFileName, true);
            }

            var json = JsonSerializer.Serialize(data);
            // TODO: add encryption

            // Write JSON to a memory stream
            using var memoryStream = new MemoryStream();

            // Zip it, zip it good ...
            using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
            {
                var zipArchiveEntry = archive.CreateEntry(jsonFileName, CompressionLevel.Optimal);
                using var entryStream = zipArchiveEntry.Open();
                using var streamWriter = new StreamWriter(entryStream);
                await streamWriter.WriteAsync(json);
            }

            // Save the zip file
            using var zipFileStream = File.Create(zipFileName);
            memoryStream.Seek(0, SeekOrigin.Begin);
            await memoryStream.CopyToAsync(zipFileStream);
        }
    }
}
