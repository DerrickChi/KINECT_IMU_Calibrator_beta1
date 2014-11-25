using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BluetoothController
{
    public class CorrectedInertialSensor : IDataProducer<InertialSensorData>, IDataProducer
    {
        private IDataProducer<InertialSensorData> accelerometer;
        private SensorCalibratorData finalCalibrator;
        private InertialSensorData OutInertialSensorData;

        public CorrectedInertialSensor(IDataProducer<InertialSensorData> imu, SensorCalibratorData calibrator)
        {
            accelerometer = imu;
            finalCalibrator = calibrator;
            accelerometer.NewTData += OnAccelNewTData;
        }
        ~CorrectedInertialSensor()
        {
            accelerometer.NewTData -= OnAccelNewTData;
        }

        private void OnAccelNewTData(object sender, InertialSensorData e)
        {
            OutInertialSensorData = new InertialSensorData() { NormalizedAccelerations = finalCalibrator.transform(e.NormalizedAccelerations), NowInTicks = e.NowInTicks };

            if (this.NewTData != null)
                this.NewTData(this, OutInertialSensorData);
            if (this.NewIData != null)
                this.NewIData(this, OutInertialSensorData);

        }

        public Type IDataType
        {
            get
            {
                return typeof(InertialSensorData);
            }
        }
        public event EventHandler<IData> NewIData;


        public event EventHandler<int> MeasuresPerSec;

        public string DeviceName
        {
            get { return "CorrectedSensor"; }
        }

        public string DeviceAddress
        {
            get { throw new NotImplementedException(); }
        }

        public SensorType SensorType
        {
            get { return BluetoothController.SensorType.CorrectedInertial; }
        }

        public event EventHandler<InertialSensorData> NewTData;


        public event EventHandler<Exception> OnException;

        public int MeasuredSamplesPerSecond
        {
            get { throw new NotImplementedException(); }
        }

        public bool IsIRestartable
        {
            get { return (this is IRestartable); }
        }

        #region IDisposable Members

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
