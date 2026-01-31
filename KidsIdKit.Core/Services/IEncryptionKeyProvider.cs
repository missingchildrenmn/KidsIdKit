namespace KidsIdKit.Core.Services;

/// <summary>
/// Provides the encryption key from the current session.
/// The key is derived from the user's PIN via PBKDF2.
/// </summary>
public interface IEncryptionKeyProvider
{
    /// <summary>
    /// Gets the encryption key from the current session.
    /// Throws InvalidOperationException if session is not unlocked.
    /// </summary>
    byte[] GetKey();
}
