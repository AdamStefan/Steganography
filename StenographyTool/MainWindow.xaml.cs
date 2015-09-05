using System;
using System.Drawing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using ImageHelpers;
using Microsoft.Win32;

namespace StenographyTool
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        #region Instance

        public MainWindow()
        {
            InitializeComponent();
        }

        #endregion

        #region Methods

        private void SelectFileDialogForEncoding_OnClick(object sender, RoutedEventArgs e)
        { 
            var openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog(this) == true)
            {
                var file = openFileDialog.FileName;                
                if (System.IO.File.Exists(file))
                {
                    FileNameForEncoding.Text = file;
                    EncodingSourceImage.Source = new BitmapImage(new Uri(file));
                }
            }
            HideMessage.IsEnabled = !String.IsNullOrWhiteSpace(FileNameForEncoding.Text) && !String.IsNullOrWhiteSpace(TxtEncodedSecretMessage.Text);            
        }

        private void SelectFileDialogForDecoding_OnClick(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog(this) == true)
            {
                var file = openFileDialog.FileName;
                if (System.IO.File.Exists(file))
                {
                    FileNameForDecoding.Text = file;
                    DecodingSourceImage.Source = new BitmapImage(new Uri(file));
                }
            }
            GetSecretMessage.IsEnabled = !String.IsNullOrWhiteSpace(FileNameForDecoding.Text);
        }

        private void HideMessage_OnClick(object sender, RoutedEventArgs e)
        {
            if (String.IsNullOrWhiteSpace(FileNameForEncoding.Text))
            {
                return;
            }
            var message = TxtEncodedSecretMessage.Text;

            var saveFileDialog = new SaveFileDialog();
            if (saveFileDialog.ShowDialog(this) == true)
            {
                WatermarkProcessor.WatermarkDctWithLsb(FileNameForEncoding.Text, saveFileDialog.FileName, message, 16);
                FileNameForDecoding.Text = saveFileDialog.FileName;
                GetSecretMessage.IsEnabled = !String.IsNullOrWhiteSpace(FileNameForDecoding.Text);
                DecodingSourceImage.Source = new BitmapImage(new Uri(FileNameForDecoding.Text));
            }
        }

        private void TxtEncodedSecretMessage_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            HideMessage.IsEnabled = !String.IsNullOrWhiteSpace(FileNameForEncoding.Text) && !String.IsNullOrWhiteSpace(TxtEncodedSecretMessage.Text);
        }

        private void GetSecretMessage_OnClick(object sender, RoutedEventArgs e)
        {            
            if (String.IsNullOrWhiteSpace(FileNameForDecoding.Text))
            {
                return;
            }

            TxtDecodedSecretMessage.Text = string.Empty;

            var message = WatermarkProcessor.GetTextWatermarkDct((Bitmap)System.Drawing.Image.FromFile(FileNameForDecoding.Text), 16);
            TxtDecodedSecretMessage.Text = message;
        }

        #endregion
    }
}
