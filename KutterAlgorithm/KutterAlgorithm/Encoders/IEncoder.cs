using System;
using System.Drawing;

namespace Steganography.Encoders
{
    public interface IEncoder
    {
        Bitmap Encode(string text, Bitmap img);

        Bitmap EncodeBits(string bitStr, Bitmap img);

        string Decode(Bitmap image);

        string DecodeBits(Bitmap image);
    }
}
