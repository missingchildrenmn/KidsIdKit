using System.Security.Cryptography;
using System.Text;
using KidsIdKit.Core.Services;

namespace KidsIdKit.Mobile.Services;

/// <summary>
/// Encryption service using System.Security.Cryptography for MAUI.
/// </summary>
public class EncryptionService : IEncryptionService
{
    private const int KeySize = 256;
    private const int IvSize = 16;

    public Task<byte[]> GenerateKeyAsync()
    {
        using var aes = Aes.Create();
        aes.KeySize = KeySize;
        aes.GenerateKey();
        return Task.FromResult(aes.Key);
    }

    public Task<byte[]> GenerateSaltAsync(int size = 32)
    {
        var salt = new byte[size];
        RandomNumberGenerator.Fill(salt);
        return Task.FromResult(salt);
    }

    public Task<byte[]> DeriveKeyAsync(string pin, byte[] salt, int iterations = 100_000)
    {
        using var pbkdf2 = new Rfc2898DeriveBytes(
            pin,
            salt,
            iterations,
            HashAlgorithmName.SHA256);

        return Task.FromResult(pbkdf2.GetBytes(KeySize / 8));
    }

    public Task<string> EncryptAsync(string plainText, byte[] key)
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

        return Task.FromResult(Convert.ToBase64String(result));
    }

    public Task<string> DecryptAsync(string encryptedText, byte[] key)
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

        return Task.FromResult(Encoding.UTF8.GetString(decryptedBytes));
    }
}
