using Bunit;
using KidsIdKit.Core.Data;
using KidsIdKit.Core.Services;
using KidsIdKit.Core.SharedComponents;
using Microsoft.AspNetCore.Components;
using Moq;
using System.Threading.Tasks;
using Xunit;

namespace KidsIdKit.Tests.KidsIdKit.Core.SharedComponents;

public class MainLayoutTests : TestContext
{
    private readonly Mock<IPinService> _mockPinService;
    private readonly Mock<IBiometricService> _mockBiometricService;
    private readonly Mock<IImportService> _mockImportService;
    private readonly Mock<IDataAccess> _mockDataAccess;
    private readonly SessionService _sessionService;
    private readonly FakeNavigationManager _navigationManager;

    public MainLayoutTests()
    {
        _mockPinService = new Mock<IPinService>();
        _mockBiometricService = new Mock<IBiometricService>();
        _mockImportService = new Mock<IImportService>();
        _mockDataAccess = new Mock<IDataAccess>();

        // A PIN is configured (not first-run setup) so the lock screen is the unlock screen.
        _mockPinService.Setup(p => p.IsPinSetAsync()).ReturnsAsync(true);
        _mockPinService.Setup(p => p.HasLegacyDataAsync()).ReturnsAsync(false);
        _mockBiometricService.Setup(b => b.IsAvailableAsync()).ReturnsAsync(false);

        _sessionService = new SessionService();

        Services.AddSingleton<ISessionService>(_sessionService);
        Services.AddSingleton(_mockPinService.Object);
        Services.AddSingleton(_mockBiometricService.Object);
        Services.AddSingleton(_mockImportService.Object);
        Services.AddSingleton(_mockDataAccess.Object);

        JSInterop.Mode = JSRuntimeMode.Loose;
        _navigationManager = Services.GetRequiredService<FakeNavigationManager>();
    }

    [Fact]
    public void Locked_InfoOnlyMode_OnInformationRoute_ShowsBody()
    {
        _sessionService.EnableInfoOnlyMode();
        _navigationManager.NavigateTo("/Information");

        var cut = RenderComponent<MainLayout>(p =>
            p.Add(x => x.Body, BodyFragment("info-body", "Safety")));

        // On an Information route in info-only mode, the content body should render.
        Assert.NotNull(cut.Find(".info-body"));
        Assert.Empty(cut.FindAll(".pin-entry-subtitle"));
    }

    [Fact]
    public void Locked_InfoOnlyMode_NavigatingToProtectedRoute_ShowsLockScreenAndExitsInfoOnly()
    {
        _sessionService.EnableInfoOnlyMode();
        _navigationManager.NavigateTo("/Information");

        var cut = RenderComponent<MainLayout>(p =>
            p.Add(x => x.Body, BodyFragment("protected-body", "Kids")));

        // Simulate tapping the back button to the protected Kids page ("/").
        _navigationManager.NavigateTo("/");

        // Navigation is allowed to land on the protected route (so the back button can
        // return the user to the sign-in screen), but the lock screen must render and
        // info-only mode must be exited so protected content never shows while locked.
        Assert.EndsWith("/", _navigationManager.Uri);
        Assert.False(_sessionService.IsInfoOnlyMode);
        Assert.NotNull(cut.Find(".pin-entry-subtitle"));
        Assert.Empty(cut.FindAll(".protected-body"));
    }

    [Fact]
    public void Locked_NotInfoOnlyMode_OnProtectedRoute_ShowsLockScreen()
    {
        _navigationManager.NavigateTo("/");

        var cut = RenderComponent<MainLayout>(p =>
            p.Add(x => x.Body, BodyFragment("protected-body", "Kids")));

        // Locked and not in info-only mode: the unlock screen shows, not the body.
        Assert.NotNull(cut.Find(".pin-entry-subtitle"));
        Assert.Empty(cut.FindAll(".protected-body"));
    }

    [Fact]
    public void Unlocked_OnProtectedRoute_ShowsBody()
    {
        _sessionService.SetKey(new byte[32]);
        _navigationManager.NavigateTo("/");

        var cut = RenderComponent<MainLayout>(p =>
            p.Add(x => x.Body, BodyFragment("protected-body", "Kids")));

        // Once unlocked, the protected body renders normally.
        Assert.NotNull(cut.Find(".protected-body"));
        Assert.Empty(cut.FindAll(".pin-entry-subtitle"));
    }

    [Fact]
    public void SignOut_WhenUnlocked_LocksSessionAndShowsLockScreen()
    {
        // Arrange - Start unlocked
        _sessionService.SetKey(new byte[32]);
        _navigationManager.NavigateTo("/");

        var cut = RenderComponent<MainLayout>(p =>
            p.Add(x => x.Body, BodyFragment("protected-body", "Kids")));

        // Verify we're unlocked and content is showing
        Assert.True(_sessionService.IsUnlocked);
        Assert.NotNull(cut.Find(".protected-body"));
        Assert.Empty(cut.FindAll(".pin-entry-subtitle"));

        // Act - Sign out
        _sessionService.SignOut();

        // The MainLayout subscribes to SessionService.OnLockStateChanged and re-renders via
        // InvokeAsync(StateHasChanged). Wait for the lock-screen element to appear rather than the
        // session flag, since IsUnlocked flips synchronously inside SignOut() before the re-render
        // happens. Waiting for the element guarantees the unlock UI has actually rendered.
        cut.WaitForElement(".pin-entry-subtitle", timeout: System.TimeSpan.FromSeconds(2));

        // Assert - Now locked and lock screen is showing
        Assert.False(_sessionService.IsUnlocked);
        Assert.NotNull(cut.Find(".pin-entry-subtitle"));
        Assert.Empty(cut.FindAll(".protected-body"));
    }

    private static RenderFragment BodyFragment(string cssClass, string text) => builder =>
    {
        builder.OpenElement(0, "div");
        builder.AddAttribute(1, "class", cssClass);
        builder.AddContent(2, text);
        builder.CloseElement();
    };
}