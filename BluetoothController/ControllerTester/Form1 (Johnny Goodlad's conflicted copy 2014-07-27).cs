using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Collections.Specialized;
using System.Collections.Concurrent;
using BluetoothController;
using InTheHand.Net.Bluetooth;
using InTheHand.Net.Sockets;
using System.IO;
using System.Windows.Forms.DataVisualization.Charting;
using BluetoothController.Kinect;
using System.Diagnostics;
using System.Runtime.ExceptionServices;
using System.Windows.Media.Media3D;
//using BluetoothController.Mathematics;
using BluetoothController.WPFUserControls;
using Microsoft.Kinect;

namespace ControllerTester
{
    public partial class Form1 : Form
    {
        private ChartArea chartarea;
        private List<IDataProducer> ActiveLoggableSensors = new List<IDataProducer>();
        private List<IDataProducer<InertialSensorData>> ActiveMotionSensors = new List<IDataProducer<InertialSensorData>>();
        private List<FileLogger> FileLoggers;
        //Note that th capacity of UISensorDataList must be greater than the max number of sensors
        private List<UISensorData> UISensorsDataList = new List<UISensorData>(25);
        private ConcurrentDictionary<string, IDataProducer> DictionaryOfIDataReciever;
        private ConcurrentDictionary<string, UISensorData> DictionaryOfUISensorData;
        private Dictionary<string, int> DictionaryOfGraphRefreshCounters;
        private string FolderLocation;
        public KinectProducer jointreader;
        private InTheHand.Net.Sockets.BluetoothDeviceInfo[] devinfo;
        private Dictionary<SensorLocation,string> SensorLocationKeyValue;
        System.Threading.Timer ReadsPerSecThread;
        System.Threading.Timer DelaySkeletalViewer;
        Quaternion RightArmQuat;
        public Form1()
        {
            //this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            //this.AutoSize = false;
            //Font = new Font(Font.Name, 8.25f * 96f / CreateGraphics().DpiX, Font.Style, Font.Unit, Font.GdiCharSet, Font.GdiVerticalFont);
            InitializeComponent();

            //this.tempfromimu.Invert();
            this.DPMC = new DataProducerManagerCollection(this.Charts);
            DictionaryOfIDataReciever = new ConcurrentDictionary<string, IDataProducer>();
            DictionaryOfUISensorData = new ConcurrentDictionary<string, UISensorData>();
            DictionaryOfGraphRefreshCounters = new Dictionary<string, int>();
            this.AccelChart.Size = new System.Drawing.Size(302, 769);
            this.AccelChart.ChartAreas.Clear();
            this.AccelChart.Legends.Clear();
            this.AccelChart.Series.Clear();
            this.AccelChart.Titles.Clear();
            chartarea = new ChartArea();
            FileLoggers = new List<FileLogger>();
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
                //Console.WriteLine("FirstChanceException event raised in {0}: {1}",
                //    AppDomain.CurrentDomain.FriendlyName, e.Exception.Message);
                Debug.WriteLine("first chance", e.Exception);
            };
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                button1_Click(this, e);
                this.QuatArrowHost.Child = new QuaternionArrow();
                this.QuatBodyViewerHost.Child = new BluetoothController.WPFUserControls.SkeletonQuatBodyViewer();
                SetupAndConnectKinect();
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
                this.SensorDataGridView.DataSource = this.UISensorsDataList;
                DelaySkeletalViewer = new System.Threading.Timer((arg) =>
                {
                    BeginInvoke((Action)delegate()
                    {
                        this.elementHost1.Child = new BluetoothController.SkeletonBodyViewer();
                        
                        this.SensorHost.Child = new BluetoothController.WPFUserControls.IMUQuatViewer();

                    });
                },
                null, 500, 0);
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
            ((ListBox)this.checkedListBox1).DataSource = devinfo;
            ((ListBox)this.checkedListBox1).DisplayMember = "DeviceAddress";
            ((ListBox)this.checkedListBox1).ValueMember = "DeviceAddress";
            this.button1.Text = "Find Bluetooth Devices";
            this.button1.Enabled = true;

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






            //Microsoft.Kinect.JointType[] jtoi = new Microsoft.Kinect.JointType[2];
            //jtoi[0] = Microsoft.Kinect.JointType.HandRight;
            //jtoi[1] = Microsoft.Kinect.JointType.HandLeft;
            //jointreader = new KinectProducer(jtoi);
            //this.DictionaryOfIDataReciever["Kinect"] = jointreader;
            //this.DictionaryOfUISensorData["Kinect"] = new UISensorData(false) { DeviceName = "Kinect", MPS = -1, SensorType = SensorType.Kinect, DataPreview = "Waiting for Data Preivew" };
            //((IRestartable)this.DictionaryOfIDataReciever["Kinect"]).Start();
            //this.DictionaryOfUISensorData["KinectPos"] = new UISensorData(false) { DeviceName = "KinectPos", SensorType = SensorType.Kinect };
            //this.AddChartAreaAndSetup(this.AccelChart, "KinectPos");
            //this.SetupChartAreaForInertial(this.AccelChart, "KinectPos");
            //this.DictionaryOfIDataReciever["Kinect"].NewIData += (senderNewIData, eNewIData) =>
            //{
            //    //Quaternion q = this.GetLeftForearmQuat((KinectData)eNewIData);
            //    bpqd = BodyPosQuatData.GetBodyPosQuatDataFromBody(((KinectData)eNewIData).Body);
            //    //RightArmQuat = new Quaternion(((KinectData)eNewIData).Body.JointOrientations[JointType.WristRight].Orientation);
            //    ((BluetoothController.WPFUserControls.SkeletonQuatBodyViewer)this.QuatBodyViewerHost.Child).AddBodyPosQuatData(bpqd);
            //    DictionaryOfUISensorData["Kinect"].DataPreview = eNewIData.ToPreviewString();
            //    this.UpDateChartWithDataFromKinect(this.AccelChart, "KinectPos", (KinectData)eNewIData);
            //    BeginInvoke((Action)delegate
            //    {
            //        ((BluetoothController.WPFUserControls.QuaternionArrow)this.QuatArrowHost.Child).Update(/*new Quaternion(((KinectData)eNewIData).Body.JointOrientations[JointType.ShoulderRight].Orientation) * new Quaternion(((KinectData)eNewIData).Body.JointOrientations[JointType.ElbowRight].Orientation) */ new Quaternion(((KinectData)eNewIData).Body.JointOrientations[JointType.WristRight].Orientation));
            //    });
            //};
            //this.DictionaryOfIDataReciever["Kinect"].MeasuresPerSec += (senderMPS, eMPS) =>
            //{
            //    this.DictionaryOfUISensorData["Kinect"].MPS = eMPS;
            //};
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
                this.LoggingButton.Enabled = true;
            }
            else
            {
                this.LoggingButton.Enabled = false;
                StringBuilder sb = new StringBuilder();
                FolderLocation = System.IO.Path.Combine(this.FileLoggerFolderBrowser.SelectedPath, "Data_Logs_" + DateTime.Now.ToString("MMM_dd_HH-mm-ss-ff"));
                System.IO.Directory.CreateDirectory(FolderLocation);
                foreach (IDataProducer idr in this.DictionaryOfIDataReciever.Values)
                    FileLoggers.Add(new FileLogger(idr, System.IO.Path.Combine(FolderLocation, "Log_" + idr.DeviceAddress + ".txt")));
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
                ((KinectProducer)this.DictionaryOfIDataReciever[KinectProducer.KinectAddress]).Dispose();
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
            foreach (IDataProducer idr in this.DictionaryOfIDataReciever.Values)
            {
                try
                {
                    if(idr.IsIRestartable)
                        ((IRestartable)idr).Stop();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("FormClosing threw an exception: {0}", ex);
                }
            }
            this.elementHost1.Dispose();
            //this.ReadsPerSecThread.Dispose();
            //Application.SetUnhandledExceptionMode(System.Windows.UnhandledExceptionMode)
            System.Threading.Thread.Sleep(200);
            

        }

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

        private void CalibrateToCord(object sender, IData e)
        {
            //q2*q1' = r
            //(new Quaternion(0,0,1,0)) * (new Quaternion(0,0,0,1))
            //* (new Quaternion(0,0,1,0)) * (new Quaternion(0,0,0,1))
            //RightArmQuat *
            InertialSensorData ISD = (InertialSensorData)e;
            //(new Quaternion(((InertialSensorData)e).Q[0], ((InertialSensorData)e).Q[1], ((InertialSensorData)e).Q[2], ((InertialSensorData)e).Q[3])).Normalize().Inverse();
            
            //arbitraryCorrection = Quaternion.Identity * ISD.QMD3;
            arbitraryCorrection = this.CorrectionFromIMUtoKinect;
            this.DPMC.DPMCollection.First(x => x.Value.UISensorData.SensorType == SensorType.Inertial).Value.IDataProducer.NewIData -= CalibrateToCord;
        }

        private void BindToRightForearmButton_Click(object sender, EventArgs e)
        {
            this.DPMC.DPMCollection.First(x => x.Value.UISensorData.SensorType == SensorType.Inertial).Value.IDataProducer.NewIData += CalibrateToRArm;
        }


        private Quaternion CorrectionFromIMUtoKinect = (new Quaternion(0.344349478371441, 0.344349478371441, 0.512653160840273, 0.458397920243442));
        private Quaternion tempfromimu;
        private void CalibrateToRArm(object sender, IData e)
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
            this.tempfromimu = ((InertialSensorData)e).QMD3;
            this.DPMC.DPMCollection.First(x => x.Value.UISensorData.SensorType == SensorType.Inertial).Value.IDataProducer.NewIData -= CalibrateToRArm;
            this.DPMC.DPMCollection.First(x => x.Value.UISensorData.SensorType == SensorType.Kinect).Value.IDataProducer.NewIData += this.CalibrateToRArmPart2;
        }
        private void CalibrateToRArmPart2(object sender, IData e)
        {


            //Quaternion invCoordinate;
            //Quaternion restoredCoordinateCorrection;

            Quaternion Correct_stok, Correct_a, Arb_sens, Arb_Kinect, Correct_stok__TIMES__Arb_sens, INVERSEOF___Correct_stok__TIMES__Arb_sens, INVERSOF__Arb_k, INVERSEOF__Arb_s, Correct_Ktos;
            //q2*q1' = r
            //(new Quaternion(0,0,1,0)) * (new Quaternion(0,0,0,1))
            //* (new Quaternion(0,0,1,0)) * (new Quaternion(0,0,0,1))
            //RightArmQuat *
            //new Quaternion(0, 0, Math.Sqrt(.5), Math.Sqrt(.5)) *
            /* (new Quaternion(0,0,1,0))* */
            //(new Quaternion(Math.Sqrt(.5), Math.Sqrt(.5), 0, 0)) *

            //coordinateCorrection =  ((new Quaternion(((InertialSensorData)e).Q)) * this.CorrectionFromIMUtoKinect).Normalize().Inverse();
            Arb_Kinect = ((KinectData)e).Body.JointOrientations[JointType.WristRight].Orientation.ToQuaternion();
            //Correct_stok = this.CorrectionFromIMUtoKinect;
            Arb_sens = tempfromimu;
            //Correct_stok__TIMES__Arb_sens = Correct_stok * Arb_sens;
            //INVERSEOF___Correct_stok__TIMES__Arb_sens = Correct_stok__TIMES__Arb_sens;
            //INVERSEOF___Correct_stok__TIMES__Arb_sens.Invert();
            //Correct_a = Kinect * INVERSEOF___Correct_stok__TIMES__Arb_sens;


            Correct_Ktos = this.CorrectionFromIMUtoKinect;
            Correct_Ktos.Invert();


            INVERSOF__Arb_k = Arb_Kinect;
            INVERSOF__Arb_k.Invert();


            INVERSEOF__Arb_s = Arb_sens;
            INVERSEOF__Arb_s.Invert();

            Correct_a = Correct_Ktos * INVERSEOF__Arb_s;

            arbitraryCorrection = Correct_a;

            //Arb_sens.Invert();
            //arbitraryCorrection = Kinect * Arb_sens;


            //arbitraryCorrection = Correct_a;





           // invCoordinate = tempfromimu;
           // invCoordinate.Invert();
           // coordinateCorrection = (this.tempfromimu * this.CorrectionFromIMUtoKinect);
           // restoredCoordinateCorrection = (coordinateCorrection * invCoordinate);
           // restoredCoordinateCorrection.Invert();
           //// arbitraryCorrection = ((KinectData)e).Body.JointOrientations[JointType.WristRight].Orientation.ToQuaternion() * restoredCoordinateCorrection;
           // arbitraryCorrection = coordinateCorrection;
           

            this.DPMC.DPMCollection.First(x => x.Value.UISensorData.SensorType == SensorType.Kinect).Value.IDataProducer.NewIData -= this.CalibrateToRArmPart2;
        }

    }
} 
