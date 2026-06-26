using KidsIdKit.Core.Services;
using Xunit;

namespace KidsIdKit.Tests.KidsIdKit.Core.Services;

public class SessionServiceTests
{
    private readonly SessionService _sessionService;

    public SessionServiceTests()
    {
        _sessionService = new SessionService();
    }

    #region SignOut Tests

    [Fact]
    public void SignOut_WhenUnlocked_ClearsSessionAndLocks()
    {
        // Arrange
        var key = new byte[32];
        _sessionService.SetKey(key);
        Assert.True(_sessionService.IsUnlocked);

        // Act
        _sessionService.SignOut();

        // Assert
        Assert.False(_sessionService.IsUnlocked);
        Assert.Null(_sessionService.DerivedKey);
    }

    [Fact]
    public void SignOut_WhenInInfoOnlyMode_ExitsInfoOnlyMode()
    {
        // Arrange
        _sessionService.EnableInfoOnlyMode();
        Assert.True(_sessionService.IsInfoOnlyMode);

        // Act
        _sessionService.SignOut();

        // Assert
        Assert.False(_sessionService.IsInfoOnlyMode);
    }

    [Fact]
    public void SignOut_WhenUnlockedAndInInfoOnlyMode_ClearsBothStates()
    {
        // Arrange
        var key = new byte[32];
        _sessionService.SetKey(key);
        _sessionService.EnableInfoOnlyMode();
        Assert.True(_sessionService.IsUnlocked);
        Assert.True(_sessionService.IsInfoOnlyMode);

        // Act
        _sessionService.SignOut();

        // Assert
        Assert.False(_sessionService.IsUnlocked);
        Assert.False(_sessionService.IsInfoOnlyMode);
        Assert.Null(_sessionService.DerivedKey);
    }

    [Fact]
    public void SignOut_FiresOnLockStateChangedEvent()
    {
        // Arrange
        var key = new byte[32];
        _sessionService.SetKey(key);
        var eventFired = false;
        _sessionService.OnLockStateChanged += () => eventFired = true;

        // Act
        _sessionService.SignOut();

        // Assert
        Assert.True(eventFired);
    }

    [Fact]
    public void SignOut_WhenAlreadyLocked_FiresOnLockStateChangedEvent()
    {
        // Arrange
        Assert.False(_sessionService.IsUnlocked);
        var eventFired = false;
        _sessionService.OnLockStateChanged += () => eventFired = true;

        // Act
        _sessionService.SignOut();

        // Assert
        Assert.True(eventFired);
    }

    [Fact]
    public void SignOut_ClearsKeyFromMemory()
    {
        // Arrange
        var key = new byte[32];
        for (int i = 0; i < key.Length; i++)
        {
            key[i] = (byte)(i + 1);
        }
        _sessionService.SetKey(key);

        // Act
        _sessionService.SignOut();

        // Assert
        // Verify the original key array was cleared (all zeros)
        Assert.All(key, b => Assert.Equal(0, b));
        Assert.Null(_sessionService.DerivedKey);
    }

    [Fact]
    public void SignOut_CanBeCalledMultipleTimes()
    {
        // Arrange
        var key = new byte[32];
        _sessionService.SetKey(key);

        // Act & Assert - Should not throw
        _sessionService.SignOut();
        _sessionService.SignOut();
        _sessionService.SignOut();

        Assert.False(_sessionService.IsUnlocked);
        Assert.Null(_sessionService.DerivedKey);
    }

    #endregion

    #region IsUnlocked Tests

    [Fact]
    public void IsUnlocked_WhenKeyIsSet_ReturnsTrue()
    {
        // Arrange
        var key = new byte[32];
        _sessionService.SetKey(key);

        // Assert
        Assert.True(_sessionService.IsUnlocked);
    }

    [Fact]
    public void IsUnlocked_WhenKeyIsNotSet_ReturnsFalse()
    {
        // Assert
        Assert.False(_sessionService.IsUnlocked);
    }

    [Fact]
    public void IsUnlocked_AfterSignOut_ReturnsFalse()
    {
        // Arrange
        var key = new byte[32];
        _sessionService.SetKey(key);
        Assert.True(_sessionService.IsUnlocked);

        // Act
        _sessionService.SignOut();

        // Assert
        Assert.False(_sessionService.IsUnlocked);
    }

    #endregion

    #region IsInfoOnlyMode Tests

    [Fact]
    public void IsInfoOnlyMode_InitiallyFalse()
    {
        // Assert
        Assert.False(_sessionService.IsInfoOnlyMode);
    }

    [Fact]
    public void IsInfoOnlyMode_AfterEnable_ReturnsTrue()
    {
        // Act
        _sessionService.EnableInfoOnlyMode();

        // Assert
        Assert.True(_sessionService.IsInfoOnlyMode);
    }

    [Fact]
    public void IsInfoOnlyMode_AfterSignOut_ReturnsFalse()
    {
        // Arrange
        _sessionService.EnableInfoOnlyMode();
        Assert.True(_sessionService.IsInfoOnlyMode);

        // Act
        _sessionService.SignOut();

        // Assert
        Assert.False(_sessionService.IsInfoOnlyMode);
    }

    #endregion

    #region LockIfNeeded Tests

    [Fact]
    public void LockIfNeeded_ComparesWithSignOut()
    {
        // This test verifies both methods clear the session state similarly

        // Arrange - First session for LockIfNeeded
        var sessionForLock = new SessionService();
        var key1 = new byte[32];
        sessionForLock.SetKey(key1);
        sessionForLock.EnableInfoOnlyMode();

        // Arrange - Second session for SignOut
        var sessionForSignOut = new SessionService();
        var key2 = new byte[32];
        sessionForSignOut.SetKey(key2);
        sessionForSignOut.EnableInfoOnlyMode();

        // Act
        sessionForLock.LockIfNeeded(); // Should lock immediately since AppExitTime is null
        sessionForSignOut.SignOut();

        // Assert - Both should have same end state
        Assert.Equal(sessionForLock.IsUnlocked, sessionForSignOut.IsUnlocked);
        Assert.Equal(sessionForLock.IsInfoOnlyMode, sessionForSignOut.IsInfoOnlyMode);
        Assert.Equal(sessionForLock.DerivedKey, sessionForSignOut.DerivedKey);
    }

    #endregion
}
