using Bunit;
using KidsIdKit.Core.Pages;
using KidsIdKit.Core.Services;
using KidsIdKit.Core.SharedComponents;
using Moq;
using NSubstitute;
using System.Threading.Tasks;

namespace KidsIdKit.Tests.KidsIdKit.Core.Pages;

public class SettingsTests : TestContext
{
    private readonly Mock<IPinService> _mockPinService;
    private readonly Mock<IBiometricService> _mockBiometricService;
    private readonly Mock<IPageState> _mockPageState;
    private readonly Mock<ICloudBackupService> _mockCloudBackupService;

    public SettingsTests()
    {
        _mockPinService = new Mock<IPinService>();
        _mockBiometricService = new Mock<IBiometricService>();
        _mockPageState = new Mock<IPageState>();
        _mockCloudBackupService = new Mock<ICloudBackupService>();
        Services.AddSingleton(_mockPinService.Object);
        Services.AddSingleton(_mockBiometricService.Object);
        Services.AddSingleton(_mockPageState.Object);
        Services.AddSingleton(_mockCloudBackupService.Object);
        JSInterop.Mode = JSRuntimeMode.Loose;
    }

    [Fact]
    public async Task ToggleOff_WhenBiometricsEnabled_CallsDisableBiometricAsync()
    {
        _mockBiometricService.Setup(b => b.IsAvailableAsync()).ReturnsAsync(true);
        _mockPinService.Setup(p => p.IsBiometricEnabledAsync()).ReturnsAsync(true);
        _mockPageState.Setup(p => p.GetStateItem<bool>("EnableBiometrics")).Returns(new IPageState.StateItem<bool>("EnableBiometrics", false));
        _mockPageState.Setup(p => p.GetStateItem<bool>("IsBiometricAvailable")).Returns(new IPageState.StateItem<bool>("IsBiometricAvailable", false));
        _mockPageState.Setup(p => p.GetStateItem<bool>("AlertShow")).Returns(new IPageState.StateItem<bool>("AlertShow", false));
        _mockPageState.Setup(p => p.GetStateItem<string>("AlertStateInformation")).Returns(new IPageState.StateItem<string>("AlertStateInformation", string.Empty));
        _mockPageState.Setup(p => p.GetStateItem<string>("AlertTitle")).Returns(new IPageState.StateItem<string>("AlertTitle", string.Empty));
        _mockPageState.Setup(p => p.GetStateItem<string>("AlertMessage")).Returns(new IPageState.StateItem<string>("AlertTitle", string.Empty));
        _mockCloudBackupService.Setup(b => b.IsCloudBackupSupported()).Returns(false);
        _mockCloudBackupService.Setup(b => b.IsCloudBackupEnabledAsync()).ReturnsAsync(false);
        _mockPageState.Setup(p => p.GetStateItem<bool>("EnableCloudBackups")).Returns(new IPageState.StateItem<bool>("EnableCloudBackups", false));
        _mockPageState.Setup(p => p.GetStateItem<bool>("IsCloudBackupSupported")).Returns(new IPageState.StateItem<bool>("IsCloudBackupSupported", false));

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
        _mockPageState.Setup(p => p.GetStateItem<bool>("EnableBiometrics")).Returns(new IPageState.StateItem<bool>("EnableBiometrics", false));
        _mockPageState.Setup(p => p.GetStateItem<bool>("IsBiometricAvailable")).Returns(new IPageState.StateItem<bool>("IsBiometricAvailable", false));
        _mockPageState.Setup(p => p.GetStateItem<bool>("AlertShow")).Returns(new IPageState.StateItem<bool>("AlertShow", false));
        _mockPageState.Setup(p => p.GetStateItem<string>("AlertStateInformation")).Returns(new IPageState.StateItem<string>("AlertStateInformation", string.Empty));
        _mockPageState.Setup(p => p.GetStateItem<string>("AlertTitle")).Returns(new IPageState.StateItem<string>("AlertTitle", string.Empty));
        _mockPageState.Setup(p => p.GetStateItem<string>("AlertMessage")).Returns(new IPageState.StateItem<string>("AlertTitle", string.Empty));
        _mockCloudBackupService.Setup(b => b.IsCloudBackupSupported()).Returns(false);
        _mockCloudBackupService.Setup(b => b.IsCloudBackupEnabledAsync()).ReturnsAsync(false);
        _mockPageState.Setup(p => p.GetStateItem<bool>("EnableCloudBackups")).Returns(new IPageState.StateItem<bool>("EnableCloudBackups", false));
        _mockPageState.Setup(p => p.GetStateItem<bool>("IsCloudBackupSupported")).Returns(new IPageState.StateItem<bool>("IsCloudBackupSupported", false));

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
        _mockPageState.Setup(p => p.GetStateItem<bool>("EnableBiometrics")).Returns(new IPageState.StateItem<bool>("EnableBiometrics", false));
        _mockPageState.Setup(p => p.GetStateItem<bool>("IsBiometricAvailable")).Returns(new IPageState.StateItem<bool>("IsBiometricAvailable", false));
        _mockPageState.Setup(p => p.GetStateItem<bool>("AlertShow")).Returns(new IPageState.StateItem<bool>("AlertShow", false));
        _mockPageState.Setup(p => p.GetStateItem<string>("AlertStateInformation")).Returns(new IPageState.StateItem<string>("AlertStateInformation", "BiometricWarning"));
        _mockPageState.Setup(p => p.GetStateItem<string>("AlertTitle")).Returns(new IPageState.StateItem<string>("AlertTitle", string.Empty));
        _mockPageState.Setup(p => p.GetStateItem<string>("AlertMessage")).Returns(new IPageState.StateItem<string>("AlertTitle", string.Empty));
        _mockCloudBackupService.Setup(b => b.IsCloudBackupSupported()).Returns(false);
        _mockCloudBackupService.Setup(b => b.IsCloudBackupEnabledAsync()).ReturnsAsync(false);
        _mockPageState.Setup(p => p.GetStateItem<bool>("EnableCloudBackups")).Returns(new IPageState.StateItem<bool>("EnableCloudBackups", false));
        _mockPageState.Setup(p => p.GetStateItem<bool>("IsCloudBackupSupported")).Returns(new IPageState.StateItem<bool>("IsCloudBackupSupported", false));

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
        _mockPageState.Setup(p => p.GetStateItem<bool>("EnableBiometrics")).Returns(new IPageState.StateItem<bool>("EnableBiometrics", false));
        _mockPageState.Setup(p => p.GetStateItem<bool>("IsBiometricAvailable")).Returns(new IPageState.StateItem<bool>("IsBiometricAvailable", false));
        _mockPageState.Setup(p => p.GetStateItem<bool>("AlertShow")).Returns(new IPageState.StateItem<bool>("AlertShow", false));
        _mockPageState.Setup(p => p.GetStateItem<string>("AlertStateInformation")).Returns(new IPageState.StateItem<string>("AlertStateInformation", "BiometricWarning"));
        _mockPageState.Setup(p => p.GetStateItem<string>("AlertTitle")).Returns(new IPageState.StateItem<string>("AlertTitle", string.Empty));
        _mockPageState.Setup(p => p.GetStateItem<string>("AlertMessage")).Returns(new IPageState.StateItem<string>("AlertTitle", string.Empty));
        _mockCloudBackupService.Setup(b => b.IsCloudBackupSupported()).Returns(false);
        _mockCloudBackupService.Setup(b => b.IsCloudBackupEnabledAsync()).ReturnsAsync(false);
        _mockPageState.Setup(p => p.GetStateItem<bool>("EnableCloudBackups")).Returns(new IPageState.StateItem<bool>("EnableCloudBackups", false));
        _mockPageState.Setup(p => p.GetStateItem<bool>("IsCloudBackupSupported")).Returns(new IPageState.StateItem<bool>("IsCloudBackupSupported", false));

        var cut = RenderComponent<Settings>();

        var editBool = cut.FindComponent<EditBool>();
        await cut.InvokeAsync(() => editBool.Instance.UpdateBool(true));

        var alert = cut.FindComponent<McmAlert>();
        await cut.InvokeAsync(() => alert.Instance.AlertClosedCallback("confirm"));

        _mockPinService.Verify(p => p.EnableBiometricAsync(), Times.Never);
    }

    [Fact]
    public async Task CloudBackup_ToggleOff_WhenCloudBackupsEnabled_CallsDisableCloudBackupAsync()
    {
        _mockBiometricService.Setup(b => b.IsAvailableAsync()).ReturnsAsync(false);
        _mockPinService.Setup(p => p.IsBiometricEnabledAsync()).ReturnsAsync(false);
        _mockPageState.Setup(p => p.GetStateItem<bool>("EnableBiometrics")).Returns(new IPageState.StateItem<bool>("EnableBiometrics", false));
        _mockPageState.Setup(p => p.GetStateItem<bool>("IsBiometricAvailable")).Returns(new IPageState.StateItem<bool>("IsBiometricAvailable", false));
        _mockPageState.Setup(p => p.GetStateItem<bool>("AlertShow")).Returns(new IPageState.StateItem<bool>("AlertShow", false));
        _mockPageState.Setup(p => p.GetStateItem<string>("AlertStateInformation")).Returns(new IPageState.StateItem<string>("AlertStateInformation", string.Empty));
        _mockPageState.Setup(p => p.GetStateItem<string>("AlertTitle")).Returns(new IPageState.StateItem<string>("AlertTitle", string.Empty));
        _mockPageState.Setup(p => p.GetStateItem<string>("AlertMessage")).Returns(new IPageState.StateItem<string>("AlertMessage", string.Empty));
        _mockCloudBackupService.Setup(b => b.IsCloudBackupSupported()).Returns(true);
        _mockCloudBackupService.Setup(b => b.IsCloudBackupEnabledAsync()).ReturnsAsync(true);
        _mockPageState.Setup(p => p.GetStateItem<bool>("EnableCloudBackups")).Returns(new IPageState.StateItem<bool>("EnableCloudBackups", true));
        _mockPageState.Setup(p => p.GetStateItem<bool>("IsCloudBackupSupported")).Returns(new IPageState.StateItem<bool>("IsCloudBackupSupported", true));

        var cut = RenderComponent<Settings>();

        var editBools = cut.FindComponents<EditBool>();
        var cloudBackupToggle = editBools[1]; // Second toggle is for cloud backups
        await cut.InvokeAsync(() => cloudBackupToggle.Instance.UpdateBool(false));

        _mockCloudBackupService.Verify(c => c.DisableCloudBackupAsync(), Times.Once);
    }

    [Fact]
    public async Task CloudBackup_ToggleOn_Cancel_DoesNotCallEnableOrDisableCloudBackup()
    {
        _mockBiometricService.Setup(b => b.IsAvailableAsync()).ReturnsAsync(false);
        _mockPinService.Setup(p => p.IsBiometricEnabledAsync()).ReturnsAsync(false);
        _mockPageState.Setup(p => p.GetStateItem<bool>("EnableBiometrics")).Returns(new IPageState.StateItem<bool>("EnableBiometrics", false));
        _mockPageState.Setup(p => p.GetStateItem<bool>("IsBiometricAvailable")).Returns(new IPageState.StateItem<bool>("IsBiometricAvailable", false));
        _mockPageState.Setup(p => p.GetStateItem<bool>("AlertShow")).Returns(new IPageState.StateItem<bool>("AlertShow", false));
        _mockPageState.Setup(p => p.GetStateItem<string>("AlertStateInformation")).Returns(new IPageState.StateItem<string>("AlertStateInformation", string.Empty));
        _mockPageState.Setup(p => p.GetStateItem<string>("AlertTitle")).Returns(new IPageState.StateItem<string>("AlertTitle", string.Empty));
        _mockPageState.Setup(p => p.GetStateItem<string>("AlertMessage")).Returns(new IPageState.StateItem<string>("AlertMessage", string.Empty));
        _mockCloudBackupService.Setup(b => b.IsCloudBackupSupported()).Returns(true);
        _mockCloudBackupService.Setup(b => b.IsCloudBackupEnabledAsync()).ReturnsAsync(false);
        _mockPageState.Setup(p => p.GetStateItem<bool>("EnableCloudBackups")).Returns(new IPageState.StateItem<bool>("EnableCloudBackups", false));
        _mockPageState.Setup(p => p.GetStateItem<bool>("IsCloudBackupSupported")).Returns(new IPageState.StateItem<bool>("IsCloudBackupSupported", true));

        var cut = RenderComponent<Settings>();

        var editBools = cut.FindComponents<EditBool>();
        var cloudBackupToggle = editBools[1]; // Second toggle is for cloud backups
        await cut.InvokeAsync(() => cloudBackupToggle.Instance.UpdateBool(true));

        var alert = cut.FindComponent<McmAlert>();
        await cut.InvokeAsync(() => alert.Instance.AlertClosedCallback("cancel"));

        _mockCloudBackupService.Verify(c => c.EnableCloudBackupAsync(), Times.Never);
        _mockCloudBackupService.Verify(c => c.DisableCloudBackupAsync(), Times.Never);
    }

    [Fact]
    public async Task CloudBackup_ToggleOn_Confirm_WhenCloudBackupSupported_CallsEnableCloudBackupAsync()
    {
        _mockBiometricService.Setup(b => b.IsAvailableAsync()).ReturnsAsync(false);
        _mockPinService.Setup(p => p.IsBiometricEnabledAsync()).ReturnsAsync(false);
        _mockPageState.Setup(p => p.GetStateItem<bool>("EnableBiometrics")).Returns(new IPageState.StateItem<bool>("EnableBiometrics", false));
        _mockPageState.Setup(p => p.GetStateItem<bool>("IsBiometricAvailable")).Returns(new IPageState.StateItem<bool>("IsBiometricAvailable", false));
        _mockPageState.Setup(p => p.GetStateItem<bool>("AlertShow")).Returns(new IPageState.StateItem<bool>("AlertShow", false));
        _mockPageState.Setup(p => p.GetStateItem<string>("AlertStateInformation")).Returns(new IPageState.StateItem<string>("AlertStateInformation", "CloudBackupWarning"));
        _mockPageState.Setup(p => p.GetStateItem<string>("AlertTitle")).Returns(new IPageState.StateItem<string>("AlertTitle", string.Empty));
        _mockPageState.Setup(p => p.GetStateItem<string>("AlertMessage")).Returns(new IPageState.StateItem<string>("AlertMessage", string.Empty));
        _mockCloudBackupService.Setup(b => b.IsCloudBackupSupported()).Returns(true);
        _mockCloudBackupService.Setup(b => b.IsCloudBackupEnabledAsync()).ReturnsAsync(false);
        _mockPageState.Setup(p => p.GetStateItem<bool>("EnableCloudBackups")).Returns(new IPageState.StateItem<bool>("EnableCloudBackups", false));
        _mockPageState.Setup(p => p.GetStateItem<bool>("IsCloudBackupSupported")).Returns(new IPageState.StateItem<bool>("IsCloudBackupSupported", true));

        var cut = RenderComponent<Settings>();

        var editBools = cut.FindComponents<EditBool>();
        var cloudBackupToggle = editBools[1]; // Second toggle is for cloud backups
        await cut.InvokeAsync(() => cloudBackupToggle.Instance.UpdateBool(true));

        var alert = cut.FindComponent<McmAlert>();
        await cut.InvokeAsync(() => alert.Instance.AlertClosedCallback("confirm"));

        _mockCloudBackupService.Verify(c => c.EnableCloudBackupAsync(), Times.Once);
    }
}
