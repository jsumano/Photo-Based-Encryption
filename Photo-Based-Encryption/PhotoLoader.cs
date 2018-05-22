﻿using System.Windows;
using System.Drawing;
using Microsoft.Win32;

namespace Photo_Based_Encryption
{
    class PhotoLoader
    {
        /// <summary>
        /// The verified loaded image
        /// </summary>
        public Bitmap Image { get; private set; }

        /// <summary>
        /// Opens the File Dialog to select the seed image.
        /// </summary>
        public void FileDialog()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image Files (*.bmp*.jpg*.png*.gif)|*.bmp;*.jpg*;.png*;.gif|Bitmap (*.bmp)|*.bmp|JPEG (*.jpg)|*.jpg|PNG (*.png)|*.png|GIF (*.gif)|*.gif|All Files (*.*)|*.*";

            // Returns if the user does not select a file
            if (openFileDialog.ShowDialog() != true)
                return;

            Bitmap loadedImage = new Bitmap(openFileDialog.FileName);
            PhotoResult photoStatus = Inspect(loadedImage);

            // If the image passes inspection it is assigned else an error message is returned
            if (photoStatus == PhotoResult.Approved)
                Image = loadedImage;
            else if (photoStatus == PhotoResult.FailedSize)
                MessageBox.Show("The minimum size for a seed image is 100x100. Please select a larger image file.");
            else if (photoStatus == PhotoResult.FailedComplexity)
                MessageBox.Show("This image is not sufficiently complex. Please select an image with a greater range of color values.");
        }

        /// <summary>
        /// Inspects the loaded image for size and complexity
        /// </summary>
        /// <param name="image">The image to inspect.</param>
        /// <returns></returns>
        private PhotoResult Inspect(Bitmap image)
        {
            if (image.Width < 100 || image.Height < 100)
                return PhotoResult.FailedSize;
            else
                return PhotoResult.Approved;
        }
        
    }
}
