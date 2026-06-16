using Bunit;
using KidsIdKit.Core.Data;
using KidsIdKit.Core.Services;
using KidsIdKit.Core.SharedComponents;
using Moq;
using Xunit;

namespace KidsIdKit.Tests.KidsIdKit.Core.SharedComponents;

public class NavMenuTests : TestContext
{
    private readonly Mock<IDataAccess> _mockDataAccess;
    private readonly SessionService _sessionService;
    private readonly FakeNavigationManager _navigationManager;

    public NavMenuTests()
    {
        _mockDataAccess = new Mock<IDataAccess>();
        _sessionService = new SessionService();

        Services.AddSingleton<ISessionService>(_sessionService);
        Services.AddSingleton(_mockDataAccess.Object);

        JSInterop.Mode = JSRuntimeMode.Loose;
        _navigationManager = Services.GetRequiredService<FakeNavigationManager>();
    }

    #region Sign Out Menu Item Tests

    [Fact]
    public void Menu_WhenUnlocked_ShowsSignOutOption()
    {
        // Arrange
        var key = new byte[32];
        _sessionService.SetKey(key);

        // Act
        var cut = RenderComponent<NavMenu>();

        // Assert
        var signOutItem = cut.Find("ion-label:contains('Sign out')");
        Assert.NotNull(signOutItem);
    }

    [Fact]
    public void Menu_WhenUnlocked_SignOutHasLogOutIcon()
    {
        // Arrange
        var key = new byte[32];
        _sessionService.SetKey(key);

        // Act
        var cut = RenderComponent<NavMenu>();

        // Assert
        var markup = cut.Markup;
        Assert.Contains("log-out-outline", markup);
    }

    [Fact]
    public void Menu_WhenUnlocked_ShowsAllExpectedMenuItems()
    {
        // Arrange
        var key = new byte[32];
        _sessionService.SetKey(key);

        // Act
        var cut = RenderComponent<NavMenu>();

        // Assert
        Assert.Contains("Kids", cut.Markup);
        Assert.Contains("Information", cut.Markup);
        Assert.Contains("Export Data", cut.Markup);
        Assert.Contains("About", cut.Markup);
        Assert.Contains("Settings", cut.Markup);
        Assert.Contains("Sign out", cut.Markup);
    }

    [Fact]
    public void Menu_WhenInInfoOnlyMode_DoesNotShowSignOutOption()
    {
        // Arrange
        _sessionService.EnableInfoOnlyMode();

        // Act
        var cut = RenderComponent<NavMenu>();

        // Assert
        Assert.DoesNotContain("Sign out", cut.Markup);
        Assert.DoesNotContain("log-out-outline", cut.Markup);
    }

    #endregion

    #region Sign Out Functionality Tests

    [Fact]
    public void SignOutMenuItem_WhenClicked_ClearsSession()
    {
        // Arrange
        var key = new byte[32];
        _sessionService.SetKey(key);
        Assert.True(_sessionService.IsUnlocked);

        var cut = RenderComponent<NavMenu>();
        var signOutItem = cut.Find("ion-item:has(ion-label:contains('Sign out'))");

        // Act
        signOutItem.Click();

        // Assert
        Assert.False(_sessionService.IsUnlocked);
        Assert.Null(_sessionService.DerivedKey);
    }

    [Fact]
    public void SignOutMenuItem_WhenClicked_NavigatesToHome()
    {
        // Arrange
        var key = new byte[32];
        _sessionService.SetKey(key);
        _navigationManager.NavigateTo("/Settings");

        var cut = RenderComponent<NavMenu>();
        var signOutItem = cut.Find("ion-item:has(ion-label:contains('Sign out'))");

        // Act
        signOutItem.Click();

        // Assert
        Assert.EndsWith("/", _navigationManager.Uri);
    }

    [Fact]
    public void SignOut_ClearsInfoOnlyMode()
    {
        // Arrange
        var key = new byte[32];
        _sessionService.SetKey(key);
        // Note: Don't enable info-only mode before rendering, as sign-out button
        // only shows when NOT in info-only mode
        Assert.False(_sessionService.IsInfoOnlyMode);

        var cut = RenderComponent<NavMenu>();

        // Manually enable info-only mode after rendering (simulating some edge case)
        _sessionService.EnableInfoOnlyMode();
        Assert.True(_sessionService.IsInfoOnlyMode);

        var signOutItem = cut.Find("ion-item:has(ion-label:contains('Sign out'))");

        // Act
        signOutItem.Click();

        // Assert
        Assert.False(_sessionService.IsInfoOnlyMode);
    }

    [Fact]
    public void SignOutMenuItem_WhenClicked_TriggersStateChange()
    {
        // Arrange
        var key = new byte[32];
        _sessionService.SetKey(key);
        var eventFired = false;
        _sessionService.OnLockStateChanged += () => eventFired = true;

        var cut = RenderComponent<NavMenu>();
        var signOutItem = cut.Find("ion-item:has(ion-label:contains('Sign out'))");

        // Act
        signOutItem.Click();

        // Assert
        Assert.True(eventFired);
    }

    [Fact]
    public void SignOutMenuItem_WhenClicked_CausesSessionToLock()
    {
        // Arrange
        var key = new byte[32];
        _sessionService.SetKey(key);

        var cut = RenderComponent<NavMenu>();
        Assert.Contains("Sign out", cut.Markup);

        var signOutItem = cut.Find("ion-item:has(ion-label:contains('Sign out'))");

        // Act
        signOutItem.Click();

        // Assert
        // After sign out, the session should be locked
        Assert.False(_sessionService.IsUnlocked);
        Assert.Null(_sessionService.DerivedKey);
        Assert.False(_sessionService.IsInfoOnlyMode);

        // Note: The menu component doesn't automatically re-render after sign-out
        // because it doesn't subscribe to SessionService.OnLockStateChanged.
        // The MainLayout will handle showing the lock screen instead.
    }

    #endregion

    #region Regular Menu Item Tests

    [Fact]
    public void RegularMenuItem_WhenClicked_NavigatesToTargetUri()
    {
        // Arrange
        var key = new byte[32];
        _sessionService.SetKey(key);

        var cut = RenderComponent<NavMenu>();
        var settingsItem = cut.Find("ion-item:has(ion-label:contains('Settings'))");

        // Act
        settingsItem.Click();

        // Assert
        Assert.EndsWith("/Settings", _navigationManager.Uri);
        // Should still be unlocked
        Assert.True(_sessionService.IsUnlocked);
    }

    [Fact]
    public void RegularMenuItem_WhenClicked_DoesNotClearSession()
    {
        // Arrange
        var key = new byte[32];
        _sessionService.SetKey(key);

        var cut = RenderComponent<NavMenu>();
        var aboutItem = cut.Find("ion-item:has(ion-label:contains('About'))");

        // Act
        aboutItem.Click();

        // Assert
        Assert.True(_sessionService.IsUnlocked);
        Assert.NotNull(_sessionService.DerivedKey);
    }

    #endregion

    #region Menu Item Grouping Tests

    [Fact]
    public void Menu_WhenUnlocked_ShowsSignOutInGroupB()
    {
        // Arrange
        var key = new byte[32];
        _sessionService.SetKey(key);

        // Act
        var cut = RenderComponent<NavMenu>();

        // Assert
        // Sign out should be in the same group as Settings (Group B)
        var markup = cut.Markup;
        var settingsIndex = markup.IndexOf("Settings");
        var signOutIndex = markup.IndexOf("Sign out");

        // Settings and Sign out should both exist
        Assert.NotEqual(-1, settingsIndex);
        Assert.NotEqual(-1, signOutIndex);

        // Sign out should appear after Settings (same group)
        Assert.True(signOutIndex > settingsIndex);
    }

    #endregion
}
