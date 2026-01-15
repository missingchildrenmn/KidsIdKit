using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using KidsIdKit.Core.Data;
using KidsIdKit.Core.Services;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;

namespace KidsIdKit.Tests.KidsIdKit.Core.Services;

public class FamilyStateServiceTests
{
    private readonly IDataAccess _mockDataAccess;
    private readonly ILogger<FamilyStateService> _mockLogger;
    private readonly FamilyStateService _service;

    public FamilyStateServiceTests()
    {
        _mockDataAccess = Substitute.For<IDataAccess>();
        _mockLogger = Substitute.For<ILogger<FamilyStateService>>();
        _service = new FamilyStateService(_mockDataAccess, _mockLogger);
    }

    #region LoadAsync Tests

    [Fact]
    public async Task LoadAsync_FirstLoad_LoadsDataFromDataAccess()
    {
        // Arrange
        var expectedFamily = new Family
        {
            Children = new List<Child>
            {
                new Child { Id = 1 }
            }
        };
        _mockDataAccess.GetDataAsync().Returns(expectedFamily);

        // Act
        await _service.LoadAsync();

        // Assert
        Assert.True(_service.IsLoaded);
        Assert.NotNull(_service.Family);
        Assert.Same(expectedFamily, _service.Family);
        await _mockDataAccess.Received(1).GetDataAsync();
    }

    [Fact]
    public async Task LoadAsync_WhenDataAccessReturnsNull_CreatesNewFamily()
    {
        // Arrange
        _mockDataAccess.GetDataAsync().Returns((Family?)null);

        // Act
        await _service.LoadAsync();

        // Assert
        Assert.True(_service.IsLoaded);
        Assert.NotNull(_service.Family);
        Assert.Empty(_service.Family.Children);
        await _mockDataAccess.Received(1).GetDataAsync();
    }

    [Fact]
    public async Task LoadAsync_WhenAlreadyLoaded_SkipsLoading()
    {
        // Arrange
        var expectedFamily = new Family();
        _mockDataAccess.GetDataAsync().Returns(expectedFamily);
        await _service.LoadAsync(); // First load
        _mockDataAccess.ClearReceivedCalls();

        // Act
        await _service.LoadAsync(); // Second load attempt

        // Assert
        Assert.True(_service.IsLoaded);
        await _mockDataAccess.DidNotReceive().GetDataAsync();
    }

    [Fact]
    public async Task LoadAsync_RaisesOnStateChangedEvent()
    {
        // Arrange
        var eventRaised = false;
        _service.OnStateChanged += () => eventRaised = true;
        _mockDataAccess.GetDataAsync().Returns(new Family());

        // Act
        await _service.LoadAsync();

        // Assert
        Assert.True(eventRaised);
    }

    #endregion

    #region SaveAsync Tests

    [Fact]
    public async Task SaveAsync_WithValidFamily_SavesDataToDataAccess()
    {
        // Arrange
        var family = new Family
        {
            Children = new List<Child>
            {
                new Child { Id = 1 }
            }
        };
        _mockDataAccess.GetDataAsync().Returns(family);
        await _service.LoadAsync();

        // Act
        await _service.SaveAsync();

        // Assert
        await _mockDataAccess.Received(1).SaveDataAsync(family);
    }

    [Fact]
    public async Task SaveAsync_WithNullFamily_DoesNotSave()
    {
        // Arrange - service not loaded, so family is null

        // Act
        await _service.SaveAsync();

        // Assert
        await _mockDataAccess.DidNotReceive().SaveDataAsync(Arg.Any<Family>());
    }

    [Fact]
    public async Task SaveAsync_RaisesOnStateChangedEvent()
    {
        // Arrange
        var eventRaised = false;
        _mockDataAccess.GetDataAsync().Returns(new Family());
        await _service.LoadAsync();
        _service.OnStateChanged += () => eventRaised = true;
        eventRaised = false; // Reset after LoadAsync

        // Act
        await _service.SaveAsync();

        // Assert
        Assert.True(eventRaised);
    }

    #endregion

    #region GetChild Tests

    [Fact]
    public async Task GetChild_WithValidIndex_ReturnsChild()
    {
        // Arrange
        var expectedChild = new Child { Id = 1 };
        var family = new Family
        {
            Children = new List<Child> { expectedChild }
        };
        _mockDataAccess.GetDataAsync().Returns(family);
        await _service.LoadAsync();

        // Act
        var result = _service.GetChild(0);

        // Assert
        Assert.NotNull(result);
        Assert.Same(expectedChild, result);
    }

    [Fact]
    public async Task GetChild_WithNegativeIndex_ReturnsNull()
    {
        // Arrange
        var family = new Family
        {
            Children = new List<Child> { new Child { Id = 1 } }
        };
        _mockDataAccess.GetDataAsync().Returns(family);
        await _service.LoadAsync();

        // Act
        var result = _service.GetChild(-1);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetChild_WithIndexOutOfBounds_ReturnsNull()
    {
        // Arrange
        var family = new Family
        {
            Children = new List<Child> { new Child { Id = 1 } }
        };
        _mockDataAccess.GetDataAsync().Returns(family);
        await _service.LoadAsync();

        // Act
        var result = _service.GetChild(5);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void GetChild_WithNullFamily_ReturnsNull()
    {
        // Arrange - service not loaded, so family is null

        // Act
        var result = _service.GetChild(0);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetChild_WithEmptyChildrenList_ReturnsNull()
    {
        // Arrange
        var family = new Family
        {
            Children = new List<Child>() // Empty list
        };
        _mockDataAccess.GetDataAsync().Returns(family);
        await _service.LoadAsync();

        // Act
        var result = _service.GetChild(0);

        // Assert
        Assert.Null(result);
    }

    #endregion

    #region NotifyStateChanged Tests

    [Fact]
    public void NotifyStateChanged_RaisesOnStateChangedEvent()
    {
        // Arrange
        var eventRaised = false;
        _service.OnStateChanged += () => eventRaised = true;

        // Act
        _service.NotifyStateChanged();

        // Assert
        Assert.True(eventRaised);
    }

    [Fact]
    public void NotifyStateChanged_WithNoSubscribers_DoesNotThrow()
    {
        // Arrange - no subscribers to OnStateChanged

        // Act & Assert
        var exception = Record.Exception(() => _service.NotifyStateChanged());
        Assert.Null(exception);
    }

    #endregion

    #region Property Tests

    [Fact]
    public void Family_BeforeLoad_ReturnsNull()
    {
        // Act & Assert
        Assert.Null(_service.Family);
    }

    [Fact]
    public void IsLoaded_BeforeLoad_ReturnsFalse()
    {
        // Act & Assert
        Assert.False(_service.IsLoaded);
    }

    [Fact]
    public async Task IsLoaded_AfterLoad_ReturnsTrue()
    {
        // Arrange
        _mockDataAccess.GetDataAsync().Returns(new Family());

        // Act
        await _service.LoadAsync();

        // Assert
        Assert.True(_service.IsLoaded);
    }

    #endregion

    #region Error Handling Tests

    [Fact]
    public async Task LoadAsync_WhenDataAccessThrows_PropagatesException()
    {
        // Arrange
        _mockDataAccess.GetDataAsync().Returns(Task.FromException<Family?>(new Exception("Data access error")));

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _service.LoadAsync());
    }

    [Fact]
    public async Task SaveAsync_WhenDataAccessThrows_PropagatesException()
    {
        // Arrange
        var family = new Family();
        _mockDataAccess.GetDataAsync().Returns(family);
        await _service.LoadAsync();
        _mockDataAccess.SaveDataAsync(Arg.Any<Family>()).Returns(Task.FromException(new Exception("Save error")));

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _service.SaveAsync());
    }

    #endregion
}
