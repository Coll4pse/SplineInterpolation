using System;

namespace SplineInterpolation
{
    public static class Sweep
    {
        public static double[] Evaluate(Matrix aCoefficients, Matrix bCoefficients)
        {
            var (U, V) = DirectMove(aCoefficients, bCoefficients);
            return ReverseMove(U, V);
        }

        private static (double[] U, double[] V) DirectMove(Matrix aCoefficients, Matrix bCoefficients)
        {
            var U = new double[aCoefficients.RowCount];
            var V = new double[aCoefficients.RowCount];
            for (var i = 0; i < aCoefficients.RowCount; i++)
            {
                var a = i == 0 ? 0 : aCoefficients[i, i - 1];
                var b = aCoefficients[i, i];
                var c = i == aCoefficients.RowCount - 1 ? 0 : aCoefficients[i, i + 1];

                if (i == 0)
                {
                    U[i] = -c / b;
                    V[i] = bCoefficients[i, 0] / b;
                }
                else
                {
                    U[i] = -c / (a * U[i - 1] + b);
                    V[i] = (bCoefficients[i, 0] - a * V[i - 1]) / (a * U[i - 1] + b);
                }
            }

            return (U, V);
        }

        private static double[] ReverseMove(double[] U, double[] V)
        {
            var solves = new double[U.Length];
            solves[U.Length - 1] = Math.Round(V[U.Length - 1], 2);
            for (var i = U.Length - 2; i >= 0; i--) solves[i] = Math.Round(U[i] * solves[i + 1] + V[i], 2);

            return solves;
        }
    }
}