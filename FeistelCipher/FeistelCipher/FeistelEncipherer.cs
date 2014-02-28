using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FeistelCipher
{
    public class FeistelEncipherer
    {
        private class IterationResult
        {
            public uint LeftSubBlock { get; set; }
            public uint RightSubBlock { get; set; }
        }

        private const int NumberOfIterations = 10;

        public string Encrypt(string text, string key)
        {
            var textBlocks = Get64BitBlocks(text);
            var superKey = Get64BitBlocks(key).First();

            for (var i = 0; i < textBlocks.Count; i++)
            {
                var l = GetLeftSubBlock(textBlocks[i]);
                var r = GetRightSubBlock(textBlocks[i]);

                var iterationResult = new IterationResult()
                    {
                        LeftSubBlock = l,
                        RightSubBlock = r
                    };
                for (var iteration = 1; iteration <= NumberOfIterations; iteration++)
                {
                    iterationResult = PerformIteration(l, r, superKey, iteration);
                    l = iterationResult.RightSubBlock;
                    r = iterationResult.LeftSubBlock;
                }

                textBlocks[i] = JoinSubBlocks(iterationResult.LeftSubBlock, iterationResult.RightSubBlock);
            }
            return GetTextFromBlocks(textBlocks);
        }

        public string Decrypt(string text, string key)
        {
            var textBlocks = Get64BitBlocks(text);
            var superKey = Get64BitBlocks(key).First();
            for (var i = 0; i < textBlocks.Count; i++)
            {
                var l = GetLeftSubBlock(textBlocks[i]);
                var r = GetRightSubBlock(textBlocks[i]);

                var iterationResult = new IterationResult()
                {
                    LeftSubBlock = l,
                    RightSubBlock = r
                };
                for (var iteration = NumberOfIterations; iteration >= 1; iteration--)
                {
                    iterationResult = PerformIteration(l, r, superKey, iteration);
                    l = iterationResult.RightSubBlock;
                    r = iterationResult.LeftSubBlock;
                }

                textBlocks[i] = JoinSubBlocks(iterationResult.LeftSubBlock, iterationResult.RightSubBlock);
            }
            return GetTextFromBlocks(textBlocks);
        }

        private IterationResult PerformIteration(uint left, uint right, ulong superKey, int iteration)
        {
            var f = CalculateF(left, GetKey(superKey, iteration));
            right = right ^ f;
            return new IterationResult()
                {
                    LeftSubBlock = left,
                    RightSubBlock = right
                };
        }

        private uint CalculateF(uint subBlock, uint key)
        {
            ushort l0 = GetLeftSubBlock(subBlock);
            ushort l1 = GetRightSubBlock(subBlock);
            var leftNotK = (ushort)~GetLeftSubBlock(key);
            var l = (ushort)(l0 ^ leftNotK); ;
            var r = (ushort)(l1.ShiftRight(4) ^ GetRightSubBlock(key));
            return JoinSubBlocks(l, r);
        }


        private List<ulong> Get64BitBlocks(string text)
        {
            var textBytes = text.ToByteArray();

            const int n = sizeof(ulong);

            var textBlocks = new List<ulong>();
            var i = 0;
            while (i < textBytes.Length)
            {
                textBlocks.Add(BitConverter.ToUInt64(textBytes, i));
                i += n;
            }
            return textBlocks;
        }

        private string GetTextFromBlocks(IEnumerable<ulong> blocks)
        {
            IEnumerable<byte> textBytes = new List<byte>();
            foreach (var block in blocks)
            {
                textBytes = textBytes.Concat(BitConverter.GetBytes(block));
            }
            return Encoding.Default.GetString(textBytes.ToArray());
        }

        private uint GetLeftSubBlock(ulong block)
        {
            var bytes = BitConverter.GetBytes(block);
            return BitConverter.ToUInt32(bytes, 0);
        }

        private uint GetRightSubBlock(ulong block)
        {
            var bytes = BitConverter.GetBytes(block);
            return BitConverter.ToUInt32(bytes, sizeof(uint));
        }

        private ushort GetLeftSubBlock(uint block)
        {
            var bytes = BitConverter.GetBytes(block);
            return BitConverter.ToUInt16(bytes, 0);
        }

        private ushort GetRightSubBlock(uint block)
        {
            var bytes = BitConverter.GetBytes(block);
            return BitConverter.ToUInt16(bytes, sizeof(ushort));
        }

        private ulong JoinSubBlocks(uint leftSubBlock, uint rightSubBlock)
        {
            var bytes = BitConverter.GetBytes(leftSubBlock).Concat(BitConverter.GetBytes(rightSubBlock)).ToArray();
            return BitConverter.ToUInt64(bytes, 0);
        }

        private uint JoinSubBlocks(ushort leftSubBlock, ushort rightSubBlock)
        {
            var bytes = BitConverter.GetBytes(leftSubBlock).Concat(BitConverter.GetBytes(rightSubBlock)).ToArray();
            return BitConverter.ToUInt32(bytes, 0);
        }

        private uint GetKey(ulong superKey, int iteration)
        {
            var key = GetLeftSubBlock(superKey.ShiftLeft(iteration * 2));
            return key;
        }
    }
}
