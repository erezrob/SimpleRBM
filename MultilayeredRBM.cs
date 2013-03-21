using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeepLearn
{
    public class MultilayeredRBM : IMultilayeredRBM
    {
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

        private RBM[] m_rbms;
       
        public MultilayeredRBM(int[] layerSizes, double learningRate)
        {
            m_rbms = new RBM[layerSizes.Length - 1];

            for (int i = 0; i < layerSizes.Length - 1; i++)
            {
                var rbm = new RBM(layerSizes[i], layerSizes[i + 1], learningRate);
                rbm.EpochEnd += new EpochEventHandler(rbm_EpochEnd);
                m_rbms[i] = rbm;
            }
        }

        void rbm_EpochEnd(object sender, EpochEventArgs e)
        {
            RaiseEpochEnd(e.SequenceNumber, e.Error);
        }

        public double[][] RunVisible(double[][] data)
        {
            data = m_rbms[0].RunVisible(data);

            for (int i = 0; i < m_rbms.Length - 1; i++)
            {
                data = m_rbms[i + 1].RunVisible(data);
            }

            return data;
        }

        public double[][] RunHidden(double[][] data)
        {
            data = m_rbms[m_rbms.Length-1].RunHidden(data);

            for (int i = m_rbms.Length - 1; i > 0; i--)
            {
                data = m_rbms[i - 1].RunHidden(data);
            }

            return data;
        }

        public double[][] Reconstruct(double[][] data)
        {
            var hl = RunVisible(data);

         //   fitRatio = (double)hl.ToJaggedArray()[0].Count(x => x >= 0.97 || x<=0.03)/hl.ToJaggedArray()[0].Length;

            return RunHidden(hl);
        }


        public double[][] Train( double[][] data, int epochs,int layerNumber ,out double error)
        {
            m_rbms[layerNumber].Train(data, epochs, out error);
            RaiseTrainEnd(error);
            return m_rbms[layerNumber].RunVisible(data);
        }

        public void Train(double[][] data, int epochs, out double error)
        {
            throw new NotSupportedException("User TrainAll or Train specific layer.");
        }

        public void AsyncTrain( double[][] data, int epochs,int layerNumber)
        {
            double error;
            var f = new TaskFactory();
            f.StartNew(new Action(() => Train( data, epochs, layerNumber, out error)));
        }

        public void AsyncTrain(double[][] data, int epochs)
        {
            AsyncTrain(data, epochs, 0);
        }

        public void TrainAll(double[][] visibleData, int epochs, int epochMultiplier)
        {
            double error;

            for (int i = 0; i < m_rbms.Length; i++)
            {
              visibleData = Train( visibleData, epochs + (epochs * i * epochMultiplier),i, out error);
              RaiseTrainEnd(error);
            }
        }

        public void AsyncTrainAll(double[][] visibleData, int epochs, int epochMultiplier)
        {
            TaskFactory f = new TaskFactory();
            f.StartNew(new Action(() => TrainAll(visibleData, epochs, epochMultiplier)));
        }
    }
}
