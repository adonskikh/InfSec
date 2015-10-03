using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KutterAlgorithm.Encoders.PixelPickers
{
    /// <summary>
    /// Выбирает пиксели для записи битов как узлы сетки с интервалом delta между пикселями
    /// </summary>
    public class GridPixelPicker : IPixelPicker
    {
        private readonly Bitmap _image;
        private readonly int _delta;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="image"></param>
        /// <param name="delta">Расстояние между пикселями</param>
        public GridPixelPicker(Bitmap image, int delta)
        {
            _image = image;
            _delta = delta;
        }

        public Point GetNextPixel(int x, int y)
        {
            x += (_delta + 1);
            if (x >= _image.Width - _delta) // Переход на новую строку
            {
                y += (_delta + 1);
                x = _delta;
                if (y >= _image.Height - _delta)
                    throw new Exception("The image is too small.");
            }
            return new Point(x, y);
        }
    }
}
