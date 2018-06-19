using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Photo_Based_Encryption
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ViewModel viewModel = new ViewModel();

        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = viewModel;
        }

        private async void LoadImageButton_Click(object sender, RoutedEventArgs e)
        {
            // Disable the LoadImageButton until the LoadPhotoAsync method has finished execution.
            LoadImageButton.IsEnabled = false;
            await viewModel.LoadPhotoAsync();
            LoadImageButton.IsEnabled = true;
        }

        private void LoadFiletoEncryptButton_Click(object sender, RoutedEventArgs e)
        {
            viewModel.LoadEncryptTargetFile();
        }

        private async void EncryptButton_Click(object sender, RoutedEventArgs e)
        {
            // Disable the other buttons and the passwordbox until the encryption operation is complete.
            // The encrypt button is handled by a bound bool in the viewmodel.
            LoadImageButton.IsEnabled = false;
            LoadFiletoEncryptButton.IsEnabled = false;
            EncryptionPasswordbox.IsEnabled = false;
            EncryptButton.Content = "Encrypting...";

            await viewModel.EncryptAsync();
            MessageBox.Show("Encryption complete!");

            // Reset the UI.
            LoadImageButton.IsEnabled = true;
            LoadFiletoEncryptButton.IsEnabled = true;
            EncryptionPasswordbox.IsEnabled = true;
            EncryptionPasswordbox.Password = "";
            EncryptButton.Content = "Encrypt";
            viewModel.StatusText = "Please select a seed image for encryption.";
            viewModel.ImagePath = "";
            viewModel.EncryptFilePath = "";
        }


        private void LoadFiletoDecryptButton_Click(object sender, RoutedEventArgs e)
        {
            viewModel.LoadDecryptTargetFile();
        }

        private void DecryptPasswordbox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            viewModel.DecryptPasscode = DecryptPasswordbox.Password;
        }

        private async void DecryptButton_Click(object sender, RoutedEventArgs e)
        {
            await viewModel.DecryptAsync();
        }

        private void EncryptionPasswordbox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            viewModel.EncryptPasscode = EncryptionPasswordbox.Password;
        }
    }
}
