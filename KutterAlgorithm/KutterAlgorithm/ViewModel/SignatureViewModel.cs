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
using Steganography.Signing;

namespace Steganography.ViewModel
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class SignatureViewModel : ViewModelBase
    {
        private string _unsignedImagePath;
        private string _signedImagePath;
        private int _delta;
        private double _lambda;
        private EncoderViewModel _selectedEncoder;


        public string UnsignedImagePath
        {
            get
            {
                return _unsignedImagePath;
            }
            set
            {
                if (_unsignedImagePath == value) return;
                _unsignedImagePath = value;
                RaisePropertyChanged(() => UnsignedImagePath);
            }
        }


        public string SignedImagePath
        {
            get
            {
                return _signedImagePath;
            }
            set
            {
                if (_signedImagePath == value) return;
                _signedImagePath = value;
                RaisePropertyChanged(() => SignedImagePath);
            }
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

        public RelayCommand OpenUnsignedImgCommand { get; private set; }
        public RelayCommand OpenSignedImgCommand { get; private set; }
        public RelayCommand SignCommand { get; private set; }
        public RelayCommand CheckCommand { get; private set; }


        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public SignatureViewModel()
        {
            Encoders = new ObservableCollection<EncoderViewModel>()
            {
                new EncoderViewModel("Kutter", () => new KutterEncoder(Delta, Lambda)),
                new EncoderViewModel("LSB", () => new LsbEncoder())
            };
            SelectedEncoder = Encoders.First();

            _delta = 2;
            _lambda = 0.01;
            _unsignedImagePath = @"Resources\WP_20150826_18_17_01_Pro.png";
            OpenUnsignedImgCommand = new RelayCommand(OpenUnsignedImage);
            OpenSignedImgCommand = new RelayCommand(OpenSignedImage);
            SignCommand = new RelayCommand(Sign, () => !string.IsNullOrEmpty(UnsignedImagePath));
            CheckCommand = new RelayCommand(CheckSignature, () => !string.IsNullOrEmpty(SignedImagePath));
        }

        private void OpenUnsignedImage()
        {
            var path = ShowOpenFileDialog();
            if (path != null)
            {
                UnsignedImagePath = path;
            }
        }

        private void OpenSignedImage()
        {
            var path = ShowOpenFileDialog();
            if (path != null)
            {
                SignedImagePath = path;
            }
        }

        private string ShowOpenFileDialog()
        {
            var dlg = new Microsoft.Win32.OpenFileDialog { DefaultExt = ".bmp", Filter = "Images|*.bmp;*.png" };
            var result = dlg.ShowDialog();

            return result == true ? dlg.FileName : null;
        }

        private void Sign()
        {
            try
            {
                var signer = new SimpleHashSigner(new LsbEncoder());
                var image = (Bitmap) Image.FromFile(UnsignedImagePath, true);
                var newImg = signer.Sign(image);
                var newPath = Path.Combine(Path.GetDirectoryName(UnsignedImagePath),
                    string.Format("{0}_{1}_{2}{3}", Path.GetFileNameWithoutExtension(UnsignedImagePath), "SIGNED",
                        DateTime.Now.ToString("ddMMyyyyHHmmss"), Path.GetExtension(UnsignedImagePath))
                    );
                newImg.Save(newPath);
                SignedImagePath = newPath;
            }
            catch (Exception e)
            {
                System.Windows.MessageBox.Show(e.Message);
            }
        }

        private void CheckSignature()
        {
        }


        ////public override void Cleanup()
        ////{
        ////    // Clean up if needed

        ////    base.Cleanup();
        ////}
    }
}