using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Steganography.Signing
{
    public class SignatureValidationResult
    {
        public bool SignatureIsValid { get; set; }

        /// <summary>
        /// Изображение, на котором отмечены модифицированные участки изображения и нанесены прочие метки в ходе проверки подписи
        /// </summary>
        public Bitmap ImageWithValidationMarks { get; set; }
    }
}
