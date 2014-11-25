using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BluetoothController;
using BluetoothController.Kinect;
using Microsoft.Kinect;

namespace SensorCalibration
{
    public class VirtualSensor : IDataReciever<VirtualSensorData>, IDataReciever
    {
        private IDataReciever<JointsOfInterestData> KinectPosData;
        private Derivative X_derivation;
        private Derivative Y_derivation;
        private Derivative Z_derivation;
        private double[] pos;
        public event EventHandler<VirtualSensorData> NewTData;
        public event EventHandler<IData> NewIData;
        public SensorType SensorType { get { return SensorType.Inertial; } }
        public bool IsGood { get;  protected set; }
        public string DeviceName { get { return "KinectVirtualSensor"; } }
        public string DeviceAddress { get { return "KinectVirtualSensor"; } }
       

        public VirtualSensor(IDataReciever<JointsOfInterestData> kinectPosData)
        {
            KinectPosData = kinectPosData;
            X_derivation = new Derivative();
            Y_derivation = new Derivative();
            Z_derivation = new Derivative();
            pos = new double[] { 0, 0, 0 };
            IsGood = true;

        }

        private VirtualSensorData vsd;
        private void OnNewTData(object sender, JointsOfInterestData e)
        {
            double[] virtualAcc = new double[] { 0, 0, 0 };
            double[] pos = mathFunctions.midpoint(e.Joints[JointType.WristRight], e.Joints[JointType.ElbowRight], 50);
            X_derivation.updateVal(pos[0], e.NowInTicks);
            Y_derivation.updateVal(pos[1], e.NowInTicks);
            Z_derivation.updateVal(pos[2], e.NowInTicks);

            virtualAcc[0] = X_derivation.getDoubleDerivative();
            virtualAcc[1] = Y_derivation.getDoubleDerivative();
            virtualAcc[2] = Z_derivation.getDoubleDerivative();
            for (int i = 0; i < 3; i++)
                vsd.accelerations[i] = virtualAcc[i];
            if (this.NewIData != null)
                this.NewIData(this, vsd);
            if (this.NewTData != null)
                this.NewTData(this, vsd);

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
    }
}
