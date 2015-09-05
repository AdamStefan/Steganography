using System;
using System.Collections;
using System.Collections.Generic;

namespace MathLibrary.Matrices
{
    public class Accessor<T> : IEnumerable<T>
    {
        #region Fields

        private readonly List<ElementLocation> _elementLocations = new List<ElementLocation>();
        private readonly MatrixBase<T> _data;
        private readonly int _length;

        #endregion

        #region Instance

        public Accessor(List<ElementLocation> elementLocations, MatrixBase<T> data)
        {
            _elementLocations = elementLocations;
            _data = data;
            _length = elementLocations.Count;
        }

        #endregion

        #region Methods

        public T this[int index]
        {
            get
            {
                var location = _elementLocations[index];
                return _data[location.RowIndex, location.ColumnIndex];
            }
            set
            {
                var location = _elementLocations[index];
                _data[location.RowIndex, location.ColumnIndex] = value;
            }
        }

        public int Length
        {
            get { return _length; }
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

        public IEnumerator<T> GetEnumerator()
        {
            return new AccessorEnumerator<T>(this);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private class AccessorEnumerator<T> : IEnumerator<T>
        {
            #region Fields

            private Accessor<T> _accessor;
            private int _curentIndex;

            #endregion

            #region Instance

            public AccessorEnumerator(Accessor<T> accessor)
            {
                _accessor = accessor;
            }

            #endregion

            public void Dispose()
            {
                _accessor = null;
            }

            public bool MoveNext()
            {
                if (_curentIndex < _accessor._length - 1)
                {
                    _curentIndex++;
                    return true;
                }
                return false;
            }

            public void Reset()
            {
                _curentIndex++;
            }

            public T Current
            {
                get { return _accessor[_curentIndex]; }
            }

            object IEnumerator.Current
            {
                get { return Current; }
            }
        }
    }
}