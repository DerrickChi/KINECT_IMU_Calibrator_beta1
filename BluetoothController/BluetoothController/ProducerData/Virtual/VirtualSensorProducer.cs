using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BluetoothController;
using BluetoothController.Kinect;
using Microsoft.Kinect;
//using Altaxo.Calc.Regression;

namespace BluetoothController
{
    public class VirtualSensorProducer : IDataProducer<VirtualSensorData>, IDataProducer
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
                    return new VirtualSensorProducer(kinectposdata, JointType.ElbowRight, JointType.WristRight, 0);
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
        private IDataProducer<KinectData> KinectPosData;
        private IFilter X_Filter;
        private IFilter Y_Filter;
        private IFilter Z_Filter;
        private double[] pos;
        public event EventHandler<VirtualSensorData> NewTData;
        public event EventHandler<IData> NewIData;
        public SensorType SensorType { get { return SensorType.Virtual; } }
        public bool IsGood { get;  protected set; }
        public string DeviceName { get { return "KinectVirtualSensor"; } }
        public string DeviceAddress { get { return "KinectVirtualSensor"; } }
        private JointType CloserJoint;
        private JointType FurtherJoint;
        private int PercentDistance;
        private readonly double[] alpha = new double[]{ 0.4, 0.8, 0.4 }; // [0] = X, [1] = Y, [2] = Z
        private readonly double[] gamma = new double[] { 0.4, 0.0, 0.0 }; 
        private const NDerivative nd = NDerivative.second;

        public enum FilterType
        {
            SGF,
            OurSGF,
            DoubleExponential
        }
       
        public VirtualSensorProducer(IDataProducer<KinectData> kinectposdata, JointType closerjoint, JointType furtherjoint, int percentdistnace, FilterType ft = FilterType.DoubleExponential)
        {
            KinectPosData = kinectposdata;
            pos = new double[] { 0, 0, 0 };
            IsGood = true;
            KinectPosData.NewTData += this.OnNewTData;
            CloserJoint = closerjoint;
            FurtherJoint = furtherjoint;
            PercentDistance = percentdistnace;
            switch (ft)
            {  
                case FilterType.SGF:
                    {
                        X_Filter = new SavitzkyGolay(7, 4);
                        Y_Filter = new SavitzkyGolay(7, 4);
                        Z_Filter = new SavitzkyGolay(7, 4);
                        break;
                    }
                case FilterType.OurSGF:
                    {
                        X_Filter = new OurSavitzkyGolayFilter(5);
                        Y_Filter = new OurSavitzkyGolayFilter(5);
                        Z_Filter = new OurSavitzkyGolayFilter(5);
                        break;
                    }
                case FilterType.DoubleExponential:
                    {
                        X_Filter = new DoubleExponentialDeriver(alpha, gamma, nd);
                        Y_Filter = new DoubleExponentialDeriver(alpha, gamma, nd);
                        Z_Filter = new DoubleExponentialDeriver(alpha, gamma, nd);
                        break;
                    }
            }
        }

        private VirtualSensorData vsd;
        private void OnNewTData(object sender, KinectData e)
        {
            double[] virtualAcc = new double[] { 0, 0, 0 };
            double[] virtualVel = new double[] { 0, 0, 0 };
            double[] pos = new double[] { e.Joints[FurtherJoint].Position.X, e.Joints[FurtherJoint].Position.Y, e.Joints[FurtherJoint].Position.Z };
            TrackingState trackingstate = e.Body.Joints[this.FurtherJoint].TrackingState;
                //= MathFunctions.midpoint(e.Joints[FurtherJoint], e.Joints[CloserJoint], 100);

            X_Filter.UpdateVal(pos[0], e.NowInTicks);
            Y_Filter.UpdateVal(pos[1], e.NowInTicks);
            Z_Filter.UpdateVal(pos[2], e.NowInTicks);
            
            //acceleration
            var temp = X_Filter.GetNDerivative(nd);
            virtualAcc[0] = temp.Item1;
            virtualAcc[1] = Y_Filter.GetNDerivative(nd).Item1 + 9.81;
            //hack make z -
            virtualAcc[2] = -Z_Filter.GetNDerivative(nd).Item1;

            //position
            double[] virtualPos = new double[] {pos[0], pos[1], pos[2]};

            vsd = new VirtualSensorData();
            vsd.NowInTicks = temp.Item2;
            //vsd.NowInTicks = e.NowInTicks;
            for (int i = 0; i < 3; i++)
            {
                vsd.acceleration[i] = virtualAcc[i];
                vsd.velocity[i] = virtualVel[i];
                vsd.position[i] = virtualPos[i];
            }
            vsd.isinferredornottracked = (trackingstate == TrackingState.Inferred) || (trackingstate == TrackingState.NotTracked);
            if (this.NewIData != null)
                this.NewIData(this, vsd);
            if (this.NewTData != null)
                this.NewTData(this, vsd);

        }


        //public event EventHandler<Exception> OnException;

        public event EventHandler<int> MeasuresPerSec;

        public int MeasuredSamplesPerSecond
        {
            get { throw new NotImplementedException(); }
        }

        public bool IsIRestartable
        {
            get { return (this is IRestartable); }
        }

        public Type IDataType
        {
            get { return typeof(VirtualSensorData); }
        }

        public void Dispose()
        { }
    }
}
