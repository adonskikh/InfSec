using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Steganography.Messages
{
    public class SelectedArea
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }

        public SelectedArea(int x, int y, int width, int height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }
    }
    public class AreaSelectedMessage
    {
        public SelectedArea Area { get; set; }

        public AreaSelectedMessage(SelectedArea area)
        {
            Area = area;
        }
    }
}
