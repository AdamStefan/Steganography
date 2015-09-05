using System;
using System.Drawing;

namespace ImageHelpers
{
    public static class ColorExtensions
    {
          /*Y_Data[i, j] = (Byte)(0.299 * R + 0.587 * G + 0.114 * B);
           Cb_Data[i, j] = (Byte)(-0.1687 * R - 0.3313 * G + 0.5 * B + 128);
           Cr_Data[i, j] = (Byte)(0.5 * R - 0.4187 * G - 0.0813 * B + 128);  */

        #region Extensions Methods

        public static double[] ToYCrCb(this Color color)
        {
            #region Version 1

            var ret = new Double[3];

            ret[0] = Math.Round(0.299 * color.R + 0.587 * color.G + 0.114 * color.B);
            ret[1] = Math.Round(-0.1687 * color.R - 0.3313 * color.G + 0.5 * color.B + 128);
            ret[2] = Math.Round(0.5 * color.R - 0.4187 * color.G - 0.0813 * color.B + 128);

           

            return ret;

            #endregion
        }

        public static Color ToRgb(this double[] yCrCbcolor)
        {
            #region Version 1

            var y = yCrCbcolor[0];
            var cb = yCrCbcolor[1];
            var cr = yCrCbcolor[2];

            var red = y + (1.4021*(cb - 128));

            if (red > 255)
            {
                red = 255;
                cb = ((255 - y)/1.4021) + 128.0;
            }
            if (red < 0)
            {
                red = 0;
                cb = 128 - (y/1.4021);
            }

            //1.0001   -0.3441   -0.7142
            var green = y + (-0.3441*(cr - 128)) - (0.7142*(cb - 128));

            bool maxCrSet = false;
            bool minCrSet = false;
            double mincr = 0.0;
            double maxcr = 0.0;

            if (green > 255)
            {
                maxcr = ((255 - y + (0.7132*(cb - 128)))/(-0.3441)) + 128;
                maxCrSet = true;
            }
            if (green < 0)
            {
                mincr = ((-y + (0.7132*(cb - 128)))/(-0.3441)) + 128;
                minCrSet = true;
            }

            var blue = y + (1.7720*(cr - 128));

            if (blue > 255)
            {
                double maxCr2 = ((255 - y)/1.7720) + 128;

                if (maxCrSet)
                {
                    cr = maxCr2 < maxcr ? maxCr2 : maxcr;

                }
                else if (minCrSet)
                {
                    cr = (mincr + maxCr2)/2;
                }
                else
                {
                    cr = maxCr2;
                }
            }

            //0.9999    0.0000    1.4021
            //1.0001   -0.3441   -0.7142
            //0.9998    1.7720   -0.0000

            if (blue < 0)
            {
                var minCr2 = ((-y)/1.7720) + 128;

                if (maxCrSet)
                {
                    cr = (minCr2 + maxcr)/2;

                }
                else if (minCrSet)
                {
                    cr = minCr2 < mincr ? mincr : minCr2;
                }
                else
                {
                    cr = minCr2;
                }

            }

            blue = y + (1.7720*(cr - 128));
            green = y + (-0.3441*(cr - 128)) - (0.7142*(cb - 128));


            var r = Math.Round(red);
            var g = Math.Round(green);
            var b = Math.Round(blue);

            #region Code

            r = r < 0 ? 0 : r > 255 ? 255 : r;
            g = g < 0 ? 0 : g > 255 ? 255 : g;
            b = b < 0 ? 0 : b > 255 ? 255 : b;

            #endregion

            var ret = Color.FromArgb(Convert.ToByte(r), Convert.ToByte(g), Convert.ToByte(b));
            return ret;


            #endregion
        }                

        #endregion
        
    }

}
