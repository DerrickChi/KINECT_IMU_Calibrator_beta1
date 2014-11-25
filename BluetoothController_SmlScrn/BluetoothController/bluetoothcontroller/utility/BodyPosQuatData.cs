using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Kinect;
using System.Windows.Media.Media3D;

namespace BluetoothController
{
    public class BodyPosQuatData
    {
        public Dictionary<JointType, Quaternion> JointQuaternions;
        public Dictionary<JointType, Point3D> JointPoints;
        public Dictionary<JointType, TrackingState> JointConfidences;


        public BodyPosQuatData()
        {
            this.JointQuaternions = new Dictionary<JointType, Quaternion>();
            this.JointPoints = new Dictionary<JointType, Point3D>();
            this.JointConfidences = new Dictionary<JointType,TrackingState>();
        }

        public void AddIfTrackedOrInferred(Joint jointtoadd, JointOrientation jointtoaddoritent, Joint jointparent)
        {
            if(jointtoadd.TrackingState != TrackingState.NotTracked && jointparent.TrackingState != TrackingState.NotTracked)
                this.JointQuaternions[jointtoadd.JointType] = this.JointQuaternions[jointparent.JointType] * jointtoaddoritent.Orientation.ToQuaternion();
            this.JointQuaternions[jointtoadd.JointType] =  jointtoaddoritent.Orientation.ToQuaternion();
            //(new Quaternion(0,1,0,0)) *
        }
        public static BodyPosQuatData GetBodyPosQuatDataFromBody(Body b)
        {
            try
            {
                BodyPosQuatData bpqd = new BodyPosQuatData();
                foreach (Joint j in b.Joints.Values)
                    bpqd.JointPoints[j.JointType] = j.Position.ToPoint3D();
                foreach (Joint j in b.Joints.Values)
                    bpqd.JointConfidences[j.JointType] = j.TrackingState;

                //Torso
                bpqd.JointQuaternions[JointType.SpineBase] = b.JointOrientations[JointType.SpineBase].Orientation.ToQuaternion();
                bpqd.AddIfTrackedOrInferred(b.Joints[JointType.SpineMid], b.JointOrientations[JointType.SpineMid], b.Joints[JointType.SpineBase]);
                bpqd.AddIfTrackedOrInferred(b.Joints[JointType.HipLeft], b.JointOrientations[JointType.HipLeft], b.Joints[JointType.SpineBase]);
                bpqd.AddIfTrackedOrInferred(b.Joints[JointType.HipRight], b.JointOrientations[JointType.HipRight], b.Joints[JointType.SpineBase]);
                bpqd.AddIfTrackedOrInferred(b.Joints[JointType.SpineShoulder], b.JointOrientations[JointType.SpineShoulder], b.Joints[JointType.SpineMid]);
                bpqd.AddIfTrackedOrInferred(b.Joints[JointType.ShoulderLeft], b.JointOrientations[JointType.ShoulderLeft], b.Joints[JointType.SpineShoulder]);
                bpqd.AddIfTrackedOrInferred(b.Joints[JointType.ShoulderRight], b.JointOrientations[JointType.ShoulderRight], b.Joints[JointType.SpineShoulder]);
                bpqd.AddIfTrackedOrInferred(b.Joints[JointType.Neck], b.JointOrientations[JointType.Neck], b.Joints[JointType.SpineShoulder]);
                bpqd.AddIfTrackedOrInferred(b.Joints[JointType.Head], b.JointOrientations[JointType.Head], b.Joints[JointType.Neck]);

                //Right Arm
                bpqd.AddIfTrackedOrInferred(b.Joints[JointType.ElbowRight], b.JointOrientations[JointType.ElbowRight], b.Joints[JointType.ShoulderRight]);
                bpqd.AddIfTrackedOrInferred(b.Joints[JointType.WristRight], b.JointOrientations[JointType.WristRight], b.Joints[JointType.ElbowRight]);
                //bpqd.AddIfTrackedOrInferred(b.Joints[JointType.HandRight], b.JointOrientations[JointType.HandRight], b.Joints[JointType.WristRight]);
                //bpqd.AddIfTrackedOrInferred(b.Joints[JointType.HandTipRight], b.JointOrientations[JointType.HandTipRight], b.Joints[JointType.HandRight]);

                //Left Arm
                bpqd.AddIfTrackedOrInferred(b.Joints[JointType.ElbowLeft], b.JointOrientations[JointType.ElbowLeft], b.Joints[JointType.ShoulderLeft]);
                bpqd.AddIfTrackedOrInferred(b.Joints[JointType.WristLeft], b.JointOrientations[JointType.WristLeft], b.Joints[JointType.ElbowLeft]);
                //bpqd.AddIfTrackedOrInferred(b.Joints[JointType.HandLeft], b.JointOrientations[JointType.HandLeft], b.Joints[JointType.WristLeft]);
                //bpqd.AddIfTrackedOrInferred(b.Joints[JointType.HandTipLeft], b.JointOrientations[JointType.HandTipLeft], b.Joints[JointType.HandLeft]);

                //Right Leg
                bpqd.AddIfTrackedOrInferred(b.Joints[JointType.KneeRight], b.JointOrientations[JointType.KneeRight], b.Joints[JointType.HipRight]);
                bpqd.AddIfTrackedOrInferred(b.Joints[JointType.AnkleRight], b.JointOrientations[JointType.AnkleRight], b.Joints[JointType.KneeRight]);
                bpqd.AddIfTrackedOrInferred(b.Joints[JointType.FootRight], b.JointOrientations[JointType.FootRight], b.Joints[JointType.AnkleRight]);

                //Left Leg
                bpqd.AddIfTrackedOrInferred(b.Joints[JointType.KneeLeft], b.JointOrientations[JointType.KneeLeft], b.Joints[JointType.HipLeft]);
                bpqd.AddIfTrackedOrInferred(b.Joints[JointType.AnkleLeft], b.JointOrientations[JointType.AnkleLeft], b.Joints[JointType.KneeLeft]);
                bpqd.AddIfTrackedOrInferred(b.Joints[JointType.FootLeft], b.JointOrientations[JointType.FootLeft], b.Joints[JointType.AnkleLeft]);

                return bpqd;
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("GetBodyPosQuatData threw: {0}", e);
                throw e;
            }
        }

        //blic bool IsTracked
    }

    public static class BodyPostQataDataExtensions
    {
        public static Quaternion ToQuaternion (this Vector4 v4)
        {
            return new Quaternion(v4.X, v4.Y, v4.Z, v4.W);
        }
        public static Point3D ToPoint3D(this CameraSpacePoint csp)
        {
            return new Point3D(csp.X, csp.Y, csp.Z);
        }
        public static BluetoothController.Mathematics.Quaternion ToBTQuaternion(this Quaternion q)
        {
            return new Mathematics.Quaternion(q);
        }
    }
}
