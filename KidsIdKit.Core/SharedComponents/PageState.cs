using static KidsIdKit.Core.SharedComponents.IPageState;

namespace KidsIdKit.Core.SharedComponents;

/// <summary>
/// Default implementation of <see cref="IPageState"/> that stores named, typed state items
/// in an in-memory list for the lifetime of the current page.
/// This is a singleton object that is normally cleared when navigating to a new page, 
/// but retains its state across component re-rendering and app suspension events.
/// </summary>
public class PageState : IPageState
{
    private bool _appSuspended;

    /// <summary>Backing store for all registered state items.</summary>
    private readonly List<object> _stateItems = new();

    /// <inheritdoc/>
    public bool AppSuspended
    {
        get => _appSuspended;
        set => _appSuspended = value;
    }

    /// <inheritdoc/>
    public void ClearStateItems()
    {
        _stateItems.Clear();
    }

    /// <inheritdoc/>
    public long StateItemCount()
    {
        return _stateItems.Count;
    }

    /// <inheritdoc/>
    public void InitStateItem<T>(string name, T value)
    {
        if (_stateItems.OfType<StateItem<T>>().Any(e => e.Name == name))
        {
            return;
        }

        _stateItems.Add(new StateItem<T>(name, value));
    }

    /// <inheritdoc/>
    public StateItem<T> GetStateItem<T>(string name)
    {
        var entry = _stateItems.OfType<StateItem<T>>().FirstOrDefault(e => e.Name == name)
            ?? throw new KeyNotFoundException($"State item '{name}' not found.");

        return entry;
    }

    /// <inheritdoc/>
    public void SetStateItem<T>(string name, T value)
    {
        var entry = _stateItems.OfType<StateItem<T>>().FirstOrDefault(e => e.Name == name)
            ?? throw new KeyNotFoundException($"State item '{name}' not found. Call InitStateItem first.");

        entry.Value = value;
    }
}
