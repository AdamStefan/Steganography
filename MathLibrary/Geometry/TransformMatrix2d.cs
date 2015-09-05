using MathLibrary.Matrices;
using System;

namespace MathLibrary.Geometry
{
    public class TransformMatrix2D : DoubleMatrix
    {

        #region Public Constructors
        /***********************/
        /* PUBLIC CONSTRUCTORS */
        /***********************/

        public TransformMatrix2D()
            : base(3)
        {
            CopyFrom(Identity(3));
        }

        #endregion Public Constructors

        #region Public Accessors
        /********************/
        /* PUBLIC ACCESSORS */
        /********************/

        public DoubleMatrix CartesianTransformationMatrix
        {
            get { throw new NotImplementedException(); }
        }

        public DoubleMatrix CartesianTranslationVector
        {
            get { throw new NotImplementedException(); }
        }

        #endregion Public Accessors

        #region Public Static Methods
        /*************************/
        /* PUBLIC STATIC METHODS */
        /*************************/

        /// <summary>
        /// 
        /// </summary>
        /// <param name="angle">Counterclockwise rotation angle in radians</param>
        /// <param name="angleRadians"></param>
        /// <returns></returns>
        public static TransformMatrix2D RotationTransform(Double angleRadians)
        {
            var result = new TransformMatrix2D();
            result[0, 0] = Math.Cos(angleRadians);
            result[0, 1] = -Math.Sin(angleRadians);
            result[1, 0] = Math.Sin(angleRadians);
            result[1, 1] = Math.Cos(angleRadians);
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="scale"></param>
        /// <returns></returns>
        public static TransformMatrix2D ScalingTransform(Double scale)
        {
            var result = new TransformMatrix2D();
            result[0, 0] = scale;
            result[1, 1] = scale;
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="shear"></param>
        /// <returns></returns>
        public static TransformMatrix2D HorizontalShearingTransform(Double shear)
        {
            var result = (TransformMatrix2D)Identity(2);
            result[0, 1] = shear;
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="shear"></param>
        /// <returns></returns>
        public static TransformMatrix2D VerticalShearingTransform(Double shear)
        {
            var result = (TransformMatrix2D)Identity(2);
            result[1, 0] = shear;
            return result;
        }


        public static Double FittingError(
            Point2DCollection sourcePoints,
            Point2DCollection targetPoints,
            TransformMatrix2D transform)
        {
            return 0;
        }


        public static TransformMatrix2D BestFitTransformation(
            Point2DCollection sourcePoints,
            Point2DCollection targetPoints)
        {
            throw new NotImplementedException();

            /*
            if (sourcePoints.Count != targetPoints.Count) throw new ArgumentException();

            DoubleMatrix c = new DoubleMatrix(3, 2);
            for (int j = 0; j < 2; j++)
                for (int i = 0; i < 3; i++)
                    for (int k = 0; k < sourcePoints.Count; k++)
                    {
                        DoubleMatrix qt = DoubleMatrix.JoinHorizontal(sourcePoints.Rows[k], DoubleMatrix.Identity(1));
                        c[i, j] += qt[i, 0] * targetPoints[j, k];
                    }

            DoubleMatrix Q = new DoubleMatrix(3, 3).Transposed;
            foreach (DoubleMatrix rowMatrix in sourcePoints.Rows)
            {
                DoubleMatrix qt = DoubleMatrix.JoinHorizontal(rowMatrix, DoubleMatrix.Identity(1));
                for (int i = 0; i < 3; i++)
                    for (int j = 0; j < 3; j++)
                    {
                        Q[i, j] += qt[i, 0] * qt[j, 0];
                    }
            }

            DoubleMatrix M = DoubleMatrix.JoinVertical(Q, c).Transposed;

            DoubleMatrix reducedRow = DoubleMatrix.GaussianElimination(M);

            affineTransformationMatrix
                = reducedRow.SubMatrix(
                    new Int32Range(dimension + 1, 2 * dimension),
                    new Int32Range(0, dimension - 1));

            translationMatrix
                = reducedRow.SubMatrix(
                    new Int32Range(dimension + 1, 2 * dimension),
                    new Int32Range(dimension, dimension));
             */
        }


        #endregion Public Static Methods

    }
}
