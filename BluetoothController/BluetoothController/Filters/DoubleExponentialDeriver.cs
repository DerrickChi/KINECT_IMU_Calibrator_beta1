using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BluetoothController
{
    public class DoubleExponentialDeriver : IFilter
    {
        private ContiniousFilter FirstDeriver;
        private ContiniousFilter SecondDeriver;
        private DoubleExponentialFirst FirstSmoother;
        private DoubleExponentialFirst SecondSmoother;
        private DoubleExponentialFirst ThirdSmoother;
        private NDerivative DerivativeOrder;

        /// <summary>
        /// The first two parameters control smoothing and the third controls how many computations of derivations with smoothing will occur
        /// </summary>
        /// <param name="alpha">Dependency on current/last point</param>
        /// <param name="gamma">Dependency on trend</param>
        /// <param name="nd">Number of derivative/smoothing computations</param>
        public DoubleExponentialDeriver(double[] alpha, double[] gamma, NDerivative nd)
        {
            FirstDeriver = new ContiniousFilter();
            SecondDeriver = new ContiniousFilter();
            FirstSmoother = new DoubleExponentialFirst(alpha[0], gamma[0]);
            SecondSmoother = new DoubleExponentialFirst(alpha[1], gamma[1]);
            ThirdSmoother = new DoubleExponentialFirst(alpha[2], gamma[2]);
            DerivativeOrder = nd;
        }

        /// <summary>
        /// Inputs raw position data into the DoubleExponentialFirst buffer
        /// </summary>
        /// <param name="newVal"></param>
        /// <param name="newTime"></param>
        /// <returns></returns>
        public IFilter UpdateVal(double newVal, long newTime)
        {
            Tuple<double, long> newData = new Tuple<double, long>(newVal, newTime);
            //inputs the raw data into filter buffer to be smoothed later 
            switch (DerivativeOrder)
            {
                case NDerivative.smooth:
                    {
                        FirstSmoother.UpdateVal(new Tuple<double,long>(newVal,newTime));
                        break;
                    }
                case NDerivative.first:
                    {
                        FirstSmoother.UpdateVal(newVal, newTime);
                        FirstDeriver.UpdateVal(FirstSmoother.SmoothDataPoints());
                        SecondSmoother.UpdateVal(FirstDeriver.Derive());
                        break;
                    }
                case NDerivative.second:
                    {
                        FirstSmoother.UpdateVal(newVal, newTime);
                        FirstDeriver.UpdateVal(FirstSmoother.SmoothDataPoints());
                        SecondSmoother.UpdateVal(FirstDeriver.Derive());
                        SecondDeriver.UpdateVal(SecondSmoother.SmoothDataPoints());
                        ThirdSmoother.UpdateVal(SecondDeriver.Derive());
                        break;
                    }
            }
            return this;
        }

        /// <summary>
        /// Returns smoothed data for position, velocity, and acceleration
        /// </summary>
        /// <param name="nd"></param>
        /// <returns></returns>
        public Tuple<double, long> GetNDerivative(NDerivative nd)
        {
            Tuple<double, long> smoothData = null;

            //Derive and smooth the raw position data 
            switch (nd)
            {
                case NDerivative.smooth:
                    {
                        if (DerivativeOrder != NDerivative.smooth)
                            throw new Exception("Must initialize class to zero(smooth) derivative order.");
                        smoothData = FirstSmoother.SmoothDataPoints();
                        break;
                    }
                case NDerivative.first:
                    {
                        if (DerivativeOrder != NDerivative.first)
                            throw new Exception("Must initialize class to first derivative order.");
                        smoothData = SecondSmoother.SmoothDataPoints();
                        break;
                    }
                case NDerivative.second:
                    {
                        if (DerivativeOrder != NDerivative.second)
                            throw new Exception("Must initialize class to second derivative order.");
                        smoothData = ThirdSmoother.SmoothDataPoints();
                        //smoothData = SecondDeriver.Derive();
                        break;
                    }
            }
            return smoothData;
        }
    }
}
