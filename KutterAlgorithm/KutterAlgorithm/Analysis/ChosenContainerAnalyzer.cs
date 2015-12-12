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
    public class ChosenContainerAnalyzer
    {
        private const int ContainerWidth = 1024;

        private const int ContainerHeight = 768;

        private const string Text = "Lorem ipsum dolor sit amet, consectetur adipisicing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua.";

        public List<AnalysisResult> Analyze(IEncoder encoder)
        {
            var results = new List<AnalysisResult>()
            {
                new AnalysisResult()
                {
                    Name = "Red",
                    Image = encoder.Encode(Text, CreateContainer(Color.FromArgb(255, 255, 0, 0)))
                },
                new AnalysisResult()
                {
                    Name = "Green",
                    Image = encoder.Encode(Text, CreateContainer(Color.FromArgb(255, 0, 255, 0)))
                },
                new AnalysisResult()
                {
                    Name = "Blue",
                    Image = encoder.Encode(Text, CreateContainer(Color.FromArgb(255, 0, 0, 255)))
                },
                new AnalysisResult()
                {
                    Name = "Black",
                    Image = encoder.Encode(Text, CreateContainer(Color.FromArgb(255, 0, 0, 0)))
                },
                new AnalysisResult()
                {
                    Name = "White",
                    Image = encoder.Encode(Text, CreateContainer(Color.FromArgb(255, 255, 255, 255)))
                },
                new AnalysisResult()
                {
                    Name = "Gray",
                    Image = encoder.Encode(Text, CreateContainer(Color.Gray))
                },
                new AnalysisResult()
                {
                    Name = "CornflowerBlue",
                    Image = encoder.Encode(Text, CreateContainer(Color.CornflowerBlue))
                }
            };
            var normalizer = new Normalyzer();
            var normalized = results.Select(r => new AnalysisResult() { Name = "NORM_" + r.Name, Image = normalizer.Normalize(r.Image) });
            return results.Concat(normalized).ToList();
        }

        private Bitmap CreateContainer(Color color)
        {
            var container = new Bitmap(ContainerWidth, ContainerHeight);
            using (var g = Graphics.FromImage(container))
            {
                g.FillRectangle(new SolidBrush(color), new Rectangle(0, 0, ContainerWidth, ContainerHeight));
            }
            return container;
        }
    }
}
