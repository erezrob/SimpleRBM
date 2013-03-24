using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeepLearn
{
    public static class Distributions
    {
        private static readonly Random Random = new Random(); //reuse this if you are generating many non time dependant numbers
        
        /// <summary>
        /// Random is not a thread-safe class, this helper function locks our global Random instance.
        /// </summary>
        /// <returns></returns>
        public static double GetRandomDouble()
        {
            var x = 0d;
            lock (Random)
                x = Random.NextDouble();
            return x;
        }
        
        /// <summary>
        /// u(0,1) normal dist
        /// </summary>
        public static double GaussianNormal()
        {

            double u1 = 0;
            while(u1 == 0.0)
                u1 = GetRandomDouble();
            double u2 = GetRandomDouble(); 
            double randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Sin(2.0 * Math.PI * u2); //random normal(0,1)
            return randStdNormal;
        }

        /// <summary>
        /// Random Gaussian Matrix
        /// </summary>
        /// <param name="rows"></param>
        /// <param name="cols"></param>
        /// <returns></returns>
        public static RealMatrix GaussianMatrix(int rows, int cols)
        {
            var matrix = new RealMatrix(rows, cols);

            Parallel.For(0, rows, i => Parallel.For(0, cols, j =>
                                                                 {
                                                                     matrix[i, j] = GaussianNormal();
                                                                 }));

            return matrix;
        }


        /// <summary>
        /// Uniform Random Matrix
        /// </summary>
        /// <param name="rows"></param>
        /// <param name="cols"></param>
        /// <returns></returns>
        public static RealMatrix UniformRandromMatrix(int rows, int cols)
        {
            var matrix = new RealMatrix(rows, cols);

            Parallel.For(0, rows, i => Parallel.For(0, cols, j =>
                                                                 {
                                                                     matrix[i, j] = GetRandomDouble(); 
                                                                 }));
            return matrix;
        }

        /// <summary>
        /// Uniform Random Boolean Matrix
        /// </summary>
        /// <param name="rows"></param>
        /// <param name="cols"></param>
        /// <returns></returns>
        public static RealMatrix UniformRandromMatrixBool(int rows, int cols)
        {
            var matrix = new RealMatrix(rows, cols);

            Parallel.For(0, rows, i => Parallel.For(0, cols, j =>
                                                                 {
                                                                     matrix[i, j] = Convert.ToInt32(GetRandomDouble());
                                                                 }));

            return matrix;
        }

        /// <summary>
        /// Uniform Random Vector
        /// </summary>
        /// <param name="numElements"></param>
        /// <returns></returns>
        public static RVector UniformRandromVector(int numElements)
        {
            var vector = new RVector(numElements);
            
            Parallel.For(0, numElements, i =>
                                             {
                                                 vector[i] = GetRandomDouble(); 
                                             });
            return vector;
        }
    }
}
