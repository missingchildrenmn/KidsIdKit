using KidsIdKit.Core.Data;
using KidsIdKit.Core.Services;
using NSubstitute;
using System;
using System.Threading.Tasks;
using System.Xml;
using static KidsIdKit.Core.Services.IExportService;

namespace KidsIdKit.Tests.KidsIdKit.Core.Services;

public class ExportServiceTests
{
    private readonly IPinService _mockPinService;
    private readonly IDataAccess _mockDataAccess;
    private readonly IFileSaverService _mockFileSaver;
    private readonly ExportService _service;

    private static readonly IPinService.PinData BasePinData = new()
    {
        Token = "test-token",
        Salt = "test-salt",
    };

    private const string FamilyData = "encrypted-family-data";
    private const string FileName = "backup.xml";

    public ExportServiceTests()
    {
        _mockPinService = Substitute.For<IPinService>();
        _mockDataAccess = Substitute.For<IDataAccess>();
        _mockFileSaver = Substitute.For<IFileSaverService>();
        _service = new ExportService(_mockPinService, _mockDataAccess, _mockFileSaver);
    }

    #region ExportData - result code tests

    [Fact]
    public async Task ExportData_WhenPinDataIsNull_ReturnsPinDataNotFound()
    {
        _mockPinService.GetPinDataAsync().Returns((IPinService.PinData?)null);

        var result = await _service.ExportData(FileName);

        Assert.Equal(ExportResult.PinDataNotFound, result);
        await _mockFileSaver.DidNotReceive().SaveFileAsync(Arg.Any<string>(), Arg.Any<string>());
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public async Task ExportData_WhenFamilyDataIsNullOrEmpty_ReturnsFamilyDataNotFound(string? familyData)
    {
        _mockPinService.GetPinDataAsync().Returns(BasePinData);
        _mockDataAccess.GetEncryptedData().Returns(familyData);

        var result = await _service.ExportData(FileName);

        Assert.Equal(ExportResult.FamilyDataNotFound, result);
        await _mockFileSaver.DidNotReceive().SaveFileAsync(Arg.Any<string>(), Arg.Any<string>());
    }

    [Fact]
    public async Task ExportData_WhenSaveSucceeds_ReturnsSuccess()
    {
        _mockPinService.GetPinDataAsync().Returns(BasePinData);
        _mockDataAccess.GetEncryptedData().Returns(FamilyData);
        _mockFileSaver.SaveFileAsync(FileName, Arg.Any<string>()).Returns(true);

        var result = await _service.ExportData(FileName);

        Assert.Equal(ExportResult.Success, result);
    }

    [Fact]
    public async Task ExportData_WhenSaveFails_ReturnsUnexpectedError()
    {
        _mockPinService.GetPinDataAsync().Returns(BasePinData);
        _mockDataAccess.GetEncryptedData().Returns(FamilyData);
        _mockFileSaver.SaveFileAsync(FileName, Arg.Any<string>()).Returns(false);

        var result = await _service.ExportData(FileName);

        Assert.Equal(ExportResult.UnexpectedError, result);
    }

    [Fact]
    public async Task ExportData_WhenExceptionThrown_ReturnsUnexpectedError()
    {
        _mockPinService.GetPinDataAsync().Returns<IPinService.PinData?>(_ => throw new InvalidOperationException("boom"));

        var result = await _service.ExportData(FileName);

        Assert.Equal(ExportResult.UnexpectedError, result);
    }

    #endregion

    #region ExportData - XML structure / content tests

    [Fact]
    public async Task ExportData_ProducesWellFormedXml()
    {
        _mockPinService.GetPinDataAsync().Returns(BasePinData);
        _mockDataAccess.GetEncryptedData().Returns(FamilyData);

        string? capturedXml = null;
        _mockFileSaver.SaveFileAsync(FileName, Arg.Do<string>(x => capturedXml = x)).Returns(true);

        await _service.ExportData(FileName);

        Assert.NotNull(capturedXml);
        var doc = new XmlDocument();
        // Should not throw
        doc.LoadXml(capturedXml);
        Assert.Equal("data", doc.DocumentElement?.Name);
    }

    [Fact]
    public async Task ExportData_XmlContainsVersion1()
    {
        _mockPinService.GetPinDataAsync().Returns(BasePinData);
        _mockDataAccess.GetEncryptedData().Returns(FamilyData);

        string? capturedXml = null;
        _mockFileSaver.SaveFileAsync(FileName, Arg.Do<string>(x => capturedXml = x)).Returns(true);

        await _service.ExportData(FileName);

        var doc = new XmlDocument();
        doc.LoadXml(capturedXml!);
        Assert.Equal("1", doc.DocumentElement?.SelectSingleNode("version")?.InnerText);
    }

    [Fact]
    public async Task ExportData_XmlContainsRequiredNodes()
    {
        _mockPinService.GetPinDataAsync().Returns(BasePinData);
        _mockDataAccess.GetEncryptedData().Returns(FamilyData);

        string? capturedXml = null;
        _mockFileSaver.SaveFileAsync(FileName, Arg.Do<string>(x => capturedXml = x)).Returns(true);

        await _service.ExportData(FileName);

        var doc = new XmlDocument();
        doc.LoadXml(capturedXml!);
        var root = doc.DocumentElement!;
        Assert.Equal(BasePinData.Token, root.SelectSingleNode("a")?.InnerText);
        Assert.Equal(BasePinData.Salt, root.SelectSingleNode("b")?.InnerText);
        Assert.Equal(FamilyData, root.SelectSingleNode("c")?.InnerText);
    }

    [Fact]
    public async Task ExportData_XmlOmitsDAndENodes()
    {
        _mockPinService.GetPinDataAsync().Returns(BasePinData);
        _mockDataAccess.GetEncryptedData().Returns(FamilyData);

        string? capturedXml = null;
        _mockFileSaver.SaveFileAsync(FileName, Arg.Do<string>(x => capturedXml = x)).Returns(true);

        await _service.ExportData(FileName);

        var doc = new XmlDocument();
        doc.LoadXml(capturedXml!);
        var root = doc.DocumentElement!;
        Assert.Null(root.SelectSingleNode("d"));
        Assert.Null(root.SelectSingleNode("e"));
    }

    [Fact]
    public async Task ExportData_PassesFileNameToSaveFileAsync()
    {
        _mockPinService.GetPinDataAsync().Returns(BasePinData);
        _mockDataAccess.GetEncryptedData().Returns(FamilyData);
        _mockFileSaver.SaveFileAsync(Arg.Any<string>(), Arg.Any<string>()).Returns(true);

        await _service.ExportData(FileName);

        await _mockFileSaver.Received(1).SaveFileAsync(FileName, Arg.Any<string>());
    }

    #endregion
}
