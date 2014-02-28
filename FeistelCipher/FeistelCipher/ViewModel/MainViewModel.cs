using System.Collections;
using GalaSoft.MvvmLight;
using System;

namespace FeistelCipher.ViewModel
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class MainViewModel : ViewModelBase
    {
        private readonly FeistelEncipherer _feistelEncipherer = new FeistelEncipherer();
        private string _originalText;
        private string _encryptedText;
        private string _decryptedText;
        private string _key;

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
                Encrypt();
                RaisePropertyChanged(() => OriginalText);
                RaisePropertyChanged(() => OriginalTextAsBitArray);
            }
        }

        public string EncryptedText
        {
            get
            {
                return _encryptedText;
            }
            set
            {
                if (_encryptedText == value) return;
                _encryptedText = value;
                Decrypt();
                RaisePropertyChanged(() => EncryptedText);
                RaisePropertyChanged(() => EncryptedTextAsBitArray);
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

        public string EncryptedTextAsBitArray
        {
            get { return (_encryptedText ?? "").ToBitString(); }
        }

        public string DecryptedTextAsBitArray
        {
            get { return (_decryptedText ?? "").ToBitString(); }
        }

        public string Key
        {
            get
            {
                return _key;
            }
            set
            {
                if (_key == value) return;
                _key = AdjustLength(value);
                Encrypt();
                RaisePropertyChanged(() => Key);
            }
        }


        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel()
        {
            _key = "password";
            OriginalText =
                "Lorem ipsum dolor sit amet, consectetur adipisicing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.";
        }


        private string AdjustLength(string s)
        {
            var fixedText = s.TrimEnd(' ');
            fixedText = fixedText.PadRight(8 * (int)Math.Ceiling((double)fixedText.Length / 8));
            return fixedText;
        }

        private void Encrypt()
        {
            EncryptedText = _feistelEncipherer.Encrypt(OriginalText, Key);
        }

        private void Decrypt()
        {
            DecryptedText = _feistelEncipherer.Decrypt(EncryptedText, Key);
        }

        ////public override void Cleanup()
        ////{
        ////    // Clean up if needed

        ////    base.Cleanup();
        ////}
    }
}