using System;
using System.Collections.Generic;
using System.Drawing;
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
        public ProcessingResult ProcessImage(string imagePath, ProcessingParameters parameters)
        {
            using (var imageFactory = new ImageFactory(preserveExifData: true))
            {
                imageFactory.Load(imagePath);
                Resize(imageFactory, parameters);
                Rotate(imageFactory, parameters);


                var newPath = Path.Combine(Path.GetDirectoryName(imagePath), "Processed",
                    string.Format("{0}_{1}{2}", Path.GetFileNameWithoutExtension(imagePath), DateTime.Now.ToString("ddMMyyyyHHmmss"), Path.GetExtension(imagePath))
                    );
                imageFactory.Save(newPath);
                var result = new ProcessingResult()
                {
                    SourceImagePath = imagePath,
                    ProcessedImagePath = newPath,
                    Parameters = parameters
                };
                return result;
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
            if (parameters.RotationAngle == 0)
            {
                return;
            }
            imageFactory.Rotate(parameters.RotationAngle);
        }
    }
}
