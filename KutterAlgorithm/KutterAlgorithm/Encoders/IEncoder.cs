using System;
using System.Drawing;

namespace KutterAlgorithm.Encoders
{
    public interface IEncoder
    {
        Bitmap Encode(string text, Bitmap img);

        string Decode(Bitmap image);

        double CalculateMse(string text, Bitmap img);

        double CalculatePerr(string text, Bitmap img);
    }
}
