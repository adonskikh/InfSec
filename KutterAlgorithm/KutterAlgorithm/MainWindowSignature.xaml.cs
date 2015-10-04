using System.Windows;
using Steganography.ViewModel;

namespace Steganography
{
    /// <summary>
    /// This application's main window.
    /// </summary>
    public partial class MainWindowSignature : Window
    {
        /// <summary>
        /// Initializes a new instance of the MainWindow class.
        /// </summary>
        public MainWindowSignature()
        {
            InitializeComponent();
            Closing += (s, e) => ViewModelLocator.Cleanup();
        }
    }
}