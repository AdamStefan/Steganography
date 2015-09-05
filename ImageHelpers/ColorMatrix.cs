using System;
using System.Drawing;
using MathLibrary.Matrices;

namespace ImageHelpers
{    

    public static class MatrixColorExtensions
    {

        public static MatrixBase<YCbCrColor> ToYCbCr(this MatrixBase<Color> matrix)
        {
            var ret = new MatrixBase<YCbCrColor>(matrix.RowCount, matrix.ColumnCount);

            for (int i = 0; i < matrix.RowCount; i++)
            {
                for (int j = 0; j < matrix.ColumnCount; j++)
                {
                    var color = matrix[i, j].ToYCrCb();
                    ret[i, j] = new YCbCrColor { Y = color[0], Cb = color[1], Cr = color[2] };
                }
            }
            return ret;
        }

        public static MatrixBase<Color> ToRgb(this MatrixBase<YCbCrColor> matrix)
        {
            var ret = new MatrixBase<Color>(matrix.RowCount, matrix.ColumnCount);

            for (int i = 0; i < matrix.RowCount; i++)
            {
                for (int j = 0; j < matrix.ColumnCount; j++)
                {
                    var color = matrix[i, j].ToRgb();
                    ret[i, j] = color;
                }
            }
            return ret;
        }

        public static DoubleMatrix GetChannel(this MatrixBase<YCbCrColor> matrix,  ChannelType channelType)
        {
            var ret = new DoubleMatrix(matrix.RowCount, matrix.ColumnCount);

            for (int i = 0; i < matrix.RowCount; i++)
            {
                for (int j = 0; j < matrix.ColumnCount; j++)
                {
                    switch (channelType)
                    {
                        case ChannelType.Y:
                            ret[i, j] = matrix[i, j].Y;
                            break;
                        case ChannelType.Cr:
                            ret[i, j] = matrix[i, j].Cr;
                            break;
                        case ChannelType.Cb:
                            ret[i, j] = matrix[i, j].Cb;
                            break;
                        default:
                            throw new ArgumentOutOfRangeException("channelType");
                    }
                }
            }

            return ret;
        }

        public static void UpdateChannel(this MatrixBase<YCbCrColor> matrix,ChannelType channelType, DoubleMatrix value)
        {

            for (int i = 0; i < matrix.RowCount; i++)
            {
                for (int j = 0; j < matrix.ColumnCount; j++)
                {
                    var item = matrix[i, j];
                    switch (channelType)
                    {
                        case ChannelType.Y:
                            item.Y = value[i, j];
                            break;
                        case ChannelType.Cr:
                            item.Cr = value[i, j];
                            break;
                        case ChannelType.Cb:
                            item.Cb = value[i, j];
                            break;
                        default:
                            throw new ArgumentOutOfRangeException("channelType");
                    }
                }
            }
        }
    }
}
