using KidsIdKit.Core.Data;
using KidsIdKit.Core.Services;
using Microsoft.AspNetCore.Components.Forms;
using NSubstitute;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace KidsIdKit.Tests.KidsIdKit.Core.Services;

public class PhotoServiceTests
{
    private readonly ISessionService _mockSessionService;

    public PhotoServiceTests()
    {
        _mockSessionService = Substitute.For<ISessionService>();
    }

    private PhotoService CreateService(ICameraService? cameraService = null)
    {
        var serviceCollection = new ServiceCollection();
        if (cameraService != null)
        {
            serviceCollection.AddSingleton(cameraService);
        }
        var serviceProvider = serviceCollection.BuildServiceProvider();
        return new PhotoService(serviceProvider, _mockSessionService);
    }

    #region IsCameraAvailable Tests

    [Fact]
    public void IsCameraAvailable_WithoutCameraService_ReturnsFalse()
    {
        var service = CreateService();

        Assert.False(service.IsCameraAvailable);
    }

    [Fact]
    public void IsCameraAvailable_WithCameraService_ReturnsTrue()
    {
        var mockCamera = Substitute.For<ICameraService>();
        var service = CreateService(mockCamera);

        Assert.True(service.IsCameraAvailable);
    }

    #endregion

    #region BeginSuppressLock / EndSuppressLock Pairing Tests

    [Fact]
    public async Task PickPhotoFromCameraAsync_OnSuccess_PairsBeginAndEndSuppressLock()
    {
        var mockCamera = Substitute.For<ICameraService>();
        mockCamera.PickPhotoAsync().Returns(new CameraPhoto(new byte[] { 1, 2, 3 }, "image/jpeg"));
        var service = CreateService(mockCamera);

        await service.PickPhotoFromCameraAsync();

        _mockSessionService.Received(1).BeginSuppressLock();
        _mockSessionService.Received(1).EndSuppressLock();
    }

    [Fact]
    public async Task PickPhotoFromCameraAsync_WhenCancelled_PairsBeginAndEndSuppressLock()
    {
        var mockCamera = Substitute.For<ICameraService>();
        mockCamera.PickPhotoAsync().Returns((CameraPhoto?)null);
        var service = CreateService(mockCamera);

        await service.PickPhotoFromCameraAsync();

        _mockSessionService.Received(1).BeginSuppressLock();
        _mockSessionService.Received(1).EndSuppressLock();
    }

    [Fact]
    public async Task PickPhotoFromCameraAsync_WhenExceptionThrown_PairsBeginAndEndSuppressLock()
    {
        var mockCamera = Substitute.For<ICameraService>();
        mockCamera.PickPhotoAsync().Returns(Task.FromException<CameraPhoto?>(new InvalidOperationException("Camera error")));
        var service = CreateService(mockCamera);

        await Assert.ThrowsAsync<InvalidOperationException>(() => service.PickPhotoFromCameraAsync());

        _mockSessionService.Received(1).BeginSuppressLock();
        _mockSessionService.Received(1).EndSuppressLock();
    }

    [Fact]
    public async Task TakePhotoFromCameraAsync_OnSuccess_PairsBeginAndEndSuppressLock()
    {
        var mockCamera = Substitute.For<ICameraService>();
        mockCamera.TakePhotoAsync().Returns(new CameraPhoto(new byte[] { 1, 2, 3 }, "image/jpeg"));
        var service = CreateService(mockCamera);

        await service.TakePhotoFromCameraAsync();

        _mockSessionService.Received(1).BeginSuppressLock();
        _mockSessionService.Received(1).EndSuppressLock();
    }

    [Fact]
    public async Task TakePhotoFromCameraAsync_WhenExceptionThrown_PairsBeginAndEndSuppressLock()
    {
        var mockCamera = Substitute.For<ICameraService>();
        mockCamera.TakePhotoAsync().Returns(Task.FromException<CameraPhoto?>(new InvalidOperationException("Camera error")));
        var service = CreateService(mockCamera);

        await Assert.ThrowsAsync<InvalidOperationException>(() => service.TakePhotoFromCameraAsync());

        _mockSessionService.Received(1).BeginSuppressLock();
        _mockSessionService.Received(1).EndSuppressLock();
    }

    [Fact]
    public async Task PickPhotoFromCameraAsync_WhenNoCameraService_DoesNotSuppressLock()
    {
        var service = CreateService();

        var result = await service.PickPhotoFromCameraAsync();

        Assert.Null(result);
        _mockSessionService.DidNotReceive().BeginSuppressLock();
        _mockSessionService.DidNotReceive().EndSuppressLock();
    }

    #endregion

    #region Camera Photo Content Tests

    [Theory]
    [InlineData("image/jpeg", "jpg")]
    [InlineData("image/png", "png")]
    [InlineData("image/heic", "heic")]
    [InlineData("image/heif", "heif")]
    [InlineData("image/gif", "gif")]
    [InlineData("image/webp", "webp")]
    [InlineData("image/unknown", "jpg")]
    public async Task PickPhotoFromCameraAsync_UsesContentTypeFromCameraPhoto(string contentType, string expectedExtension)
    {
        var bytes = new byte[] { 10, 20, 30 };
        var mockCamera = Substitute.For<ICameraService>();
        mockCamera.PickPhotoAsync().Returns(new CameraPhoto(bytes, contentType));
        var service = CreateService(mockCamera);

        var result = await service.PickPhotoFromCameraAsync();

        Assert.NotNull(result);
        Assert.EndsWith($".{expectedExtension}", result.FileName);
        Assert.StartsWith($"data:{contentType};base64,", result.ImageSource);
        Assert.Equal($"data:{contentType};base64,{Convert.ToBase64String(bytes)}", result.ImageSource);
    }

    [Theory]
    [InlineData("image/jpeg", "jpg")]
    [InlineData("image/png", "png")]
    [InlineData("image/heic", "heic")]
    [InlineData("image/heif", "heif")]
    [InlineData("image/gif", "gif")]
    [InlineData("image/webp", "webp")]
    [InlineData("image/unknown", "jpg")]
    public async Task TakePhotoFromCameraAsync_UsesContentTypeFromCameraPhoto(string contentType, string expectedExtension)
    {
        var bytes = new byte[] { 10, 20, 30 };
        var mockCamera = Substitute.For<ICameraService>();
        mockCamera.TakePhotoAsync().Returns(new CameraPhoto(bytes, contentType));
        var service = CreateService(mockCamera);

        var result = await service.TakePhotoFromCameraAsync();

        Assert.NotNull(result);
        Assert.EndsWith($".{expectedExtension}", result.FileName);
        Assert.Equal($"data:{contentType};base64,{Convert.ToBase64String(bytes)}", result.ImageSource);
    }

    [Fact]
    public async Task PickPhotoFromCameraAsync_WhenCancelledReturnsNull_ReturnsNullPhoto()
    {
        var mockCamera = Substitute.For<ICameraService>();
        mockCamera.PickPhotoAsync().Returns((CameraPhoto?)null);
        var service = CreateService(mockCamera);

        var result = await service.PickPhotoFromCameraAsync();

        Assert.Null(result);
    }

    #endregion

    #region CreatePhotoFromBrowserFileAsync Tests

    [Fact]
    public async Task CreatePhotoFromBrowserFileAsync_SetsFileNameFromBrowserFile()
    {
        var bytes = new byte[] { 1, 2, 3 };
        var (file, service) = CreateBrowserFileServiceWithMock("photo.jpg", "image/jpeg", bytes);

        var result = await service.CreatePhotoFromBrowserFileAsync(file);

        Assert.Equal("photo.jpg", result.FileName);
    }

    [Fact]
    public async Task CreatePhotoFromBrowserFileAsync_SetsImageSourceWithCorrectDataUri()
    {
        var bytes = new byte[] { 10, 20, 30, 40, 50 };
        var (file, service) = CreateBrowserFileServiceWithMock("photo.png", "image/png", bytes);

        var result = await service.CreatePhotoFromBrowserFileAsync(file);

        Assert.Equal($"data:image/png;base64,{Convert.ToBase64String(bytes)}", result.ImageSource);
    }

    [Fact]
    public async Task CreatePhotoFromBrowserFileAsync_NullFile_ThrowsArgumentNullException()
    {
        var service = CreateService();

        await Assert.ThrowsAsync<ArgumentNullException>(() =>
            service.CreatePhotoFromBrowserFileAsync(null!));
    }

    private (IBrowserFile file, TestableBrowserFilePhotoService service) CreateBrowserFileServiceWithMock(
        string fileName, string contentType, byte[] imageBytes)
    {
        var resizedFile = Substitute.For<IBrowserFile>();
        resizedFile.ContentType.Returns(contentType);
        resizedFile.OpenReadStream(Arg.Any<long>(), Arg.Any<CancellationToken>())
            .Returns(new MemoryStream(imageBytes));

        var file = Substitute.For<IBrowserFile>();
        file.Name.Returns(fileName);
        file.ContentType.Returns(contentType);

        var serviceProvider = new ServiceCollection().BuildServiceProvider();
        var service = new TestableBrowserFilePhotoService(serviceProvider, _mockSessionService, resizedFile);

        return (file, service);
    }

    /// <summary>
    /// Testable subclass that replaces the JS-backed <see cref="IBrowserFile.RequestImageFileAsync"/>
    /// extension method with a controllable substitute, enabling unit tests without a real browser context.
    /// </summary>
    private sealed class TestableBrowserFilePhotoService(
        IServiceProvider sp,
        ISessionService session,
        IBrowserFile resizedFile) : PhotoService(sp, session)
    {
        protected override ValueTask<IBrowserFile> RequestImageFileAsync(
            IBrowserFile file, string format, int maxWidth, int maxHeight)
            => ValueTask.FromResult(resizedFile);
    }

    #endregion
}
