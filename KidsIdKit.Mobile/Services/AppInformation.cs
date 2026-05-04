using KidsIdKit.Core.Services;

namespace KidsIdKit.Mobile.Services;

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
