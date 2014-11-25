using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BluetoothController
{
    public class ActionsPerSecond
    {

        private DateTime NextStatusUpdateTime = DateTime.MinValue;
        private uint ActionsSinceUpdate = 0;
        private Stopwatch stopwatch = null;
        public event EventHandler<double> ActionsPerSecondUpdate;
        public long MillisecondsBetweenUpdate {get; set;}
        public double APS { get; protected set; }
        public ActionsPerSecond(long millisecondbetweenupdates = 1000, bool startnow = true)
        {
            MillisecondsBetweenUpdate = millisecondbetweenupdates;
            this.stopwatch = new Stopwatch();
            this.StartOrRestart();
        }
        public bool TryActionsPerSecondUpdate()
        {
            if (DateTime.UtcNow >= this.NextStatusUpdateTime)
            {
                // calcuate fps based on last frame received
                if (this.stopwatch.IsRunning)
                {
                    this.stopwatch.Stop();
                    APS = this.ActionsSinceUpdate / this.stopwatch.Elapsed.TotalSeconds;
                    this.stopwatch.Reset();
                    if (this.ActionsPerSecondUpdate != null)
                        this.ActionsPerSecondUpdate(this, APS);
                    this.ActionsSinceUpdate = 0;
                    this.stopwatch.Start();
                }

                this.NextStatusUpdateTime = DateTime.UtcNow + TimeSpan.FromMilliseconds(this.MillisecondsBetweenUpdate);
                return true;
            }
            return false;
        }
        public void IncrimentActions()
        {
            ActionsSinceUpdate++;
        }
        public void StartOrRestart()
        {
            if (this.stopwatch.IsRunning)
                this.stopwatch.Restart();
            else
                this.stopwatch.Start();
        }


    }
}
