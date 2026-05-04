using KidsIdKit.Core.Services;

namespace KidsIdKit.Web.Services;

public class AppInformation : IAppInformation
{
    public string VersionName
    {
        get
        {
            return "1.0.0";
        }
    }
}
