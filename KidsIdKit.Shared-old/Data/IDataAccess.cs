namespace KidsIdKit.Core.Data;

public interface IDataAccess
{
    Task<Family?> GetDataAsync();

    Task SaveDataAsync(Family data);
}
