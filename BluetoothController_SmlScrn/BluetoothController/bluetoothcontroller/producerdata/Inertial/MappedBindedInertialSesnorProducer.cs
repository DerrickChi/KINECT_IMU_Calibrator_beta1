using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace BluetoothController
{
    public class MappedBindedInertialSesnorProducer : IDataProducer, IDataProducer<InertialSensorData>
    {
        public IMapper Mapper { get; protected set; }
        protected IDataProducer<InertialSensorData> Binder;
        protected InertialSensorData Data;
        protected DoubleExponentialDeriver X_smoother;
        protected DoubleExponentialDeriver Y_smoother;
        protected DoubleExponentialDeriver Z_smoother;
        private readonly double[] alpha = new double[] { 0.7, double.NaN, double.NaN};
        private readonly double[] gamma = new double[] { 0.3, double.NaN, double.NaN };

        public MappedBindedInertialSesnorProducer(IDataProducer<InertialSensorData> binder, IMapper mapper)
        {
            if (binder == null)
                throw new ArgumentException("Cannot be null.", binder.GetType().Name);
            if (mapper == null)
                throw new ArgumentException("Cannot be null.", mapper.GetType().Name);

            this.Binder = binder;
            this.Mapper = mapper;

            this.Binder.NewTData += OnNewTDataReciever;
            X_smoother = new DoubleExponentialDeriver(alpha, gamma, NDerivative.smooth);
            Y_smoother = new DoubleExponentialDeriver(alpha, gamma, NDerivative.smooth);
            Z_smoother = new DoubleExponentialDeriver(alpha, gamma, NDerivative.smooth);
        }

        public void OnNewTDataReciever(object sender, InertialSensorData e)
        {
            double[] smoothedAcceleration = new double[] { 0, 0, 0 };
            this.Data = new InertialSensorData(e);
            X_smoother.UpdateVal(Data.NormalizedAccelerations[0], e.NowInTicks);
            Y_smoother.UpdateVal(Data.NormalizedAccelerations[1], e.NowInTicks);
            Z_smoother.UpdateVal(Data.NormalizedAccelerations[2], e.NowInTicks);

            smoothedAcceleration[0] = X_smoother.GetNDerivative(NDerivative.smooth).Item1;
            smoothedAcceleration[1] = Y_smoother.GetNDerivative(NDerivative.smooth).Item1;
            smoothedAcceleration[2] = Z_smoother.GetNDerivative(NDerivative.smooth).Item1;

            Data.NormalizedAccelerations = smoothedAcceleration;
            //double[] tdouble = new double[3];
            //for (int i = 0; i < tdouble.Length; i++)
            //    tdouble[i] = this.Data.NormalizedAccelerations[i];// = Stabilizer.Stabilize(Data.NormalizedAccelerations, e.gyropscopes, e.NowInTicks);
            //this.Data.NormalizedAccelerations = tdouble;
            //Vector3D invector3d = new Vector3D(tdouble[0], tdouble[1], tdouble[2]);
            Vector3D invector3d = this.Data.AccelerationAsVMD3;
            Vector3D outvector3d = this.Mapper.Map(this.Data.QuaternionAsQMD3, invector3d);

            this.Data.AccelerationAsVMD3 = outvector3d;
            this.Data.QuaternionAsQMD3 = (Mapper as QuaternionCoordinateMapper).Correction;

            if (this.NewIData != null)
                this.NewIData(this, this.Data);
            if (this.NewTData != null)
                this.NewTData(this, this.Data);
        }

        #region IDataProducer Members
        public event EventHandler<InertialSensorData> NewTData;

        public event EventHandler<IData> NewIData;

        public event EventHandler<int> MeasuresPerSec;

        public SensorType SensorType
        {
            get { return SensorType.MappedIntertial; }
        }

        public Type IDataType
        {
            get { return typeof(InertialSensorData); }
        }

        public string DeviceName
        {
            get { return "MappedBind_" + this.Binder.DeviceName; }
        }

        public string DeviceAddress
        {
            get { return "MappedBind_" + this.Binder.DeviceAddress; }
        }

        public bool IsIRestartable
        {
            get { return (this is IRestartable); }
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            this.Binder.NewTData -= this.OnNewTDataReciever;
        }

        #endregion
    }
}
