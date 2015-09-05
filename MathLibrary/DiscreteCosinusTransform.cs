using MathLibrary.Matrices;
using System;

namespace MathLibrary
{
    /// <summary>
    /// Implementation of 2D Discrete Cosine Transform
    /// </summary>
    public static class DiscreteCosineTransform
    {
        /// <summary>
        /// Initialize alpha coefficient array 
        /// </summary>
        private static Double[,] InitCoefficientsMatrix(int dim)
        {
            var coefficientsMatrix = new double[dim, dim];

            for (int i = 0; i < dim; i++)
            {
                coefficientsMatrix[i, 0] = Math.Sqrt(2.0) / dim;
                coefficientsMatrix[0, i] = Math.Sqrt(2.0) / dim;
            }

            coefficientsMatrix[0, 0] = 1.0 / dim;

            for (var i = 1; i < dim; i++)
            {
                for (var j = 1; j < dim; j++)
                {
                    coefficientsMatrix[i, j] = 2.0 / dim;
                }
            }
            return coefficientsMatrix;
        }

        private static bool IsQuadricMatrix<T>(T[,] matrix)
        {
            var columnsCount = matrix.GetLength(0);
            var rowsCount = matrix.GetLength(1);
            return (columnsCount == rowsCount);
        }

        public static Double[,] ForwardDct(Double[,] input)
        {
            if (IsQuadricMatrix(input) == false)
            {
                throw new ArgumentException("Matrix must be quadric");
            }

            int n = input.GetLength(0);

            double[,] coefficientsMatrix = InitCoefficientsMatrix(n);
            var output = new Double[n, n];

            for (int u = 0; u <= n - 1; u++)
            {
                for (int v = 0; v <= n - 1; v++)
                {
                    double sum = 0.0;
                    for (int x = 0; x <= n - 1; x++)
                    {
                        for (int y = 0; y <= n - 1; y++)
                        {
                            sum += input[x, y] * Math.Cos(((2.0 * x + 1.0) / (2.0 * n)) * u * Math.PI) * Math.Cos(((2.0 * y + 1.0) / (2.0 * n)) * v * Math.PI);
                        }
                    }
                    sum *= coefficientsMatrix[u, v];
                    output[u, v] = Math.Round(sum);
                }
            }
            return output;
        }

        public static Double[,] InverseDct(Double[,] input)
        {
            if (IsQuadricMatrix(input) == false)
            {
                throw new ArgumentException("Matrix must be quadric");
            }

            int n = input.GetLength(0);

            Double[,] coefficientsMatrix = InitCoefficientsMatrix(n);
            var output = new Double[n, n];

            for (int x = 0; x <= n - 1; x++)
            {
                for (int y = 0; y <= n - 1; y++)
                {
                    double sum = 0.0;
                    for (int u = 0; u <= n - 1; u++)
                    {
                        for (int v = 0; v <= n - 1; v++)
                        {
                            sum += coefficientsMatrix[u, v] * input[u, v] * Math.Cos(((2.0 * x + 1.0) / (2.0 * n)) * u * Math.PI) * Math.Cos(((2.0 * y + 1.0) / (2.0 * n)) * v * Math.PI);
                        }
                    }
                    output[x, y] = Math.Round(sum);
                    if (output[x, y] < 0)
                    {

                    }
                }
            }
            return output;
        }


        #region DCT for 8x8 double block

        public static DoubleMatrix ForwardDct8Block(DoubleMatrix input)
        {
            //DoubleMatrix temp = DoubleMatrix.Identity(8) / 2;
            //temp[0, 0] = 0.5 / Math.Sqrt(2);

            ////DCT basis vector
            //DoubleMatrix helper = new DoubleMatrix(8);
            //for (int rowIndex = 0; rowIndex < 8; rowIndex++)
            //{
            //    for (int columnIndex = 0; columnIndex < 8; columnIndex++)
            //    {
            //        helper[columnIndex, rowIndex] = Math.Cos((2 * (double)rowIndex + 1) * (double)columnIndex * Math.PI / 16.0);
            //    }
            //}

            //helper = temp * helper;
            //return helper * input * helper.Transposed;
            var dct = _initializationMatrixDct8Block * input * _initializationMatrixDct8BlockTranpose;
            //for (int i = 0; i < dct.RowCount; i++)
            //{
            //    for (int j = 0; j < dct.ColumnCount; j++)
            //    {
            //        dct[i, j] = Math.Floor(dct[i, j]);
            //    }
            //}
           
            return dct;
        }

        public static DoubleMatrix InverseDct8Block(DoubleMatrix input)
        {
            //DoubleMatrix temp = DoubleMatrix.Identity(8) / 2;
            //temp[0, 0] = 0.5 / Math.Sqrt(2);
            ////DCT basis vector
            //DoubleMatrix helper = new DoubleMatrix(8);
            //for (int rowIndex = 0; rowIndex < 8; rowIndex++)
            //{
            //    for (int columnIndex = 0; columnIndex < 8; columnIndex++)
            //    {
            //        helper[columnIndex, rowIndex] = Math.Cos((2 * (double)rowIndex + 1) * (double)columnIndex * Math.PI / 16.0);
            //    }
            //}
            //helper = temp * helper;
            //return helper.Transposed * input * helper;

            var dct = _initializationMatrixDct8BlockTranpose * input * _initializationMatrixDct8Block;
            //for (int i = 0; i < dct.RowCount; i++)
            //{
            //    for (int j = 0; j < dct.ColumnCount; j++)
            //    {
            //        dct[i, j] = Math.Round(dct[i, j]);
            //    }
            //}
            return dct;
        }

        private static DoubleMatrix _initializationMatrixDct8Block;
        private static DoubleMatrix _initializationMatrixDct8BlockTranpose;

        #endregion       

        #region InitMethods

        public static void Init()
        {
            DoubleMatrix temp = DoubleMatrix.Identity(8) / 2;
            temp[0, 0] = 0.5 / Math.Sqrt(2);

            //DCT basis vector
            var helper = new DoubleMatrix(8);
            for (int rowIndex = 0; rowIndex < 8; rowIndex++)
            {
                for (int columnIndex = 0; columnIndex < 8; columnIndex++)
                {
                    helper[columnIndex, rowIndex] = Math.Cos((2 * (double)rowIndex + 1) * columnIndex * Math.PI / 16.0);
                }
            }

            helper = temp * helper;
            _initializationMatrixDct8Block = helper;
            _initializationMatrixDct8BlockTranpose = _initializationMatrixDct8Block.Transposed;
        }

        static DiscreteCosineTransform()
        {
            Init();
        }

        #endregion

    }

    

    
}
