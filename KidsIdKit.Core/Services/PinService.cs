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
    ILogger<PinService> logger) : IPinService
{
    private const string SaltKey = "KidsIdKit_Salt";
    private const string TokenKey = "KidsIdKit_Token";
    private const string LegacyKeyKey = "KidsIdKit_EncKey";
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
        var legacyKeyExists = await storageService.ExistsAsync(LegacyKeyKey);
        var pinIsSet = await IsPinSetAsync();

        // Has legacy data if old key exists but PIN is not yet set
        return legacyKeyExists && !pinIsSet;
    }

    public async Task MigrateLegacyDataAsync(string pin)
    {
        ValidatePin(pin);

        // Read the legacy key
        var legacyKey = await storageService.ReadAsync(LegacyKeyKey);
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
        await storageService.DeleteAsync(LegacyKeyKey);

        logger.LogInformation("Legacy data migrated to PIN-based encryption");
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
}
