using Digital.Slovensko.Ekosystem.GeneratorPP.Models.BySquare;

namespace Digital.Slovensko.Ekosystem.GeneratorPP.Implementation
{
    /// <summary>
    /// Interface for BySquare data model Xml serializers.
    /// </summary>
    public interface IBySquareXmlSerializer
    {
        /// <summary>
        /// Deserializes the set of codes.
        /// </summary>
        /// <param name="response">The response.</param>
        /// <returns></returns>
        StringSetOfCodes DeserializeSetOfCodes(string response);

        /// <summary>
        /// Serializes as XML.
        /// </summary>
        /// <param name="bySquareDocument">The by square document.</param>
        /// <returns></returns>
        string SerializeAsXml(BySquareDocument bySquareDocument);

        /// <summary>
        /// Serializes as XML.
        /// </summary>
        /// <param name="bySquareXmlDocuments">The by square XML documents.</param>
        /// <returns></returns>
        string SerializeAsXml(BySquareXmlDocuments bySquareXmlDocuments);
    }
}