using KidsIdKit.Core.Services;

namespace KidsIdKit.Mobile.Services;

/// <summary>
/// Provides the encryption key from the current session.
/// The key is derived from the user's PIN via PBKDF2.
/// </summary>
public class EncryptionKeyProvider(ISessionService sessionService) : IEncryptionKeyProvider
{
    public byte[] GetKey()
    {
        if (!sessionService.IsUnlocked || sessionService.DerivedKey == null)
        {
            throw new InvalidOperationException("Session is not unlocked. User must enter PIN first.");
        }

        return sessionService.DerivedKey;
    }
}
