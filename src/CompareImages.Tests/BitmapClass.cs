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

        [DataTestMethod]
        [DataRow("1", "img1.jpg", "img1.jpg")]
        [DataRow("kitty", "img1.jpg", "img1.jpg")]
        [DataRow("nature", "img1.jpg", "img1.jpg")]
        public void SameImages(string imageSet, string image1Name, string image2Name)
        {
            using (var image1 = new Bitmap(Path.Combine("Images", imageSet, image1Name)))
            using (var image2 = new Bitmap(Path.Combine("Images", imageSet, image2Name)))
            {
                var img1 = Bitmap2ByteArray(image1);
                var img2 = Bitmap2ByteArray(image2);

                var distance = HistogramDistance.Calculate(img1, img2, false);
                Assert.AreEqual(distance, 0);

                distance = HistogramDistance.Calculate(img1, img2, true);
                Assert.AreEqual(distance, 0);
            }

        }

        [DataTestMethod]
        [DataRow("1", "img1.jpg", "img2.jpg")]
        [DataRow("kitty", "img1.jpg", "img2.jpg")]
        [DataRow("nature", "img1.jpg", "img2.jpg")]
        public void SimilarImages(string imageSet, string image1Name, string image2Name)
        {
            using (var image1 = new Bitmap(Path.Combine("Images", imageSet, image1Name)))
            using (var image2 = new Bitmap(Path.Combine("Images", imageSet, image2Name)))
            {
                var img1 = Bitmap2ByteArray(image1);
                var img2 = Bitmap2ByteArray(image2);

                var distance = HistogramDistance.Calculate(img1, img2, false);
                Assert.IsTrue(distance < 10000);

                distance = HistogramDistance.Calculate(img1, img2, true);
                Assert.IsTrue(distance < 0.01);
            }
        }

        [DataTestMethod]
        [DataRow("1", "img1.jpg", "diff.bmp")]
        [DataRow("kitty", "img1.jpg", "diff.bmp")]
        [DataRow("nature", "img1.jpg", "diff.bmp")]
        public void DifferentImages(string imageSet, string image1Name, string image2Name)
        {
            using (var image1 = new Bitmap(Path.Combine("Images", imageSet, image1Name)))
            using (var image2 = new Bitmap(Path.Combine("Images", imageSet, image2Name)))
            {
                var img1 = Bitmap2ByteArray(image1);
                var img2 = Bitmap2ByteArray(image2);

                var distance = HistogramDistance.Calculate(img1, img2, false);
                Assert.IsTrue(distance > 100000);

                distance = HistogramDistance.Calculate(img1, img2, true);
                Assert.IsTrue(distance > 0.1);
            }
        }
    }
}