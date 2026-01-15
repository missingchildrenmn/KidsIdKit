using KidsIdKit.Core.Data;
using Microsoft.Extensions.Logging;

namespace KidsIdKit.Core.Services;

/// <summary>
/// Manages the family state for the application.
/// This service is registered as Scoped to share state across components within a session.
/// </summary>
public class FamilyStateService(IDataAccess dataAccess, ILogger<FamilyStateService> logger) : IFamilyStateService
{
    private Family? _family;

    public Family? Family => _family;

    public bool IsLoaded { get; private set; }

    public event Action? OnStateChanged;

    public async Task LoadAsync()
    {
        if (IsLoaded)
        {
            logger.LogDebug("Family data already loaded, skipping");
            return;
        }

        logger.LogDebug("Loading family data");
        _family = await dataAccess.GetDataAsync();
        _family ??= new Family();
        IsLoaded = true;
        OnStateChanged?.Invoke();
    }

    public async Task SaveAsync()
    {
        if (_family == null)
        {
            logger.LogWarning("Attempted to save null family data");
            return;
        }

        logger.LogDebug("Saving family data");
        await dataAccess.SaveDataAsync(_family);
        OnStateChanged?.Invoke();
    }

    public Child? GetChild(int index)
    {
        if (_family == null)
        {
            logger.LogWarning("Attempted to get child when family is null");
            return null;
        }

        if (index < 0 || index >= _family.Children.Count)
        {
            logger.LogWarning("Child index {Index} out of bounds (count: {Count})", index, _family.Children.Count);
            return null;
        }

        return _family.Children[index];
    }

    public void NotifyStateChanged()
    {
        OnStateChanged?.Invoke();
    }
}
