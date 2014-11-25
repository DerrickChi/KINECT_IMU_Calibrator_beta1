using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Altaxo.Calc.LinearAlgebra;

namespace BluetoothController
{
    public class Coefficients : IVector
    {
        private double[] coefficients;
        private const int size = 11;

        public Coefficients()
        {
            coefficients = new double[size];
            for (int i = 0; i < size; i++)
                coefficients[i] = 0;
        }

        public double this[int i]
        {
            get
            {
                return coefficients[i];
            }
            set
            {
                coefficients[i] = value;
            }
        }

        public int Length
        {
            get { return size; }
        }
    }
}
