using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;

namespace SplineInterpolation
{
    public static class Interpolation
    {
        public static Polynomial[] Evaluate(double[] x, double[] y)
        {
            var h = CreateH(x);
            var a = y.SkipLast(1).ToArray();
            
            var matrices = CreateMatrix(h, y);
            var c = new double[x.Length - 1];
            Sweep.Evaluate(matrices.aCoefficients, matrices.bCoefficietns).CopyTo(c, 1);
            
            var b = EvaluateB(y, c, h);
            var d = EvaluateD(c, h);

            return ConstructPolynomials(a, b, c, d, x).ToArray();
        }

        private static (Matrix aCoefficients, Matrix bCoefficietns) CreateMatrix(double[] h, double[] y)
        {
            var arrA = new double[h.Length - 1, h.Length - 1];
            var arrB = new double[h.Length - 1, 1];

            for (int i = 0; i < arrB.Length; i++)
            {
                if (i != 0)
                    arrA[i, i - 1] = h[i];
                arrA[i, i] = 2 * (h[i] + h[i + 1]);
                if (i != arrB.Length - 1)
                    arrA[i, i + 1] = h[i + 1];
                arrB[i, 0] = 3 * ((y[i + 2] - y[i + 1]) / h[i + 1] - (y[i + 1] - y[i]) / h[i]);
            }

            return (new Matrix(arrA), new Matrix(arrB));
        }

        private static double[] CreateH(double[] x)
        {
            var result = new double[x.Length - 1];
            for (int i = 0; i < result.Length; i++)
            {
                result[i] = x[i + 1] - x[i];
            }

            return result;
        }

        private static double[] EvaluateB(double[] y, double[] c, double[] h)
        {
            var b = new double[c.Length];
            for (int i = 0; i < b.Length - 1; i++)
            {
                b[i] = (y[i + 1] - y[i]) / h[i] - h[i] / 3 * (c[i + 1] + 2 * c[i]);
            }

            b[^1] = (y[^2] - y[^1]) / h[^1] - 2d / 3 * c[^1] * h[^1];
            return b;
        }

        private static double[] EvaluateD(double[] c, double[] h)
        {
            var d = new double[c.Length];
            for (int i = 0; i < d.Length - 1; i++)
            {
                d[i] = (c[i + 1] - c[i]) / (3 * h[i]);
            }

            d[^1] = -c[^1] / (3 * h[^1]);
            return d;
        }

        private static IEnumerable<Polynomial> ConstructPolynomials(double[] a, double[] b, double[] c, double[] d, double[] x)
        {
            for (int i = 0; i < a.Length; i++)
            {
                var p = new Polynomial(a[i]);
                p += new Polynomial(-x[i], 1) * b[i];
                p += (new Polynomial(-x[i], 1) ^ 2) * c[i];
                p += (new Polynomial(-x[i], 1) ^ 3) * d[i];
                yield return p;
            }
        }
    }
}