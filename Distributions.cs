using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DeepLearn
{
    class Distributions
    {
        private static Random rand = new Random(); //reuse this if you are generating many
        /// <summary>
        /// u(0,1) normal dist
        /// </summary>
        public static double GaussianNormal()
        {
          
            double u1 = rand.NextDouble(); //these are uniform(0,1) random doubles
            double u2 = rand.NextDouble();
            double randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) *
                         Math.Sin(2.0 * Math.PI * u2); //random normal(0,1)
            //double randNormal =
            //             mean + stdDev * randStdNormal; //random normal(mean,stdDev^2)
            return randStdNormal;
        }

        public static RealMatrix GaussianMatrix(int rows, int cols)
        {
            RealMatrix matrix = new RealMatrix(rows, cols);

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    matrix[i, j] = GaussianNormal();
                }   
            }
            return matrix;
        }

        public static RealMatrix UniformRandromMatrix(int rows, int cols)
        {
            RealMatrix matrix = new RealMatrix(rows, cols);

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    matrix[i, j] = rand.NextDouble();
                }
            }
            return matrix;
        }
    }
}
