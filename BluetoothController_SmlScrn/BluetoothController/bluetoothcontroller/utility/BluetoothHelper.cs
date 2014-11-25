using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BluetoothController;
using InTheHand.Net.Bluetooth;
using InTheHand.Net.Sockets;
using InTheHand.Net;

namespace BluetoothController
{
    public class Bluetooth
    {
        public static BluetoothDeviceInfo[] GetBluetoothDeviceInfo()
        {
            BluetoothClient btclient = new BluetoothClient();
            return btclient.DiscoverDevices();
        }

        public static BluetoothClient ConnectToBluetoothDevice(BluetoothDeviceInfo deviceinfos)
        {
            //deviceinfos.SetServiceState(BluetoothService.SerialPort, true, true);
            var ep = new BluetoothEndPoint(deviceinfos.DeviceAddress, BluetoothService.SerialPort);
            var btc = new BluetoothClient();
            btc.Encrypt = false;
            btc.Authenticate = false;
            btc.Connect(ep);
            return btc;
        } 
    }

    public class BlueToothSensorData
    {
        private BluetoothClient btclient;
        public BlueToothSensorData(BluetoothClient _btclient)
        {
            btclient = _btclient;
        }
        
    }

}
