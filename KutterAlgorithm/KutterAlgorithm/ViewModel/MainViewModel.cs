using System.Collections;
using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Threading;
using GalaSoft.MvvmLight;
using System;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Threading;
using Steganography.Encoders;
using System.Collections.Generic;
using Steganography.Encoders.ErrorEstimation;
using Steganography.ImageProcessing;
using System.Xml.Serialization;
using System.Xml;

namespace Steganography.ViewModel
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class MainViewModel : ViewModelBase
    {
        public class ChartPoint
        {
            public double Value { get; set; }
            public double Parameter { get; set; }
        }

        private string _originalText;
        private string _imagePath;
        private string _decodedText;
        private int _delta;
        private double _lambda;
        private EncoderViewModel _selectedEncoder;


        public string OriginalText
        {
            get
            {
                return _originalText;
            }
            set
            {
                if (_originalText == value) return;
                _originalText = value;
                RaisePropertyChanged(() => OriginalText);
                RaisePropertyChanged(() => OriginalTextAsBitArray);
            }
        }

        public string ImagePath
        {
            get
            {
                return _imagePath;
            }
            set
            {
                if (_imagePath == value) return;
                _imagePath = value;
                RaisePropertyChanged(() => ImagePath);
            }
        }

        public string DecodedText
        {
            get
            {
                return _decodedText;
            }
            set
            {
                if (_decodedText == value) return;
                _decodedText = value;
                RaisePropertyChanged(() => DecodedText);
                RaisePropertyChanged(() => DecodedTextAsBitArray);
            }
        }

        public string OriginalTextAsBitArray
        {
            get { return (_originalText ?? "").ToBitString(); }
        }

        public string DecodedTextAsBitArray
        {
            get { return (_decodedText ?? "").ToBitString(); }
        }

        public int Delta
        {
            get
            {
                return _delta;
            }
            set
            {
                if (_delta == value) return;
                _delta = value;
                RaisePropertyChanged(() => Delta);
            }
        }

        public double Lambda
        {
            get
            {
                return _lambda;
            }
            set
            {
                if (_lambda == value) return;
                _lambda = value;
                RaisePropertyChanged(() => Lambda);
            }
        }

        public ObservableCollection<EncoderViewModel> Encoders { get; private set; }

        public EncoderViewModel SelectedEncoder
        {
            get { return _selectedEncoder; }
            set
            {
                _selectedEncoder = value;
                RaisePropertyChanged(() => SelectedEncoder);
            }
        }

        public RelayCommand OpenImgCommand { get; private set; }
        public RelayCommand EncodeCommand { get; private set; }


        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel()
        {
            PerrDeltaChartData = new ObservableCollection<ChartPoint>();
            MseDeltaChartData = new ObservableCollection<ChartPoint>();
            PerrAlphaChartData = new ObservableCollection<ChartPoint>();
            MseAlphaChartData = new ObservableCollection<ChartPoint>();

            Encoders = new ObservableCollection<EncoderViewModel>()
            {
                new EncoderViewModel("Kutter", () => new KutterEncoder(Delta, Lambda)),
                new EncoderViewModel("LSB", () => new LsbEncoder())
            };
            SelectedEncoder = Encoders.First();

            _delta = 2;
            _lambda = 0.01;
            _imagePath = @"Resources\WP_20150826_18_17_01_Pro.png";
            OpenImgCommand = new RelayCommand(OpenImg);
            EncodeCommand = new RelayCommand(Encode, () => !string.IsNullOrEmpty(ImagePath));
            AnalyzeCommand = new RelayCommand(Analyze, () => !string.IsNullOrEmpty(ImagePath));
            AnalyzeTransformCommand = new RelayCommand(AnalyzeTransform, () => !string.IsNullOrEmpty(ImagePath));
            OriginalText =
                "Lorem ipsum dolor sit amet, consectetur adipisicing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. ";
        }

        private void OpenImg()
        {
            var dlg = new Microsoft.Win32.OpenFileDialog { DefaultExt = ".bmp", Filter = "Images|*.bmp;*.png" };
            var result = dlg.ShowDialog();

            if (result == true)
            {
                ImagePath = dlg.FileName;
            }
        }

        private void Encode()
        {
            try
            {
                var encoder = SelectedEncoder.GetEncoderInstance();
                var image = (Bitmap)Image.FromFile(ImagePath, true);
                var newImg = encoder.Encode(OriginalText, image);
                var newPath = Path.Combine(Path.GetDirectoryName(ImagePath),
                    string.Format("{0}_{1}_{2}{3}", Path.GetFileNameWithoutExtension(ImagePath), SelectedEncoder.Name,  GetTimeStamp(), Path.GetExtension(ImagePath))
                    );
                newImg.Save(newPath);

                newImg = (Bitmap)Image.FromFile(newPath, true);
                try
                {
                    DecodedText = encoder.Decode(newImg);
                }
                catch (Exception)
                {
                    DecodedText = "При декодировании возникли ошибки";
                }
            }
            catch (Exception e)
            {
                System.Windows.MessageBox.Show(e.Message);
            }
        }

        private void Decode()
        {
        }

        private string GetTimeStamp()
        {
            return DateTime.Now.ToString("yyyyMMddHHmmss");
        }

        #region Errors & graphs

        private const int MinDelta = 1;
        private const double MinLambda = 0.002;
        private const int MaxDelta = 8;
        private const double MaxLambda = 0.03;
        private const int DeltaStep = 1;
        private const double LambdaStep = 0.002;

        public ObservableCollection<ChartPoint> PerrDeltaChartData { get; private set; }
        public ObservableCollection<ChartPoint> MseDeltaChartData { get; private set; }
        public ObservableCollection<ChartPoint> PerrAlphaChartData { get; private set; }
        public ObservableCollection<ChartPoint> MseAlphaChartData { get; private set; }
        public RelayCommand AnalyzeCommand { get; private set; }
        public RelayCommand AnalyzeTransformCommand { get; private set; }

        private void Analyze()
        {
            var image = (Bitmap)Image.FromFile(ImagePath, true);
            AnalyzeDeltaMse(image, OriginalText, Lambda);
            AnalyzeDeltaPerr(image, OriginalText, Lambda);
            AnalyzeAlphaMse(image, OriginalText, Delta);
            AnalyzeAlphaPerr(image, OriginalText, Delta);
        }

        private void AnalyzeTransform()
        {
            var test = new RobustnessTest();
            var encoder = SelectedEncoder.GetEncoderInstance();
            var text = OriginalText;
            var imagePath = ImagePath;
            var results = test.Test(imagePath, text, encoder);
            SaveResults(results);
        }

        private void SaveResults(List<RobustnessTestResult> results)
        {
            var path = Path.Combine(
                Path.GetDirectoryName(ImagePath), 
                "Processed",
                string.Format("Transform_{0}.xml", GetTimeStamp())
                );
            var serializer = new XmlSerializer(results.GetType());
            using (var writer = new StreamWriter(path))
            {
                serializer.Serialize(writer, results);
            }
        }


        private void AnalyzeDeltaPerr(Bitmap img, string text, double alpha)
        {
            PerrDeltaChartData.Clear();
            var image = (Bitmap)img.Clone();
            Task.Factory.StartNew(() =>
            {
                for (int delta = MinDelta; delta <= MaxDelta; delta += DeltaStep)
                {
                    var pErr = CalculatePerr(text, image, delta, alpha).Result;
                    var point = new ChartPoint() { Parameter = delta, Value = pErr };
                    DispatcherHelper.CheckBeginInvokeOnUI(() => PerrDeltaChartData.Add(point));
                }
            });
        }

        private void AnalyzeDeltaMse(Bitmap img, string text, double alpha)
        {
            MseDeltaChartData.Clear();
            var image = (Bitmap)img.Clone();
            Task.Factory.StartNew(() =>
            {
                for (int delta = MinDelta; delta <= MaxDelta; delta += DeltaStep)
                {
                    var mse = CalculateMse(text, image, delta, alpha).Result;
                    var point = new ChartPoint() { Parameter = delta, Value = Math.Round(mse, 2) };
                    DispatcherHelper.CheckBeginInvokeOnUI(() => MseDeltaChartData.Add(point));
                }
            });
        }

        private void AnalyzeAlphaPerr(Bitmap img, string text, int delta)
        {
            PerrAlphaChartData.Clear();
            var image = (Bitmap)img.Clone();
            Task.Factory.StartNew(() =>
            {
                for (double alpha = MinLambda; alpha <= MaxLambda; alpha += LambdaStep)
                {
                    var pErr = CalculatePerr(text, image, delta, alpha).Result;
                    var point = new ChartPoint() { Parameter = alpha, Value = pErr };
                    DispatcherHelper.CheckBeginInvokeOnUI(() => PerrAlphaChartData.Add(point));
                }
            });
        }

        private void AnalyzeAlphaMse(Bitmap img, string text, int delta)
        {
            MseAlphaChartData.Clear();
            var image = (Bitmap)img.Clone();
            Task.Factory.StartNew(() =>
            {
                for (double alpha = MinLambda; alpha <= MaxLambda; alpha += LambdaStep)
                {
                    var mse = CalculateMse(text, image, delta, alpha).Result;
                    var point = new ChartPoint() { Parameter = alpha, Value = mse };
                    DispatcherHelper.CheckBeginInvokeOnUI(() => MseAlphaChartData.Add(point));
                }
            });
        }

        private Task<double> CalculatePerr(string text, Bitmap emptyContainer, int delta, double alpha)
        {
            return Task.Factory.StartNew(() =>
            {
                var kutter = new KutterEncoder(delta, alpha);
                var fullContainer = kutter.Encode(text, emptyContainer);
                var estimator = new BitReadingErrorEstimator();
                return estimator.Estimate(text, fullContainer, kutter);
            });
        }

        private Task<double> CalculateMse(string text, Bitmap emptyContainer, int delta, double alpha)
        {
            return Task.Factory.StartNew(() =>
            {
                var kutter = new KutterEncoder(delta, alpha);
                var fullContainer = kutter.Encode(text, emptyContainer);
                var estimator = new MseEstimator();
                return estimator.Estimate(emptyContainer, fullContainer);
            });
        }
        #endregion

        ////public override void Cleanup()
        ////{
        ////    // Clean up if needed

        ////    base.Cleanup();
        ////}
    }
}