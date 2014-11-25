using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;
using System.Windows.Media;
using MathNet.Numerics.LinearAlgebra;

namespace BluetoothController
{
    public class OrientationStabilizer
    {
        double[] TotalAngleChange;
        double[] CurrentRotationVelocity;
        double[] PreviousRotationVelocity;
        double CurrentTime;
        double PreviousTime;
        //double[,] RollRotationMatrix; // xy
        //double[,] YawRotationMatrix; //yz
        //double[,] PitchRotationMatrix; //xz
        double[,] TotalRotationMatrix;
        const double angularConst = Math.PI / (180.0*16.4);

        public OrientationStabilizer()
        {
            TotalAngleChange = new double[] { 0, 0, 0 };
            CurrentRotationVelocity = new double[] { 0, 0, 0 };
            PreviousRotationVelocity = new double[] { 0, 0, 0 };
            //RollRotationMatrix = new double[,] {{1,0,0},{0,1,0},{0,0,1}};
            //YawRotationMatrix = new double[,] {{1,0,0},{0,1,0},{0,0,1}};
            //PitchRotationMatrix = new double[,] {{1,0,0},{0,1,0},{0,0,1}};
            TotalRotationMatrix = new double[,] { { 1, 0, 0 }, { 0, 1, 0 }, { 0, 0, 1 } };
            CurrentTime = 0;
            PreviousTime = 0;
        }

        public void Track(short[] gyroscopeData, long time)
        {
            double[] midAngularVelocity = new double[] { 0, 0, 0 };//middle Riemann Sum
            CurrentTime = (double)time * (1.0/(double)TimeSpan.TicksPerSecond);
            for (int i = 0; i < 3; i++)
            {
                CurrentRotationVelocity[i] = (double)gyroscopeData[i] * angularConst;//converting to rad/s
                midAngularVelocity[i] = (CurrentRotationVelocity[i] + PreviousRotationVelocity[i])/2;
                TotalAngleChange[i] += midAngularVelocity[i] * (CurrentTime - PreviousTime);
                PreviousRotationVelocity[i] = CurrentRotationVelocity[i];
            }
            PreviousTime = CurrentTime;
        }

        public void GenerateRotationMatrices()
        {
            double yawAngle = TotalAngleChange[0]; //yz: rotation around x
            double yawCos = Math.Cos(yawAngle);
            double yawSin = Math.Sin(yawAngle);
            double pitchAngle = TotalAngleChange[1]; //xz: rotation around y
            double pitchCos = Math.Cos(pitchAngle);
            double pitchSin = Math.Sin(pitchAngle);
            double rollAngle = TotalAngleChange[2]; //xy: rotation around z
            double rollCos = Math.Cos(rollAngle);
            double rollSin = Math.Sin(rollAngle);

            TotalRotationMatrix[0, 0] = pitchCos * rollCos;
            TotalRotationMatrix[0, 1] = -pitchCos * rollSin;
            TotalRotationMatrix[0, 2] = pitchSin;
            TotalRotationMatrix[1, 0] = yawCos * rollSin + yawSin * pitchSin * rollCos;
            TotalRotationMatrix[1, 1] = yawCos * rollCos - yawSin * pitchSin * rollSin;
            TotalRotationMatrix[1, 2] = -yawSin * pitchCos;
            TotalRotationMatrix[2, 0] = yawSin * rollSin - yawCos * pitchSin * rollCos;
            TotalRotationMatrix[2, 1] = yawSin * rollCos + yawCos * pitchSin * rollSin;
            TotalRotationMatrix[2, 2] = yawCos * pitchCos;

           // TotalRotationMatrix = MathFunctions.invert3DMatrix(TotalRotationMatrix);

        }

        public double[] MapAcceleration(double[] NormalizedAcceleration)
        {
            return MathFunctions.MatrixVectorMultiply(NormalizedAcceleration, TotalRotationMatrix); 
        }

        public double[] Stabilize(double[] accelerometer, short[] gyroscope, long time)
        {
            Track(gyroscope, time);
            GenerateRotationMatrices();
            return MapAcceleration(accelerometer);
        }

        public double[,] RotationMatrix
        {
            get { return TotalRotationMatrix; }
            protected set { TotalRotationMatrix = value; }
        }
    }
}
