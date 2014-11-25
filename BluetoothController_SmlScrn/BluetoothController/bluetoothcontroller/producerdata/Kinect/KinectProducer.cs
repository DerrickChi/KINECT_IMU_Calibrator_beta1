using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using BluetoothController;
using Microsoft.Kinect;

namespace BluetoothController.Kinect
{
    /// <summary>
    /// //This class will be run on a worker thread so that it won't block datacapture
    /// </summary>
    public class KinectProducer : IDataProducer<KinectData>, IDataProducer, IRestartable, IDisposable
    {
        /// <summary>
        /// The address of the kinect sensor
        /// </summary>
        public static string KinectAddress {get {return "Kinect"; } }

        //For tasks
        private CancellationTokenSource Cancle = new CancellationTokenSource();
        private Task WorkerTask;


        public JointType[] JointsOfInterest;
        private Thread WorkerThread;
        private ThreadStart WorkerFunction;
        private Microsoft.Kinect.KinectSensor Kinect;
        //public KinectSensor Sensor { get { return Kinect; } }
        private Body[] Bodies = null;
        private BodyFrameReference BFRefence;
        private ActionsPerSecond APS;
        private BodyFrameReader BFReader;
        private BodyFrame BFFrame;
        //ugly implementation of the joints but it will do
        protected Dictionary<JointType, Joint> _Joints;
        public Dictionary<JointType, Joint> Joints
        {
            get { return _Joints; }
            protected set { _Joints = value; }
        }

        public KinectProducer(JointType[] jointsofinterest)
        {
            WorkerFunction = new ThreadStart(this.StartForWorkerThread);
            WorkerThread = new Thread(WorkerFunction);
            JointsOfInterest = jointsofinterest;
            Kinect = KinectSensor.GetDefault();
            FrameDescription fd = Kinect.DepthFrameSource.FrameDescription;
            Kinect.Open();
            //Kinect.Open();
            Thread.Sleep(200);
            //Kinect.Close();
            if(this.Kinect.IsOpen)
                BFReader = Kinect.BodyFrameSource.OpenReader();
            //else
            //    this.Kinect.IsAvailableChanged.
            //Kinect.Close();
         }
        ~KinectProducer()
        {
            //this.ReadsPerSecThread.Dispose();
            if (this.BFReader != null)
            {
                this.BFReader.FrameArrived -= OnFrameArrived;
                this.BFReader.Dispose();
                this.BFReader = null;
            }

            if (this.Kinect != null)
            {
                this.Kinect.Close();
                this.Kinect = null;
            }
        }

        #region IDataReciever
        public event EventHandler<KinectData> NewTData;
        public event EventHandler<IData> NewIData;
        public event EventHandler<int> MeasuresPerSec;
        public bool IsGood { get; protected set; }
        public string DeviceName
        {
            get 
            {
                return "KinectSensor";
                //return Kinect.UniqueKinectId;
            }
        }
        public bool IsIRestartable
        {
            get { return (this is IRestartable); }
        }

        public Type IDataType
        {
            get { return typeof(KinectData); }
        }
        public string DeviceAddress
        {
            get
            {
                return KinectProducer.KinectAddress;
            }
        }
        public SensorType SensorType
        {
            get
            {
                return SensorType.Kinect;
            }
        }
        #endregion

        #region IRestartable
        public event EventHandler<Exception> ExceptionThrown;
        public Exception Exception { get; set; }
        public void Start()
        {
            WorkerTask = Task.Factory.StartNew(this.StartForWorkerThread, Cancle.Token);
           
        }

        public void Stop()
        {
            this.BFReader.FrameArrived -= OnFrameArrived;
            WorkerTask.Dispose();
            Kinect.Close();

        }

        public void Restart()
        {
            Reset();
            Start();
        }

        public void Reset()
        {
            throw new NotImplementedException();
        }
        #endregion

        private void StartForWorkerThread()
        {
            this.APS = new ActionsPerSecond();
            this.APS.ActionsPerSecondUpdate += (senderAPSU, eAPSU) =>
            {
                if (this.MeasuresPerSec != null)
                    this.MeasuresPerSec(this, (int)eAPSU);
            };


            //When there is new body frame
            try
            {
                BFReader.FrameArrived += OnFrameArrived;
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("BFReader.FrameArrived += method: threw error, likely no kinect conneted: {0}", e);
                IsGood = false;
                this.Exception = e;
                ExceptionThrown(this, e);
            }
        }

        private void OnFrameArrived(object sender, BodyFrameArrivedEventArgs e)
        {
            BFRefence = e.FrameReference;
            try
            {
                //this.APS.IncrimentActions();
                this.APS.TryActionsPerSecondUpdate();
                BFFrame = BFRefence.AcquireFrame();
                using (BFFrame)
                {
                    this.APS.IncrimentActions();
                    //BFFrame.GetAndRefreshBodyData(Bodies);
                    BFRefence = e.FrameReference;
                    Bodies = new Body[BFFrame.BodyCount];
                    BFFrame.GetAndRefreshBodyData(Bodies);
                    foreach (Body b in Bodies)
                    {
                        if (b.IsTracked)
                        {
                            KinectData joid = new KinectData(JointsOfInterest, b);
                            if (NewTData != null)
                                NewTData(this, joid);
                            if (NewIData != null)
                                NewIData(this, joid);
                        }
                    }
                }
            }
            catch (Exception e1) 
            {
                throw e1;
            }
        }

        #region IDisposable Members

        public void Dispose()
        {
            if (this.BFReader != null)
            {
                this.BFReader.FrameArrived -= OnFrameArrived;
                this.BFReader.Dispose();
            }
            this.BFReader = null;
            this.Kinect.Close();
            this.Kinect = null;
        }

        #endregion
    }
}
