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
    public class GridPixelPickerExcludeColor : IPixelPicker
    {
        private readonly Color _excludedColor;
        private readonly GridPixelPicker _gridPixelPicker;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="delta">Расстояние между пикселями</param>
        /// <param name="excludedColor"></param>
        public GridPixelPickerExcludeColor(int delta, Color excludedColor)
        {
            _excludedColor = excludedColor;
            _gridPixelPicker = new GridPixelPicker(delta);
        }

        public Point GetNextPixel(Bitmap image, int x, int y)
        {
            var point = _gridPixelPicker.GetNextPixel(image, x, y);
            while (image.GetPixel(point.X, point.Y).ToArgb() == _excludedColor.ToArgb())
            {
                point = _gridPixelPicker.GetNextPixel(image, point.X, point.Y);
            }
            return point;
        }
    }
}
