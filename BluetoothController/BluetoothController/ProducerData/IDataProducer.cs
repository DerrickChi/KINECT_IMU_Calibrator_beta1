using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BluetoothController
{
    /// <summary>
    /// This is the generica IdataProducer. This is primarily so that there can be ONE collection of all IDataProducers.
    /// This collection of IDataProducers will be held by <see cref="DataProducerManagerCollection"/>
    /// </summary>
    public interface IDataProducer : IDisposable
    {
        event EventHandler<IData> NewIData;
        event EventHandler<int> MeasuresPerSec;
        SensorType SensorType { get; }
        Type IDataType { get; }
        string DeviceName { get; }
        string DeviceAddress { get; }
        bool IsIRestartable { get; }
    }
    /// <summary>
    /// This interface contains the typed event.
    /// This allows for more readable code when dealing with cascading sensor or sensors that requrie another sensor to feed data
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IDataProducer<T>: IDataProducer where T : IData 
    {
        event EventHandler<T> NewTData;
    }
}
