using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Steganography.ImageProcessing
{
    [Serializable]
    public class RobustnessTestResult
    {
        public string SourceImagePath { get; set; }
        public string ProcessedImagePath { get; set; }
        public double ErrorRate { get; set; }
        public double ErrorRateTransformed { get; set; }

        public ProcessingParameters Parameters { get; set; }
    }
}
