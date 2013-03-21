using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeepLearn
{
    //public class DualLayerRBM
    //{
    //    public event EpochEventHandler EpochEnd;
    //    public void RaiseEpochEnd(int seq, double err)
    //    {
    //        if (EpochEnd != null)
    //            EpochEnd(this, new EpochEventArgs(seq, err));
    //    }

    //    private readonly int m_numVisible;
    //    private readonly int m_numFirstHidden;
    //    private readonly int m_numSecondHidden;

    //    private RBM m_shallowRbm;
    //    private RBM m_deepRbm;

    //    public DualLayerRBM(int numVisible, int numFirstHidden, int numSecondHidden, double learningRate)
    //    {
    //        m_numVisible = numVisible;
    //        m_numFirstHidden = numFirstHidden;
    //        m_numSecondHidden = numSecondHidden;

    //        m_shallowRbm = new RBM(numVisible, numFirstHidden, learningRate);
    //        m_deepRbm = new RBM(numFirstHidden, numSecondHidden, learningRate);

    //        m_shallowRbm.EpochEnd += new EpochEventHandler(m_shallowRbm_EpochEnd);
    //        m_deepRbm.EpochEnd += new EpochEventHandler(m_shallowRbm_EpochEnd);
    //    }

        
    //    void m_shallowRbm_EpochEnd(object sender, EpochEventArgs e)
    //    {
    //        RaiseEpochEnd(e.SequenceNumber, e.Error);
    //    }

    //    public void AsyncTrainShallow(RealMatrix data, int maxEpochs)
    //    {
    //        double e = 0;
    //        TaskFactory f = new TaskFactory();
    //        f.StartNew(new Action(() => TrainShallow(data, maxEpochs, out e)));
    //    }

    //    public void AsyncTrainDeep(RealMatrix shallowData, int maxEpochs)
    //    {
    //        var data = m_shallowRbm.RunVisible(shallowData);
    //        double e = 0;
    //        TaskFactory f = new TaskFactory();
    //        f.StartNew(new Action(() => TrainDeep(data, maxEpochs, out e)));
    //    }

    //    public void TrainShallow(RealMatrix data, int maxEpochs, out double error)
    //    {
    //        m_shallowRbm.Train(data, maxEpochs, out error);
    //    }

    //    public void TrainDeep(RealMatrix data, int maxEpochs, out double error)
    //    {
    //        m_deepRbm.Train(data, maxEpochs, out error);
    //    }

    //    public RealMatrix RunVisible(RealMatrix data)
    //    {
    //       return m_deepRbm.RunVisible(m_shallowRbm.RunVisible(data));
    //    }

    //    public RealMatrix RunHidden(RealMatrix data)
    //    {
    //        return m_shallowRbm.RunHidden(m_deepRbm.RunHidden(data));
    //    }

    //    public double[] Reconstruct(double[] sample)
    //    {
    //        return RunHidden(RunVisible(new RealMatrix(new[]{sample}))).ToJaggedArray()[0];
    //    }
    //}
}
