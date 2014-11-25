using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Kinect;

namespace BluetoothController.Mathematics
{
    /// <summary>
    /// 
    /// </summary>
    public class Quaternion
    {
        private object _Lock = new object();
        public Quaternion() { }
        public Quaternion(double w, double x, double y, double z)
        {
            W = w;
            X = x;
            Y = y;
            Z = z;
        }
        public Quaternion(double[] wxyz)
        {
            W = wxyz[0];
            X = wxyz[1];
            Y = wxyz[2];    
            Z = wxyz[3];
        }
        public Quaternion(Vector4 v4)
        {
            W = v4.W;
            X = v4.X;
            Y = v4.Y;
            Z = v4.Z;
        }
        public Quaternion(System.Windows.Media.Media3D.Quaternion q)
        {
            W = q.W;
            X = q.X;
            Y = q.Y;
            Z = q.Z;
        }
        public Quaternion(Quaternion q)
        {
            this.W = q.W;
            this.X = q.X;
            this.Y = q.Y;
            this.Z = q.Z;
        }
        public double W { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }
        public static Quaternion operator* (Quaternion l, Quaternion r)
        {
            return new Quaternion(
                l.W * r.W -     l.X * r.X -     l.Y * r.Y -     l.Z * r.Z,
                l.W * r.X +     l.X * r.W +     l.Y * r.Z -     l.Z * r.Y,
                l.W * r.Y -     l.X * r.Z +     l.Y * r.W +     l.Z * r.X,
                l.W * r.Z +     l.X * r.Y +     l.Y * r.X +     l.Z * r.W
                );
        }
        //public static Quaternion operator* (Vector4 l, Vector4 r)
        //{
        //    return ((Quaternion)l) * ((Quaternion)r);
        //}
        public static implicit operator System.Windows.Media.Media3D.Quaternion?(Quaternion q)
        {
            if(q == null)
                return null;
            return new System.Windows.Media.Media3D.Quaternion(q.X, q.Y, q.Z, q.W);
        }
        public static implicit operator Quaternion(Vector4 v4)
        {
            return new Quaternion(v4);
        }
        public Quaternion Inverse()
        {
            lock (_Lock)
            {
                Quaternion returnquaternion = new Quaternion(this);
                //returnquaternion.X = -returnquaternion.X; returnquaternion.Y = -returnquaternion.Y; returnquaternion.Z = -returnquaternion.Z;

                returnquaternion.W = -returnquaternion.W;
                return returnquaternion;
            }
        }
        public static  Quaternion Identity
        { 
            get
            {
                return new Quaternion(1,0,0,0);
            }
        }

  

        public Quaternion Normalize()
        {
            Quaternion returnquat;
            lock (_Lock)
            {
                returnquat = new Quaternion(this);
            }
            double magnitude = Math.Sqrt(returnquat.W * returnquat.W + returnquat.X * returnquat.X + returnquat.Y * returnquat.Y + returnquat.Z * returnquat.Z);
            returnquat.W /= magnitude;
            returnquat.X /= magnitude;
            returnquat.Y /= magnitude;
            returnquat.Z /= magnitude;
            return this;
        }
    }
}
