using Digital.Slovensko.Ekosystem.GeneratorPP.Models.BySquare;

namespace Digital.Slovensko.Ekosystem.GeneratorPP.Implementation
{
    /// <summary>
    /// Interface for QR string generators.
    /// </summary>
    public interface IBySquareEncoder
    {
        /// <summary>
        /// Serializes given document as a QR string.
        /// </summary>
        /// <param name="document">A BySquareDocument instance.</param>
        /// <returns>QR string.</returns>
        string Encode(BySquareDocument document);
    }
}
