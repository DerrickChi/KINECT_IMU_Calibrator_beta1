using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BluetoothController
{
    public interface IRegulatedEvent<TEventArgs>
    {
        event EventHandler<TEventArgs> NewRegulatedEvent;
        void OnNewTEventArgs(object sender, TEventArgs e);
    }
}