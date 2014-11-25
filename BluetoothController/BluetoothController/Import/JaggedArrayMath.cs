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

using System;

namespace Altaxo.Calc.LinearAlgebra
{
	/// <summary>
	/// This provides array math for a special case of matrices, so called jagged arrays.
	/// </summary>
	public class JaggedArrayMath
	{
		private JaggedArrayMath() { }

		/// <summary>
		/// Allocates an array of n x m values.
		/// </summary>
		/// <param name="n">First matrix dimension (rows).</param>
		/// <param name="m">Second matrix dimension( columns).</param>
		/// <returns>Array of dimensions n x m.</returns>
		public static double[][] GetMatrixArray(int n, int m)
		{
			double[][] result = new double[n][];
			for (int i = 0; i < n; i++)
				result[i] = new double[m];

			return result;
		}
	}
}
