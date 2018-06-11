using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Security.Cryptography;
using System.Runtime.InteropServices;

namespace Photo_Based_Encryption
{
    class Encryption
    {

        private string imagePath = "";

        /// <summary>
        /// Calls the encryption method and sets the image path.
        /// </summary>
        /// <param name="path">The path for the seed image.</param>
        public void CallEncrypt(string targetPath, string pass, string photoPath)
        {
            imagePath = photoPath;
            FileEncrypt(targetPath, pass);

        }

        private void FileEncrypt(string targetFile, string password)
        {
            // Generate the salt from random pixel values.
            Bitmap bitmap = new Bitmap(imagePath);
            byte[] salt = PixelReader.GetPixelKey(bitmap);
        }
    }
}
