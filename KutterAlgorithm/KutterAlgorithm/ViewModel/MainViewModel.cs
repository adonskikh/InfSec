using System.Collections;
using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Threading;
using GalaSoft.MvvmLight;
using System;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Threading;
using KutterAlgorithm.Encoders;

namespace KutterAlgorithm.ViewModel
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

        private readonly KutterEncoder _encoder = new KutterEncoder();
        private string _originalText;
        private string _imagePath;
        private string _decodedText;
        private int _delta;
        private double _lambda;
        private const int MinDelta = 1;
        private const double MinAlpha = 0.01;
        private const int MaxDelta = 20;
        private const double MaxAlpha = 0.3;
        private const int DeltaStep = 1;
        private const double AlphaStep = 0.01;


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

        public ObservableCollection<ChartPoint> PerrDeltaChartData { get; private set; }
        public ObservableCollection<ChartPoint> MseDeltaChartData { get; private set; }
        public ObservableCollection<ChartPoint> PerrAlphaChartData { get; private set; }
        public ObservableCollection<ChartPoint> MseAlphaChartData { get; private set; }

        public RelayCommand OpenImgCommand { get; private set; }
        public RelayCommand EncodeCommand { get; private set; }
        public RelayCommand AnalyzeCommand { get; private set; }


        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel()
        {
            PerrDeltaChartData = new ObservableCollection<ChartPoint>();
            MseDeltaChartData = new ObservableCollection<ChartPoint>();
            PerrAlphaChartData = new ObservableCollection<ChartPoint>();
            MseAlphaChartData = new ObservableCollection<ChartPoint>();
            _delta = 2;
            _lambda = 0.01;
            _imagePath = @"Resources\WP_20150826_18_17_01_Pro.png";
            OpenImgCommand = new RelayCommand(OpenImg);
            EncodeCommand = new RelayCommand(Encode, () => !string.IsNullOrEmpty(ImagePath));
            AnalyzeCommand = new RelayCommand(Analyze, () => !string.IsNullOrEmpty(ImagePath));
            OriginalText =
                "Lorem ipsum dolor sit amet, consectetur adipisicing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. " +
                "Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat.";
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
                var image = (Bitmap)Image.FromFile(ImagePath, true);
                var newImg = _encoder.Encode(OriginalText, image, Delta, Lambda);
                var newPath = Path.Combine(Path.GetDirectoryName(ImagePath),
                                           Path.GetFileNameWithoutExtension(ImagePath) + DateTime.Now.ToString("_ddMMyyyyHHmmss") + Path.GetExtension(ImagePath));
                newImg.Save(newPath);

                newImg = (Bitmap)Image.FromFile(newPath, true);
                try
                {
                    DecodedText = _encoder.Decode(newImg, Delta, Lambda);
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

        #region Errors & graphs
        private void Analyze()
        {
            var image = (Bitmap)Image.FromFile(ImagePath, true);
            AnalyzeDeltaMse(image, OriginalText, Lambda);
            AnalyzeDeltaPerr(image, OriginalText, Lambda);
            AnalyzeAlphaMse(image, OriginalText, Delta);
            AnalyzeAlphaPerr(image, OriginalText, Delta);
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
                    var point = new ChartPoint() { Parameter = delta, Value = mse };
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
                for (double alpha = MinAlpha; alpha <= MaxAlpha; alpha += AlphaStep)
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
                for (double alpha = MinAlpha; alpha <= MaxAlpha; alpha += AlphaStep)
                {
                    var mse = CalculateMse(text, image, delta, alpha).Result;
                    var point = new ChartPoint() { Parameter = alpha, Value = mse };
                    DispatcherHelper.CheckBeginInvokeOnUI(() => MseAlphaChartData.Add(point));
                }
            });
        }

        private Task<double> CalculatePerr(string text, Bitmap image, int delta, double alpha)
        {
            return Task.Factory.StartNew(() =>
            {
                var kutter = new KutterEncoder();
                return kutter.CalculatePerr(text, image, delta, alpha);
            });
        }

        private Task<double> CalculateMse(string text, Bitmap image, int delta, double alpha)
        {
            return Task.Factory.StartNew(() =>
            {
                var kutter = new KutterEncoder();
                return kutter.CalculateMse(text, image, delta, alpha);
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