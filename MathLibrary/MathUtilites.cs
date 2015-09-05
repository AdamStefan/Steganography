using MathLibrary.Matrices;
using System;
using System.Collections.Generic;

namespace MathLibrary
{
    public static class MathUtilities
    {
        /// <summary>
        /// Split a 2d double array into block of size equal with blocksize
        /// </summary>
        /// <param name="input"></param>
        /// <param name="blockSize"></param>
        /// <returns></returns>
        public static IList<MatrixBase<T>> Split2DArrayToBlocks<T>(MatrixBase<T> input, int blockSize)
        {
            var numberOfBlocks = (input.RowCount*input.ColumnCount)/(blockSize*blockSize);
            var rows = input.RowCount; // number of rows
            var columns = input.ColumnCount; // number of columns
            var numberOfBlocksPerRow = rows/blockSize;
            var numberOfBlocksPerColumn = columns/blockSize;


            var ret = new List<MatrixBase<T>>();

            for (int i = 0; i < numberOfBlocksPerRow; i++)
            {
                for (int j = 0; j < numberOfBlocksPerColumn; j++)
                {
                    var blockRowStartIndex = i*blockSize;
                    var blockColumnStartIndex = j*blockSize;

                    ret.Add(input.SubMatrix(new Int32Range(blockRowStartIndex, blockRowStartIndex + blockSize),
                                            new Int32Range(blockColumnStartIndex, blockColumnStartIndex + blockSize)));
                }
            }

            if (ret.Count != numberOfBlocks)
            {
                throw new Exception("error on splitting input to blocks");
            }

            return ret;
        }



        


        /// <summary>
        /// each block should be a square (rows = height)
        /// all the blocks should have the same size
        /// </summary>
        /// <param name="blocks"></param>
        /// <param name="rows"></param>
        /// <param name="columns"></param>
        /// <returns></returns>
        public static MatrixBase<T> Merge2DArraysBlocks<T>(IList<MatrixBase<T>> blocks, int rows, int columns)
        {
            if (blocks.Count == 0)
            {
                return null;
            }
            var blockSize = blocks[0].RowCount;
            var ret = new MatrixBase<T>(rows, columns);


            for (int i = 0; i < blocks.Count; i++)
            {
                var blockRowStartIndex = ((i * blockSize) / columns) * blockSize;
                var blockColumnStartIndex = ((i * blockSize) % columns);

                for (int blockRowIndex = 0; blockRowIndex < blockSize; blockRowIndex++)
                {
                    for (int blockColumnIndex = 0; blockColumnIndex < blockSize; blockColumnIndex++)
                    {
                        var value = blocks[i][blockRowIndex, blockColumnIndex];
                        ret[blockRowStartIndex + blockRowIndex, blockColumnStartIndex + blockColumnIndex] = value;
                    }
                }
            }

            return ret;
        }        
        
        

        public static bool GetBit(byte[] bytes, int index)
        {
            var blockByteindex = index / 8;
            var bitIndex = index % 8;
            byte block = bytes[blockByteindex];
            int ret;
            unchecked
            {
                ret = (byte)(1 << (7 - bitIndex)) & block;
            }

            return ret > 0;
        }

        public static byte Reverse(this byte value)
        {
            int result = 0;
            for (int i = 0; i < 8; i++)
            {
                result = result << 1;
                result += value & 1;
                value = (byte)(value >> 1);
            }

            return (byte)result;
        }

        public static void AddBit(ref byte current, bool value)
        {
            unchecked
            {
                current = (byte)(current << 1);
            }
            if (value)
            {
                current++;
            }
        }
      


    }

    public static class IntExtensions
    {
        public static int Modulo(this int value, int modulo)
        {
            int ret = value % modulo;
            return ret < 0 ? ret + modulo : ret;
        }
    }

    public static class DoubleArrayExtension
    {
        public static double[,] DivideEachItemWith(this double[,] value, double[,] divizor)
        {
            var height = value.GetLength(0);
            var width = value.GetLength(1);
            

            if (height != divizor.GetLength(0) || width != divizor.GetLength(1))
            {
                throw new ArgumentException("the divizor should have the same size");
            }
            var ret = new double[height, width];

            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    if (Math.Abs(divizor[i, j] - 0) < 0.000000001)
                    {
                        throw new DivideByZeroException();
                    }
                    ret[i, j] = value[i, j] / divizor[i, j];
                }
            }

            return ret;
        }

        public static double[,] MultiplyEachItemWith(this double[,] value, double[,] multiplier)
        {
            var height = value.GetLength(0);
            var width = value.GetLength(1);


            if (height != multiplier.GetLength(0) || width != multiplier.GetLength(1))
            {
                throw new ArgumentException("the divizor should have the same size");
            }
            var ret = new double[height,width];

            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {

                    ret[i, j] = value[i, j]*multiplier[i, j];
                }
            }
            return ret;
        }


        public static double[,] Add(this double[,] current, double value)
        {
            var height = current.GetLength(0);
            var width = current.GetLength(1);

            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    current[i, j] += value;
                }
            }

            return current;
        }
    }
}
