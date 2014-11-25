using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BluetoothController.Kinect
{
    /// <summary>
    /// This describes the possible location for the acceromenter
    /// </summary>
    public enum SensorLocation
    {
        [Description("Not Applicable")]
        NotApplicable,
        [Description("Not Set")]
        NotSet,
        [Description("Forearm Right")]
        ForearmRight,
        [Description("Forearm Left")]
        ForearmLeft,
        [Description("Upper Arm Right")]
        UpperArmRight,
        [Description("Upper Arm Left")]
        UpperArmLeft,
        [Description("Ankel Right")]
        AnkleRight,
        [Description("Ankel Left")]
        AnkelLeft
    }
}
