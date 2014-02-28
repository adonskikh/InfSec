using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System
{
    public static class SystemExtention
    {
        public static uint ShiftLeft(this uint value, int shift)
        {
            const int size = sizeof(uint) * 8;
            shift = shift % size;
            return ((value << shift) | (value >> (size - shift)));
        }

        public static uint ShiftRight(this uint value, int shift)
        {
            const int size = sizeof(int) * 8;
            shift = shift % size;
            return ((value >> shift) | (value << (size - shift)));
        }

        public static ulong ShiftLeft(this ulong value, int shift)
        {
            const int size = sizeof(long) * 8;
            shift = shift % size;
            return ((value << shift) | (value >> (size - shift)));
        }

        public static ulong ShiftRight(this ulong value, int shift)
        {
            const int size = sizeof(long) * 8;
            shift = shift % size;
            return ((value >> shift) | (value << (size - shift)));
        }

        public static ushort ShiftLeft(this ushort value, int shift)
        {
            const int size = sizeof(short) * 8;
            shift = shift % size;
            return (ushort)((value << shift) | (value >> (size - shift)));
        }

        public static ushort ShiftRight(this ushort value, int shift)
        {
            const int size = sizeof(short) * 8;
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

        public static BitArray ToBitArray(this ushort s)
        {
            return new BitArray(s.ToByteArray());
        }
        

        public static string ToBitString(this string input)
        {
            return ConvertBytesToBitString(input.ToByteArray());
        }

        public static string ToBitString(this ulong input)
        {
            return ConvertBytesToBitString(input.ToByteArray());
        }

        public static string ToBitString(this uint input)
        {
            return ConvertBytesToBitString(input.ToByteArray());
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
                sb.Append(Convert.ToString(b, 2).PadLeft(8, '0'));
            }
            return sb.ToString();
        }
    }
}
