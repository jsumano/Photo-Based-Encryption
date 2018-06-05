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
                        // Creates a string to parse.
                        string colorText = image.GetPixel(row, column).ToString();
                        // Splits colorText so that pixel values can be parsed more easily.
                        string[] tokens = colorText.Split(' ', '[', ']', ',', '=');
                        // Creates a new color using the RGB pixel values.
                        Color color = new Color();
                        color = Color.FromArgb(Convert.ToInt32(tokens[6]), Convert.ToInt32(tokens[9]), Convert.ToInt32(tokens[12]));

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
    }
}
