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
        public float ScaleX { get; set; }
        public float ScaleY { get; set; }
        public int RotationAngle { get; set; }
        public float CropPercentX { get; set; }
        public float CropPercentY { get; set; }

        public ProcessingParameters()
        {
            ScaleX = 1;
            ScaleY = 1;
            CropPercentX = 1;
            CropPercentY = 1;
            RotationAngle = 0;
        }
    }
}
