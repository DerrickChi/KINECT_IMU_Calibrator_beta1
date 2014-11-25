
using System;

namespace Altaxo.Calc.LinearAlgebra
{
    /// <summary>
    /// VectorMath provides common static functions concerning vectors.
    /// </summary>
    public static class VectorMath
    {

        /// <summary>
        /// Serves as Wrapper for an double array to plug-in where a IROVector is neccessary.
        /// </summary>
        private class RODoubleArrayWrapper : IROVector
        {
            protected double[] _x;
            private int _length;

            /// <summary>
            /// Constructor, takes a double array for wrapping.
            /// </summary>
            /// <param name="x"></param>
            public RODoubleArrayWrapper(double[] x)
            {
                _x = x;
                _length = _x.Length;
            }

            /// <summary>
            /// Constructor, takes a double array for wrapping.
            /// </summary>
            /// <param name="x"></param>
            /// <param name="usedlength">The length used for the vector.</param>
            public RODoubleArrayWrapper(double[] x, int usedlength)
            {
                if (usedlength > x.Length)
                    throw new ArgumentException("Length provided in argument usedlength is greater than length of array");

                _x = x;
                _length = usedlength;
            }

            /// <summary>Gets the value at index i with 0 &lt;= i &lt;=Length-1.</summary>
            /// <value>The element at index i.</value>
            public double this[int i] { get { return _x[i]; } }

            /// <summary>The number of elements of this vector.</summary>
            public int Length { get { return _length; } }  // change this later to length property
        }

        private class RWDoubleArrayWrapper : RODoubleArrayWrapper, IVector
        {
            public RWDoubleArrayWrapper(double[] x)
                : base(x)
            {
            }

            public RWDoubleArrayWrapper(double[] x, int usedlength)
                : base(x, usedlength)
            {
            }

            #region IVector Members

            public new double this[int i]
            {
                get { return _x[i]; }
                set { _x[i] = value; }
            }

            #endregion IVector Members
        }

        ///// <summary>
        ///// Serves as Wrapper for an double array to plug-in where a IROVector is neccessary.
        ///// </summary>
        private class RODoubleArraySectionWrapper : IROVector
        {
            protected double[] _x;
            protected int _start;
            protected int _length;

            /// <summary>
            /// Constructor, takes a double array for wrapping.
            /// </summary>
            /// <param name="x"></param>
            public RODoubleArraySectionWrapper(double[] x)
            {
                _x = x;
                _start = 0;
                _length = _x.Length;
            }

            /// <summary>
            /// Constructor, takes a double array for wrapping.
            /// </summary>
            /// <param name="x"></param>
            /// <param name="start">Index of the element in <paramref name="x"/> used as the first element of the vector.</param>
            /// <param name="usedlength">The length used for the vector.</param>
            public RODoubleArraySectionWrapper(double[] x, int start, int usedlength)
            {
                if (start < 0)
                    throw new ArgumentException("start is negative");
                if (usedlength < 0)
                    throw new ArgumentException("usedlength is negative");

                if ((start + usedlength) > x.Length)
                    throw new ArgumentException("Length provided in argument usedlength is greater than length of array");

                _x = x;
                _start = start;
                _length = usedlength;
            }

            /// <summary>Gets the value at index i with 0 &lt;= i &lt;=Length-1.</summary>
            /// <value>The element at index i.</value>
            public double this[int i] { get { return _x[i + _start]; } }

            /// <summary>The number of elements of this vector.</summary>
            public int Length { get { return _length; } }  // change this later to length property
        }

        private class RWDoubleArraySectionWrapper : RODoubleArraySectionWrapper, IVector
        {
            public RWDoubleArraySectionWrapper(double[] x)
                : base(x)
            {
            }

            public RWDoubleArraySectionWrapper(double[] x, int start, int usedlength)
                : base(x, start, usedlength)
            {
            }

            #region IVector Members

            public new double this[int i]
            {
                get { return _x[i + _start]; }
                set { _x[i + _start] = value; }
            }

            #endregion IVector Members
        }

        ///// <summary>
        ///// Serves as wrapper for an IROVector to get only a section of the original wrapper.
        ///// </summary>
        private class ROVectorSectionWrapper : IROVector
        {
            protected IROVector _x;
            private int _start;
            private int _length;

            /// <summary>
            /// Constructor, takes a double array for wrapping.
            /// </summary>
            /// <param name="x"></param>
            /// <param name="start">Start index of the section to wrap.</param>
            /// <param name="len">Length of the section to wrap.</param>
            public ROVectorSectionWrapper(IROVector x, int start, int len)
            {
                if (start >= x.Length)
                    throw new ArgumentException("Start of the section is beyond length of the vector");
                if (start + len > x.Length)
                    throw new ArgumentException("End of the section is beyond length of the vector");

                _x = x;
                _start = start;
                _length = len;
            }

            /// <summary>Gets the value at index i with 0 &lt;= i &lt;=Length-1.</summary>
            /// <value>The element at index i.</value>
            public double this[int i] { get { return _x[i + _start]; } }

            /// <summary>The number of elements of this vector.</summary>
            public int Length { get { return _length; } }  // change this later to length property
        }

        /// <summary>
        /// Serves as wrapper for an IVector to get only a section of the original wrapper.
        /// </summary>
        private class RWVectorSectionWrapper : IVector
        {
            protected IVector _x;
            private int _start;
            private int _length;

            /// <summary>
            /// Constructor, takes a double array for wrapping.
            /// </summary>
            /// <param name="x"></param>
            /// <param name="start">Start index of the section to wrap.</param>
            /// <param name="len">Length of the section to wrap.</param>
            public RWVectorSectionWrapper(IVector x, int start, int len)
            {
                if (start >= x.Length)
                    throw new ArgumentException("Start of the section is beyond length of the vector");
                if (start + len >= x.Length)
                    throw new ArgumentException("End of the section is beyond length of the vector");

                _x = x;
                _start = start;
                _length = len;
            }

            /// <summary>Gets the value at index i with 0 &lt;= i &lt;=Length-1.</summary>
            /// <value>The element at index i.</value>
            public double this[int i]
            {
                get { return _x[i + _start]; }
                set { _x[i + _start] = value; }
            }

            /// <summary>The number of elements of this vector.</summary>
            public int Length { get { return _length; } }  // change this later to length property
        }

        /// <summary>
        /// Serves as Wrapper for an short array to plug-in where a IROVector is neccessary.
        /// </summary>
        private class ROIntArraySectionWrapper : IROVector
        {
            protected int[] _x;
            protected int _start;
            protected int _length;

            /// <summary>
            /// Constructor, takes a double array for wrapping.
            /// </summary>
            /// <param name="x"></param>
            public ROIntArraySectionWrapper(int[] x)
            {
                _x = x;
                _start = 0;
                _length = _x.Length;
            }

            /// <summary>
            /// Constructor, takes a double array for wrapping.
            /// </summary>
            /// <param name="x"></param>
            /// <param name="start">Index of the element in <paramref name="x"/> used as the first element of the vector.</param>
            /// <param name="usedlength">The length used for the vector.</param>
            public ROIntArraySectionWrapper(int[] x, int start, int usedlength)
            {
                if (start < 0)
                    throw new ArgumentException("start is negative");
                if (usedlength < 0)
                    throw new ArgumentException("usedlength is negative");

                if ((start + usedlength) > x.Length)
                    throw new ArgumentException("Length provided in argument usedlength is greater than length of array");

                _x = x;
                _start = start;
                _length = usedlength;
            }

            /// <summary>Gets the value at index i with 0 &lt;= i &lt;=Length-1.</summary>
            /// <value>The element at index i.</value>
            public double this[int i] { get { return _x[i + _start]; } }

            /// <summary>The number of elements of this vector.</summary>
            public int Length { get { return _length; } }  // change this later to length property
        }

        /// <summary>
        /// Serves as Wrapper for an short array to plug-in where a IROVector is neccessary.
        /// </summary>
        private class ROShortArraySectionWrapper : IROVector
        {
            protected short[] _x;
            protected int _start;
            protected int _length;

            /// <summary>
            /// Constructor, takes a double array for wrapping.
            /// </summary>
            /// <param name="x"></param>
            public ROShortArraySectionWrapper(short[] x)
            {
                _x = x;
                _start = 0;
                _length = _x.Length;
            }

            /// <summary>
            /// Constructor, takes a double array for wrapping.
            /// </summary>
            /// <param name="x"></param>
            /// <param name="start">Index of the element in <paramref name="x"/> used as the first element of the vector.</param>
            /// <param name="usedlength">The length used for the vector.</param>
            public ROShortArraySectionWrapper(short[] x, int start, int usedlength)
            {
                if (start < 0)
                    throw new ArgumentException("start is negative");
                if (usedlength < 0)
                    throw new ArgumentException("usedlength is negative");

                if ((start + usedlength) > x.Length)
                    throw new ArgumentException("Length provided in argument usedlength is greater than length of array");

                _x = x;
                _start = start;
                _length = usedlength;
            }

            /// <summary>Gets the value at index i with 0 &lt;= i &lt;=Length-1.</summary>
            /// <value>The element at index i.</value>
            public double this[int i] { get { return _x[i + _start]; } }

            /// <summary>The number of elements of this vector.</summary>
            public int Length { get { return _length; } }  // change this later to length property
        }


        /// <summary>
        /// Wraps a double[] array to get a IVector.
        /// </summary>
        /// <param name="x">The array to wrap.</param>
        /// <returns>A wrapper objects with the <see cref="IVector" /> interface that wraps the provided array.</returns>
        public static IVector ToVector(double[] x)
        {
            return new RWDoubleArrayWrapper(x);
        }

        /// <summary>
        /// Wraps a double[] array to get a IVector.
        /// </summary>
        /// <param name="x">The array to wrap.</param>
        /// <param name="usedlength">Used length of the array to get the wrapped vector (i.e. the vector wraps around <paramref name="x"/>[0..usedLength-1]).</param>
        /// <returns>A wrapper objects with the <see cref="IVector" /> interface that wraps the provided array.</returns>
        public static IVector ToVector(double[] x, int usedlength)
        {
            return new RWDoubleArrayWrapper(x, usedlength);
        }

        /// <summary>
        /// Wraps part of a double[] array to get a IVector.
        /// </summary>
        /// <param name="x">The array to wrap.</param>
        /// <param name="start">Index of first element of <paramref name="x"/> to use.</param>
        /// <param name="count">Number of elements of <paramref name="x"/> to use.</param>
        /// <returns>A wrapper objects with the <see cref="IVector" /> interface that wraps part of the provided array.</returns>
        public static IVector ToVector(double[] x, int start, int count)
        {
            if (0 == start)
                return new RWDoubleArrayWrapper(x, count);
            else
                return new RWDoubleArraySectionWrapper(x, start, count);
        }

        //#endregion From/To double[]

        #region from/to IROVector

        /// <summary>
        /// Wraps a section of a original vector <c>x</c> into a new vector.
        /// </summary>
        /// <param name="x">Original vector.</param>
        /// <param name="start">Index of the start of the section to wrap.</param>
        /// <param name="len">Length (=number of elements) of the section to wrap.</param>
        /// <returns>A IROVector that contains the section from <c>start</c> to <c>start+len-1</c> of the original vector.</returns>
        public static IROVector ToROVector(this IROVector x, int start, int len)
        {
            return new ROVectorSectionWrapper(x, start, len);
        }

        #endregion from/to IROVector


        /// <summary>
        /// Wraps a section of a original vector <c>x</c> into a new vector.
        /// </summary>
        /// <param name="x">Original vector.</param>
        /// <param name="start">Index of the start of the section to wrap.</param>
        /// <param name="len">Length (=number of elements) of the section to wrap.</param>
        /// <returns>A IVector that contains the section from <c>start</c> to <c>start+len-1</c> of the original vector.</returns>
        public static IVector ToVector(IVector x, int start, int len)
        {
            return new RWVectorSectionWrapper(x, start, len);
        }


        ///// <summary>
        ///// Returns the sum of the elements in xarray.
        ///// </summary>
        ///// <param name="xarray">The array.</param>
        ///// <returns>The sum of all elements in xarray.</returns>
        public static double Sum(this double[] xarray)
        {
            double sum = 0;
            for (int i = 0; i < xarray.Length; i++)
                sum += xarray[i];

            return sum;
        }

        ///// <summary>
        ///// Returns the sum of the elements in the vector.
        ///// </summary>
        ///// <param name="xarray">The vector.</param>
        ///// <returns>The sum of all elements in xarray.</returns>
        public static double Sum(this IROVector xarray)
        {
            double sum = 0;
            for (int i = 0; i < xarray.Length; i++)
                sum += xarray[i];

            return sum;
        }
    }
}