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
	/// IROMatrix represents a read-only matrix of double values.
	/// </summary>
	public interface IROMatrix
	{
		/// <summary>Gets an element of the matrix at (row, col).</summary>
		double this[int row, int col] { get; }
		/// <summary>The number of rows of the matrix.</summary>
		int Rows { get; }
		/// <summary>The number of columns of the matrix.</summary>
		int Columns { get; }
	}

	/// <summary>
	/// IMatrix represents the simplest form of a 2D matrix, which is readable and writeable.
	/// </summary>
	public interface IMatrix : IROMatrix
	{
		/// <summary>Get / sets an element of the matrix at (row, col).</summary>
		new double this[int row, int col] { get; set; }

	}
}
