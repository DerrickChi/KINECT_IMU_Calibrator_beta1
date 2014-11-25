using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BluetoothController;
using BluetoothController.Kinect;
using Microsoft.Kinect;

namespace SensorCalibration
{
    public static class mathFunctions
    {
        public static double angleBetweenVectors(double[] aVec1, double[] aVec2)
        {
            double sum = 0, m1 = 0, m2 = 0, angle = 0;

            //dot product
            for (int i = 0; i < 3; i++)
                sum += aVec1[i] * aVec2[i];

            //magnitude
            m1 = Math.Sqrt((aVec1[0] * aVec1[0]) + (aVec1[1] * aVec1[1]) + (aVec1[2] * aVec1[2]));
            m2 = Math.Sqrt((aVec2[0] * aVec2[0]) + (aVec2[1] * aVec2[1]) + (aVec2[2] * aVec2[2]));

            angle = Math.Acos(sum / (m1 * m2));
            return angle; //radians? 
        }

        public enum MatrixType
        { Roll, Yaw }

        public static double[,] matrixAvg(CircularBuffer<SensorCalibrator> calibrators, MatrixType mt)
        {
            double[,] aggregateMatrix = new double[,] { { 0, 0, 0 }, { 0, 0, 0 }, { 0, 0, 0 } };
            double[,] avgMatrix = new double[,] { { 0, 0, 0 }, { 0, 0, 0 }, { 0, 0, 0 } };
            for (int i = 0; i < calibrators.size; i++)
                for (int j = 0; j < 3; j++)
                    for (int k = 0; k < 3; k++)
                    {
                        if (mt == MatrixType.Roll)
                            aggregateMatrix[j, k] += calibrators[i].rollCorrection[j, k];
                        else
                            aggregateMatrix[j, k] += calibrators[i].yawCorrection[j, k];
                    }

            for (int j = 0; j < 3; j++)
                for (int k = 0; k < 3; k++)
                    avgMatrix[j, k] = aggregateMatrix[j, k] / calibrators.size;

            return avgMatrix;
        }

        public static double correctionConstAvg(CircularBuffer<SensorCalibrator> calibrators)
        {
            double aggregateConst = 0;
            for (int i = 0; i < calibrators.size; i++)
            { aggregateConst += calibrators[i].correctionConst; }

            return aggregateConst / calibrators.size;
        }



        public static double[] midpoint(Joint j1, Joint j2, int pos)
        {
            double[] midPos = new double[3];

            //X
            midPos[0] = (j2.Position.X - j1.Position.X) * (double)(pos / 100) + j1.Position.X;
            //Y
            midPos[1] = (j2.Position.Y - j1.Position.Y) * (double)(pos / 100) + j1.Position.Y;
            //Z
            midPos[2] = (j2.Position.Z - j1.Position.Z) * (double)(pos / 100) + j1.Position.Z;

            return midPos;
        }

    }
}
