using AngleSharp.Dom;
using Bunit;
using KidsIdKit.Core.Data;
using KidsIdKit.Core.Services;
using KidsIdKit.Core.SharedComponents;
using Moq;
using System.Linq;
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

    private static IElement FindMenuItem(IRenderedComponent<NavMenu> cut, string caption)
        => cut.FindAll("ion-item").Single(item => item.TextContent.Contains(caption));

    public void SignOutMenuItemTests()
    {
        [Fact]
        void Menu_WhenUnlocked_ShowsSignOutOption()
        {
            // Arrange
            var key = new byte[32];
            _sessionService.SetKey(key);

            // Act
            var cut = RenderComponent<NavMenu>();

            // Assert
            var labels = cut.FindAll("ion-label");
            Assert.Contains(labels, label => label.TextContent.Contains("Sign out"));
        }

        [Fact]
        void Menu_WhenUnlocked_SignOutHasLogOutIcon()
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
        void Menu_WhenUnlocked_ShowsAllExpectedMenuItems()
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
        void Menu_WhenInInfoOnlyMode_DoesNotShowSignOutOption()
        {
            // Arrange
            _sessionService.EnableInfoOnlyMode();

            // Act
            var cut = RenderComponent<NavMenu>();

            // Assert
            Assert.DoesNotContain("Sign out", cut.Markup);
            Assert.DoesNotContain("log-out-outline", cut.Markup);
        }
    }

    public void SignOutFunctionalityTests()
    {
        [Fact]
        void SignOutMenuItem_WhenClicked_NavigatesToSignoutPage()
        {
            // Arrange
            var key = new byte[32];
            _sessionService.SetKey(key);

            var cut = RenderComponent<NavMenu>();
            var signOutItem = FindMenuItem(cut, "Sign out");

            // Act
            signOutItem.Click();

            // Assert
            // NavMenu only navigates to the /Signout page. Clearing the session is the
            // responsibility of that page (covered by SignoutTests) and SessionService.SignOut
            // (covered by SessionServiceTests), so the session is still unlocked here.
            Assert.EndsWith("/Signout", _navigationManager.Uri);
            Assert.True(_sessionService.IsUnlocked);
        }
    }

    public void RegularMenuItemTests()
    {
        [Fact]
        void RegularMenuItem_WhenClicked_NavigatesToTargetUri()
        {
            // Arrange
            var key = new byte[32];
            _sessionService.SetKey(key);

            var cut = RenderComponent<NavMenu>();
            var settingsItem = FindMenuItem(cut, "Settings");

            // Act
            settingsItem.Click();

            // Assert
            Assert.EndsWith("/Settings", _navigationManager.Uri);
            // Should still be unlocked
            Assert.True(_sessionService.IsUnlocked);
        }

        [Fact]
        void RegularMenuItem_WhenClicked_DoesNotClearSession()
        {
            // Arrange
            var key = new byte[32];
            _sessionService.SetKey(key);

            var cut = RenderComponent<NavMenu>();
            var aboutItem = FindMenuItem(cut, "About");

            // Act
            aboutItem.Click();

            // Assert
            Assert.True(_sessionService.IsUnlocked);
            Assert.NotNull(_sessionService.DerivedKey);
        }

        [Fact]
        void Menu_WhenUnlocked_ShowsSignOutInGroupB()
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
    }
}