using ICSharpCode.SharpZipLib.Zip;
using KidsIdKit.Core.Services;

namespace KidsIdKit.Web.Services;

/// <summary>
/// Compression service using SharpZipLib for Blazor WebAssembly.
/// </summary>
public class SharpZipCompressionService : ICompressionService
{
    public Task<byte[]> CompressAsync(string content, string entryName)
    {
        using var memoryStream = new MemoryStream();
        using (var zipStream = new ZipOutputStream(memoryStream))
        {
            var entry = new ZipEntry(entryName);
            zipStream.PutNextEntry(entry);
            var contentBytes = System.Text.Encoding.UTF8.GetBytes(content);
            zipStream.Write(contentBytes, 0, contentBytes.Length);
            zipStream.CloseEntry();
            zipStream.IsStreamOwner = false;
            zipStream.Finish();
        }

        return Task.FromResult(memoryStream.ToArray());
    }

    public async Task<string?> DecompressAsync(byte[] compressedData, string entryName)
    {
        using var zipStream = new MemoryStream(compressedData);
        using var zipFile = new ZipFile(zipStream);

        var entry = zipFile.GetEntry(entryName);
        if (entry == null)
        {
            return null;
        }

        using var entryStream = zipFile.GetInputStream(entry);
        using var streamReader = new StreamReader(entryStream);
        return await streamReader.ReadToEndAsync();
    }
}
