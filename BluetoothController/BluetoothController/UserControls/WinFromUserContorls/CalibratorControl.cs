using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BluetoothController.UserControls.WinFromUserContorls
{
    public partial class CalibratorControl : UserControl
    {
        private Merge Calibrator = null;

        public event EventHandler SetupClicked;

        public event EventHandler<SensorCalibratorData> CalibrationComplete;
        public IDataProducer<InertialSensorData> InertialSesnor { get; set; }
        public IDataProducer<VirtualSensorData> VirtualSesnor { get; set; }


        protected bool IsReady = false;
        protected CancellationTokenSource CancelToken = new CancellationTokenSource();
        public CalibratorControl()
        {
            //w/o these error checking may not work
            this.InertialSesnor = null;
            this.VirtualSesnor = null;

            InitializeComponent();

        }

        protected void SetupButton_Click(object sender, EventArgs e)
        {
            this.InertialSesnor = null;
            this.VirtualSesnor = null;
            //Call the event to let the IDataProducer's be bound by form
            if(this.SetupClicked != null)
                this.SetupClicked(this, new EventArgs());

            //Fail softly
            if (this.InertialSesnor == null || this.VirtualSesnor == null)
            {
                //block user from doing dumb stuff
                this.Disable();
                return;
            }

            //Validate Data again (good practice)
            //If bad, fail hard
            if (this.InertialSesnor == null)
                throw new ArgumentException("Cannot be null.", this.InertialSesnor.GetType().Name);
            if (this.VirtualSesnor == null)
                throw new ArgumentException("Cannot be null.", this.VirtualSesnor.GetType().Name);


            this.Calibrator = new Merge(this.VirtualSesnor, this.InertialSesnor);
            this.BadProgressBarUpdater(Merge.CalibrationLookBackTimeInSec);
        }

        private void CalibrateButton_Click(object sender, EventArgs e)
        {
            //Make sure that the event is unsubscribed so there will be no reference against calibrator
            //in the event they sent a new calibration.

            this.CalibrateButton.Text = "Calibrating!";
            this.CalibrateButton.Enabled = false;


            this.Calibrator.CalibrationComplete += this.Calibrator_CalibrationComplete;
            //TODO: this will need protection in the event of bad data
            this.Calibrator.Calibrate();
            //this.Calibrator.CalibrationComplete -= this.Calibrator_CalibrationComplete;

        }

        void Calibrator_CalibrationComplete(object sender, SensorCalibratorData e)
        {
            this.Invoke((Action)delegate()
            {
                this.OutputTextBox.Text = e.ToPreviewString();
                this.CalibrateButton.Text = "Calibrate";
                this.CalibrateButton.Enabled = true;
            });
            if (this.CalibrationComplete != null)
                this.CalibrationComplete(this, e);
            this.Calibrator.CalibrationComplete -= this.Calibrator_CalibrationComplete;
        }



        protected void Disable()
        {
            this.IsReady = false;
            this.CalibrateButton.Enabled = false;
            this.DataProgressBar.Value = 0;
            this.DataProgressBar.Enabled = false;
            this.OutputTextBox.Enabled = false;
        }
        protected void BadProgressBarUpdater(int time)
        {
            this.IsReady = false;
            this.DataProgressBar.Enabled = true;
            this.CalibrateButton.Enabled = false;
            this.DataProgressBar.ForeColor = System.Drawing.Color.Red;
            DateTime starttime = DateTime.Now;
            DateTime finishtime = DateTime.Now.AddSeconds(time);
            var task = Task.Factory.StartNew((e) =>
            {
                while (DateTime.Now < finishtime)
                {
                    this.CancelToken.Token.ThrowIfCancellationRequested();
                    this.Invoke((Action)delegate()
                    {
                        this.DataProgressBar.Value = (int)(((double)(DateTime.Now - starttime).Seconds) / ((double)time) * 100.0);
                    });
                    Thread.Sleep(250);
                }
                this.Invoke((Action)delegate()
                {
                    this.DataProgressBar.Value = 100;
                    this.DataProgressBar.ForeColor = System.Drawing.Color.LightGreen;
                    this.CalibrateButton.Enabled = true;
                    this.OutputTextBox.Enabled = true;
                });
                this.IsReady = true;
            }, this.CancelToken , TaskCreationOptions.LongRunning);
        }


        
    }

    public class CalibratorProgressBar : ProgressBar
    {
        public CalibratorProgressBar()
        {
            this.SetStyle(ControlStyles.UserPaint, true);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Rectangle rec = e.ClipRectangle;

            rec.Width = (int)(rec.Width * ((double)Value / Maximum)) - 4;
            if (ProgressBarRenderer.IsSupported)
                ProgressBarRenderer.DrawHorizontalBar(e.Graphics, e.ClipRectangle);
            rec.Height = rec.Height - 4;
            e.Graphics.FillRectangle(new SolidBrush(base.ForeColor), 2, 2, rec.Width, rec.Height);
        }
    }
}
