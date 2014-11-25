using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BluetoothController
{
    public class ConcurrentCappedQueue<T>
    {
        private const int defaultsize = 3;
        private int CappedSize;
        private ConcurrentQueue<T> ConQueue;
        public ConcurrentCappedQueue() : this(defaultsize)
        { }
        public ConcurrentCappedQueue(int capsize)
        {
            this.CappedSize = capsize;
            this.ConQueue = new ConcurrentQueue<T>();
            
        }
        public bool Add(T item)
        {
            if (this.ConQueue.Count < this.CappedSize)
            {
                this.ConQueue.Enqueue(item);
                return true;
            }
            else
                return false;
        }
        public bool TryDequeue(out T item)
        {
            //bool gooddequeue = false;
            return this.ConQueue.TryDequeue(out item);
        }
        public bool IsEmpty
        {
            get
            {
                return this.ConQueue.IsEmpty;
            }
        }
    }
}
