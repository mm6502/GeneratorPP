using System.Xml.Serialization;

namespace GeneratorPP.Core.Models.BySquare
{
    [XmlRoot(Namespace = Namespaces.BySquare)]
    [XmlInclude(typeof(Pay))]
    [XmlInclude(typeof(Invoice))]
    public abstract class BySquareDocument
    {
        // nothing ATM
    }
}