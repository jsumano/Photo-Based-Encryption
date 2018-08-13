using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows;

namespace Photo_Based_Encryption
{
    /// <summary>
    /// Performs image analysis functions.
    /// </summary>
    public static class PixelReader
    {
        /// <summary>
        /// Returns a bool value indicating whether the image has met the color threshold.
        /// </summary>
        /// <param name="image">The image to be counted.</param>
        /// <param name="threshold">The number of different color values needed.</param>
        /// <returns></returns>
        public static bool MeetsColorThreshold(Bitmap image, int threshold)
        {
            // Create a list to store the color values.
            List<Color> imageColors = new List<Color>();
         
            // Iterate through all the pixels in the image until the threshold is reached.
            for (int row = 0; row < image.Height; row++)
            {
                for (int column = 0; column < image.Width; column++)
                {
                    // Create an array with the RGB values for the current pixel.
                    byte[] rgbValues = GetRGB(image, column, row);

                    // Skips the pixel if the RGB values are equal to filter out grayscale images.
                    if (rgbValues[0] == rgbValues[1] && rgbValues[0] == rgbValues[2])
                        continue;

                    // Creates a new color using the RGB pixel values.
                    Color color = new Color();
                    color = Color.FromArgb(rgbValues[0], rgbValues[1], rgbValues[2]);

                    // Adds this color to the list of different colors if it isn't contained within the collection.
                    if (!imageColors.Contains(color))
                        imageColors.Add(color);
                    // If the threshold is met the image passes for complexity.
                    if (imageColors.Count() == threshold)
                        return true;
                }
            }
            // The threshold was not met.
            return false;
        }

        /// <summary>
        /// Returns an array of random pixel values from an image.
        /// </summary>
        /// <param name="bitmap">The image to search for pixel values.</param>
        /// <returns></returns>
        public static byte[] GetPixelKey(Bitmap bitmap)
        {
            Random rand = new Random();
            // Initialize the key.
            byte[] key = new byte[32];

            // Fill the key with random pixel values.
            for(int i =0; i < key.Length; i++)
            {
                byte[] rgb = new byte[3];
                // Selects a new pixel if rgb values are equal to filter out grayscale.
                do
                {
                    // Selects a random row of pixels.
                    int row = rand.Next(bitmap.Height);
                    // Selects a random column of pixels.
                    int column = rand.Next(bitmap.Width);
                    // Returns the rgb values.
                    rgb = GetRGB(bitmap, column, row);
                }
                while (rgb[0] == rgb[1] && rgb[0] == rgb[2]);
                
                // Select the red green or blue value at random and add to the key.
                key[i] = rgb[rand.Next(3)];
            }

            return key;
        }

        /// <summary>
        /// Returns an array of bytes with the RGB values for the specified pixel. 
        /// </summary>
        /// <param name="bitmap">The image.</param>
        /// <param name="x">The x value of the pixel.</param>
        /// <param name="y">The y value of the pixel.</param>
        /// <returns></returns>
        private static byte[] GetRGB(Bitmap bitmap, int x, int y)
        {
            byte[] rgb = new byte[3];
            // Creates a string to parse.
            string colorText = bitmap.GetPixel(x, y).ToString();
            // Splits colorText so that pixel values can be parsed more easily.
            string[] tokens = colorText.Split(' ', '[', ']', ',', '=');
            // Assigns the RGB values.
            rgb[0] = Convert.ToByte(tokens[6]);
            rgb[1] = Convert.ToByte(tokens[9]);
            rgb[2] = Convert.ToByte(tokens[12]);
            return rgb;
        }

    }
}
