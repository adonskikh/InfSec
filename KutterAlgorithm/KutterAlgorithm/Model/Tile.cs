using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Steganography.Model
{
    public class Tile : IDisposable
    {
        public int X { get; set; }
        public int Y { get; set; }
        public Bitmap Bitmap { get; set; }

        public Tile(int x, int y, Bitmap bitmap)
        {
            X = x;
            Y = y;
            Bitmap = bitmap;
        }

        public void Dispose()
        {
            Bitmap.Dispose();
        }
    }
}
