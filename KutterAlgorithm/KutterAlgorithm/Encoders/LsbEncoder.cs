using System.Drawing;
using Steganography.Encoders.PixelPickers;

namespace Steganography.Encoders
{
    public class LsbEncoder : EncoderBase
    {
        public LsbEncoder()
        {
            var delta = 2;
            PixelPicker = new GridPixelPicker(delta);
        }

        /// <summary>
        /// Записывает 1 бит сообщения в указанную точку с координатами (x,y)
        /// </summary>
        /// <param name="image"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="bit">'1' или '0'</param>
        protected override void WriteBit(Bitmap image, int x, int y, char bit)
        {
            var pixel = image.GetPixel(x, y);
            var r = pixel.R;
            var g = pixel.G;
            var b = pixel.B;

            if (bit == '1')
            {
                b = (byte)(b | 1); // LSB 1
            }
            else
            {
                b = (byte)(b & 254); // LSB 0
            }

            image.SetPixel(x, y, Color.FromArgb(pixel.A, r, g, b));
        }

        /// <summary>
        /// Читает 1 бит закодированного сообщения для указанной точки с координатами (x,y)
        /// </summary>
        /// <param name="image"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns>'1' или '0'</returns>
        protected override char ReadBit(Bitmap image, int x, int y)
        {
            var pixel = image.GetPixel(x, y);
            var b = pixel.B;

            if ((b & 1) == 1) // LSB 1
            {
                return '1';
            }
            else // LSB 0
            {
                return '0';
            }
        }
    }
}
