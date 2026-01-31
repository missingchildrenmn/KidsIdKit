using KidsIdKit.Core.Services;
using Microsoft.JSInterop;

namespace KidsIdKit.Web.Services;

/// <summary>
/// Encryption service using Web Crypto API via JavaScript interop for Blazor WebAssembly.
/// </summary>
public class EncryptionService(IJSRuntime jsRuntime) : IEncryptionService
{
    public async Task<byte[]> GenerateKeyAsync()
    {
        var base64 = await jsRuntime.InvokeAsync<string>("encryptionInterop.generateKey");
        return Convert.FromBase64String(base64);
    }

    public async Task<byte[]> GenerateSaltAsync(int size = 32)
    {
        var base64 = await jsRuntime.InvokeAsync<string>("encryptionInterop.generateSalt", size);
        return Convert.FromBase64String(base64);
    }

    public async Task<byte[]> DeriveKeyAsync(string pin, byte[] salt, int iterations = 100_000)
    {
        var saltBase64 = Convert.ToBase64String(salt);
        var base64 = await jsRuntime.InvokeAsync<string>("encryptionInterop.deriveKey", pin, saltBase64, iterations);
        return Convert.FromBase64String(base64);
    }

    public async Task<string> EncryptAsync(string plainText, byte[] key)
    {
        var keyBase64 = Convert.ToBase64String(key);
        return await jsRuntime.InvokeAsync<string>("encryptionInterop.encrypt", plainText, keyBase64);
    }

    public async Task<string> DecryptAsync(string encryptedText, byte[] key)
    {
        var keyBase64 = Convert.ToBase64String(key);
        return await jsRuntime.InvokeAsync<string>("encryptionInterop.decrypt", encryptedText, keyBase64);
    }
}
