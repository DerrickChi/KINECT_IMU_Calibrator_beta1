using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BluetoothController
{
    /// <summary>
    /// Smooths data using a double exponential filter
    /// </summary>
    class DoubleExponentialFirst
    {
        private double currentRawPosition;
        private double previousRawPosition;
        private double previousSmoothPosition;
        private double previousB;
        private double alpha;
        private double gamma;
        private int size;
        private long currentTime;
       

        /// <summary>
        /// Choose parameters for smoothing.
        /// </summary>
        /// <param name="newAlpha">Domain: Alpha must be between 0 and 1</param>
        /// <param name="newGamma">Gamma: Alpha must be between 0 and 1</param>
        public DoubleExponentialFirst(double newAlpha, double newGamma)
        {
            alpha = newAlpha;
            gamma = newGamma;
            currentRawPosition = 0;
            previousSmoothPosition = 0;
            previousRawPosition = 0;
            currentTime = 0;
            size = 0;
        }

        /// <summary>
        /// Inputs data(position, velocity, and acceleration) with corresponding time into buffer
        /// </summary>
        /// <param name="d"></param>
        /// <param name="l"></param>
        /// <returns></returns>
        public DoubleExponentialFirst UpdateVal(double d, long l) { return this.UpdateVal(new Tuple<double,long>(d,l));}
        /// <summary>
        /// Inputs data(position, velocity, and acceleration) with corresponding time into buffer
        /// </summary>
        /// <param name="d"></param>
        /// <param name="l"></param>
        /// <returns></returns>
        public DoubleExponentialFirst UpdateVal(Tuple<double, long> newData)
        {
            double newPosition = newData.Item1;
            long newTime = newData.Item2;
            previousRawPosition = currentRawPosition;
            currentRawPosition = newPosition;
            currentTime = newTime;
            if (size < 3)
                size++;
            return this;
        }

        /// <summary>
        /// Applies the smoothing algorithm to the raw data
        /// </summary>
        /// <returns></returns>
        public Tuple<double, long> SmoothDataPoints()
        {
            try
            {
                double currentSmoothPosition = double.NaN;
                double currentB = 0;
                if (size > 2)
                {
                    currentSmoothPosition = alpha * currentRawPosition + (1.0 - alpha) * (previousSmoothPosition + previousB);
                    currentB = gamma * (currentSmoothPosition - previousSmoothPosition) + (1.0 - gamma) * previousB;
                    previousSmoothPosition = currentSmoothPosition;
                    previousB = currentB;
                }
                else if (size == 2)
                {
                    previousB = currentRawPosition - previousRawPosition;
                    currentSmoothPosition = currentRawPosition;
                    previousSmoothPosition = currentSmoothPosition;
                }

                return new Tuple<double, long>(currentSmoothPosition, currentTime);
            }
            catch (Exception e)
            { throw e; }
        }
    }
}
