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
        public Bitmap Encode(string text, Bitmap image, int delta, double alpha)
        {
            var binText = text.ToBitString();
            int size = binText.Length;
            _size = size;
            binText = size.ToBitString() + binText;
            int x = delta, y = delta;
            foreach (var bit in binText)
            {
                var pixel = image.GetPixel(x, y);
                var r = pixel.R;
                var g = pixel.G;
                var b = pixel.B;
                var l = 0.3 * r + 0.59 * g + 0.11 * b;

                var diff = (byte)(alpha * l);
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
                if (x >= image.Width - delta)
                {
                    y += (delta + 1);
                    x = delta;
                    if (y >= image.Height - delta)
                        throw new Exception("The image is too small.");
                }
            };
            return image;
        }

        public string Decode(Bitmap image, int delta, double alpha)
        {
            var result = "";
            int x = delta, y = delta;
            int bitCount = 0;
            int length = -1;
            while (result.Length < length || length < 0)
            {
                var pixel = image.GetPixel(x, y);
                var b = pixel.B;
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
                if (bitCount == sizeof (int) * 8 && length < 0)
                {
                    //length = result.ToIntFromBinary();
                    length = _size;
                    result = "";
                }

                x += (delta + 1);
                if (x >= image.Width - delta)
                {
                    y += (delta + 1);
                    x = delta;
                    if (y >= image.Height - delta)
                        return AdjustLength(result).ToStringFromBinary();
                }
            };
            return AdjustLength(result).ToStringFromBinary();
        }



        private string AdjustLength(string s)
        {
            var fixedText = s.TrimEnd(' ');
            fixedText = fixedText.PadRight(8 * (int)Math.Ceiling((double)fixedText.Length / 8));
            return fixedText;
        }
    }
}
