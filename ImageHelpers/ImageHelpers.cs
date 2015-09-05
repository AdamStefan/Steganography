using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using MathLibrary.Matrices;

namespace ImageHelpers
{
    public static class ImageHelpers
    {
        #region ImageProcessing

        #region Image To  Array conversion

        public static List<double[,]> ImageToArray(string imageFile, PixelFormatType pixelFormatType)
        {
            var sourceimage = (Bitmap)Image.FromFile(imageFile);
            return ImageToArray(sourceimage, pixelFormatType);
        }

        //public static List<double[,]> ImageToArray(Bitmap input, PixelFormatType pixelFormatType)
        //{
        //    var height = input.Height; //rows
        //    var width = input.Width; // columns

        //    var redArray = new double[height, width];
        //    var greenArray = new double[height, width];
        //    var blueArray = new double[height, width];

        //    for (var i = 0; i < height; i++)
        //    {
        //        for (var j = 0; j < width; j++)
        //        {
        //            var pixel = input.GetPixel(j, i);
        //            switch (pixelFormatType)
        //            {
        //                case PixelFormatType.RGB:
        //                    redArray[i, j] = pixel.R;
        //                    greenArray[i, j] = pixel.G;
        //                    blueArray[i, j] = pixel.B;
        //                    break;
        //                case PixelFormatType.YCrCb:
        //                    var ycrb = pixel.ToYCrCb();
        //                    redArray[i, j] = ycrb[0];
        //                    greenArray[i, j] = ycrb[1];
        //                    blueArray[i, j] = ycrb[2];
        //                    break;
        //            }
        //        }
        //    }
        //    return new List<double[,]> { redArray, greenArray, blueArray };
        //}

        public static bool ArrayToImage(List<double[,]> input, string fileName, PixelFormatType currentPixelFormat, ImageFormat format = null)
        {
            var ret = ArrayToImage(input, currentPixelFormat);
            if (format != null)
            {
                ret.Save(fileName, format);
            }
            else
            {
                ret.Save(fileName);
            }
            return true;
        }

        public static List<double[,]> ImageToArray(Bitmap input, PixelFormatType pixelFomatType)
        {
            var height = input.Height; //rows
            var width = input.Width; // columns

            var redArray = new double[height, width];
            var greenArray = new double[height, width];
            var blueArray = new double[height, width];
         
            // Lock the bitmap's bits. 
            var rect = new Rectangle(0, 0, input.Width, input.Height);
            BitmapData bmpData =
              input.LockBits(rect, ImageLockMode.ReadOnly,
              input.PixelFormat);

            // Get the address of the first line.
            IntPtr ptr = bmpData.Scan0;

            // Declare an array to hold the bytes of the bitmap.
            int bytes = bmpData.Stride * input.Height;
            var rgbValues = new byte[bytes];

            //byte[] rgbValues = new byte[bytes];

            // Copy the RGB values into the array.
            System.Runtime.InteropServices.Marshal.Copy(ptr, rgbValues, 0, bytes);            

            int cnt = 0;
            for (int pos = 0; pos + 2 < rgbValues.Length; pos += 3)
            {

                var red = rgbValues[pos + 2];
                var green = rgbValues[pos + 1];
                var blue = rgbValues[pos];
                var pixel = Color.FromArgb(red, green, blue);

                var i = cnt / input.Width; // rowindex
                var j = cnt % input.Width; //columnIndex

                switch (pixelFomatType)
                {
                    case PixelFormatType.Rgb:
                        redArray[i, j] = pixel.R;
                        greenArray[i, j] = pixel.G;
                        blueArray[i, j] = pixel.B;
                        break;
                    case PixelFormatType.YCrCb:
                        var ycrb = pixel.ToYCrCb();
                        redArray[i, j] = ycrb[0];
                        greenArray[i, j] = ycrb[1];
                        blueArray[i, j] = ycrb[2];
                        break;
                }
                          
                cnt++;
            }
            input.UnlockBits(bmpData);

            return new List<double[,]> { redArray, greenArray, blueArray };
        }


        public static MatrixBase<Color> ImageToArray(Bitmap input)
        {
            var height = input.Height; //rows
            var width = input.Width; // columns            

            var ret = new MatrixBase<Color>(height, width);

            // Lock the bitmap's bits. 
            var rect = new Rectangle(0, 0, input.Width, input.Height);
            BitmapData bmpData =
                input.LockBits(rect, ImageLockMode.ReadOnly,
                               input.PixelFormat);

            // Get the address of the first line.
            IntPtr ptr = bmpData.Scan0;

            // Declare an array to hold the bytes of the bitmap.
            int bytes = bmpData.Stride*input.Height;
            var rgbValues = new byte[bytes];

            //byte[] rgbValues = new byte[bytes];

            // Copy the RGB values into the array.
            System.Runtime.InteropServices.Marshal.Copy(ptr, rgbValues, 0, bytes);

            var cnt = 0;
            for (var pos = 0; pos + 2 < rgbValues.Length; pos += 3)
            {

                var red = rgbValues[pos + 2];
                var green = rgbValues[pos + 1];
                var blue = rgbValues[pos];                

                var pixel = Color.FromArgb(red, green, blue);

                var i = cnt/input.Width; // rowindex
                var j = cnt%input.Width; //columnIndex

                ret[i, j] = pixel;

                cnt++;
            }
            input.UnlockBits(bmpData);

            return ret;
        }

        public static Bitmap ArrayToImage(List<double[,]> input, PixelFormatType currentPixelFormat)
        {
            var height = input[0].GetLength(0); //rows
            var width = input[0].GetLength(1); // columns

            var bitmap = new Bitmap(width, height);


            for (var i = 0; i < height; i++)
            {
                for (var j = 0; j < width; j++)
                {
                    var pixel = new Color();
                    switch (currentPixelFormat)
                    {
                        case PixelFormatType.Rgb:
                            pixel = Color.FromArgb(Convert.ToByte(input[0][i, j]), Convert.ToByte(input[1][i, j]), Convert.ToByte(input[2][i, j]));
                            break;
                        case PixelFormatType.YCrCb:
                            var ycrcbPixel = new[] { input[0][i, j], input[1][i, j], input[2][i, j] };
                            pixel = ycrcbPixel.ToRgb();
                            break;
                    }
                    bitmap.SetPixel(j, i, pixel);
                }
            }

            return bitmap;
        }


        public static Bitmap ArrayToImage(MatrixBase<Color> input)
        {
            var height = input.RowCount; //rows
            var width = input.ColumnCount; // columns

            var bitmap = new Bitmap(width, height, PixelFormat.Format24bppRgb);


            for (var i = 0; i < height; i++)
            {
                for (var j = 0; j < width; j++)
                {
                    bitmap.SetPixel(j, i, input[i,j]);
                }
            }

            return bitmap;
        }

        #endregion

        #region Convert Image to GrayScale

        public static Bitmap ConvertImageToGrayScale(Bitmap input)
        {
            var ret = new Bitmap(input);
            var height = input.Height; //rows
            var width = input.Width; // columns

            for (var i = 0; i < height; i++)
            {
                for (var j = 0; j < width; j++)
                {
                    var pixel = input.GetPixel(j, i);

                    var grayValue = (pixel.R + pixel.G + pixel.B) / 3;
                    ret.SetPixel(j, i, Color.FromArgb(grayValue, grayValue, grayValue));
                }
            }
            return ret;
        }

        public static void ConvertImageToGrayScale(string sourceFile, string destinationFile)
        {
            var sourceimage = (Bitmap)Image.FromFile(sourceFile);
            var ret = ConvertImageToGrayScale(sourceimage);
            ret.Save(destinationFile);
        }

        #endregion
                
        #endregion        

        #region CodecsProviders

        public static ImageCodecInfo GetCodecInfo(string mimeType = "image/jpeg")
        {
            var encoders = ImageCodecInfo.GetImageEncoders();

            return encoders.FirstOrDefault(t => t.MimeType == mimeType);
        }        

        #endregion

    }
}
