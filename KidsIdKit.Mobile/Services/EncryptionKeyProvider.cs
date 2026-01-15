using KidsIdKit.Core.Services;

namespace KidsIdKit.Mobile.Services;

/// <summary>
/// Provides encryption key storage using MAUI SecureStorage.
/// On iOS this uses the Keychain, on Android it uses the Keystore.
/// </summary>
public class EncryptionKeyProvider : IEncryptionKeyProvider
{
    private const string KeyStorageKey = "KidsIdKit_EncKey";
    private byte[]? _cachedKey;

    public async Task<byte[]> GetOrCreateKeyAsync()
    {
        if (_cachedKey != null)
            return _cachedKey;

        try
        {
            var existingKeyBase64 = await SecureStorage.Default.GetAsync(KeyStorageKey);
            if (!string.IsNullOrEmpty(existingKeyBase64))
            {
                var existingKey = Convert.FromBase64String(existingKeyBase64);
                if (existingKey.Length == 32)
                {
                    _cachedKey = existingKey;
                    return existingKey;
                }
            }
        }
        catch (Exception)
        {
            // SecureStorage may fail on some devices; continue to generate new key
        }

        // Generate new key
        var newKey = EncryptionHelper.GenerateKey();
        try
        {
            await SecureStorage.Default.SetAsync(KeyStorageKey, Convert.ToBase64String(newKey));
        }
        catch (Exception)
        {
            // If we can't store securely, still use the key for this session
            // Data will be encrypted but key will be regenerated next session
        }

        _cachedKey = newKey;
        return newKey;
    }

    public async Task<bool> KeyExistsAsync()
    {
        try
        {
            var key = await SecureStorage.Default.GetAsync(KeyStorageKey);
            return !string.IsNullOrEmpty(key);
        }
        catch
        {
            return false;
        }
    }
}
