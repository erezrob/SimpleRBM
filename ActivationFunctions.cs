using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DeepLearn
{
    public static class ActivationFunctions
    {
        /// <summary>
        /// Basic sigmoid function
        /// </summary>
        /// <param name="x">Real number</param>
        /// <returns>Real number</returns>
        public static double Logistic(double x)
        {
            return 1d / (1d + Math.Exp(-x));
        }
        
        /// <summary>
        /// Apply a logistic function on all elements of a matrix
        /// </summary>
        /// <param name="matrix">Matrix</param>
        /// <returns>Logistic matrix</returns>
        public static RealMatrix Logistic(RealMatrix matrix)
        {
            var result = new RealMatrix(matrix.Height, matrix.Width);

            for (int i = 0; i < matrix.Height; i++)
            {
                for (int j = 0; j < matrix.Width; j++)
                {
                    result[i, j] = Logistic(matrix[i, j]);
                }
            }
            return result;
        }

        public static RVector Logistic(RVector vector)
        {
            var result = new RVector(vector.Length);

            for (int i = 0; i < vector.Length; i++)
            {

                result[i] = Logistic(vector[i]);
               
            }
            return result;
        }
    }
}
