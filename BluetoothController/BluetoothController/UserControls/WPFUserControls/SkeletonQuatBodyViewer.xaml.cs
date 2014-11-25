using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Collections.Concurrent;
using System.Windows.Media.Media3D;
using BluetoothController;

namespace BluetoothController.WPFUserControls
{
    /// <summary>
    /// Interaction logic for KinectQuatBodyViewer.xaml
    /// </summary>
    public partial class SkeletonQuatBodyViewer : UserControl
    {
        private ConcurrentQueue<BodyPosQuatData> KQBVDQeueu = new ConcurrentQueue<BodyPosQuatData>();
        private CancellationTokenSource Cancle = new CancellationTokenSource();
        public SkeletonQuatBodyViewer()
        {
            InitializeComponent();
            this.Unloaded += (sender, e) => { this.Cancle.Cancel(); };
            Task.Factory.StartNew(this.WorkerFucntion, this.Dispatcher, Cancle.Token);
        }

        protected void WorkerFucntion(object d)
        {
            BodyPosQuatData bpqd;
            bool gooddequeue = false; 
            while (!this.Cancle.IsCancellationRequested)
            {
                if (this.KQBVDQeueu.IsEmpty == true)
                {
                    Thread.Yield();
                    continue;
                }
                gooddequeue = this.KQBVDQeueu.TryDequeue(out bpqd);
                if (gooddequeue)
                    ((System.Windows.Threading.Dispatcher)d).Invoke(new Action<ModelVisual3D, BodyPosQuatData>(KinectQuatViewer.CreateKinectQuatBody), KinectModel, bpqd);

            }
        }
        public bool AddBodyPosQuatData(BodyPosQuatData bpqd)
        {
            if (this.KQBVDQeueu.Count < 1)
            {
                this.KQBVDQeueu.Enqueue(bpqd);
                return true;
            }
            else
                return false;

        }

    }
}
