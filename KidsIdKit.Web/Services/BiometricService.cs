using KidsIdKit.Core.Services;

namespace KidsIdKit.Web.Services;

/// <summary>
/// No-op biometric service for Blazor WebAssembly.
/// Biometric authentication is not available in the browser.
/// </summary>
public class BiometricService : IBiometricService
{
    public Task<bool> IsAvailableAsync() => Task.FromResult(false);

    public Task<bool> AuthenticateAsync(string reason) => Task.FromResult(false);
}