using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.IO;
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

            // Create a file stream for the output file.
            FileStream fsOutput = new FileStream(targetFile + ".aes", FileMode.Create);

            // Convert password into an array of bytes.
            byte[] passwordBytes = System.Text.Encoding.UTF8.GetBytes(password);

            // Create AES.
            Aes aes = Aes.Create();
            aes.BlockSize = 128;
            // Use the maximum keysize.
            aes.KeySize = 256;
            // Set the padding mode to PKCS7 which pads using bytes equal to the total number of padding bytes.
            aes.Padding = PaddingMode.PKCS7;
        }
    }
}
