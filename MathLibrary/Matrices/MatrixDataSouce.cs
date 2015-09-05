using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MathLibrary.Matrices
{
    /// <summary>Generic matrix data provider</summary>
    /// <typeparam name="T">Element type of this data source</typeparam>
    public class MatrixDataSource<T>
    {

        #region Private Members
        /*******************/
        /* PRIVATE MEMBERS */
        /*******************/
        private T[,] _data;

        #endregion Private Members


        #region Public Constructors
        /***********************/
        /* PUBLIC CONSTRUCTORS */
        /***********************/

        /// <summary>Instatiate a new empty (0x0) matrix data source</summary>
        public MatrixDataSource() : this(0) { }


        /// <summary>Instantiate a new matrix data source of the specified rank (r x r)</summary>
        /// <param name="rank">Rank of the matrix (number of rows and number of columns)</param>
        public MatrixDataSource(Int32 rank) : this(rank, rank) { }


        /// <summary>Instantiate a new matrix data source of the specified number of rows and columns (c x r)</summary>
        /// <param name="columns">Number of columns in the data source</param>
        /// <param name="rows">Number of rows in the data source</param>
        public MatrixDataSource(Int32 rows, Int32 columns)
        {
            _data = new T[rows, columns];
        }

        /// <summary>Instantiate a new matrix data source with using the provided 2 dimensional data array</summary>
        /// <param name="dataArray">Two-dimensional array of data to use when instantiating the matrix</param>
        public MatrixDataSource(T[,] dataArray)
        {
            // row first, translate it into column first representation
            Int32 rowCount = dataArray.GetLength(0);
            Int32 columnCount = dataArray.GetLength(1);
            _data = new T[rowCount, columnCount];
            for (Int32 i = 0; i < rowCount; i++)
                for (Int32 j = 0; j < columnCount; j++)
                    _data[i, j] = dataArray[i, j];
        }

        #endregion Public Constructors


        #region Public Getters / Setters
        /****************************/
        /* PUBLIC GETTERS / SETTERS */
        /****************************/

        /// <summary>Gets or sets the data source value at the specified column and row</summary>
        public T this[Int32 row, Int32 column]
        {
            get { return _data[row, column]; }
            set { _data[row, column] = value; }
        }

        /// <summary>Gets the number of columns in this matrix data source</summary>
        public Int32 ColumnCount { get { return _data.GetLength(1); } }

        /// <summary>Gets the number of rows in this matrix data source</summary>
        public Int32 RowCount { get { return _data.GetLength(0); } }

        /// <summary>Gets the default matrix data view including all columns and rows in this data source</summary>
        public MatrixBase<T> DefaultView { get { return new MatrixBase<T>(this); } }

        public T[,] GetData()
        {
            return _data;
        }

        #endregion Public Getters / Setters

    }
}
