using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;
using InTheHand.Net.Sockets;
using InTheHand.Net.Bluetooth;
using InTheHand.Net;


namespace BluetoothController
{
    public class MappedInheretedInertialSensorProducer : RawInertialSensorProducer
    {
        public IMapper Mapper { get; protected set; }
        public MappedInheretedInertialSensorProducer(BluetoothAddress btaddress, IMapper mapper) : base(btaddress)
        {
            if (btaddress == null)
                throw new ArgumentException("Cannot be null.", btaddress.GetType().Name);
            if (mapper == null)
                throw new ArgumentException("Cannot be null.", mapper.GetType().Name);
            this.Mapper = mapper;
            base.DeviceAddress = "MappedInher_" + base.DeviceAddress;
            base.DeviceName = "MappedInher_" + base.DeviceName;
        }

        protected override void FireEvents()
        {
            double[] tdouble = new double[3];
            Vector3D tvector3d;

            tdouble = base.Data.NormalizedAccelerations;
            tvector3d = new Vector3D(tdouble[0], tdouble[1],tdouble[2]);

            tvector3d = this.Mapper.Map(base.Data.QuaternionAsQMD3, tvector3d);

            tdouble[0] = tvector3d.X;
            tdouble[1] = tvector3d.Y;
            tdouble[2] = tvector3d.Z;

            base.Data.NormalizedAccelerations = tdouble;

            base.FireEvents();
        }
         

    }
}
