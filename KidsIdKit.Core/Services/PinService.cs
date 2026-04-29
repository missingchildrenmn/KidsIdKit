using KidsIdKit.Core.Data;
using Microsoft.Extensions.Logging;

namespace KidsIdKit.Core.Services;

/// <summary>
/// Manages PIN-based authentication and key derivation using PBKDF2.
/// </summary>
public class PinService(
    IStorageService storageService,
    ISessionService sessionService,
    IEncryptionService encryptionService,
    IDataAccess dataAccess,
    IBiometricService biometricService,
    ILogger<PinService> logger) : IPinService
{
    private const string SaltKey = "KidsIdKit_Salt";
    private const string TokenKey = "KidsIdKit_Token";
    private const string LegacyKeyStorageKey = "KidsIdKit_EncKey";
    private const string BiometricKeyStorageKey = "KidsIdKit_BiometricKey";
    private const string VerificationPhrase = "KidsIdKit";

    public async Task<bool> IsPinSetAsync()
    {
        var saltExists = await storageService.ExistsAsync(SaltKey);
        var tokenExists = await storageService.ExistsAsync(TokenKey);
        return saltExists && tokenExists;
    }

    public async Task SetPinAsync(string pin)
    {
        ValidatePin(pin);

        // Generate new salt
        var salt = await encryptionService.GenerateSaltAsync();

        // Derive key from PIN
        var derivedKey = await encryptionService.DeriveKeyAsync(pin, salt);

        // Create verification token by encrypting known phrase
        var token = await encryptionService.EncryptAsync(VerificationPhrase, derivedKey);

        // Store salt and token
        await storageService.WriteAsync(SaltKey, salt);
        await storageService.WriteAsync(TokenKey, System.Text.Encoding.UTF8.GetBytes(token));

        // Unlock the session
        sessionService.SetKey(derivedKey);

        logger.LogInformation("PIN set successfully");
    }

    public async Task<bool> ValidatePinAsync(string pin)
    {
        ValidatePin(pin);

        try
        {
            // Read salt
            var salt = await storageService.ReadAsync(SaltKey);
            if (salt == null)
            {
                logger.LogWarning("Salt not found - PIN not set");
                return false;
            }

            // Read verification token
            var tokenBytes = await storageService.ReadAsync(TokenKey);
            if (tokenBytes == null)
            {
                logger.LogWarning("Verification token not found - PIN not set");
                return false;
            }

            var token = System.Text.Encoding.UTF8.GetString(tokenBytes);

            // Derive key from PIN
            var derivedKey = await encryptionService.DeriveKeyAsync(pin, salt);

            // Try to decrypt verification token
            var decrypted = await encryptionService.DecryptAsync(token, derivedKey);

            if (decrypted == VerificationPhrase)
            {
                // PIN is correct - unlock session
                sessionService.SetKey(derivedKey);
                logger.LogInformation("PIN validated successfully");
                return true;
            }

            logger.LogWarning("PIN validation failed - decrypted phrase mismatch");
            return false;
        }
        catch (Exception ex)
        {
            // Decryption failed - wrong PIN
            logger.LogWarning(ex, "PIN validation failed - decryption error");
            return false;
        }
    }

    public async Task<bool> HasLegacyDataAsync()
    {
        // Check if there's a legacy auto-generated key
        var legacyKeyExists = await storageService.ExistsAsync(LegacyKeyStorageKey);
        var pinIsSet = await IsPinSetAsync();

        // Has legacy data if old key exists but PIN is not yet set
        return legacyKeyExists && !pinIsSet;
    }

    public async Task MigrateLegacyDataAsync(string pin)
    {
        ValidatePin(pin);

        // Read the legacy key
        var legacyKey = await storageService.ReadAsync(LegacyKeyStorageKey);
        if (legacyKey == null)
        {
            throw new InvalidOperationException("No legacy key found to migrate from");
        }

        // Temporarily set the legacy key to decrypt data
        sessionService.SetKey(legacyKey);

        // Read data with the legacy key
        var family = await dataAccess.GetDataAsync();

        // Now set up the new PIN (this creates new key and updates session)
        await SetPinAsync(pin);

        // Re-save the data (will be encrypted with new PIN-derived key)
        if (family != null)
        {
            await dataAccess.SaveDataAsync(family);
        }

        // Delete the legacy key
        await storageService.DeleteAsync(LegacyKeyStorageKey);

        logger.LogInformation("Legacy data migrated to PIN-based encryption");
    }

    public async Task<bool> IsBiometricEnabledAsync()
    {
        return await storageService.ExistsAsync(BiometricKeyStorageKey);
    }

    public async Task EnableBiometricAsync()
    {
        var key = sessionService.DerivedKey
            ?? throw new InvalidOperationException("Session must be unlocked before enabling biometric sign-in");

        await storageService.WriteAsync(BiometricKeyStorageKey, key);
        logger.LogInformation("Biometric sign-in enabled");
    }

    public async Task<bool> ValidateBiometricAsync()
    {
        try
        {
            var keyExists = await storageService.ExistsAsync(BiometricKeyStorageKey);
            if (!keyExists)
            {
                logger.LogWarning("Biometric key not found - biometric sign-in not enabled");
                return false;
            }

            var success = await biometricService.AuthenticateAsync("Verify your identity to unlock");
            if (!success)
            {
                logger.LogInformation("Biometric authentication was not successful");
                return false;
            }

            var key = await storageService.ReadAsync(BiometricKeyStorageKey);
            if (key == null)
            {
                logger.LogWarning("Biometric key was removed during authentication");
                return false;
            }

            sessionService.SetKey(key);
            logger.LogInformation("Biometric authentication succeeded, session unlocked");
            return true;
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Biometric authentication failed");
            return false;
        }
    }

    private static void ValidatePin(string pin)
    {
        if (string.IsNullOrEmpty(pin))
        {
            throw new ArgumentException("PIN cannot be empty", nameof(pin));
        }

        if (pin.Length < 4 || pin.Length > 6)
        {
            throw new ArgumentException("PIN must be 4-6 digits", nameof(pin));
        }

        if (!pin.All(char.IsDigit))
        {
            throw new ArgumentException("PIN must contain only digits", nameof(pin));
        }
    }

    public async Task<IPinService.PinData?> GetPinDataAsync()
    {
        var returnValue = new IPinService.PinData();
        var salt = await storageService.ReadAsync(SaltKey);
        if (salt == null)
        {
            logger.LogWarning("Salt not found - PIN not set");
            return null;
        }
        returnValue.Salt = Convert.ToBase64String(salt);

        var token = await storageService.ReadAsync(TokenKey);
        if (token == null)
        {
            logger.LogWarning("Token not found - PIN not set");
            return null;
        }
        returnValue.Token = Convert.ToBase64String(token);

        return returnValue;
    }

    public async Task SetPinDataAsync(IPinService.PinData pinData)
    {
        if (string.IsNullOrEmpty(pinData.Salt))
        {
            logger.LogWarning("Salt not found - PIN not set");
            return;
        }
        await storageService.WriteAsync(SaltKey, Convert.FromBase64String(pinData.Salt));

        if (string.IsNullOrEmpty(pinData.Token))
        {
            logger.LogWarning("Token not found - PIN not set");
            return;
        }
        await storageService.WriteAsync(TokenKey, Convert.FromBase64String(pinData.Token));

        // Whenever PIN data is set directly, we should clear any biometric key to avoid inconsistencies
        await DisableBiometricAsync();

    }

    public async Task DisableBiometricAsync()
    {
        if (await storageService.ExistsAsync(BiometricKeyStorageKey))
        {
            await storageService.DeleteAsync(BiometricKeyStorageKey);
            logger.LogInformation("Biometric sign-in disabled");
        }
    }
}
