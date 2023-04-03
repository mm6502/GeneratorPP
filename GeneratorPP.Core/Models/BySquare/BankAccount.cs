using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;

namespace GeneratorPP.Core.Models.BySquare
{
    [XmlRoot(ElementName = "BankAccount", Namespace = Namespaces.BySquare)]
    public class BankAccount
    {
        [XmlElement(Order = 13, ElementName = "IBAN", IsNullable = false)]
        [StringLength(maximumLength: 34)]
        // ReSharper disable once InconsistentNaming
        public string? IBAN { get; set; }

        [XmlElement(Order = 14, ElementName = "BIC", IsNullable = false)]
        [StringLength(maximumLength: 11)]
        // ReSharper disable once InconsistentNaming
        public string? BIC { get; set; }
    }
}
