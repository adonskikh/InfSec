using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Steganography.Encoders;

namespace Steganography.Signing
{
    public class SimpleHashSigner
    {
        private readonly IEncoder _signatureEncoder;

        public SimpleHashSigner(IEncoder signatureEncoder)
        {
            _signatureEncoder = signatureEncoder;
        }

        public Bitmap Sign(Bitmap unsignedImage)
        {
            var image = (Bitmap)unsignedImage.Clone();
            System.Windows.MessageBox.Show("1");
            return image;
        }
    }
}
