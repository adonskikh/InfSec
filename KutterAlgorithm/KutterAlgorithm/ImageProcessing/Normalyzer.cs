using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Steganography.ImageProcessing
{
    public class Normalyzer
    {
        public Bitmap Normalize(Bitmap bitmap)
        {
            var normalized = new Bitmap(bitmap.Width, bitmap.Height);
            var maxR = 0;
            var maxG = 0;
            var maxB = 0;
            var minR = 255;
            var minG = 255;
            var minB = 255;
            for (int x = 0; x < bitmap.Width; x++)
            {
                for (int y = 0; y < bitmap.Height; y++)
                {
                    var pixel = bitmap.GetPixel(x, y);
                    maxR = Math.Max(maxR, pixel.R);
                    maxG = Math.Max(maxG, pixel.G);
                    maxB = Math.Max(maxB, pixel.B);
                    minR = Math.Min(minR, pixel.R);
                    minG = Math.Min(minG, pixel.G);
                    minB = Math.Min(minB, pixel.B);
                }
            }
            for (int x = 0; x < bitmap.Width; x++)
            {
                for (int y = 0; y < bitmap.Height; y++)
                {
                    var pixel = bitmap.GetPixel(x, y);
                    var r = Normalize(pixel.R, minR, maxR, 0, 255);
                    var g = Normalize(pixel.G, minG, maxG, 0, 255);
                    var b = Normalize(pixel.B, minB, maxB, 0, 255);
                    normalized.SetPixel(x, y, Color.FromArgb(255, r, g, b));
                }
            }
            return normalized;
        }

        private int Normalize(int val, int minVal, int maxVal, int min, int max)
        {
            if (minVal == maxVal)
            {
                return min;
            }
            return ((max - min) * (val - minVal)) / (maxVal - minVal) + min;
        }
    }
}
