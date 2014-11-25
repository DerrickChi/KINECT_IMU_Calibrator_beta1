using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using InTheHand.Net.Sockets;
using InTheHand.Net.Bluetooth;
using InTheHand.Net;

namespace BluetoothController
{
    public class RawInertialSensorProducer : IDataProducer<InertialSensorData>, IDataProducer, IRestartable
    {
        //This class needs some work in how it will need to call another thread to make the changes.
        //That means that the start event will have to be on another thread, and will likely mean that there
        //Will need to be extra private calls in order to get this to work00
        private const int PACKET_START = 208;
        private const int PACKET_END = 13;
        private volatile bool RequestStop = false;
        private bool ReadingPacket = false;
        private byte[] Packet = new byte[32];
        private int Ix = 0;
        private Stream BTStream;
        private volatile int MeasureSamplesPerSec = 0;
        private BluetoothClient BTClient = new BluetoothClient();
        //private BluetoothDeviceInfo BTInfo;
        private Task WorkerTask;
        private CancellationTokenSource Cancle;
        private Thread WorkerThread;
        private ThreadStart WorkerFunction;
        //System.Threading.Timer ReadsPerSecThread;
        private ActionsPerSecond ActionsPerSec;
        //private InertialSensorData SensorData;
        protected InertialSensorData Data;
        protected RawInertialSensorProducer()
        {
            WorkerFunction = new ThreadStart(this.StartForWorkerThread);
            WorkerThread = new Thread(WorkerFunction);
        }
        public RawInertialSensorProducer(BluetoothDeviceInfo btinfo)
        {
            BTClient.Connect(new InTheHand.Net.BluetoothEndPoint(btinfo.DeviceAddress, BluetoothService.SerialPort));
            //BTInfo = btinfo;
            this.DeviceAddress = btinfo.DeviceAddress.ToString();
            this.DeviceName = btinfo.DeviceAddress.ToString();
            BTStream = new BufferedStream(BTClient.GetStream());
            WorkerFunction = new ThreadStart(this.StartForWorkerThread);
            WorkerThread = new Thread(WorkerFunction);
        }
        public RawInertialSensorProducer(string btaddress)
        {
            this.BTClient.Connect(new BluetoothEndPoint(BluetoothAddress.Parse(btaddress), BluetoothService.SerialPort));
            BTStream = new BufferedStream(BTClient.GetStream());
            WorkerFunction = new ThreadStart(this.StartForWorkerThread);
            WorkerThread = new Thread(this.WorkerFunction);
            this.DeviceAddress = btaddress;
            this.DeviceName = btaddress;
        }

        public RawInertialSensorProducer(BluetoothAddress btaddress)
        {
            //mkae sure btclient isn't null... it doesn't throw
            this.BTClient.Connect(new BluetoothEndPoint(btaddress, BluetoothService.SerialPort));
            BTStream = new BufferedStream(BTClient.GetStream());
            WorkerFunction = new ThreadStart(this.StartForWorkerThread);
            WorkerThread = new Thread(this.WorkerFunction);
            this.DeviceAddress = btaddress.ToString();
            this.DeviceName = btaddress.ToString();

        }

        public void Dispose()
        {
            try
            {
                BTStream.Dispose();
                BTClient.Client.Disconnect(false);
                BTClient.Dispose();
                if (WorkerTask.Status != System.Threading.Tasks.TaskStatus.RanToCompletion)
                    WorkerTask.Dispose();
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("~RawInterntialSensorProducer threw exception: {0}", e);
            }
        }

        #region IDataReciever
        public event EventHandler<InertialSensorData> NewTData;
        public event EventHandler<IData> NewIData;
        public event EventHandler<int> MeasuresPerSec;
        public bool IsIRestartable
        { get { return (this is IRestartable); } }
        public Type IDataType
        { get { return typeof(InertialSensorData); } }
        public virtual SensorType SensorType
        { get { return SensorType.Inertial; } }
        public virtual string DeviceName
        { get; protected set; }
        public virtual string DeviceAddress
        { get; protected set; }
        public int MeasuredSamplesPerSecond
        { get { return MeasureSamplesPerSec; } }
        #endregion

        #region IRestartable
        public Exception Exception
        { get; protected set; }
        public event EventHandler<Exception> ExceptionThrown;
        public bool IsGood { get; protected set; }
        public void Reset()
        {
            //WorkerThread.Abort();

            try
            {
                this.Cancle.Cancel();
                this.WorkerTask.Dispose();

                //todo
                BTStream.Close();
                //BTClient = Bluetooth.ConnectToBluetoothDevice(BTInfo);
                //BTStream = BTClient.GetStream();

            }
            catch (Exception)
            { }
        }

        public void Start()
        {
            WorkerThread.Start();
            //WorkerTask =  Task.Factory.StartNew(StartForWorkerThread, Cancle.Token);
        }


        public void Restart()
        {
            Reset();
            Start();
        }

        public void Stop()
        {
            this.Cancle.Cancel();
        }

        #endregion

        private void StartForWorkerThread()
        {
            bool isgood = true;
            int readbyte = 0;
            ActionsPerSec = new ActionsPerSecond();
            this.ActionsPerSec.ActionsPerSecondUpdate += (senderAPSU, eAPSU) =>
            {
                if (MeasuresPerSec != null)
                    MeasuresPerSec(this, (int)eAPSU);
            };
            EmptyBuffer();
            while (isgood)//&& !this.Cancle.IsCancellationRequested)
            {
                try
                {
                    readbyte = BTStream.ReadByte();
                    if (Ix == 32)
                    {
                        EmptyBuffer();
                    }
                    if (readbyte == PACKET_START && !ReadingPacket)
                    {
                        Packet[Ix++] = (byte)readbyte;
                        ReadingPacket = true;
                    }
                    else if (readbyte == PACKET_END && ReadingPacket && Ix == 31)
                    {
                        this.ActionsPerSec.IncrimentActions();
                        this.ActionsPerSec.TryActionsPerSecondUpdate();
                        Packet[Ix++] = (byte)readbyte;
                        //If all things are null add that in
                        DecodePacket();
                        this.FireEvents();
                        EmptyBuffer();
                    }
                    else if (ReadingPacket)
                    {
                        Packet[Ix++] = (byte)readbyte;
                    }
                    else
                    {
                        if (readbyte == -1)
                            throw new InvalidOperationException("The Bluetooth Device has been disconnected");
                        //TODO: Say that something went wrong
                    }
                }

                catch (Exception e)
                {
                    isgood = false;
                    Exception = e;
                    if (ExceptionThrown != null)
                    {
                        ExceptionThrown(this, Exception);
                    }
                }
            }

        }

        protected virtual void FireEvents() 
        {
            if (this.NewTData != null)
                NewTData(this, this.Data);
            if (this.NewIData != null)
                NewIData(this, this.Data);
        }
        private void EmptyBuffer()
        {
            Array.Clear(Packet, 0, Packet.Length);
            ReadingPacket = false;
            Ix = 0;
        }
        private void DecodePacket()
        {
            this.Data = new InertialSensorData();
            if (Packet[0] != PACKET_START && Packet[31] != PACKET_END)
                return;
            for (int i = 1, j = 0; i <= 6; i += 2, j++)
                Data.Accelarometers[j] = (short)((0x0) | (Packet[i] & 0xFF) | (Packet[i + 1] << 8));
            for (int i = 7, j = 0; i <= 12; i += 2, j++)
                Data.gyropscopes[j] = (short)((0x0) | (Packet[i] & 0xFF) | (Packet[i + 1] << 8));
            for (int i = 13, j = 0; i <= 28; i += 4, j++)
                Data.quaternion[j] =
                    (0x0) | (Packet[i] & 0xFF)
                            | ((Packet[i + 1] & 0XFF) << 8)
                            | ((Packet[i + 2] & 0xFF) << 16)
                            | (Packet[i + 3] << 24);
        }
    }
}
