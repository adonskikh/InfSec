using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Steganography.Encoders.ErrorEstimation
{
    public class HammingDistanceCalculator
    {
        /// <summary> 
        /// Вычисляет расстояние хэмминга между символьными представлениями двух строк
        /// </summary>
        /// <param name="str1"></param>
        /// <param name="str2"></param>
        /// <returns></returns>
        public int Calculate(string str1, string str2)
        {
            int errors = Enumerable.Range(0, Math.Min(str1.Length, str2.Length))
                .Count(i => str1[i] != str2[i]);
            errors += Math.Abs(str1.Length - str2.Length);
            return errors;
        }

        /// <summary>
        /// Вычисляет расстояние хэмминга между битовыми представлениями двух строк
        /// </summary>
        /// <param name="str1"></param>
        /// <param name="str2"></param>
        /// <returns></returns>
        public int CalculateBinary(string str1, string str2)
        {
            var bits1 = str1.ToBitString();
            var bits2 = str2.ToBitString();
            return Calculate(bits1, bits2);
        }

        /// <summary>
        /// Оценивает долю неправильно прочитанных бит с использованием расстояния Хэмминга
        /// </summary>
        /// <param name="hiddenText"></param>
        /// <param name="fullContainer"></param>
        /// <param name="encoder"></param>
        /// <returns></returns>
        public double EstimateBitErrorRate(string hiddenText, Bitmap fullContainer, IEncoder encoder)
        {
            var bits = hiddenText.ToBitString();
            var newBits = encoder.DecodeBits(fullContainer);
            return ((double)CalculateBinary(bits, newBits)) / bits.Length;
        }
    }
}
