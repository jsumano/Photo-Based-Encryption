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
        public string TargetFilePath { get; set; }

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
                return ImagePath != null && TargetFilePath != null && 
                    Passcode != "" && Passcode != null && CryptoStatus == EncryptionStatus.Idle;
            }
        }

        /// <summary>
        /// The password entered by the user.
        /// </summary>
        public string Passcode { private get; set; }

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
            TargetFilePath = null;
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
        public void LoadTargetFile()
        {
            // Resets target file path
            TargetFilePath = null;

            // Instantiates file dialog.
            OpenFileDialog openFileDialog = new OpenFileDialog();
            // Returns if the user does not select a file.
            if (openFileDialog.ShowDialog() != true)
                return;

            // Set the path.
            TargetFilePath = openFileDialog.FileName;
        }

        /// <summary>
        /// Calls the file encryption.
        /// </summary>
        public async Task EncryptAsync()
        {
            CryptoStatus = EncryptionStatus.Encrypting;
            Encryption encryption = new Encryption();
            await Task.Run(()=>encryption.Encrypt(TargetFilePath, Passcode, ImagePath));
            await Task.Delay(3000);
            CryptoStatus = EncryptionStatus.Idle;
        }
    }
}
