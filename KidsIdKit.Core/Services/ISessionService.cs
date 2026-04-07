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
    /// Returns true if the user chose to view informational content without signing in.
    /// </summary>
    bool IsInfoOnlyMode { get; }

    /// <summary>
    /// The derived encryption key. Null if session is locked.
    /// </summary>
    byte[]? DerivedKey { get; }

    /// <summary>
    /// Sets the derived encryption key, unlocking the session.
    /// </summary>
    void SetKey(byte[] key);

    /// <summary>
    /// Enables info-only mode, allowing informational pages without a PIN.
    /// </summary>
    void EnableInfoOnlyMode();

    /// <summary>
    /// Clears the encryption key, locking the session.
    /// </summary>
    void Lock();

    /// <summary>
    /// Returns true if session locking is temporarily suppressed
    /// (e.g., while a native picker is open).
    /// </summary>
    bool IsLockSuppressed { get; }

    /// <summary>
    /// Temporarily suppresses session locking. Must be paired with <see cref="EndSuppressLock"/>.
    /// </summary>
    void BeginSuppressLock();

    /// <summary>
    /// Ends the lock suppression started by <see cref="BeginSuppressLock"/>.
    /// </summary>
    void EndSuppressLock();

    /// <summary>
    /// Event fired when the session lock state changes.
    /// </summary>
    event Action? OnLockStateChanged;

    /// <summary>
    /// Gets or sets a value indicating whether the PIN entry was successful.
    /// </summary>
    bool PinSuccess { get; set; }
}