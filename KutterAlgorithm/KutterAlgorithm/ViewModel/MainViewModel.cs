using System.Collections;
using System.Drawing;
using System.IO;
using GalaSoft.MvvmLight;
using System;
using GalaSoft.MvvmLight.Command;

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
        private readonly KutterEncipherer _kutterEncipherer = new KutterEncipherer();
        private string _originalText;
        private string _imagePath;
        private string _decryptedText;
        private int _delta;
        private double _alpha;


        public string OriginalText
        {
            get
            {
                return _originalText;
            }
            set
            {
                if (_originalText == value) return;
                _originalText = AdjustLength(value);
                //Encrypt();
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

        public string DecryptedText
        {
            get
            {
                return _decryptedText;
            }
            set
            {
                if (_decryptedText == value) return;
                _decryptedText = value;
                RaisePropertyChanged(() => DecryptedText);
                RaisePropertyChanged(() => DecryptedTextAsBitArray);
            }
        }

        public string OriginalTextAsBitArray
        {
            get { return (_originalText ?? "").ToBitString(); }
        }

        //public string EncryptedTextAsBitArray
        //{
        //    get { return (_encryptedText ?? "").ToBitString(); }
        //}

        public string DecryptedTextAsBitArray
        {
            get { return (_decryptedText ?? "").ToBitString(); }
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
                //Encrypt();
                RaisePropertyChanged(() => Delta);
            }
        }

        public double Alpha
        {
            get
            {
                return _alpha;
            }
            set
            {
                if (_alpha == value) return;
                _alpha = value;
                //Encrypt();
                RaisePropertyChanged(() => Alpha);
            }
        }

        public RelayCommand OpenImgCommand { get; private set; }
        public RelayCommand EncodeCommand { get; private set; }


        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel()
        {
            _delta = 2;
            _alpha = 0.01;
            _imagePath = @"C:\Users\Artyom\Desktop\TESV 2012-01-22 16-20-55-73.bmp";
            OpenImgCommand = new RelayCommand(OpenImg);
            EncodeCommand = new RelayCommand(Encode, () => !string.IsNullOrEmpty(ImagePath));
            OriginalText =
                "Lorem ipsum dolor sit amet, consectetur adipisicing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.";
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

        private string AdjustLength(string s)
        {
            var fixedText = s.TrimEnd(' ');
            fixedText = fixedText.PadRight(8 * (int)Math.Ceiling((double)fixedText.Length / 8));
            return fixedText;
        }

        private void Encode()
        {
            try
            {
                var image = (Bitmap)Image.FromFile(ImagePath, true);
                var newImg = _kutterEncipherer.Encode(OriginalText, image, Delta, Alpha);
                var newPath = Path.Combine(Path.GetDirectoryName(ImagePath),
                                           Path.GetFileNameWithoutExtension(ImagePath) + DateTime.Now.ToString("ddMMyyyyHHmmss") + Path.GetExtension(ImagePath));
                newImg.Save(newPath);

                newImg = (Bitmap)Image.FromFile(newPath, true);
                try
                {
                    DecryptedText = _kutterEncipherer.Decode(newImg, Delta, Alpha);
                }
                catch (Exception)
                {
                    DecryptedText = "При декодировании возникли ошибки";
                }
            }
            catch (Exception e)
            {
                System.Windows.MessageBox.Show(e.Message);
            }
        }

        private void Decode()
        {
            //DecryptedText = _feistelEncipherer.Decrypt(EncryptedText, Delta);
        }

        ////public override void Cleanup()
        ////{
        ////    // Clean up if needed

        ////    base.Cleanup();
        ////}
    }
}