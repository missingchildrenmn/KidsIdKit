using KidsIdKit.Core.Services;

namespace KidsIdKit.Services;

public class AppInformation : IAppInformation
{
    public string VersionName
    {
        get
        {
            return AppInfo.Current.VersionString;
        }
    }
}
