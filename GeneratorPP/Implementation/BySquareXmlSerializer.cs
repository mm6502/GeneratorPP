using System.IO;
using System.Xml;
using System.Xml.Serialization;
using Digital.Slovensko.Ekosystem.GeneratorPP.Models.BySquare;

namespace Digital.Slovensko.Ekosystem.GeneratorPP.Implementation
{
    /// <summary>
    /// BySquare data model Xml serializer.
    /// </summary>
    /// <seealso cref="IBySquareXmlSerializer" />
    public class BySquareXmlSerializer : IBySquareXmlSerializer
    {
        /// <summary>
        /// Serializes the given document as XML.
        /// </summary>
        /// <param name="bySquareXmlDocuments">The by square XML documents.</param>
        public string SerializeAsXml(BySquareXmlDocuments bySquareXmlDocuments)
        {
            using var ms = new MemoryStream();
            using (var xtw = XmlWriter.Create(ms,
                new XmlWriterSettings()
                {
                    ConformanceLevel = ConformanceLevel.Auto,
                    CloseOutput = false,
                    Indent = true,
                    IndentChars = "    ",
                    OmitXmlDeclaration = true,
                }))
            {
                var ds = new XmlSerializer(typeof(BySquareXmlDocuments));
                ds.Serialize(xtw, bySquareXmlDocuments);
                xtw.Flush();
            }

            ms.Position = 0;

            using var sr = new StreamReader(ms);
            {
                return sr.ReadToEnd();
            }
        }

        /// <summary>
        /// Serializes the given document as XML.
        /// </summary>
        /// <param name="bySquareDocument">The by square document.</param>
        public string SerializeAsXml(BySquareDocument bySquareDocument)
        {
            using var ms = new MemoryStream();
            using (var xtw = XmlWriter.Create(ms,
                new XmlWriterSettings()
                {
                    ConformanceLevel = ConformanceLevel.Auto,
                    CloseOutput = false,
                    Indent = true,
                    IndentChars = "    ",
                    OmitXmlDeclaration = true,
                }))
            {
                var ds = new XmlSerializer(bySquareDocument.GetType());
                ds.Serialize(xtw, bySquareDocument);
                xtw.Flush();
            }

            ms.Position = 0;

            using var sr = new StreamReader(ms);
            {
                return sr.ReadToEnd();
            }
        }

        /// <summary>
        /// Deserializes the set of codes.
        /// </summary>
        /// <param name="response">The response from web API QR string generator.</param>
        public StringSetOfCodes DeserializeSetOfCodes(string response)
        {
            using var sr = new StringReader(response);
            using var xtw = XmlReader.Create(sr,
                new XmlReaderSettings()
                {
                    ConformanceLevel = ConformanceLevel.Auto,
                    CloseInput = true,
                }
            );
            var ds = new XmlSerializer(typeof(StringSetOfCodes));
            var result = (StringSetOfCodes)ds.Deserialize(xtw);
            return result;
        }
    }
}