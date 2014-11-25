using BluetoothController;
using BluetoothController.Kinect;
//using BluetoothController.Mathematics;
using System.Windows.Media.Media3D;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using System.Linq;
namespace ControllerTester
{
    public partial class Form1 : Form
    {
        public DataProducerManagerCollection DPMC;
        private const double AxisSize = 160;
        private string GetKinectVirutalName(object o)
        {
            return "KinectVirtual_" + ((UISensorData)o).DeviceName + '_' + ((UISensorData)o).SensorLocation;
        }
        public Quaternion arbitraryCorrection;
        public Quaternion coordinateCorrection;
        private void OnItemChecked(object sender, ItemCheckEventArgs e)
        {
            foreach (int i in this.BluethoothDevsCheckedListBox.SelectedIndices)
            {
                if (!this.DPMC.DPMCollection.ContainsKey(devinfo[i].DeviceAddress.ToString()))
                {
                    DataProducerManager IntertialSesnor;
                    DataProducerManager MappedInertialSensor;


                    if ((IntertialSesnor = this.DPMC.AddRawInertialSensor(devinfo[i].DeviceAddress)) != null)
                    {
                        (IntertialSesnor.IDataProducer as IDataProducer<InertialSensorData>).NewTData += (senderNewTData, eNewTData) =>
                            {
                                BeginInvoke((Action)delegate
                                {
                                    Quaternion temp = eNewTData.QuaternionAsQMD3;
                                    ((BluetoothController.WPFUserControls.IMUQuatViewer)this.SensorHost.Child).Update(temp, arbitraryCorrection);
                                });
                            };
                        MappedInertialSensor = DPMC.AddMappedBindedInertialSensor(IntertialSesnor, new QuaternionCoordinateMapper() { Initial = new Quaternion(0.010259, -0.008919, -0.001392, 0.999907), CoefficientVector = new Vector3D(-1.0, -1.0, 1.0) });
                    }
                }
            }

            //bool isfound = false;
            //string deviceaddress = string.empty;
            //foreach (var idr in this.dictionaryofidatareciever.where(x => x.value.sensortype == sensortype.inertial))
            //{
            //    foreach (int i in this.checkedlistbox1.selectedindices)
            //    {
            //        if (idr.key == devinfo[i].deviceaddress.tostring())
            //        {
            //            deviceaddress = devinfo[i].deviceaddress.tostring();
            //            isfound = true;
            //            break;
            //        }
            //        if (!isfound)
            //        {
            //            idataproducer tremoveidr;
            //            uisensordata tremoveuisd;
            //            string s = this.dictionaryofuisensordata.first(x => x.key.contains("kinectvirtual_") && x.key.contains(deviceaddress)).value.   
            //            this.dictionaryofidatareciever.tryremove(deviceaddress, out tremoveidr);
            //            this.dictionaryofuisensordata.tryremove(deviceaddress, out tremoveuisd);
            //        }
            //    }

            //}
        }
    }
}
