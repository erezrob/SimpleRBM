/*
 * Restricted Boltzman machine implementation in C#
 * Author: Erez Robinson (erez.robinson@gmail.com)  
 */

using System;
using System.Linq;
using System.Threading;

namespace DeepLearn
{
    class Program
    {
        static void Main(string[] args)
        {
            var trainingData =  DataParser.Parse("optdigits-tra.txt").Take(100).ToArray();

            //Although it is tempting to say that the final hidden layer has 10 features (10 numbers) but let's keep it real.
            var rbm = new MultilayeredRBM(new[] {1024, 128, 16}, 0.4);
        
            rbm.TrainAll(trainingData, 450, 3);
            
            Console.WriteLine("\n\n");
            
            //Take a sample of input arrays and try to reconstruct them.
            var reconstructedItems = rbm.Reconstruct(trainingData.Skip(50).Take(5).ToArray());

            reconstructedItems.ToList().ForEach(x =>
                                                    {
                                                        Console.WriteLine("");
                                                        x.PrintMap(32);
                                                    });
            
            Console.ReadKey();
        }
    }
}
