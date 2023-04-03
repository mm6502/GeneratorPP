using System.Xml.Serialization;

namespace GeneratorPP.Core.Models.BySquare
{
    [XmlRoot(ElementName = "BySquareCredentials", Namespace = Namespaces.Empty)]
    public class BySquareCredentials
    {
        [XmlElement(Order = 1, ElementName = "Username")]
        public string? Username { get; set; }

        [XmlElement(Order = 2, ElementName = "Password")]
        public string? Password { get; set; }

        [XmlElement(Order = 3, ElementName = "ServiceId")]
        public string? ServiceId { get; set; }

        [XmlElement(Order = 4, ElementName = "ServiceUserId")]
        public string? ServiceUserId { get; set; }
    }
}