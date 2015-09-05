using System;
using System.Drawing;
using System.Drawing.Imaging;
using MathLibrary.Matrices;
using System.Linq;

namespace ImageHelpers
{
    public class JpegUtilitites
    {
        #region Fields

        private static readonly ArrayAccessorFactory<Double> BlockAccessorFactory = new ArrayAccessorFactory<double>(8);
        private static readonly ZigZagAccessorFactory<Double> ZigZagAccessorFactory = new ZigZagAccessorFactory<double>(8);        

        #endregion

        /// <summary>
        /// this method is available only for jpeg images
        /// </summary>
        /// <param name="jpegImage"> represents the jpegImage</param>
        /// <param name="quantizationTableType"></param>
        /// <returns></returns>
        public static Double[,] GetQuantizationTable(Bitmap jpegImage, QuantizationTableType quantizationTableType)
        {            
            switch (quantizationTableType)
            {
                case QuantizationTableType.Luminance:
                    //if (jpegImage.PropertyIdList.Contains(ImagePropertiesConstants.PropertyTagLuminanceTable))
                    //{
                    //    var luminanceProperty = jpegImage.GetPropertyItem(ImagePropertiesConstants.PropertyTagLuminanceTable);
                    //    var value = luminanceProperty.Value;
                    //    var ret = new DoubleMatrix(8);
                    //    var acccessor = ZigZagAccessorFactory.Access(ret);
                    //    for (int index = 0; index < 64; index++)
                    //    {
                    //        acccessor[index] = BitConverter.ToUInt16(value, index * 2);
                    //    }
                    //    return ret;
                    //}
                    return ImagePropertiesConstants.LuminanceQuantizationTable;
                case QuantizationTableType.Chrominance:
                    //if (jpegImage.PropertyIdList.Contains(ImagePropertiesConstants.PropertyTagChrominanceTable))
                    //{
                    //    var chrominanceProperty = jpegImage.GetPropertyItem(ImagePropertiesConstants.PropertyTagChrominanceTable);
                    //    var value = chrominanceProperty.Value;
                    //    var ret = new DoubleMatrix(8);
                    //    var acccessor = ZigZagAccessorFactory.Access(ret);
                    //    for (int index = 0; index < 64; index++)
                    //    {
                    //        acccessor[index] = BitConverter.ToUInt16(value, index * 2);
                    //    }
                    //    return ret;
                    //}
                    return ImagePropertiesConstants.ChrominanceQuantizationTable;
            }
            return null;
        }

        


        /// <summary>
        /// returns the jpeg quantization tables in an  short array with 64 items
        /// </summary>
        /// <param name="jpegImage"></param>
        /// <param name="quantizationTableType"></param>
        /// <returns></returns>
        public static short[] GetQuantizationTableArray(Bitmap jpegImage, QuantizationTableType quantizationTableType)
        {
            Accessor<double> accessor;            
            var ret = new short[64];
            var j = 0;
            switch (quantizationTableType)
            {
                case QuantizationTableType.Luminance:
                    //if (jpegImage.PropertyIdList.Contains(ImagePropertiesConstants.PropertyTagLuminanceTable))
                    //{
                    //    var luminanceProperty = jpegImage.GetPropertyItem(ImagePropertiesConstants.PropertyTagLuminanceTable);
                    //    var value = luminanceProperty.Value;
                    //    for (int index = 0; index < 64; index++)
                    //    {
                    //        ret[index] = BitConverter.ToInt16(value, index * 2);
                    //    }
                    //    return ret;
                    //}
                    accessor = ZigZagAccessorFactory.Access(ImagePropertiesConstants.LuminanceQuantizationTable);
                    for (int index = 0; index < 64; index++)
                    {
                        ret[j] = Convert.ToInt16(accessor[index]);
                    }
                    return ret;
                case QuantizationTableType.Chrominance:
                    //if (jpegImage.PropertyIdList.Contains(ImagePropertiesConstants.PropertyTagChrominanceTable))
                    //{
                    //    var chrominanceProperty = jpegImage.GetPropertyItem(ImagePropertiesConstants.PropertyTagChrominanceTable);
                    //    var value = chrominanceProperty.Value;
                    //    for (int index = 0; index < 64; index++)
                    //    {
                    //        ret[index] = BitConverter.ToInt16(value, index * 2);
                    //    }
                    //    return ret;
                    //}

                    accessor = ZigZagAccessorFactory.Access(ImagePropertiesConstants.LuminanceQuantizationTable);
                    for (int index = 0; index < 64; index++)
                    {
                        ret[j] = Convert.ToInt16(accessor[index]);
                    }

                    return ret;
            }

            return null;
        }


        /// <summary>
        /// save image in a jpeg format using specific luminance and chrominance quantization tables
        /// </summary>
        /// <param name="jpegImage"></param>
        /// <param name="destinationFile"></param>
        /// <param name="luminanceQuantizationTable"></param>
        /// <param name="chrominanceQuantizationTable"></param>
        public static void Save(Bitmap jpegImage, string destinationFile, short[] luminanceQuantizationTable, short[] chrominanceQuantizationTable)
        {
            var encoderParameters = new EncoderParameters(2);
            Encoder luminanceTableEncoder = Encoder.LuminanceTable;
            Encoder chrominanceTableEncoder = Encoder.ChrominanceTable;
            var luminance = new EncoderParameter(luminanceTableEncoder, luminanceQuantizationTable);
            var chrominance = new EncoderParameter(chrominanceTableEncoder, chrominanceQuantizationTable);
            
            encoderParameters.Param[0] = luminance;
            encoderParameters.Param[1] = chrominance;

            var jpegEncoderInfo = ImageHelpers.GetCodecInfo();

            jpegImage.Save(destinationFile, jpegEncoderInfo, encoderParameters);
        }



        /// <summary>
        /// Do Fast Cosine Transform,Quantization
        /// </summary>
        /// <param name="channel_data"></param>
        /// <param name="quant_table"></param>
        /// <returns></returns>
        private DoubleMatrix Do_FDCT_Quantization_And_ZigZag(DoubleMatrix channel_data, DoubleMatrix quant_table)
        {

            double tmp0, tmp1, tmp2, tmp3, tmp4, tmp5, tmp6, tmp7;
            double tmp10, tmp11, tmp12, tmp13;
            double z1, z2, z3, z4, z5, z11, z13;
            var tempData = channel_data.Copy;
            var outdata = new DoubleMatrix(8);
            Double temp;
            SByte ctr;            
            int k = 0;

            var accessor = BlockAccessorFactory.Access(tempData);
            

            /* Pass 1: process rows. */

            for (ctr = 7; ctr >= 0; ctr--)
            {
                tmp0 = accessor[0 + k] + accessor[7 + k];
                tmp7 = accessor[0 + k] - accessor[7 + k];
                tmp1 = accessor[1 + k] + accessor[6 + k];
                tmp6 = accessor[1 + k] - accessor[6 + k];
                tmp2 = accessor[2 + k] + accessor[5 + k];
                tmp5 = accessor[2 + k] - accessor[5 + k];
                tmp3 = accessor[3 + k] + accessor[4 + k];
                tmp4 = accessor[3 + k] - accessor[4 + k];

                /* Even part */

                tmp10 = tmp0 + tmp3;	/* phase 2 */
                tmp13 = tmp0 - tmp3;
                tmp11 = tmp1 + tmp2;
                tmp12 = tmp1 - tmp2;

                accessor[0 + k] = tmp10 + tmp11; /* phase 3 */
                accessor[4 + k] = tmp10 - tmp11;

                z1 = (tmp12 + tmp13) * ((float)0.707106781); /* c4 */
                accessor[2 + k] = tmp13 + z1;	/* phase 5 */
                accessor[6 + k] = tmp13 - z1;

                /* Odd part */

                tmp10 = tmp4 + tmp5;	/* phase 2 */
                tmp11 = tmp5 + tmp6;
                tmp12 = tmp6 + tmp7;

                /* The rotator is modified from fig 4-8 to avoid extra negations. */
                z5 = (tmp10 - tmp12) * ((float)0.382683433); /* c6 */
                z2 = ((float)0.541196100) * tmp10 + z5; /* c2-c6 */
                z4 = ((float)1.306562965) * tmp12 + z5; /* c2+c6 */
                z3 = tmp11 * ((float)0.707106781); /* c4 */

                z11 = tmp7 + z3;		/* phase 5 */
                z13 = tmp7 - z3;

                accessor[5 + k] = z13 + z2;	/* phase 6 */
                accessor[3 + k] = z13 - z2;
                accessor[1 + k] = z11 + z4;
                accessor[7 + k] = z11 - z4;

                k += 8;  /* advance pointer to next row */
            }

            /* Pass 2: process columns. */

            k = 0;

            for (ctr = 7; ctr >= 0; ctr--)
            {
                tmp0 = accessor[0 + k] + accessor[56 + k];
                tmp7 = accessor[0 + k] - accessor[56 + k];
                tmp1 = accessor[8 + k] + accessor[48 + k];
                tmp6 = accessor[8 + k] - accessor[48 + k];
                tmp2 = accessor[16 + k] + accessor[40 + k];
                tmp5 = accessor[16 + k] - accessor[40 + k];
                tmp3 = accessor[24 + k] + accessor[32 + k];
                tmp4 = accessor[24 + k] - accessor[32 + k];

                /* Even part */

                tmp10 = tmp0 + tmp3;	/* phase 2 */
                tmp13 = tmp0 - tmp3;
                tmp11 = tmp1 + tmp2;
                tmp12 = tmp1 - tmp2;

                accessor[0 + k] = tmp10 + tmp11; /* phase 3 */
                accessor[32 + k] = tmp10 - tmp11;

                z1 = (tmp12 + tmp13) * ((float)0.707106781); /* c4 */
                accessor[16 + k] = tmp13 + z1; /* phase 5 */
                accessor[48 + k] = tmp13 - z1;

                /* Odd part */

                tmp10 = tmp4 + tmp5;	/* phase 2 */
                tmp11 = tmp5 + tmp6;
                tmp12 = tmp6 + tmp7;

                /* The rotator is modified from fig 4-8 to avoid extra negations. */
                z5 = (tmp10 - tmp12) * ((float)0.382683433); /* c6 */
                z2 = ((float)0.541196100) * tmp10 + z5; /* c2-c6 */
                z4 = ((float)1.306562965) * tmp12 + z5; /* c2+c6 */
                z3 = tmp11 * ((float)0.707106781); /* c4 */

                z11 = tmp7 + z3;		/* phase 5 */
                z13 = tmp7 - z3;

                accessor[40 + k] = z13 + z2; /* phase 6 */
                accessor[24 + k] = z13 - z2;
                accessor[8 + k] = z11 + z4;
                accessor[56 + k] = z11 - z4;


                k++;   /* advance pointer to next column */
            }

             //Do Quantization, ZigZag and proper roundoff.
            //for (i = 0; i < 64; i++)
            //{
            //    temp = tempData[i] * quant_table[i];
            //    outdata[Tables.ZigZag[i]] = (Int16)((Int16)(temp + 16384.5) - 16384);
            //}

            return outdata;
        }       

    }
}
