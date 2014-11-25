using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BluetoothController
{
    [TestClass]
    class MathTests
    {
        [TestMethod]
        public void MatrixEquals()
        {
            double[,] testMatrix = new double[,] { { 1, 0, 0 }, { 0, 1, 0 }, { 0, 0, 1 } };
            double[,] testMatrix2 = new double[,] { { 1, 0, 0 }, { 0, 1, 0 }, { 0, 0, 1 } };
            Assert.AreEqual(testMatrix, testMatrix2);
        }   

    }
}
