using System.Collections.Generic;
using System.Windows;
using System.Drawing;
using Microsoft.Win32;
using System.Threading.Tasks;

namespace Photo_Based_Encryption
{
    class PhotoLoader
    {
        /// <summary>
        /// Opens the File Dialog to select the seed image. Returns a Bitmap of the selected image.
        /// </summary>
        public string FileDialog()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Image Files (*.bmp*.jpg*.png*.gif)|*.bmp;*.jpg;*.png;*.gif|Bitmap (*.bmp)|*.bmp|JPEG (*.jpg)|*.jpg|PNG (*.png)|*.png|GIF (*.gif)|*.gif|All Files (*.*)|*.*"
            };

            // Resets the path and returns if the user does not select a file.
            if (openFileDialog.ShowDialog() != true)
                return null;

            return openFileDialog.FileName;
        }


        /// <summary>
        /// Inspects the loaded image for size and complexity.
        /// </summary>
        /// <param name="image">The image to inspect.</param>
        /// <returns></returns>
        public PhotoResult Inspect(Bitmap image)
        {

            if (image.Width < 100 || image.Height < 100)
                return PhotoResult.FailedSize;
            else if (PixelReader.ColorCount(image, 10) == false)
                return PhotoResult.FailedComplexity;
            else
                return PhotoResult.Approved;
        }
        
    }
}
