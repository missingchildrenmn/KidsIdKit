using KidsIdKit.Core.Data;
using System.Xml;
using static KidsIdKit.Core.Services.IImportService;

namespace KidsIdKit.Core.Services;

public abstract class ImportServiceBase : IImportService
{
    protected IPinService _pinService;
    protected IDataAccess _dataAccessService;

    public ImportServiceBase(
        IPinService pinService,
        IDataAccess dataAccessService)
    {
        _pinService = pinService;
        _dataAccessService = dataAccessService;
    }

    public abstract Task<string?> SelectFile();

    public async Task<XmlDocument?> LoadXmlFromContentAsync(string xmlContent)
    {
        try
        {
            if (string.IsNullOrEmpty(xmlContent))
            {
                Console.WriteLine("XML content cannot be null or empty.");
                return null;
            }

            // Load the XML content into an XmlDocument
            var xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(xmlContent);

            return xmlDocument;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"ImportServiceBase: Error loading XML from content: {ex.Message}");
            return null;
        }
    }

    private XmlImportResult ValidateXmlVersion(XmlElement bodyNode)
    {
        try
        {
            XmlNode? versionNode = bodyNode.SelectSingleNode("version");
            if (versionNode == null)
            {
                return XmlImportResult.InvalidXmlStructure;
            }

            // Check if the version node has a numeric value
            if (!int.TryParse(versionNode.InnerText, out int version))
            {
                return XmlImportResult.InvalidXmlStructure;
            }

            // Check if the version is 1
            if (version != 1)
            {
                return XmlImportResult.InvalidVersion;
            }

            return XmlImportResult.Success;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"ImportServiceBase: Error validating version: {ex.Message}");
            return XmlImportResult.InvalidXmlStructure;
        }
    }

    public async Task<XmlImportResult> ImportXml(XmlDocument xmlDocument)
    {
        if (xmlDocument == null)
        {
            return XmlImportResult.InvalidXmlStructure;
        }

        // Look for the version node under the body node
        XmlElement? bodyNode = xmlDocument.DocumentElement;
        if (bodyNode == null)
        {
            return XmlImportResult.InvalidXmlStructure;
        }

        var isValid = ValidateXmlVersion(bodyNode);
        if (isValid != XmlImportResult.Success)
        {
            return isValid;
        }

        IPinService.PinData pinData = new IPinService.PinData();
        string? familyInfo = string.Empty;

        XmlNode? aNode = bodyNode.SelectSingleNode("a");
        if (aNode == null)
        {
            return XmlImportResult.InvalidXmlStructure;
        }
        pinData.Token = aNode.InnerText;

        XmlNode? bNode = bodyNode.SelectSingleNode("b");
        if (bNode == null)
        {
            return XmlImportResult.InvalidXmlStructure;
        }
        pinData.Salt = bNode.InnerText;

        XmlNode? cNode = bodyNode.SelectSingleNode("c");
        if (cNode == null)
        {
            return XmlImportResult.InvalidXmlStructure;
        }
        familyInfo = cNode.InnerText;

        XmlNode? dNode = bodyNode.SelectSingleNode("d");
        if (dNode != null)
        {
            pinData.LegacyKey = dNode.InnerText;
        }

        XmlNode? eNode = bodyNode.SelectSingleNode("e");
        if (eNode != null)
        {
            pinData.BiometricKey = eNode.InnerText;
        }

        await _pinService.SetPinDataAsync(pinData);
        await _dataAccessService.SetEncryptedData(familyInfo);

        return XmlImportResult.Success;
    }
}
