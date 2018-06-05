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
        /// Returns a bool value indicating whether the image has a sufficient number of different
        /// color values to be used as a seed image.
        /// </summary>
        /// <param name="image">The image to be counted.</param>
        /// <param name="threshold">The number of different color values needed.</param>
        /// <returns></returns>
        public static bool ColorCount(Bitmap image, int threshold)
        {
            List<Color> imageColors = new List<Color>();

                    
                // Iterate through all the pixels in the image until the threshold is reached.
                for (int row = 0; row < image.Width; row++)
                {
                    for (int column = 0; column < image.Height; column++)
                    {
                        // Create an array with the RGB values for the current pixel.
                        byte[] rgbValues = GetRGB(image, row, column);
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
        /// Returns an array of bytes with the RGB values for the specified pixel. 
        /// </summary>
        /// <param name="bitmap"></param>
        /// <param name="row"></param>
        /// <param name="column"></param>
        /// <returns></returns>
        private static byte[] GetRGB(Bitmap bitmap, int row, int column)
        {
            byte[] rgb = new byte[3];
            // Creates a string to parse.
            string colorText = bitmap.GetPixel(row, column).ToString();
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
