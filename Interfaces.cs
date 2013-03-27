using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DeepLearn
{
    public interface IRBM
    {
        double[][] GetHiddenLayer(double[][] data);
        double[][] GetVisibleLayer(double[][] data);
        double[][] Reconstruct(double[][] data);
        double[][] DayDream(int numberOfSamples);

        void Train(double[][] data, int epochs, out double error);
        void AsyncTrain(double[][] data, int epochs);
           
        event EpochEventHandler EpochEnd;
        event EpochEventHandler TrainEnd;
    }

    public interface IDBN 
    {
        double[][] Encode(double[][] data);
        double[][] Decode(double[][] data);
        double[][] Reconstruct(double[][] data);
        double[][] DayDream(int numberOfSamples);


        double[][] Train(double[][] data, int epochs,int layerPosition, out double error);
        void AsyncTrain(double[][] data, int epochs, int layerPosition);

        void TrainAll(double[][] visibleData, int epochs, int epochMultiplier);
        void AsyncTrainAll(double[][] visibleData, int epochs, int epochMultiplier);
    }
}
