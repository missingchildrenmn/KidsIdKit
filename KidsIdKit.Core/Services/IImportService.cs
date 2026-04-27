using System.Xml;

namespace KidsIdKit.Core.Services;

public interface IImportService
{
    Task<string?> SelectFile();
    Task<XmlDocument?> LoadXmlFromContentAsync(string xmlContent);

    Task<XmlImportResult> ImportXml(XmlDocument xmlDocument);


    public enum XmlImportResult
    {
        Success = 0,
        InvalidVersion = 1,
        InvalidXmlStructure = 2
    }

}
