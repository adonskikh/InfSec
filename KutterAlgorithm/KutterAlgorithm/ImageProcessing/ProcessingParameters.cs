using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Steganography.ImageProcessing
{
    [Serializable]
    public class ProcessingParameters
    {
        public double ScaleX { get; set; }
        public double ScaleY { get; set; }
        public int RotationAngle { get; set; }

        public ProcessingParameters()
        {
            ScaleX = 1;
            ScaleY = 1;
            RotationAngle = 0;
        }
    }
}
