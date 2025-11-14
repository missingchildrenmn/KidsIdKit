namespace KidsIdKit.Core.Data
{
    /// <summary>
    /// Maps to identity principal provided by credentials provider.
    /// TODO: Elaborate for token management, sessions, issuer URI, timestamp last verified, etc.
    /// </summary>
    public class UserIdentity
    {
        public string? ProviderName { get; set; }
        public string? UserIdFromProvider { get; set; }
    }
}
