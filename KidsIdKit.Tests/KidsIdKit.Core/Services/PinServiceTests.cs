using KidsIdKit.Core.Data;
using KidsIdKit.Core.Services;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Text;
using System.Threading.Tasks;

namespace KidsIdKit.Tests.KidsIdKit.Core.Services;

public class PinServiceTests
{
    private readonly Mock<IStorageService> _mockStorageService;
    private readonly Mock<ISessionService> _mockSessionService;
    private readonly Mock<IEncryptionService> _mockEncryptionService;
    private readonly Mock<IDataAccess> _mockDataAccess;
    private readonly Mock<ILogger<PinService>> _mockLogger;
    private readonly PinService _pinService;

    // Test constants
    private const string TestPin = "1234";
    private const string WrongPin = "9999";
    private const string VerificationPhrase = "KidsIdKit";
    private readonly byte[] _testSalt = Encoding.UTF8.GetBytes("test-salt-32-bytes-for-pbkdf2!!");
    private readonly byte[] _testDerivedKey = Encoding.UTF8.GetBytes("test-derived-key-32-bytes-here!");
    private readonly byte[] _wrongDerivedKey = Encoding.UTF8.GetBytes("wrong-derived-key-32-bytes-here");
    private const string TestEncryptedToken = "encrypted-verification-token";

    public PinServiceTests()
    {
        _mockStorageService = new Mock<IStorageService>();
        _mockSessionService = new Mock<ISessionService>();
        _mockEncryptionService = new Mock<IEncryptionService>();
        _mockDataAccess = new Mock<IDataAccess>();
        _mockLogger = new Mock<ILogger<PinService>>();

        _pinService = new PinService(
            _mockStorageService.Object,
            _mockSessionService.Object,
            _mockEncryptionService.Object,
            _mockDataAccess.Object,
            _mockLogger.Object
        );
    }

    #region IsPinSetAsync Tests

    [Fact]
    public async Task IsPinSetAsync_WhenSaltAndTokenExist_ReturnsTrue()
    {
        // Arrange
        _mockStorageService.Setup(s => s.ExistsAsync("KidsIdKit_Salt")).ReturnsAsync(true);
        _mockStorageService.Setup(s => s.ExistsAsync("KidsIdKit_Token")).ReturnsAsync(true);

        // Act
        var result = await _pinService.IsPinSetAsync();

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task IsPinSetAsync_WhenSaltMissing_ReturnsFalse()
    {
        // Arrange
        _mockStorageService.Setup(s => s.ExistsAsync("KidsIdKit_Salt")).ReturnsAsync(false);
        _mockStorageService.Setup(s => s.ExistsAsync("KidsIdKit_Token")).ReturnsAsync(true);

        // Act
        var result = await _pinService.IsPinSetAsync();

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task IsPinSetAsync_WhenTokenMissing_ReturnsFalse()
    {
        // Arrange
        _mockStorageService.Setup(s => s.ExistsAsync("KidsIdKit_Salt")).ReturnsAsync(true);
        _mockStorageService.Setup(s => s.ExistsAsync("KidsIdKit_Token")).ReturnsAsync(false);

        // Act
        var result = await _pinService.IsPinSetAsync();

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task IsPinSetAsync_WhenBothMissing_ReturnsFalse()
    {
        // Arrange
        _mockStorageService.Setup(s => s.ExistsAsync("KidsIdKit_Salt")).ReturnsAsync(false);
        _mockStorageService.Setup(s => s.ExistsAsync("KidsIdKit_Token")).ReturnsAsync(false);

        // Act
        var result = await _pinService.IsPinSetAsync();

        // Assert
        Assert.False(result);
    }

    #endregion

    #region SetPinAsync Tests

    [Fact]
    public async Task SetPinAsync_WithValidPin_CreatesAndStoresSaltAndToken()
    {
        // Arrange
        _mockEncryptionService.Setup(e => e.GenerateSaltAsync(It.IsAny<int>())).ReturnsAsync(_testSalt);
        _mockEncryptionService.Setup(e => e.DeriveKeyAsync(TestPin, _testSalt, It.IsAny<int>())).ReturnsAsync(_testDerivedKey);
        _mockEncryptionService.Setup(e => e.EncryptAsync(VerificationPhrase, _testDerivedKey)).ReturnsAsync(TestEncryptedToken);

        // Act
        await _pinService.SetPinAsync(TestPin);

        // Assert
        _mockEncryptionService.Verify(e => e.GenerateSaltAsync(It.IsAny<int>()), Times.Once);
        _mockEncryptionService.Verify(e => e.DeriveKeyAsync(TestPin, _testSalt, It.IsAny<int>()), Times.Once);
        _mockEncryptionService.Verify(e => e.EncryptAsync(VerificationPhrase, _testDerivedKey), Times.Once);
        _mockStorageService.Verify(s => s.WriteAsync("KidsIdKit_Salt", _testSalt), Times.Once);
        _mockStorageService.Verify(s => s.WriteAsync("KidsIdKit_Token", It.IsAny<byte[]>()), Times.Once);
        _mockSessionService.Verify(s => s.SetKey(_testDerivedKey), Times.Once);
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public async Task SetPinAsync_WithEmptyPin_ThrowsArgumentException(string? pin)
    {
        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(() => _pinService.SetPinAsync(pin!));
        Assert.Contains("PIN cannot be empty", exception.Message);
    }

    [Theory]
    [InlineData("123")]      // Too short
    [InlineData("1234567")]  // Too long
    public async Task SetPinAsync_WithInvalidLength_ThrowsArgumentException(string pin)
    {
        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(() => _pinService.SetPinAsync(pin));
        Assert.Contains("PIN must be 4-6 digits", exception.Message);
    }

    [Theory]
    [InlineData("12a4")]
    [InlineData("abcd")]
    [InlineData("12!4")]
    public async Task SetPinAsync_WithNonDigits_ThrowsArgumentException(string pin)
    {
        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(() => _pinService.SetPinAsync(pin));
        Assert.Contains("PIN must contain only digits", exception.Message);
    }

    #endregion

    #region ValidatePinAsync Tests - Correct PIN

    [Fact]
    public async Task ValidatePinAsync_WithCorrectPin_ReturnsTrue()
    {
        // Arrange
        _mockStorageService.Setup(s => s.ReadAsync("KidsIdKit_Salt")).ReturnsAsync(_testSalt);
        _mockStorageService.Setup(s => s.ReadAsync("KidsIdKit_Token")).ReturnsAsync(Encoding.UTF8.GetBytes(TestEncryptedToken));
        _mockEncryptionService.Setup(e => e.DeriveKeyAsync(TestPin, _testSalt, It.IsAny<int>())).ReturnsAsync(_testDerivedKey);
        _mockEncryptionService.Setup(e => e.DecryptAsync(TestEncryptedToken, _testDerivedKey)).ReturnsAsync(VerificationPhrase);

        // Act
        var result = await _pinService.ValidatePinAsync(TestPin);

        // Assert
        Assert.True(result);
        _mockSessionService.Verify(s => s.SetKey(_testDerivedKey), Times.Once);
    }

    [Fact]
    public async Task ValidatePinAsync_WithCorrectPin_UnlocksSession()
    {
        // Arrange
        _mockStorageService.Setup(s => s.ReadAsync("KidsIdKit_Salt")).ReturnsAsync(_testSalt);
        _mockStorageService.Setup(s => s.ReadAsync("KidsIdKit_Token")).ReturnsAsync(Encoding.UTF8.GetBytes(TestEncryptedToken));
        _mockEncryptionService.Setup(e => e.DeriveKeyAsync(TestPin, _testSalt, It.IsAny<int>())).ReturnsAsync(_testDerivedKey);
        _mockEncryptionService.Setup(e => e.DecryptAsync(TestEncryptedToken, _testDerivedKey)).ReturnsAsync(VerificationPhrase);

        // Act
        await _pinService.ValidatePinAsync(TestPin);

        // Assert
        _mockSessionService.Verify(s => s.SetKey(_testDerivedKey), Times.Once);
    }

    #endregion

    #region ValidatePinAsync Tests - Incorrect PIN

    [Fact]
    public async Task ValidatePinAsync_WithIncorrectPin_ReturnsFalse()
    {
        // Arrange
        _mockStorageService.Setup(s => s.ReadAsync("KidsIdKit_Salt")).ReturnsAsync(_testSalt);
        _mockStorageService.Setup(s => s.ReadAsync("KidsIdKit_Token")).ReturnsAsync(Encoding.UTF8.GetBytes(TestEncryptedToken));
        _mockEncryptionService.Setup(e => e.DeriveKeyAsync(WrongPin, _testSalt, It.IsAny<int>())).ReturnsAsync(_wrongDerivedKey);
        _mockEncryptionService.Setup(e => e.DecryptAsync(TestEncryptedToken, _wrongDerivedKey)).ThrowsAsync(new Exception("Decryption failed"));

        // Act
        var result = await _pinService.ValidatePinAsync(WrongPin);

        // Assert
        Assert.False(result);
        _mockSessionService.Verify(s => s.SetKey(It.IsAny<byte[]>()), Times.Never);
    }

    [Fact]
    public async Task ValidatePinAsync_WithIncorrectPin_DoesNotUnlockSession()
    {
        // Arrange
        _mockStorageService.Setup(s => s.ReadAsync("KidsIdKit_Salt")).ReturnsAsync(_testSalt);
        _mockStorageService.Setup(s => s.ReadAsync("KidsIdKit_Token")).ReturnsAsync(Encoding.UTF8.GetBytes(TestEncryptedToken));
        _mockEncryptionService.Setup(e => e.DeriveKeyAsync(WrongPin, _testSalt, It.IsAny<int>())).ReturnsAsync(_wrongDerivedKey);
        _mockEncryptionService.Setup(e => e.DecryptAsync(TestEncryptedToken, _wrongDerivedKey)).ThrowsAsync(new Exception("Decryption failed"));

        // Act
        await _pinService.ValidatePinAsync(WrongPin);

        // Assert
        _mockSessionService.Verify(s => s.SetKey(It.IsAny<byte[]>()), Times.Never);
    }

    [Fact]
    public async Task ValidatePinAsync_WhenDecryptedPhraseMismatch_ReturnsFalse()
    {
        // Arrange - Decryption succeeds but returns wrong phrase
        _mockStorageService.Setup(s => s.ReadAsync("KidsIdKit_Salt")).ReturnsAsync(_testSalt);
        _mockStorageService.Setup(s => s.ReadAsync("KidsIdKit_Token")).ReturnsAsync(Encoding.UTF8.GetBytes(TestEncryptedToken));
        _mockEncryptionService.Setup(e => e.DeriveKeyAsync(WrongPin, _testSalt, It.IsAny<int>())).ReturnsAsync(_wrongDerivedKey);
        _mockEncryptionService.Setup(e => e.DecryptAsync(TestEncryptedToken, _wrongDerivedKey)).ReturnsAsync("WrongPhrase");

        // Act
        var result = await _pinService.ValidatePinAsync(WrongPin);

        // Assert
        Assert.False(result);
        _mockSessionService.Verify(s => s.SetKey(It.IsAny<byte[]>()), Times.Never);
    }

    #endregion

    #region ValidatePinAsync Tests - Corrupted Storage

    [Fact]
    public async Task ValidatePinAsync_WhenSaltMissing_ReturnsFalse()
    {
        // Arrange
        _mockStorageService.Setup(s => s.ReadAsync("KidsIdKit_Salt")).ReturnsAsync((byte[]?)null);

        // Act
        var result = await _pinService.ValidatePinAsync(TestPin);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task ValidatePinAsync_WhenTokenMissing_ReturnsFalse()
    {
        // Arrange
        _mockStorageService.Setup(s => s.ReadAsync("KidsIdKit_Salt")).ReturnsAsync(_testSalt);
        _mockStorageService.Setup(s => s.ReadAsync("KidsIdKit_Token")).ReturnsAsync((byte[]?)null);

        // Act
        var result = await _pinService.ValidatePinAsync(TestPin);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task ValidatePinAsync_WhenTokenCorrupted_ReturnsFalse()
    {
        // Arrange - Token contains invalid UTF-8 or corrupted data
        var corruptedToken = new byte[] { 0xFF, 0xFE, 0xFD };
        _mockStorageService.Setup(s => s.ReadAsync("KidsIdKit_Salt")).ReturnsAsync(_testSalt);
        _mockStorageService.Setup(s => s.ReadAsync("KidsIdKit_Token")).ReturnsAsync(corruptedToken);
        _mockEncryptionService.Setup(e => e.DeriveKeyAsync(TestPin, _testSalt, It.IsAny<int>())).ReturnsAsync(_testDerivedKey);
        _mockEncryptionService.Setup(e => e.DecryptAsync(It.IsAny<string>(), _testDerivedKey)).ThrowsAsync(new Exception("Invalid format"));

        // Act
        var result = await _pinService.ValidatePinAsync(TestPin);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task ValidatePinAsync_WhenDecryptionThrowsException_ReturnsFalse()
    {
        // Arrange
        _mockStorageService.Setup(s => s.ReadAsync("KidsIdKit_Salt")).ReturnsAsync(_testSalt);
        _mockStorageService.Setup(s => s.ReadAsync("KidsIdKit_Token")).ReturnsAsync(Encoding.UTF8.GetBytes(TestEncryptedToken));
        _mockEncryptionService.Setup(e => e.DeriveKeyAsync(TestPin, _testSalt, It.IsAny<int>())).ReturnsAsync(_testDerivedKey);
        _mockEncryptionService.Setup(e => e.DecryptAsync(TestEncryptedToken, _testDerivedKey)).ThrowsAsync(new InvalidOperationException("Decryption failed"));

        // Act
        var result = await _pinService.ValidatePinAsync(TestPin);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task ValidatePinAsync_WhenStorageThrowsException_ReturnsFalse()
    {
        // Arrange
        _mockStorageService.Setup(s => s.ReadAsync("KidsIdKit_Salt")).ThrowsAsync(new Exception("Storage corrupted"));

        // Act
        var result = await _pinService.ValidatePinAsync(TestPin);

        // Assert
        Assert.False(result);
    }

    #endregion

    #region HasLegacyDataAsync Tests

    [Fact]
    public async Task HasLegacyDataAsync_WhenLegacyKeyExistsAndNoPinSet_ReturnsTrue()
    {
        // Arrange
        _mockStorageService.Setup(s => s.ExistsAsync("KidsIdKit_EncKey")).ReturnsAsync(true);
        _mockStorageService.Setup(s => s.ExistsAsync("KidsIdKit_Salt")).ReturnsAsync(false);
        _mockStorageService.Setup(s => s.ExistsAsync("KidsIdKit_Token")).ReturnsAsync(false);

        // Act
        var result = await _pinService.HasLegacyDataAsync();

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task HasLegacyDataAsync_WhenLegacyKeyExistsButPinSet_ReturnsFalse()
    {
        // Arrange
        _mockStorageService.Setup(s => s.ExistsAsync("KidsIdKit_EncKey")).ReturnsAsync(true);
        _mockStorageService.Setup(s => s.ExistsAsync("KidsIdKit_Salt")).ReturnsAsync(true);
        _mockStorageService.Setup(s => s.ExistsAsync("KidsIdKit_Token")).ReturnsAsync(true);

        // Act
        var result = await _pinService.HasLegacyDataAsync();

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task HasLegacyDataAsync_WhenNoLegacyKey_ReturnsFalse()
    {
        // Arrange
        _mockStorageService.Setup(s => s.ExistsAsync("KidsIdKit_EncKey")).ReturnsAsync(false);
        _mockStorageService.Setup(s => s.ExistsAsync("KidsIdKit_Salt")).ReturnsAsync(false);
        _mockStorageService.Setup(s => s.ExistsAsync("KidsIdKit_Token")).ReturnsAsync(false);

        // Act
        var result = await _pinService.HasLegacyDataAsync();

        // Assert
        Assert.False(result);
    }

    #endregion

    #region MigrateLegacyDataAsync Tests

    [Fact]
    public async Task MigrateLegacyDataAsync_WithValidData_MigratesSuccessfully()
    {
        // Arrange
        var legacyKey = Encoding.UTF8.GetBytes("legacy-key-32-bytes-for-test!!");
        var testFamily = new Family();
        testFamily.Children.Add(new Child { Id = 1 });

        _mockStorageService.Setup(s => s.ReadAsync("KidsIdKit_EncKey")).ReturnsAsync(legacyKey);
        _mockDataAccess.Setup(d => d.GetDataAsync()).ReturnsAsync(testFamily);
        _mockEncryptionService.Setup(e => e.GenerateSaltAsync(It.IsAny<int>())).ReturnsAsync(_testSalt);
        _mockEncryptionService.Setup(e => e.DeriveKeyAsync(TestPin, _testSalt, It.IsAny<int>())).ReturnsAsync(_testDerivedKey);
        _mockEncryptionService.Setup(e => e.EncryptAsync(VerificationPhrase, _testDerivedKey)).ReturnsAsync(TestEncryptedToken);

        // Act
        await _pinService.MigrateLegacyDataAsync(TestPin);

        // Assert
        _mockSessionService.Verify(s => s.SetKey(legacyKey), Times.Once, "Should set legacy key first");
        _mockDataAccess.Verify(d => d.GetDataAsync(), Times.Once, "Should read data with legacy key");
        _mockSessionService.Verify(s => s.SetKey(_testDerivedKey), Times.Once, "Should set new PIN-derived key");
        _mockDataAccess.Verify(d => d.SaveDataAsync(testFamily), Times.Once, "Should save data with new key");
        _mockStorageService.Verify(s => s.DeleteAsync("KidsIdKit_EncKey"), Times.Once, "Should delete legacy key");
    }

    [Fact]
    public async Task MigrateLegacyDataAsync_WhenNoLegacyKey_ThrowsInvalidOperationException()
    {
        // Arrange
        _mockStorageService.Setup(s => s.ReadAsync("KidsIdKit_EncKey")).ReturnsAsync((byte[]?)null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => _pinService.MigrateLegacyDataAsync(TestPin));
        Assert.Contains("No legacy key found", exception.Message);
    }

    [Fact]
    public async Task MigrateLegacyDataAsync_WithNullFamilyData_CompletesWithoutSaving()
    {
        // Arrange
        var legacyKey = Encoding.UTF8.GetBytes("legacy-key-32-bytes-for-test!!");

        _mockStorageService.Setup(s => s.ReadAsync("KidsIdKit_EncKey")).ReturnsAsync(legacyKey);
        _mockDataAccess.Setup(d => d.GetDataAsync()).ReturnsAsync((Family?)null);
        _mockEncryptionService.Setup(e => e.GenerateSaltAsync(It.IsAny<int>())).ReturnsAsync(_testSalt);
        _mockEncryptionService.Setup(e => e.DeriveKeyAsync(TestPin, _testSalt, It.IsAny<int>())).ReturnsAsync(_testDerivedKey);
        _mockEncryptionService.Setup(e => e.EncryptAsync(VerificationPhrase, _testDerivedKey)).ReturnsAsync(TestEncryptedToken);

        // Act
        await _pinService.MigrateLegacyDataAsync(TestPin);

        // Assert
        _mockDataAccess.Verify(d => d.SaveDataAsync(It.IsAny<Family>()), Times.Never, "Should not save null data");
        _mockStorageService.Verify(s => s.DeleteAsync("KidsIdKit_EncKey"), Times.Once, "Should still delete legacy key");
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("123")]
    [InlineData("abc4")]
    public async Task MigrateLegacyDataAsync_WithInvalidPin_ThrowsArgumentException(string? pin)
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _pinService.MigrateLegacyDataAsync(pin!));
    }

    #endregion

    #region PBKDF2 Key Derivation Flow Tests

    [Fact]
    public async Task SetPinAsync_UsesPBKDF2WithCorrectParameters()
    {
        // Arrange
        _mockEncryptionService.Setup(e => e.GenerateSaltAsync(It.IsAny<int>())).ReturnsAsync(_testSalt);
        _mockEncryptionService.Setup(e => e.DeriveKeyAsync(TestPin, _testSalt, 100_000)).ReturnsAsync(_testDerivedKey);
        _mockEncryptionService.Setup(e => e.EncryptAsync(VerificationPhrase, _testDerivedKey)).ReturnsAsync(TestEncryptedToken);

        // Act
        await _pinService.SetPinAsync(TestPin);

        // Assert
        _mockEncryptionService.Verify(e => e.DeriveKeyAsync(TestPin, _testSalt, It.IsAny<int>()), Times.Once);
    }

    [Fact]
    public async Task ValidatePinAsync_UsesSamePBKDF2Process()
    {
        // Arrange
        _mockStorageService.Setup(s => s.ReadAsync("KidsIdKit_Salt")).ReturnsAsync(_testSalt);
        _mockStorageService.Setup(s => s.ReadAsync("KidsIdKit_Token")).ReturnsAsync(Encoding.UTF8.GetBytes(TestEncryptedToken));
        _mockEncryptionService.Setup(e => e.DeriveKeyAsync(TestPin, _testSalt, 100_000)).ReturnsAsync(_testDerivedKey);
        _mockEncryptionService.Setup(e => e.DecryptAsync(TestEncryptedToken, _testDerivedKey)).ReturnsAsync(VerificationPhrase);

        // Act
        await _pinService.ValidatePinAsync(TestPin);

        // Assert
        _mockEncryptionService.Verify(e => e.DeriveKeyAsync(TestPin, _testSalt, It.IsAny<int>()), Times.Once);
    }

    #endregion

    #region Verification Token Tests

    [Fact]
    public async Task SetPinAsync_CreatesVerificationTokenFromKnownPhrase()
    {
        // Arrange
        _mockEncryptionService.Setup(e => e.GenerateSaltAsync(It.IsAny<int>())).ReturnsAsync(_testSalt);
        _mockEncryptionService.Setup(e => e.DeriveKeyAsync(TestPin, _testSalt, It.IsAny<int>())).ReturnsAsync(_testDerivedKey);
        _mockEncryptionService.Setup(e => e.EncryptAsync(VerificationPhrase, _testDerivedKey)).ReturnsAsync(TestEncryptedToken);

        // Act
        await _pinService.SetPinAsync(TestPin);

        // Assert
        _mockEncryptionService.Verify(e => e.EncryptAsync(VerificationPhrase, _testDerivedKey), Times.Once);
    }

    [Fact]
    public async Task ValidatePinAsync_DecryptsVerificationToken()
    {
        // Arrange
        _mockStorageService.Setup(s => s.ReadAsync("KidsIdKit_Salt")).ReturnsAsync(_testSalt);
        _mockStorageService.Setup(s => s.ReadAsync("KidsIdKit_Token")).ReturnsAsync(Encoding.UTF8.GetBytes(TestEncryptedToken));
        _mockEncryptionService.Setup(e => e.DeriveKeyAsync(TestPin, _testSalt, It.IsAny<int>())).ReturnsAsync(_testDerivedKey);
        _mockEncryptionService.Setup(e => e.DecryptAsync(TestEncryptedToken, _testDerivedKey)).ReturnsAsync(VerificationPhrase);

        // Act
        await _pinService.ValidatePinAsync(TestPin);

        // Assert
        _mockEncryptionService.Verify(e => e.DecryptAsync(TestEncryptedToken, _testDerivedKey), Times.Once);
    }

    #endregion
}
