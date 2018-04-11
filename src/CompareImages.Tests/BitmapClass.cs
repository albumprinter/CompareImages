using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CompareImages;

namespace CompareImages.Tests
{
    [TestClass]
    public class BitmapClass
    {
        private static byte[] Bitmap2ByteArray(Bitmap image)
        {
            Rectangle rect = new Rectangle(0, 0, image.Width, image.Height);

            var bitmapData = image.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadOnly, image.PixelFormat);
            var length = bitmapData.Stride * bitmapData.Height;

            byte[] bytes = new byte[length];

            Marshal.Copy(bitmapData.Scan0, bytes, 0, length);
            image.UnlockBits(bitmapData);

            return bytes;
        }

        [TestMethod]
        public void SameImages()
        {
            var img1 = Bitmap2ByteArray(new Bitmap(Path.Combine("Images", "1img1.jpg")));
            var img2 = Bitmap2ByteArray(new Bitmap(Path.Combine("Images", "1img1.jpg")));

            var distance = HistogramDistance.Calculate(img1, img2, false);

            Assert.AreEqual(distance, 0);
        }

        [TestMethod]
        public void SimilarImages()
        {
            var img1 = Bitmap2ByteArray(new Bitmap(Path.Combine("Images", "1img1.jpg")));
            var img2 = Bitmap2ByteArray(new Bitmap(Path.Combine("Images", "1img2.jpg")));

            var distance = HistogramDistance.Calculate(img1, img2, false);

            Assert.IsTrue(distance < 10000);
        }

        [TestMethod]
        public void DifferentImages()
        {
            var img1 = Bitmap2ByteArray(new Bitmap(Path.Combine("Images", "1img1.jpg")));
            var img2 = Bitmap2ByteArray(new Bitmap(Path.Combine("Images", "2img1.jpg")));

            var distance = HistogramDistance.Calculate(img1, img2, false);

            Assert.IsTrue(distance > 100000);
        }
    }
}
