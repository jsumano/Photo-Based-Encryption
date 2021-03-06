﻿using System;
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
            byte[] salt = PixelReader.GetPixelKey(new Bitmap(imagePath));

            // Convert password into an array of bytes.
            byte[] passwordBytes = System.Text.Encoding.UTF8.GetBytes(password);

            // Convert file into an array of bytes;
            byte[] fileBytes = File.ReadAllBytes(targetFile);


            using (MemoryStream ms = new MemoryStream())
            {

                Aes aes = CreateAes(passwordBytes, salt);

                using (CryptoStream cs = new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write))
                {
                    cs.Write(fileBytes, 0, fileBytes.Length);
                }
                byte[] encryptedBytes = ms.ToArray();

                // Create an array to store the data to be written.
                byte[] data = new byte[salt.Length + encryptedBytes.Length];
                // Write the salt to the beginning of the array
                for (int i = 0; i < salt.Length; i++)
                    data[i] = salt[i];
                // Write the encrypted bytes after the salt in the array.
                for (int j = 0; j < encryptedBytes.Length; j++)
                    data[j + salt.Length] = encryptedBytes[j];

                using (FileStream fs = new FileStream(targetFile + ".aes", FileMode.Create))
                {
                    // Write the data to the file.
                    fs.Write(data, 0, data.Length);
                }
            }

        }



        /// <summary>
        /// Decrypts a file using a password.
        /// </summary>
        /// <param name="inputFile">The file to decrypt.</param>
        /// <param name="password">The password used to encrypt the file.</param>
        /// <param name="destination">The destination of the decrypted file.</param>
        public CryptoResult Decrypt(string inputFile, string password, string destination)
        {
            // Convert password into an array of bytes.
            byte[] passwordBytes = System.Text.Encoding.UTF8.GetBytes(password);

            using (MemoryStream ms = new MemoryStream())
            {
                
                byte[] buffer = File.ReadAllBytes(inputFile);
                // Create an array to store the salt
                byte[] salt = new byte[32];

                // Retrieve the salt from the buffer.
                for (int i = 0; i < salt.Length; i++)
                    salt[i] = buffer[i];

                Aes aes = CreateAes(passwordBytes, salt);

                string outputPath = GetOutputPath(inputFile, destination);

                try
                {
                    using (CryptoStream cs = new CryptoStream(ms, aes.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        // Write from the buffer to the memory stream starting after the salt value.
                        cs.Write(buffer, salt.Length, buffer.Length - salt.Length);
                    }

                }
                catch (CryptographicException)
                {
                    return CryptoResult.Failed;
                }
                

                byte[] decryptedBytes = ms.ToArray();

                using (FileStream fs = new FileStream(outputPath, FileMode.Create))
                {
                    fs.Write(decryptedBytes, 0, decryptedBytes.Length);
                }

                return CryptoResult.Complete;
                
            }

        }

        /// <summary>
        /// Creates an AES object for encryption and decryption.
        /// </summary>
        /// <param name="passwordBytes">The user input password converted to bytes.</param>
        /// <param name="salt">The salt read from an image file or from a file to be decrypted.</param>
        /// <returns></returns>
        private Aes CreateAes(byte[] passwordBytes, byte[] salt)
        {
            Aes aes = Aes.Create();
            aes.KeySize = 256;
            aes.BlockSize = 128;

            var key = new Rfc2898DeriveBytes(passwordBytes, salt, 1000);
            aes.Key = key.GetBytes(aes.KeySize / 8);
            aes.IV = key.GetBytes(aes.BlockSize / 8);
            aes.Padding = PaddingMode.PKCS7;
            aes.Mode = CipherMode.CBC;

            return aes;
        }

        /// <summary>
        /// Returns a concatenated string composed of the input file name and the target destination that is properly formatted
        /// for use as a destination path.
        /// </summary>
        /// <param name="inputFile">The file name.</param>
        /// <param name="destination">The path of the destination file.</param>
        /// <returns></returns>
        private string GetOutputPath(string inputFile, string destination)
        {
            // Get the filename.
            string[] tokens = inputFile.Split('\\');
            string encryptedFileName = tokens[tokens.Length - 1];

            // Trim the.aes extension off the file name.
            string decryptedFileName = "";
            for (int i = 0; i < encryptedFileName.Length - 4; i++)
                decryptedFileName += encryptedFileName[i];

            destination = destination.Replace("\\", "/") + "/";
            return destination + decryptedFileName;
        }

    }
}
