using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using System.Windows.Media.Media3D;
using BluetoothController;
using BluetoothController.Kinect;
using BluetoothController.WPFUserControls;
using Microsoft.Kinect;

namespace ControllerTester
{
    public partial class Form1 : Form
    {
        private List<FileLogger> FileLoggers;
        private string FolderLocation;
        private InTheHand.Net.Sockets.BluetoothDeviceInfo[] devinfo;
        private Dictionary<SensorLocation,string> SensorLocationKeyValue;
        private System.Threading.Timer ReadsPerSecThread, DelaySkeletalViewer;
        Quaternion RightArmQuat;
        private Quaternion KinectBinder = Quaternion.Identity;
        protected readonly Quaternion DisplayConstRotation = new Quaternion(0, 0, 1, 0);
        protected readonly Quaternion MappingConstCorrection = new Quaternion(0, 0, Math.Sqrt(.5) , Math.Sqrt(.5)) * new Quaternion(0, 1, 0, 0);
        public Form1()
        {
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            InitializeComponent();
            this.CalibratorControl.SetupClicked += CalibratorControl_SetupClicked;
            this.DPMC = new DataProducerManagerCollection(this.Charts);
            this.FileLoggers = new List<FileLogger>();
            this.SensorDataGridView.AutoGenerateColumns = false;
            this.SensorDataGridView.Columns["DeviceName"].DataPropertyName = "DeviceName";
            this.SensorDataGridView.Columns["MPS"].DataPropertyName = "MPS";
            this.SensorDataGridView.Columns["DataPreview"].DataPropertyName = "DataPreview";
            this.SensorDataGridView.Columns["DataPreview"].DefaultCellStyle.WrapMode = DataGridViewTriState.True;
            this.SensorLocationKeyValue = new Dictionary<SensorLocation,string>();
            foreach (SensorLocation value in Enum.GetValues(typeof(SensorLocation)))
                this.SensorLocationKeyValue.Add(value,this.GetDescription(value));
            this.SensorDataGridView.Columns["SensorLocationCol"].DataPropertyName = "SensorLocation";
            this.SensorDataGridView.Columns["SensorLocationCol"].ReadOnly = false;
            ((DataGridViewComboBoxColumn)this.SensorDataGridView.Columns["SensorLocationCol"]).DataSource = this.SensorLocationKeyValue.ToList();
            ((DataGridViewComboBoxColumn)this.SensorDataGridView.Columns["SensorLocationCol"]).DisplayMember = "Value";
            ((DataGridViewComboBoxColumn)this.SensorDataGridView.Columns["SensorLocationCol"]).ValueMember = "Key";

            AppDomain.CurrentDomain.FirstChanceException +=
            (object source, FirstChanceExceptionEventArgs e) =>
            {
                Debug.WriteLine("first chance", e.Exception);
            };
        }


        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                SetupAndConnectKinect();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("SetupAndConnectKinect Threw: ", ex);
            }
            try
            {
                button1_Click(this, e);
                this.QuatArrowHost.Child = new QuaternionArrow();
                this.QuatBodyViewerHost.Child = new BluetoothController.WPFUserControls.SkeletonQuatBodyViewer();
                ReadsPerSecThread = new System.Threading.Timer((arg) =>
                {
                    try
                    {
                        BeginInvoke((Action)delegate()
                        {
                            this.SensorDataGridView.DataSource = this.DPMC.UISensors.Values.ToList();
                            //this.SensorDataGridView.DataSource = this.DictionaryOfUISensorData.Values.ToList();
                        });
                    }
                    catch
                    { }
                }, null, 0, ((int)(1000.0 / 1)));
                //this.SensorDataGridView.DataSource = this.UISensorsDataList;
                DelaySkeletalViewer = new System.Threading.Timer((arg) =>
                {
                    BeginInvoke((Action)delegate()
                    {
                        this.elementHost1.Child = new BluetoothController.SkeletonBodyViewer();
                        this.SensorHost.Child = new BluetoothController.WPFUserControls.IMUQuatViewer();
                    });
                },
                null, 100, 0);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                Debug.WriteLine(ex.StackTrace);
            }
        }
        public string GetDescription(Enum enumValue)
        {
            object[] attr = enumValue.GetType().GetField(enumValue.ToString())
                .GetCustomAttributes(typeof(DescriptionAttribute), false);

            return attr.Length > 0
               ? ((DescriptionAttribute)attr[0]).Description
               : enumValue.ToString();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            button1.Enabled = false;
            button1.Text = "Searching for Bluetooth Devices: Wait!";
            this.SetListBox();

        }

        private async void SetListBox()
        {
            devinfo = await Task.Run<InTheHand.Net.Sockets.BluetoothDeviceInfo[]>(() => Bluetooth.GetBluetoothDeviceInfo());
            try
            {
                ((ListBox)this.BluethoothDevsCheckedListBox).DataSource = devinfo;
                ((ListBox)this.BluethoothDevsCheckedListBox).DisplayMember = "DeviceAddress";
                ((ListBox)this.BluethoothDevsCheckedListBox).ValueMember = "DeviceAddress";
                this.button1.Text = "Find Bluetooth Devices";
                this.button1.Enabled = true;
            }
            catch (Exception e)
            {
                Debug.WriteLine("SetListBox in Form1 threw an exception");
                throw e;
            }
        }
        BodyPosQuatData bpqd;
        protected void SetupAndConnectKinect ()
        {
            this.DPMC.AddKinectSensor();

            (this.DPMC[KinectProducer.KinectAddress].IDataProducer as IDataProducer<KinectData>).NewTData += (senderNewTData, eNewTData) =>
                {
                    //Quaternion q = this.GetLeftForearmQuat((KinectData)eNewIData);
                    bpqd = BodyPosQuatData.GetBodyPosQuatDataFromBody(eNewTData.Body);
                    RightArmQuat = ((KinectData)eNewTData).Body.JointOrientations[JointType.WristRight].Orientation.ToQuaternion();
                    ((BluetoothController.WPFUserControls.SkeletonQuatBodyViewer)this.QuatBodyViewerHost.Child).AddBodyPosQuatData(bpqd);
                    BeginInvoke((Action)delegate
                    {
                        ((BluetoothController.WPFUserControls.QuaternionArrow)this.QuatArrowHost.Child).Update(eNewTData.Body.JointOrientations[JointType.WristRight].Orientation.ToQuaternion());
                    });

                };
        }

        private void BrowseButton_Click(object sender, EventArgs e)
        {
            FileLoggerFolderBrowser.ShowDialog();
            FileLocationTextBox.Text = FileLoggerFolderBrowser.SelectedPath;
            if (this.FileLoggerFolderBrowser.SelectedPath != null)
                this.LoggingButton.Enabled = true;

            
        }

        bool islogging = false;
        private void LoggingButton_Click(object sender, EventArgs e)
        {
            if(islogging)
            {
                this.BrowseButton.Enabled = false;
                this.LoggingButton.Enabled = false;
                islogging = false;
                foreach(ILogger logger in this.FileLoggers)
                    logger.StopLogging();
                this.LoggingButton.Text = "Start Data Logging";
                this.BrowseButton.Enabled = true;
                this.LoggingButton.Enabled = true;
            }
            else
            {
                this.LoggingButton.Enabled = false;
                StringBuilder sb = new StringBuilder();
                FolderLocation = System.IO.Path.Combine(this.FileLoggerFolderBrowser.SelectedPath, "Data_Logs_" + DateTime.Now.ToString("MMM_dd_HH-mm-ss-ff"));
                System.IO.Directory.CreateDirectory(FolderLocation);
                foreach (var idr in this.DPMC.DPMCollection.Values.Where(x => x.IDataProducer.SensorType == SensorType.Inertial || x.IDataProducer.SensorType == SensorType.MappedIntertial || x.IDataProducer.SensorType == SensorType.Virtual))
                    FileLoggers.Add(new FileLogger(idr.IDataProducer, System.IO.Path.Combine(FolderLocation, "Log_" + idr.IDataProducer.DeviceAddress + ".csv")));
                foreach (BluetoothController.ILogger logger in this.FileLoggers)
                    logger.StartLogging();
                islogging = true;
                this.LoggingButton.Text = "Stop Data Logging";
                this.BrowseButton.Enabled = true;
                this.LoggingButton.Enabled = true;
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                this.DPMC.Dispose();
                System.Threading.Thread.Sleep(100);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Kinect.Dispost() on FormClosing Threw an Exception: {0}", ex);
            }
            foreach (ILogger logger in this.FileLoggers)
            {
                try
                {
                    logger.StopLogging();
                }
                catch(Exception ex)
                {
                    Debug.WriteLine("FormClosing threw an exception: {0}", ex);
                }
            }
            //foreach (IDataProducer idr in this.DPMC.DPMCollection.Values)
            //{
            //    try
            //    {
            //        if(idr.IsIRestartable)
            //            ((IRestartable)idr).Stop();
            //    }
            //    catch (Exception ex)
            //    {
            //        Debug.WriteLine("FormClosing threw an exception: {0}", ex);
            //    }
            //}
            this.elementHost1.Dispose();
            System.Threading.Thread.Sleep(200);
        }

        void CalibratorControl_SetupClicked(object sender, EventArgs e)
        {
            DataProducerManager temp_DPM;
            //this protects against bad binding... the null shold be in CalibratorControll too
            this.CalibratorControl.VirtualSesnor = null;
            this.CalibratorControl.InertialSesnor = null;
            temp_DPM = DPMC.DPMCollection.Values.FirstOrDefault(x => x.IDataProducer.SensorType == SensorType.Virtual);
            if (temp_DPM != null)
                this.CalibratorControl.VirtualSesnor = temp_DPM.IDataProducer as IDataProducer<VirtualSensorData>;

            temp_DPM = DPMC.DPMCollection.Values.FirstOrDefault(x => x.IDataProducer.SensorType == SensorType.MappedIntertial);
            if(temp_DPM != null)
                this.CalibratorControl.InertialSesnor = temp_DPM.IDataProducer as IDataProducer<InertialSensorData>;
        }

        #region DataView Events
        private void SensorDataGridView_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            this.ReadsPerSecThread.Change(System.Threading.Timeout.Infinite, System.Threading.Timeout.Infinite);
        }

        private void SensorDataGridView_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            this.ReadsPerSecThread.Change(0, 100);
        }

        private void SensorDataGridView_Click(object sender, EventArgs e)
        {
            this.ReadsPerSecThread.Change(System.Threading.Timeout.Infinite, System.Threading.Timeout.Infinite);
        }

        private void SensorDataGridView_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            this.SensorDataGridView_Click(sender, new EventArgs());
        }

        private void SensorDataGridView_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            this.SensorDataGridView.CommitEdit(DataGridViewDataErrorContexts.Commit);
            this.ReadsPerSecThread.Change(0, 100);
        }

        private void DefaultOrientButton_Click(object sender, EventArgs e)
        {
            this.DPMC.DPMCollection.First(x => x.Value.UISensorData.SensorType == SensorType.Inertial).Value.IDataProducer.NewIData += CalibrateToCord;

        }
        #endregion

        #region Cilibrations
        protected Quaternion BindingCorrection;
        private void CalibrateToCord (object sender, IData e)
        {
            InertialSensorData ISD = (InertialSensorData)e;
            Quaternion Arb_Sensor = ISD.QuaternionAsQMD3, Arb_SensorInvert = ISD.QuaternionAsQMD3;
            Arb_SensorInvert.Invert();
            this.BindingCorrection = Arb_SensorInvert;
             arbitraryCorrection = this.DisplayConstRotation * Arb_SensorInvert;

            ((this.DPMC.DPMCollection.First(x => x.Value.UISensorData.SensorType == SensorType.MappedIntertial).Value.IDataProducer as MappedBindedInertialSesnorProducer).Mapper as QuaternionCoordinateMapper).Initial = new Quaternion(0,1,0,0) *  Arb_Sensor;
            
            //arbitraryCorrection = Quaternion.Identity * ISD.QMD3;
            //arbitraryCorrection = this.CorrectionFromIMUtoKinect;
            this.DPMC.DPMCollection.First(x => x.Value.UISensorData.SensorType == SensorType.Inertial).Value.IDataProducer.NewIData -= CalibrateToCord;
        }


        private void BindToRightForearmButton_Click(object sender, EventArgs e)
        {
            this.DPMC.DPMCollection.First(x => x.Value.UISensorData.SensorType == SensorType.Inertial).Value.IDataProducer.NewIData += CalibrateToRArmPart1;
        }


        private Quaternion tempfromimu;
        private void CalibrateToRArmPart1(object sender, IData e)
        {
            //Quaternion invCoordinate;
            //Quaternion restoredCoordinateCorrection;
            ////q2*q1' = r
            ////(new Quaternion(0,0,1,0)) * (new Quaternion(0,0,0,1))
            ////* (new Quaternion(0,0,1,0)) * (new Quaternion(0,0,0,1))
            ////RightArmQuat *
            ////new Quaternion(0, 0, Math.Sqrt(.5), Math.Sqrt(.5)) *
            ///* (new Quaternion(0,0,1,0))* */
            ////(new Quaternion(Math.Sqrt(.5), Math.Sqrt(.5), 0, 0)) *

            ////coordinateCorrection =  ((new Quaternion(((InertialSensorData)e).Q)) * this.CorrectionFromIMUtoKinect).Normalize().Inverse();

            //invCoordinate = (new Quaternion(((InertialSensorData)e).Q)).Inverse();
            //coordinateCorrection = ((new Quaternion(((InertialSensorData)e).Q)) * this.CorrectionFromIMUtoKinect);
            //restoredCoordinateCorrection = (coordinateCorrection * invCoordinate).Inverse();
            //arbitraryCorrection = RightArmQuat * restoredCoordinateCorrection;
            this.tempfromimu = ((InertialSensorData)e).QuaternionAsQMD3;
            this.DPMC.DPMCollection.First(x => x.Value.UISensorData.SensorType == SensorType.Inertial).Value.IDataProducer.NewIData -= CalibrateToRArmPart1;
            this.DPMC.DPMCollection.First(x => x.Value.UISensorData.SensorType == SensorType.Kinect).Value.IDataProducer.NewIData += this.CalibrateToRArmPart2;
        }
        private void CalibrateToRArmPart2(object sender, IData e)
        {


            //Quaternion invCoordinate;
            //Quaternion restoredCoordinateCorrection;
            //q2*q1' = r
            //(new Quaternion(0,0,1,0)) * (new Quaternion(0,0,0,1))
            //* (new Quaternion(0,0,1,0)) * (new Quaternion(0,0,0,1))
            //RightArmQuat *
            //new Quaternion(0, 0, Math.Sqrt(.5), Math.Sqrt(.5)) *
            /* (new Quaternion(0,0,1,0))* */
            //(new Quaternion(Math.Sqrt(.5), Math.Sqrt(.5), 0, 0)) *

            //coordinateCorrection =  ((new Quaternion(((InertialSensorData)e).Q)) * this.CorrectionFromIMUtoKinect).Normalize().Inverse();

           // invCoordinate = tempfromimu;
           // invCoordinate.Invert();
           // coordinateCorrection = (this.tempfromimu * this.CorrectionFromIMUtoKinect);
           // restoredCoordinateCorrection = (coordinateCorrection * invCoordinate);
           // restoredCoordinateCorrection.Invert();
           //// arbitraryCorrection = ((KinectData)e).Body.JointOrientations[JointType.WristRight].Orientation.ToQuaternion() * restoredCoordinateCorrection;
           // arbitraryCorrection = coordinateCorrection;


            Quaternion Arb_Sensor,INVERSEOF_Arb_Sensor , Arb_Kinect, GraphicsTransform, CordinateTransform, Correction_Sensor2Kinect;


            //This should be 90 degress around y going anticlockwise;
            //GraphicsTransform = new Quaternion();

            Arb_Sensor = this.tempfromimu;
            Arb_Kinect = ((KinectData)e).Body.JointOrientations[JointType.WristRight].Orientation.ToQuaternion();
            INVERSEOF_Arb_Sensor = Arb_Sensor;
            INVERSEOF_Arb_Sensor.Invert();

            Correction_Sensor2Kinect = Arb_Kinect * INVERSEOF_Arb_Sensor;
            Correction_Sensor2Kinect = (new Quaternion(0, Math.Sqrt(.5), 0, Math.Sqrt(.5))) * this.DisplayConstRotation * Correction_Sensor2Kinect; 
            arbitraryCorrection = Correction_Sensor2Kinect;


            ((this.DPMC.DPMCollection.First(x => x.Value.UISensorData.SensorType == SensorType.MappedIntertial).Value.IDataProducer as MappedBindedInertialSesnorProducer).Mapper as QuaternionCoordinateMapper).Initial = new Quaternion(0, 1, 0, 0) * Arb_Sensor;

            this.DPMC.DPMCollection.First(x => x.Value.UISensorData.SensorType == SensorType.Kinect).Value.IDataProducer.NewIData -= this.CalibrateToRArmPart2;
        }
        #endregion


        CorrectionViaLikeFrames CorLikeFrames;
        private void CalibrateLikeFrameButton_Click(object sender, EventArgs e)
        {
            try
            {
                //call a new CorrectionViaLikeFrames to make sure any bad state is throw away
                this.CorLikeFrames = new CorrectionViaLikeFrames(this.DPMC.DPMCollection.Values.First(x => x.IDataProducer.SensorType == SensorType.Kinect).IDataProducer as IDataProducer<KinectData>,
                    this.DPMC.DPMCollection.Values.First(x => x.IDataProducer.SensorType == SensorType.Inertial).IDataProducer as IDataProducer<InertialSensorData>);
                this.CorLikeFrames.PreCorrection = this.BindingCorrection;
                this.CorLikeFrames.CorrectionCompleted += (senderCor, eCor) =>
                    {
                        this.Invoke((Action)delegate()
                        {
                            StringBuilder sb = new StringBuilder();
                            sb.AppendFormat("w:{1},{0} x:{2},{0} y:{3},{0} z:{4}{0}{0}",
                                Environment.NewLine,
                                eCor.W,
                                eCor.X,
                                eCor.Y,
                                eCor.Z);
                            sb.Append(eCor.ToPitchYawRoll().ToEnvironNewLineString());
                            this.LikeFramesTextBox.Text = sb.ToString();
                        });
                    };
                this.CorLikeFrames.StartCorrection();
            }
            catch (Exception) { }
        }

        private void Form1_Load_1(object sender, EventArgs e)
        {

        }
    }
} 
