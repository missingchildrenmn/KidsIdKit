namespace KidsIdKit.Core.Services;

/// <summary>
/// Manages encryption session state by holding the derived key in memory.
/// Key is lost when the app closes (web) or goes to background (mobile).
/// </summary>
public class SessionService : ISessionService
{
    private byte[]? _derivedKey;
    private int _suppressLockCount;

    public bool IsUnlocked => _derivedKey != null;

    public bool IsLockSuppressed => _suppressLockCount > 0;

    public void BeginSuppressLock() => Interlocked.Increment(ref _suppressLockCount);

    public void EndSuppressLock() => Interlocked.Decrement(ref _suppressLockCount);

    public bool IsInfoOnlyMode { get; private set; }

    public byte[]? DerivedKey => _derivedKey;

    public event Action? OnLockStateChanged;

    public void SetKey(byte[] key)
    {
        if (key == null || key.Length != 32)
        {
            throw new ArgumentException("Key must be exactly 32 bytes", nameof(key));
        }

        _derivedKey = key;
        OnLockStateChanged?.Invoke();
    }

    public void EnableInfoOnlyMode()
    {
        IsInfoOnlyMode = true;
        OnLockStateChanged?.Invoke();
    }

    public void Lock()
    {
        IsInfoOnlyMode = false;
        if (_derivedKey != null)
        {
            // Clear the key from memory
            Array.Clear(_derivedKey, 0, _derivedKey.Length);
            _derivedKey = null;
        }
        OnLockStateChanged?.Invoke();
    }
}
