using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Steganography.Encoders;
using Steganography.Encoders.PixelPickers;
using Steganography.Messages;

namespace Steganography.ImageProcessing
{
    public class AreaHider
    {
        private readonly Color _hiddenAreaColor = Color.Lime;
        private const int IntSize = sizeof(int)*8;
        private readonly LsbEncoder _encoder;
        private readonly FeistelEncoder _feistelEncoder;
        private const string Key = "Password";

        public AreaHider()
        {
            var pixelPicker = new GridPixelPickerExcludeColor(1, _hiddenAreaColor);
            _encoder = new LsbEncoder(pixelPicker);
            _feistelEncoder = new FeistelEncoder();
        }

        public Bitmap HideArea(Bitmap img, SelectedArea area)
        {
            var serializedArea = SerializeImageArea(img, area);
            var encryptedBits = _feistelEncoder.Encrypt(serializedArea.ToStringFromBinary(), Key);
            var image = (Bitmap)img.Clone();
            using (var graphics = Graphics.FromImage(image))
            {
                graphics.FillRectangle(new SolidBrush(_hiddenAreaColor), area.X, area.Y, area.Width, area.Height);
            }
            return _encoder.Encode(encryptedBits, image);
        }

        public Bitmap ShowArea(Bitmap img)
        {
            var bits = _encoder.Decode(img);
            var deryptedBits = _feistelEncoder.Decrypt(bits, Key).ToBitString();
            var image = (Bitmap)img.Clone();
            return DeserializeImageArea(image, deryptedBits);
        }

        private Bitmap GetArea(Bitmap img, SelectedArea area)
        {
            return img.GetArea(area.X, area.Y, area.Width, area.Height);
        }

        private string SerializeImageArea(Bitmap img, SelectedArea area)
        {
            var hiddenArea = GetArea(img, area);
            var bits = hiddenArea.ToBitString();
            bits = area.X.ToBitString() + area.Y.ToBitString() + area.Width.ToBitString() + area.Height.ToBitString() + bits;
            return bits;
        }

        private Bitmap DeserializeImageArea(Bitmap fullImg, string bits)
        {
            var xBits = bits.Substring(0, IntSize);
            var yBits = bits.Substring(IntSize * 1, IntSize);
            var widthBits = bits.Substring(IntSize * 2, IntSize);
            var heightBits = bits.Substring(IntSize * 3, IntSize);
            var x = xBits.ToIntFromBinary();
            var y = yBits.ToIntFromBinary();
            var width = widthBits.ToIntFromBinary();
            var height = heightBits.ToIntFromBinary();
            var imgBits = bits.Substring(IntSize * 4);
            var hiddenArea = imgBits.ToBitmapFromBinary(width, height);
            using (var graphics = Graphics.FromImage(fullImg))
            {
                graphics.DrawImage(hiddenArea, x, y);
            }
            return fullImg;
        }
    }
}
