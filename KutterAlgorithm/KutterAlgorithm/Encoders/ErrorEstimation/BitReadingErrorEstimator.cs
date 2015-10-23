using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Steganography.Encoders.ErrorEstimation
{
    public class BitReadingErrorEstimator
    {
        public double Estimate(string hiddenText, Bitmap fullContainer, IEncoder encoder)
        {
            var bits = hiddenText.ToBitString();
            var newBits = encoder.DecodeBits(fullContainer);
            double errors = Enumerable.Range(0, Math.Min(bits.Length, newBits.Length))
                .Count(i => bits[i] != newBits[i]);
            errors += Math.Abs(bits.Length - newBits.Length);
            return errors / bits.Length;
        }
    }
}
