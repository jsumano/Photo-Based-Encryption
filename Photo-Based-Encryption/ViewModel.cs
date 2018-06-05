using PropertyChanged;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using Microsoft.Win32;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Photo_Based_Encryption
{

    [AddINotifyPropertyChangedInterface]
    public class ViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged = (sender, e) => { };

        private PhotoLoader photoLoader;

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
        /// Default constructor.
        /// </summary>
        public ViewModel()
        {
            photoLoader = new PhotoLoader();
            StatusText = "Please select a seed image for encryption.";
            ImagePath = null;
            TargetFilePath = null;
        }


        public async Task LoadPhotoAsync()
        {
            // Loads the photo into a temporary Bitmap
            string loadedPath = photoLoader.FileDialog();

            if (loadedPath == null)
            {
                ImagePath = null;
                return;
            }

            // Inspects the photo for size and complexity
            StatusText = "Analyzing image...";
            // Create a bitmap of the image file to inspect.
            Bitmap loadedPhoto = new Bitmap(loadedPath);
            PhotoResult result = await photoLoader.InspectAsync(loadedPhoto);

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

        public void Encrypt()
        {

        }
    }
}
