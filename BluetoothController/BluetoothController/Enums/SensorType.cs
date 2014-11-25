using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BluetoothController
{
    /// <summary>
    /// This enum represents the type of IDataProducer<T> associated with the IDataProucer
    /// Know what Type the sensor is will enable a person to cast the sensor correctly
    /// </summary>
    public enum SensorType
    {
        Kinect,
        Inertial,
        MappedIntertial,
        Virtual,
        CorrectedInertial,
        MappedVirtual
    }
}
