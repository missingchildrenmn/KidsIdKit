namespace KidsIdKit.Core.Services;

/// <summary>
/// Provides compression and decompression services.
/// Platform-specific implementations handle different compression libraries.
/// </summary>
public interface ICompressionService
{
    /// <summary>
    /// Compresses content into a ZIP archive with the specified entry name.
    /// </summary>
    /// <param name="content">The string content to compress.</param>
    /// <param name="entryName">The name of the entry within the archive.</param>
    /// <returns>The compressed data as a byte array.</returns>
    Task<byte[]> CompressAsync(string content, string entryName);

    /// <summary>
    /// Decompresses a ZIP archive and extracts the specified entry.
    /// </summary>
    /// <param name="compressedData">The compressed data.</param>
    /// <param name="entryName">The name of the entry to extract.</param>
    /// <returns>The decompressed content, or null if the entry doesn't exist.</returns>
    Task<string?> DecompressAsync(byte[] compressedData, string entryName);
}
