namespace KidsIdKit.Core.Services;

using Microsoft.AspNetCore.Components;

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
    /// Exits info-only mode, returning the app to its locked (sign-in) state.
    /// </summary>
    void ExitInfoOnlyMode();

    /// <summary>
    /// Enables info-only mode and navigates to the information(al) pages. The sign-in screen
    /// remains in the navigation history so the back button returns the user to it.
    /// </summary>
    void NavigateToInfoOnly(NavigationManager navigationManager);

    /// <summary>
    /// Locks the session by clearing the encryption key only when the configured locking conditions are met,
    /// such as when enough time has elapsed since the app was last exited; otherwise leaves the session unlocked.
    /// </summary>
    void LockIfNeeded();

    /// <summary>
    /// Event fired when the session lock state changes.
    /// </summary>
    event Action? OnLockStateChanged;

    /// <summary>
    /// Gets or sets the application exit time used to determine session-lock behavior.
    /// </summary>
    DateTime? AppExitTime { get; set; }
}