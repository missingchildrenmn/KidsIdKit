namespace KidsIdKit.Core.Services;

/// <summary>
/// Provides encryption key storage. Platform-specific implementations
/// should use secure storage (Keychain, Keystore, etc.) where available.
/// </summary>
public interface IEncryptionKeyProvider
{
    /// <summary>
    /// Gets the encryption key, generating one if it doesn't exist.
    /// </summary>
    Task<byte[]> GetOrCreateKeyAsync();

    /// <summary>
    /// Checks if an encryption key exists.
    /// </summary>
    Task<bool> KeyExistsAsync();
}
