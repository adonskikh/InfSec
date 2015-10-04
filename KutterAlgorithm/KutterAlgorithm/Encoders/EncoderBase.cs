using System;
using System.Drawing;
using System.Linq;
using Steganography.Encoders.PixelPickers;

namespace Steganography.Encoders
{
    public abstract class EncoderBase : IEncoder
    {
        private int _size = 0;
        protected IPixelPicker PixelPicker;

        public Bitmap Encode(string text, Bitmap img)
        {
            var image = (Bitmap)img.Clone();
            var binText = text.ToBitString();
            int size = binText.Length;
            _size = size;
            binText = size.ToBitString() + binText; // Добавление размера исходного сообщения к самому сообщению
            int x = 0, y = 0;
            foreach (var bit in binText)
            {
                var nextPixel = PixelPicker.GetNextPixel(img, x, y);
                x = nextPixel.X;
                y = nextPixel.Y;

                WriteBit(image, x, y, bit);
            }
            return image;
        }

        /// <summary>
        /// Записывает 1 бит сообщения в указанную точку с координатами (x,y)
        /// </summary>
        /// <param name="image"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="bit">'1' или '0'</param>
        protected abstract void WriteBit(Bitmap image, int x, int y, char bit);

        public string Decode(Bitmap image)
        {
            return ReadBits(image).ToStringFromBinary();
        }

        private string ReadBits(Bitmap image)
        {
            var result = "";
            int x = 0, y = 0;
            int bitCount = 0;
            int length = -1;
            while (result.Length < length || length < 0)
            {
                var nextPixel = PixelPicker.GetNextPixel(image, x, y);
                x = nextPixel.X;
                y = nextPixel.Y;

                result += ReadBit(image, x, y);
                ++bitCount;

                // Выделение длины сообщения (записывается как первые 32 бита кодированного сообщения)
                if (bitCount == sizeof(int) * SystemExtention.BitsInByte && length < 0)
                {
                    length = result.ToIntFromBinary();
                    //length = _size;
                    result = "";
                }
            };
            return result;
        }

        /// <summary>
        /// Читает 1 бит закодированного сообщения для указанной точки с координатами (x,y)
        /// </summary>
        /// <param name="image"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns>'1' или '0'</returns>
        protected abstract char ReadBit(Bitmap image, int x, int y);

        public double CalculateMse(string text, Bitmap img)
        {
            var newImg = Encode(text, img);
            double err = 0;
            for (int x = 0; x < img.Width; x++)
            {
                for (int y = 0; y < img.Height; y++)
                {
                    err += Math.Pow(img.GetPixel(x, y).B - newImg.GetPixel(x, y).B, 2);
                }
            }
            return err / (img.Height * img.Width);
        }

        public double CalculatePerr(string text, Bitmap img)
        {
            var bits = text.ToBitString();
            var newImg = Encode(text, img);
            var newBits = ReadBits(newImg);
            double errors = Enumerable.Range(0, Math.Min(bits.Length, newBits.Length))
                .Count(i => bits[i] != newBits[i]);
            errors += Math.Abs(bits.Length - newBits.Length);
            return errors / bits.Length;
        }
    }
}