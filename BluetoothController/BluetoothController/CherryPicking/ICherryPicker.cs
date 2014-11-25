using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BluetoothController
{
    public static class ICherryPicker
    {
        public const double MaxAcceleration = 8.0;

        public static double[] FilterInferred(double[] data, bool isTracked)
        {
            if (!isTracked)
                for (int i = 0; i < 3; i++)
                    data[i] = double.NaN;
            return data;
        }

        public static double[] FilterHighAcceleration(double[] data)
        {
            bool IsGood = true;
            if (Math.Abs(data[0]) > MaxAcceleration)
                IsGood = false;
            if (Math.Abs(data[1]) > MaxAcceleration + 9.81) //due to gravity offset
                IsGood = false;
            if (Math.Abs(data[2]) > MaxAcceleration)
                IsGood = false;

            return FilterInferred(data, IsGood);
        }

        public static double[] CherryPick(double[] data, bool isTracked)
        {
            double[] filter1 = FilterInferred(data, isTracked);
            return FilterHighAcceleration(filter1);
        }
    }
}
