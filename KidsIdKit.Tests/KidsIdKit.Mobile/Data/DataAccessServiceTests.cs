//using System.IO;
//using System.Threading.Tasks;
//using Xunit;
//using KidsIdKit.Mobile.Data;

// namespace KidsIdKit.Tests.KidsIdKit.Mobile.Data;

//public class DataAccessServiceTests
//{
//    private string GetTempDirectory()
//    {
//        var tempDir = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
//        Directory.CreateDirectory(tempDir);
//        return tempDir;
//    }

//    [Fact]
//    public async Task SaveAndLoadFamily_CompressedZip_WorksCorrectly()
//    {
//        // Arrange
//        var tempDir = GetTempDirectory();
//        var projectName = "kidsidkitdata";
//        var zipFileName = Path.Combine(tempDir, projectName + ".zip");
//        var backupZipFileName = Path.Combine(tempDir, projectName + ".bak.zip");

//        var service = new DataAccessServiceForTest(zipFileName, backupZipFileName, projectName + ".dat");

//        var family = new Family();
//        family.Children.Add(new Child { Id = 1 });

//        // Act
//        await service.SaveDataAsync(family);
//        var loadedFamily = await service.GetDataAsync();

//        // Assert
//        Assert.NotNull(loadedFamily);
//        Assert.Single(loadedFamily.Children);
//        Assert.Equal(1, loadedFamily.Children[0].Id);

//        // Cleanup
//        Directory.Delete(tempDir, true);
//    }

//    // Testable subclass to override file paths
//    private class DataAccessServiceForTest : IDataAccess
//    {
//        private readonly string zipFileName;
//        private readonly string backupZipFileName;
//        private readonly string jsonFileName;

//        public DataAccessServiceForTest(string zipFileName, string backupZipFileName, string jsonFileName)
//        {
//            this.zipFileName = zipFileName;
//            this.backupZipFileName = backupZipFileName;
//            this.jsonFileName = jsonFileName;
//        }

//        public async Task<Family?> GetDataAsync()
//        {
//            try
//            {
//                if (File.Exists(zipFileName))
//                {
//                    using var zipStream = File.OpenRead(zipFileName);
//                    using var archive = new System.IO.Compression.ZipArchive(zipStream, System.IO.Compression.ZipArchiveMode.Read);
//                    var entry = archive.GetEntry(jsonFileName);
//                    if (entry != null)
//                    {
//                        using var entryStream = entry.Open();
//                        using var reader = new StreamReader(entryStream);
//                        var json = await reader.ReadToEndAsync();
//                        return System.Text.Json.JsonSerializer.Deserialize<Family>(json);
//                    }
//                }
//                return new Family();
//            }
//            catch
//            {
//                return new Family();
//            }
//        }

//        public async Task SaveDataAsync(Family data)
//        {
//            if (File.Exists(zipFileName))
//                File.Copy(zipFileName, backupZipFileName, true);

//            var json = System.Text.Json.JsonSerializer.Serialize(data);

//            using var memStream = new MemoryStream();
//            using (var archive = new System.IO.Compression.ZipArchive(memStream, System.IO.Compression.ZipArchiveMode.Create, true))
//            {
//                var entry = archive.CreateEntry(jsonFileName, System.IO.Compression.CompressionLevel.Optimal);
//                using var entryStream = entry.Open();
//                using var writer = new StreamWriter(entryStream);
//                await writer.WriteAsync(json);
//            }

//            using var fileStream = File.Create(zipFileName);
//            memStream.Seek(0, SeekOrigin.Begin);
//            await memStream.CopyToAsync(fileStream);
//        }
//    }
//}
