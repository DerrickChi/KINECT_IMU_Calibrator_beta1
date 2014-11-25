using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BluetoothController;
using BluetoothController.Kinect;
using Microsoft.Kinect;
using System.Windows.Media.Media3D;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BluetoothController
{
    public static class MathFunctions
    {
        public enum Plane
        { xy, xz, yz}

        /// <summary>
        /// Compute the angle between 3D vectors on a 2D plane
        /// </summary>
        /// <param name="aVec1"></param>
        /// <param name="aVec2"></param>
        /// <param name="p"></param>
        /// <returns></returns>
        public static double angleBetweenVectors(double[] aVec1, double[] aVec2, Plane p)
        {
            double sum = 0, m1 = 0, m2 = 0, angle = 0;
            int a = 0, b = 0;

            switch (p)
            {
                case Plane.xy:
                    { 
                        a = 0; 
                        b = 1; 
                        break;
                    }
                case Plane.xz:
                    {
                        a = 0;
                        b = 2;
                        break;
                    }
                case Plane.yz:
                    {
                        a = 1;
                        b = 2;
                        break;
                    }
            }


            //dot product
            sum += aVec1[a] * aVec2[a];
            sum += aVec1[b] * aVec2[b];

            //magnitude
            m1 = Math.Sqrt((aVec1[a] * aVec1[a]) + (aVec1[b] * aVec1[b]));
            m2 = Math.Sqrt((aVec2[a] * aVec2[a]) + (aVec2[b] * aVec2[b]));

            angle = Math.Acos(sum / (m1 * m2));
            return angle; //radians? 
        }

        public enum MatrixType
        { Roll, Yaw }

        /// <summary>
        /// Compute an average matrix for all matrices in the calibrators buffer
        /// </summary>
        /// <param name="calibrators"></param>
        /// <param name="mt"></param>
        /// <returns></returns>
        public static double[,] matrixAvg(CircularBuffer<SensorCalibratorData> calibrators, MatrixType mt)
        {
            double[,] aggregateMatrix = new double[,] { { 0, 0, 0 }, { 0, 0, 0 }, { 0, 0, 0 } };
            double[,] avgMatrix = new double[,] { { 0, 0, 0 }, { 0, 0, 0 }, { 0, 0, 0 } };
            double[,] UsableSize = new double[,] { { 0, 0, 0 }, { 0, 0, 0 }, { 0, 0, 0 } };
            for (int i = 0; i < calibrators.Capacity; i++)
                for (int j = 0; j < 3; j++)
                    for (int k = 0; k < 3; k++)
                    {
                        if (mt == MatrixType.Roll && !double.IsNaN(calibrators[i].rollCorrection[j,k]))
                        {
                            UsableSize[j, k]++;
                            aggregateMatrix[j, k] += calibrators[i].rollCorrection[j, k];
                        }
                        else if (mt == MatrixType.Yaw && !double.IsNaN(calibrators[i].yawCorrection[j, k]))
                        {
                            UsableSize[j, k]++;
                            aggregateMatrix[j, k] += calibrators[i].yawCorrection[j, k];
                        }
                    }

            for (int j = 0; j < 3; j++)
                for (int k = 0; k < 3; k++)
                    avgMatrix[j, k] = aggregateMatrix[j, k] / UsableSize[j, k];

            return avgMatrix;
        }

        /// <summary>
        /// Compute an average linear correction constant for all constants in the calibrators buffer
        /// </summary>
        /// <param name="calibrators"></param>
        /// <returns></returns>
        public static double correctionConstAvg(CircularBuffer<SensorCalibratorData> calibrators)
        {
            double aggregateConst = 0;
            double UsableSize = 0;
            for (int i = 0; i < calibrators.Capacity; i++)
            {
                if (calibrators[i].correctionConst != double.NaN)
                {
                    aggregateConst += calibrators[i].correctionConst;
                    UsableSize++;
                }
            }

            return aggregateConst / UsableSize;
        }


        public static double linearDisplacementAvg(CircularBuffer<SensorCalibratorData> calibrators)
        {
            double aggregateDisplacement = 0;
            double UsableSize = 0;
            for (int i = 0; i < calibrators.Capacity; i++)
            {
                if (calibrators[i].correctionConst != double.NaN)
                {
                    aggregateDisplacement += calibrators[i].Displacement;
                    UsableSize++;
                }
            }

            return aggregateDisplacement / UsableSize;
        }


        /// <summary>
        /// This can be used to get the location of the point we are interested in between two joints
        /// </summary>
        /// <param name="j1">Usually further (FROM THE BODY) joint</param>
        /// <param name="j2">usually closer joint</param>
        /// <param name="pos">0 means at j1, 100 means at j2</param>
        /// <returns>a 3 vector with the location of the point of interest</returns>
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


        /// <summary>
        /// Performs a rotation that maintains vector magnitude
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static Quaternion HamiltonianMultiply(Quaternion a, Quaternion b)
        {
            Quaternion temp = a * b;
            a.Invert();
            return temp * a;
        }

        public static double[,] QuaternionToMatrix(Quaternion q)
        {
            double[,] matrix = new double[3, 3];
            double qx = q.X, qy = q.Y, qz = q.Z, qw = q.W;
            matrix[0, 0] = 1 - 2 * qy * qy - 2 * qz * qz;
            matrix[0, 1] = 2 * qx * qy - 2 * qz * qw;
            matrix[0, 2] = 2 * qx * qz + 2 * qy * qw;
            matrix[1, 0] = 2 * qx * qy + 2 * qz * qw;
            matrix[1, 1] = 1 - 2 * qx * qx - 2 * qz * qz;
            matrix[1, 2] = 2 * qy * qz - 2 * qx * qw;
            matrix[2, 0] = 2 * qx * qz - 2 * qy * qw;
            matrix[2, 1] = 2 * qy * qz + 2 * qx * qw;
            matrix[2, 2] = 1 - 2 * qx * qx - 2 * qy * qy;

            return matrix;
        }

        public static double[] MatrixVectorMultiply(double[] vector, double[,] matrix3Dim)
        {
            double[] output = new double[] { 0, 0, 0 };
            for (int i = 0; i < 3; i++)
                for (int j = 0; j < 3; j++)
                    output[i] += matrix3Dim[i, j] * vector[j];
            return output;
        }
    }

    //[global::Microsoft.VisualStudio.TestTools.UnitTesting.TestClass]
    //public class MyTestClass
    //{

    // [TestMethod]
    //  public void MyTestMethod()
    //  {
    //      double[,] testMatrix = new double[,] { { 1, 0, 0 }, { 0, 1, 0 }, { 0, 0, 1 } };
    //      double[,] testMatrix2 = new double[,] { { 1, 0, 0 }, { 0, 1, 0 }, { 0, 0, 1 } };
    //      Assert.AreEqual(testMatrix, testMatrix2);

        
    //  }   
    //}
}

