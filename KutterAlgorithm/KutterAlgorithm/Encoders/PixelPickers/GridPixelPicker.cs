using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Steganography.Exceptions;

namespace Steganography.Encoders.PixelPickers
{
    /// <summary>
    /// Выбирает пиксели для записи битов как узлы сетки с интервалом delta между пикселями
    /// </summary>
    public class GridPixelPicker : IPixelPicker
    {
        private readonly int _delta;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="delta">Расстояние между пикселями</param>
        public GridPixelPicker(int delta)
        {
            _delta = delta;
        }

        public Point GetNextPixel(Bitmap image, int x, int y)
        {
            if (x == 0 && y == 0)
            {
                return new Point(_delta, _delta); // первая по порядку точка
            }
            x += (_delta + 1);
            if (x >= image.Width - _delta) // Переход на новую строку
            {
                y += (_delta + 1);
                x = _delta;
                if (y >= image.Height - _delta)
                    throw new SteganographyException("The image is too small.");
            }
            return new Point(x, y);
        }
    }
}
