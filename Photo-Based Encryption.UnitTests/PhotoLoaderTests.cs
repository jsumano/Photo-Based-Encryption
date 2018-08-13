using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Photo_Based_Encryption;
using System.Threading.Tasks;

namespace Photo_Based_Encryption.UnitTests
{
    [TestClass]
    public class PhotoLoaderTests
    {
        [TestMethod]
        public async Task InspectAsync_ImageBelowMinimumSize_ReturnsPhotoResultFailedSize()
        {
            PhotoResult result = PhotoResult.Unchecked;

            result = await PhotoLoader.InspectAsync(Properties.Resources.small);

            Assert.AreEqual(PhotoResult.FailedSize, result);
        }

        [TestMethod]
        public async Task InspectAsync_ImageAboveMinimumSizeGrayscale_ReturnsPhotoResultFailedComplexity()
        {
            PhotoResult result = PhotoResult.Unchecked;

            result = await PhotoLoader.InspectAsync(Properties.Resources.grayscale100x100);

            Assert.AreEqual(PhotoResult.FailedComplexity, result);
        }

        [TestMethod]
        public async Task InspectAsync_ImageMeetsRequirements_ReturnsPhotoResultApproved()
        {
            PhotoResult result = PhotoResult.Unchecked;

            result = await PhotoLoader.InspectAsync(Properties.Resources.colorwheel100x100);

            Assert.AreEqual(PhotoResult.Approved, result);
        }
    }
}
