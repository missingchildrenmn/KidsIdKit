using KidsIdKit.Core.Data;
using System.Xml;

namespace KidsIdKit.Core.Services;

public class ExportService : IExportService
{
    protected IPinService _pinService;
    protected IDataAccess _dataAccessService;
    protected IFileSaverService _fileSaverService;

    public ExportService(
        IPinService pinService,
        IDataAccess dataAccessService,
        IFileSaverService fileSaverService)
    {
        _pinService = pinService;
        _dataAccessService = dataAccessService;
        _fileSaverService = fileSaverService;
    }

    public async Task<IExportService.ExportResult> ExportData(string fileName)
    {
        IExportService.ExportResult result = IExportService.ExportResult.UnexpectedError;
        try
        {
            var pinData = await _pinService.GetPinDataAsync();
            if (pinData == null)
            {
                result = IExportService.ExportResult.PinDataNotFound;
                return result;
            }

            var familyInfo = await _dataAccessService.GetEncryptedData();
            if (string.IsNullOrEmpty(familyInfo))
            {
                result = IExportService.ExportResult.FamilyDataNotFound;
                return result;
            }

            var xml = GetOutputXML(pinData, familyInfo);

            var saveResult = await _fileSaverService.SaveFileAsync(fileName, xml);

            result = saveResult ? IExportService.ExportResult.Success : IExportService.ExportResult.UnexpectedError;
        }
        catch (Exception ex)
        {
            result = IExportService.ExportResult.UnexpectedError;
            Console.WriteLine($"ExportData: Exception occurred - {ex.Message}");
        }
        return result;
    }

    protected string GetOutputXML(IPinService.PinData pinData, string familyInfo)
    {
        XmlDocument xmlDoc = new XmlDocument();

        XmlNode rootNode = xmlDoc.CreateElement("data");
        xmlDoc.AppendChild(rootNode);

        XmlNode version = xmlDoc.CreateElement("version");
        version.InnerText = "1";
        rootNode.AppendChild(version);

        XmlNode a = xmlDoc.CreateElement("a");
        a.InnerText = pinData.Token!;
        rootNode.AppendChild(a);

        XmlNode b = xmlDoc.CreateElement("b");
        b.InnerText = pinData.Salt!;
        rootNode.AppendChild(b);

        XmlNode c = xmlDoc.CreateElement("c");
        c.InnerText = familyInfo;
        rootNode.AppendChild(c);

        if (!string.IsNullOrEmpty(pinData.LegacyKey))
        {
            XmlNode d = xmlDoc.CreateElement("d");
            d.InnerText = pinData.LegacyKey;
            rootNode.AppendChild(d);
        }

        if (!string.IsNullOrEmpty(pinData.BiometricKey))
        {
            XmlNode e = xmlDoc.CreateElement("e");
            e.InnerText = pinData.BiometricKey;
            rootNode.AppendChild(e);
        }

        return xmlDoc.OuterXml;
    }
}
