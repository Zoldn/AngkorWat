using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing.Imaging;
using System.Drawing;

namespace AngkorWat.Components
{
    internal class TargetImageLoader
    {
        public string FilePath { get; set; }
        public TargetImageLoader(string filePath)
        {
            FilePath = filePath;
        }

        public byte[] Run()
        {
            byte[] rgbValues = null;

            using (var imageIn = Image.FromFile(FilePath))
            using (var bmp = new Bitmap(imageIn))
            {
                // Lock the pixel data to gain low level access:
                BitmapData bmpData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadWrite, bmp.PixelFormat);

                // Get the address of the first line.
                IntPtr ptr = bmpData.Scan0;

                // Declare an array to hold the bytes of the bitmap.
                int bytes = Math.Abs(bmpData.Stride) * bmp.Height;
                rgbValues = new byte[bytes];

                // Copy the RGB values into the array.
                System.Runtime.InteropServices.Marshal.Copy(ptr, rgbValues, 0, bytes);

                // Unlock the pixel data:
                bmp.UnlockBits(bmpData);
            }

            return rgbValues;
        }
    }
}
