using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
    }
}
