using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace BluetoothController
{
    public interface IMapper
    {
        Quaternion Correction { get; set; }
        Vector3D Map (Quaternion sensor, Vector3D vector);
    }
}
