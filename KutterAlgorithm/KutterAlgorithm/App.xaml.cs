using System;
using System.Windows;
using GalaSoft.MvvmLight.Threading;

namespace Steganography
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        static App()
        {
            DispatcherHelper.Initialize();
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
#if CONCEALMENT
            var mainView = new MainWindowConcealment();
            mainView.Show();
#else
#if SIGNATURE
            var mainView = new MainWindowSignature();
            mainView.Show();
#else
            var mainView = new MainWindow();
            mainView.Show();
#endif
#endif
        }
    }
}
