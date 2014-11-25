using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Kinect;
using BluetoothController.Kinect;
using InTheHand.Net.Bluetooth;
using InTheHand.Net.Sockets;
using InTheHand.Net;

namespace BluetoothController
{
    public class DataProducerManager
    {
        protected DataProducerManagerCollection ParentCollection { get; set; }
        public string UniqueDeviceAddress { get; set; }
        public IDataProducer IDataProducer { get; set; }
        private UISensorData UISD;
        public UISensorData UISensorData 
        {
            get
            {
                return UISD;
            }
            set
            {
                this.UISD = ParentCollection.UISensors[this.UniqueDeviceAddress] = value;
            }
        }
        public ChartInfo ChartInfo { get; set; }
        public List<DataProducerManager> DPMChildrenList { get; set; }

        public DataProducerManager(DataProducerManagerCollection parent)
        {
            ParentCollection = parent;
            this.DPMChildrenList = new List<DataProducerManager>();
        }
    }
}



//    public static DataProducerManager AddKinectSensor(bool IsReconnectable = false)
//    {
//        DataProducerManager KinectDpm = new DataProducerManager()
//        {
//            IDataProducer = new KinectProducer(new JointType[] { JointType.HandLeft, JointType.HandRight }),
//            UniqueDeviceAddress = KinectProducer.KinectAddress,
//            UISensorData = new UISensorData() { SensorLocation = SensorLocation.NotApplicable, DataPreview = "Loading, Please wait :)", MPS = -1, DeviceName = KinectProducer.KinectAddress, SensorType = SensorType.Kinect }
//        };
//        return KinectDpm;
//    }

//    public static DataProducerManager AddRawInertialSensor(BluetoothAddress btaddress, SensorGrapherControl sg, bool isreconnectable = false)
//    {
//        if (btaddress == null)
//            throw new ArgumentException(btaddress.GetType().Name + " Cannot be null.");
//        DataProducerManager RawInterntialSensor = new DataProducerManager()
//        {
//            UniqueDeviceAddress = btaddress.ToString(),
//            IDataProducer = new RawInertialSensorProducer(btaddress),
//            UISensorData = new UISensorData(true) { DeviceName = btaddress.ToString(), SensorType = SensorType.Inertial, MPS = -1, DataPreview = "Loading, Please wait :)", SensorLocation = SensorLocation.NotSet },
//            ChartInfo = sg.SetupChartAreaFor(btaddress.ToString(), SensorGrapherControl.ChartTypes.Acceleration, DataProducerManager.RawIntertialSensorUpdateFreq)
//        };




//        return RawInterntialSensor;
//    }

//}

//public class DataProducerManagerCollection
//{

//}
