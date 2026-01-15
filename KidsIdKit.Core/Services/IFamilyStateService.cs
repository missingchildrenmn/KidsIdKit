using KidsIdKit.Core.Data;

namespace KidsIdKit.Core.Services;

/// <summary>
/// Manages the family state for the application.
/// Replaces the static DataStore pattern with dependency-injected state management.
/// </summary>
public interface IFamilyStateService
{
    /// <summary>
    /// Gets the current family data. May be null if not yet loaded.
    /// </summary>
    Family? Family { get; }

    /// <summary>
    /// Gets whether the family data has been loaded.
    /// </summary>
    bool IsLoaded { get; }

    /// <summary>
    /// Loads the family data from storage.
    /// </summary>
    Task LoadAsync();

    /// <summary>
    /// Saves the current family data to storage.
    /// </summary>
    Task SaveAsync();

    /// <summary>
    /// Gets a child by index with bounds checking.
    /// </summary>
    /// <param name="index">The index of the child.</param>
    /// <returns>The child at the specified index, or null if out of bounds.</returns>
    Child? GetChild(int index);

    /// <summary>
    /// Notifies the service that data has changed and needs to be saved.
    /// </summary>
    void NotifyStateChanged();

    /// <summary>
    /// Event raised when the family state changes.
    /// </summary>
    event Action? OnStateChanged;
}
