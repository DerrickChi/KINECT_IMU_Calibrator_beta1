using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BluetoothController;
using BluetoothController.Kinect;
using Microsoft.Kinect;


namespace BluetoothController
{
    class Merge
    {
        private IDataProducer<InertialSensorData> ActualSensor;
        private IDataProducer<VirtualSensorData> IdealSensor;
        public event EventHandler<SensorCalibrator> CalibrationComplete;
        CircularBuffer<AccelerationTime> virtualAcc; //kinect 
        CircularBuffer<AccelerationTime> actualAcc; //sensor
        CircularBuffer<SensorCalibrator> calibrators;
        SensorCalibrator finalCalibrator;
        private const int KinectFPS = 30;
        private const int InertialMPS = 200;
        private const int CalibrationLookBackTimeInSec = 10;



        public Merge(IDataProducer<VirtualSensorData> idealsensor, IDataProducer<InertialSensorData> actualsensor)
        {
            this.ActualSensor = actualsensor;
            IdealSensor = idealsensor;

            ActualSensor.NewTData += MyFunctionToDealWithGettingData;
            IdealSensor.NewTData += OnIdealSensorNewTData;
            virtualAcc = new CircularBuffer<AccelerationTime>(KinectFPS * CalibrationLookBackTimeInSec); //kinect
            actualAcc = new CircularBuffer<AccelerationTime>(InertialMPS * CalibrationLookBackTimeInSec); //sensor
            calibrators = new CircularBuffer<SensorCalibrator>(KinectFPS * CalibrationLookBackTimeInSec - 4); //calibrators
            finalCalibrator = new SensorCalibrator();

        }

        ~Merge()
        {
            ActualSensor.NewTData -= this.MyFunctionToDealWithGettingData;
            IdealSensor.NewTData -= OnIdealSensorNewTData;
        }

        private void OnIdealSensorNewTData(object sender, VirtualSensorData e)
        {
            //derivation and store into buffer
            AccelerationTime t = new AccelerationTime();
            t.setVal(e.acceleration, e.NowInTicks);
            actualAcc.Enqueue(t);
        }
        System.Threading.Thread WorkerThread;
        System.Threading.ThreadStart WorkerFunction;
        public Merge Calibrate()
        {
            if (WorkerThread.IsAlive)
                this.WorkerThread.Abort();
            WorkerFunction = new System.Threading.ThreadStart(() =>
            {
                this.mathCalibrate();
                if(this.CalibrationComplete != null)
                    this.CalibrationComplete(this, this.finalCalibrator);
                ActualSensor.NewTData += this.MyFunctionToDealWithGettingData;
                IdealSensor.NewTData += this.OnIdealSensorNewTData;
            });
            this.WorkerThread = new System.Threading.Thread(WorkerFunction);
            ActualSensor.NewTData -= this.MyFunctionToDealWithGettingData;
            IdealSensor.NewTData -= this.OnIdealSensorNewTData;
            this.WorkerThread.Start();
            return this;

        }

        private void MyFunctionToDealWithGettingData(object sender, InertialSensorData e)
        {
            const double angularConst = Math.PI/(16.4*180);
            //getting acceleration data from sensors
            double xAcc = (double)e.Accelarometers[0] / 2080; 
            double yAcc = (double)e.Accelarometers[1] / 2080;
            double zAcc = (double)e.Accelarometers[2] / 2080;

            double xRot = (double)e.gyropscopes[0]*angularConst;
            double yRot = (double)e.gyropscopes[1]*angularConst;
            double zRot = (double)e.gyropscopes[2]*angularConst;

            double[] tempAngularActualAcc = new double[] { xRot, yRot, zRot }; 
            double[] tempActualAcc = new double[] { xAcc, yAcc, zAcc };
            AccelerationTime tempActualAccTime = new AccelerationTime();
            tempActualAccTime.setVal(tempActualAcc, tempAngularActualAcc, e.NowInTicks);

            actualAcc.Enqueue(tempActualAccTime);
        }
        /// <summary>
        /// parameter pos indcates point between joint1 (0) and joint2 (100)
        /// </summary>
        /// <param name="j1"></param>
        /// <param name="j2"></param>
        /// <param name="pos"></param>
       

        public void mathCalibrate()
        {
            int actualAccIndex = searchClosestTime(virtualAcc[2].Time); //Searches for the closest time in the actualAcc buffer to begin the calibration
            double[] avgActualAcc = new double[] { 0, 0, 0 };
            double[] idealAcc = new double[] { 0, 0, 0 };

            //NEED TO CONVERT FROM VIRTUAL TO IDEAL 

            for (int i = 2; i < virtualAcc.size - 2; i++)
            {
                avgActualAcc = averageActualAcc(actualAccIndex); //average 6 sensor accelertion vectors
                calibrators[i].setData(avgActualAcc, virtualAcc[i].angularAcceleration, virtualAcc[i].linearAcceleration); //(CHANGE FROM VIRTUAL TO IDEAL)
                calibrators[i].generateCorrections();
                actualAccIndex = actualAccIndex + 6;
            }

            finalCalibrator.correctionConst = MathFunctions.correctionConstAvg(calibrators);
            finalCalibrator.rollCorrection = MathFunctions.matrixAvg(calibrators, MathFunctions.MatrixType.Roll);
            finalCalibrator.yawCorrection = MathFunctions.matrixAvg(calibrators, MathFunctions.MatrixType.Yaw);
        }

        //Searches for closest time in actual acceleration buffer
        public int searchClosestTime(long timeInTicks)
        {
            int index = 0;
            for (int i = 0; i < actualAcc.size; i++)
            {
                if (actualAcc[i].Time >= timeInTicks)
                { break; }
                index++;
            }

            return index;   
        }

        public double[] averageActualAcc(int index)
        {
            double totalX = 0, totalY = 0, totalZ = 0;
            for (int i = 0; i < 6; i++)
            {
                totalX += actualAcc[i - 2].linearAcceleration[0];
                totalY += actualAcc[i - 2].linearAcceleration[1];
                totalZ += actualAcc[i - 2].linearAcceleration[2];
            }

            return new double[] { totalX / 6, totalY / 6, totalZ / 6 };
        }

    }
}
