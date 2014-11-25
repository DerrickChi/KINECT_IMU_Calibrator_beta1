using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BluetoothController;
using BluetoothController.Kinect;

namespace BluetoothController
{
 

    public class SensorCalibrator
    {
        private double[] actualLinearAcc;
        private double[] idealLinearAcc;
        private double[] correctedLinearAcc; //should be ideal
        private double[] angularAcc;
        private double linearCorrectionConst;
        private double[,] yawCorrectionMatrix;
        private double[,] rollCorrectionMatrix;
        private double yawAngle;
        private double rollAngle;

        public SensorCalibrator()
        {
            linearCorrectionConst = 1;
            double[,] yawCorrectionMatrix = new double[,] { { 1, 0, 0 }, { 0, 1, 0 }, { 0, 0, 1 } };
            double[,] rollCorrectionMatrix = new double[,] { { 1, 0, 0 }, { 0, 1, 0 }, { 0, 0, 1 } };
            actualLinearAcc = new double[] { 0, 0, 0 };
            idealLinearAcc = new double[] { 0, 0, 0 };
            correctedLinearAcc = new double[] { 0, 0, 0 };
            angularAcc = new double[] { 0, 0, 0 };
            yawAngle = 0;
            rollAngle = 0;
        }

        public void setData(double[] actualVec, double[] angularActualVec, double[] idealVec)
        {
            angularAcc = angularActualVec;
            actualLinearAcc = actualVec;
            idealLinearAcc = idealVec;
        }

        public void generateLinearCorrection()
        {
            //[0] = x, [1] = y, [2] = z
            double actualAvgRadius = 0;
            double idealAvgRadius = 0;

            //Calculation of correctionConst
            for (int i = 0; i < 3; i++)
            {
                actualAvgRadius += actualLinearAcc[i] / angularAcc[i];
                idealAvgRadius += idealLinearAcc[i] / angularAcc[i];
            }

            //actualAvgRadius = sum1 / 3;
            //idealAvgRadius = sum2 / 3;
            linearCorrectionConst = idealAvgRadius / actualAvgRadius;
        }

        //rotation of the yz plane
        public void generateYawCorrection()
        {
            yawAngle = MathFunctions.angleBetweenVectors(actualLinearAcc, idealLinearAcc, MathFunctions.Plane.yz);
            double cosRot = Math.Cos(yawAngle);
            double sinRot = Math.Sin(yawAngle);

            yawCorrectionMatrix[0, 0] = 1;
            yawCorrectionMatrix[0, 1] = 0;
            yawCorrectionMatrix[0, 2] = 0;
            yawCorrectionMatrix[1, 0] = 0;
            yawCorrectionMatrix[1, 1] = cosRot;
            yawCorrectionMatrix[1, 2] = -sinRot;
            yawCorrectionMatrix[2, 0] = 0;
            yawCorrectionMatrix[2, 1] = sinRot;
            yawCorrectionMatrix[2, 2] = cosRot;
        }

        //rotation of the xy plane
        public void generateRollCorrection()
        {
            rollAngle = MathFunctions.angleBetweenVectors(actualLinearAcc, idealLinearAcc, MathFunctions.Plane.xy);
            double cosRot = Math.Cos(rollAngle);
            double sinRot = Math.Sin(rollAngle);

            rollCorrectionMatrix[0, 0] = cosRot;
            rollCorrectionMatrix[0, 1] = -sinRot;
            rollCorrectionMatrix[0, 2] = 0;
            rollCorrectionMatrix[1, 0] = sinRot;
            rollCorrectionMatrix[1, 1] = cosRot;
            rollCorrectionMatrix[1, 2] = 0;
            rollCorrectionMatrix[2, 0] = 0;
            rollCorrectionMatrix[2, 1] = 0;
            rollCorrectionMatrix[2, 2] = 1;
        }

        public double correctionConst
        {
            get { return linearCorrectionConst;  }
            set { linearCorrectionConst = value; }
        }

        public double[,] yawCorrection
        {
            get { return yawCorrectionMatrix; }
            set { yawCorrectionMatrix = value;  }
        }

        public double[,] rollCorrection
        {
            get { return rollCorrectionMatrix; }
            set { rollCorrectionMatrix = value; }
        }


        public void generateCorrections()
        {
            generateLinearCorrection();
            generateYawCorrection();
            generateRollCorrection();
        }

        /// <summary>
        /// This parameter represents a raw acceleration that is not the same acceleration as the actualLinearAcceleration member
        /// </summary>
        /// <param name="actualAcc"></param>
        /// <returns></returns>
        public double[] transform(double[] actualAcc)
        {
            double[] tempAcc = new double[] {0,0,0};
            double[] tempAcc2 = new double[] {0,0,0};

            //linear correction
            for (int i = 0; i < 3; i++)
                tempAcc[i] = actualAcc[i] * linearCorrectionConst;

            //yaw correction
            for (int i = 0; i < 3; i++)
                for (int j = 0; j < 3; j++)
                    tempAcc2[i] += yawCorrectionMatrix[i, j] * tempAcc[j];

            //roll correction
            for (int i = 0; i < 3; i++)
                for (int j = 0; j < 3; j++)
                    correctedLinearAcc[i] += rollCorrectionMatrix[i, j] * tempAcc2[j];

            return correctedLinearAcc;
        }
        
    }
}
