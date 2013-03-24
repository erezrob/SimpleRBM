using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DeepLearn
{
    public  static class ExtensionClasses
    {
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

        public static double[] Flatten(this double[][] arr)
        {
            return arr.SelectMany(x => x).ToArray();
        }
    }
}
