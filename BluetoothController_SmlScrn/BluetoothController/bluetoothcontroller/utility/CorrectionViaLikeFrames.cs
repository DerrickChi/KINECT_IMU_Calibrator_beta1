using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Kinect;
using System.Windows.Media.Media3D;
using BluetoothController.Kinect;
using System.Threading;

namespace BluetoothController
{
    public class CorrectionViaLikeFrames
    {
        private const int BUFFERSIZE = 2;
        private const int SECONDSTOAVERAGE = 5;
        public event EventHandler<Quaternion> CorrectionCompleted;
        IDataProducer<InertialSensorData> IDataISD;
        IDataProducer<KinectData> IDataKD;
        private ConcurrentCappedQueue<Quaternion?> ISDQuatQueue = new ConcurrentCappedQueue<Quaternion?>(BUFFERSIZE);
        private ConcurrentCappedQueue<Quaternion?> KDQuatQueue = new ConcurrentCappedQueue<Quaternion?>(BUFFERSIZE);
        private JointType JT;
        private Quaternion QuatAverageSummation = new Quaternion(0, 0, 0, 0);
        private int SamplesRecorded = 0;
        private readonly object _Lock = new object();
        private Quaternion? _PreCorrection;
        public Quaternion? PreCorrection
        {
            get
            {
                lock (this._Lock)
                {
                    return this._PreCorrection;
                }
            }
            set
            {
                lock (this._Lock)
                {
                    this._PreCorrection = value;
                }
            }
        }
        public CorrectionViaLikeFrames(IDataProducer<KinectData> idatakd, IDataProducer<InertialSensorData> idataisd, JointType jt = JointType.WristRight)
        {
            this.IDataISD = idataisd;
            this.IDataKD = idatakd;
            this.JT = jt;
        }


        public void StartCorrection()
        {
            Task.Factory.StartNew(this.GenerateCorrection);
        }

        protected void GenerateCorrection()
        {
            DateTime looptill = DateTime.Now.AddSeconds(SECONDSTOAVERAGE);
            Quaternion? isdquat = null;
            Quaternion? kdquat = null;
            this.IDataISD.NewTData += this.OnISDNewTData;
            this.IDataKD.NewTData += this.OnKDNewTData;
            while (DateTime.Now < looptill)
            {
                if (!this.ISDQuatQueue.IsEmpty && !this.KDQuatQueue.IsEmpty)
                {
                    if (this.ISDQuatQueue.TryDequeue(out isdquat) && this.KDQuatQueue.TryDequeue(out kdquat))
                    {
                        if (kdquat.HasValue)
                        {
                            Quaternion Correction, CorrectSensorInverse, KinectJoint;
                            CorrectSensorInverse = (this.PreCorrection ?? Quaternion.Identity) * isdquat.Value;
                            CorrectSensorInverse.Invert();
                            KinectJoint = kdquat.Value;
                            Correction = CorrectSensorInverse * KinectJoint;
                            this.QuatAverageSummation += Correction;
                            this.SamplesRecorded++;
                        }
                    }
                }
                else
                    Thread.Yield();
            }
            this.IDataISD.NewTData -= this.OnISDNewTData;
            this.IDataKD.NewTData -= this.OnKDNewTData;


            Quaternion AveragedQuaternion = new Quaternion(0, 0, 0, 0);
            AveragedQuaternion.W = this.QuatAverageSummation.W / this.SamplesRecorded;
            AveragedQuaternion.X = this.QuatAverageSummation.X / this.SamplesRecorded;
            AveragedQuaternion.Y = this.QuatAverageSummation.Y / this.SamplesRecorded;
            AveragedQuaternion.Z = this.QuatAverageSummation.Z / this.SamplesRecorded;
            AveragedQuaternion.Normalize();

            if (this.CorrectionCompleted != null)
                this.CorrectionCompleted(this, AveragedQuaternion);
        }

        public void OnKDNewTData(object sender, KinectData e)
        {
            if (e.Body.Joints[this.JT].TrackingState == TrackingState.Tracked)
            {
                this.KDQuatQueue.Add(e.Body.JointOrientations[this.JT].Orientation.ToQuaternion());
            }
            else
                this.KDQuatQueue.Add(null);
        }
        public void OnISDNewTData(object sender, InertialSensorData e)
        {
            this.ISDQuatQueue.Add(e.QuaternionAsQMD3);
        }

    }
}



//public Quaternion GetCorrectionQuaternionSensorToKinect(Quaternion kinectjointquat, Quaternion senorquat)
//{
//    Quaternion CorrectSensorInverse;
//    CorrectSensorInverse = senorquat;
//    CorrectSensorInverse.Invert();
//    return CorrectSensorInverse * kinectjointquat;
//}