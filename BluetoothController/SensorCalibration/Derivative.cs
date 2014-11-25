using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SensorCalibration
{
    class Derivative
    {
        double[] values;
        long[] timeStamps;
        int size;
        public bool active { get; protected set; }

        public Derivative()
        {
            active = false;
            values = new double[] { 0, 0, 0, 0, 0 };
            timeStamps = new long[] { 0, 0, 0, 0, 0 };
            size = 0;
        }


        public void updateVal(double newVal, long newTime)
        {

            for (int i = 0; i < 4; i++)
            {
                values[i] = values[i + 1];
                timeStamps[i] = timeStamps[i + 1];
            }
            values[4] = newVal;
            timeStamps[4] = newTime;

            active = (size >= 5);
            if (!active)
                size++;
        }

        public double getDerivative()
        {
            double acc = 0;
            if (active)
                acc = (values[2] - values[0]) / (timeStamps[2] - timeStamps[0]);
            else
                acc = 0;

            return acc;
        }

        public double getDoubleDerivative()
        {
            double acc = 0;
            double v1, v2 = 0;
            if (active)
            {
                v1 = (values[2] - values[0]) / (timeStamps[2] - timeStamps[0]);
                v2 = (values[4] - values[2]) / (timeStamps[4] - timeStamps[2]);
                acc = (v2 - v1) / (timeStamps[3] - timeStamps[1]);
            }
            else
                acc = 0;

            return acc;

        }
    }

}
