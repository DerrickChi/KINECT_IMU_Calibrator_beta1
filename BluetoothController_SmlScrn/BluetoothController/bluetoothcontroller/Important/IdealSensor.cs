using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BluetoothController
{
    public static class IdealSensor
    {
       
        public static double[] transform(double yawAngle, double rollAngle, double[] virtualAcc)
        {
            double[,] yawCorrectionMatrix = new double[3, 3];
            double[,] rollCorrectionMatrix = new double[3, 3];
            double[] tempAcc = new double[] { 0, 0, 0 };
            double[] idealAcc = new double[] { 0, 0, 0 }; 
            double yawCosRot = Math.Cos(yawAngle);
            double yawSinRot = Math.Sin(yawAngle);
            double rollCosRot = Math.Cos(rollAngle);
            double rollSinRot = Math.Sin(rollAngle);

            yawCorrectionMatrix[0, 0] = 1;
            yawCorrectionMatrix[0, 1] = 0;
            yawCorrectionMatrix[0, 2] = 0;
            yawCorrectionMatrix[1, 0] = 0;
            yawCorrectionMatrix[1, 1] = yawCosRot;
            yawCorrectionMatrix[1, 2] = -yawSinRot;
            yawCorrectionMatrix[2, 0] = 0;
            yawCorrectionMatrix[2, 1] = yawSinRot;
            yawCorrectionMatrix[2, 2] = yawCosRot;

            rollCorrectionMatrix[0, 0] = rollCosRot;
            rollCorrectionMatrix[0, 1] = -rollSinRot;
            rollCorrectionMatrix[0, 2] = 0;
            rollCorrectionMatrix[1, 0] = rollSinRot;
            rollCorrectionMatrix[1, 1] = rollCosRot;
            rollCorrectionMatrix[1, 2] = 0;
            rollCorrectionMatrix[2, 0] = 0;
            rollCorrectionMatrix[2, 1] = 0;
            rollCorrectionMatrix[2, 2] = 1;

            //yaw correction
            for (int i = 0; i < 3; i++)
                for (int j = 0; j < 3; j++)
                    tempAcc[i] += yawCorrectionMatrix[i, j] * virtualAcc[j];

            //roll correction
            for (int i = 0; i < 3; i++)
                for (int j = 0; j < 3; j++)
                    idealAcc[i] += rollCorrectionMatrix[i, j] * tempAcc[j];

            return idealAcc;
        }

    }
}
