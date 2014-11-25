using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Kinect;

namespace BluetoothController
{
    public class OurSavitzkyGolayFilter : IFilter
    {
        private Tuple<double,long>[] rawPositionData;
        //private Tuple<double,long>[] smoothPositionData;
        private int windowSize;
        private const int polynomialOrder = 3;

        public OurSavitzkyGolayFilter(int numberOfPoints)
        {
            windowSize = numberOfPoints;
            rawPositionData = new Tuple<double,long>[windowSize];
            for (int i = 0; i < windowSize; i++)
                rawPositionData[i] = new Tuple<double, long>(double.NaN, 0);
        }

        public IFilter UpdateVal(double newPosition, long newTime)
        {
            Tuple<double, long> newVal = new Tuple<double, long>(newPosition, newTime);
            for (int i = 0; i < windowSize - 1; i++)
                rawPositionData[i] = rawPositionData[i + 1];
            rawPositionData[windowSize - 1] = newVal;
            return this;
        }

        //public enum NDerivative
        //{ first, second}

        public Tuple<double, long> GetNDerivative(NDerivative nd)
        {
            for (int i = 0; i < windowSize; i++)
                if (rawPositionData[i].Item1 == double.NaN)
                    return null;

            //averageTimeSpan
            long sum = 0;
            long hj = 0;
            double h;
            double NDerivativeVal = 0;

            for (int i = 0; i < windowSize - 1; i++)
                sum += rawPositionData[i + 1].Item2 - rawPositionData[i].Item2;

            hj = sum / (windowSize - 1);
            h = (double)hj * (1.0 / TimeSpan.TicksPerSecond);

            if (windowSize == 5)
            {
                if (nd == NDerivative.second)
                    NDerivativeVal = (1.0 / (7.0 * h * h)) * (2.0 * rawPositionData[0].Item1 - rawPositionData[1].Item1 - 2.0 * rawPositionData[2].Item1 - rawPositionData[3].Item1 + 2.0 * rawPositionData[4].Item1);
                else if (nd == NDerivative.first)
                    NDerivativeVal = (1.0 / (12.0 * h)) * (rawPositionData[0].Item1 - 8.0 * rawPositionData[1].Item1 + 8.0 * rawPositionData[3].Item1 - rawPositionData[4].Item1);
                else
                    NDerivativeVal = (1.0 / 35.0) * (-3.0 * rawPositionData[0].Item1 + 12.0 * rawPositionData[1].Item1 + 17.0 * rawPositionData[2].Item1 + 12.0 * rawPositionData[3].Item1 + -3.0 * rawPositionData[4].Item1);
            }

            if (windowSize == 7)
                if (nd == NDerivative.second)
                    NDerivativeVal = (1.0 / (42.0 * h * h)) * (5.0 * rawPositionData[0].Item1 - 3.0 * rawPositionData[2].Item1 - 4.0 * rawPositionData[3].Item1 - 3.0 * rawPositionData[4].Item1 + 5.0 * rawPositionData[6].Item1);

            if (windowSize == 9)
                if (nd == NDerivative.second)
                    NDerivativeVal = (1.0 / (462.0 * h * h)) * (28.0 * rawPositionData[0].Item1 + 7.0 * rawPositionData[1].Item1 - 8.0 * rawPositionData[2].Item1 - 17.0 * rawPositionData[3].Item1 - 20.0 * rawPositionData[4].Item1 - 17.0 * rawPositionData[5].Item1 - 8.0 * rawPositionData[6].Item1 + 7.0 * rawPositionData[7].Item1 + 20.0 * rawPositionData[8].Item1);

            Tuple<double, long> NthDerivative = new Tuple<double, long>(NDerivativeVal, rawPositionData[windowSize/2].Item2);

            return NthDerivative;
        }
    }
}
