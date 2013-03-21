using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DeepLearn
{
    public interface IRBM
    {
        double[][] RunVisible(double[][] data);
        double[][] RunHidden(double[][] data);
        double[][] Reconstruct(double[][] data);

        void Train(double[][] data, int epochs, out double error);
        void AsyncTrain(double[][] data, int epochs);
           
        event EpochEventHandler EpochEnd;
        event EpochEventHandler TrainEnd;
    }

    public interface IMultilayeredRBM : IRBM
    {
        double[][] Train(double[][] data, int epochs,int layerPosition, out double error);
        void AsyncTrain(double[][] data, int epochs, int layerPosition);

        void TrainAll(double[][] visibleData, int epochs, int epochMultiplier);
        void AsyncTrainAll(double[][] visibleData, int epochs, int epochMultiplier);
    }
}
