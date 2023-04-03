using System.Xml.Serialization;

namespace GeneratorPP.Core.Models.BySquare
{
    [XmlRoot(ElementName = "Invoice", Namespace = Namespaces.BySquare)]
    public class Invoice : BySquareDocument
    {
        // nothing ATM
    }
}