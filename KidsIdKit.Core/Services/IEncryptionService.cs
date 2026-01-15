namespace KidsIdKit.Core.Services;

/// <summary>
/// Platform-specific encryption service. Web uses Web Crypto API, Mobile uses System.Security.Cryptography.
/// </summary>
public interface IEncryptionService
{
    /// <summary>
    /// Generates a random 32-byte encryption key.
    /// </summary>
    Task<byte[]> GenerateKeyAsync();

    /// <summary>
    /// Generates a random salt for key derivation.
    /// </summary>
    Task<byte[]> GenerateSaltAsync(int size = 32);

    /// <summary>
    /// Derives a 32-byte key from a PIN using PBKDF2.
    /// </summary>
    Task<byte[]> DeriveKeyAsync(string pin, byte[] salt, int iterations = 100_000);

    /// <summary>
    /// Encrypts plaintext using AES-256-CBC.
    /// </summary>
    /// <returns>Base64-encoded encrypted data with IV prepended.</returns>
    Task<string> EncryptAsync(string plainText, byte[] key);

    /// <summary>
    /// Decrypts data that was encrypted with EncryptAsync.
    /// </summary>
    Task<string> DecryptAsync(string encryptedText, byte[] key);
}
