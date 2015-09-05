using System.Linq;
using MathLibrary.Matrices;
using System;
using System.Collections.Generic;

namespace MathLibrary.Geometry
{
    public class Point2DCollection : List<Point2D>
    {

        #region Public Constructors
        /***********************/
        /* PUBLIC CONSTRUCTORS */
        /***********************/

        #endregion Public Constructors


        #region Public Methods
        /******************/
        /* PUBLIC METHODS */
        /******************/

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public DoubleMatrix AsAugmentedMatrix()
        {
            var matrix = new DoubleMatrix(Count, 3);
            for (int j = 0; j < Count; j++)
            {
                matrix[j, 0] = this[j].AugmentedMatrix[0, 0];
                matrix[j, 1] = this[j].AugmentedMatrix[0, 1];
                matrix[j, 2] = this[j].AugmentedMatrix[0, 2];
            }
            return matrix;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public DoubleMatrix AsMatrix()
        {
            var matrix = new DoubleMatrix(Count, 2);
            for (int j = 0; j < Count; j++)
            {
                matrix[j, 0] = this[j].AugmentedMatrix[0, 0];
                matrix[j, 1] = this[j].AugmentedMatrix[0, 1];
            }
            return matrix;
        }


        /*
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Point[] AsPointArray()
        {
            Point[] points = new Point[this.Count];
            for (int j = 0; j < this.Count; j++)
                points[j] = this[j].AsPoint();
            return points;
        }
         */

        #endregion Public Methods


        #region Public Static Methods
        /*************************/
        /* PUBLIC STATIC METHODS */
        /*************************/

        /// <summary>
        /// 
        /// </summary>
        /// <param name="matrix"></param>
        /// <returns></returns>
        public static Point2DCollection FromMatrix(DoubleMatrix matrix)
        {
            if (matrix == null) throw new ArgumentNullException("matrix");
            if (matrix.ColumnCount != 3) throw new DimensionMismatchException();

            var collection = new Point2DCollection();
            collection.AddRange(matrix.Rows.Select(row => new Point2D(row)));

            return collection;
        }


        /*
        /// <summary>
        /// 
        /// </summary>
        /// <param name="points"></param>
        /// <returns></returns>
        public static Point2dCollection FromPointArray(Point[] points)
        {
            if (points == null) throw new ArgumentNullException("points");

            Point2dCollection collection = new Point2dCollection();
            foreach (Point point in points)
                collection.Add(Point2d.FromPoint(point));

            return collection;
        }
        */
        #endregion Public Static Methods

    }
}
