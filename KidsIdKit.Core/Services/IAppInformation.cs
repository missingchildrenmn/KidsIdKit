
namespace KidsIdKit.Core.Services;

/// <summary>
/// Provides access to application-level information such as the current version.
/// Implement this interface per platform to surface native app metadata.
/// </summary>
public interface IAppInformation
{
    /// <summary>
    /// Gets the human-readable version name of the application (e.g. "1.0.3").
    /// </summary>
    string VersionName { get; }
}
