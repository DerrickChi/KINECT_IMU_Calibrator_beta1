using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BluetoothController
{
    public class Matrix3Dim : System.Object
    {
        private double[,] matrix3Dim;

        public Matrix3Dim()
        {
            matrix3Dim = new double[,] { { 0, 0, 0, }, { 0, 0, 0, }, { 0, 0, 0, } };
        }

        public Matrix3Dim(double m11, double m12, double m13, double m21, double m22, double m23, double m31, double m32, double m33)
        {
            matrix3Dim = new double[,] { { m11, m12, m13 }, { m21, m22, m23 }, { m31, m32, m33 } };
        }

        public double[,] OriginalMatrix
        {
            get { return matrix3Dim; }
            set { matrix3Dim = value; }
        }

        //public override bool Equals(Matrix3Dim obj)
        //{
        //    bool isEqual = true;
        //    for (int i = 0; i < 3; i++)
        //        for (int j = 0; j < 3; j++)
        //        {
        //            if (matrix3Dim[i, j] == obj.OriginalMatrix[i, j])
        //                isEqual = false;
        //        }

        //    return isEqual;
        //}

        /// <summary>
        /// Computes determinant of a 2x2 matrix
        /// </summary>
        /// <param name="matrix2D"></param>
        /// <returns></returns>
        public static double determinant2D(double[,] matrix2D)
        { return matrix2D[0, 0] * matrix2D[1, 1] - matrix2D[0, 1] * matrix2D[1, 0]; }

        /// <summary>
        /// Computes the determinant of a 3x3 matrix
        /// </summary>
        /// <param name="matrix3D"></param>
        /// <returns></returns>
        public double determinant3D()
        {
            double[][,] determinantList = new double[3][,];
            double determinant = 0;

            for (int i = 0; i < 3; i++)
                determinantList[i] = new double[2, 2];

            for (int i = 0; i < 3; i++) //iterates through determinantList
            {
                int m = 0, n = 0;
                for (int k = 0; k < 3; k++) //iterates throught matrix3D rows
                    for (int l = 1; l < 3; l++) //iterates throught matrix3D cols
                    {
                        if (k != i)
                        {
                            determinantList[i][m, n] = matrix3Dim[k, l];
                            if (n == 1)
                            { m++; n = 0; }
                            else
                                n++;

                        }
                    }
            }
            for (int i = 0; i < 3; i++)
                determinant += matrix3Dim[i, 0] * determinant2D(determinantList[i]);

            return determinant;
        }

        /// <summary>
        /// Applies a 3x3 matrix to a 3x1 vector
        /// </summary>
        /// <param name="input"></param>
        /// <param name="matrix"></param>
        /// <returns></returns>
        public double[] MultiplyToVector(double[] vector)
        {
            double[] output = new double[] { 0, 0, 0 };
            for (int i = 0; i < 3; i++)
                for (int j = 0; j < 3; j++)
                    output[i] += matrix3Dim[i, j] * vector[j];
            return output;
        }

        /// <summary>
        /// Computes the adjoint of a 3x3 matrix used for the computation of the inverse
        /// </summary>
        /// <param name="matrix3D"></param>
        /// <returns></returns>
        public double[,] GenerateAdjoint()
        {
            double[,][,] cofactorlist = new double[3, 3][,];
            double[,] cofactor = new double[3, 3];
            double[,] adjoint = new double[3, 3];

            for (int i = 0; i < 3; i++) //iterates through cofactorlist rows
                for (int j = 0; j < 3; j++) //iterates through cofactorlist cols
                    cofactorlist[i, j] = new double[2, 2];


            for (int i = 0; i < 3; i++) //iterates through cofactorlist rows
                for (int j = 0; j < 3; j++) //iterates through cofactorlist cols
                {
                    int m = 0, n = 0;
                    for (int k = 0; k < 3; k++) //iterates throught matrix3D rows
                        for (int l = 0; l < 3; l++) //iterates throught matrix3D cols
                        {
                            if (k != i && l != j)
                            {
                                cofactorlist[i, j][m, n] = matrix3Dim[k, l];
                                if (n == 1)
                                { m++; n = 0; }
                                else
                                    n++;
                            }
                        }
                }

            for (int i = 0; i < 3; i++)
                for (int j = 0; j < 3; j++)
                    cofactor[i, j] = Math.Pow(-1.0, i + j) * determinant2D(cofactorlist[i, j]);

            for (int i = 0; i < 3; i++)
                for (int j = 0; j < 3; j++)
                    adjoint[i, j] = cofactor[j, i];

            return adjoint;
        }

        /// <summary>
        /// Computes the inverse of a 3x3 matrix. Used for inverting the rotation matrix that maintains the fixed coordinate system of the sensor.
        /// </summary>
        /// <param name="matrix"></param>
        /// <returns></returns>
        public double[,] invert3DMatrix()
        {
            double determinant = determinant3D();
            double[,] adjoint = GenerateAdjoint();
            double[,] invertedMatrix = new double[3, 3];

            for (int i = 0; i < 3; i++)
                for (int j = 0; j < 3; j++)
                    invertedMatrix[i, j] = adjoint[i, j] / determinant;

            return invertedMatrix;
        }

    }
}
