using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Kinect;
using System.Windows.Media.Media3D;

namespace BluetoothController
{
    public static class ExtensionMethods
    {
        //public static string JointPositionsToString(IDictionary<JointType, CameraSpacePoint> jointpositions)
        //{
        //    throw new NotImplementedException();
        //    StringBuilder builder = new StringBuilder();
        //    foreach(JointType joint in jointpositions)
        //    {

        //    }
        //    return builder.ToString();
        //}

        public static bool IsSimilarByDelta(this Quaternion q, Quaternion closeto, int precision)
        {
            double delta = Math.Pow(10.0, (double)precision);

            if (Math.Abs(q.X - closeto.X) < delta &&
                Math.Abs(q.Y - closeto.Y) < delta &&
                Math.Abs(q.Z - closeto.Z) < delta &&
                Math.Abs(q.W - closeto.W) < delta)
                return true;
            else return false;
        }

        public static bool IsSimilarByDelta(this Vector3D v, Vector3D closeto, int precision)
        {
            double delta = Math.Pow(10.0, (double)precision);
            if (Math.Abs(v.X - closeto.X) < delta &&
                Math.Abs(v.Y - closeto.Y) < delta &&
                Math.Abs(v.Z - closeto.Z) < delta)
                return true;
            else return false;

 
        }
        public static double[] ToArray(this Vector3D v)
        {
            double[] vecasarray = new double[3];
            vecasarray[0] = v.X;
            vecasarray[1] = v.Y;
            vecasarray[2] = v.Z;
            return vecasarray;
        }
        public static Vector3D ToVector(this double[] a)
        {
            return new Vector3D(a[0], a[1], a[2]);
        }

        public static RollPitchYaw ToPitchYawRoll(this Quaternion q)
        {
            RollPitchYaw rpy = new RollPitchYaw();
            //Formula from:
            //http://en.wikipedia.org/wiki/Conversion_between_quaternions_and_Euler_angles
            rpy.Roll = Math.Atan2(2.0 * (q.W * q.X + q.Y * q.Z), 1.0 - 2.0 * (Math.Pow(q.X, 2.0) * Math.Pow(q.Y, 2.0)));
            rpy.Pitch = Math.Asin(2.0 * (q.W * q.Y - q.Z * q.X));
            rpy.Yaw = Math.Atan2(2.0 * (q.W * q.Z + q.X * q.Y), 1 - 2.0 * (Math.Pow(q.Y, 2.0) + Math.Pow(q.Z, 2.0)));
            return rpy;
        }

        public static JointType ToJointType(this BluetoothController.Kinect.SensorLocation sl)
        {
            switch (sl)
            {
                case Kinect.SensorLocation.ForearmLeft:
                    return JointType.WristLeft;
                case Kinect.SensorLocation.ForearmRight:
                    return JointType.WristRight;
            }
            return JointType.WristRight;
        }
    }
}
