using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BluetoothController
{
    /// <summary>
    /// This class how the mapped and raw accelerometers move data.
    /// This class is typically passed when the events are fired.
    /// </summary>
    public class InertialSensorData : IData
    {

        #region Constructors
        public InertialSensorData()
        {
            Accelarometers = new short[3];
            gyropscopes = new short[3];
            quaternion = new int[4];
            NowInTicks = DateTime.UtcNow.Ticks;
        }

        public InertialSensorData(InertialSensorData c)
        {

            this.accelarometers = new short[c.accelarometers.Length];
            for (int i = 0; i < this.accelarometers.Length; i++)
                this.accelarometers[i] = c.accelarometers[i];

            this.gyropscopes = new short[c.gyropscopes.Length];
            for (int i = 0; i < this.gyropscopes.Length; i++)
                this.gyropscopes[i] = c.gyropscopes[i];

            this.quaternion = new int[c.quaternion.Length];
            for (int i = 0; i < this.quaternion.Length; i++)
                this.quaternion[i] = c.quaternion[i];

            this.NowInTicks = c.NowInTicks;
        }
        #endregion

        #region Accelerations
        private short[] accelarometers;
        public short[] Accelarometers
        {
            get
            {
                return accelarometers;
            }
            set
            {
                accelarometers = value;
            }
        }
        public double[] NormalizedAccelerations
        {
            get
            {
                return new double[] { accelarometers[0] / 2048.0 * 9.8, accelarometers[1] / 2048.0 * 9.8, accelarometers[2] / 2048.0 * 9.8 }; ;
            }
            set 
            {
                accelarometers = new short[] {(short)(value[0] * 2048.0 / 9.8), (short)(value[1] * 2048.0 / 9.8), (short)(value[2] * 2048.0 / 9.8) };
            }
        }
        public System.Windows.Media.Media3D.Vector3D AccelerationAsVMD3
        {
            get 
            {
                return new System.Windows.Media.Media3D.Vector3D(this.NormalizedAccelerations[0], this.NormalizedAccelerations[1], this.NormalizedAccelerations[2]);
            }
            set
            {
                this.accelarometers = new short[]
                {
                    (short)(value.X * 2048.0 / 9.8),
                    (short)(value.Y * 2048.0 / 9.8),
                    (short)(value.Z * 2048.0 / 9.8)
                };
            }
        }
        #endregion

        #region Gryoscopres

        public short[] gyropscopes;

        #endregion

        #region Quaternion

        public int[] quaternion;
        public double[] NormalizedQuaternion
        {
            get
            {
                return new double[] { quaternion[0] / 1073741824.0, quaternion[1] / 1073741824.0, quaternion[2] / 1073741824.0, quaternion[3] / 1073741824.0 };
            }
        }

        public System.Windows.Media.Media3D.Quaternion QuaternionAsQMD3
        {
            get 
            {
                return new System.Windows.Media.Media3D.Quaternion(this.NormalizedQuaternion[1],this.NormalizedQuaternion[2],this.NormalizedQuaternion[3],this.NormalizedQuaternion[0]);
            }
            set
            {
                quaternion[0] = (int)(value.W * 1073741824.0);
                quaternion[1] = (int)(value.X * 1073741824.0);
                quaternion[2] = (int)(value.Y * 1073741824.0);
                quaternion[3] = (int)(value.Z * 1073741824.0);
            }
        }

        #endregion

        #region IData Members

        public long NowInTicks { get; set; }

        string IData.ToPreviewString()
        {
            double[] q = new double[] { quaternion[0] / 1073741824.0, quaternion[1] / 1073741824.0, quaternion[2] / 1073741824.0, quaternion[3] / 1073741824.0 };
            StringBuilder sb = new StringBuilder();
            //sb.AppendFormat("ax:{0}, ay:{1}, az: {2}", Accelarometers[0], Accelarometers[1], Accelarometers[2]);
            sb.AppendFormat("qw:{0}, qx:{2}, qy: {2}, qz: {3}", quaternion[0] / 1073741824.0, quaternion[1] / 1073741824.0, quaternion[2] / 1073741824.0, quaternion[3] / 1073741824.0);
            sb.Append("\r\n");
            sb.AppendFormat("angle:{0}, x:{1}, y:{2}, z:{3}",
                2 * Math.Acos(q[0]) * 180 / Math.PI,
                q[1] / (Math.Sqrt(1 - Math.Pow(q[0], 2.0))),
                q[2] / (Math.Sqrt(1 - Math.Pow(q[0], 2.0))),
                q[3] / (Math.Sqrt(1 - Math.Pow(q[0], 2.0)))
                );
            return sb.ToString();
        }



        string IData.ToWindowsFormString()
        {
            throw new NotImplementedException();
        }

        string IData.ToFileHeaderString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("{0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}, {9}, {10}",
                "Read time",
                "Ax",
                "Ay",
                "Az",
                "Gx",
                "Gy",
                "Gz",
                "Qw",
                "Qx",
                "Qy",
                "Qz");
            sb.AppendLine();
            return sb.ToString();
        }

        string IData.ToFileString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("{0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}, {9}, {10}",
                NowInTicks,
                Accelarometers[0],  //1
                Accelarometers[1],
                Accelarometers[2],
                gyropscopes[0],      //4
                gyropscopes[1],
                gyropscopes[2],
                quaternion[0],      //7
                quaternion[1],
                quaternion[2],
                quaternion[3]);
            sb.AppendLine();
            return sb.ToString();
        }
        #endregion

    }
}