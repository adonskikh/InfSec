using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace KutterAlgorithm
{
    public class KutterEncipherer
    {
        private int _size = 0;
        public Bitmap Encode(string text, Bitmap img, int delta, double lambda)
        {
            var image = (Bitmap)img.Clone();
            var binText = text.ToBitString();
            int size = binText.Length;
            _size = size;
            binText = size.ToBitString() + binText; // Добавление размера исходного сообщения к самому сообщению
            int x = delta, y = delta;
            foreach (var bit in binText)
            {
                var pixel = image.GetPixel(x, y);
                var r = pixel.R;
                var g = pixel.G;
                var b = pixel.B;
                var l = 0.3 * r + 0.59 * g + 0.11 * b;

                var diff = (byte)(lambda * l);
                if (bit == '1')
                {
                    b += diff;
                    if (b > 255)
                        b = 255;
                }
                else
                {
                    if (diff > b)
                        b = 0;
                    else
                        b -= diff;
                }

                image.SetPixel(x, y, Color.FromArgb(pixel.A, r, g, b));

                x += (delta + 1);
                if (x >= image.Width - delta) // Переход на новую строку
                {
                    y += (delta + 1);
                    x = delta;
                    if (y >= image.Height - delta)
                        throw new Exception("The image is too small.");
                }
            };
            return image;
        }

        public string Decode(Bitmap image, int delta, double lambda)
        {
            return AdjustLength(ReadBits(image, delta, lambda)).ToStringFromBinary();
        }

        public string ReadBits(Bitmap image, int delta, double lambda)
        {
            var result = "";
            int x = delta, y = delta;
            int bitCount = 0;
            int length = -1;
            while (result.Length < length || length < 0)
            {
                var pixel = image.GetPixel(x, y);
                var b = pixel.B;

                // Вычисление среднего значения синего в окрестности
                double bAvg = 0;
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
                bAvg /= 4 * delta;

                if (bAvg <= b)
                {
                    result += "1";
                }
                else
                {
                    result += "0";
                }
                ++bitCount;

                // Чтение длины сообщения
                if (bitCount == sizeof(int) * 8 && length < 0)
                {
                    //length = result.ToIntFromBinary();
                    length = _size;
                    result = "";
                }

                x += (delta + 1);
                if (x >= image.Width - delta) // Переход на новую строку
                {
                    y += (delta + 1);
                    x = delta;
                    if (y >= image.Height - delta)
                        return result;
                }
            };
            return result;
        }

        private string AdjustLength(string s)
        {
            var fixedText = s.TrimEnd(' ');
            fixedText = fixedText.PadRight(8 * (int)Math.Ceiling((double)fixedText.Length / 8));
            return fixedText;
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
