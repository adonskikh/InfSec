using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Steganography.ImageProcessing
{
    [Serializable]
    public class ProcessingResult
    {
        public string SourceImagePath { get; set; }
        public string ProcessedImagePath { get; set; }
        public ProcessingParameters Parameters { get; set; }
    }
}
