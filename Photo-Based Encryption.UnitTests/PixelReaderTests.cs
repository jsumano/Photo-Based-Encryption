using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Photo_Based_Encryption;
using System.Threading.Tasks;
using System.Linq;

namespace Photo_Based_Encryption.UnitTests
{
    [TestClass]
    public class PixelReaderTests
    {

        [TestMethod]
        public void MeetsColorThreshold_GrayscaleImage_ReturnsFalse()
        {
            bool result = false;

            result = PixelReader.MeetsColorThreshold(Properties.Resources.grayscale100x100, 100);

            Assert.AreEqual(false, result);
        }

        [TestMethod]
        public void MeetsColorThreshold_ColorCountBelowThreshold_ReturnsFalse()
        {
            bool result = false;

            result = PixelReader.MeetsColorThreshold(Properties.Resources.small, 100);

            Assert.AreEqual(false, result);
        }

        [TestMethod]
        public void MeetsColorThreshold_ColorAboveThreshold_ReturnsTrue()
        {
            bool result = false;

            result = PixelReader.MeetsColorThreshold(Properties.Resources.colorwheel100x100, 100);

            Assert.AreEqual(true, result);
        }

        [TestMethod]
        public void MeetsColorThreshold_ColorAtThreshold_ReturnsTrue()
        {
            bool result = false;

            result = PixelReader.MeetsColorThreshold(Properties.Resources.colors10x10, 100);

            Assert.AreEqual(true, result);
        }

        [TestMethod]
        public void GetRGB_Monochromatic_Returns102_255_79()
        {
            byte[] result = PixelReader.GetRGB(Properties.Resources.small, 0, 0);
            byte[] expected = new byte[] { 102, 255, 79 };

            Assert.IsTrue(expected.SequenceEqual(result)); 
        }

    }
}
