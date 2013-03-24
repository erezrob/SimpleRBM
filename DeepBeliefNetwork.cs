using System;
using System.Threading.Tasks;

namespace DeepLearn
{
    /// <summary>
    /// Simple Deep Belief Network - Built on multiple restricted boltzmann machines
    /// </summary>
    public class DeepBeliefNetwork : IDBN
    {
        #region Events
        public event EpochEventHandler EpochEnd;
        public void RaiseEpochEnd(int seq, double err)
        {
            if (EpochEnd != null)
                EpochEnd(this, new EpochEventArgs(seq, err));
        }

        public event EpochEventHandler TrainEnd;
        public void RaiseTrainEnd(double error)
        {
            if (EpochEnd != null)
                EpochEnd(this, new EpochEventArgs(0, error));
        } 
        #endregion

        #region Private Members
        private readonly RBM[] m_rbms; 
        #endregion

        #region Ctors
        public DeepBeliefNetwork(int[] layerSizes, double learningRate)
        {
            m_rbms = new RBM[layerSizes.Length - 1];

            for (int i = 0; i < layerSizes.Length - 1; i++)
            {
                var rbm = new RBM(layerSizes[i], layerSizes[i + 1], learningRate);
                rbm.EpochEnd += OnRbm_EpochEnd;
                m_rbms[i] = rbm;
            }
        } 
        #endregion

        #region Private Methods
        private void OnRbm_EpochEnd(object sender, EpochEventArgs e)
        {
            RaiseEpochEnd(e.SequenceNumber, e.Error);
        } 
        #endregion

        #region Public Methods
        public double[][] Encode(double[][] data)
        {
            data = m_rbms[0].GetHiddenLayer(data);

            for (int i = 0; i < m_rbms.Length - 1; i++)
            {
                data = m_rbms[i + 1].GetHiddenLayer(data);
            }

            return data;
        }

        public double[][] Decode(double[][] data)
        {
            data = m_rbms[m_rbms.Length - 1].GetVisibleLayer(data);

            for (int i = m_rbms.Length - 1; i > 0; i--)
            {
                data = m_rbms[i - 1].GetVisibleLayer(data);
            }

            return data;
        }

        public double[][] Reconstruct(double[][] data)
        {
            var hl = Encode(data);
            return Decode(hl);
        }

        public double[][] DayDream(int numOfDreams)
        {
            var dreamRawData = Distributions.UniformRandromMatrixBool(numOfDreams, m_rbms[0].NumberOfVisibleElements);

            var ret = Reconstruct(dreamRawData);

            return ret;
        }

        public double[][] Train(double[][] data, int epochs, int layerNumber, out double error)
        {
            m_rbms[layerNumber].Train(data, epochs, out error);
            RaiseTrainEnd(error);
            return m_rbms[layerNumber].GetHiddenLayer(data);
        }

        public void AsyncTrain(double[][] data, int epochs, int layerNumber)
        {
            double error;
            var f = new TaskFactory();
            f.StartNew(new Action(() => Train(data, epochs, layerNumber, out error)));
        }

        public void TrainAll(double[][] visibleData, int epochs, int epochMultiplier)
        {
            double error;

            for (int i = 0; i < m_rbms.Length; i++)
            {
                visibleData = Train(visibleData, epochs, i, out error);
                epochs = epochs * epochMultiplier;
                RaiseTrainEnd(error);
            }
        }

        public void AsyncTrainAll(double[][] visibleData, int epochs, int epochMultiplier)
        {
            var f = new TaskFactory();
            f.StartNew(() => TrainAll(visibleData, epochs, epochMultiplier));
        } 
        #endregion
    }
}
