using System;
using System.Collections.Generic;

namespace MathLibrary.Matrices
{
    public class ZigZagAccessorFactory<T> : AccessorFactory<T>
    {
        #region Fields


        private readonly List<ElementLocation> _elementLocations = new List<ElementLocation>();
        private readonly int _rows;
        private readonly int _columns;

        #endregion

        #region Methods

        public override Accessor<T> Access(MatrixBase<T> data)
        {
            return new Accessor<T>(_elementLocations, data);
        }

        #endregion

        #region Instance

        public ZigZagAccessorFactory(int size)
            : this(size, size)
        {
        }

        public ZigZagAccessorFactory(int rows, int columns)
        {
            _rows = rows;
            _columns = columns;
            InitializeZigZagScan(rows, columns);
        }

        #endregion

        #region Methods

        private void InitializeZigZagScan(int rows, int columns)
        {
            var height = rows;
            var width = columns;

            var currentRowIndex = 0;
            var currentColumnIndex = 0;
            var nextDirection = Direction.Right;
            var length = rows*columns;


            for (int i = 0; i < length; i++)
            {
                _elementLocations.Add(new ElementLocation {RowIndex = currentRowIndex, ColumnIndex = currentColumnIndex});

                if (nextDirection.HasFlag(Direction.Right))
                {
                    currentColumnIndex++;
                }
                if (nextDirection.HasFlag(Direction.Left))
                {
                    currentColumnIndex--;
                }
                if (nextDirection.HasFlag(Direction.Down))
                {
                    currentRowIndex++;
                }
                if (nextDirection.HasFlag(Direction.Up))
                {
                    currentRowIndex--;
                }
                nextDirection = GetNextDirection(width, height, currentRowIndex, currentColumnIndex, nextDirection);
            }

        }

        private static Direction GetNextDirection(int width, int height, int currentRowIndex, int currentColumnIndex, Direction currentDirection)
        {
            switch (currentDirection)
            {
                case Direction.Right:
                    if (currentRowIndex + 1 < height)
                    {
                        return Direction.Down | Direction.Left;
                    }
                    return Direction.Up | Direction.Right;

                case Direction.Down:
                    if (currentColumnIndex + 1 < width)
                    {
                        return Direction.Up | Direction.Right;
                    }
                    return Direction.Down | Direction.Left;
            }

            if (currentRowIndex == 0)
            {
                return currentColumnIndex + 1 < width ? Direction.Right : Direction.Down;
            }

            if (currentColumnIndex == 0)
            {
                return currentRowIndex + 1 < height ? Direction.Down : Direction.Right;
            }

            if (currentColumnIndex + 1 == width)
            {
                return Direction.Down;
            }

            if (currentRowIndex + 1 == height)
            {
                return Direction.Right;
            }

            return currentDirection;
        }

        #endregion

        #region HelperObjects

        [Flags]
        private enum Direction
        {
            Right = 1,
            Left = 2,
            Down = 4,
            Up = 8
        }


        #endregion
    }

}