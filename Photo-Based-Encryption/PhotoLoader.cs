using System.Windows;
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
            openFileDialog.Filter = "Image Files (*.bmp*.jpg*.png*.gif)|*.bmp*.jpg*.png*.gif|Bitmap (*.bmp)|*.bmp|JPEG (*.jpg)|*.jpg|PNG (*.png)|*.png|GIF (*.gif)|*.gif|All Files (*.*)|*.*";

            // Returns if the user does not select a file
            if (openFileDialog.ShowDialog() != true)
                return;


        }
        
    }
}
