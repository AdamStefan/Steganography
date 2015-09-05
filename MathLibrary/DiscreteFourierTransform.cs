using System;
using System.Drawing;
using System.Numerics;

namespace MathLibrary
{
    public class DiscreteFourierTransform
    {       

        public static Complex[] Dft(Complex[] signal)
        {
            var signalLength = signal.Length;
            var ret = new Complex[signalLength];
            var phasePart = -2 * Math.PI / signalLength;

            for (int frequency = 0; frequency < signalLength; frequency++)
            {
                ret[frequency] = new Complex();
                var phasePartFrequency = phasePart * frequency;
                for (int step = 0; step < signal.Length; step++)
                {
                    var phase = phasePartFrequency *  step;
                    var temp = signal[step] * Complex.FromPolarCoordinates(1, phase);
                    ret[frequency] += temp;
                }
            }
            
            return ret;
        }

        private static Complex[] Fft(Complex[] signal)
        {
            return FftTransform(signal);
        }

        public static Complex[,] Fft(Complex[,] signal, bool completeInverse)
        {
            var rows = signal.GetLength(0);
            var columns = signal.GetLength(1);
            var ret = new Complex[rows, columns];
            var prod = 1 / Math.Sqrt(rows * columns);

            var phasePart = -2 * Math.PI;
            phasePart = completeInverse ? -phasePart : phasePart;

            for (int rowIndex = 0; rowIndex < rows; rowIndex++)
            {
                for (int columnIndex = 0; columnIndex < columns; columnIndex++)
                {
                    Complex sum = 0;
                    for (int rIndex = 0; rIndex < rows; rIndex++)
                    {
                        double rowsPart = rowIndex * rIndex / (double)rows;
                        for (int cIndex = 0; cIndex < columns; cIndex++)
                        {
                            double columnsPart = columnIndex * cIndex / (double)columns;
                            sum += signal[rIndex, cIndex] * Complex.FromPolarCoordinates(1, phasePart * (rowsPart + columnsPart));
                        }
                    }
                    ret[rowIndex, columnIndex] = sum * prod;
                }
            }

            return ret;
        }
        
        private static Complex[] FftTransform(Complex[] signal)
        {
            var signalLength = signal.Length;
            var ret = new Complex[signalLength];
            Complex[] evenSignalPart;
            if (signalLength == 1)
            {
                ret[0] = signal[0];
                return ret;
            }

            evenSignalPart = new Complex[signalLength / 2];
            Complex[] oddSignalPart = new Complex[signalLength / 2];

            for (var index = 0; index < signalLength / 2; index++)
            {
                evenSignalPart[index] = signal[2 * index];
                oddSignalPart[index] = signal[2 * index + 1];
            }
            var oddFft = FftTransform(oddSignalPart);
            var evenFft = FftTransform(evenSignalPart);

            var phaseConstant = -2 * Math.PI / signalLength;
            for (var index = 0; index < signalLength / 2; index++)
            {
                var temp = Complex.FromPolarCoordinates(1, phaseConstant * index);
                oddFft[index] *= temp;
                ret[index] = evenFft[index] + oddFft[index];
                ret[index + signalLength / 2] = evenFft[index] - oddFft[index];
            }

            return ret;
        }

        public Complex[, ,] ReadBitmap(Bitmap bitmap)
        {
            var ret = new Complex[bitmap.Height, bitmap.Width, 3];

            for (int row = 0; row < bitmap.Height; row++)
            {
                for (int column = 0; column < bitmap.Width; column++)
                {
                    Color pixelColor = bitmap.GetPixel(row, column);
                    ret[row, column, 0] = pixelColor.R;
                    ret[row, column, 1] = pixelColor.R;
                    ret[row, column, 2] = pixelColor.R;
                }
            }

            return ret;
        }

        public Complex[, ,] GenerateGaussianNoise(int height, int width)
        {
            var ret = new Complex[height, width, 3];
            var redRandom = new Random();
            var greenRandom = new Random();
            var blueRandom = new Random();
            double redMean = 0, greenMean = 0, blueMean = 0;


            for (int row = 0; row < height; row++)
            {
                for (int column = 0; column < width; column++)
                {
                    var nextRedRandom = redRandom.Next(0, 255);
                    var nextGreenRandom = redRandom.Next(0, 255);
                    var nextBlueRandom = redRandom.Next(0, 255);

                    redMean += nextRedRandom;
                    greenMean += nextGreenRandom;
                    blueMean += nextBlueRandom;

                    ret[row, column, 0] = nextRedRandom;
                    ret[row, column, 1] = nextGreenRandom;
                    ret[row, column, 2] = nextBlueRandom;
                }
            }
            var noOfCells = height * width;

            redMean = redMean / noOfCells;
            greenMean = greenMean / noOfCells;
            blueMean = blueMean / noOfCells;

            for (int row = 0; row < height; row++)
            {
                for (int column = 0; column < width; column++)
                {
                    ret[row, column, 0] -= redMean;
                    ret[row, column, 1] -= greenMean;
                    ret[row, column, 2] -= blueMean;
                }
            }

            return ret;
        }

        public double[,] EmbeddMessage(byte[] binaryMessage, double[,] pattern)
        {
            var rows = pattern.GetLength(0);
            var columns = pattern.GetLength(1);            

            var ret = new double[rows, columns];
            for (int row = 0; row < rows; row++)
            {
                for (int column = 0; column < columns; column++)
                {
                    var index = (row + 1) * (column + 1) % binaryMessage.Length;
                    var bitvalue = HelperExtensions.GetBit(binaryMessage, index) ? 1 : -1;
                    ret[row, column] = pattern[row, column] * bitvalue;
                }
            }

            return ret;
        }

        public static double[,] CosinusTransfrom(double[,] signal)
        {
            var rows = signal.GetLength(0);
            var columns = signal.GetLength(1);
            var ret = new double[rows, columns];
            var prodCoef = 2 / (Math.Sqrt(rows * columns));
            var rowCosConstant = Math.PI / (2 * rows);
            var columnCosConstant = Math.PI / (2 * columns);

            double tempValue = 1 / Math.Sqrt(2.0);

            for (int rowIndex = 0; rowIndex < rows; rowIndex++)
            {
                for (int columnIndex = 0; columnIndex < columns; columnIndex++)
                {
                    ret[rowIndex, columnIndex] = 0;

                    #region Determination

                    for (int i = 0; i < rows; i++)
                    {
                        for (int j = 0; j < columns; j++)
                        {
                            double sigmaiProd = (i == 0 ? tempValue : 1.0) * (j == 0 ? tempValue : 1.0);

                            var rowsCos = Math.Cos(rowCosConstant * rowIndex * (2 * i + 1));
                            var columnCos = Math.Cos(columnCosConstant * columnIndex * (2 * j + 1));

                            ret[rowIndex, columnIndex] += sigmaiProd * signal[i, j] * rowsCos * columnCos;
                        }
                    }

                    ret[rowIndex, columnIndex] = prodCoef * ret[rowIndex, columnIndex];

                    #endregion
                }
            }

            return ret;
        }      

        public double ComputePerceptibility(double[,] image, double[,] codedWave)
        {
            var rows = image.GetLength(0);
            var columns = image.GetLength(1);

            if (rows != codedWave.GetLength(0) || columns != codedWave.GetLength(1))
            {
                throw new Exception("Input arrays have different size");
            }

            var ret = (image.Length - HelperExtensions.FullCorrelation(image, codedWave)) / HelperExtensions.ShortCorrelation(image, codedWave);

            return ret;
        }

        public void EmbedWaterMark(string message, Bitmap bitmap, double alpha)
        {
            var height = bitmap.Height;
            var width = bitmap.Width;
            var binaryMessage = System.Text.ASCIIEncoding.ASCII.GetBytes(message);
            var encoderPattern = HelperExtensions.GenerateNoise(bitmap.Height, bitmap.Width, 0, 255, 12342);
            var embeddedMessage = EmbeddMessage(binaryMessage, encoderPattern);

            for (int row = 0; row < height; row++)
            {
                for (int column = 0; column < width; column++)
                {
                    var currentPixel = bitmap.GetPixel(row, column);
                    var currentBit= (row * column) % binaryMessage.Length;
                    //if (binaryMessage[cu])
                    //{
                    //    currentPixel.R = alpha 
                    //}
                    
 
                    ///ret[row, column, 1] -= greenMean;
                    //ret[row, column, 2] -= blueMean;
                }
            } 
        }
              
    }

    public static class HelperExtensions
    {
        public static bool GetBit(this byte[] bytes, int index)
        {
            byte block = bytes[index / 8];
            var bitIndex = index % 8;
            var temp = block >> bitIndex;
            return temp % 2 == 1;
        }

        public static int Modulo(this int current, int value)
        {
            var ret = current % value;
            return ret > 0 ? ret : ret + value;
        }


        //public static double[,] Add(this double[,] input, double value)
        //{
        //    var rows = input.GetLength(0);
        //    var columns = input.GetLength(1);

        //    for (int rowIndex = 0; rowIndex < rows; rowIndex++)
        //    {
        //        for (int columnIndex = 0; columnIndex < columns; columnIndex++)
        //        {

        //            input[rowIndex, columnIndex] = input[rowIndex, columnIndex] + value;
        //        }
        //    }

        //    return input;
        //}

        public static double[,] Add(this double[,] input, double[,] value)
        {
            var rows = input.GetLength(0);
            var columns = input.GetLength(1);

            for (int rowIndex = 0; rowIndex < rows; rowIndex++)
            {
                for (int columnIndex = 0; columnIndex < columns; columnIndex++)
                {

                    input[rowIndex, columnIndex] = input[rowIndex, columnIndex] + value[rowIndex,columnIndex];
                }
            }

            return input;
        }

        public static double[,] Multiply(this double[,] input, double value)
        {
            var rows = input.GetLength(0);
            var columns = input.GetLength(1);

            for (int rowIndex = 0; rowIndex < rows; rowIndex++)
            {
                for (int columnIndex = 0; columnIndex < columns; columnIndex++)
                {

                    input[rowIndex, columnIndex] = input[rowIndex, columnIndex] * value;
                }
            }

            return input;
        }

        public static double StandardDeviation(double[,] input)
        {
            double ret = 0;
            double mean = 0;
            var rows = input.GetLength(0);
            var columns = input.GetLength(1);

            for (int rowIndex = 0; rowIndex < rows; rowIndex++)
            {
                for (int columnIndex = 0; columnIndex < columns; columnIndex++)
                {

                    mean += input[rowIndex, columnIndex];
                }
            }

            mean = mean / (rows * columns);

            for (int rowIndex = 0; rowIndex < rows; rowIndex++)
            {
                for (int columnIndex = 0; columnIndex < columns; columnIndex++)
                {
                    var temp = input[rowIndex, columnIndex] - mean;
                    ret += temp * temp;
                }
            }
            return Math.Sqrt(ret / mean);
        }

        public static double ShortCorrelation(double[,] firstInput, double[,] secondInput)
        {
            double ret = 0;

            var rows = firstInput.GetLength(0);
            var columns = firstInput.GetLength(1);

            if (rows != secondInput.GetLength(0) || columns != secondInput.GetLength(1))
            {
                throw new Exception("Input arrays have different size");
            }

            for (int rowIndex = 0; rowIndex < rows; rowIndex++)
            {
                for (int columnIndex = 0; columnIndex < columns; columnIndex++)
                {
                    ret = firstInput[rowIndex, columnIndex] * secondInput[rowIndex, columnIndex];
                }
            }

            return ret / firstInput.Length;
        }


        public static double FullCorrelation(double[,] firstInput, double[,] secondInput)
        {
            double ret = 0;

            var rows = firstInput.GetLength(0);
            var columns = firstInput.GetLength(1);

            if (rows != secondInput.GetLength(0) || columns != secondInput.GetLength(1))
            {
                throw new Exception("Input arrays have different size");
            }
            var length = firstInput.Length;

            double productValue = 0;
            double sumInputValue = 0;
            double sumSquareInputValue = 0;
            double sumSecondInputValue = 0;
            double sumSquareSecondInputValue = 0;

            for (int rowIndex = 0; rowIndex < rows; rowIndex++)
            {
                for (int columnIndex = 0; columnIndex < columns; columnIndex++)
                {
                    productValue += firstInput[rowIndex, columnIndex] * secondInput[rowIndex, columnIndex];

                    sumInputValue += firstInput[rowIndex, columnIndex];
                    sumSquareInputValue += (firstInput[rowIndex, columnIndex] * firstInput[rowIndex, columnIndex]);

                    sumSecondInputValue += secondInput[rowIndex, columnIndex];
                    sumSquareSecondInputValue += (secondInput[rowIndex, columnIndex] * secondInput[rowIndex, columnIndex]);
                }
            }

            var nominator = (length * productValue) - (sumInputValue * sumSecondInputValue);
            var denominatorLeft = Math.Sqrt((length * sumSquareInputValue) - (sumInputValue * sumInputValue));
            var denominatorRight = Math.Sqrt((length * sumSquareSecondInputValue) - (sumSecondInputValue * sumSecondInputValue));

            ret = nominator / (denominatorLeft * denominatorRight);

            return ret;
        }

        public static double[,] GenerateNoise(int rows, int columns, int minValue, int maxValue, int seed)
        {
            var ret = new double[rows, columns];
            Random randomGenerator = seed == 0 ? new Random() : new Random(seed);
            double mean = 0;
            var cellsNo = rows * columns;

            for (int rowIndex = 0; rowIndex < rows; rowIndex++)
            {
                for (int columnIndex = 0; columnIndex < columns; columnIndex++)
                {
                    var temp = randomGenerator.Next(minValue, maxValue);
                    mean = mean + temp;
                    ret[columnIndex, rowIndex] = temp;
                }
            }

            //mean = mean / (cellsNo);

            //double std = 0;

            //for (int rowIndex = 0; rowIndex < rows; rowIndex++)
            //{
            //    for (int columnIndex = 0; columnIndex < columns; columnIndex++)
            //    {
            //        ret[columnIndex, rowIndex] = ret[columnIndex, rowIndex] - mean;
            //        std += ret[columnIndex, rowIndex] * ret[columnIndex, rowIndex];
            //    }
            //}

            //std = Math.Sqrt(std / (cellsNo));


            //for (int rowIndex = 0; rowIndex < rows; rowIndex++)
            //{
            //    for (int columnIndex = 0; columnIndex < columns; columnIndex++)
            //    {
            //        ret[columnIndex, rowIndex] = ret[columnIndex, rowIndex] / std;
            //    }
            //}

            return ret;
        }
    }


    ///// <summary>
    ///// Implementation of 2D Discrete Cosine Transform
    ///// </summary>
    //public static class DiscreteCosineTransform2D
    //{
    //    /// <summary>
    //    /// Initialize alpha coefficient array 
    //    /// </summary>
    //    private static Double[,] InitCoefficientsMatrix(int dim)
    //    {
    //        Double[,] coefficientsMatrix = new double[dim, dim];

    //        for (int i = 0; i < dim; i++)
    //        {
    //            coefficientsMatrix[i, 0] = System.Math.Sqrt(2.0) / dim;
    //            coefficientsMatrix[0, i] = System.Math.Sqrt(2.0) / dim;
    //        }

    //        coefficientsMatrix[0, 0] = 1.0 / dim;

    //        for (int i = 1; i < dim; i++)
    //        {
    //            for (int j = 1; j < dim; j++)
    //            {
    //                coefficientsMatrix[i, j] = 2.0 / dim;
    //            }
    //        }
    //        return coefficientsMatrix;
    //    }
    //    private static bool IsQuadricMatrix<T>(T[,] matrix)
    //    {
    //        int columnsCount = matrix.GetLength(0);
    //        int rowsCount = matrix.GetLength(1);
    //        return (columnsCount == rowsCount);
    //    }
    //    public static Double[,] ForwardDCT(Double[,] input)
    //    {
    //        double sqrtOfLength = System.Math.Sqrt(input.Length);

    //        if (DiscreteCosineTransform2D.IsQuadricMatrix<Double>(input) == false)
    //        {
    //            throw new ArgumentException("Matrix must be quadric");
    //        }

    //        int N = input.GetLength(0);

    //        double[,] coefficientsMatrix = InitCoefficientsMatrix(N);
    //        Double[,] output = new Double[N, N];

    //        for (int u = 0; u <= N - 1; u++)
    //        {
    //            for (int v = 0; v <= N - 1; v++)
    //            {
    //                double sum = 0.0;
    //                for (int x = 0; x <= N - 1; x++)
    //                {
    //                    for (int y = 0; y <= N - 1; y++)
    //                    {
    //                        sum += input[x, y] * System.Math.Cos(((2.0 * x + 1.0) / (2.0 * N)) * u * System.Math.PI) * System.Math.Cos(((2.0 * y + 1.0) / (2.0 * N)) * v * System.Math.PI);
    //                    }
    //                }
    //                sum *= coefficientsMatrix[u, v];
    //                output[u, v] = System.Math.Round(sum);
    //            }
    //        }
    //        return output;
    //    }
    //    public static Double[,] InverseDCT(Double[,] input)
    //    {
    //        double sqrtOfLength = System.Math.Sqrt(input.Length);

    //        if (DiscreteCosineTransform2D.IsQuadricMatrix<Double>(input) == false)
    //        {
    //            throw new ArgumentException("Matrix must be quadric");
    //        }

    //        int N = input.GetLength(0);

    //        Double[,] coefficientsMatrix = InitCoefficientsMatrix(N);
    //        Double[,] output = new Double[N, N];

    //        for (int x = 0; x <= N - 1; x++)
    //        {
    //            for (int y = 0; y <= N - 1; y++)
    //            {
    //                double sum = 0.0;
    //                for (int u = 0; u <= N - 1; u++)
    //                {
    //                    for (int v = 0; v <= N - 1; v++)
    //                    {
    //                        sum += coefficientsMatrix[u, v] * input[u, v] * System.Math.Cos(((2.0 * x + 1.0) / (2.0 * N)) * u * System.Math.PI) * System.Math.Cos(((2.0 * y + 1.0) / (2.0 * N)) * v * System.Math.PI);
    //                    }
    //                };
    //                output[x, y] = System.Math.Round(sum);
    //            }
    //        }
    //        return output;
    //    }

    //}
    //public class FastDCT2D
    //{
    //    public Bitmap Obj;                   // Input Object Image
    //    public Bitmap DCTMap;                //Colour DCT Map
    //    public Bitmap IDCTImage;

    //    public int[,] GreyImage;             //GreyScale Image Array Generated from input Image
    //    public double[,] Input;              //Greyscale Image in Double Format

    //    public double[,] DCTCoefficients;
    //    public double[,] IDTCoefficients;
    //    private double[,] DCTkernel;        // DCT Kernel to find Transform Coefficients

    //    int Width, Height;
    //    int Order;
    //    /// <summary>
    //    /// Parameterized Constructor for FFT Reads Input Bitmap to a Greyscale Array
    //    /// </summary>
    //    /// <param name="Input">Input Image</param>
    //    public FastDCT2D(Bitmap Input, int DCTOrder)
    //    {
    //        Obj = Input;
    //        Width = Input.Width;
    //        Height = Input.Height;
    //        Order = DCTOrder;
    //        ReadImage();
    //    }
    //    /// <summary>
    //    /// Parameterized Constructor for FFT
    //    /// </summary>
    //    /// <param name="Input">Greyscale Array</param>
    //    public FastDCT2D(int[,] InputImageData, int order)
    //    {
    //        int i, j;
    //        GreyImage = InputImageData;
    //        Width = InputImageData.GetLength(0);
    //        Height = InputImageData.GetLength(1);
    //        for (i = 0; i <= Width - 1; i++)
    //            for (j = 0; j <= Height - 1; j++)
    //            {
    //                Input[i, j] = (Double)(InputImageData[i, j]);
    //            }
    //        Order = order;//Order of Inverse 2D DCT
    //    }
    //    /// <summary>
    //    /// Constructor For Inverse DCT
    //    /// </summary>
    //    /// <param name="InputData"></param>
    //    public FastDCT2D(double[,] DCTCoeffInput)
    //    {

    //        DCTCoefficients = DCTCoeffInput;
    //        Width = DCTCoeffInput.GetLength(0);
    //        Height = DCTCoeffInput.GetLength(1);

    //    }
    //    /// <summary>
    //    /// Read Bitmap Image to 2D Integer Grey Levels Array for Proccessing
    //    /// </summary>
    //    private void ReadImage()
    //    {
    //        int i, j;
    //        GreyImage = new int[Width, Height];  //[Row,Column]
    //        Input = new double[Width, Height];  //[Row,Column]
    //        Bitmap image = Obj;
    //        BitmapData bitmapData1 = image.LockBits(new Rectangle(0, 0, image.Width, image.Height),
    //                                 ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
    //        unsafe
    //        {
    //            byte* imagePointer1 = (byte*)bitmapData1.Scan0;

    //            for (i = 0; i < bitmapData1.Height; i++)
    //            {
    //                for (j = 0; j < bitmapData1.Width; j++)
    //                {
    //                    GreyImage[j, i] = (int)((imagePointer1[0] + imagePointer1[1] + imagePointer1[2]) / 3.0);
    //                    Input[j, i] = (double)GreyImage[j, i];
    //                    //4 bytes per pixel
    //                    imagePointer1 += 4;
    //                }//end for j
    //                //4 bytes per pixel
    //                imagePointer1 += bitmapData1.Stride - (bitmapData1.Width * 4);
    //            }//end for i
    //        }//end unsafe
    //        image.UnlockBits(bitmapData1);
    //        return;
    //    }
    //    /// <summary>
    //    /// Display Subroutine for Inverse DCT Image
    //    /// </summary>
    //    /// <param name="image">IDCT Coefficients Array</param>
    //    /// <returns>Bitmap for DCT Inverse</returns>
    //    public Bitmap Displayimage(double[,] image)
    //    {
    //        int i, j;
    //        Bitmap output = new Bitmap(image.GetLength(0), image.GetLength(1));
    //        BitmapData bitmapData1 = output.LockBits(new Rectangle(0, 0, image.GetLength(0), image.GetLength(1)),
    //                                 ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
    //        unsafe
    //        {
    //            byte* imagePointer1 = (byte*)bitmapData1.Scan0;
    //            for (i = 0; i < bitmapData1.Height; i++)
    //            {
    //                for (j = 0; j < bitmapData1.Width; j++)
    //                {
    //                    imagePointer1[0] = (byte)image[j, i];
    //                    imagePointer1[1] = (byte)image[j, i];
    //                    imagePointer1[2] = (byte)image[j, i];
    //                    imagePointer1[3] = 255;
    //                    //4 bytes per pixel
    //                    imagePointer1 += 4;
    //                }//end for j
    //                //4 bytes per pixel
    //                imagePointer1 += (bitmapData1.Stride - (bitmapData1.Width * 4));
    //            }//end for i
    //        }//end unsafe
    //        output.UnlockBits(bitmapData1);
    //        return output;// col;

    //    }
    //    public Bitmap Displaymap(int[,] output)
    //    {
    //        int i, j;
    //        Bitmap image = new Bitmap(output.GetLength(0), output.GetLength(1));
    //        BitmapData bitmapData1 = image.LockBits(new Rectangle(0, 0, output.GetLength(0), output.GetLength(1)),
    //                                 ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
    //        unsafe
    //        {
    //            byte* imagePointer1 = (byte*)bitmapData1.Scan0;
    //            for (i = 0; i < bitmapData1.Height; i++)
    //            {
    //                for (j = 0; j < bitmapData1.Width; j++)
    //                {
    //                    if (output[j, i] < 0)
    //                    {
    //                        // Changing to Red Color
    //                        // Changing to Green Color
    //                        imagePointer1[0] = 0; //(byte)output[j, i];
    //                        imagePointer1[1] = 255;
    //                        imagePointer1[2] = 0; //(byte)output[j, i];
    //                    }
    //                    else if ((output[j, i] >= 0) && (output[j, i] < 50))
    //                    {   // Changing to Green Color
    //                        imagePointer1[0] = (byte)((output[j, i]) * 4);  //(byte)output[j, i];
    //                        imagePointer1[1] = 0;
    //                        imagePointer1[2] = 0;// 0; //(byte)output[j, i];
    //                    }
    //                    else if ((output[j, i] >= 50) && (output[j, i] < 100))
    //                    {
    //                        imagePointer1[0] = 0;//(byte)(-output[j, i]);
    //                        imagePointer1[1] = (byte)(output[j, i] * 2);// (byte)(output[j, i]);
    //                        imagePointer1[2] = (byte)(output[j, i] * 2);
    //                    }
    //                    else if ((output[j, i] >= 100) && (output[j, i] < 255))
    //                    {   // Changing to Green Color
    //                        imagePointer1[0] = 0; //(byte)output[j, i];
    //                        imagePointer1[1] = (byte)(output[j, i]);// (byte)(output[j, i]);
    //                        imagePointer1[2] = 0;  //(byte)output[j, i];
    //                    }
    //                    else if ((output[j, i] > 255))
    //                    {   // Changing to Green Color
    //                        imagePointer1[0] = 0;  //(byte)output[j, i];
    //                        imagePointer1[1] = 0; //(byte)(output[j, i]);
    //                        imagePointer1[2] = (byte)((output[j, i]) * 0.7);
    //                    }
    //                    imagePointer1[3] = 255;
    //                    //4 bytes per pixel
    //                    imagePointer1 += 4;
    //                }//end for j
    //                //4 bytes per pixel
    //                imagePointer1 += (bitmapData1.Stride - (bitmapData1.Width * 4));
    //            }//end for i
    //        }//end unsafe
    //        image.UnlockBits(bitmapData1);
    //        return image;// col;

    //    }
    //    /// <summary>
    //    /// Fast 2D DCT of the Image Array
    //    /// </summary>
    //    /// <param name="input"></param>
    //    public void FastDCT()
    //    {
    //        double[,] temp = new double[Width, Height];
    //        DCTCoefficients = new double[Width, Height];
    //        DCTkernel = new double[Width, Height];
    //        DCTkernel = GenerateDCTmatrix(Order);
    //        temp = multiply(DCTkernel, Input);
    //        DCTCoefficients = multiply(temp, Transpose(DCTkernel));
    //        DCTPlotGenerate();
    //        return;
    //    }
    //    public void FastInverseDCT()
    //    {
    //        double[,] temp = new double[Width, Height];
    //        IDTCoefficients = new double[Width, Height];
    //        DCTkernel = new double[Width, Height];
    //        DCTkernel = Transpose(GenerateDCTmatrix(Order));
    //        temp = multiply(DCTkernel, DCTCoefficients);
    //        IDTCoefficients = multiply(temp, Transpose(DCTkernel));
    //        IDCTImage = Displayimage(IDTCoefficients);
    //        return;
    //    }
    //    /// <summary>
    //    /// Generates DCT Matrix for Given Order 
    //    /// </summary>
    //    /// <param name="order">Dimension of the matrix</param>
    //    /// <returns>DCT Kernel for given Order</returns>
    //    public double[,] GenerateDCTmatrix(int order)
    //    {
    //        int i, j;
    //        int N;
    //        N = order;
    //        double alpha;
    //        double denominator;
    //        double[,] DCTCoeff = new double[N, N];
    //        for (j = 0; j <= N - 1; j++)
    //        {
    //            DCTCoeff[0, j] = Math.Sqrt(1 / (double)N);
    //        }
    //        alpha = Math.Sqrt(2 / (double)N);
    //        denominator = (double)2 * N;
    //        for (j = 0; j <= N - 1; j++)
    //            for (i = 1; i <= N - 1; i++)
    //            {
    //                DCTCoeff[i, j] = alpha * Math.Cos(((2 * j + 1) * i * 3.14159) / denominator);
    //            }

    //        return (DCTCoeff);
    //    }
    //    private double[,] multiply(double[,] m1, double[,] m2)
    //    {
    //        int row, col, i, j, k;
    //        row = col = m1.GetLength(0);
    //        double[,] m3 = new double[row, col];
    //        double sum;
    //        for (i = 0; i <= row - 1; i++)
    //        {
    //            for (j = 0; j <= col - 1; j++)
    //            {
    //                //Application.DoEvents();
    //                sum = 0;
    //                for (k = 0; k <= row - 1; k++)
    //                {
    //                    sum = sum + m1[i, k] * m2[k, j];
    //                }
    //                m3[i, j] = sum;
    //            }
    //        }
    //        return m3;
    //    }
    //    private double[,] Transpose(double[,] m)
    //    {
    //        int i, j;
    //        int Width, Height;
    //        Width = m.GetLength(0);
    //        Height = m.GetLength(1);

    //        double[,] mt = new double[m.GetLength(0), m.GetLength(1)];

    //        for (i = 0; i <= Height - 1; i++)
    //            for (j = 0; j <= Width - 1; j++)
    //            {
    //                mt[j, i] = m[i, j];
    //            }
    //        return (mt);
    //    }
    //    private void DCTPlotGenerate()
    //    {
    //        int i, j;
    //        int[,] temp = new int[Width, Height];
    //        double[,] DCTLog = new double[Width, Height];

    //        // Compressing Range By taking Log    
    //        for (i = 0; i <= Width - 1; i++)
    //            for (j = 0; j <= Height - 1; j++)
    //            {
    //                DCTLog[i, j] = Math.Log(1 + Math.Abs((int)DCTCoefficients[i, j]));

    //            }

    //        //Normalizing Array
    //        double min, max;
    //        min = max = DCTLog[1, 1];

    //        for (i = 1; i <= Width - 1; i++)
    //            for (j = 1; j <= Height - 1; j++)
    //            {
    //                if (DCTLog[i, j] > max)
    //                    max = DCTLog[i, j];
    //                if (DCTLog[i, j] < min)
    //                    min = DCTLog[i, j];
    //            }
    //        for (i = 0; i <= Width - 1; i++)
    //            for (j = 0; j <= Height - 1; j++)
    //            {
    //                temp[i, j] = (int)(((float)(DCTLog[i, j] - min) / (float)(max - min)) * 750);
    //            }

    //        DCTMap = Displaymap(temp);
    //    }
    //}
}
