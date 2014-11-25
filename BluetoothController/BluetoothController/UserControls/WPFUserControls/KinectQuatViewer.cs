using HelixToolkit.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using Microsoft.Kinect;

namespace BluetoothController.WPFUserControls
{
    public static class KinectQuatViewer
    {
        public static void CreateKinectQuatBody(ModelVisual3D model, BodyPosQuatData bpqd)
        {
            Color[] TrackedColors = new Color[] { Color.FromRgb(152, 152, 152),
                Color.FromRgb(30, 187, 238),
                Color.FromRgb(68, 35, 80),
                Colors.Green };
            Color[] InferredColors = new Color[] { Color.FromRgb(152, 152, 152),
                Color.FromRgb(30, 187, 238),
                Color.FromRgb(68, 35, 80),
                Colors.Red };

            model.Children.Clear();
            foreach(JointType j in bpqd.JointQuaternions.Keys)
            {
                KinectQuatViewer.CreateACoordinateArrow(model, bpqd.JointPoints[j], bpqd.JointQuaternions[j], bpqd.JointConfidences[j] == TrackingState.Tracked ? TrackedColors : InferredColors);
            }
            DrawBones(model, bpqd);
            model.Transform = new RotateTransform3D(new QuaternionRotation3D(new Quaternion(0, 1, 0, 0)));
        }

        public static void DrawBones(ModelVisual3D model, BodyPosQuatData bpqd)
        {
 	        DrawIfExistsUsingTrackingState(model, bpqd, JointType.Head, JointType.Neck);
            DrawIfExistsUsingTrackingState(model, bpqd, JointType.Neck, JointType.SpineShoulder);
            DrawIfExistsUsingTrackingState(model, bpqd, JointType.SpineShoulder, JointType.SpineMid);
            DrawIfExistsUsingTrackingState(model, bpqd, JointType.SpineMid, JointType.SpineBase);
            DrawIfExistsUsingTrackingState(model, bpqd, JointType.SpineShoulder, JointType.ShoulderRight);
            DrawIfExistsUsingTrackingState(model, bpqd, JointType.SpineShoulder, JointType.ShoulderLeft);
            DrawIfExistsUsingTrackingState(model, bpqd, JointType.SpineBase, JointType.HipRight);
            DrawIfExistsUsingTrackingState(model, bpqd, JointType.SpineBase, JointType.HipLeft);

            // Right Arm    
            DrawIfExistsUsingTrackingState(model, bpqd, JointType.ShoulderRight, JointType.ElbowRight);
            DrawIfExistsUsingTrackingState(model, bpqd, JointType.ElbowRight, JointType.WristRight);
            //DrawIfExistsUsingTrackingState(model, bpqd, JointType.WristRight, JointType.HandRight);
            //DrawIfExistsUsingTrackingState(model, bpqd, JointType.HandRight, JointType.HandTipRight);
            //DrawIfExistsUsingTrackingState(model, bpqd, JointType.WristRight, JointType.ThumbRight);

            // Left Arm
            DrawIfExistsUsingTrackingState(model, bpqd, JointType.ShoulderLeft, JointType.ElbowLeft);
            DrawIfExistsUsingTrackingState(model, bpqd, JointType.ElbowLeft, JointType.WristLeft);
            //DrawIfExistsUsingTrackingState(model, bpqd, JointType.WristLeft, JointType.HandLeft);
            //DrawIfExistsUsingTrackingState(model, bpqd, JointType.HandLeft, JointType.HandTipLeft);
            //DrawIfExistsUsingTrackingState(model, bpqd, JointType.WristLeft, JointType.ThumbLeft);

            // Right Leg
            DrawIfExistsUsingTrackingState(model, bpqd, JointType.HipRight, JointType.KneeRight);
            DrawIfExistsUsingTrackingState(model, bpqd, JointType.KneeRight, JointType.AnkleRight);
            DrawIfExistsUsingTrackingState(model, bpqd, JointType.AnkleRight, JointType.FootRight);

            // Left Leg
            DrawIfExistsUsingTrackingState(model, bpqd, JointType.HipLeft, JointType.KneeLeft);
            DrawIfExistsUsingTrackingState(model, bpqd, JointType.KneeLeft, JointType.AnkleLeft);
            DrawIfExistsUsingTrackingState(model, bpqd, JointType.AnkleLeft, JointType.FootLeft);
        }
        public static void DrawIfExistsUsingTrackingState(ModelVisual3D model, BodyPosQuatData bpqd, JointType j1, JointType j2)
        {
            if(bpqd.JointConfidences[j1] == TrackingState.Tracked && bpqd.JointConfidences[j2] == TrackingState.Tracked)
                CreateALine(model, bpqd.JointPoints[j1], bpqd.JointPoints[j2], Colors.Green);

             else if(bpqd.JointConfidences[j1] == TrackingState.Tracked && bpqd.JointConfidences[j2] == TrackingState.Inferred
                  || bpqd.JointConfidences[j1] == TrackingState.Inferred && bpqd.JointConfidences[j2] == TrackingState.Tracked )
                CreateALine(model, bpqd.JointPoints[j1], bpqd.JointPoints[j2], Colors.Yellow);

            else if(bpqd.JointConfidences[j1] == TrackingState.Inferred && bpqd.JointConfidences[j2] == TrackingState.Inferred)
                CreateALine(model, bpqd.JointPoints[j1], bpqd.JointPoints[j2], Colors.Green);
        }

        public static void CreateACoordinateArrow(ModelVisual3D model, Point3D point, Quaternion quat, Color[] colors)
        {
            if (colors.Length != 4)
                throw new ArgumentException("Incorrect Number of Colors");
            CreateACoordinateArrow(model,
                point,
                quat,
                colors[0],
                colors[1],
                colors[2],
                colors[3]);
        }

        public static void CreateACoordinateArrow(ModelVisual3D model, Point3D point, Quaternion quat, Color xaxiscolor, Color yaxiscolor, Color zaxiscolor, Color basecolor)
        {
            double ArrowLength = .10;
            double ArrowDiameter = .01;


            var rotatetransform = new RotateTransform3D(new QuaternionRotation3D(quat)) { CenterX = point.X, CenterY = point.Y, CenterZ = point.Z };
            var xaxis = new ArrowVisual3D();
            xaxis.BeginEdit();
            xaxis.Fill = new SolidColorBrush(xaxiscolor);
            xaxis.Diameter = ArrowDiameter;
            xaxis.Origin = point;
            xaxis.Direction = new Vector3D(ArrowLength, 0, 0);
            xaxis.Transform = rotatetransform;
            xaxis.EndEdit();
            model.Children.Add(xaxis);

            var yaxis = new ArrowVisual3D();
            yaxis.BeginEdit();
            yaxis.Fill = new SolidColorBrush(yaxiscolor);
            yaxis.Diameter = ArrowDiameter;
            yaxis.Origin = point;
            yaxis.Direction = new Vector3D(0, ArrowLength, 0);
            yaxis.Transform = rotatetransform;
            yaxis.EndEdit();
            model.Children.Add(yaxis);

            var zaxis = new ArrowVisual3D();
            zaxis.BeginEdit();
            zaxis.Fill = new SolidColorBrush(zaxiscolor);
            zaxis.Diameter = ArrowDiameter;
            zaxis.Origin = point;
            zaxis.Direction = new Vector3D(0, 0, ArrowLength);
            zaxis.Transform = rotatetransform;
            zaxis.EndEdit();
            model.Children.Add(zaxis);

            model.Children.Add(new CubeVisual3D {Center = point, SideLength = ArrowDiameter * 3.5, Fill = new SolidColorBrush(basecolor) });
        }
        public static void CreateALine(ModelVisual3D model, Point3D p1, Point3D p2, Color c)
        {
            model.Children.Add(new PipeVisual3D {Fill = new SolidColorBrush(c), Visible = true,  Point1 = p1, Point2 =p2, Diameter = .02, InnerDiameter = 0.0});
        }
    }
}
