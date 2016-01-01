using System.Collections;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Threading;
using GalaSoft.MvvmLight;
using System;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Threading;
using Steganography.Encoders;
using Steganography.ImageProcessing;
using Steganography.Messages;
using Steganography.Signing;

namespace Steganography.ViewModel
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class ConcealmentViewModel : ViewModelBase
    {
        private string _originalImagePath;
        private string _processedImagePath;
        private SelectedArea _selectedArea;


        public string OriginalImagePath
        {
            get
            {
                return _originalImagePath;
            }
            set
            {
                if (_originalImagePath == value) return;
                _originalImagePath = value;
                RaisePropertyChanged(() => OriginalImagePath);
                OriginalImage = (Bitmap)Image.FromFile(OriginalImagePath, true);
            }
        }


        public string ProcessedImagePath
        {
            get
            {
                return _processedImagePath;
            }
            set
            {
                if (_processedImagePath == value) return;
                _processedImagePath = value;
                RaisePropertyChanged(() => ProcessedImagePath);
                ProcessedImage = (Bitmap)Image.FromFile(ProcessedImagePath, true);
            }
        }

        private Bitmap _originalImage;

        public Bitmap OriginalImage
        {
            get
            {
                return _originalImage;
            }
            set
            {
                if (_originalImage == value) return;
                _originalImage = value;
                RaisePropertyChanged(() => OriginalImage);
            }
        }

        private Bitmap _processedImage;

        public Bitmap ProcessedImage
        {
            get
            {
                return _processedImage;
            }
            set
            {
                if (_processedImage == value) return;
                _processedImage = value;
                RaisePropertyChanged(() => ProcessedImage);
            }
        }

        public RelayCommand OpenOriginalImgCommand { get; private set; }
        public RelayCommand OpenProcessedImgCommand { get; private set; }
        public RelayCommand HideCommand { get; private set; }
        public RelayCommand ShowCommand { get; private set; }


        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public ConcealmentViewModel()
        {
            OriginalImagePath = @"Resources\Document.png";
            OpenOriginalImgCommand = new RelayCommand(OpenOriginalImage);
            OpenProcessedImgCommand = new RelayCommand(OpenProcessedImage);
            HideCommand = new RelayCommand(Hide, () => !string.IsNullOrEmpty(OriginalImagePath) && _selectedArea != null);
            ShowCommand = new RelayCommand(Show, () => !string.IsNullOrEmpty(ProcessedImagePath));
            Messenger.Default.Register<AreaSelectedMessage>(this, msg => _selectedArea = msg.Area);
        }

        private void OpenOriginalImage()
        {
            var path = ShowOpenFileDialog();
            if (path != null)
            {
                OriginalImagePath = path;
            }
        }

        private void OpenProcessedImage()
        {
            var path = ShowOpenFileDialog();
            if (path != null)
            {
                ProcessedImagePath = path;
            }
        }

        private string ShowOpenFileDialog()
        {
            var dlg = new Microsoft.Win32.OpenFileDialog { DefaultExt = ".bmp", Filter = "Images|*.bmp;*.png" };
            var result = dlg.ShowDialog();

            return result == true ? dlg.FileName : null;
        }

        private void Hide()
        {
            try
            {
                var hider = new AreaHider();
                var newImg = hider.HideArea(OriginalImage, _selectedArea);
                var newPath = Path.Combine(Path.GetDirectoryName(OriginalImagePath),
                    string.Format("{0}_{1}_{2}{3}", Path.GetFileNameWithoutExtension(OriginalImagePath), "PROCESSED",
                        GetTimeStamp(), Path.GetExtension(OriginalImagePath))
                    );
                newImg.Save(newPath);
                ProcessedImagePath = newPath;
            }
            catch (Exception e)
            {
                System.Windows.MessageBox.Show(e.Message);
            }
        }

        private void Show()
        {
            try
            {
                var signer = new AreaHider();
                var result = signer.ShowArea(ProcessedImage);
                var path = Path.Combine(
                    Path.GetDirectoryName(OriginalImagePath),
                    string.Format("{0}_{1}_{2}{3}", Path.GetFileNameWithoutExtension(OriginalImagePath),
                        "RESTORED",
                        GetTimeStamp(),
                        Path.GetExtension(OriginalImagePath))
                    );
                result.Save(path);
                Process.Start(path);
            }
            catch (Exception e)
            {
                System.Windows.MessageBox.Show(e.Message);
            }
        }

        private string GetTimeStamp()
        {
            return DateTime.Now.ToString("yyyyMMddHHmmss");
        }


        //public override void Cleanup()
        //{
        //    // Clean up if needed

        //    base.Cleanup();
        //}
    }
}