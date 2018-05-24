using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Photo_Based_Encryption
{
    public static class PixelReader
    {
        /// <summary>
        /// Returns a bool value indicating whether the image has enough different colors to
        /// be used as a seed image.
        /// </summary>
        /// <param name="image">The image to be counted.</param>
        /// <param name="threshold">The number of different color values needed.</param>
        /// <returns></returns>
        public static bool ColorCount(Bitmap image, int threshold)
        {
            List<Color> imageColors = new List<Color>();
            return false;
        }
    }
}
