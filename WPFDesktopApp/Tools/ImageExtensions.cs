using System;
using System.IO;
using System.Windows.Media.Imaging;

namespace WPFDesktopApp.Tools
{
    public static class ImageExtensions
    {
        public static BitmapImage? LoadImage(byte[] imageData)
        {
            if (imageData == null || imageData.Length == 0) { return null; }

            var image = new BitmapImage();
            using (var mem = new MemoryStream(imageData))
            {
                mem.Position = 0;
                image.BeginInit();
                image.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.UriSource = null;
                image.StreamSource = mem;
                image.EndInit();
            }
            image.Freeze();
            return image;
        }

        public static byte[] GetBytes(this BitmapImage image, BitmapEncoder encoder)
        {
            using (MemoryStream memStream = new MemoryStream())
            {
                encoder.Frames.Add(BitmapFrame.Create(image));
                encoder.Save(memStream);

                return memStream.ToArray();
            }
        }
    }
}