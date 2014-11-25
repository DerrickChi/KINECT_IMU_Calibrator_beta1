using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace BluetoothController
{
    public class QuaternionCoordinateMapper : IMapper
    {
        object _Lock = new object();
        //private Quaternion DesiredFrame;
        //public Quaternion DesiredFrameQuaternion 
        //{
        //    get
        //    {
        //        lock (_Lock)
        //        {
        //            return this.DesiredFrame;
        //        }
        //    }
        //    set
        //    {
        //        lock (_Lock)
        //        {
        //            this.DesiredFrame = value;
        //        }
        //    }
        //}
        private Quaternion _Initial;
        public Quaternion Initial
        {
            get
            {
                lock(_Lock)
                {
                    return this._Initial;
                }
            }
            set
            {
                lock(_Lock)
                {
                    this._Initial = value;
                }
            }
        }
        public Quaternion InitialInverse 
        {
            get
            {
                lock (_Lock)
                {
                    Quaternion inverse = this.Initial;
                    inverse.Invert();
                    return inverse;
                }
            }
        }
        private Quaternion _Correction;
        public Quaternion CorrectionInverse
        {
            get
            {
                lock (_Lock)
                {
                    Quaternion correctioninverse = this._Correction;
                    correctioninverse.Invert();
                    return correctioninverse;
                }
            }
        }
        public Quaternion Correction
        {
            get
            {
                lock (_Lock)
                {
                    return this._Correction;
                }
            }
            set
            {
                lock (_Lock)
                {
                    this._Correction = value;
                }
            }
        }

        private Vector3D _CoefficientVector;
        public Vector3D CoefficientVector
        {
            get
            {
                lock (this._Lock)
                {
                    return this._CoefficientVector;
                }
            }
            set
            {
                lock(this._Lock)
                {
                    this._CoefficientVector = value;
                }
            }
        }

        public QuaternionCoordinateMapper() 
        {
            //this.DesiredFrameQuaternion = Quaternion.Identity;
            this.Correction = Quaternion.Identity;
            this.Initial = Quaternion.Identity;
            this.CoefficientVector = new Vector3D(1.0, 1.0, 1.0);
        }

        public QuaternionCoordinateMapper(Quaternion rotationquaternion)
        {
            if(rotationquaternion == null)
                throw new ArgumentException("Cannot be null", rotationquaternion.GetType().Name);
            this._Correction = rotationquaternion;
        }

        public Vector3D Map (Quaternion sensor, Vector3D vector)
        {
            if (this._Correction == null)
                throw new InvalidOperationException(this.Correction.GetType().Name + " is not set.");
            if (this.Initial == null)
                throw new InvalidOperationException(this.Initial.GetType().Name + " is not set.");
            //ADD protection

            Quaternion VectorAsQuaternion = new Quaternion(this.CoefficientVector.X * vector.X, this.CoefficientVector.Y * vector.Y, this.CoefficientVector.Z * vector.Z, 0.0);
            lock (_Lock)
            {

                this.Correction = this.InitialInverse * sensor; 

                Quaternion RotatedVectorAsQuaternion = this.Correction * VectorAsQuaternion * this.CorrectionInverse;

                return new Vector3D(RotatedVectorAsQuaternion.X,RotatedVectorAsQuaternion.Y,RotatedVectorAsQuaternion.Z);
            }
        }
    }
}
