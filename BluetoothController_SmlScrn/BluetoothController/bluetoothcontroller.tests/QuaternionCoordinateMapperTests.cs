using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using BluetoothController;

using System.Windows.Media.Media3D;


namespace BluetoothController.Tests
{
    [TestClass]
    public class QuaternionCoordinateMapperTests
    {
        [TestClass]
        public class CorrectionTests
        {
            [TestMethod]
            public void SettingCorrectionsSetsCorrectionInverse()
            {
                //Arrange
                QuaternionCoordinateMapper Mapper = new QuaternionCoordinateMapper();
                Quaternion inverse;

                //Act
                Mapper.Correction = new Quaternion(-1, 3, 1, 2);
                inverse = Mapper.Correction;
                inverse.Invert();

                //Assert
                Assert.AreEqual(Mapper.CorrectionInverse, inverse);
            }
        }

        [TestClass]
        public class InitialQuaternionTests
        {
            [TestMethod]
            public void InitialQuaternionSets()
            {
                QuaternionCoordinateMapper Mapper = new QuaternionCoordinateMapper();
                Quaternion quatvalue = new Quaternion(1, 2, 3, 4);

                //Act
                Mapper.Initial = quatvalue;
                
                //Assert
                Assert.AreEqual(Mapper.Initial, quatvalue);
            }
        }

        [TestClass]
        public class MapTests
        {
            [TestMethod]
            public void AccelAtIdentityIsSame()
            {
                //Arrange
                QuaternionCoordinateMapper Mapper = new QuaternionCoordinateMapper();
                Mapper.Initial = Quaternion.Identity;
                Vector3D expectedvector = new Vector3D(0.012207, 0.008301, 1.003906), actualvector;

                //Act
                actualvector = Mapper.Map(Quaternion.Identity, expectedvector);

                //Assert
                Assert.AreEqual(expectedvector, actualvector);
                
            }

            [TestMethod]
            public void AccelAtInitialIsSame()
            {
                //Arrange
                QuaternionCoordinateMapper Mapper = new QuaternionCoordinateMapper();
                Quaternion initialquaternion = new Quaternion(0.003632D, -0.005046D, -0.004869D, 0.999969D);
                Mapper.Initial = initialquaternion;
                Vector3D expectedvector = new Vector3D(0.012207D, 0.008301D, 1.003906D), actualvector;

                //Act
                actualvector = Mapper.Map(initialquaternion, expectedvector);

                //Assert
                Assert.IsTrue(actualvector.IsSimilarByDelta(expectedvector, -5));
            }

            [TestMethod]
            public void AccelCorrection1()
            {
                //Arrange
                QuaternionCoordinateMapper Mapper = new QuaternionCoordinateMapper();
                Quaternion initialquaternion = new Quaternion(0.003632D, -0.005046D, -0.004869D, 0.999969D);
                Mapper.Initial = initialquaternion;
                Quaternion sensorquaternion = new Quaternion(-0.013280, -0.009689, 0.928746, 0.370351);

                Vector3D sensorvector = new Vector3D(-0.122070,-0.024414, 0.973145),  expectedvector = new Vector3D(0.084869, -0.066376, 0.975141), actualvector;

                //Act
                actualvector =  Mapper.Map(sensorquaternion, sensorvector);

                //Assert
                Assert.IsTrue(actualvector.IsSimilarByDelta(expectedvector, -4));
            }

            [TestMethod]
            public void AccelCorrection2()
            {
                //Arrange
                QuaternionCoordinateMapper Mapper = new QuaternionCoordinateMapper();
                Quaternion initialquaternion = new Quaternion(0.010259, -0.008919, -0.001392,  0.999907);
                Mapper.Initial = initialquaternion;
                Quaternion sensorquaternion = new Quaternion(0.051725, -0.658204, 0.036181, 0.750189);

                Vector3D sensorvector = new Vector3D(1.003418, 0.025879, 0.102051), expectedvector = new Vector3D(0.045759, 0.019828, 1.007693), actualvector;

                //Act
                actualvector = Mapper.Map(sensorquaternion, sensorvector);

                //Assert
                Assert.IsTrue(actualvector.IsSimilarByDelta(expectedvector, -4));
            }
            
        }



        [TestClass]
        public class QuaternionTests
        {
            [TestMethod]
            public void InvertIsCorrect()
            {
                //Arrange
                Quaternion quattoinvert, knowninvert;

                //Act
                quattoinvert = new Quaternion(-1, 3, 1, 2);
                quattoinvert.Invert();
                knowninvert = new Quaternion (1.0/15.0, -1.0/5.0,-1.0/15.0, 2.0/15.0);

                //Assert
                Assert.AreEqual(quattoinvert, knowninvert);

                
            }

            [TestMethod]
            public void QuatTimeInverseIsIdentity()
            {
                //Arrange
                Quaternion test = new Quaternion(0.003632, -0.005046, -0.004869, 0.999969);
                Quaternion testinverse = test;
                testinverse.Invert();

                Quaternion shouldbeidentity = test * testinverse;

                Assert.IsTrue(shouldbeidentity.IsSimilarByDelta(Quaternion.Identity, -15));
            }

            [TestMethod]
            public void QuatMultByAnInverse()
            {

                Quaternion initialquaternion = new Quaternion(0.003632D, -0.005046D, -0.004869D, 0.999969D);
                Quaternion sensorquaternion = new Quaternion(-0.013280, -0.009689, 0.928746, 0.370351);
                Quaternion initialinversequaternion = initialquaternion;
                initialinversequaternion.Invert();
                Quaternion expected = new Quaternion(-0.009891, -0.004511, 0.930623, 0.365818);

                Quaternion actual = initialinversequaternion  * sensorquaternion;

                Assert.IsTrue(actual.IsSimilarByDelta(expected, -4));

                
            }

        }
    }
}
