namespace KidsIdKit.Data;

public interface IDataAccess
{
    Task<Family?> GetDataAsync();

    Task SaveDataAsync(Family data);

    /// <summary>
    /// Gets the last date/time the data was updated/saved
    /// </summary>
    Task<DateTime?> GetLastUpdatedDateTimeAsync();
}
