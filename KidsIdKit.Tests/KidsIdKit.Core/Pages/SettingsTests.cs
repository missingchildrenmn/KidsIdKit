using Bunit;
using KidsIdKit.Core.Pages;
using KidsIdKit.Core.Services;
using KidsIdKit.Core.SharedComponents;
using Moq;
using System.Threading.Tasks;

namespace KidsIdKit.Tests.KidsIdKit.Core.Pages;

public class SettingsTests : TestContext
{
    private readonly Mock<IPinService> _mockPinService;
    private readonly Mock<IBiometricService> _mockBiometricService;

    public SettingsTests()
    {
        _mockPinService = new Mock<IPinService>();
        _mockBiometricService = new Mock<IBiometricService>();
        Services.AddSingleton(_mockPinService.Object);
        Services.AddSingleton(_mockBiometricService.Object);
        JSInterop.Mode = JSRuntimeMode.Loose;
    }

    [Fact]
    public async Task ToggleOff_WhenBiometricsEnabled_CallsDisableBiometricAsync()
    {
        _mockBiometricService.Setup(b => b.IsAvailableAsync()).ReturnsAsync(true);
        _mockPinService.Setup(p => p.IsBiometricEnabledAsync()).ReturnsAsync(true);

        var cut = RenderComponent<Settings>();

        var editBool = cut.FindComponent<EditBool>();
        await cut.InvokeAsync(() => editBool.Instance.UpdateBool(false));

        _mockPinService.Verify(p => p.DisableBiometricAsync(), Times.Once);
    }

    [Fact]
    public async Task ToggleOn_Cancel_DoesNotCallEnableOrDisableBiometric()
    {
        _mockBiometricService.Setup(b => b.IsAvailableAsync()).ReturnsAsync(true);
        _mockPinService.Setup(p => p.IsBiometricEnabledAsync()).ReturnsAsync(false);

        var cut = RenderComponent<Settings>();

        var editBool = cut.FindComponent<EditBool>();
        await cut.InvokeAsync(() => editBool.Instance.UpdateBool(true));

        var alert = cut.FindComponent<McmAlert>();
        await cut.InvokeAsync(() => alert.Instance.AlertClosedCallback("cancel"));

        _mockPinService.Verify(p => p.EnableBiometricAsync(), Times.Never);
        _mockPinService.Verify(p => p.DisableBiometricAsync(), Times.Never);
    }

    [Fact]
    public async Task ToggleOn_Confirm_WhenBiometricsAvailable_CallsEnableBiometricAsync()
    {
        _mockBiometricService.Setup(b => b.IsAvailableAsync()).ReturnsAsync(true);
        _mockPinService.Setup(p => p.IsBiometricEnabledAsync()).ReturnsAsync(false);

        var cut = RenderComponent<Settings>();

        var editBool = cut.FindComponent<EditBool>();
        await cut.InvokeAsync(() => editBool.Instance.UpdateBool(true));

        var alert = cut.FindComponent<McmAlert>();
        await cut.InvokeAsync(() => alert.Instance.AlertClosedCallback("confirm"));

        _mockPinService.Verify(p => p.EnableBiometricAsync(), Times.Once);
    }

    [Fact]
    public async Task ToggleOn_Confirm_WhenBiometricsUnavailable_DoesNotCallEnableBiometricAsync()
    {
        _mockBiometricService.SetupSequence(b => b.IsAvailableAsync())
            .ReturnsAsync(true)   // Initial check: biometrics appear available so toggle is shown
            .ReturnsAsync(false); // Confirmation check: biometrics become unavailable before enabling
        _mockPinService.Setup(p => p.IsBiometricEnabledAsync()).ReturnsAsync(false);

        var cut = RenderComponent<Settings>();

        var editBool = cut.FindComponent<EditBool>();
        await cut.InvokeAsync(() => editBool.Instance.UpdateBool(true));

        var alert = cut.FindComponent<McmAlert>();
        await cut.InvokeAsync(() => alert.Instance.AlertClosedCallback("confirm"));

        _mockPinService.Verify(p => p.EnableBiometricAsync(), Times.Never);
    }
}
