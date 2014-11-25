using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BluetoothController
{
    public interface ILogger
    {
        void StartLogging();
        void StopLogging();
    }
}
