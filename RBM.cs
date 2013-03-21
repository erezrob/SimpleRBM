using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeepLearn
{
    public class RBM : IRBM
    {
        public event EpochEventHandler EpochEnd;

        public void RaiseEpochEnd(int seq, double err)
        {
            if (EpochEnd != null)
                EpochEnd(this, new EpochEventArgs(seq, err));
        }

        public event EpochEventHandler TrainEnd;

        public void RaiseTrainEnd(int seq, double err)
        {
            if (TrainEnd != null)
                TrainEnd(this, new EpochEventArgs(seq, err));
        }

        private readonly int m_numHidden;
        private readonly int m_numVisible;
        private readonly double m_learningRate;
        private RealMatrix m_weights;

        public RealMatrix Weights
        {
            get { return m_weights; }
        }

        public RBM(int numVisible, int numHidden, double learningRate = 0.1)
        {
            m_numHidden = numHidden;
            m_numVisible = numVisible;
            m_learningRate = learningRate;

            m_weights = 0.1*Distributions.GaussianMatrix(numVisible, numHidden);
            m_weights = m_weights.InsertRow(0);
            m_weights = m_weights.InsertCol(0);
        }

        public double[][] RunVisible(double[][] dataArray)
        {
            var num_examples = dataArray.Length;

            // Create a matrix, where each row is to be the hidden units (plus a bias unit)
            // sampled from a training example.
            var hidden_states = RealMatrix.Ones(num_examples, m_numHidden + 1);

            var data = new RealMatrix(dataArray);
            // Insert bias units of 1 into the first column of data.
            data = data.InsertCol(1); // np.insert(data, 0, 1, axis = 1)

            // Calculate the activations of the hidden units.
            var hiddenActivations = data*m_weights;
            // Calculate the probabilities of turning the hidden units on.
            var hiddenProbs = ActivationFunctions.Logistic(hiddenActivations);
            // Turn the hidden units on with their specified probabilities.
            hidden_states = hiddenProbs > Distributions.UniformRandromMatrix(num_examples, m_numHidden + 1);
          
            // Ignore the bias units.
            hidden_states = hidden_states.RemoveFirstCol(); //  hidden_states[:,1:]
            return hidden_states;
        }

        public double[][] RunHidden(double[][] dataArray)
        {
            var numExamples = dataArray.Length;

            // Create a matrix, where each row is to be the visible units (plus a bias unit)
            // sampled from a training example.
           
            var data = new RealMatrix(dataArray);
            // Insert bias units of 1 into the first column of data.
            data = data.InsertCol(1);

            // Calculate the activations of the visible units.
            var visibleActivations = data*m_weights.Transpose;
            // Calculate the probabilities of turning the visible units on.
            var visibleProbs = ActivationFunctions.Logistic(visibleActivations);
            // Turn the visible units on with their specified probabilities.
            var visibleStates = visibleProbs > Distributions.UniformRandromMatrix(numExamples, m_numVisible + 1);
         
            // Ignore the bias units.
            visibleStates = visibleStates.RemoveFirstCol(); //visible_states[:,1:]
            return visibleStates;
        }

        public void AsyncTrain(double[][] data, int maxEpochs)
        {
            double e = 0;
            TaskFactory f = new TaskFactory();
            f.StartNew(new Action(() => Train(data, maxEpochs, out e)));
        }
        

        public void Train(double[][] dataArray, int maxEpochs, out double error)
        {
            error = 0;

            var numExamples = dataArray.Length;
            var data = new RealMatrix(dataArray);

            data = data.InsertCol(1);

            for (int i = 0; i < maxEpochs; i++)
            {
                var posHiddenActivations = data*m_weights;
                var posHiddenProbs = ActivationFunctions.Logistic(posHiddenActivations);
                var posHiddenStates = posHiddenProbs > Distributions.UniformRandromMatrix(numExamples, m_numHidden + 1);
                var posAssociations = data.Transpose*posHiddenProbs;

                var negVisibleActivations = posHiddenStates*m_weights.Transpose;
                var negVisibleProbs = ActivationFunctions.Logistic(negVisibleActivations);

                negVisibleProbs = negVisibleProbs.Update(0, 1, 1);
                var negHiddenActivations = negVisibleProbs*m_weights;
                var negHiddenProbs = ActivationFunctions.Logistic(negHiddenActivations);

                var negAssociations = negVisibleProbs.Transpose*negHiddenProbs;

                m_weights = m_weights + (m_learningRate*((posAssociations - negAssociations)/numExamples));

                error = ((data - negVisibleProbs) ^ 2).Sum();
                RaiseEpochEnd(i, error);
                Console.WriteLine("Epoch {0}: error is {1}", i, error);
            }

            RaiseTrainEnd(maxEpochs, error);
        }

        public void TrainAll(double[][] data, int maxEpochs, int epochMultiplier)
        {
            double error;
            Train(data,maxEpochs, out error);
        }




        public double[][] Reconstruct(double[][] data)
        {
            var hl = RunVisible(data);
            return RunHidden(hl);
        }
    }

    public delegate void EpochEventHandler(object sender, EpochEventArgs e);
    public class EpochEventArgs : EventArgs
    {
        public int SequenceNumber;
        public double Error;

        public EpochEventArgs(int sequenceNum, double error)
        {
            SequenceNumber = sequenceNum;
            Error = error;
        }
    }
}
