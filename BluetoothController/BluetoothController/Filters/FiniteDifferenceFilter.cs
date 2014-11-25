using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BluetoothController
{
    public class FiniteDifferenceFilter :IFilter
    {
        public enum FiniteDifferenceType
        {
            Central,
            Forward,
            BackWard
        }

        public int Accuracy {get; protected set;}
        public FiniteDifferenceType FiniteDifference {get; protected set;}
        private double[] BufferDouble;
        private long[] TicksBuffer;
        private double AverageTimeSpan;
        private int Count = 0;
        private bool IsReady = false;

        /// <summary>
        /// Create an instance of FiniteDiffernceFilter
        /// </summary>
        ///<remarks>
        ///Will Throw Invalide
        ///</remarks>
        /// <param name="accuracy">Accuracy of derivative from 2 - 8 for centeral or 2 - 4 for forward or backward</param>
        /// 
        public FiniteDifferenceFilter(int accuracy,  FiniteDifferenceType fdt)
        {
            if ((fdt == FiniteDifferenceType.Central) && 
                (accuracy % 2 != 0 ||
                accuracy > 2 ||
                accuracy < 8))
                throw new ArgumentException("Accuracy was invalid", accuracy.GetType().Name);
            if ((fdt == FiniteDifferenceType.BackWard) &&
                (accuracy < 2 ||
                accuracy > 4))
                throw new ArgumentException("Accuracy was invalid", accuracy.GetType().Name);


            //if(fdt == FiniteDifferenceType.Central)
            //    accuracy /=2;
            this.Accuracy = accuracy;
            this.FiniteDifference = fdt;
            this.BufferDouble = new double[accuracy + 1];
            this.TicksBuffer = new long[accuracy + 1];
            for (int i = 0; i < this.BufferDouble.Length; i++)
            {
                this.BufferDouble[i] = double.NaN;
                this.TicksBuffer[i] = long.MinValue;
            }
        } 

        private readonly double[][][] CentralDifferenceCoefficents =
        {   new double[][]
            {
                new double[]{-1.0/2.0, 0.0, 1.0/2.0},
                new double[]{1.0/12.0, -2.0/3.0, 0.0, 2.0/3.0, -1.0/12.0},
                new double[]{-1.0/60.0,	3.0/20.0, -3.0/4.0,	0.0, 3.0/4.0, -3.0/20.0, 1.0/60.0},
                new double[]{1.0/280.0,	-4.0/105.0,	1.0/5.0, -4.0/5.0, 0.0, 4.0/5.0, -1.0/5.0, 4.0/105.0, -1.0/280.0}

            },
            new double[][]
            {
                new double[]{1.0, -2.0, 1.0},
                new double[]{-1.0/12.0, 4.0/3.0, -5.0/2.0, 4.0/3.0, -1.0/12.0},
                new double[]{1.0/90.0, -3.0/20.0, 3.0/2.0, -49.0/18.0, 3.0/2.0, -3.0/20.0,	1.0/90.0},
                new double[]{-1.0/560.0, 8.0/315.0, -1.0/5.0, 8.0/5.0, -205.0/72.0,	8.0/5.0, -1.0/5.0, 8.0/315.0, -1.0/560.0}
            }
        };

        private readonly double[][][] BackWardDifferenceCoefficents =
        {   new double[][]
            {
                new double[]{1.0, -1.0},
                new double[]{3.0/2.0,-2.0,1.0/2.0},
                new double[]{11.0/6.0,-3.0, 3.0/2.0, -1.0/3.0},
                new double[]{25.0/12.0, -4.0, 3.0, -4.0/3.0, 1.0/4.0}

            },
            new double[][]
            {
                new double[]{},
                new double[]{1.0, -2.0, 1.0},
                new double[]{2.0, -5.0, 4.0, -1.0},
                new double[]{35.0/12.0, -26.0/3.0, 19.0/2.0, -14.0/3.0, 11.0/12.0},
                new double[]{15.0/4.0, -77.0/6.0, 107.0/6.0, -13.0, 61.0/12.0, -5.0/6.0}
            }
        };

    
        #region IFilter Members

        public IFilter UpdateVal(double newVal, long newTime)
        {
            for (int i = 0; i < this.BufferDouble.Length - 1; i++)
            {
                this.BufferDouble[i] = this.BufferDouble[i + 1];
                this.TicksBuffer[i] = this.TicksBuffer[i + 1];
            }
            this.BufferDouble[BufferDouble.Length - 1] = newVal;
            this.TicksBuffer[TicksBuffer.Length - 1] = newTime;
            if (this.IsReady)
            {
                 this.AverageTimeSpan = this.GetAverageTimeSpan(this.TicksBuffer);
            }
            else
            {
                Count++;
                if (Count >= this.BufferDouble.Length - 1)
                    this.IsReady = true;
            }
            return this;
        }

        private double GetAverageTimeSpan(long[] p)
        {
            long ticksdif = 0;
            for (int i = 0; i < p.Length - 2; i++)
                ticksdif += p[i + 1] - p[i];
            return (ticksdif / (p.Length - 2)) * (1.0 / TimeSpan.TicksPerSecond); 

        }

        public Tuple<double,long> GetNDerivative(NDerivative nd)
        {
            if (!this.IsReady)
                return null;
            if (nd == NDerivative.smooth)
                return new Tuple<double, long>(this.BufferDouble[this.BufferDouble.Length - 1], this.TicksBuffer[this.TicksBuffer.Length - 1]);
            double sum = 0;
            switch (this.FiniteDifference)
            {
                case FiniteDifferenceType.BackWard:
                    if (nd == NDerivative.first)
                    {
                        for (int i = 0; i < this.BufferDouble.Length; i++)
                            sum += this.BufferDouble[this.BufferDouble.Length - 1 - i] * this.BackWardDifferenceCoefficents[0][Accuracy - 1][i];
                        return new Tuple<double, long>(sum / AverageTimeSpan, this.TicksBuffer[this.TicksBuffer.Length - 1]);
                    }
                    else if (nd == NDerivative.second)
                    {
                            for (int i = 0; i < this.BufferDouble.Length; i++)
                                sum += this.BufferDouble[this.BufferDouble.Length - 1 - i] * this.BackWardDifferenceCoefficents[1][Accuracy - 1][i];
                            return new Tuple<double, long>(sum / Math.Pow(AverageTimeSpan, 2.0), this.TicksBuffer[this.TicksBuffer.Length - 1]);
                    }
                    else
                        throw new ArgumentException(nd.GetType().Name);
                case FiniteDifferenceType.Central:
                    if (nd == NDerivative.first)
                    {
                        for (int i = 0; i < this.BufferDouble.Length; i++)
                            sum += this.BufferDouble[i] * this.CentralDifferenceCoefficents[1][(Accuracy - 1) / 2][i];
                        return new Tuple<double, long>(sum / AverageTimeSpan, this.TicksBuffer[this.TicksBuffer.Length - 1]);
                    }
                    else if (nd == NDerivative.second)
                    {
                        for (int i = 0; i < this.BufferDouble.Length; i++)
                            sum += this.BufferDouble[i] * this.CentralDifferenceCoefficents[1][(Accuracy - 1) / 2][i];
                        return new Tuple<double, long>(sum / Math.Pow(AverageTimeSpan, 2.0), this.TicksBuffer[this.TicksBuffer.Length - 1]);
                    }
                    else
                        throw new ArgumentException(nd.GetType().Name);

            }
            throw new ArgumentException(this.FiniteDifference.GetType().Name);
        }

        #endregion
        }
}
