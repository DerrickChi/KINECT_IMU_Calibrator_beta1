using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BluetoothController.Kinect;

namespace BluetoothController
{
    public class UISensorData
    {
        private SensorLocation sensorlocation = SensorLocation.NotSet;
        public event EventHandler<SensorLocation> SensorLocationSet;
        public string DeviceName {get; set;}
        public int MPS{get; set;}
        public SensorType SensorType {get; set;}
        private string datapreview;
        public string DataPreview { get; set; }
        public SensorLocation SensorLocation
        {
            get
            {
                return this.sensorlocation;
            }
            set
            {
                if (this.SensorType == BluetoothController.SensorType.Inertial)
                {
                    sensorlocation = value;
                    if (this.SensorLocationSet != null)
                        this.SensorLocationSet(this, sensorlocation);
                }
            }
        }
        public UISensorData(bool IsApplicable = true)
        {
            if (!IsApplicable)
                sensorlocation = Kinect.SensorLocation.NotApplicable;
        }
    }
}
