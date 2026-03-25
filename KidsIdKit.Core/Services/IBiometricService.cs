namespace KidsIdKit.Core.Services;

/// <summary>
/// Provides platform-specific biometric authentication (fingerprint, face recognition).
/// </summary>
public interface IBiometricService
{
    /// <summary>
    /// Returns true if biometric hardware is available and enrolled on the device.
    /// </summary>
    Task<bool> IsAvailableAsync();

    /// <summary>
    /// Prompts the user for biometric authentication.
    /// </summary>
    /// <param name="reason">Description shown to the user explaining why authentication is needed.</param>
    /// <returns>True if authentication succeeded, false otherwise.</returns>
    Task<bool> AuthenticateAsync(string reason);
}
