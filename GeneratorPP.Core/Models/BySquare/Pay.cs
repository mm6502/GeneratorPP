using System.Collections.Generic;
using System.Xml.Serialization;

namespace GeneratorPP.Core.Models.BySquare
{
    [XmlRoot(ElementName = "Pay", Namespace = Namespaces.BySquare)]
    public class Pay : BySquareDocument
    {
        public Pay()
        { }

        public Pay(params Payment[] payments)
        {
            this.Payments.AddRange(payments);
        }

        [XmlAttribute(AttributeName = "type", Namespace = Namespaces.SchemaInstance)]
        public string Type { get; set; } = "Pay";

        [XmlArray(Order = 2, ElementName = "Payments")]
        public List<Payment> Payments { get; set; } = new List<Payment>();
    }
}