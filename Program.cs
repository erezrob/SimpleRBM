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
            //Our dataset cosists of images of handwritten digits (0-9)
            //Let's only take 100 of those for training
            var trainingData =  DataParser.Parse("optdigits-tra.txt").Take(100).ToArray();

            //Although it is tempting to say that the final hidden layer has 10 features (10 numbers) but let's keep it real.
            var rbm = new DeepBeliefNetwork(new[] {1024, 50,16}, 0.3);
        
            rbm.TrainAll(trainingData, 150, 5);
     
            
            Console.WriteLine("\n\n");
            
            //Take a sample of input arrays and try to reconstruct them.
            var reconstructedItems = rbm.Reconstruct(trainingData.Skip(50).Take(2).ToArray());
         
            reconstructedItems.ToList().ForEach(x =>
                                                    {
                                                        Console.WriteLine("");
                                                        x.PrintMap(32);
                                                    });

            Console.ReadKey();
            Console.WriteLine("\n\n");

            //Day dream 10 images
            rbm.DayDream(10).ToList().ForEach(x =>
                                                  {
                                                      Console.WriteLine("");
                                                      x.PrintMap(32);
                                                  });
            
            Console.ReadKey();
        }
    }
}
