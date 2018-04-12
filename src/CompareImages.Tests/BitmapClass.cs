using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CompareImages.Tests
{
    [TestClass]
    public class BitmapClass
    {

        private static byte[] Bitmap2ByteArray(Bitmap image)
        {
            var rect = new Rectangle(0, 0, image.Width, image.Height);

            var bitmapData = image.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadOnly, image.PixelFormat);
            var length = bitmapData.Stride * bitmapData.Height;

            var bytes = new byte[length];

            Marshal.Copy(bitmapData.Scan0, bytes, 0, length);
            image.UnlockBits(bitmapData);

            return bytes;
        }

        [TestMethod]
        [DataTestMethod]
        [DataRow("1")]
        [DataRow("kitty")]
        [DataRow("nature")]
        public void SameImages(string imageSet)
        {
            var img1 = Bitmap2ByteArray(new Bitmap(Path.Combine("Images", imageSet, "img1.jpg")));
            var img2 = Bitmap2ByteArray(new Bitmap(Path.Combine("Images", imageSet, "img1.jpg")));

            var distance = HistogramDistance.Calculate(img1, img2, false);
            Assert.AreEqual(distance, 0);

            distance = HistogramDistance.Calculate(img1, img2, true);
            Assert.AreEqual(distance, 0);
        }

        [TestMethod]
        [DataTestMethod]
        [DataRow("1")]
        [DataRow("kitty")]
        [DataRow("nature")]
        public void SimilarImages(string imageSet)
        {
            var img1 = Bitmap2ByteArray(new Bitmap(Path.Combine("Images", imageSet, "img1.jpg")));
            var img2 = Bitmap2ByteArray(new Bitmap(Path.Combine("Images", imageSet, "img2.jpg")));

            var distance = HistogramDistance.Calculate(img1, img2, false);
            Assert.IsTrue(distance < 10000);

            distance = HistogramDistance.Calculate(img1, img2, true);
            Assert.IsTrue(distance < 0.01);
        }

        [TestMethod]
        [DataTestMethod]
        [DataRow("1")]
        [DataRow("kitty")]
        [DataRow("nature")]
        public void DifferentImages(string imageSet)
        {
            var img1 = Bitmap2ByteArray(new Bitmap(Path.Combine("Images", imageSet, "img1.jpg")));
            var img2 = Bitmap2ByteArray(new Bitmap(Path.Combine("Images", imageSet, "diff.bmp")));

            var distance = HistogramDistance.Calculate(img1, img2, false);
            Assert.IsTrue(distance > 100000);

            distance = HistogramDistance.Calculate(img1, img2, true);
            Assert.IsTrue(distance > 0.1);
        }
    }
}