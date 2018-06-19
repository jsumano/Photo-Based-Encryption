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
        /// <summary>
        /// Encrypts a file using a password and seed image.
        /// </summary>
        /// <param name="targetFile">The file to encrypt.</param>
        /// <param name="password">The password.</param>
        /// <param name="imagePath">The path of the seed image.</param>
        public void Encrypt(string targetFile, string password, string imagePath)
        {
            // Generate the salt from random pixel values.
            Bitmap bitmap = new Bitmap(imagePath);
            byte[] salt = PixelReader.GetPixelKey(bitmap);

            // Create a file stream for the output file.
            FileStream fsCrypt = new FileStream(targetFile + ".aes", FileMode.Create);

            // Convert password into an array of bytes.
            byte[] passwordBytes = System.Text.Encoding.UTF8.GetBytes(password);

            // Create AES.
            Aes aes = Aes.Create();
            aes.BlockSize = 128;
            // Use the maximum keysize.
            aes.KeySize = 256;
            // Set the padding mode to PKCS7 which pads using bytes equal to the total number of padding bytes.
            aes.Padding = PaddingMode.PKCS7;

            // Append the salt to the password and hash 5000 times.
            var key = new Rfc2898DeriveBytes(passwordBytes, salt, 5000);
            // Generate the key using the GetBytes method to return pseudo-random key bytes.
            aes.Key = key.GetBytes(aes.KeySize / 8);
            // Set the initialization vector.
            aes.IV = key.GetBytes(aes.BlockSize / 8);
            // Set the cipher mode.
            aes.Mode = CipherMode.CFB;

            // Write the salt to the zero index of the output file.
            fsCrypt.Write(salt, 0, salt.Length);

            // Create the cryptostream.
            CryptoStream cryptoStream = new CryptoStream(fsCrypt, aes.CreateEncryptor(), CryptoStreamMode.Write);

            FileStream fsInput = new FileStream(targetFile, FileMode.Open);

            int read;
            byte[] buffer = new byte[1048576];

            // Write to the cryptostream.

            try
            {
                while ((read = cryptoStream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    cryptoStream.Write(buffer, 0, read);
                }
            }
            catch (CryptographicException ex_CryptographicException)
            {
                Console.WriteLine("CryptographicException error: " + ex_CryptographicException.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
            
            // Close streams.
            try
            {
                cryptoStream.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error by closing CryptoStream: " + ex.Message);
            }
            finally
            {
                fsCrypt.Close();
                fsInput.Close();
            }
        }




        public void Decrypt(string inputFile, string password)
        {
            // Convert password into an array of bytes.
            byte[] passwordBytes = System.Text.Encoding.UTF8.GetBytes(password);
            // Create a new array with same length as salt to store salt.
            byte[] salt = new byte[32];

            // Open the encrypted file.
            FileStream fsCrypt = new FileStream(inputFile, FileMode.Open);
            // Read the salt into the salt buffer. 
            fsCrypt.Read(salt, 0, salt.Length);

            // Create the AES with the same settings as the encrypted file.
            Aes aes = Aes.Create();
            aes.BlockSize = 128;
            var key = new Rfc2898DeriveBytes(passwordBytes, salt, 50000);
            aes.Key = key.GetBytes(aes.KeySize / 8);
            aes.IV = key.GetBytes(aes.BlockSize / 8);
            aes.Padding = PaddingMode.PKCS7;
            aes.Mode = CipherMode.CFB;

            CryptoStream cs = new CryptoStream(fsCrypt, aes.CreateDecryptor(), CryptoStreamMode.Read);

            // Trims the .aes extension off the file name.
            string outputPath = "";
            for (int i = 0; i < inputFile.Length - 4; i++)
                outputPath += inputFile[i];

            FileStream fsOut = new FileStream(outputPath, FileMode.Create);

            int read;
            byte[] buffer = new byte[1048576];

            try
            {
                while ((read = cs.Read(buffer, 0, buffer.Length)) > 0)
                {
                    fsOut.Write(buffer, 0, read);
                }
            }
            catch (CryptographicException ex_CryptographicException)
            {
                Console.WriteLine("CryptographicException error: " + ex_CryptographicException.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }

            try
            {
                cs.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error by closing CryptoStream: " + ex.Message);
            }
            finally
            {
                fsOut.Close();
                fsCrypt.Close();
            }
        }
    }
}
