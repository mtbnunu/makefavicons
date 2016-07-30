using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MakeFavicons
{
    public class PNGResizer
    {
        public PNGResizer(byte[] inputImage)
        {
            this.InputImage = inputImage;
            this.Cache = new Dictionary<int, byte[]>();
        }

        private byte[] InputImage { get; set; }
        private Dictionary<int,byte[]> Cache { get; set; }


        // Following code is referenced from 
        // http://www.codeproject.com/Tips/816996/Resize-PNG-Image-and-Keep-Transparent-Background

        internal byte[] ResizeImage(int size)
        {
            if (Cache.ContainsKey(size))
            {
                return Cache[size];
            }


            System.Drawing.Image img = MakeImage(this.InputImage);
            int width = Convert.ToInt32(Convert.ToDouble(img.Width) *
            (Convert.ToDouble(size) / Convert.ToDouble(img.Height)));
            if (size != 0)
            {
                if (width > size)
                {
                    size = Convert.ToInt32(Convert.ToDouble(img.Height) *
                    (Convert.ToDouble(size) / Convert.ToDouble(img.Width)));
                    width = size;
                }
            }
            System.Drawing.Size s = new System.Drawing.Size(width, size);
            System.Drawing.Image resizedImg = Resize(img, s);
            using (System.IO.MemoryStream memStream = new System.IO.MemoryStream())
            {
                if (System.Drawing.Imaging.ImageFormat.Png.Equals(img.RawFormat))
                {
                    resizedImg.Save(memStream, System.Drawing.Imaging.ImageFormat.Png);
                }
                else //of course you could check for bmp, jpg, etc depending on what you allowed
                {
                    resizedImg.Save(memStream, System.Drawing.Imaging.ImageFormat.Jpeg);
                }
                var result = memStream.ToArray();
                Cache[size] = result;
                return result;
            }
        }

        private System.Drawing.Image MakeImage(byte[] byteArrayIn)
        {
            System.IO.MemoryStream ms = new System.IO.MemoryStream(byteArrayIn);
            System.Drawing.Image returnImage = System.Drawing.Image.FromStream(ms);
            return returnImage;
        }

        private static System.Drawing.Image Resize(System.Drawing.Image image,
            System.Drawing.Size size)
        {
            System.Drawing.Image newImage = new System.Drawing.Bitmap(size.Width, size.Height);
            using (System.Drawing.Graphics graphicsHandle = System.Drawing.Graphics.FromImage(newImage))
            {
                graphicsHandle.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                graphicsHandle.InterpolationMode =
                           System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                graphicsHandle.DrawImage(image, 0, 0, size.Width, size.Height);
            }
            return newImage;
        }
    }
}
