using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BluetoothController
{
    /// <summary>
    /// This inferface if implemented allows for the functionally to restart, stop and catch any exceptions that are thrown
    /// this class can be used to allow the user to dynamically reconnect or remove devices.
    /// </summary>
    public interface IRestartable
    {
        void Start();
        void Stop();
        void Reset();
        void Restart();
        /// <summary>
        /// If true: the thread is good
        /// </summary>
        bool IsGood { get; }
        /// <summary>
        /// <remarks>Considering removing this as it creates some overhead and allows for multiple methods to read the exception
        /// I think it would be better to only read the exception when it is on the event and respond correctly then</remarks>
        /// </summary>
        Exception Exception { get; }
        event EventHandler<Exception> ExceptionThrown; 
    }
}
