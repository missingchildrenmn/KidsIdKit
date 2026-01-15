using Blazored.LocalStorage;
using KidsIdKit.Core.Services;

namespace KidsIdKit.Web.Services;

/// <summary>
/// Provides encryption key storage using browser localStorage.
/// Note: This provides obfuscation rather than true security since the key
/// is stored alongside the data. For stronger security, consider adding
/// a user-provided password.
/// </summary>
public class EncryptionKeyProvider(ILocalStorageService localStorage) : IEncryptionKeyProvider
{
    private const string KeyStorageKey = "KidsIdKit_EncKey";
    private byte[]? _cachedKey;

    public async Task<byte[]> GetOrCreateKeyAsync()
    {
        if (_cachedKey != null)
            return _cachedKey;

        var existingKey = await localStorage.GetItemAsync<byte[]>(KeyStorageKey);
        if (existingKey != null && existingKey.Length == 32)
        {
            _cachedKey = existingKey;
            return existingKey;
        }

        // Generate new key
        var newKey = EncryptionHelper.GenerateKey();
        await localStorage.SetItemAsync(KeyStorageKey, newKey);
        _cachedKey = newKey;
        return newKey;
    }

    public async Task<bool> KeyExistsAsync()
    {
        var key = await localStorage.GetItemAsync<byte[]>(KeyStorageKey);
        return key != null && key.Length == 32;
    }
}
