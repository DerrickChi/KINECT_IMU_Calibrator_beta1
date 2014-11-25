using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BluetoothController
{
    /// <summary>
    /// Performs a central derivation on incoming data
    /// </summary>
    public class ContiniousFilter
    {
        double[] values;
        long[] timeStamps;
        int size;
        public bool active { get; protected set; }

        public ContiniousFilter()
        {
            active = false;
            values = new double[] { 0, 0, 0 };
            timeStamps = new long[] { 0, 0, 0 };
            size = 0;
        }

        public long[] time
        {
            get { return timeStamps; }
            protected set { timeStamps = value; }
        }

        /// <summary>
        /// Inputs data(position, velocity, and acceleration) with corresponding time into buffer and removes oldest data
        /// </summary>
        /// <param name="d"></param>
        /// <param name="l"></param>
        /// <returns></returns>
        public ContiniousFilter UpdateVal(double d, long l) { return this.UpdateVal(new Tuple<double, long>(d, l)); }
        /// <summary>
        /// Inputs data(position, velocity, and acceleration) with corresponding time into buffer and removes oldest data
        /// </summary>
        /// <param name="d"></param>
        /// <param name="l"></param>
        /// <returns></returns>
        public ContiniousFilter UpdateVal(Tuple<double, long> newData)
        {
            double newVal = newData.Item1;
            long newTime = newData.Item2;
            for (int i = 0; i < 2; i++)
            {
                values[i] = values[i + 1];
                timeStamps[i] = timeStamps[i + 1];
            }
            values[2] = newVal;
            timeStamps[2] = newTime;

            active = (size >= 3);
            if (!active)
                size++;

            return this;
        }

        /// <summary>
        /// Performs the central derivation on data that is currently stored in buffer
        /// </summary>
        /// <returns></returns>
        public Tuple<double, long> Derive()
        {
            double acc = 0;
            double timeInterval1 = (timeStamps[2] - timeStamps[0])*(1.0 / ((double)TimeSpan.TicksPerSecond)); //first derivative
            
            if (active)
                acc = (values[2] - values[0]) / timeInterval1;

            return new Tuple<double, long>(acc, timeStamps[1]);
        }
    }

}
