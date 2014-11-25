#region Copyright
/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2011 Dr. Dirk Lellinger
//
//    This program is free software; you can redistribute it and/or modify
//    it under the terms of the GNU General Public License as published by
//    the Free Software Foundation; either version 2 of the License, or
//    (at your option) any later version.
//
//    This program is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//    GNU General Public License for more details.
//
//    You should have received a copy of the GNU General Public License
//    along with this program; if not, write to the Free Software
//    Foundation, Inc., 675 Mass Ave, Cambridge, MA 02139, USA.
//
/////////////////////////////////////////////////////////////////////////////
#endregion


// -------------------------------------------------------------------------
// Lutz Roeder's .NET Mapack, adapted from Mapack for COM and Jama routines.
// Copyright (C) 2001-2003 Lutz Roeder. All rights reserved.
// http://www.aisto.com/roeder/dotnet
// roeder@aisto.com
// -------------------------------------------------------------------------


namespace Altaxo.Calc.LinearAlgebra
{
	using System;
	using System.Text;

	/// <summary>Matrix provides the fundamental operations of numerical linear algebra.</summary>
	public interface IMapackMatrix : IMatrix
	{
		/// <summary>Returns a sub matrix extracted from the current matrix.</summary>
		/// <param name="startRow">Start row index.</param>
		/// <param name="endRow">End row index;</param>
		/// <param name="startColumn">Start column index;</param>
		/// <param name="endColumn">End column index;</param>
		IMapackMatrix Submatrix(int startRow, int endRow, int startColumn, int endColumn);

		/// <summary>Returns a sub matrix extracted from the current matrix.</summary>
		/// <param name="r">Array of row indices;</param>
		/// <param name="c">Array of row indices;</param>
		IMapackMatrix Submatrix(int[] r, int[] c);

		/// <summary>Returns a sub matrix extracted from the current matrix.</summary>
		/// <param name="startRow">Starttial row index.</param>
		/// <param name="endRow">End row index.</param>
		/// <param name="c">Array of row indices.</param>
		IMapackMatrix Submatrix(int startRow, int endRow, int[] c);

		/// <summary>Returns a sub matrix extracted from the current matrix.</summary>
		/// <param name="r">Array of row indices.</param>
		/// <param name="startColumn">Start column index.</param>
		/// <param name="endColumn">End column index.</param>
		IMapackMatrix Submatrix(int[] r, int startColumn, int endColumn);

		/// <summary>Creates a copy of the matrix.</summary>
		IMapackMatrix Clone();

		/// <summary>Returns the transposed matrix.</summary>
		IMapackMatrix Transpose();

		/// <summary>Determinant if matrix is square.</summary>
		double Determinant { get; }

		/// <summary>Returns the One Norm for the matrix.</summary>
		/// <value>The maximum column sum.</value>
		double Norm1 { get; }

		/// <summary>Returns the Infinity Norm for the matrix.</summary>
		/// <value>The maximum row sum.</value>
		double InfinityNorm { get; }

		/// <summary>Return <see langword="true"/> if the matrix is a square matrix.</summary>
		bool IsSquare { get; }

		/// <summary>Returns <see langword="true"/> if the matrix is symmetric.</summary>
		bool IsSymmetric { get; }


		/// <summary>Matrix addition.</summary>
		IMapackMatrix Addition(IMapackMatrix B);

		/// <summary>Matrix-matrix multiplication.</summary>
		IMapackMatrix Multiply(IMapackMatrix B);

		/// <summary>Matrix-scalar multiplication.</summary>
		IMapackMatrix Multiply(double s);

		/// <summary>Matrix subtraction.</summary>
		IMapackMatrix Subtraction(IMapackMatrix B);

		/// <summary>Returns the LHS solution vector if the matrix is square or the least squares solution otherwise.</summary>
		IMapackMatrix Solve(IMapackMatrix rhs);
	}

	/// <summary>
	///   LU decomposition of a rectangular matrix.
	/// </summary>
	/// <remarks>
	///   For an m-by-n matrix <c>A</c> with m >= n, the LU decomposition is an m-by-n
	///   unit lower triangular matrix <c>L</c>, an n-by-n upper triangular matrix <c>U</c>,
	///   and a permutation vector <c>piv</c> of length m so that <c>A(piv)=L*U</c>.
	///   If m &lt; n, then <c>L</c> is m-by-m and <c>U</c> is m-by-n.
	///   The LU decompostion with pivoting always exists, even if the matrix is
	///   singular, so the constructor will never fail.  The primary use of the
	///   LU decomposition is in the solution of square systems of simultaneous
	///   linear equations. This will fail if <see cref="IsNonSingular"/> returns <see langword="false"/>.
	/// </remarks>
	public interface ILuDecomposition
	{
		/// <summary>Returns if the matrix is non-singular.</summary>
		bool IsNonSingular { get; }

		/// <summary>Returns the determinant of the matrix.</summary>
		double Determinant { get; }

		/// <summary>Returns the lower triangular factor <c>L</c> with <c>A=LU</c>.</summary>
		IMapackMatrix LowerTriangularFactor { get; }

		/// <summary>Returns the lower triangular factor <c>L</c> with <c>A=LU</c>.</summary>
		IMapackMatrix UpperTriangularFactor { get; }

		/// <summary>Returns the pivot permuation vector.</summary>
		double[] PivotPermutationVector { get; }

		/// <summary>Solves a set of equation systems of type <c>A * X = B</c>.</summary>
		/// <param name="rhs">Right hand side matrix with as many rows as <c>A</c> and any number of columns.</param>
		/// <returns>Matrix <c>X</c> so that <c>L * U * X = B</c>.</returns>
		IMapackMatrix Solve(IMapackMatrix rhs);
	}



	/// <summary>Matrix provides the fundamental operations of numerical linear algebra.</summary>
	public class Matrix : IMapackMatrix
	{
		private double[][] data;
		private int rows;
		private int columns;

		/// <summary>Constructs an empty matrix of the given size.</summary>
		/// <param name="rows">Number of rows.</param>
		/// <param name="columns">Number of columns.</param>
		public Matrix(int rows, int columns)
		{
			this.rows = rows;
			this.columns = columns;
			this.data = new double[rows][];
			for (int i = 0; i < rows; i++)
			{
				this.data[i] = new double[columns];
			}
		}

		/// <summary>Constructs a matrix of the given size and assigns a given value to all diagonal elements.</summary>
		/// <param name="rows">Number of rows.</param>
		/// <param name="columns">Number of columns.</param>
		/// <param name="value">Value to assign to the diagnoal elements.</param>
		public Matrix(int rows, int columns, double value)
		{
			this.rows = rows;
			this.columns = columns;
			this.data = new double[rows][];
			for (int i = 0; i < rows; i++)
			{
				data[i] = new double[columns];
			}
			for (int i = 0; i < rows; i++)
			{
				data[i][i] = value;
			}
		}

		/// <summary>Constructs a matrix from the given array.</summary>
		/// <param name="data">The array the matrix gets constructed from.</param>
		public Matrix(double[][] data)
		{
			this.rows = data.Length;
			this.columns = data[0].Length;

			for (int i = 0; i < rows; i++)
			{
				if (data[i].Length != columns)
				{
					throw new ArgumentException();
				}
			}

			this.data = data;
		}

		double[][] Array
		{
			get { return data; }
		}

		/// <summary>Returns the number of columns.</summary>
		public int Rows
		{
			get { return rows; }
		}

		/// <summary>Returns the number of columns.</summary>
		public int Columns
		{
			get { return columns; }
		}

		/// <summary>Return <see langword="true"/> if the matrix is a square matrix.</summary>
		public bool IsSquare
		{
			get { return (rows == columns); }
		}

		/// <summary>Returns <see langword="true"/> if the matrix is symmetric.</summary>
		public bool IsSymmetric
		{
			get
			{
				if (this.IsSquare)
				{
					for (int i = 0; i < rows; i++)
					{
						for (int j = 0; j <= i; j++)
						{
							if (data[i][j] != data[j][i])
							{
								return false;
							}
						}
					}

					return true;
				}

				return false;
			}
		}

		/// <summary>Access the value at the given location.</summary>
		public double this[int i, int j]
		{
			set { data[i][j] = value; }
			get { return data[i][j]; }
		}

		/// <summary>Returns a sub matrix extracted from the current matrix.</summary>
		/// <param name="i0">Starttial row index</param>
		/// <param name="i1">End row index</param>
		/// <param name="j0">Start column index</param>
		/// <param name="j1">End column index</param>
		public IMapackMatrix Submatrix(int i0, int i1, int j0, int j1)
		{
			Matrix X = new Matrix(i1 - i0 + 1, j1 - j0 + 1);
			double[][] x = X.Array;
			for (int i = i0; i <= i1; i++)
				for (int j = j0; j <= j1; j++)
					x[i - i0][j - j0] = data[i][j];
			return X;
		}

		/// <summary>Returns a sub matrix extracted from the current matrix.</summary>
		/// <param name="r">Array of row indices</param>
		/// <param name="c">Array of row indices</param>
		public IMapackMatrix Submatrix(int[] r, int[] c)
		{
			Matrix X = new Matrix(r.Length, c.Length);
			double[][] x = X.Array;
			for (int i = 0; i < r.Length; i++)
				for (int j = 0; j < c.Length; j++)
					x[i][j] = data[r[i]][c[j]];
			return X;
		}

		/// <summary>Returns a sub matrix extracted from the current matrix.</summary>
		/// <param name="i0">Starttial row index</param>
		/// <param name="i1">End row index</param>
		/// <param name="c">Array of row indices</param>
		public IMapackMatrix Submatrix(int i0, int i1, int[] c)
		{
			Matrix X = new Matrix(i1 - i0 + 1, c.Length);
			double[][] x = X.Array;
			for (int i = i0; i <= i1; i++)
				for (int j = 0; j < c.Length; j++)
					x[i - i0][j] = data[i][c[j]];
			return X;
		}

		/// <summary>Returns a sub matrix extracted from the current matrix.</summary>
		/// <param name="r">Array of row indices</param>
		/// <param name="j0">Start column index</param>
		/// <param name="j1">End column index</param>
		public IMapackMatrix Submatrix(int[] r, int j0, int j1)
		{
			Matrix X = new Matrix(r.Length, j1 - j0 + 1);
			double[][] x = X.Array;
			for (int i = 0; i < r.Length; i++)
				for (int j = j0; j <= j1; j++)
					x[i][j - j0] = data[r[i]][j];
			return X;
		}

		/// <summary>Creates a copy of the matrix.</summary>
		public IMapackMatrix Clone()
		{
			Matrix X = new Matrix(rows, columns);
			double[][] x = X.Array;
			for (int i = 0; i < rows; i++)
				for (int j = 0; j < columns; j++)
					x[i][j] = data[i][j];
			return X;
		}

		/// <summary>Returns the transposed matrix.</summary>
		public IMapackMatrix Transpose()
		{
			Matrix X = new Matrix(columns, rows);
			double[][] x = X.Array;
			for (int i = 0; i < rows; i++)
				for (int j = 0; j < columns; j++)
					x[j][i] = data[i][j];
			return X;
		}

		/// <summary>Returns the One Norm for the matrix.</summary>
		/// <value>The maximum column sum.</value>
		public double Norm1
		{
			get
			{
				double f = 0;
				for (int j = 0; j < columns; j++)
				{
					double s = 0;
					for (int i = 0; i < rows; i++)
						s += Math.Abs(data[i][j]);
					f = Math.Max(f, s);
				}
				return f;
			}
		}

		/// <summary>Returns the Infinity Norm for the matrix.</summary>
		/// <value>The maximum row sum.</value>
		public double InfinityNorm
		{
			get
			{
				double f = 0;
				for (int i = 0; i < rows; i++)
				{
					double s = 0;
					for (int j = 0; j < columns; j++)
						s += Math.Abs(data[i][j]);
					f = Math.Max(f, s);
				}
				return f;
			}
		}


		/// <summary>Unary minus.</summary>
		public IMapackMatrix UnaryMinus()
		{
			int rows = this.rows;
			int columns = this.columns;
			Matrix X = new Matrix(rows, columns);
			double[][] x = X.Array;
			for (int i = 0; i < rows; i++)
				for (int j = 0; j < columns; j++)
					x[i][j] = -data[i][j];
			return X;
		}

		/// <summary>Matrix addition.</summary>
		public IMapackMatrix Addition(IMapackMatrix B)
		{
			if ((rows != B.Rows) || (columns != B.Columns))
			{
				throw new ArgumentException("Matrix dimension do not match.");
			}
			Matrix X = new Matrix(rows, columns);
			double[][] x = X.Array;
			for (int i = 0; i < rows; i++)
			{
				for (int j = 0; j < columns; j++)
				{
					x[i][j] = data[i][j] + B[i, j];
				}
			}
			return X;
		}

		/// <summary>Matrix subtraction.</summary>
		public IMapackMatrix Subtraction(IMapackMatrix B)
		{
			if ((rows != B.Rows) || (columns != B.Columns))
			{
				throw new ArgumentException("Matrix dimension do not match.");
			}
			Matrix X = new Matrix(rows, columns);
			double[][] x = X.Array;
			for (int i = 0; i < rows; i++)
			{
				for (int j = 0; j < columns; j++)
				{
					x[i][j] = data[i][j] - B[i, j];
				}
			}
			return X;
		}

		/// <summary>Matrix-scalar multiplication.</summary>
		public IMapackMatrix Multiply(double s)
		{
			Matrix X = new Matrix(rows, columns);
			double[][] x = X.Array;
			for (int i = 0; i < rows; i++)
			{
				for (int j = 0; j < columns; j++)
				{
					x[i][j] = data[i][j] * s;
				}
			}

			return X;
		}

		/// <summary>Matrix-matrix multiplication.</summary>
		public IMapackMatrix Multiply(IMapackMatrix B)
		{
			if (B.Rows != this.columns)
			{
				throw new ArgumentException("Matrix dimensions are not valid.");
			}

			int columns = B.Columns;
			Matrix X = new Matrix(rows, columns);
			double[][] x = X.Array;

			int size = this.columns;
			double[] column = new double[size];
			for (int j = 0; j < columns; j++)
			{
				for (int k = 0; k < size; k++)
				{
					column[k] = B[k, j];
				}
				for (int i = 0; i < rows; i++)
				{
					double[] row = data[i];
					double s = 0;
					for (int k = 0; k < size; k++)
					{
						s += row[k] * column[k];
					}
					x[i][j] = s;
				}
			}

			return X;
		}

		/// <summary>Returns the LHS solution vetor if the matrix is square or the least squares solution otherwise.</summary>
		public IMapackMatrix Solve(IMapackMatrix rhs)
		{
            System.Diagnostics.Debug.WriteLine("There is an issue in IMapackMatrix");
			return GetLuDecomposition().Solve(rhs);
		}

		/// <summary>Inverse of the matrix if matrix is square, pseudoinverse otherwise.</summary>
        //public IMapackMatrix Inverse
        //{
        //    get { return Solve(Diagonal(rows, rows, 1.0)); }
        //}

		/// <summary>Determinant if matrix is square.</summary>
		public double Determinant
		{
			get { return GetLuDecomposition().Determinant; }
		}

        ///// <summary>Returns the trace of the matrix.</summary>
        ///// <returns>Sum of the diagonal elements.</returns>
        //public double Trace
        //{
        //    get
        //    {
        //        double trace = 0;
        //        for (int i = 0; i < Math.Min(rows, columns); i++)
        //        {
        //            trace += data[i][i];
        //        }
        //        return trace;
        //    }
		//}

		/// <summary>Returns the matrix in a textual form.</summary>
		public override string ToString()
		{
			StringBuilder builder = new StringBuilder();
			for (int i = 0; i < rows; i++)
			{
				for (int j = 0; j < columns; j++)
					builder.Append(data[i][j] + " ");

				builder.Append(Environment.NewLine);
			}

			return builder.ToString();
		}

		/// <summary>Returns the LU decomposition for this matrix.</summary>
		public ILuDecomposition GetLuDecomposition()
		{
			return new LuDecomposition(this);
		}
		private class LuDecomposition : ILuDecomposition
		{
			private Matrix LU;
			private int pivotSign;
			private int[] pivotVector;

			public LuDecomposition(Matrix A)
			{
				LU = (Matrix)A.Clone();
				double[][] lu = LU.Array;
				int rows = A.Rows;
				int columns = A.Columns;
				pivotVector = new int[rows];
				for (int i = 0; i < rows; i++)
					pivotVector[i] = i;
				pivotSign = 1;
				double[] LUrowi;
				double[] LUcolj = new double[rows];

				// Outer loop.
				for (int j = 0; j < columns; j++)
				{
					// Make a copy of the j-th column to localize references.
					for (int i = 0; i < rows; i++)
						LUcolj[i] = lu[i][j];

					// Apply previous transformations.
					for (int i = 0; i < rows; i++)
					{
						LUrowi = lu[i];

						// Most of the time is spent in the following dot product.
						int kmax = Math.Min(i, j);
						double s = 0.0;
						for (int k = 0; k < kmax; k++)
							s += LUrowi[k] * LUcolj[k];
						LUrowi[j] = LUcolj[i] -= s;
					}

					// Find pivot and exchange if necessary.
					int p = j;
					for (int i = j + 1; i < rows; i++)
						if (Math.Abs(LUcolj[i]) > Math.Abs(LUcolj[p]))
							p = i;

					if (p != j)
					{
						for (int k = 0; k < columns; k++)
						{
							double t = lu[p][k];
							lu[p][k] = lu[j][k];
							lu[j][k] = t;
						}

						int v = pivotVector[p];
						pivotVector[p] = pivotVector[j];
						pivotVector[j] = v;

						pivotSign = -pivotSign;
					}

					// Compute multipliers.

					if (j < rows & lu[j][j] != 0.0)
					{
						for (int i = j + 1; i < rows; i++)
						{
							lu[i][j] /= lu[j][j];
						}
					}
				}
			}

			public bool IsNonSingular
			{
				get
				{
					for (int j = 0; j < LU.Columns; j++)
						if (LU[j, j] == 0)
							return false;
					return true;
				}
			}

			public double Determinant
			{
				get
				{
					if (LU.Rows != LU.Columns) throw new ArgumentException("Matrix must be square.");
					double determinant = (double)pivotSign;
					for (int j = 0; j < LU.Columns; j++)
						determinant *= LU[j, j];
					return determinant;
				}
			}

			public IMapackMatrix LowerTriangularFactor
			{
				get
				{
					int rows = LU.Rows;
					int columns = LU.Columns;
					Matrix X = new Matrix(rows, columns);
					for (int i = 0; i < rows; i++)
						for (int j = 0; j < columns; j++)
							if (i > j)
								X[i, j] = LU[i, j];
							else if (i == j)
								X[i, j] = 1.0;
							else
								X[i, j] = 0.0;
					return X;
				}
			}

			public IMapackMatrix UpperTriangularFactor
			{
				get
				{
					int rows = LU.Rows;
					int columns = LU.Columns;
					Matrix X = new Matrix(rows, columns);
					for (int i = 0; i < rows; i++)
						for (int j = 0; j < columns; j++)
							if (i <= j)
								X[i, j] = LU[i, j];
							else
								X[i, j] = 0.0;
					return X;
				}
			}

			public double[] PivotPermutationVector
			{
				get
				{
					int rows = LU.Rows;
					double[] p = new double[rows];
					for (int i = 0; i < rows; i++)
						p[i] = (double)pivotVector[i];
					return p;
				}
			}

			public IMapackMatrix Solve(IMapackMatrix B)
			{
				if (B.Rows != LU.Rows) throw new ArgumentException("Invalid matrix dimensions.");
				if (!IsNonSingular) throw new InvalidOperationException("Matrix is singular");

				// Copy right hand side with pivoting
				int count = B.Columns;
				IMapackMatrix X = B.Submatrix(pivotVector, 0, count - 1);

				int rows = LU.Rows;
				int columns = LU.Columns;
				double[][] lu = LU.Array;

				// Solve L*Y = B(piv,:)
				for (int k = 0; k < columns; k++)
				{
					for (int i = k + 1; i < columns; i++)
					{
						for (int j = 0; j < count; j++)
						{
							X[i, j] -= X[k, j] * lu[i][k];
						}
					}
				}

				// Solve U*X = Y;
				for (int k = columns - 1; k >= 0; k--)
				{
					for (int j = 0; j < count; j++)
					{
						X[k, j] /= lu[k][k];
					}

					for (int i = 0; i < k; i++)
					{
						for (int j = 0; j < count; j++)
						{
							X[i, j] -= X[k, j] * lu[i][k];
						}
					}
				}

				return X;
			}
		}
	}
}
