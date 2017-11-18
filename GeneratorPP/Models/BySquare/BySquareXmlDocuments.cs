using System.Collections.Generic;
using System.Xml.Serialization;

namespace Digital.Slovensko.Ekosystem.GeneratorPP.Models.BySquare
{
    [XmlRoot(ElementName = "BySquareXmlDocuments", Namespace = Namespaces.Empty)]
    public class BySquareXmlDocuments : BySquareCredentials
    {
        public BySquareXmlDocuments()
        { }

        public BySquareXmlDocuments(params BySquareDocument[] documents)
        {
            this.Documents.AddRange(documents);
        }

        [XmlArray(Order = 10, ElementName = "Documents")]
        [XmlArrayItem(ElementName = "Pay", Type = typeof(Pay), Namespace = Namespaces.BySquare)]
        [XmlArrayItem(ElementName = "Invoice", Type = typeof(Invoice), Namespace = Namespaces.BySquare)]
        public List<BySquareDocument> Documents { get; set; } = new List<BySquareDocument>();
    }
}