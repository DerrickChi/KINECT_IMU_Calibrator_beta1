using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SensorCalibration
{
    class AccelerationTime
    {
        private double[] linearAcc;
        private double[] angularAcc;
        private long time;

        public AccelerationTime()
        {
            linearAcc = new double[] { 0, 0, 0 };
            angularAcc = new double[] { 0, 0, 0 }; 
            time = 0;
        }

        public void setVal(double[] newLinearAcc, long newTime)
        {
            linearAcc = newLinearAcc;
            time = newTime;
        }

        public void setVal(double[] newLinearAcc, double[] newAngularAcc, long newTime)
        {
            linearAcc = newLinearAcc;
            angularAcc = newAngularAcc;
            time = newTime;
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
        

    }
}