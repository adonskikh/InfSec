using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Steganography.Encoders;
using Steganography.ImageProcessing;

namespace Steganography.Analysis
{
    public class KnownContainerAnalyzer
    {
        private const int ContainerWidth = 1024;

        private const int ContainerHeight = 768;

        private const string Text = "Lorem ipsum dolor sit amet, consectetur adipisicing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua.";

        public List<AnalysisResult> Analyze(Bitmap emptyContainer, Bitmap fullContainer)
        {
            var results = new List<AnalysisResult>()
            {
                new AnalysisResult()
                {
                    Name = "Empty",
                    Image = emptyContainer
                },
                new AnalysisResult()
                {
                    Name = "Full",
                    Image = fullContainer
                },
            };
            var diff = new AnalysisResult()
            {
                Name = "Diff",
                Image = CalculateDiff(emptyContainer, fullContainer)
            };
            var normalizer = new Normalyzer();
            var normDiff = new AnalysisResult()
            {
                Name = "DiffNormalized",
                Image = normalizer.Normalize(diff.Image)
            };
            results.Add(diff);
            results.Add(normDiff);
            return results;
        }

        private Bitmap CalculateDiff(Bitmap emptyContainer, Bitmap fullContainer)
        {
            var diff = new Bitmap(emptyContainer.Width, emptyContainer.Height);
            for (int x = 0; x < emptyContainer.Width; x++)
            {
                for (int y = 0; y < emptyContainer.Height; y++)
                {
                    var pixelEmpty = emptyContainer.GetPixel(x, y);
                    var pixelFull = fullContainer.GetPixel(x, y);
                    var r = CalculateDiff(pixelFull.R, pixelEmpty.R, 0, 255);
                    var g = CalculateDiff(pixelFull.G, pixelEmpty.G, 0, 255);
                    var b = CalculateDiff(pixelFull.B, pixelEmpty.B, 0, 255);
                    diff.SetPixel(x, y, Color.FromArgb(255, r, g, b));
                }
            }
            return diff;
        }

        private int CalculateDiff(int val1, int val2, int min, int max)
        {
            var diff = val1 - val2;
            if (diff > 0)
            {
                return max;
            }
            if (diff < 0)
            {
                return min;
            }
            return (max-min)/2;
        }
    }
}
