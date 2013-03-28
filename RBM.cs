using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeepLearn
{
    /// <summary>
    /// Simple Restricted Boltzmann Machine implementation
    /// </summary>
    public class RBM : IRBM
    {
        #region Events
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
        #endregion

        #region Private fields
        private readonly int m_numHiddenElements;
        private readonly int m_numVisibleElements;
        private readonly double m_learningRate;
        private RealMatrix m_weights; 
        #endregion

        #region Public Properties
        public int NumberOfVisibleElements { get { return m_numVisibleElements; } }
        #endregion
       
        public RBM(int numVisible, int numHidden, double learningRate = 0.1)
        {
            m_numHiddenElements = numHidden;
            m_numVisibleElements = numVisible;
            m_learningRate = learningRate;

            m_weights = 0.1*Distributions.GaussianMatrix(numVisible, numHidden);
            m_weights = m_weights.InsertRow(0);
            m_weights = m_weights.InsertCol(0);
        }

        #region Public Methods

        /// <summary>
        /// Get the hidden layer features from a visible layer
        /// </summary>
        /// <param name="dataArray"></param>
        /// <returns></returns>
        public double[][] GetHiddenLayer(double[][] dataArray)
        {
            var num_examples = dataArray.Length;

            // Create a matrix, where each row is to be the hidden units (plus a bias unit)
            // sampled from a training example.
            var hidden_states = RealMatrix.Ones(num_examples, m_numHiddenElements + 1);

            var data = new RealMatrix(dataArray);
            // Insert bias units of 1 into the first column of data.
            data = data.InsertCol(1); // np.insert(data, 0, 1, axis = 1)

            // Calculate the activations of the hidden units.
            var hiddenActivations = data * m_weights;
            // Calculate the probabilities of turning the hidden units on.
            var hiddenProbs = ActivationFunctions.Logistic(hiddenActivations);
            // Turn the hidden units on with their specified probabilities.
            hidden_states = hiddenProbs > Distributions.UniformRandromMatrix(num_examples, m_numHiddenElements + 1);

            // Ignore the bias units.
            hidden_states = hidden_states.RemoveFirstCol(); //  hidden_states[:,1:]
            return hidden_states;
        }

        /// <summary>
        /// Get the visible layer from a hidden layer
        /// </summary>
        /// <param name="dataArray"></param>
        /// <returns></returns>
        public double[][] GetVisibleLayer(double[][] dataArray)
        {
            var numExamples = dataArray.Length;

            // Create a matrix, where each row is to be the visible units (plus a bias unit)
            // sampled from a training example.

            var data = new RealMatrix(dataArray);
            // Insert bias units of 1 into the first column of data.
            data = data.InsertCol(1);

            // Calculate the activations of the visible units.
            var visibleActivations = data * m_weights.Transpose;
            // Calculate the probabilities of turning the visible units on.
            var visibleProbs = ActivationFunctions.Logistic(visibleActivations);
            // Turn the visible units on with their specified probabilities.
            var visibleStates = visibleProbs > Distributions.UniformRandromMatrix(numExamples, m_numVisibleElements + 1);

            // Ignore the bias units.
            visibleStates = visibleStates.RemoveFirstCol(); //visible_states[:,1:]
            return visibleStates;
        }

        /// <summary>
        /// Day dream - Reconstruct a randrom matrix (An interesting way of seeing strong features the machine has learnt)
        /// </summary>
        /// <param name="numberOfSamples">How many images/dreams</param>
        /// <returns>Array of Reconstructed dreams</returns>
        public double[][] DayDream(int numberOfSamples)
        {
            var data = RealMatrix.Ones(numberOfSamples, m_numVisibleElements + 1);
            data.Update(0, 1, Distributions.UniformRandromMatrixBool(1, m_numVisibleElements), 1);

            for (int i = 0; i < numberOfSamples; i++)
            {
                var visible = data.Submatrix(i, 0, 1).ToVector();
                var hidden_activations = (visible*m_weights).ToVector();
                var hidden_probs = ActivationFunctions.Logistic(hidden_activations);
                var hidden_states = hidden_probs > RVector.Random(m_numHiddenElements + 1);
                hidden_states[0] = 1;

                var visible_activations = (hidden_states*m_weights.Transpose).ToVector();
                var visible_probs = ActivationFunctions.Logistic(visible_activations);
                var visible_states = visible_probs > RVector.Random(m_numVisibleElements + 1);
                data.Update(visible_states, 0, false, i, 0);
            }

            return data.Submatrix(0, 1).ToArray();
        }
    
        public void AsyncTrain(double[][] data, int maxEpochs)
        {
            double e = 0;
            var f = new TaskFactory();
            f.StartNew(new Action(() => Train(data, maxEpochs, out e)));
        }

        public void Train(double[][] dataArray, int maxEpochs, out double error)
        {
            error = 0;

            var numExamples = dataArray.Length;
            var data = new RealMatrix(dataArray);

            data = data.InsertCol(1);
            Stopwatch sw = new Stopwatch();

            for (int i = 0; i < maxEpochs; i++)
            {
                sw.Start();
                var posHiddenActivations = data * m_weights;
                var posHiddenProbs = ActivationFunctions.Logistic(posHiddenActivations);
                var posHiddenStates = posHiddenProbs > Distributions.UniformRandromMatrix(numExamples, m_numHiddenElements + 1);
                var posAssociations = data.Transpose * posHiddenProbs;

                var negVisibleActivations = posHiddenStates * m_weights.Transpose;
                var negVisibleProbs = ActivationFunctions.Logistic(negVisibleActivations);

                negVisibleProbs = negVisibleProbs.Update(0, 1, 1);
                var negHiddenActivations = negVisibleProbs * m_weights;
                var negHiddenProbs = ActivationFunctions.Logistic(negHiddenActivations);

                var negAssociations = negVisibleProbs.Transpose * negHiddenProbs;

                m_weights = m_weights + (m_learningRate * ((posAssociations - negAssociations) / numExamples));

                sw.Stop();
                error = ((data - negVisibleProbs) ^ 2).Sum();
                RaiseEpochEnd(i, error);
                Console.WriteLine("Epoch {0}: error is {1}, computation time (ms): {2}", i, error,sw.ElapsedMilliseconds);
                sw.Reset();
            }

            RaiseTrainEnd(maxEpochs, error);
        }
        
        public double[][] Reconstruct(double[][] data)
        {
            var hl = GetHiddenLayer(data);
            return GetVisibleLayer(hl);
        }

        public RealMatrix GetWeights()
        {
            return new RealMatrix(m_weights); //Make a Copy
        }
        #endregion
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
