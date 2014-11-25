using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace BluetoothController
{
    /// <summary>
    /// This class will reduce the frequency of a given event to a specific interval.
    /// It will send out the last update if the event is no longer being called.
    /// </summary>
    public class TimedBasedEventRegulator<TEventArgs> : IRegulatedEvent<TEventArgs>
    {
        private Timer CallBackTimer = new Timer();
        private Stopwatch FrequnecyRegulatorStopwatch = new Stopwatch();
        private TEventArgs MostRecentIData;
        private bool IsUnsent = false;
        private int EventHertz;
        private int FrequencyIntervalinMilliSec
        {
            get
            {
                return TimeSpan.FromSeconds(1.0 / this.EventHertz).Milliseconds;
            }
        }
        public TimedBasedEventRegulator(int eventhertz = 20 )
        {
            this.EventHertz = eventhertz;
            this.CallBackTimer.Elapsed += OnCallBackTimerElapsed;
        }

        private void OnCallBackTimerElapsed(object sender, ElapsedEventArgs e)
        {
            this.CallBackTimer.Enabled = false;
            this.SendMostRecentIData();
        }

        protected void SendMostRecentIData()
        {
            if (this.NewRegulatedEvent != null && this.IsUnsent)
            {
                this.NewRegulatedEvent(this, this.MostRecentIData);
                this.IsUnsent = false; ;
            }
        }

        #region IRegulatedEvent<TEventArgs> Members

        public event EventHandler<TEventArgs> NewRegulatedEvent;

        public void OnNewTEventArgs(object sender, TEventArgs e)
        {
            this.MostRecentIData = e;
            this.IsUnsent = true;

            //Make sure that the call back is on when ever some data has come in.
            this.CallBackTimer.Enabled = true;
            //Pushback the call back
            this.CallBackTimer.Interval = this.FrequencyIntervalinMilliSec;

            if (!this.FrequnecyRegulatorStopwatch.IsRunning)
                this.FrequnecyRegulatorStopwatch.Start();
            if (this.FrequnecyRegulatorStopwatch.ElapsedMilliseconds >= this.FrequencyIntervalinMilliSec)
            {
                this.FrequnecyRegulatorStopwatch.Restart();
                this.SendMostRecentIData();
            }
        }


        #endregion
    }
}