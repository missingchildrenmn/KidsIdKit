using KidsIdKit.Data;

namespace KidsIdKit.Data
{
  public interface IDataAccess
  {
    Family? GetData();
    void SaveData(Family data);
  }
}
