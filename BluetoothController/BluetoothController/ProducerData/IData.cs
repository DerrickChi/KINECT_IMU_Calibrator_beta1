using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BluetoothController
{
    /// <summary>
    /// This interface is used to allow the data to be written to the screen, file, preview ect.
    /// </summary>
    public interface IData
    {
        string ToWindowsFormString ();
        string ToFileHeaderString();
        string ToFileString();
        string ToPreviewString();
        long NowInTicks { get;  set; }
    }
}
