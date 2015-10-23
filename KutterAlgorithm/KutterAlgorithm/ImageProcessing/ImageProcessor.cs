using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImageProcessor;
using ImageProcessor.Imaging;

namespace Steganography.ImageProcessing
{
    public class ImageProcessor
    {
        public Bitmap ProcessImage(Bitmap image, ProcessingParameters parameters)
        {
            using (var imageFactory = new ImageFactory(preserveExifData: true))
            {
                using (var stream = new MemoryStream())
                {
                    image.Save(stream, ImageFormat.Png);
                    imageFactory.Load(stream);
                    Resize(imageFactory, parameters);
                    Crop(imageFactory, parameters);
                    Rotate(imageFactory, parameters);
                    imageFactory.Save(stream);
                    var result = new Bitmap(stream);
                    return result;
                }
            }
        }

        private void Resize(ImageFactory imageFactory, ProcessingParameters parameters)
        {
            if (parameters.ScaleX == 1 && parameters.ScaleY == 1)
            {
                return;
            }
            var size = new Size((int)(imageFactory.Image.Width * parameters.ScaleX), (int)(imageFactory.Image.Height * parameters.ScaleY));
            var resizeLayer = new ResizeLayer(size, ResizeMode.Stretch);
            imageFactory.Resize(resizeLayer);
        }

        private void Rotate(ImageFactory imageFactory, ProcessingParameters parameters)
        {
            if (parameters.RotationAngle == 0)
            {
                return;
            }
            imageFactory.Rotate(parameters.RotationAngle);
        }

        private void Crop(ImageFactory imageFactory, ProcessingParameters parameters)
        {
            if (parameters.CropPercentX == 0 && parameters.CropPercentY == 0)
            {
                return;
            }
            imageFactory.Crop(new CropLayer(0, 0, parameters.CropPercentX, parameters.CropPercentY, CropMode.Percentage));
        }
    }
}
