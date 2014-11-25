using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BluetoothController;
using BluetoothController.Kinect;

namespace BluetoothController 
{
 
    /// <summary>
    /// Calculates correction variables to virtually adjust motion sensors into the desired orientation and position
    /// </summary>
    public class SensorCalibratorData : IData
    {
        private double[] actualLinearAcc;
        private double[] idealLinearAcc;
        private double[] correctedLinearAcc; //should be ideal
        private double[] angularAcc;
        private double linearCorrectionConst;
        private double linearDisplacement;
        private double[,] yawCorrectionMatrix;
        private double[,] rollCorrectionMatrix;
        private double yawAngle;
        private double rollAngle;
        public bool CalibrationFailed { get; set; }
        public bool DataIsGood { get; set; }

      
        public SensorCalibratorData()
        {
            linearCorrectionConst = 1;
            linearDisplacement = 0;
            yawCorrectionMatrix = new double[,] { { 1, 0, 0 }, { 0, 1, 0 }, { 0, 0, 1 } };
            rollCorrectionMatrix = new double[,] { { 1, 0, 0 }, { 0, 1, 0 }, { 0, 0, 1 } };
            actualLinearAcc = new double[] { 0, 0, 0 };
            idealLinearAcc = new double[] { 0, 0, 0 };
            correctedLinearAcc = new double[] { 0, 0, 0 };
            angularAcc = new double[] { 0, 0, 0 };
            yawAngle = 0;
            rollAngle = 0;
            CalibrationFailed = false;
        }

        public void setData(double[] actualVec, double[] angularActualVec, double[] idealVec, bool goodData)
        {
            angularAcc = angularActualVec;
            actualLinearAcc = actualVec;
            idealLinearAcc = idealVec;
            DataIsGood = goodData;
        }

        /// <summary>
        /// Computes a linear correction constant that vertically shifts the IMUs 
        /// </summary>
        public void generateLinearCorrection()
        {
            //[0] = x, [1] = y, [2] = z
            double actualAvgRadius = 0;
            double idealAvgRadius = 0;
            double actualAvgRadiusAggregate = 0, linearAvgRadiusAggregate = 0;

            //Calculation of correctionConst
            for (int i = 0; i < 3; i++)
            {
                actualAvgRadiusAggregate += actualLinearAcc[i] / angularAcc[i];
                linearAvgRadiusAggregate += idealLinearAcc[i] / angularAcc[i];
            }

            actualAvgRadius = actualAvgRadiusAggregate / 3;
            idealAvgRadius = linearAvgRadiusAggregate / 3;

            linearDisplacement = Math.Abs(idealAvgRadius - actualAvgRadius);
            linearCorrectionConst = idealAvgRadius / actualAvgRadius;
        }

        /// <summary>
        ///Generates a rotation matrix that rotates acceleration vectors in the yz plane (yaw)
        /// </summary>
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

        /// <summary>
        ///Generates a rotation matrix that rotates acceleration vectors in the xy plane (roll)
        /// </summary>
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

        public double ExtractYawAngle()
        { return Math.Acos(yawCorrectionMatrix[1, 1]) * (180.0/Math.PI); }

        public double ExtractRollAngle()
        { return Math.Acos(rollCorrectionMatrix[1, 1])*(180.0/Math.PI); }

        public double correctionConst
        {
            get { return linearCorrectionConst;  }
            set { linearCorrectionConst = value; }
        }

        public double Displacement
        {
            get { return linearDisplacement; }
            set { linearDisplacement = value; }
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

        /// <summary>
        /// Generates corrections for linear, yaw, and roll misplacements
        /// </summary>
        public void generateCorrections()
        {
            generateLinearCorrection();
            generateYawCorrection();
            generateRollCorrection();
        }

        /// <summary>
        /// Applies the correction varibles to future raw data
        /// </summary>
        /// <param name="actualAcc"> This parameter represents a raw acceleration that is not the same acceleration as the actualLinearAcceleration member</param>
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


        public string ToWindowsFormString()
        {
            throw new NotImplementedException();
        }

        public string ToFileString()
        {
            throw new NotImplementedException();
        }

        public string ToPreviewString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("Linear Displacement(m): {0}, Yaw Displacement(rad): {1}, Roll Displacement(rad): {2}", linearDisplacement, ExtractYawAngle(), ExtractRollAngle());
            return sb.ToString();
        }

        public long NowInTicks
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        #region IData Members


        public string ToFileHeaderString()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
