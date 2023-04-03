using System;
using System.IO;
using System.Linq;
using QRCoder;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace GeneratorPP.Core.Implementation
{
    public class ImageManipulator : IImageManipulator
    {
        /// <summary>
        /// Gets or sets the logo.
        /// </summary>
        private Image<Rgba32> Logo { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ImageManipulator"/> class.
        /// </summary>
        public ImageManipulator()
        {
            // load pay by square logo 
            this.Logo = this.LoadLogo();
        }

        /// <summary>
        /// Loads the logo.
        /// </summary>
        private Image<Rgba32> LoadLogo()
        {
            var stream = new MemoryStream(Resources.bysquare_logo);
            return PngDecoder.Instance.Decode<Rgba32>(new DecoderOptions(), stream);
        }
        
        /// <summary>
        /// Gets the image data in specified format.
        /// </summary>
        /// <param name="bitmap">The bitmap.</param>
        public byte[] GetImageData(Image<Rgba32> bitmap)
        {
            using var ms = new MemoryStream();
            bitmap.SaveAsPng(ms, new PngEncoder { BitDepth = PngBitDepth.Bit4, ColorType = PngColorType.Palette });
            ms.Position = 0;
            return ms.ToArray();
        }

        /// <summary>
        /// Creates the qr code with logo.
        /// </summary>
        /// <param name="qrString">The QR string.</param>
        public Image<Rgba32> CreateQrCodeWithLogo(string qrString)
        {
            // generate QR code image
            var qrCodeImage = this.CreateQrCode(qrString);

            // create final image
            return this.CombineQrImageWithLogo(qrCodeImage, this.Logo);
        }

        /// <summary>
        /// Creates the QR code.
        /// </summary>
        /// <param name="qrString">The qrString.</param>
        private Image<Rgba32> CreateQrCode(string qrString)
        {
            // how much space is available on canvas for the qr code
            const int pixelSpace = 316;

            // initialize qr generator
            var qrGenerator = new QRCodeGenerator();
            var qrData = qrGenerator.CreateQrCode(qrString, QRCodeGenerator.ECCLevel.L);

            // get the "ASCII bitmap"
            var bitmapStrings = new AsciiQRCode(qrData).GetLineByLineGraphic(1, "1", "0");
            if ((bitmapStrings == null) || (bitmapStrings.Length < 1))
                throw new InvalidDataException("QRCoder produced invalid QR string.");

            // remove the side padding (if any)
            var fromIndex = bitmapStrings
                .Select((o) => o.IndexOf('1'))
                .Where((o) => o >= 0)
                .Min();
            var length = bitmapStrings
                .Select((o) => o.LastIndexOf('1') + 1 - fromIndex)
                .Where((o) => o >= 0)
                .Max();
            bitmapStrings = bitmapStrings
                .Select(line => line.Substring(fromIndex, length))
                .ToArray();

            // how many real pixels is one "qr pixel"
            var pixelSize = pixelSpace / bitmapStrings[0].Length;

            // create image of proper size
            var qrCodeImage = new Image<Rgba32>(pixelSize * bitmapStrings[0].Length, pixelSize * bitmapStrings.Length);

            // drwa the qr code 
            for (var j = 0; j < bitmapStrings.Length; j++)
            {
                var row = bitmapStrings[j];
                for (var i = 0; i < row.Length; i++)
                {
                    var qrPixel = (Rgba32) ((row[i] == '0') ? Color.White : Color.Black);
                    this.DrawQrPixel(qrCodeImage, pixelSize, i, j, qrPixel);
                }
            }
            return qrCodeImage;
        }

        /// <summary>
        /// Draws a QR pixel.
        /// </summary>
        /// <param name="qrCodeImage">The qr code image.</param>
        /// <param name="pixelSize">Size of the pixel.</param>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="color">The color.</param>
        private void DrawQrPixel(Image<Rgba32> qrCodeImage, int pixelSize, int x, int y, Rgba32 color)
        {
            var dy = y * pixelSize;
            for (var j = 0; j < pixelSize; j++, dy++)
            {
                var dx = x * pixelSize;
                for (var i = 0; i < pixelSize; i++, dx++)
                {
                    qrCodeImage[dx,dy] = color;
                }
            }
        }

        /// <summary>
        /// Combines the QR image with logo.
        /// </summary>
        /// <param name="qrCode">The qr code.</param>
        /// <param name="logo">The logo.</param>
        /// <returns></returns>
        private Image<Rgba32> CombineQrImageWithLogo(Image<Rgba32> qrCode, Image<Rgba32> logo)
        {
            // center of the QR code on canvas
            var middle = new Point(192, 192);

            // calculate the QR code location
            var qrCodeLocation = new Point(middle.X - qrCode.Width / 2, middle.Y - qrCode.Height / 2);

            // compose the result
            var result = logo.Clone();

            result.Mutate(i => i
                .DrawImage(qrCode, qrCodeLocation, new GraphicsOptions() { Antialias = true })
            );

            return result;
        }

        /// <summary>
        /// Encodes the image as base64 string.
        /// </summary>
        /// <param name="bitmap">The bitmap.</param>
        public string EncodeImageAsBase64String(Image<Rgba32> bitmap)
        {
            using var ms = new MemoryStream();
            bitmap.SaveAsPng(ms, new PngEncoder { BitDepth = PngBitDepth.Bit4, ColorType = PngColorType.Palette });
            ms.Position = 0;
            return "data:image/png;base64," + Convert.ToBase64String(ms.ToArray());
        }
    }
}
