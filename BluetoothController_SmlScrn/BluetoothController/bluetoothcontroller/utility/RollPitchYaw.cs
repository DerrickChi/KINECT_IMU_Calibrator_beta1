using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BluetoothController
{
    public struct RollPitchYaw
    {
        public double Roll;
        public double Pitch;
        public double Yaw;
        public RollPitchYaw(double roll, double pitch, double yaw)
        {
            this.Roll = roll;
            this.Pitch = pitch;
            this.Yaw = yaw;
        }
        public override string ToString()
        {
            return String.Format("Φ(R):{0}, θ(P):{1}, ψ(Y):{2}", Roll, Pitch, Yaw);
        }
        public string ToEnvironNewLineString()
        {
            return String.Format("Φ(Roll):{1},{0} θ(Pitch):{2},{0} ψ(Yaw):{3}", Environment.NewLine, Roll, Pitch, Yaw);
        }
    }
}
