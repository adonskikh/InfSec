using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Documents;
using Steganography.Encoders.PixelPickers;
using Steganography.Exceptions;

namespace Steganography.Encoders
{
    public abstract class EncoderBase : IEncoder
    {
        private int _size = 0;
        /// <summary>
        /// Количество раз, которые записывается длина сообщения
        /// </summary>
        private const int LengthRecordsCount = 15;
        private const int LengthRecordSize = sizeof(int) * SystemExtention.BitsInByte;
        protected IPixelPicker PixelPicker;

        public Bitmap Encode(string text, Bitmap img)
        {
            var binText = text.ToBitString();
            return EncodeBits(binText, img);
        }

        public Bitmap EncodeBits(string bitStr, Bitmap img)
        {
            var image = (Bitmap)img.Clone();
            int size = bitStr.Length;
            _size = size;
            bitStr = string.Concat(Enumerable.Repeat(size.ToBitString(), LengthRecordsCount)) + bitStr; // Добавление размера исходного сообщения к самому сообщению
            int x = 0, y = 0;
            foreach (var bit in bitStr)
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
            return DecodeBits(image).ToStringFromBinary();
        }

        public string DecodeBits(Bitmap image)
        {
            var result = "";
            int x = 0, y = 0;
            int bitCount = 0;
            int length = -1;
            while (result.Length < length || length < 0)
            {
                try
                {
                    var nextPixel = PixelPicker.GetNextPixel(image, x, y);
                    x = nextPixel.X;
                    y = nextPixel.Y;

                    result += ReadBit(image, x, y);
                    ++bitCount;

                    // Выделение длины сообщения (записывается как первые 32 бита кодированного сообщения)
                    if (bitCount == LengthRecordSize * LengthRecordsCount && length < 0)
                    {
                        var lengthRecords = new List<int>();
                        for (int i = 0; i < result.Length; i += LengthRecordSize)
                        {
                            var lengthBits = result.Substring(i, LengthRecordSize);
                            lengthRecords.Add(lengthBits.ToIntFromBinary());
                        }
                        // в качестве длины сообщения берется наиболее часто встречающееся значение, чтобы избежать части ошибок
                        length = lengthRecords
                            .GroupBy(l => l)
                            .OrderByDescending(g => g.Count())
                            .First() // throws InvalidOperationException if myArray is empty
                            .Key;
                        //length = _size;
                        result = "";
                    }
                }
                catch (SteganographyException e) // превышение размера изображения при чтении
                {
                    break;
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

    }
}