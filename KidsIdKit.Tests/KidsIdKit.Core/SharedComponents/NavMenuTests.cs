using AngleSharp.Dom;
using Bunit;
using KidsIdKit.Core.Data;
using KidsIdKit.Core.Services;
using KidsIdKit.Core.SharedComponents;
using Moq;
using System.Linq;
using Xunit;

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

    public class SignOutMenuItemTests : TestContext
    {
        private readonly Mock<IDataAccess> _mockDataAccess;
        private readonly SessionService _sessionService;

        public SignOutMenuItemTests()
        {
            _mockDataAccess = new Mock<IDataAccess>();
            _sessionService = new SessionService();
            Services.AddSingleton<ISessionService>(_sessionService);
            Services.AddSingleton(_mockDataAccess.Object);
            JSInterop.Mode = JSRuntimeMode.Loose;
        }

        [Fact]
        public void WhenUnlocked_MenuShowsSignOutOption()
        {
            var key = new byte[32];
            _sessionService.SetKey(key);
            var cut = RenderComponent<NavMenu>();
            var labels = cut.FindAll("ion-label");
            Assert.Contains(labels, label => label.TextContent.Contains("Sign out"));
        }

        [Fact]
        public void WhenUnlocked_MenuShowsSignOutWithLogOutIcon()
        {
            var key = new byte[32];
            _sessionService.SetKey(key);
            var cut = RenderComponent<NavMenu>();
            var markup = cut.Markup;
            Assert.Contains("log-out-outline", markup);
        }

        [Fact]
        public void WhenUnlocked_MenuShowsAllExpectedMenuItems()
        {
            var key = new byte[32];
            _sessionService.SetKey(key);
            var cut = RenderComponent<NavMenu>();
            Assert.Contains("Kids", cut.Markup);
            Assert.Contains("Information", cut.Markup);
            Assert.Contains("Export Data", cut.Markup);
            Assert.Contains("About", cut.Markup);
            Assert.Contains("Settings", cut.Markup);
            Assert.Contains("Sign out", cut.Markup);
        }

        [Fact]
        public void WhenInInfoOnlyMode_MenuDoesNotShowSignOutOption()
        {
            _sessionService.EnableInfoOnlyMode();
            var cut = RenderComponent<NavMenu>();
            Assert.DoesNotContain("Sign out", cut.Markup);
            Assert.DoesNotContain("log-out-outline", cut.Markup);
        }
    }

    public class SignOutFunctionalityTests : TestContext
    {
        private readonly Mock<IDataAccess> _mockDataAccess;
        private readonly SessionService _sessionService;
        private readonly FakeNavigationManager _navigationManager;

        public SignOutFunctionalityTests()
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

        [Fact]
        public void WhenTapped_SignOutMenuItemNavigatesToSignoutPage()
        {
            var key = new byte[32];
            _sessionService.SetKey(key);
            var cut = RenderComponent<NavMenu>();
            var signOutItem = FindMenuItem(cut, "Sign out");
            signOutItem.Click();
            Assert.EndsWith("/Signout", _navigationManager.Uri);
            Assert.True(_sessionService.IsUnlocked);
        }
    }

    public class RegularMenuItemTests : TestContext
    {
        private readonly Mock<IDataAccess> _mockDataAccess;
        private readonly SessionService _sessionService;
        private readonly FakeNavigationManager _navigationManager;

        public RegularMenuItemTests()
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

        [Fact]
        public void WhenTapped_RegularMenuItemNavigatesToTargetUri()
        {
            var key = new byte[32];
            _sessionService.SetKey(key);
            var cut = RenderComponent<NavMenu>();
            var settingsItem = FindMenuItem(cut, "Settings");
            settingsItem.Click();
            Assert.EndsWith("/Settings", _navigationManager.Uri);
            Assert.True(_sessionService.IsUnlocked);
        }

        [Fact]
        public void WhenTapped_RegularMenuItemDoesNotClearSession()
        {
            var key = new byte[32];
            _sessionService.SetKey(key);
            var cut = RenderComponent<NavMenu>();
            var aboutItem = FindMenuItem(cut, "About");
            aboutItem.Click();
            Assert.True(_sessionService.IsUnlocked);
            Assert.NotNull(_sessionService.DerivedKey);
        }

        [Fact]
        public void WhenUnlocked_MenuShowsSignOutInGroupB()
        {
            var key = new byte[32];
            _sessionService.SetKey(key);
            var cut = RenderComponent<NavMenu>();
            var markup = cut.Markup;
            var settingsIndex = markup.IndexOf("Settings");
            var signOutIndex = markup.IndexOf("Sign out");
            Assert.NotEqual(-1, settingsIndex);
            Assert.NotEqual(-1, signOutIndex);
            Assert.True(signOutIndex > settingsIndex);
        }
    }
}