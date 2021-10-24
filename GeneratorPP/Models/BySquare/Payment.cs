using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Xml.Serialization;
using Digital.Slovensko.Ekosystem.GeneratorPP.Implementation;

namespace Digital.Slovensko.Ekosystem.GeneratorPP.Models.BySquare
{
    [XmlRoot(ElementName = "Payment", Namespace = Namespaces.BySquare)]
    public class Payment
    {
        public Payment()
        { }

        public Payment(params BankAccount[] accounts)
        {
            this.BankAccounts.AddRange(accounts);
        }

        private PaymentOptions _paymentOptions = PaymentOptions.PaymentOrder;

        [XmlElement(Order = 3, ElementName = "PaymentOptions")]
        public string PaymentOptionsString
        {
            get { return string.Join(" ", this._paymentOptions.GetSetFlagsXmlStrings()); }
            set { this._paymentOptions = Enum.Parse<PaymentOptions>(value, true); }
        }

        [XmlIgnore]
        public PaymentOptions PaymentOptionsEnum
        {
            get { return this._paymentOptions; }
            set { this._paymentOptions = value; }
        }

        [XmlElement(Order = 4, ElementName = "Amount", IsNullable = true)]
        [Range(minimum: 0, maximum: 9999)]
        [Required]
        public decimal? Amount { get; set; } = null;

        [XmlElement(Order = 5, ElementName = "CurrencyCode")]
        [StringLength(maximumLength: 3, MinimumLength = 3)]
        [Required]
        public string CurrencyCode { get; set; } = "EUR";

        [XmlIgnore]
        public DateTime? PaymentDueDate { get; set; } = null;

        [XmlElement(Order = 6, ElementName = "PaymentDueDate", IsNullable = true)]
        public string PaymentDueDateString
        {
            get => PaymentDueDate?.ToString("yyyy-MM-dd");
            set => DateTime.Parse(value, CultureInfo.InvariantCulture);
        }

        [XmlElement(Order = 7, ElementName = "VariableSymbol")]
        [StringLength(maximumLength: 10)]
        public string VariableSymbol { get; set; } = null;

        [XmlElement(Order = 8, ElementName = "ConstantSymbol")]
        [StringLength(maximumLength: 4)]
        public string ConstantSymbol { get; set; } = null;

        [XmlElement(Order = 9, ElementName = "SpecificSymbol")]
        [StringLength(maximumLength: 10)]
        public string SpecificSymbol { get; set; } = null;

        [XmlElement(Order = 10, ElementName = "OriginatorsReferenceInformation")]
        [StringLength(maximumLength: 35)]
        public string OriginatorsReferenceInformation { get; set; }

        [XmlElement(Order = 11, ElementName = "PaymentNote")]
        [StringLength(maximumLength: 140)]
        public string PaymentNote { get; set; }

        [XmlArray(Order = 12, ElementName = "BankAccounts")]
        public List<BankAccount> BankAccounts { get; set; } = new List<BankAccount>();

        [XmlElement(Order = 15, ElementName = "StandingOrderExt", IsNullable = true)]
        public string StandingOrderExt { get; } = null;

        [XmlElement(Order = 20, ElementName = "DirectDebitExt", IsNullable = true)]
        public string DirectDebitExt { get; } = null;

        [XmlElement(Order = 31, ElementName = "BeneficiaryName")]
        [StringLength(maximumLength: 70)]
        public string BeneficiaryName { get; set; }

        [XmlElement(Order = 32, ElementName = "BeneficiaryAddressLine1")]
        [StringLength(maximumLength: 70)]
        public string BeneficiaryAddressLine1 { get; set; }

        [XmlElement(Order = 33, ElementName = "BeneficiaryAddressLine2")]
        [StringLength(maximumLength: 70)]
        public string BeneficiaryAddressLine2 { get; set; }
    }
}