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

        public void Encrypt(string targetFile, string password, string imagePath)
        {
            // Generate the salt from random pixel values.
            Bitmap bitmap = new Bitmap(imagePath);
            byte[] salt = PixelReader.GetPixelKey(bitmap);
        }
    }
}
