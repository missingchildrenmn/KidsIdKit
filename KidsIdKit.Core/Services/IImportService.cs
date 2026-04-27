using System.Xml;

namespace KidsIdKit.Core.Services;

public interface IImportService
{
    Task<string?> SelectFile();

    XmlDocument? LoadXmlFromContent(string xmlContent);

    Task<XmlImportResult> ImportXml(XmlDocument xmlDocument);


    public enum XmlImportResult
    {
        Success = 0,
        InvalidVersion = 1,
        InvalidXmlStructure = 2
    }

}
