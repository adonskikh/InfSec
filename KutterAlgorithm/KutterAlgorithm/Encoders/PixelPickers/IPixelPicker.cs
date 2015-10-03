using System.Drawing;

namespace KutterAlgorithm.Encoders.PixelPickers
{
    /// <summary>
    /// Общий интерфейс для классов, которые выбирают, в какую точку изображения записывать следующий бит данных
    /// </summary>
    public interface IPixelPicker
    {
        /// <summary>
        /// Определяет точку, в которую следует записывать следующий бит данных по координатам текущей точки
        /// </summary>
        /// <returns></returns>
        Point GetNextPixel(Bitmap image, int x, int y);
    }
}
