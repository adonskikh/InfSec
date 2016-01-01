using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using GalaSoft.MvvmLight.Messaging;
using Steganography.Messages;
using Steganography.ViewModel;

namespace Steganography
{
    /// <summary>
    /// This application's main window.
    /// </summary>
    public partial class MainWindowConcealment : Window
    {
        private bool _isDragging = false;
        private Point _anchorPoint = new Point();
        private Point _anchorPointImg = new Point();
        private Point _currentPointImg = new Point();

        /// <summary>
        /// Initializes a new instance of the MainWindow class.
        /// </summary>
        public MainWindowConcealment()
        {
            InitializeComponent();
            Closing += (s, e) => ViewModelLocator.Cleanup();
        }

        private void OriginalImageContainer_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            _anchorPoint.X = e.GetPosition(BackPlane).X;
            _anchorPoint.Y = e.GetPosition(BackPlane).Y;
            _anchorPointImg.X = e.GetPosition(OriginalImage).X;
            _anchorPointImg.Y = e.GetPosition(OriginalImage).Y;
            _isDragging = true;
        }

        private void OriginalImageContainer_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (_isDragging)
            {
                var x = e.GetPosition(BackPlane).X;
                var y = e.GetPosition(BackPlane).Y;
                _currentPointImg.X = e.GetPosition(OriginalImage).X;
                _currentPointImg.Y = e.GetPosition(OriginalImage).Y;
//                Debug.WriteLine("Canvas: x = {0} y = {1} X = {2} Y = {3} Width = {4} Height = {5}", x, y, _anchorPoint.X, _anchorPoint.Y, Math.Abs(x - _anchorPoint.X), Math.Abs(y - _anchorPoint.Y));
//                Debug.WriteLine("Image: x = {0} y = {1} X = {2} Y = {3} Width = {4} Height = {5}", _currentPointImg.X, _currentPointImg.Y, _anchorPointImg.X, _anchorPointImg.Y, Math.Abs(_currentPointImg.X - _anchorPointImg.X), Math.Abs(_currentPointImg.Y - _anchorPointImg.Y));

                SelectRectangle.SetValue(Canvas.LeftProperty, Math.Min(x, _anchorPoint.X));
                SelectRectangle.SetValue(Canvas.TopProperty, Math.Min(y, _anchorPoint.Y));

                SelectRectangle.Width = Math.Abs(x - _anchorPoint.X);
                SelectRectangle.Height = Math.Abs(y - _anchorPoint.Y);

                if (SelectRectangle.Visibility != Visibility.Visible)
                {
                    SelectRectangle.Visibility = Visibility.Visible;
                }
            }
        }

        private void OriginalImageContainer_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            ResetSelection();
        }

        private void OriginalImageContainer_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            ResetSelection();
        }

        private void ResetSelection()
        {
            //SelectRectangle.Visibility = Visibility.Collapsed;
            if (_isDragging)
            {
                _isDragging = false;
                var x = (int)Math.Min(_anchorPointImg.X, _currentPointImg.X);
                var y = (int)Math.Min(_anchorPointImg.Y, _currentPointImg.Y);
                var width = (int)Math.Abs(_currentPointImg.X - _anchorPointImg.X);
                var height = (int)Math.Abs(_currentPointImg.Y - _anchorPointImg.Y);
                Messenger.Default.Send(new AreaSelectedMessage(new SelectedArea(x, y, width, height)));
            }
        }
    }
}