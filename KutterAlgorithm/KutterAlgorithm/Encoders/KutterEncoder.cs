using System;
using System.Drawing;
using System.Linq;
using KutterAlgorithm.Encoders.PixelPickers;

namespace KutterAlgorithm.Encoders
{
    public class KutterEncoder
    {
        private int _size = 0;
        private IPixelPicker _pixelPicker;
        public Bitmap Encode(string text, Bitmap img, int delta, double lambda)
        {
            _pixelPicker = new GridPixelPicker(img, delta);
            var image = (Bitmap)img.Clone();
            var binText = text.ToBitString();
            int size = binText.Length;
            _size = size;
            binText = size.ToBitString() + binText; // Добавление размера исходного сообщения к самому сообщению
            int x = delta, y = delta;
            foreach (var bit in binText)
            {
                WriteBit(image, x, y, lambda, bit);

                var nextPixel = _pixelPicker.GetNextPixel(x, y);
                x = nextPixel.X;
                y = nextPixel.Y;
            }
            return image;
        }

        /// <summary>
        /// Записывает 1 бит сообщения в указанную точку с координатами (x,y)
        /// </summary>
        /// <param name="image"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="lambda"></param>
        /// <param name="bit"></param>
        private void WriteBit(Bitmap image, int x, int y, double lambda, char bit)
        {
            var pixel = image.GetPixel(x, y);
            var r = pixel.R;
            var g = pixel.G;
            var b = pixel.B;
            var l = 0.3 * r + 0.59 * g + 0.11 * b;

            var diff = (byte)(lambda * l);
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

        public string Decode(Bitmap image, int delta, double lambda)
        {
            return ReadBits(image, delta, lambda).ToStringFromBinary();
        }

        public string ReadBits(Bitmap image, int delta, double lambda)
        {
            _pixelPicker = new GridPixelPicker(image, delta);
            var result = "";
            int x = delta, y = delta;
            int bitCount = 0;
            int length = -1;
            while (result.Length < length || length < 0)
            {
                result += ReadBit(image, x, y, delta);
                ++bitCount;

                // Выделение длины сообщения (записывается как первые 32 бита кодированного сообщения)
                if (bitCount == sizeof(int) * SystemExtention.BitsInByte && length < 0)
                {
                    length = result.ToIntFromBinary();
                    //length = _size;
                    result = "";
                }

                var nextPixel = _pixelPicker.GetNextPixel(x, y);
                x = nextPixel.X;
                y = nextPixel.Y;
            };
            return result;
        }

        /// <summary>
        /// Читает 1 бит закодированного сообщения по окрестности delta указанной точки с координатами (x,y)
        /// </summary>
        /// <param name="image"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="delta"></param>
        /// <returns></returns>
        private char ReadBit(Bitmap image, int x, int y, int delta)
        {
            var pixel = image.GetPixel(x, y);
            var b = pixel.B;

            // Вычисление среднего значения синего в окрестности
            double bAvg = 0;
            const int directions = 4;
            for (int i = x - delta; i <= x + delta; i++)
            {
                if (i != x)
                {
                    bAvg += image.GetPixel(i, y).B;
                }
            }
            for (int i = y - delta; i <= y + delta; i++)
            {
                if (i != y)
                {
                    bAvg += image.GetPixel(x, i).B;
                }
            }
            bAvg /= directions * delta;

            if (bAvg <= b)
            {
                return '1';
            }
            else
            {
                return '0';
            }
        }

        public double CalculateMse(string text, Bitmap img, int delta, double lambda)
        {
            var newImg = Encode(text, img, delta, lambda);
            double err = 0;
            for (int x = 0; x < img.Width; x++)
            {
                for (int y = 0; y < img.Height; y++)
                {
                    err += Math.Pow(img.GetPixel(x, y).B - newImg.GetPixel(x, y).B, 2);
                }
            }
            return err / (img.Height * img.Width);
        }

        public double CalculatePerr(string text, Bitmap img, int delta, double lambda)
        {
            var bits = text.ToBitString();
            var newImg = Encode(text, img, delta, lambda);
            var newBits = ReadBits(newImg, delta, lambda);
            double errors = Enumerable.Range(0, Math.Min(bits.Length, newBits.Length))
                       .Count(i => bits[i] != newBits[i]);
            errors += Math.Abs(bits.Length - newBits.Length);
            return errors / bits.Length;
        }
    }
}
