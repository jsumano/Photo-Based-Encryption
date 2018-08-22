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

        public static void Message(string text)
        {
            MessageBox.Show(text);
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
            await EncryptAsync();
        }

        private async Task EncryptAsync()
        {
            // Disable the buttons and the passwordbox until the encryption operation is complete.
            LoadImageButton.IsEnabled = false;
            LoadFiletoEncryptButton.IsEnabled = false;
            EncryptionPasswordbox.IsEnabled = false;
            EncryptButton.Content = "Encrypting...";

            await viewModel.EncryptAsync();
            MessageBox.Show("Encryption complete!");

            // Reset the UI.
            LoadFiletoEncryptButton.IsEnabled = true;
            EncryptionPasswordbox.IsEnabled = true;
            EncryptionPasswordbox.Password = "";
            EncryptButton.Content = "Encrypt";
            viewModel.StatusText = "Please select a seed image for salt generation.";
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
            await DecryptAsync();
        }

        private async Task DecryptAsync()
        {
            // Disable the buttons and the passwordbox until the decryption operation is complete.
            LoadFiletoDecryptButton.IsEnabled = false;
            DestinationButton.IsEnabled = false;
            DecryptButton.Content = "Decrypting...";

            CryptoResult result = await viewModel.DecryptAsync();
            if (result == CryptoResult.Complete)
            {
                MessageBox.Show("Decryption complete.");
                viewModel.DecryptFilePath = "";
                viewModel.DestinationFilePath = "";
            }

            else
                MessageBox.Show("Incorrect password. Please enter the password used to encrypt the file.");

            // Reset the UI.
            LoadFiletoDecryptButton.IsEnabled = true;
            DestinationButton.IsEnabled = true;
            DecryptPasswordbox.Password = "";
            DecryptButton.Content = "Decrypt";
        }

        private void EncryptionPasswordbox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            viewModel.EncryptPasscode = EncryptionPasswordbox.Password;
        }

        private void DestinationButton_Click(object sender, RoutedEventArgs e)
        {
            viewModel.SelectDestination();
        }

        private async void EncryptionPasswordbox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Return)
                return;
            if (viewModel.ReadyToEncrypt)
                await EncryptAsync();
        }

        private async void DecryptPasswordbox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Return)
                return;
            if (viewModel.ReadyToDecrypt)
                await DecryptAsync();
        }
    }
}
