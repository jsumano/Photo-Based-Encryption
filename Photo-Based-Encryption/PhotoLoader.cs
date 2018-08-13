using System.Collections.Generic;
using System.Drawing;
using Microsoft.Win32;
using System.Threading.Tasks;
using System;

namespace Photo_Based_Encryption
{
    /// <summary>
    /// Performs photo loading operations.
    /// </summary>
    internal static class PhotoLoader
    {
        /// <summary>
        /// Opens the File Dialog to select the seed image. Returns the path of the selected image.
        /// </summary>
        public static string FileDialog()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Image Files (*.bmp*.jpg*.png*.gif)|*.bmp;*.jpg;*.png;*.gif|Bitmap (*.bmp)|*.bmp|JPEG (*.jpg)|*.jpg|PNG (*.png)|*.png|GIF (*.gif)|*.gif|All Files (*.*)|*.*"
            };

            // Returns if the user does not select a file.
            if (openFileDialog.ShowDialog() != true)
                return null;

            return openFileDialog.FileName;
        }


        /// <summary>
        /// Inspects the loaded image for size and complexity.
        /// </summary>
        /// <param name="image">The path of the image to inspect.</param>
        /// <returns></returns>
        public static async Task<PhotoResult> InspectAsync(Bitmap image)
        {
            // The image fails if it is less than 100x100 pixels.
            if (image.Width < 100 || image.Height < 100)
                return PhotoResult.FailedSize;

            // Checks the image to see if it contains enough different color values to reach the specified threshold.
            bool passComplexity = await Task.Run(() => PixelReader.ColorCount(image, 100));

            if (!passComplexity)
                return PhotoResult.FailedComplexity;
            else
                return PhotoResult.Approved;
        }

    }
}
