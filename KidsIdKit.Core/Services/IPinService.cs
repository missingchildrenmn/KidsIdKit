namespace KidsIdKit.Core.Services;

/// <summary>
/// Manages PIN-based authentication and key derivation.
/// </summary>
public interface IPinService
{
    /// <summary>
    /// Checks if a PIN has been set (salt and verification token exist in storage).
    /// </summary>
    Task<bool> IsPinSetAsync();

    /// <summary>
    /// Sets up a new PIN. Generates salt, derives key, stores verification token.
    /// Also unlocks the session with the derived key.
    /// </summary>
    /// <param name="pin">The PIN to set (4-6 digits)</param>
    Task SetPinAsync(string pin);

    /// <summary>
    /// Validates the PIN by attempting to decrypt the verification token.
    /// On success, unlocks the session with the derived key.
    /// </summary>
    /// <param name="pin">The PIN to validate</param>
    /// <returns>True if PIN is correct, false otherwise</returns>
    Task<bool> ValidatePinAsync(string pin);

    /// <summary>
    /// Checks if there is existing data that needs migration (encrypted with old auto-generated key).
    /// </summary>
    Task<bool> HasLegacyDataAsync();

    /// <summary>
    /// Migrates legacy data encrypted with auto-generated key to PIN-based encryption.
    /// </summary>
    /// <param name="pin">The new PIN to use</param>
    Task MigrateLegacyDataAsync(string pin);
}
