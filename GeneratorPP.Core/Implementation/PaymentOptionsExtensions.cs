using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Xml.Serialization;
using GeneratorPP.Core.Models.BySquare;

namespace GeneratorPP.Core.Implementation
{
    /// <summary>
    /// Extensions for <seealso cref="PaymentOptions"/> to help with serialization.
    /// </summary>
    public static class PaymentOptionsExtensions
    {
        /// <summary>
        /// Gets the set flags as XML strings.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <exception cref="InvalidEnumArgumentException">Value must be an Enum marked with Flags attribute.</exception>
        public static IEnumerable<string> GetSetFlagsXmlStrings(this PaymentOptions value)
        {
            var type = typeof(PaymentOptions);

            if (type.GetCustomAttribute<FlagsAttribute>() == null)
                throw new InvalidEnumArgumentException("Value must be an Enum marked with Flags attribute.");

            var values = Enum.GetValues(type)
                .OfType<PaymentOptions>()
                .Where(o => o == (o & value));

            return values.Select(GetXmlAttrNameFromEnumValue);
        }

        /// <summary>
        /// Gets the XML attribute name from enum value.
        /// </summary>
        /// <param name="pEnumVal">The enum value.</param>
        public static string GetXmlAttrNameFromEnumValue(PaymentOptions pEnumVal)
        {
            var info = typeof(PaymentOptions).GetField(Enum.GetName(typeof(PaymentOptions), pEnumVal));
            return info.GetCustomAttribute<XmlEnumAttribute>(false)?.Name;
        }
    }
}
