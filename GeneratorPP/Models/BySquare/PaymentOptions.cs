using System;
using System.Xml.Serialization;

namespace Digital.Slovensko.Ekosystem.GeneratorPP.Models.BySquare
{
    [Flags]
    public enum PaymentOptions
    {
        [XmlEnum("paymentorder")]
        PaymentOrder = 1,

        [XmlEnum("standingorder")]
        StandingOrder = 2,

        [XmlEnum("directdebit")]
        DirectDebit = 4,
    }
}