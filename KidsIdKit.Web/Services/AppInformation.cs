using KidsIdKit.Core.Services;
using System.Reflection;

namespace KidsIdKit.Web.Services;

public class AppInformation : IAppInformation
{
    public string VersionName
    {
        get
        {
            var assembly = typeof(AppInformation).Assembly;
            var informationalVersion = assembly
                .GetCustomAttribute<System.Reflection.AssemblyInformationalVersionAttribute>()
                ?.InformationalVersion;

            if (!string.IsNullOrWhiteSpace(informationalVersion))
            {
                return informationalVersion;
            }

            return assembly.GetName().Version?.ToString() ?? string.Empty;
        }
    }
}
