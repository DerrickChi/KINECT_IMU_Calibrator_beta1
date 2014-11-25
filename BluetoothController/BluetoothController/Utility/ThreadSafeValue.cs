using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BluetoothController
{
    public class ThreadSafeValue<T>
    {
        private object _Lock = new object();
        protected T _Value;
        public T Value
        {
            get
            {
                lock (this._Lock)
                {
                    return this._Value;
                }
            }
            set
            {
                lock (this._Lock)
                {
                    this._Value = value;
                }
            }
        }


    }
}
