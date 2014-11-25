using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace BluetoothController
{
    public class FileLogger : ILogger
    {
        private bool firstwrite = true;
        public StreamWriter OStream;
        private string FileLocation;
        private IDataProducer DataReciever;
        public FileLogger(IDataProducer datareciever, string filelocation, bool startloggingnow = false)
        {
            DataReciever = datareciever;
            FileLocation = filelocation;
            if (FileLocation == null)
                throw new NullReferenceException("Must set filelocation");
            OStream = new StreamWriter(filelocation, false);
            if (startloggingnow)
                ((ILogger)this).StartLogging();
        }
        ~FileLogger()
        {
            try
            {
                this.DataReciever.NewIData -= LogData;
                OStream.Dispose();
            }
            catch
            { }
        }
        void ILogger.StartLogging()
        {
            this.DataReciever.NewIData += LogData;
        }

        private void LogData(object sender, IData e)
        {
            if (firstwrite)
            {
                this.OStream.Write(e.ToFileHeaderString());
                this.firstwrite = false;
            }
            OStream.Write(e.ToFileString());
        }

        void ILogger.StopLogging()
        {
            try
            {
                this.DataReciever.NewIData -= LogData;
                this.OStream.Flush();
                OStream.Close();
            }
            catch (Exception) { }
        }



    }
}

