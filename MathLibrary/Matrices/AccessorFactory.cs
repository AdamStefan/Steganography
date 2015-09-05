namespace MathLibrary.Matrices
{
    public abstract class AccessorFactory<T>
    {
        public abstract Accessor<T> Access(MatrixBase<T> data);
    }
}