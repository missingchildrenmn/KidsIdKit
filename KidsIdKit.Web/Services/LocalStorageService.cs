using Blazored.LocalStorage;
using KidsIdKit.Core.Services;

namespace KidsIdKit.Web.Services;

/// <summary>
/// Storage service using browser LocalStorage for Blazor WebAssembly.
/// </summary>
public class LocalStorageService(ILocalStorageService localStorage) : IStorageService
{
    public async Task<byte[]?> ReadAsync(string key)
    {
        return await localStorage.GetItemAsync<byte[]>(key);
    }

    public async Task WriteAsync(string key, byte[] data)
    {
        await localStorage.SetItemAsync(key, data);
    }

    public async Task DeleteAsync(string key)
    {
        await localStorage.RemoveItemAsync(key);
    }

    public async Task<bool> ExistsAsync(string key)
    {
        return await localStorage.ContainKeyAsync(key);
    }

    public Task BackupAsync(string key, string backupKey)
    {
        // Browser LocalStorage doesn't support backup - this is a no-op
        return Task.CompletedTask;
    }
}
