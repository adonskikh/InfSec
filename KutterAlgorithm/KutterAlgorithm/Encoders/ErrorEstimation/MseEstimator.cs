using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Steganography.Encoders.ErrorEstimation
{
    public class MseEstimator
    {
        public double Estimate(Bitmap emptyContainer, Bitmap fullContainer)
        {
            double err = 0;
            var width = Math.Min(emptyContainer.Width, fullContainer.Width);
            var height = Math.Min(emptyContainer.Height, fullContainer.Height);
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    err += Math.Pow(emptyContainer.GetPixel(x, y).B - fullContainer.GetPixel(x, y).B, 2);
                }
            }
            return err / (width * height);
        }
    }
}
