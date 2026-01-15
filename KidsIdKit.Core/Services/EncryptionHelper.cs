using System.Security.Cryptography;
using System.Text;

namespace KidsIdKit.Core.Services;

/// <summary>
/// Provides AES-256 encryption/decryption for data at rest.
/// Note: AES is supported in Blazor WebAssembly despite the CA1416 analyzer warning.
/// </summary>
#pragma warning disable CA1416 // AES is supported in Blazor WebAssembly
public static class EncryptionHelper
{
    private const int KeySize = 256;
    private const int IvSize = 16;

    /// <summary>
    /// Generates a new random encryption key.
    /// </summary>
    public static byte[] GenerateKey()
    {
        using var aes = Aes.Create();
        aes.KeySize = KeySize;
        aes.GenerateKey();
        return aes.Key;
    }

    /// <summary>
    /// Encrypts a string using AES-256.
    /// </summary>
    /// <param name="plainText">The text to encrypt.</param>
    /// <param name="key">The encryption key (32 bytes for AES-256).</param>
    /// <returns>Base64-encoded encrypted data with IV prepended.</returns>
    public static string Encrypt(string plainText, byte[] key)
    {
        ArgumentNullException.ThrowIfNull(plainText);
        ArgumentNullException.ThrowIfNull(key);

        if (key.Length != KeySize / 8)
            throw new ArgumentException($"Key must be {KeySize / 8} bytes for AES-256.", nameof(key));

        using var aes = Aes.Create();
        aes.KeySize = KeySize;
        aes.Key = key;
        aes.GenerateIV();

        using var encryptor = aes.CreateEncryptor();
        var plainBytes = Encoding.UTF8.GetBytes(plainText);
        var encryptedBytes = encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);

        // Prepend IV to encrypted data
        var result = new byte[aes.IV.Length + encryptedBytes.Length];
        Buffer.BlockCopy(aes.IV, 0, result, 0, aes.IV.Length);
        Buffer.BlockCopy(encryptedBytes, 0, result, aes.IV.Length, encryptedBytes.Length);

        return Convert.ToBase64String(result);
    }

    /// <summary>
    /// Decrypts a string that was encrypted using the Encrypt method.
    /// </summary>
    /// <param name="encryptedText">Base64-encoded encrypted data with IV prepended.</param>
    /// <param name="key">The encryption key (32 bytes for AES-256).</param>
    /// <returns>The decrypted plain text.</returns>
    public static string Decrypt(string encryptedText, byte[] key)
    {
        ArgumentNullException.ThrowIfNull(encryptedText);
        ArgumentNullException.ThrowIfNull(key);

        if (key.Length != KeySize / 8)
            throw new ArgumentException($"Key must be {KeySize / 8} bytes for AES-256.", nameof(key));

        var encryptedBytes = Convert.FromBase64String(encryptedText);

        if (encryptedBytes.Length < IvSize)
            throw new ArgumentException("Encrypted data is too short.", nameof(encryptedText));

        // Extract IV from the beginning
        var iv = new byte[IvSize];
        Buffer.BlockCopy(encryptedBytes, 0, iv, 0, IvSize);

        // Extract encrypted content
        var cipherBytes = new byte[encryptedBytes.Length - IvSize];
        Buffer.BlockCopy(encryptedBytes, IvSize, cipherBytes, 0, cipherBytes.Length);

        using var aes = Aes.Create();
        aes.KeySize = KeySize;
        aes.Key = key;
        aes.IV = iv;

        using var decryptor = aes.CreateDecryptor();
        var decryptedBytes = decryptor.TransformFinalBlock(cipherBytes, 0, cipherBytes.Length);

        return Encoding.UTF8.GetString(decryptedBytes);
    }
}
