﻿using PropertyChanged;
using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using W32 = Microsoft.Win32;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;


namespace Photo_Based_Encryption
{

    [AddINotifyPropertyChangedInterface]
    public class ViewModel : INotifyPropertyChanged
    {

        public event PropertyChangedEventHandler PropertyChanged = (sender, e) => { };

        #region Public Properties
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
        /// The destination file directory for the decrypted file.
        /// </summary>
        public string DestinationFilePath { get; set; }

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
                    !String.IsNullOrEmpty(EncryptPasscode) && CryptoStatus == EncryptionStatus.Idle;
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
                return DecryptFilePath != null && DestinationFilePath != null && !String.IsNullOrEmpty(DecryptPasscode) && CryptoStatus == EncryptionStatus.Idle;
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

        #endregion

        /// <summary>
        /// Default constructor.
        /// </summary>
        public ViewModel()
        {
            StatusText = "Please select a seed image for salt generation.";
            ImagePath = null;
            EncryptFilePath = null;
            DecryptFilePath = null;
            DestinationFilePath = null;
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

            Bitmap image = default;
            try
            {
                image = new Bitmap(loadedPath);
            }
            catch (Exception)
            {
                StatusText = "Invalid file or file type.";
                return;
            }

            // Inspects the photo for size and complexity
            StatusText = "Analyzing image...";
            PhotoResult result = await PhotoLoader.InspectAsync(image);

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
            W32.OpenFileDialog openFileDialog = new W32.OpenFileDialog();
            // Returns if the user does not select a file.
            if (openFileDialog.ShowDialog() != true)
                return;

            if (Path.GetExtension(openFileDialog.FileName) == ".aes")
            {
                MainWindow.Message("Encrypted files cannot be encrypted a second time. Please select a file that has not been encrypted.");
                return;
            }
                
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
            W32.OpenFileDialog openFileDialog = new W32.OpenFileDialog() { Filter = "AES (*.aes)|*.aes" };

            // Returns if the user does not select a file.
            if (openFileDialog.ShowDialog() != true)
                return;

            // Set the path.
            DecryptFilePath = openFileDialog.FileName;
        }

        /// <summary>
        /// Selects the destination path for the decrypted output file.
        /// </summary>
        public void SelectDestination()
        {
            // Resets target file path
            DestinationFilePath = null;

            using(FolderBrowserDialog fbd = new FolderBrowserDialog())
            {
                if(fbd.ShowDialog() == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                {
                    DestinationFilePath = fbd.SelectedPath;
                }
            }
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

            // Reset to initial settings once encryption is complete.
            CryptoStatus = EncryptionStatus.Idle;
            StatusText = "Please select a seed image for salt generation.";
            ImagePath = "";
            EncryptFilePath = "";
            MainWindow.Message("Encryption complete!");
        }

        /// <summary>
        /// Calls the file decryption.
        /// </summary>
        public async Task DecryptAsync()
        {
            CryptoStatus = EncryptionStatus.Decrypting;

            Encryption encryption = new Encryption();
            CryptoResult result = await Task.Run(() => encryption.Decrypt(DecryptFilePath, DecryptPasscode, DestinationFilePath));

            if (result == CryptoResult.Complete)
            {
                MainWindow.Message("Decryption complete!");
                DecryptFilePath = "";
                DestinationFilePath = "";
            }
            else
                MainWindow.Message("Incorrect password. Please enter the password used to encrypt the file.");

            // Reset statuses once completed.
            CryptoStatus = EncryptionStatus.Idle;
        }

    }
}
