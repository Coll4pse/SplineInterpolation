namespace SplineInterpolation
{
    public struct Matrix
    {
        private readonly double[,] matrix;

        public int RowCount => matrix.GetLength(0);

        public int ColumnCount => matrix.GetLength(1);

        public Matrix(int n, int m)
        {
            matrix = new double[n, m];
        }

        public Matrix(double[,] array) : this(array.GetLength(0), array.GetLength(1))
        {
            for (var i = 0; i < RowCount; i++)
            for (var j = 0; j < ColumnCount; j++)
                matrix[i, j] = array[i, j];
        }

        public Matrix SumRows(int sourceRowIndex, int destinationRowIndex, double modifier = 1)
        {
            var newMatrix = Clone();
            for (var i = 0; i < ColumnCount; i++)
                newMatrix[destinationRowIndex, i] += newMatrix[sourceRowIndex, i] * modifier;

            return newMatrix;
        }

        public Matrix SwapRows(int firstRowIndex, int secondRowIndex)
        {
            var newMatrix = Clone();
            for (var i = 0; i < ColumnCount; i++)
            {
                newMatrix[firstRowIndex, i] = matrix[secondRowIndex, i];
                newMatrix[secondRowIndex, i] = matrix[firstRowIndex, i];
            }

            return newMatrix;
        }

        public Matrix RowToEnd(int rowIndex)
        {
            var newMatrix = Clone();
            for (var i = rowIndex + 1; i < RowCount; i++)
            {
                newMatrix.SwapRows(rowIndex, i);
                rowIndex = i;
            }

            return newMatrix;
        }

        public double this[int i, int j]
        {
            get => matrix[i, j];
            set => matrix[i, j] = value;
        }

        public Matrix Clone()
        {
            return new Matrix(matrix);
        }
    }
}