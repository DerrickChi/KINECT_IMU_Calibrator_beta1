using BluetoothController.Kinect;
using Microsoft.Kinect;
using System;
using System.Threading;

namespace BluetoothController
{
    public class ThreadedVirtualSensorProducer :  IDataProducer<VirtualSensorData>, IDataProducer
    {

        public static VirtualSensorProducer GetVirtualSensorHelper(IDataProducer<KinectData> kinectposdata, SensorLocation sl)
        {
            switch(sl)
            {
                case SensorLocation.NotApplicable:
                    return null;
                case SensorLocation.NotSet:
                    return null;
                case SensorLocation.ForearmRight:
                    return new VirtualSensorProducer(kinectposdata, JointType.WristRight, JointType.ElbowRight, 0);
                case SensorLocation.ForearmLeft:
                    return new VirtualSensorProducer(kinectposdata, JointType.WristLeft, JointType.ElbowLeft, 0);
                case SensorLocation.UpperArmRight:
                    return new VirtualSensorProducer(kinectposdata, JointType.ElbowRight, JointType.ShoulderRight, 0);
                case SensorLocation.UpperArmLeft:
                    return new VirtualSensorProducer(kinectposdata, JointType.ElbowLeft, JointType.ShoulderLeft, 0);
                case SensorLocation.AnkleRight:
                    return new VirtualSensorProducer(kinectposdata, JointType.KneeRight, JointType.AnkleRight, 0);
                case SensorLocation.AnkelLeft:
                    return new VirtualSensorProducer(kinectposdata, JointType.KneeLeft, JointType.AnkleLeft, 0);
                default:
                    throw new InvalidOperationException("Bad Enum given as param");
            }
        }

        private System.Collections.Concurrent.ConcurrentQueue<KinectData> KinectDataConQueue = new System.Collections.Concurrent.ConcurrentQueue<KinectData>();
        private Thread WorkerThread;
        private ThreadStart WorkerFunction;
        private IDataProducer<KinectData> KinectPosData;
        private IFilter X_Filter;
        private IFilter Y_Filter;
        private IFilter Z_Filter;
        private double[] pos;
        public event EventHandler<VirtualSensorData> NewTData;
        public event EventHandler<IData> NewIData;
        public SensorType SensorType { get { return SensorType.Inertial; } }
        public bool IsGood { get;  protected set; }
        public string DeviceName { get { return "KinectVirtualSensor"; } }
        public string DeviceAddress { get { return "KinectVirtualSensor"; } }
        private JointType CloserJoint;
        private JointType FurtherJoint;
        private int PercentDistance;

        public enum FilterType
        {
            SGF,
            Continious
        }
       
        public ThreadedVirtualSensorProducer (IDataProducer<KinectData> kinectposdata, JointType closerjoint, JointType furtherjoint, int percentdistnace, FilterType ft = FilterType.SGF)
        {
            KinectPosData = kinectposdata;
            pos = new double[] { 0, 0, 0 };
            IsGood = true;
            KinectPosData.NewTData += this.OnNewTData;
            WorkerFunction = new ThreadStart(this.StartForWorkerThread);
            WorkerThread = new Thread(this.WorkerFunction);
            CloserJoint = closerjoint;
            FurtherJoint = furtherjoint;
            PercentDistance = percentdistnace;
            if (ft == FilterType.SGF)
            {
                X_Filter = new SavitzkyGolay(11, 7);
                Y_Filter = new SavitzkyGolay(11, 7);
                Z_Filter = new SavitzkyGolay(11, 7);
            }
        }

        private void StartForWorkerThread()
        {
            while (true)
            {
                if (!this.KinectDataConQueue.IsEmpty)
                {
                    KinectData e;
                    if (this.KinectDataConQueue.TryDequeue(out e))
                    {
                        double[] smoothPos = new double[] { 0, 0, 0 };
                        double[] virtualAcc = new double[] { 0, 0, 0 };
                        double[] virtualVel = new double[] { 0, 0, 0 };
                        double[] pos = MathFunctions.midpoint(e.Joints[FurtherJoint], e.Joints[CloserJoint], 0);

                        X_Filter.UpdateVal(pos[0], e.NowInTicks);
                        Y_Filter.UpdateVal(pos[1], e.NowInTicks);
                        Z_Filter.UpdateVal(pos[2], e.NowInTicks);

                        //position
                        smoothPos[0] = X_Filter.GetNDerivative(NDerivative.smooth).Item1;
                        smoothPos[1] = Y_Filter.GetNDerivative(NDerivative.smooth).Item1;
                        smoothPos[2] = Z_Filter.GetNDerivative(NDerivative.smooth).Item1;

                        //velocity
                        virtualVel[0] = X_Filter.GetNDerivative(NDerivative.first).Item1;
                        virtualVel[1] = Y_Filter.GetNDerivative(NDerivative.first).Item1;
                        virtualVel[2] = Z_Filter.GetNDerivative(NDerivative.first).Item1;

                        //acceleration
                        var temp = X_Filter.GetNDerivative(NDerivative.second);
                        virtualAcc[0] = temp.Item1;
                        virtualAcc[1] = Y_Filter.GetNDerivative(NDerivative.second).Item1 + 9.81;
                        virtualAcc[2] = Z_Filter.GetNDerivative(NDerivative.second).Item1;

                        vsd = new VirtualSensorData();
                        vsd.NowInTicks = temp.Item2;
                        for (int i = 0; i < 3; i++)
                        {
                            vsd.acceleration[i] = virtualAcc[i];
                            vsd.velocity[i] = virtualVel[i];
                            vsd.position[i] = smoothPos[i];
                        }
                        if (this.NewIData != null)
                            this.NewIData(this, vsd);
                        if (this.NewTData != null)
                            this.NewTData(this, vsd);
                    }
                }
                else
                    Thread.Sleep(10);
            }
        }



        private VirtualSensorData vsd;
        private void OnNewTData(object sender, KinectData e)
        {
            this.KinectDataConQueue.Enqueue(e);
        }




        public event EventHandler<Exception> OnException;

        public event EventHandler<int> MeasuresPerSec;

        public int MeasuredSamplesPerSecond
        {
            get { throw new NotImplementedException(); }
        }

        public void Start()
        {
            throw new NotImplementedException();
        }

        public void Stop()
        {
            throw new NotImplementedException();
        }

        public void Restart()
        {
            throw new NotImplementedException();
        }

        public void Reset()
        {
            throw new NotImplementedException();
        }


        public bool IsIRestartable
        {
            get { return (this is IRestartable); }
        }
        public Type IDataType
        {
            get
            {
                return typeof(VirtualSensorData);
            }
        }

        #region IDisposable Members

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
