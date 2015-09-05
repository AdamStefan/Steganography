using System;
using System.Drawing;

namespace ImageHelpers
{
    public class YCbCrColor
    {
        #region Fields

        private Double _y;
        private Double _cb;
        private Double _cr;

        #endregion

        #region Instance

        public YCbCrColor(double[] color)
        {
            _y = color[0];
            _cb = color[1];
            _cr = color[2];
        }

        public YCbCrColor()
        {
           
        }

        #endregion

        #region Properties

        public Double Y
        {
            get
            {
                return _y;
            }
            set
            {
                _y = value;
            }
        }

        public Double Cb
        {
            get
            {
                return _cb;
            }
            set { _cb = value; }
        }

        public Double Cr
        {
            get
            {
                return _cr;
            }
            set
            {
                  _cr = value; 
            }
        }

        #endregion

        #region Methods

        public Color ToRgb()
        {
            #region Version 1

            var y = Y;
            var cr = Cb;
            var cb = Cr;

            var red = y + (1.4021 * (cb - 128));

            if (red > 255)
            {
                red = 255;
                cb = ((255 - y) / 1.4021) + 128.0;
            }
            if (red < 0)
            {
                red = 0;
                cb = 128 - (y / 1.4021);
            }

            //1.0001   -0.3441   -0.7142
            var green = y + (-0.3441 * (cr - 128)) - (0.7142 * (cb - 128));

            bool maxCrSet = false;
            bool minCrSet = false;
            double mincr = 0.0;
            double maxcr = 0.0;

            if (green > 255)
            {
                maxcr = ((255 - y + (0.7132 * (cb - 128))) / (-0.3441)) + 128;
                maxCrSet = true;
            }
            if (green < 0)
            {
                mincr = ((-y + (0.7132 * (cb - 128))) / (-0.3441)) + 128;
                minCrSet = true;
            }

            var blue = y + (1.7720 * (cr - 128));

            if (blue > 255)
            {
                double maxCr2 = ((255 - y) / 1.7720) + 128;

                if (maxCrSet)
                {
                    cr = maxCr2 < maxcr ? maxCr2 : maxcr;

                }
                else if (minCrSet)
                {
                    cr = (mincr + maxCr2) / 2;
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
                var minCr2 = ((-y) / 1.7720) + 128;

                if (maxCrSet)
                {
                    cr = (minCr2 + maxcr) / 2;

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

            blue = y + (1.7720 * (cr - 128));
            green = y + (-0.3441 * (cr - 128)) - (0.7142 * (cb - 128));


            var r = Math.Floor(red);
            var g = Math.Floor(green);
            var b = Math.Floor(blue);

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
