using System.Xml.Serialization;

namespace Digital.Slovensko.Ekosystem.GeneratorPP.Models.BySquare
{
    [XmlRoot(ElementName = "StringSetOfCodes", Namespace = Namespaces.Empty)]
    public class StringSetOfCodes
    {
        [XmlElement(Order = 1, ElementName = "PayBySquare")]
        public string PayBySquare { get; set; }
    }
}