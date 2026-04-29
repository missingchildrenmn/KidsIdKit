using KidsIdKit.Core.Data;
using KidsIdKit.Core.Services;
using NSubstitute;
using System.Threading.Tasks;
using System.Xml;
using static KidsIdKit.Core.Services.IImportService;

namespace KidsIdKit.Tests.KidsIdKit.Core.Services;

/// <summary>
/// Concrete subclass used only for testing the abstract ImportServiceBase.
/// </summary>
public class TestImportService(IPinService pinService, IDataAccess dataAccessService)
    : ImportServiceBase(pinService, dataAccessService)
{
    public override Task<string?> SelectFile() => Task.FromResult<string?>(null);
}

public class ImportServiceBaseTests
{
    private readonly IPinService _mockPinService;
    private readonly IDataAccess _mockDataAccess;
    private readonly TestImportService _service;

    // A valid minimal XML payload (version=1, required a/b/c nodes).
    private const string ValidXml =
        "<data><version>1</version><a>token</a><b>salt</b><c>familydata</c></data>";

    public ImportServiceBaseTests()
    {
        _mockPinService = Substitute.For<IPinService>();
        _mockDataAccess = Substitute.For<IDataAccess>();
        _service = new TestImportService(_mockPinService, _mockDataAccess);
    }

    #region LoadXmlFromContent Tests

    [Fact]
    public void LoadXmlFromContent_WithValidXml_ReturnsXmlDocument()
    {
        var result = _service.LoadXmlFromContent(ValidXml);

        Assert.NotNull(result);
        Assert.Equal("data", result.DocumentElement?.Name);
    }

    [Theory]
    [InlineData("")]
    [InlineData(null!)]
    public void LoadXmlFromContent_WithNullOrEmptyContent_ReturnsNull(string? content)
    {
        var result = _service.LoadXmlFromContent(content!);

        Assert.Null(result);
    }

    [Fact]
    public void LoadXmlFromContent_WithMalformedXml_ReturnsNull()
    {
        var result = _service.LoadXmlFromContent("<data><unclosed>");

        Assert.Null(result);
    }

    #endregion

    #region ImportXml – structural / version validation

    [Fact]
    public async Task ImportXml_WithNullDocument_ReturnsInvalidXmlStructure()
    {
        var result = await _service.ImportXml(null!);

        Assert.Equal(XmlImportResult.InvalidXmlStructure, result);
    }

    [Fact]
    public async Task ImportXml_WithMissingVersionNode_ReturnsInvalidXmlStructure()
    {
        var xml = "<data><a>token</a><b>salt</b><c>family</c></data>";
        var doc = _service.LoadXmlFromContent(xml)!;

        var result = await _service.ImportXml(doc);

        Assert.Equal(XmlImportResult.InvalidXmlStructure, result);
    }

    [Fact]
    public async Task ImportXml_WithNonNumericVersion_ReturnsInvalidXmlStructure()
    {
        var xml = "<data><version>abc</version><a>token</a><b>salt</b><c>family</c></data>";
        var doc = _service.LoadXmlFromContent(xml)!;

        var result = await _service.ImportXml(doc);

        Assert.Equal(XmlImportResult.InvalidXmlStructure, result);
    }

    [Fact]
    public async Task ImportXml_WithUnsupportedVersion_ReturnsInvalidVersion()
    {
        var xml = "<data><version>2</version><a>token</a><b>salt</b><c>family</c></data>";
        var doc = _service.LoadXmlFromContent(xml)!;

        var result = await _service.ImportXml(doc);

        Assert.Equal(XmlImportResult.InvalidVersion, result);
    }

    [Fact]
    public async Task ImportXml_WithMissingANode_ReturnsInvalidXmlStructure()
    {
        var xml = "<data><version>1</version><b>salt</b><c>family</c></data>";
        var doc = _service.LoadXmlFromContent(xml)!;

        var result = await _service.ImportXml(doc);

        Assert.Equal(XmlImportResult.InvalidXmlStructure, result);
    }

    [Fact]
    public async Task ImportXml_WithMissingBNode_ReturnsInvalidXmlStructure()
    {
        var xml = "<data><version>1</version><a>token</a><c>family</c></data>";
        var doc = _service.LoadXmlFromContent(xml)!;

        var result = await _service.ImportXml(doc);

        Assert.Equal(XmlImportResult.InvalidXmlStructure, result);
    }

    [Fact]
    public async Task ImportXml_WithMissingCNode_ReturnsInvalidXmlStructure()
    {
        var xml = "<data><version>1</version><a>token</a><b>salt</b></data>";
        var doc = _service.LoadXmlFromContent(xml)!;

        var result = await _service.ImportXml(doc);

        Assert.Equal(XmlImportResult.InvalidXmlStructure, result);
    }

    #endregion

    #region ImportXml – success cases

    [Fact]
    public async Task ImportXml_WithValidXml_ReturnsSuccess()
    {
        var doc = _service.LoadXmlFromContent(ValidXml)!;

        var result = await _service.ImportXml(doc);

        Assert.Equal(XmlImportResult.Success, result);
    }

    [Fact]
    public async Task ImportXml_WithValidXml_CallsSetPinDataAsyncWithCorrectValues()
    {
        var doc = _service.LoadXmlFromContent(ValidXml)!;

        await _service.ImportXml(doc);

        await _mockPinService.Received(1).SetPinDataAsync(Arg.Is<IPinService.PinData>(p =>
            p.Token == "token" &&
            p.Salt == "salt"));
    }

    [Fact]
    public async Task ImportXml_WithValidXml_CallsSetEncryptedDataWithCorrectValue()
    {
        var doc = _service.LoadXmlFromContent(ValidXml)!;

        await _service.ImportXml(doc);

        await _mockDataAccess.Received(1).SetEncryptedData("familydata");
    }

    #endregion
}
