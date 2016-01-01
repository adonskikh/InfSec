using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using Steganography.Model;

namespace System
{
    public static class SystemExtention
    {
        public const int BitsInByte = 8;

        public static uint ShiftLeft(this uint value, int shift)
        {
            const int size = sizeof(uint) * BitsInByte;
            shift = shift % size;
            return ((value << shift) | (value >> (size - shift)));
        }

        public static uint ShiftRight(this uint value, int shift)
        {
            const int size = sizeof(int) * BitsInByte;
            shift = shift % size;
            return ((value >> shift) | (value << (size - shift)));
        }

        public static ulong ShiftLeft(this ulong value, int shift)
        {
            const int size = sizeof(long) * BitsInByte;
            shift = shift % size;
            return ((value << shift) | (value >> (size - shift)));
        }

        public static ulong ShiftRight(this ulong value, int shift)
        {
            const int size = sizeof(long) * BitsInByte;
            shift = shift % size;
            return ((value >> shift) | (value << (size - shift)));
        }

        public static ushort ShiftLeft(this ushort value, int shift)
        {
            const int size = sizeof(short) * BitsInByte;
            shift = shift % size;
            return (ushort)((value << shift) | (value >> (size - shift)));
        }

        public static ushort ShiftRight(this ushort value, int shift)
        {
            const int size = sizeof(short) * BitsInByte;
            shift = shift % size;
            return (ushort)((value >> shift) | (value << (size - shift)));
        }


        public static byte[] ToByteArray(this string s)
        {
            return Encoding.Default.GetBytes(s);
        }

        public static byte[] ToByteArray(this ulong s)
        {
            return BitConverter.GetBytes(s);
        }

        public static byte[] ToByteArray(this uint s)
        {
            return BitConverter.GetBytes(s);
        }

        public static byte[] ToByteArray(this int s)
        {
            return BitConverter.GetBytes(s);
        }

        public static byte[] ToByteArray(this byte s)
        {
            return BitConverter.GetBytes(s);
        }

        public static byte[] ToByteArray(this ushort s)
        {
            return BitConverter.GetBytes(s);
        }


        public static BitArray ToBitArray(this string s)
        {
            return new BitArray(s.ToByteArray());
        }

        public static BitArray ToBitArray(this ulong s)
        {
            return new BitArray(s.ToByteArray());
        }

        public static BitArray ToBitArray(this uint s)
        {
            return new BitArray(s.ToByteArray());
        }

        public static BitArray ToBitArray(this int s)
        {
            return new BitArray(s.ToByteArray());
        }

        public static BitArray ToBitArray(this ushort s)
        {
            return new BitArray(s.ToByteArray());
        }


        public static string ToBitString(this string input)
        {
            return ConvertBytesToBitString(input.ToByteArray());
        }

        public static string ToStringFromBinary(this string data)
        {
            var byteList = new List<Byte>();

            for (int i = 0; i < data.Length; i += BitsInByte)
            {
                byteList.Add(Convert.ToByte(data.Substring(i, BitsInByte), 2));
            }

            return Encoding.Default.GetString(byteList.ToArray());
        }

        public static string ToBitString(this ulong input)
        {
            return ConvertBytesToBitString(input.ToByteArray());
        }

        public static string ToBitString(this uint input)
        {
            return ConvertBytesToBitString(input.ToByteArray());
        }

        public static string ToBitString(this byte input)
        {
            return Convert.ToString(input, 2).PadLeft(8, '0');
        }

        public static string ToBitString(this int input)
        {
            return ConvertBytesToBitString(input.ToByteArray());
        }

        public static int ToIntFromBinary(this string data)
        {
            var byteList = new List<Byte>();

            for (int i = 0; i < data.Length; i += BitsInByte)
            {
                byteList.Add(Convert.ToByte(data.Substring(i, BitsInByte), 2));
            }

            return BitConverter.ToInt32(byteList.ToArray(), 0);
        }

        public static byte ToByteFromBinary(this string data)
        {
            return Convert.ToByte(data.Substring(0, BitsInByte), 2);
        }

        public static string ToBitString(this ushort input)
        {
            return ConvertBytesToBitString(input.ToByteArray());
        }


        private static string ConvertBytesToBitString(IEnumerable<byte> bytes)
        {
            var sb = new StringBuilder();
            foreach (byte b in bytes)
            {
                sb.Append(Convert.ToString(b, 2).PadLeft(BitsInByte, '0'));
            }
            return sb.ToString();
        }
        public static string Reverse(this string s)
        {
            char[] charArray = s.ToCharArray();
            Array.Reverse(charArray);
            return new string(charArray);
        }
        
        /// <summary>
        /// Возвращает указанный фрагмент изображения как новый Bitmap
        /// </summary>
        /// <param name="img"></param>
        /// <param name="xStart"></param>
        /// <param name="yStart"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public static Bitmap GetArea(this Bitmap img, int xStart, int yStart, int width, int height)
        {
            var srcArea = new Rectangle(xStart, yStart, width, height);
            var destArea = new Rectangle(0, 0, width, height);
            var bitmap = new Bitmap(srcArea.Width, srcArea.Height, img.PixelFormat);

            using (var g = Graphics.FromImage(bitmap))
            {
                g.DrawImage(img, destArea, srcArea, GraphicsUnit.Pixel);
                return bitmap;
            }
        }
        
        /// <summary>
        /// Разбивает изображение на тайлы указанного размера
        /// </summary>
        /// <param name="img"></param>
        /// <param name="tileWidth"></param>
        /// <param name="tileHeight"></param>
        /// <returns></returns>
        public static List<Tile> SplitIntoTiles(this Bitmap img, int tileWidth, int tileHeight)
        {
            var tiles = new List<Tile>();
            for (var y = 0; y < img.Height; y += tileHeight)
            {
                if (y + tileHeight - 1 >= img.Height)
                {
                    continue;
                }
                for (var x = 0; x < img.Width; x += tileWidth)
                {
                    if (x + tileWidth - 1 >= img.Width)
                    {
                        continue;
                    }
                    tiles.Add(new Tile(x, y, img.GetArea(x, y, tileWidth, tileHeight)));
                }
            }
            return tiles;
        }

        /// <summary>
        /// Вычисляет среднюю яркость для пикселей указанного фрагмента изображения
        /// </summary>
        /// <param name="img"></param>
        /// <param name="xStart"></param>
        /// <param name="yStart"></param>
        /// <param name="areaWidth"></param>
        /// <param name="areaHeight"></param>
        /// <returns></returns>
        public static byte CalculateAvgAreaBrightness(this Bitmap img, int xStart, int yStart, int areaWidth, int areaHeight)
        {
            double sum = 0.0;
            for (var y = yStart; y < yStart + areaHeight; y++)
            {
                for (var x = xStart; x < xStart + areaWidth; x++)
                {
                    var pixel = img.GetPixel(x, y);
                    sum += pixel.R;
                    sum += pixel.G;
                    sum += pixel.B;
                }
            }
            checked
            {
                return (byte)(sum / (areaHeight * areaWidth * 3));
            }
        }

        public static string ToBitString(this Bitmap img)
        {
            var strBuilder = new StringBuilder();
            for (int y = 0; y < img.Height; y++)
            {
                for (int x = 0; x < img.Width; x++)
                {
                    var pixel = img.GetPixel(x, y);
                    strBuilder.Append(pixel.R.ToBitString());
                    strBuilder.Append(pixel.G.ToBitString());
                    strBuilder.Append(pixel.B.ToBitString());
                }
            }
            return strBuilder.ToString();
        }

        public static Bitmap ToBitmapFromBinary(this string bits, int width, int height)
        {
            var bitmap = new Bitmap(width, height);
            var x = 0;
            var y = 0;
            for (int i = 0; i < bits.Length; i += BitsInByte * 3)
            {
                var r = bits.Substring(i + 0 * BitsInByte, BitsInByte).ToByteFromBinary();
                var g = bits.Substring(i + 1 * BitsInByte, BitsInByte).ToByteFromBinary();
                var b = bits.Substring(i + 2 * BitsInByte, BitsInByte).ToByteFromBinary();
                bitmap.SetPixel(x, y, Color.FromArgb(r, g, b));
                x++;
                if (x >= width)
                {
                    x = 0;
                    y++;
                }
            }
            return bitmap;
        }
    }
}
