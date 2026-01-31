namespace KidsIdKit.Core.Services;

/// <summary>
/// Manages the encryption session state. Holds the derived encryption key in memory.
/// </summary>
public interface ISessionService
{
    /// <summary>
    /// Returns true if a valid encryption key is available (user has entered correct PIN).
    /// </summary>
    bool IsUnlocked { get; }

    /// <summary>
    /// The derived encryption key. Null if session is locked.
    /// </summary>
    byte[]? DerivedKey { get; }

    /// <summary>
    /// Sets the derived encryption key, unlocking the session.
    /// </summary>
    void SetKey(byte[] key);

    /// <summary>
    /// Clears the encryption key, locking the session.
    /// </summary>
    void Lock();

    /// <summary>
    /// Event fired when the session lock state changes.
    /// </summary>
    event Action? OnLockStateChanged;
}
