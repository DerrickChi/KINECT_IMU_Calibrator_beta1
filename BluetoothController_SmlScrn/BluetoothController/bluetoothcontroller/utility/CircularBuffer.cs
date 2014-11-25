using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BluetoothController
{
    public class CircularBuffer<T> 
        //: IEnumerable<T>
    {
        T[] Buffer;
        int Head;
        int Tail;
        int Length;
        int BufferSize;
        object _lock = new object();

        public CircularBuffer(int buffersize)
        {
            BufferSize = buffersize;
            Buffer = new T[BufferSize];
            Head = BufferSize - 1;
            Tail = 0;
        }
        public bool IsEmpty
        {
            get { return Length == 0; }
        }

        public bool IsFull
        {
            get { return Length == BufferSize; }
        }

        public int size
        {
            get { return Length; }
        }

        public int Capacity
        {
            get
            {
                return Buffer.Length;
            }
        }

        public T Dequeue()
        {
            lock (_lock)
            {
                if (IsEmpty)
                {
                    return default(T) ;
                }
                T dequeued = Buffer[Tail];
                Tail = NextPosition(Tail);
                Length--;
                return dequeued;
            }
        }
        private int NextPosition(int position)
        {
            return (position + 1) % BufferSize;
        }

        public CircularBuffer<T> Enqueue(T add)
        {
            lock(_lock)
            {
                Head = NextPosition(Head);
                Buffer[Head] = add;
                if (IsFull)
                    Tail = NextPosition(Tail);
                else
                    Length++;
                return this;
            }
        }

        public T this[int i]
        {
            get 
            {
                if (i >= BufferSize)
                    throw new Exception("Index for buffer is greater than buffer size");
                if (i < 0)
                    throw new Exception("Can not access a negative index");
                int pos = (Tail + i) % BufferSize; 
                return Buffer[pos]; 
            }
            set 
            {
                if (i >= BufferSize)
                    throw new Exception("Index for buffer is greater than buffer size");
                if (i < 0)
                    throw new Exception("Can not access a negative index");
                int pos = (Tail + i) % BufferSize; 
                Buffer[pos] = value; 
            }
        }
    }
}
