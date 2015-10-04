using System.Drawing;
using Steganography.Encoders.PixelPickers;

namespace Steganography.Encoders
{
    public class KutterEncoder : EncoderBase
    {
        private readonly int _delta;
        private readonly double _lambda;

        public KutterEncoder(int delta, double lambda)
        {
            _delta = delta;
            _lambda = lambda;
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
            var l = 0.3 * r + 0.59 * g + 0.11 * b;

            var diff = (byte)(_lambda * l);
            if (bit == '1')
            {
                if ((int)b + (int)diff > 255) // защита от переполнения
                {
                    b = 255;
                }
                else
                {
                    b += diff;
                }
            }
            else
            {
                if (diff > b) // защита от переполнения
                    b = 0;
                else
                    b -= diff;
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

            // Вычисление среднего значения синего в окрестности
            double bAvg = 0;
            const int directions = 4;
            for (int i = x - _delta; i <= x + _delta; i++)
            {
                if (i != x)
                {
                    bAvg += image.GetPixel(i, y).B;
                }
            }
            for (int i = y - _delta; i <= y + _delta; i++)
            {
                if (i != y)
                {
                    bAvg += image.GetPixel(x, i).B;
                }
            }
            bAvg /= directions * _delta;

            if (bAvg <= b)
            {
                return '1';
            }
            else
            {
                return '0';
            }
        }
    }
}
