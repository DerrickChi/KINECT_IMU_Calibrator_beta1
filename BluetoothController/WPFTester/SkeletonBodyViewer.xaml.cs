using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
using Microsoft.Kinect;
using System.ComponentModel;

namespace WPFTester
{
    /// <summary>
    /// Interaction logic for SkeletonBodyViewer.xaml
    /// </summary>
    public partial class SkeletonBodyViewer : UserControl, INotifyPropertyChanged
    {
        private const double HandSize = 30;
        private const double JointThickness = 3;
        private const double ClipBoundsThickness = 10;
        private readonly Brush handClosedBrush = new SolidColorBrush(Color.FromArgb(122, 255, 0, 0));
        private readonly Brush handOpenBrush = new SolidColorBrush(Color.FromArgb(128, 0, 255, 0));
        private readonly Brush handLassoBrush = new SolidColorBrush(Color.FromArgb(128, 0, 0, 255));
        private readonly Brush trackedJointBrush = new SolidColorBrush(Color.FromArgb(255, 68, 192, 68));      
        private readonly Brush inferredJointBrush = Brushes.Yellow;
        private readonly Pen trackedBonePen = new Pen(Brushes.Green, 6);  
        private readonly Pen inferredBonePen = new Pen(Brushes.Gray, 1);
        private int DisplayHeight, DisplayWidth;
        private KinectSensor Kinect;
        private Body[] Bodies = null;
        private CoordinateMapper CordMapper = null;
        private BodyFrameReader reader = null;
        DrawingGroup SkeletonDrawing;
        DrawingImage SkeletonImage;
        public SkeletonBodyViewer()
        { 
            InitializeComponent(); }
        public SkeletonBodyViewer(KinectSensor sensor)
        {
            PassSensor(sensor);
            InitializeComponent();
        }
        public void PassSensor(KinectSensor kinect)
        {
            Kinect = kinect;
            kinect.Close();
            if (!Kinect.IsOpen)
                kinect.Open();
            FrameDescription frameDesciption = this.Kinect.DepthFrameSource.FrameDescription;
            DisplayHeight = frameDesciption.Height;
            DisplayWidth = frameDesciption.Width;
            reader = Kinect.BodyFrameSource.OpenReader();
            SkeletonDrawing = new DrawingGroup();
            SkeletonImage = new DrawingImage(SkeletonDrawing);
            DataContext = this;
            reader.FrameArrived += reader_FrameArrived;
        }

        void reader_FrameArrived(object sender, BodyFrameArrivedEventArgs e)
        {
            BodyFrameReference BFReference = e.FrameReference;
            try
            {
                BodyFrame frame = BFReference.AcquireFrame();
                if (frame != null)
                {
                    using (frame)
                    {
                        using (DrawingContext dc = SkeletonDrawing.Open())
                        {
                            dc.DrawRectangle(Brushes.Black, null, new Rect(0.0, 0.0, DisplayWidth, DisplayHeight));
                            if (Bodies == null)
                                Bodies = new Body[frame.BodyCount];
                            frame.GetAndRefreshBodyData(Bodies);
                            foreach (Body b in Bodies)
                            {
                                if (b.IsTracked)
                                {
                                    this.DrawClippedEdges(b, dc);
                                    IReadOnlyDictionary<JointType, Joint> joints = b.Joints;

                                    Dictionary<JointType, Point> jointsPoints = new Dictionary<JointType, Point>();
                                    foreach (JointType jt in joints.Keys)
                                    {
                                        DepthSpacePoint dsp = CordMapper.MapCameraPointToDepthSpace(joints[jt].Position);
                                        jointsPoints[jt] = new Point(dsp.X, dsp.Y);

                                    }
                                    this.DrawBody(joints, jointsPoints, dc);
                                    DrawHand (b.HandLeftState, jointsPoints[JointType.HandLeft], dc);
                                    DrawHand(b.HandRightState, jointsPoints[JointType.HandRight], dc);
                                }
                                this.SkeletonDrawing.ClipGeometry = new RectangleGeometry(new Rect(0.0, 0.0, DisplayWidth, DisplayHeight));
                            }
                        }
                    }
                }
            }
            catch (Exception) { }

        }
        private void DrawHand(HandState handState, Point handPosition, DrawingContext drawingContext)
        {
            switch (handState)
            {
                case HandState.Closed:
                    drawingContext.DrawEllipse(this.handClosedBrush, null, handPosition, HandSize, HandSize);
                    break;

                case HandState.Open:
                    drawingContext.DrawEllipse(this.handOpenBrush, null, handPosition, HandSize, HandSize);
                    break;

                case HandState.Lasso:
                    drawingContext.DrawEllipse(this.handLassoBrush, null, handPosition, HandSize, HandSize);
                    break;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public ImageSource ImageSource
        {
            get
            {
                return SkeletonImage;
            }
        }
        private void DrawBody(IReadOnlyDictionary<JointType, Joint> joints, IDictionary<JointType, Point> jointPoints, DrawingContext drawingContext)
        {
            // Draw the bones

            // Torso
            this.DrawBone(joints, jointPoints, JointType.Head, JointType.Neck, drawingContext);
            this.DrawBone(joints, jointPoints, JointType.Neck, JointType.SpineShoulder, drawingContext);
            this.DrawBone(joints, jointPoints, JointType.SpineShoulder, JointType.SpineMid, drawingContext);
            this.DrawBone(joints, jointPoints, JointType.SpineMid, JointType.SpineBase, drawingContext);
            this.DrawBone(joints, jointPoints, JointType.SpineShoulder, JointType.ShoulderRight, drawingContext);
            this.DrawBone(joints, jointPoints, JointType.SpineShoulder, JointType.ShoulderLeft, drawingContext);
            this.DrawBone(joints, jointPoints, JointType.SpineBase, JointType.HipRight, drawingContext);
            this.DrawBone(joints, jointPoints, JointType.SpineBase, JointType.HipLeft, drawingContext);

            // Right Arm    
            this.DrawBone(joints, jointPoints, JointType.ShoulderRight, JointType.ElbowRight, drawingContext);
            this.DrawBone(joints, jointPoints, JointType.ElbowRight, JointType.WristRight, drawingContext);
            this.DrawBone(joints, jointPoints, JointType.WristRight, JointType.HandRight, drawingContext);
            this.DrawBone(joints, jointPoints, JointType.HandRight, JointType.HandTipRight, drawingContext);
            this.DrawBone(joints, jointPoints, JointType.WristRight, JointType.ThumbRight, drawingContext);

            // Left Arm
            this.DrawBone(joints, jointPoints, JointType.ShoulderLeft, JointType.ElbowLeft, drawingContext);
            this.DrawBone(joints, jointPoints, JointType.ElbowLeft, JointType.WristLeft, drawingContext);
            this.DrawBone(joints, jointPoints, JointType.WristLeft, JointType.HandLeft, drawingContext);
            this.DrawBone(joints, jointPoints, JointType.HandLeft, JointType.HandTipLeft, drawingContext);
            this.DrawBone(joints, jointPoints, JointType.WristLeft, JointType.ThumbLeft, drawingContext);

            // Right Leg
            this.DrawBone(joints, jointPoints, JointType.HipRight, JointType.KneeRight, drawingContext);
            this.DrawBone(joints, jointPoints, JointType.KneeRight, JointType.AnkleRight, drawingContext);
            this.DrawBone(joints, jointPoints, JointType.AnkleRight, JointType.FootRight, drawingContext);

            // Left Leg
            this.DrawBone(joints, jointPoints, JointType.HipLeft, JointType.KneeLeft, drawingContext);
            this.DrawBone(joints, jointPoints, JointType.KneeLeft, JointType.AnkleLeft, drawingContext);
            this.DrawBone(joints, jointPoints, JointType.AnkleLeft, JointType.FootLeft, drawingContext);

            // Draw the joints
            foreach (JointType jointType in joints.Keys)
            {
                Brush drawBrush = null;

                TrackingState trackingState = joints[jointType].TrackingState;

                if (trackingState == TrackingState.Tracked)
                {
                    drawBrush = this.trackedJointBrush;
                }
                else if (trackingState == TrackingState.Inferred)
                {
                    drawBrush = this.inferredJointBrush;
                }

                if (drawBrush != null)
                {
                    drawingContext.DrawEllipse(drawBrush, null, jointPoints[jointType], JointThickness, JointThickness);
                }
            }
        }


        private void DrawBone(IReadOnlyDictionary<JointType, Joint> joints, IDictionary<JointType, Point> jointPoints, JointType jointType0, JointType jointType1, DrawingContext drawingContext)
        {
            Joint joint0 = joints[jointType0];
            Joint joint1 = joints[jointType1];

            // If we can't find either of these joints, exit
            if (joint0.TrackingState == TrackingState.NotTracked ||
                joint1.TrackingState == TrackingState.NotTracked)
            {
                return;
            }

            // Don't draw if both points are inferred
            if (joint0.TrackingState == TrackingState.Inferred &&
                joint1.TrackingState == TrackingState.Inferred)
            {
                return;
            }

            // We assume all drawn bones are inferred unless BOTH joints are tracked
            Pen drawPen = this.inferredBonePen;
            if ((joint0.TrackingState == TrackingState.Tracked) && (joint1.TrackingState == TrackingState.Tracked))
            {
                drawPen = this.trackedBonePen;
            }

            drawingContext.DrawLine(drawPen, jointPoints[jointType0], jointPoints[jointType1]);
        }

        private void DrawClippedEdges(Body body, DrawingContext drawingContext)
        {
            FrameEdges clippedEdges = body.ClippedEdges;

            if (clippedEdges.HasFlag(FrameEdges.Bottom))
            {
                drawingContext.DrawRectangle(
                    Brushes.Red,
                    null,
                    new Rect(0, this.DisplayHeight - ClipBoundsThickness, this.DisplayWidth, ClipBoundsThickness));
            }

            if (clippedEdges.HasFlag(FrameEdges.Top))
            {
                drawingContext.DrawRectangle(
                    Brushes.Red,
                    null,
                    new Rect(0, 0, this.DisplayWidth, ClipBoundsThickness));
            }

            if (clippedEdges.HasFlag(FrameEdges.Left))
            {
                drawingContext.DrawRectangle(
                    Brushes.Red,
                    null,
                    new Rect(0, 0, ClipBoundsThickness, this.DisplayHeight));
            }

            if (clippedEdges.HasFlag(FrameEdges.Right))
            {
                drawingContext.DrawRectangle(
                    Brushes.Red,
                    null,
                    new Rect(this.DisplayWidth - ClipBoundsThickness, 0, ClipBoundsThickness, this.DisplayHeight));
            }
        }
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {

        }
    }
}
