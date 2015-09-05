using System.Collections.Generic;

namespace MathLibrary.Matrices
{
    public class ArrayAccessorFactory<T> : AccessorFactory<T>
    {

        #region Fields

        private readonly List<ElementLocation> _elementLocations = new List<ElementLocation>();
        private readonly int _rows;
        private readonly int _columns;


        #endregion

        #region Instance

        public ArrayAccessorFactory(int size)
            : this(size, size)
        {
        }

        public ArrayAccessorFactory(int rows, int columns)
        {
            _rows = rows;
            _columns = columns;
            InitializeAccessor();
        }

        #endregion

        private void InitializeAccessor()
        {
            for (int i = 0; i < _rows; i++)
            {
                for (int j = 0; j < _columns; j++)
                {
                    var elementLocation = new ElementLocation {RowIndex = i, ColumnIndex = j};
                    _elementLocations.Add(elementLocation);
                }
            }
        }

        public override Accessor<T> Access(MatrixBase<T> data)
        {
            return new Accessor<T>(_elementLocations, data);
        }
    }
}