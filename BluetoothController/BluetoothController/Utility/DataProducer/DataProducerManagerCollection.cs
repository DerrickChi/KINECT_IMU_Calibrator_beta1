using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BluetoothController.Kinect;
using Microsoft.Kinect;
using InTheHand.Net;

namespace BluetoothController
{
    public class DataProducerManagerCollection : IDisposable
    {
        private static int RawIntertialSensorUpdateFreq { get { return 20; } }
        public ConcurrentDictionary<string, DataProducerManager> DPMCollection = new ConcurrentDictionary<string, DataProducerManager>();
        private ConcurrentDictionary<string, UISensorData> UISensorDataDictionary = new ConcurrentDictionary<string, UISensorData>();
        public ConcurrentDictionary<string, UISensorData> UISensors
        {get { return this.UISensorDataDictionary; } }
        SensorGrapherControl SensorGrapher;
        private List<Action> MethodsToCallOnDispose = new List<Action>();


        public DataProducerManagerCollection(SensorGrapherControl sg)
        {
            this.SensorGrapher = sg;
        }


        #region Kinect Sensor
        public DataProducerManager AddKinectSensor(bool IsReconnectable = false)
        {
            DataProducerManager KinectDpm = new DataProducerManager(this)
            {
                IDataProducer = new KinectProducer(new JointType[] { JointType.HandLeft, JointType.HandRight }),
                UniqueDeviceAddress = KinectProducer.KinectAddress,
                UISensorData = new UISensorData(false) { SensorLocation = SensorLocation.NotApplicable, DataPreview = "Loading, Please wait :)", MPS = -1, DeviceName = KinectProducer.KinectAddress, SensorType = SensorType.Kinect }
            };
            this.DPMCollection[KinectProducer.KinectAddress] = KinectDpm;
            (KinectDpm.IDataProducer as IRestartable).Start();
            this.MethodsToCallOnDispose.Add((Action)delegate() { ((IDisposable)KinectDpm.IDataProducer).Dispose(); });
            return KinectDpm;
        }
        #endregion

        #region Raw Inertial Sensor
        public DataProducerManager AddRawInertialSensor(BluetoothAddress btaddress, bool isreconnectable = false)
        {
            if (btaddress == null)
                throw new ArgumentException(btaddress.GetType().Name + " Cannot be null.");
            DataProducerManager RawInertialSensor;
            try
            {
                RawInertialSensor = new DataProducerManager(this)
                {
                    UniqueDeviceAddress = btaddress.ToString(),
                    IDataProducer = new RawInertialSensorProducer(btaddress),
                    UISensorData = new UISensorData(true) { DeviceName = btaddress.ToString(), SensorType = SensorType.Inertial, MPS = -1, DataPreview = "Loading, Please wait :)", SensorLocation = SensorLocation.NotSet },
                    ChartInfo = this.SensorGrapher.SetupChartAreaFor(btaddress.ToString(), SensorGrapherControl.ChartTypes.Acceleration, DataProducerManagerCollection.RawIntertialSensorUpdateFreq)
                };
            }
            catch
            {
                return null;
            }
            RawInertialSensor.IDataProducer.NewIData += (senderNewIData, eNewIData) =>
                {
                    this.UISensorDataDictionary[RawInertialSensor.ChartInfo.RootName].DataPreview = eNewIData.ToPreviewString();
                    this.SensorGrapher.UpdateGraph(RawInertialSensor.ChartInfo, ((InertialSensorData)eNewIData).NormalizedAccelerations, eNewIData.NowInTicks);
                };
            RawInertialSensor.IDataProducer.MeasuresPerSec += (senderMeasuresPerSec, eMeasuresPerSec) =>
                {
                    RawInertialSensor.UISensorData.MPS = eMeasuresPerSec;
                };
            RawInertialSensor.UISensorData.SensorLocationSet += (senderSensorLocationSet, eSensorLocationSet) =>
                {
                    foreach (var child in RawInertialSensor.DPMChildrenList.Where(x => x.IDataProducer.SensorType == SensorType.Virtual))
                    {
                        RemoveDataProducerManager(child);
                    }
                    //Note this can't be in the foreach b/c you can't remove from an Ienumerable collection


                    RawInertialSensor.DPMChildrenList.RemoveAll(x => x.IDataProducer.SensorType == SensorType.Virtual);
                    DataProducerManager tMappedVirtualInertialSensor = null;
                    var tVirtualInertialSensor = this.AddVirtualInertialSensor(RawInertialSensor, this.DPMCollection[KinectProducer.KinectAddress], eSensorLocationSet);
                   
                    //check that the sensor was actually created, if it was add it
                    if (tVirtualInertialSensor != null)
                    {
                        RawInertialSensor.DPMChildrenList.Add(tVirtualInertialSensor);
                        tMappedVirtualInertialSensor = this.AddMappedVirtualInertialSensor(RawInertialSensor, this.DPMCollection[KinectProducer.KinectAddress], tVirtualInertialSensor, eSensorLocationSet.ToJointType());
                        if (tMappedVirtualInertialSensor != null)
                            RawInertialSensor.DPMChildrenList.Add(tMappedVirtualInertialSensor);
                    }
                };
            this.DPMCollection[btaddress.ToString()] = RawInertialSensor;
            (RawInertialSensor.IDataProducer as IRestartable).Start();
            this.MethodsToCallOnDispose.Add((Action)delegate() { RawInertialSensor.IDataProducer.Dispose(); });
            return RawInertialSensor;
        }
        #endregion

        #region Binded Inertial Sensor
        public DataProducerManager AddMappedBindedInertialSensor(DataProducerManager rawintertialsensor, IMapper mapper)
        {
            if (rawintertialsensor == null)
                throw new ArgumentException("Cannot be null.", rawintertialsensor.GetType().Name);
            if (mapper == null)
                throw new ArgumentException("Cannot be null.", mapper.GetType().Name);

            DataProducerManager MappedBindedIertialSensorDPM = new DataProducerManager(this)
            {
                IDataProducer = new MappedBindedInertialSesnorProducer((IDataProducer<InertialSensorData >) rawintertialsensor.IDataProducer, mapper),
            };
            MappedBindedIertialSensorDPM.UniqueDeviceAddress = MappedBindedIertialSensorDPM.IDataProducer.DeviceAddress;
            MappedBindedIertialSensorDPM.UISensorData = new UISensorData(false) { DataPreview = "Loading, Please wait :)", SensorLocation = SensorLocation.NotApplicable, MPS = -1, DeviceName = MappedBindedIertialSensorDPM.UniqueDeviceAddress, SensorType = SensorType.MappedIntertial };
            MappedBindedIertialSensorDPM.ChartInfo = this.SensorGrapher.SetupChartAreaFor(MappedBindedIertialSensorDPM.UniqueDeviceAddress, BluetoothController.SensorGrapherControl.ChartTypes.Acceleration, DataProducerManagerCollection.RawIntertialSensorUpdateFreq);

            MappedBindedIertialSensorDPM.IDataProducer.NewIData += (senderNewIData, eNewIData) =>
                {
                    this.UISensorDataDictionary[MappedBindedIertialSensorDPM.ChartInfo.RootName].DataPreview = eNewIData.ToPreviewString();
                    this.SensorGrapher.UpdateGraph(MappedBindedIertialSensorDPM.ChartInfo, ((InertialSensorData)eNewIData).NormalizedAccelerations, eNewIData.NowInTicks);
                };




            rawintertialsensor.DPMChildrenList.Add(MappedBindedIertialSensorDPM);
            this.DPMCollection[MappedBindedIertialSensorDPM.UniqueDeviceAddress] = MappedBindedIertialSensorDPM;
            return MappedBindedIertialSensorDPM;
        }
        #endregion

        #region Virtual Inertial
        public DataProducerManager AddVirtualInertialSensor(DataProducerManager btproducer, DataProducerManager kinectsensor, SensorLocation sl)
        {
            DataProducerManager VirtualSensorDPM = new DataProducerManager(this)
            { 
                IDataProducer = VirtualSensorProducer.GetVirtualSensorHelper((IDataProducer<KinectData>)kinectsensor.IDataProducer, sl),
            };
            if (VirtualSensorDPM.IDataProducer == null)
                return null;
            VirtualSensorDPM.UniqueDeviceAddress = VirtualSensorDPM.IDataProducer.DeviceAddress + "-" + btproducer.UniqueDeviceAddress + "-" + sl;
            VirtualSensorDPM.UISensorData = new UISensorData(false) { DataPreview = "Loading, Please wait :)", SensorLocation = SensorLocation.NotApplicable, DeviceName = VirtualSensorDPM.UniqueDeviceAddress, MPS = -1, SensorType = SensorType.Virtual };
            VirtualSensorDPM.ChartInfo = this.SensorGrapher.SetupChartAreaFor(VirtualSensorDPM.UniqueDeviceAddress, BluetoothController.SensorGrapherControl.ChartTypes.Acceleration, 2);
            VirtualSensorDPM.IDataProducer.NewIData += (senderNewIData, eNewIData) =>
                {
                    VirtualSensorDPM.UISensorData.DataPreview = eNewIData.ToPreviewString();
                    this.SensorGrapher.UpdateGraph(VirtualSensorDPM.ChartInfo, ((VirtualSensorData)eNewIData).acceleration, eNewIData.NowInTicks);

                };
            this.DPMCollection[VirtualSensorDPM.UniqueDeviceAddress] = VirtualSensorDPM;
            return VirtualSensorDPM;

        }
        #endregion

        #region Mapped Virtual Inertial
        public DataProducerManager AddMappedVirtualInertialSensor(DataProducerManager btproducer, DataProducerManager kinectsensor, DataProducerManager virutalsensor, JointType jt)
        {
            DataProducerManager MappedVirtualSensorDPM = new DataProducerManager(this)
            {
                IDataProducer = new MappedVirtualSensorProducer(kinectsensor.IDataProducer as IDataProducer<KinectData>,
                    virutalsensor.IDataProducer as IDataProducer<VirtualSensorData>, jt)
            };
            MappedVirtualSensorDPM.UniqueDeviceAddress = "Mapped-" + virutalsensor.UniqueDeviceAddress;
            MappedVirtualSensorDPM.UISensorData = new UISensorData(false) { DataPreview = "Loading, Please wait :)", SensorLocation = SensorLocation.NotApplicable, DeviceName = MappedVirtualSensorDPM.UniqueDeviceAddress, MPS = -1, SensorType = SensorType.MappedVirtual };
            MappedVirtualSensorDPM.ChartInfo = this.SensorGrapher.SetupChartAreaFor(MappedVirtualSensorDPM.UniqueDeviceAddress, SensorGrapherControl.ChartTypes.Acceleration, 2);
            MappedVirtualSensorDPM.IDataProducer.NewIData += (senderNewIData, eNewIData) =>
                {
                    MappedVirtualSensorDPM.UISensorData.DataPreview = eNewIData.ToPreviewString();
                    this.SensorGrapher.UpdateGraph(MappedVirtualSensorDPM.ChartInfo, (eNewIData as VirtualSensorData).acceleration, eNewIData.NowInTicks);
                };
            this.DPMCollection[MappedVirtualSensorDPM.UniqueDeviceAddress] = MappedVirtualSensorDPM;
            return MappedVirtualSensorDPM;
        }
        #endregion

        public void RemoveDataProducerManager(DataProducerManager dpm)
        {
            dpm.IDataProducer.Dispose();
            this.SensorGrapher.RemoveChartViaChartInfo(dpm.ChartInfo);
            UISensorData tUISD;
            this.UISensorDataDictionary.TryRemove(dpm.UniqueDeviceAddress, out tUISD);
            DataProducerManager tDPM;
            this.DPMCollection.TryRemove(dpm.UniqueDeviceAddress, out tDPM);
            //this.UISensorDataDictionary[dpm.UniqueDeviceAddress] = null;
        }



        public DataProducerManager this[string uniquedeviceaddress]
        { get { return this.DPMCollection[uniquedeviceaddress]; } }
        //public ConcurrentDictionary<string, DataProducerManager> this {return this.}
        #region IDisposable Members

        public void Dispose()
        {
            foreach (Action a in this.MethodsToCallOnDispose)
                a();
        }

        #endregion
    }
}
