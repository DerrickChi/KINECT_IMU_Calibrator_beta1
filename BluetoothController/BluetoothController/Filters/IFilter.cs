using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BluetoothController
{
    public enum NDerivative {smooth, first, second}
    public interface IFilter
    {
        IFilter UpdateVal(double newVal, long newTime);
        Tuple<double, long> GetNDerivative(NDerivative nd);
    }


}
