using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Photo_Based_Encryption;
using System.Threading.Tasks;
using System.Linq;
using System.Drawing;

namespace Photo_Based_Encryption.UnitTests
{
    [TestClass]
    public class PixelReaderTests
    {

        [TestMethod]
        public void MeetsColorThreshold_GrayscaleImage_ReturnsFalse()
        {
            bool result = PixelReader.MeetsColorThreshold(Properties.Resources.grayscale100x100, 100);

            Assert.AreEqual(false, result);
        }

        [TestMethod]
        public void MeetsColorThreshold_ColorCountBelowThreshold_ReturnsFalse()
        {
            bool result = PixelReader.MeetsColorThreshold(Properties.Resources.small, 100);

            Assert.AreEqual(false, result);
        }

        [TestMethod]
        public void MeetsColorThreshold_ColorAboveThreshold_ReturnsTrue()
        {
            bool result = PixelReader.MeetsColorThreshold(Properties.Resources.colorwheel100x100, 100);

            Assert.AreEqual(true, result);
        }

        [TestMethod]
        public void MeetsColorThreshold_ColorAtThreshold_ReturnsTrue()
        {
            bool result = PixelReader.MeetsColorThreshold(Properties.Resources.colors10x10, 100);

            Assert.AreEqual(true, result);
        }

        [TestMethod]
        public void GetRGB_RandomlyGeneratedPixel_ReturnsEqual()
        {
            for(int i = 0; i < 100; i++)
            {
                Bitmap bitmap = new Bitmap(Properties.Resources.blank1x1);
                Random rand = new Random();
                byte[] rgb = new byte[] { (byte)rand.Next(255), (byte)rand.Next(255), (byte)rand.Next(255) };
                bitmap.SetPixel(0, 0, Color.FromArgb(255, rgb[0], rgb[1], rgb[2]));
                byte[] result = PixelReader.GetRGB(bitmap, 0, 0);

                Assert.IsTrue(rgb.SequenceEqual(result));
            }
            
        }

    }
}
