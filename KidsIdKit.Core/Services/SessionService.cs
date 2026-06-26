namespace KidsIdKit.Core.Services;

using Microsoft.AspNetCore.Components;

/// <summary>
/// Manages encryption session state by holding the derived key in memory.
/// Key is lost when the app closes (web) or goes to background (mobile).
/// </summary>
public class SessionService : ISessionService
{
    protected byte[]? _derivedKey;

    public bool IsUnlocked => _derivedKey != null;

    public bool IsInfoOnlyMode { get; private set; }

    public byte[]? DerivedKey => _derivedKey;

    public DateTime? AppExitTime { get; set; }

    public event Action? OnLockStateChanged;

    public void SetKey(byte[] key)
    {
        if (key == null || key.Length != 32)
        {
            throw new ArgumentException("Key must be exactly 32 bytes long", nameof(key));
        }

        _derivedKey = key;
        OnLockStateChanged?.Invoke();
    }

    public void EnableInfoOnlyMode()
    {
        IsInfoOnlyMode = true;
        OnLockStateChanged?.Invoke();
    }

    public void ExitInfoOnlyMode()
    {
        if (IsInfoOnlyMode)
        {
            IsInfoOnlyMode = false;
            OnLockStateChanged?.Invoke();
        }
    }

    public void NavigateToInfoOnly(NavigationManager navigationManager)
    {
        EnableInfoOnlyMode();

        // Push (do not replace) so the sign-in screen stays in history and the
        // back button returns the user to it rather than exiting the app.
        navigationManager.NavigateTo("/Information", forceLoad: false, replace: false);
    }

    public void LockIfNeeded()
    {
        if (AppExitTime == null || AppExitTime?.AddSeconds(30) < DateTime.UtcNow)
        {
            SignOut();
        }
        AppExitTime = null;
    }

    public void SignOut()
    {
        IsInfoOnlyMode = false;
        if (_derivedKey != null)
        {
            ClearKeyFromMemory(_derivedKey);
        }
        OnLockStateChanged?.Invoke();
    }

    protected virtual void ClearKeyFromMemory(byte[] key)
    {
        Array.Clear(key, 0, key.Length);
        _derivedKey = null;
    }
}
