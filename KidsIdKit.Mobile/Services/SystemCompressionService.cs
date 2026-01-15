using System.IO.Compression;
using KidsIdKit.Core.Services;

namespace KidsIdKit.Mobile.Services;

/// <summary>
/// Compression service using System.IO.Compression for MAUI.
/// </summary>
public class SystemCompressionService : ICompressionService
{
    public async Task<byte[]> CompressAsync(string content, string entryName)
    {
        using var memoryStream = new MemoryStream();
        using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
        {
            var zipEntry = archive.CreateEntry(entryName, CompressionLevel.Optimal);
            using var entryStream = zipEntry.Open();
            using var streamWriter = new StreamWriter(entryStream);
            await streamWriter.WriteAsync(content);
        }

        return memoryStream.ToArray();
    }

    public async Task<string?> DecompressAsync(byte[] compressedData, string entryName)
    {
        using var memoryStream = new MemoryStream(compressedData);
        using var archive = new ZipArchive(memoryStream, ZipArchiveMode.Read);

        var entry = archive.GetEntry(entryName);
        if (entry == null)
        {
            return null;
        }

        using var entryStream = entry.Open();
        using var streamReader = new StreamReader(entryStream);
        return await streamReader.ReadToEndAsync();
    }
}
