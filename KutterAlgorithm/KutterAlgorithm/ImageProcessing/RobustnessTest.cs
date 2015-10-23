using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Steganography.Encoders;
using Steganography.Encoders.ErrorEstimation;

namespace Steganography.ImageProcessing
{
    public class RobustnessTest
    {
        readonly ImageProcessor processor = new ImageProcessor();
        readonly BitReadingErrorEstimator errorEstimator = new BitReadingErrorEstimator();

        public List<RobustnessTestResult> Test(string imagePath, string text, IEncoder encoder)
        {
            var results = new List<RobustnessTestResult>();
            var emptyContainer = LoadImage(imagePath);
            var fullContainer = encoder.Encode(text, emptyContainer);
            var untransformedContainerErrorRate = errorEstimator.Estimate(text, fullContainer, encoder);
            for (var angle = 0; angle <= 0; angle++)
            {
                for (var scale = 1.0f; scale >= 1f; scale-=0.1f)
                {
                    for (var cropPercent = 0; cropPercent <= 20; cropPercent+= 4)
                    {
                        var parameters = new ProcessingParameters()
                        {
                            RotationAngle = angle,
                            ScaleX = scale,
                            ScaleY = scale,
                            CropPercentX = cropPercent,
                            CropPercentY = cropPercent
                        };
                        var result = RunSingleTest(imagePath, fullContainer, text, encoder, parameters);
                        result.ErrorRate = untransformedContainerErrorRate;
                        results.Add(result);
                    }
                }
            }
            return results;
        }

        /// <summary>
        /// Выполняет один тест чтения скрываемого текста при искажении с указанными параметрами и возвращает его результат
        /// </summary>
        /// <param name="sourceImagePath">Путь к исходному изображению</param>
        /// <param name="fullContainer">Заполненный контейнер</param>
        /// <param name="hiddenText"></param>
        /// <param name="encoder"></param>
        /// <param name="parameters"></param>
        /// <param name="saveProcessed"></param>
        /// <returns></returns>
        private RobustnessTestResult RunSingleTest(string sourceImagePath, Bitmap fullContainer, string hiddenText, IEncoder encoder, ProcessingParameters parameters, bool saveProcessed = true)
        {
            var processedImage = processor.ProcessImage(fullContainer, parameters);
            var result = new RobustnessTestResult()
            {
                Parameters = parameters,
                SourceImagePath = sourceImagePath
            };
            if (saveProcessed)
            {
                var processedPath = Path.Combine(
                    Path.GetDirectoryName(sourceImagePath),
                    "Processed",
                    string.Format("{0}_{1}{2}", Path.GetFileNameWithoutExtension(sourceImagePath), GetTimeStamp(), Path.GetExtension(sourceImagePath))
                    );
                processedImage.Save(processedPath);
                result.ProcessedImagePath = processedPath;
            }
            result.ErrorRateTransformed = errorEstimator.Estimate(hiddenText, processedImage, encoder);
            return result;
        }

        private Bitmap LoadImage(string imagePath)
        {
            return (Bitmap)Image.FromFile(imagePath, true);
        }

        private string GetTimeStamp()
        {
            return DateTime.Now.ToString("yyyyMMddHHmmssfff");
        }
    }
}
