
using System;

namespace Altaxo.Calc.LinearAlgebra
{
	public interface INumericSequence
	{
		/// <summary>Gets the element of the sequence at index i.</summary>
		/// <value>The element at index i.</value>
		double this[int i] { get; }
	}

	/// <summary>
	/// Interface for a read-only vector of double values. The first valid index of this vector is 0, the last one in (<see cref="Length"/>-1).
	/// </summary>
	public interface IROVector : INumericSequence
	{
		/// <summary>The number of elements of this vector.</summary>
		int Length { get; }  // change this later to length property
	}

	/// <summary>
	/// Interface for a readable and writeable vector of double values.
	/// </summary>
	public interface IVector : IROVector
	{
		/// <summary>Read/write Accessor for the element at index i.</summary>
		/// <value>The element at index i.</value>
		new double this[int i] { get; set; }
	}
}
