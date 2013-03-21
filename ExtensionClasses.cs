using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DeepLearn
{
    public  static class ExtensionClasses
    {
        public static int N = 2;
        public static void Topt(double[][] A, double[][] B, double[][] C)
        {
            var source = Enumerable.Range(0, N);
            var pquery = from num in source.AsParallel()
                         select num;
            pquery.ForAll((e) => Popt(A, B, C, e));
        }
        public static void Popt(double[][] A, double[][] B, double[][] C, int i)
        {
            double[] iRowA = A[i];
            double[] iRowC = C[i];
            for (int k = 0; k < N; k++)
            {
                double[] kRowB = B[k];
                double ikA = iRowA[k];
                for (int j = 0; j < N; j++)
                {
                    iRowC[j] += ikA * kRowB[j];
                }
            }
        }

        public static void PrintMap(this double[] arr,  int rows)
        {
            for (int i = 0; i < arr.Length; i++)
            {
                if(i % rows == 0)
                    Console.WriteLine("");
                Console.Write(arr[i].ToString("N0"));
            }
        }

        public static void PrintMap(this RealMatrix mat, int rows)
        {
            mat.ToArray().Flatten().PrintMap(rows);
        }

        public static double[] Flatten(this double[,] arr)
        {
            return arr.Cast<double>().ToArray();
        }
    }
}
