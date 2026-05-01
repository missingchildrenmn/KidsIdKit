
namespace KidsIdKit.Core.SharedComponents;

/// <summary>
/// Defines the contract for managing per-page state, including app suspension tracking
/// and a typed collection of named state items.
/// This is a singleton object that is normally cleared when navigating to a new page, 
/// but retains its state across component re-rendering and app suspension events.
/// </summary>
public interface IPageState
{
    /// <summary>
    /// Gets or sets a value indicating whether the application is currently suspended.
    /// </summary>
    bool AppSuspended { get; set; }

    /// <summary>
    /// Removes all state items from the collection.
    /// </summary>
    void ClearStateItems();

    /// <summary>
    /// Returns the number of state items currently held in the collection.
    /// </summary>
    /// <returns>The count of state items.</returns>
    long StateItemCount();

    /// <summary>
    /// Adds a new state item with the given name and initial value if one does not already exist.
    /// If a state item with the same name and type <typeparamref name="T"/> already exists, this call is a no-op.
    /// </summary>
    /// <typeparam name="T">The type of the state item's value.</typeparam>
    /// <param name="name">The unique name that identifies the state item.</param>
    /// <param name="value">The initial value to assign to the state item.</param>
    void InitStateItem<T>(string name, T value);

    /// <summary>
    /// Retrieves the state item with the specified name and type.
    /// </summary>
    /// <typeparam name="T">The type of the state item's value.</typeparam>
    /// <param name="name">The name of the state item to retrieve.</param>
    /// <returns>The matching <see cref="StateItem{T}"/>.</returns>
    /// <exception cref="KeyNotFoundException">
    /// Thrown when no state item with the given name and type <typeparamref name="T"/> exists.
    /// </exception>
    StateItem<T> GetStateItem<T>(string name);

    /// <summary>
    /// Updates the value of an existing state item.
    /// </summary>
    /// <typeparam name="T">The type of the state item's value.</typeparam>
    /// <param name="name">The name of the state item to update.</param>
    /// <param name="value">The new value to assign.</param>
    /// <exception cref="KeyNotFoundException">
    /// Thrown when no state item with the given name and type <typeparamref name="T"/> exists.
    /// Call <see cref="InitStateItem{T}"/> first.
    /// </exception>
    void SetStateItem<T>(string name, T value);

    /// <summary>
    /// Provides a non-generic base contract for a named state item,
    /// allowing heterogeneous collections of <see cref="StateItem{T}"/> instances
    /// to be stored and iterated without knowledge of the concrete value type.
    /// </summary>
    public interface IStateItem
    {
        /// <summary>
        /// Gets the name that uniquely identifies this state item 
        /// within a <see cref="IPageState"/> collection.
        /// </summary>
        string Name { get; }
    }

    /// <summary>
    /// Represents a named, typed value stored within a <see cref="IPageState"/> instance.
    /// </summary>
    /// <typeparam name="T">The type of the stored value.</typeparam>
    /// <param name="name">The unique name that identifies this state item.</param>
    /// <param name="value">The initial value of the state item.</param>
    public class StateItem<T>(string name, T value) : IStateItem
    {
        /// <summary>Gets the name that uniquely identifies this state item.</summary>
        public string Name { get; } = name;

        /// <summary>Gets or sets the value of this state item.</summary>
        public T Value { get; set; } = value;
    }
}
