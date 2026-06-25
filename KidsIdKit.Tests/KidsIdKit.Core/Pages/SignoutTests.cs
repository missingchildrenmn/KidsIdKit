using Bunit;
using KidsIdKit.Core.Pages;
using KidsIdKit.Core.Services;
using Xunit;

namespace KidsIdKit.Tests.KidsIdKit.Core.Pages;

public class SignoutTests : TestContext
{
    private readonly SessionService _sessionService;
    private readonly FakeNavigationManager _navigationManager;

    public SignoutTests()
    {
        _sessionService = new SessionService();
        Services.AddSingleton<ISessionService>(_sessionService);
        JSInterop.Mode = JSRuntimeMode.Loose;
        _navigationManager = Services.GetRequiredService<FakeNavigationManager>();
    }

    [Fact]
    public void Signout_OnInitialized_CallsSessionServiceSignOut()
    {
        // Arrange
        var key = new byte[32];
        _sessionService.SetKey(key);
        Assert.True(_sessionService.IsUnlocked);

        // Act
        RenderComponent<Signout>();

        // Assert
        Assert.False(_sessionService.IsUnlocked);
    }

    [Fact]
    public void Signout_OnInitialized_NavigatesToHome()
    {
        // Arrange
        var key = new byte[32];
        _sessionService.SetKey(key);

        // Act
        RenderComponent<Signout>();

        // Assert
        Assert.EndsWith("/", _navigationManager.Uri);
    }

    [Fact]
    public void Signout_OnInitialized_ReplacesHistoryEntry()
    {
        // Arrange
        var key = new byte[32];
        _sessionService.SetKey(key);

        // Act
        RenderComponent<Signout>();

        // Assert
        // Navigation must use replace: true so /Signout isn't kept on the
        // history stack; otherwise, tapping Back would return here and redirect again.
        var navigation = Assert.Single(_navigationManager.History);
        Assert.True(navigation.Options.ReplaceHistoryEntry);
    }
}