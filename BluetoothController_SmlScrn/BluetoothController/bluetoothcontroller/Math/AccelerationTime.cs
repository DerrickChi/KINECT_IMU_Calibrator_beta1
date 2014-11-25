using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BluetoothController
{
    /// <summary>
    /// Buffer which stores acceleration data with respective time stamps
    /// </summary>
    class AccelerationTime
    {
        private double[] linearAcc;
        private double[] angularAcc;
        private long time;
        private bool isgood = true;

        public AccelerationTime()
        {
            linearAcc = new double[] { 0, 0, 0 };
            angularAcc = new double[] { 0, 0, 0 }; 
            time = 0;

        }

        /// <summary>
        /// Places new acceleration data and respective time stamps into a buffer
        /// </summary>
        /// <param name="newLinearAcc"></param>
        /// <param name="newTime"></param>
        public void setVal(double[] newLinearAcc, long newTime, bool good = true)
        {
            linearAcc = newLinearAcc;
            time = newTime;
            this.isgood = good;
        }

        /// <summary>
        /// Places new linear and angular acceleration data with respective time stamps into a buffer
        /// </summary>
        /// <param name="newLinearAcc"></param>
        /// <param name="newTime"></param>
        public void setVal(double[] newLinearAcc, double[] newAngularAcc, long newTime, bool good = true)
        {
            linearAcc = newLinearAcc;
            angularAcc = newAngularAcc;
            time = newTime;
            isgood = good;
        }

        public double[] linearAcceleration
        {
            get { return linearAcc; }
        }

        public double[] angularAcceleration
        {
            get { return angularAcc; }
        }

        public long Time
        {
            get { return time; }
        }
        public bool IsGood
        {
            get { return this.isgood; }
        }
        

    }
}