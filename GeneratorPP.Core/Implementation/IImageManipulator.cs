using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace GeneratorPP.Core.Implementation
{
    /// <summary>
    /// Interface for image manipulators.
    /// </summary>
    public interface IImageManipulator
    {
        /// <summary>
        /// Creates the QR code with logo.
        /// </summary>
        /// <param name="qrString">The qrString.</param>
        Image<Rgba32> CreateQrCodeWithLogo(string qrString);

        /// <summary>
        /// Gets the image data in specified format.
        /// </summary>
        /// <param name="bitmap">The bitmap.</param>
        byte[] GetImageData(Image<Rgba32> bitmap);
    }
}