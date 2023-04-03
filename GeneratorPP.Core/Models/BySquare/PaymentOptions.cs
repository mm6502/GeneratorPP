using System;
using System.Xml.Serialization;

namespace GeneratorPP.Core.Models.BySquare
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