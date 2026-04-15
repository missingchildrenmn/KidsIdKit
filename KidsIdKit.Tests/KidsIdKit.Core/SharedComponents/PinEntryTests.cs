using Bunit;
using KidsIdKit.Core.Services;
using KidsIdKit.Core.SharedComponents;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Moq;
using System;
using System.Threading.Tasks;

namespace KidsIdKit.Tests.KidsIdKit.Core.SharedComponents;

public class PinEntryTests : TestContext
{
    private readonly Mock<IPinService> _mockPinService;
    private readonly Mock<IBiometricService> _mockBiometricService;

    public PinEntryTests()
    {
        _mockPinService = new Mock<IPinService>();
        _mockBiometricService = new Mock<IBiometricService>();
        _mockBiometricService.Setup(b => b.IsAvailableAsync()).ReturnsAsync(false);
        Services.AddSingleton(_mockPinService.Object);
        Services.AddSingleton(_mockBiometricService.Object);
    }

    #region Rendering Tests

    [Fact]
    public void SetupMode_RendersCreateYourPinTitle()
    {
        var cut = RenderComponent<PinEntry>(p => p.Add(x => x.IsSetupMode, true));

        var heading = cut.Find("h2");
        Assert.Equal("Create Your PIN", heading.TextContent);
    }

    [Fact]
    public void UnlockMode_RendersEnterYourPinTitle()
    {
        var cut = RenderComponent<PinEntry>(p => p.Add(x => x.IsSetupMode, false));

        var heading = cut.Find("h2");
        Assert.Equal("Enter Your PIN", heading.TextContent);
    }

    [Fact]
    public void SetupMode_NoLegacyData_RendersProtectSubtitle()
    {
        var cut = RenderComponent<PinEntry>(p => p
            .Add(x => x.IsSetupMode, true)
            .Add(x => x.HasLegacyData, false));

        var subtitle = cut.Find(".pin-entry-subtitle");
        Assert.Equal("Set a PIN to protect your children's information", subtitle.TextContent);
    }

    [Fact]
    public void SetupMode_WithLegacyData_RendersSecureSubtitle()
    {
        var cut = RenderComponent<PinEntry>(p => p
            .Add(x => x.IsSetupMode, true)
            .Add(x => x.HasLegacyData, true));

        var subtitle = cut.Find(".pin-entry-subtitle");
        Assert.Equal("Set a PIN to secure your existing data", subtitle.TextContent);
    }

    [Fact]
    public void UnlockMode_RendersUnlockSubtitle()
    {
        var cut = RenderComponent<PinEntry>(p => p.Add(x => x.IsSetupMode, false));

        var subtitle = cut.Find(".pin-entry-subtitle");
        Assert.Equal("Enter your PIN to unlock", subtitle.TextContent);
    }

    [Fact]
    public void RendersSixPinInputs()
    {
        var cut = RenderComponent<PinEntry>(p => p.Add(x => x.IsSetupMode, false));

        var inputs = cut.FindAll("input.pin-digit");
        Assert.Equal(6, inputs.Count);
    }

    [Fact]
    public void PinInputs_HavePasswordType()
    {
        var cut = RenderComponent<PinEntry>(p => p.Add(x => x.IsSetupMode, false));

        var inputs = cut.FindAll("input.pin-digit");
        Assert.All(inputs, input => Assert.Equal("password", input.GetAttribute("type")));
    }

    [Fact]
    public void PinInputs_HaveNumericInputMode()
    {
        var cut = RenderComponent<PinEntry>(p => p.Add(x => x.IsSetupMode, false));

        var inputs = cut.FindAll("input.pin-digit");
        Assert.All(inputs, input => Assert.Equal("numeric", input.GetAttribute("inputmode")));
    }

    [Fact]
    public void SetupMode_ShowsSetPinButtonText()
    {
        var cut = RenderComponent<PinEntry>(p => p.Add(x => x.IsSetupMode, true));

        var button = cut.Find(".pin-actions button.btn-primary");
        Assert.Contains("Set PIN", button.TextContent);
    }

    [Fact]
    public void UnlockMode_ShowsUnlockButtonText()
    {
        var cut = RenderComponent<PinEntry>(p => p.Add(x => x.IsSetupMode, false));

        var button = cut.Find(".pin-actions button.btn-primary");
        Assert.Contains("Unlock", button.TextContent);
    }

    [Fact]
    public void SetupMode_ShowsPinHint()
    {
        var cut = RenderComponent<PinEntry>(p => p.Add(x => x.IsSetupMode, true));

        var hint = cut.Find(".pin-hint");
        Assert.Equal("Enter a 4-6 digit PIN to protect your data", hint.TextContent);
    }

    [Fact]
    public void UnlockMode_DoesNotShowPinHint()
    {
        var cut = RenderComponent<PinEntry>(p => p.Add(x => x.IsSetupMode, false));

        Assert.Throws<Bunit.ElementNotFoundException>(() => cut.Find(".pin-hint"));
    }

    [Fact]
    public void NoSkipDelegate_DoesNotShowSkipSection()
    {
        var cut = RenderComponent<PinEntry>(p => p.Add(x => x.IsSetupMode, false));

        Assert.Throws<Bunit.ElementNotFoundException>(() => cut.Find(".pin-skip-section"));
    }

    [Fact]
    public void WithSkipDelegate_ShowsSkipSection()
    {
        var cut = RenderComponent<PinEntry>(p => p
            .Add(x => x.IsSetupMode, false)
            .Add(x => x.OnSkipToInfo, EventCallback.Factory.Create(this, () => { })));

        var skipSection = cut.Find(".pin-skip-section");
        Assert.NotNull(skipSection);
        Assert.Contains("View safety information without signing in", skipSection.TextContent);
    }

    [Fact]
    public void NoErrorMessage_DoesNotShowAlert()
    {
        var cut = RenderComponent<PinEntry>(p => p.Add(x => x.IsSetupMode, false));

        Assert.Throws<Bunit.ElementNotFoundException>(() => cut.Find(".alert-danger"));
    }

    #endregion

    #region Submit Button State Tests

    [Fact]
    public void SubmitButton_DisabledInitially()
    {
        var cut = RenderComponent<PinEntry>(p => p.Add(x => x.IsSetupMode, false));

        var button = cut.Find(".pin-actions button.btn-primary");
        Assert.True(button.HasAttribute("disabled"));
    }

    [Fact]
    public async Task SubmitButton_EnabledAfterFourDigits()
    {
        var cut = RenderComponent<PinEntry>(p => p.Add(x => x.IsSetupMode, false));

        for (int i = 0; i < 4; i++)
        {
            var inputs = cut.FindAll("input.pin-digit");
            await inputs[i].InputAsync(new ChangeEventArgs { Value = (i + 1).ToString() });
        }

        var button = cut.Find(".pin-actions button.btn-primary");
        Assert.False(button.HasAttribute("disabled"));
    }

    [Fact]
    public async Task SubmitButton_DisabledWithThreeDigits()
    {
        var cut = RenderComponent<PinEntry>(p => p.Add(x => x.IsSetupMode, false));

        for (int i = 0; i < 3; i++)
        {
            var inputs = cut.FindAll("input.pin-digit");
            await inputs[i].InputAsync(new ChangeEventArgs { Value = (i + 1).ToString() });
        }

        var button = cut.Find(".pin-actions button.btn-primary");
        Assert.True(button.HasAttribute("disabled"));
    }

    #endregion

    #region Input Behavior Tests

    [Fact]
    public async Task SingleDigitInput_SetsValueInField()
    {
        var cut = RenderComponent<PinEntry>(p => p.Add(x => x.IsSetupMode, false));
        var inputs = cut.FindAll("input.pin-digit");

        await inputs[0].InputAsync(new ChangeEventArgs { Value = "5" });

        // Re-query after render
        inputs = cut.FindAll("input.pin-digit");
        Assert.Equal("5", inputs[0].GetAttribute("value"));
    }

    [Fact]
    public async Task NonDigitInput_IsRejected()
    {
        var cut = RenderComponent<PinEntry>(p => p.Add(x => x.IsSetupMode, false));
        var inputs = cut.FindAll("input.pin-digit");

        await inputs[0].InputAsync(new ChangeEventArgs { Value = "a" });

        inputs = cut.FindAll("input.pin-digit");
        Assert.NotEqual("a", inputs[0].GetAttribute("value"));
    }

    [Fact]
    public async Task PasteMultipleDigits_DistributesAcrossFields()
    {
        var cut = RenderComponent<PinEntry>(p => p.Add(x => x.IsSetupMode, true));
        var inputs = cut.FindAll("input.pin-digit");

        await inputs[0].InputAsync(new ChangeEventArgs { Value = "1234" });

        inputs = cut.FindAll("input.pin-digit");
        Assert.Equal("1", inputs[0].GetAttribute("value"));
        Assert.Equal("2", inputs[1].GetAttribute("value"));
        Assert.Equal("3", inputs[2].GetAttribute("value"));
        Assert.Equal("4", inputs[3].GetAttribute("value"));
    }

    [Fact]
    public async Task PasteFromMiddleIndex_DistributesCorrectly()
    {
        var cut = RenderComponent<PinEntry>(p => p.Add(x => x.IsSetupMode, true));
        var inputs = cut.FindAll("input.pin-digit");

        await inputs[2].InputAsync(new ChangeEventArgs { Value = "789" });

        inputs = cut.FindAll("input.pin-digit");
        Assert.Equal("7", inputs[2].GetAttribute("value"));
        Assert.Equal("8", inputs[3].GetAttribute("value"));
        Assert.Equal("9", inputs[4].GetAttribute("value"));
    }

    [Fact]
    public async Task PasteMoreThanSixDigits_OnlyFillsAvailableFields()
    {
        var cut = RenderComponent<PinEntry>(p => p.Add(x => x.IsSetupMode, true));
        var inputs = cut.FindAll("input.pin-digit");

        await inputs[0].InputAsync(new ChangeEventArgs { Value = "12345678" });

        inputs = cut.FindAll("input.pin-digit");
        Assert.Equal("1", inputs[0].GetAttribute("value"));
        Assert.Equal("2", inputs[1].GetAttribute("value"));
        Assert.Equal("3", inputs[2].GetAttribute("value"));
        Assert.Equal("4", inputs[3].GetAttribute("value"));
        Assert.Equal("5", inputs[4].GetAttribute("value"));
        Assert.Equal("6", inputs[5].GetAttribute("value"));
    }

    [Fact]
    public async Task PasteWithNonDigits_FiltersToDigitsOnly()
    {
        var cut = RenderComponent<PinEntry>(p => p.Add(x => x.IsSetupMode, true));
        var inputs = cut.FindAll("input.pin-digit");

        await inputs[0].InputAsync(new ChangeEventArgs { Value = "1a2b3c4d" });

        inputs = cut.FindAll("input.pin-digit");
        Assert.Equal("1", inputs[0].GetAttribute("value"));
        Assert.Equal("2", inputs[1].GetAttribute("value"));
        Assert.Equal("3", inputs[2].GetAttribute("value"));
        Assert.Equal("4", inputs[3].GetAttribute("value"));
    }

    [Fact]
    public async Task EmptyInput_DoesNotChangeValue()
    {
        var cut = RenderComponent<PinEntry>(p => p.Add(x => x.IsSetupMode, false));
        var inputs = cut.FindAll("input.pin-digit");

        await inputs[0].InputAsync(new ChangeEventArgs { Value = "" });

        inputs = cut.FindAll("input.pin-digit");
        Assert.True(string.IsNullOrEmpty(inputs[0].GetAttribute("value")));
    }

    [Fact]
    public async Task NullInput_DoesNotChangeValue()
    {
        var cut = RenderComponent<PinEntry>(p => p.Add(x => x.IsSetupMode, false));
        var inputs = cut.FindAll("input.pin-digit");

        await inputs[0].InputAsync(new ChangeEventArgs { Value = null });

        inputs = cut.FindAll("input.pin-digit");
        Assert.True(string.IsNullOrEmpty(inputs[0].GetAttribute("value")));
    }

    #endregion

    #region KeyDown Behavior Tests

    [Fact]
    public async Task EnterKey_WithFourDigits_SubmitsPin()
    {
        _mockPinService.Setup(s => s.ValidatePinAsync("1234")).ReturnsAsync(true);
        var unlocked = false;
        var cut = RenderComponent<PinEntry>(p => p
            .Add(x => x.IsSetupMode, false)
            .Add(x => x.OnUnlocked, EventCallback.Factory.Create(this, () => unlocked = true)));
        for (int i = 0; i < 4; i++)
        {
            var inputs = cut.FindAll("input.pin-digit");
            await inputs[i].InputAsync(new ChangeEventArgs { Value = (i + 1).ToString() });
        }

        var finalInputs = cut.FindAll("input.pin-digit");
        finalInputs[3].KeyDown(new KeyboardEventArgs { Key = "Enter" });

        _mockPinService.Verify(s => s.ValidatePinAsync("1234"), Times.Once);
        Assert.True(unlocked);
    }

    [Fact]
    public async Task EnterKey_WithFewerThanFourDigits_DoesNotSubmit()
    {
        var cut = RenderComponent<PinEntry>(p => p.Add(x => x.IsSetupMode, false));
        var inputs = cut.FindAll("input.pin-digit");

        await inputs[0].InputAsync(new ChangeEventArgs { Value = "1" });
        inputs = cut.FindAll("input.pin-digit");
        await inputs[0].KeyDownAsync(new KeyboardEventArgs { Key = "Enter" });

        _mockPinService.Verify(s => s.ValidatePinAsync(It.IsAny<string>()), Times.Never);
    }

    #endregion

    #region Submit - Setup Mode Tests

    [Fact]
    public async Task SetupMode_Submit_CallsSetPinAsync()
    {
        var unlocked = false;
        var cut = RenderComponent<PinEntry>(p => p
            .Add(x => x.IsSetupMode, true)
            .Add(x => x.HasLegacyData, false)
            .Add(x => x.OnUnlocked, EventCallback.Factory.Create(this, () => unlocked = true)));

        for (int i = 0; i < 4; i++)
        {
            var inputs = cut.FindAll("input.pin-digit");
            await inputs[i].InputAsync(new ChangeEventArgs { Value = (i + 1).ToString() });
        }

        cut.Find(".pin-actions button.btn-primary").Click();

        _mockPinService.Verify(s => s.SetPinAsync("1234"), Times.Once);
        Assert.True(unlocked);
    }

    [Fact]
    public async Task SetupMode_WithLegacyData_Submit_CallsMigrateLegacyDataAsync()
    {
        var unlocked = false;
        var cut = RenderComponent<PinEntry>(p => p
            .Add(x => x.IsSetupMode, true)
            .Add(x => x.HasLegacyData, true)
            .Add(x => x.OnUnlocked, EventCallback.Factory.Create(this, () => unlocked = true)));

        for (int i = 0; i < 4; i++)
        {
            var inputs = cut.FindAll("input.pin-digit");
            await inputs[i].InputAsync(new ChangeEventArgs { Value = (i + 1).ToString() });
        }

        cut.Find(".pin-actions button.btn-primary").Click();

        _mockPinService.Verify(s => s.MigrateLegacyDataAsync("1234"), Times.Once);
        _mockPinService.Verify(s => s.SetPinAsync(It.IsAny<string>()), Times.Never);
        Assert.True(unlocked);
    }

    [Fact]
    public async Task SetupMode_FiveDigitPin_CallsSetPinAsyncWithFiveDigits()
    {
        var unlocked = false;
        var cut = RenderComponent<PinEntry>(p => p
            .Add(x => x.IsSetupMode, true)
            .Add(x => x.OnUnlocked, EventCallback.Factory.Create(this, () => unlocked = true)));

        for (int i = 0; i < 5; i++)
        {
            var inputs = cut.FindAll("input.pin-digit");
            await inputs[i].InputAsync(new ChangeEventArgs { Value = (i + 1).ToString() });
        }

        cut.Find(".pin-actions button.btn-primary").Click();

        _mockPinService.Verify(s => s.SetPinAsync("12345"), Times.Once);
        Assert.True(unlocked);
    }

    #endregion

    #region Submit - Unlock Mode Tests

    [Fact]
    public async Task UnlockMode_ValidPin_CallsValidatePinAsyncAndUnlocks()
    {
        _mockPinService.Setup(s => s.ValidatePinAsync("1234")).ReturnsAsync(true);
        var unlocked = false;
        var cut = RenderComponent<PinEntry>(p => p
            .Add(x => x.IsSetupMode, false)
            .Add(x => x.OnUnlocked, EventCallback.Factory.Create(this, () => unlocked = true)));

        for (int i = 0; i < 4; i++)
        {
            var inputs = cut.FindAll("input.pin-digit");
            await inputs[i].InputAsync(new ChangeEventArgs { Value = (i + 1).ToString() });
        }

        cut.Find(".pin-actions button.btn-primary").Click();

        _mockPinService.Verify(s => s.ValidatePinAsync("1234"), Times.Once);
        Assert.True(unlocked);
    }

    [Fact]
    public async Task UnlockMode_InvalidPin_ShowsError()
    {
        _mockPinService.Setup(s => s.ValidatePinAsync("1234")).ReturnsAsync(false);
        var cut = RenderComponent<PinEntry>(p => p
            .Add(x => x.IsSetupMode, false)
            .Add(x => x.OnUnlocked, EventCallback.Factory.Create(this, () => { })));

        for (int i = 0; i < 4; i++)
        {
            var inputs = cut.FindAll("input.pin-digit");
            await inputs[i].InputAsync(new ChangeEventArgs { Value = (i + 1).ToString() });
        }

        cut.Find(".pin-actions button.btn-primary").Click();

        var alert = cut.Find(".alert-danger");
        Assert.Equal("Incorrect PIN. Please try again.", alert.TextContent);
    }

    [Fact]
    public async Task UnlockMode_InvalidPin_ClearsPinFields()
    {
        _mockPinService.Setup(s => s.ValidatePinAsync("1234")).ReturnsAsync(false);
        var cut = RenderComponent<PinEntry>(p => p
            .Add(x => x.IsSetupMode, false)
            .Add(x => x.OnUnlocked, EventCallback.Factory.Create(this, () => { })));

        for (int i = 0; i < 4; i++)
        {
            var inputs = cut.FindAll("input.pin-digit");
            await inputs[i].InputAsync(new ChangeEventArgs { Value = (i + 1).ToString() });
        }

        cut.Find(".pin-actions button.btn-primary").Click();

        var finalInputs = cut.FindAll("input.pin-digit");
        Assert.All(finalInputs, input =>
            Assert.True(string.IsNullOrEmpty(input.GetAttribute("value"))));
    }

    [Fact]
    public async Task UnlockMode_InvalidPin_DoesNotInvokeOnUnlocked()
    {
        _mockPinService.Setup(s => s.ValidatePinAsync("1234")).ReturnsAsync(false);
        var unlocked = false;
        var cut = RenderComponent<PinEntry>(p => p
            .Add(x => x.IsSetupMode, false)
            .Add(x => x.OnUnlocked, EventCallback.Factory.Create(this, () => unlocked = true)));

        for (int i = 0; i < 4; i++)
        {
            var inputs = cut.FindAll("input.pin-digit");
            await inputs[i].InputAsync(new ChangeEventArgs { Value = (i + 1).ToString() });
        }

        cut.Find(".pin-actions button.btn-primary").Click();

        Assert.False(unlocked);
    }

    #endregion

    #region Auto-Submit Tests

    [Fact]
    public async Task SixthDigitInput_AutoSubmits()
    {
        _mockPinService.Setup(s => s.ValidatePinAsync("123456")).ReturnsAsync(true);
        var unlocked = false;
        var cut = RenderComponent<PinEntry>(p => p
            .Add(x => x.IsSetupMode, false)
            .Add(x => x.OnUnlocked, EventCallback.Factory.Create(this, () => unlocked = true)));
        var inputs = cut.FindAll("input.pin-digit");

        for (int i = 0; i < 6; i++)
        {
            inputs = cut.FindAll("input.pin-digit");
            await inputs[i].InputAsync(new ChangeEventArgs { Value = (i + 1).ToString() });
        }

        _mockPinService.Verify(s => s.ValidatePinAsync("123456"), Times.Once);
        Assert.True(unlocked);
    }

    [Fact]
    public async Task PasteSixDigits_AutoSubmits()
    {
        _mockPinService.Setup(s => s.ValidatePinAsync("123456")).ReturnsAsync(true);
        var unlocked = false;
        var cut = RenderComponent<PinEntry>(p => p
            .Add(x => x.IsSetupMode, false)
            .Add(x => x.OnUnlocked, EventCallback.Factory.Create(this, () => unlocked = true)));
        var inputs = cut.FindAll("input.pin-digit");

        await inputs[0].InputAsync(new ChangeEventArgs { Value = "123456" });

        _mockPinService.Verify(s => s.ValidatePinAsync("123456"), Times.Once);
        Assert.True(unlocked);
    }

    #endregion

    #region Error Handling Tests

    [Fact]
    public async Task PinServiceThrows_ShowsErrorMessage()
    {
        _mockPinService.Setup(s => s.ValidatePinAsync(It.IsAny<string>()))
            .ThrowsAsync(new InvalidOperationException("Service failure"));
        var cut = RenderComponent<PinEntry>(p => p
            .Add(x => x.IsSetupMode, false)
            .Add(x => x.OnUnlocked, EventCallback.Factory.Create(this, () => { })));

        for (int i = 0; i < 4; i++)
        {
            var inputs = cut.FindAll("input.pin-digit");
            await inputs[i].InputAsync(new ChangeEventArgs { Value = (i + 1).ToString() });
        }

        cut.Find(".pin-actions button.btn-primary").Click();

        var alert = cut.Find(".alert-danger");
        Assert.Equal("An error occurred: Service failure", alert.TextContent);
    }

    [Fact]
    public async Task PinServiceThrows_ClearsPinFields()
    {
        _mockPinService.Setup(s => s.ValidatePinAsync(It.IsAny<string>()))
            .ThrowsAsync(new Exception("fail"));
        var cut = RenderComponent<PinEntry>(p => p
            .Add(x => x.IsSetupMode, false)
            .Add(x => x.OnUnlocked, EventCallback.Factory.Create(this, () => { })));

        for (int i = 0; i < 4; i++)
        {
            var inputs = cut.FindAll("input.pin-digit");
            await inputs[i].InputAsync(new ChangeEventArgs { Value = (i + 1).ToString() });
        }

        cut.Find(".pin-actions button.btn-primary").Click();

        var finalInputs = cut.FindAll("input.pin-digit");
        Assert.All(finalInputs, input =>
            Assert.True(string.IsNullOrEmpty(input.GetAttribute("value"))));
    }

    #endregion

    #region PIN Length Info Tests

    [Fact]
    public async Task SetupMode_FourDigitsEntered_ShowsPinLengthInfo()
    {
        var cut = RenderComponent<PinEntry>(p => p.Add(x => x.IsSetupMode, true));

        for (int i = 0; i < 4; i++)
        {
            var inputs = cut.FindAll("input.pin-digit");
            await inputs[i].InputAsync(new ChangeEventArgs { Value = (i + 1).ToString() });
        }

        var info = cut.Find(".pin-length-info");
        Assert.Equal("PIN length: 4 digits", info.TextContent);
    }

    [Fact]
    public async Task SetupMode_SixDigitsEntered_ShowsPinLengthInfo()
    {
        _mockPinService.Setup(s => s.SetPinAsync(It.IsAny<string>())).Returns(Task.CompletedTask);
        var cut = RenderComponent<PinEntry>(p => p
            .Add(x => x.IsSetupMode, true)
            .Add(x => x.OnUnlocked, EventCallback.Factory.Create(this, () => { })));

        for (int i = 0; i < 5; i++)
        {
            var inputs = cut.FindAll("input.pin-digit");
            await inputs[i].InputAsync(new ChangeEventArgs { Value = (i + 1).ToString() });
        }

        var info = cut.Find(".pin-length-info");
        Assert.Equal("PIN length: 5 digits", info.TextContent);
    }

    [Fact]
    public async Task SetupMode_LessThanFourDigits_DoesNotShowPinLengthInfo()
    {
        var cut = RenderComponent<PinEntry>(p => p.Add(x => x.IsSetupMode, true));

        for (int i = 0; i < 3; i++)
        {
            var inputs = cut.FindAll("input.pin-digit");
            await inputs[i].InputAsync(new ChangeEventArgs { Value = (i + 1).ToString() });
        }

        Assert.Throws<Bunit.ElementNotFoundException>(() => cut.Find(".pin-length-info"));
    }

    [Fact]
    public async Task UnlockMode_FourDigitsEntered_DoesNotShowPinLengthInfo()
    {
        var cut = RenderComponent<PinEntry>(p => p.Add(x => x.IsSetupMode, false));

        for (int i = 0; i < 4; i++)
        {
            var inputs = cut.FindAll("input.pin-digit");
            await inputs[i].InputAsync(new ChangeEventArgs { Value = (i + 1).ToString() });
        }

        Assert.Throws<Bunit.ElementNotFoundException>(() => cut.Find(".pin-length-info"));
    }

    #endregion

    #region Skip to Info Tests

    [Fact]
    public void SkipButton_InvokesOnSkipToInfoCallback()
    {
        var skipped = false;
        var cut = RenderComponent<PinEntry>(p => p
            .Add(x => x.IsSetupMode, false)
            .Add(x => x.OnSkipToInfo, EventCallback.Factory.Create(this, () => skipped = true)));

        cut.Find(".btn-skip-info").Click();

        Assert.True(skipped);
    }

    #endregion

    #region Helper Methods

    private async Task EnterPin(IRenderedComponent<PinEntry> cut, string pin)
    {
        var inputs = cut.FindAll("input.pin-digit");
        for (int i = 0; i < pin.Length && i < 6; i++)
        {
            await inputs[i].InputAsync(new ChangeEventArgs { Value = pin[i].ToString() });
            inputs = cut.FindAll("input.pin-digit");
        }
    }

    #endregion
}