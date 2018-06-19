using PropertyChanged;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using Microsoft.Win32;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Photo_Based_Encryption
{

    [AddINotifyPropertyChangedInterface]
    public class ViewModel : INotifyPropertyChanged
    {
       
        public event PropertyChangedEventHandler PropertyChanged = (sender, e) => { };

        /// <summary>
        /// The file path for the seed image.
        /// </summary>
        public string ImagePath { get; set; }

        /// <summary>
        /// The file path of the file to encrypt.
        /// </summary>
        public string EncryptFilePath { get; set; }

        /// <summary>
        /// The file path of the file to decrypt.
        /// </summary>
        public string DecryptFilePath { get; set; }

        /// <summary>
        /// A description of the current status.
        /// </summary>
        public string StatusText { get; set; }

        /// <summary>
        /// Bound bool that controls whether the encryption button is enabled. Returns true when all the conditions for file
        /// encryption have been met.
        /// </summary>
        public bool ReadyToEncrypt
        {
            get
            {
                return ImagePath != null && EncryptFilePath != null && 
                    EncryptPasscode != "" && EncryptPasscode != null && CryptoStatus == EncryptionStatus.Idle;
            }
        }

        /// <summary>
        /// Bound bool that controls whether the decrypt button is enabled. Returns true when all the conditions for file
        /// decryption have been met.
        /// </summary>
        public bool ReadyToDecrypt
        {
            get
            {
                return DecryptFilePath != null && DecryptPasscode != "" && DecryptPasscode != null && CryptoStatus == EncryptionStatus.Idle;
            }
        }

        /// <summary>
        /// The password entered by the user for encryption.
        /// </summary>
        public string EncryptPasscode { private get; set; }

        /// <summary>
        /// The password entered by the user for encryption.
        /// </summary>
        public string DecryptPasscode { private get; set; }

        /// <summary>
        /// The current encryption status.
        /// </summary>
        public EncryptionStatus CryptoStatus { get; set; }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public ViewModel()
        {
            StatusText = "Please select a seed image for encryption.";
            ImagePath = null;
            EncryptFilePath = null;
            DecryptFilePath = null;
            CryptoStatus = EncryptionStatus.Idle;
        }

        /// <summary>
        /// Selects the photo to be used for encryption seeding.
        /// </summary>
        /// <returns></returns>
        public async Task LoadPhotoAsync()
        {
            // Reset the image path.
            ImagePath = null;
            
            // Stores the file path selected by the user.
            string loadedPath = PhotoLoader.FileDialog();

            if (loadedPath == null)
                return;

            // Inspects the photo for size and complexity
            StatusText = "Analyzing image...";
            // Create a bitmap of the image file to inspect.
            Bitmap loadedPhoto = new Bitmap(loadedPath);
            // Inspect the image.
            PhotoResult result = await PhotoLoader.InspectAsync(loadedPhoto);

            // If the image passes inspection it is assigned else an error message is returned.
            if (result == PhotoResult.Approved)
            {
                ImagePath = loadedPath;
                StatusText = "Approved";
                return;
            }
            else if (result == PhotoResult.FailedSize)
                StatusText = "The minimum size for a seed image is 100x100. Please select a larger image file.";
            else if (result == PhotoResult.FailedComplexity)
                StatusText = "This image is not sufficiently complex. Please select an image with a greater range of color values.";

            // Resets the image path if the image fails inspection.
            ImagePath = null;
        }

        /// <summary>
        /// Selects the path of the file to be encrypted.
        /// </summary>
        public void LoadEncryptTargetFile()
        {
            // Resets target file path
            EncryptFilePath = null;

            // Instantiates file dialog.
            OpenFileDialog openFileDialog = new OpenFileDialog();
            // Returns if the user does not select a file.
            if (openFileDialog.ShowDialog() != true)
                return;

            // Set the path.
            EncryptFilePath = openFileDialog.FileName;
        }

        /// <summary>
        /// Selects the path of the file to be decrypted.
        /// </summary>
        public void LoadDecryptTargetFile()
        {
            // Resets target file path
            DecryptFilePath = null;

            // Instantiates file dialog.
            OpenFileDialog openFileDialog = new OpenFileDialog();
            // Returns if the user does not select a file.
            if (openFileDialog.ShowDialog() != true)
                return;

            // Set the path.
            DecryptFilePath = openFileDialog.FileName;
        }

        /// <summary>
        /// Calls the file encryption.
        /// </summary>
        public async Task EncryptAsync()
        {
            CryptoStatus = EncryptionStatus.Encrypting;
            StatusText = "Encrypting...";

            Encryption encryption = new Encryption();
            await Task.Run(()=>encryption.Encrypt(EncryptFilePath, EncryptPasscode, ImagePath));

            // Reset statuses once completed.
            CryptoStatus = EncryptionStatus.Idle;
            StatusText = "Encryption Complete.";
        }

        /// <summary>
        /// Calls the file encryption.
        /// </summary>
        public async Task DecryptAsync()
        {
            CryptoStatus = EncryptionStatus.Decrypting;
            StatusText = "Encrypting...";

            Encryption encryption = new Encryption();
            await Task.Run(() => encryption.Decrypt(DecryptFilePath, DecryptPasscode));

            // Reset statuses once completed.
            CryptoStatus = EncryptionStatus.Idle;
        }
    }
}
