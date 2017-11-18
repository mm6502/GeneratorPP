using System;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Hosting;
using QRCoder;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Helpers;
using SixLabors.Primitives;

namespace Digital.Slovensko.Ekosystem.GeneratorPP.Implementation
{
    public class ImageManipulator : IImageManipulator
    {
        /// <summary>
        /// The environment.
        /// </summary>
        private readonly IHostingEnvironment _environment;

        /// <summary>
        /// Gets or sets the logo.
        /// </summary>
        private Image<Rgba32> Logo { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ImageManipulator"/> class.
        /// </summary>
        /// <param name="environment">The environment.</param>
        public ImageManipulator(IHostingEnvironment environment)
        {
            this._environment = environment;

            // load pay by square logo 
            this.Logo = this.LoadLogo();
        }

        /// <summary>
        /// Loads the logo.
        /// </summary>
        private Image<Rgba32> LoadLogo()
        {
            var logoPath = Path.Combine(this._environment.WebRootPath, "images", "bysquare.logo.png");
            using (var stream = File.Open(logoPath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                return new PngDecoder().Decode<Rgba32>(Configuration.Default, stream);
            }
        }
        
        /// <summary>
        /// Gets the image data in specified format.
        /// </summary>
        /// <param name="bitmap">The bitmap.</param>
        public byte[] GetImageData(Image<Rgba32> bitmap)
        {
            using (var ms = new MemoryStream())
            {
                bitmap.SaveAsPng(ms, new PngEncoder { PaletteSize = 16, PngColorType = PngColorType.Palette });
                ms.Position = 0;
                return ms.ToArray();
            }
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
            var fromIndex = bitmapStrings[0].IndexOf('1');
            var length = bitmapStrings[0].LastIndexOf('1') + 1 - fromIndex;
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
                    var qrPixel = ((row[i] == '0') ? Rgba32.White : Rgba32.Black);
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
                .DrawImage(qrCode, qrCode.Size(), qrCodeLocation, GraphicsOptions.Default)
            );

            return result;
        }

        /// <summary>
        /// Encodes the image as base64 string.
        /// </summary>
        /// <param name="bitmap">The bitmap.</param>
        public string EncodeImageAsBase64String(Image<Rgba32> bitmap)
        {
            using (var ms = new MemoryStream())
            {
                bitmap.SaveAsPng(ms, new PngEncoder { PaletteSize = 128, PngColorType = PngColorType.Palette });
                ms.Position = 0;
                return "data:image/png;base64," + Convert.ToBase64String(ms.ToArray());
            }
        }
    }
}
