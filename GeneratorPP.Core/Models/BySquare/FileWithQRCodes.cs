using System.Xml.Serialization;

namespace GeneratorPP.Core.Models.BySquare
{
    [XmlRoot(ElementName = "FileWithQRCodes", Namespace = Namespaces.Empty)]
    public class FileWithQRCodes : BySquareCredentials
    {
        // nothing ATM
    }
}