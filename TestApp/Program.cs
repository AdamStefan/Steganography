using System;
using MathLibrary;
using MathLibrary.Matrices;
using ImageHelpers;
using System.Drawing;
using System.IO;

namespace TestApp
{
    class Program
    {
        static void Main(string[] args)
        {            
            var ret = HelperExtensions.GenerateNoise(8, 8, 0, 255, 12312);

            #region TestMethods

            //TestBitOperation();
            //TestColorConversion();
            //TestMatrixOperations();
            //TestDct();
            //WatermarkWithText();

            #endregion


            var sourceFile = "d:\\testBinary1.jpg";

            var bytes = File.ReadAllBytes("d:\\text.rar");
            var destFile = "d:\\WatermarkedpozaTest22.jpg";

            WatermarkProcessor.WatermarkDctWithLsb(sourceFile, destFile, bytes, 0);
            var byteMessage = WatermarkProcessor.GetWaterMarkDct((Bitmap)Image.FromFile(destFile), 0);

            //var newBytes = new byte[bytes.Length];

            //for (int index = 0; index < bytes.Length; index++)
            //{
            //    newBytes[index] = byteMessage[index];
            //}

            for (int index = 0; index < bytes.Length; index++)
            {
                if (byteMessage[index] != bytes[index])
                {
                }
            }

            System.IO.File.WriteAllBytes("d:\\test2.rar", byteMessage);
            
        }

        #region TestMethods

        public static void TestBitOperation()
        {
            var bytes = System.Text.Encoding.ASCII.GetBytes("A");
            var values = new bool[8];
            for (int i = 0; i < 8; i++)
            {
                values[i] = MathUtilities.GetBit(bytes, i);
            }
        }

        private static void TestColorConversion()
        {
            Color color = Color.FromArgb(255, 0, 0);

            var Y = 0.299 * color.R + 0.587 * color.G + 0.114 * color.B;
            var Cb = 128 - 0.1687 * color.R - 0.3312 * color.G + 0.5 * color.B;
            var Cr = 128 + 0.5 * color.R - 0.4186 * color.G - 0.0813 * color.B;

            //y = (y * 219.0) + 16.0; //min value 16 maxvalue 235
            //cB = (cB * 224.0) + 128.0; //min value 16  max value 240
            //cR = (cR * 224.0) + 128.0; // minvalue 16 max value 240
            //var ycbcrColor = new Double[] { 16, 16, 16 };
            //var testcolor = ycbcrColor.ToRGB();

            var test = color.ToYCrCb();
            var reverse = test.ToRgb();
        }

        private static void TestMatrixOperations()
        {
            DoubleMatrix a = new Double[,] { { 6F, 5F },
                                             { 2F, 3F } };
            var ret4 = a * a.Inverse;            
            
        }

        public static void TestDct()
        {
            var asd = new Double[8, 8];

            for (int index = 0; index < 8; index++)
            {
                for (int jindex = 0; jindex < 8; jindex++)
                {
                    asd[index, jindex] = 10 * index + jindex;
                }
            }
            var ret = DiscreteCosineTransform.ForwardDct8Block(asd);
            var ret1 = DiscreteCosineTransform.InverseDct8Block(ret);
        }

        public static void WatermarkWithText()
        {
            //var sourceFile = "d:\\Test.jpg";
            //var sourceFile = "d:\\testQT.jpg";
            //var sourceFile = "d:\\test6.jpg";
            //var sourceFile = "d:\\test6.jpg";
            //var sourceFile = "e:\\PozaTest.jpg";
            var sourceFile = "d:\\testBinary.jpg";
                        

            var destFile = "e:\\WatermarkedpozaTest22.jpg";
            //var image = Image.FromFile(sourceFile);
            //image.SaveAdd(

            //var encoderParam = new System.Drawing.Imaging.EncoderParameters();
            //encoderParam.Param[0] = new System.Drawing.Imaging.EncoderParameter(new 

            //System.Drawing.Imaging.Encoder.ChrominanceTable


            //var code = @"The thermoelectric power was determined in three liquid ?l-S alloys (54; 50 and 45at.%Tl) at temperatures in?~uding the pre-frezing range. For each alloy a dual behavior was identified on The curve ?Seebeck coefficient versus temperature? (or better versus overheating of the melt above the liquidus point expressed as ?T=T-Tliq). In the high teeperature range the behavior was similar to the one  we have obtained previously for liquid alloys richer in thallium (57.1 and 64.7at.%Tl), namely decrease of the Seebeck coefficient as the temperature increases (a behavior specific for a one band semi";
            var code = @"The oldest definition of liquid semiconductors was given as early as 1960 and it considers this class of materials as 'electronically conducting liquids with electrical conductivities less than the range of liquid metals' [A.F.Ioffe and A.R.Regel, Progr. Semicond.4, 238(1960)]. The same authors suggest an upper limit for the electrical conductivity of liquid semiconductors set at a value ~104 ohm-1 cm-1 (the value for liquid mercury). However as pointed seventeen years later by Cutler [M.Cutler, Liquid semicoductors, Academic Press, New York (1977)] a correction is required in this defini";
            //var bytes = System.Text.Encoding.ASCII.GetBytes(code);
            //var text2 = System.Text.Encoding.ASCII.GetString(bytes);
            
            WatermarkProcessor.WatermarkDctWithLsb(sourceFile, destFile, code, 0);
            var text = WatermarkProcessor.GetTextWatermarkDct((Bitmap)Image.FromFile(destFile), 0);
            var asd = "The thermoelectric power was determined in three liquid ?l-S alloys (54; 50 and 45at.%Tl) at temperatures in?~uding the pre-frezing range. For each alloy a dual behavior was identified on The curve ?Seebeck coefficient versus temperature? (or better versus overheating of the melt above the liquidus point expressed as ?T=T-Tliq). In the high teeperature range the behavior was similar to the one  we have obtained previously for liquid alloys richer in thallium (57.1 and 64.7at.%Tl), namely decrease of the Seebeck coefficient as the temperature increases (a behavior specific for a one band semi";
        }

        #endregion
        
    }
}
