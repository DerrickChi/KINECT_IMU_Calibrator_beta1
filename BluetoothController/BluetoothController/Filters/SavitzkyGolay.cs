using BluetoothController;
using System;
using Altaxo.Calc.LinearAlgebra;

namespace BluetoothController
{
  /// <summary>
  /// Stores the set of parameters necessary to calculate Savitzky-Golay coefficients.
  /// </summary>
  public class SavitzkyGolayParameters
  {
    /// <summary>
    /// Number of points used for Savitzky Golay Coefficients. Must be a odd positive number.
    /// </summary>
    public int NumberOfPoints=7;
    
    /// <summary>
    /// Polynomial order used to calculate Savitzky-Golay coefficients. Has to be a positive number.
    /// </summary>
    public int PolynomialOrder=5;
    
    /// <summary>
    /// Derivative order. Has to be zero or positive. A value of zero is used to smooth a function.
    /// </summary>
    public int DerivativeOrder=0;
  }

  /// <summary>
  /// SavitzkyGolay implements the calculation of the Savitzky-Golay filter coefficients and their application
  /// to smoth data, and to calculate derivatives.
  /// </summary>
  /// <remarks>Ref.: "Numerical recipes in C", chapter 14.8</remarks>
  public class SavitzkyGolay : IFilter
  {
    /// <summary>
    /// Calculate Savitzky-Golay coefficients.
    /// </summary>
    /// <param name="leftpoints">Points on the left side included in the regression.</param>
    /// <param name="rightpoints">Points to the right side included in the regression.</param>
    /// <param name="derivativeorder">Order of derivative for which the coefficients are calculated.</param>
    /// <param name="polynomialorder">Order of the regression polynomial.</param>
    /// <param name="coefficients">Output: On return, contains the calculated coefficients.</param>
    public static void GetCoefficients(int leftpoints, int rightpoints, int derivativeorder, int polynomialorder, IVector coefficients)
    {
      int totalpoints = leftpoints+rightpoints+1;
      // Presumtions leftpoints and rightpoints must be >=0
      if(leftpoints<0)
        throw new ArgumentException("Argument leftpoints must not be <=0!");
      if(rightpoints<0)
        throw new ArgumentException("Argument rightpoints must not be <=0!");
      if(totalpoints<=1)
        throw new ArgumentException("Argument leftpoints and rightpoints must not both be zero!");
      if(polynomialorder>=totalpoints)
        throw new ArgumentException("Argument polynomialorder must not be smaller than total number of points");
      if(derivativeorder>polynomialorder)
        throw new ArgumentException("Argument derivativeorder must not be greater than polynomialorder!");
      if(coefficients==null || coefficients.Length<totalpoints)
        throw new ArgumentException("Vector of coefficients is either null or too short");
      // totalpoints must be greater than 1

      // Set up the design matrix
      // this is the matrix of i^j where i ranges from -leftpoints..rightpoints and j from 0 to polynomialorder 
      // as usual for regression, we not use the matrix directly, but instead the covariance matrix At*A
      Matrix mat = new Matrix(polynomialorder+1,polynomialorder+1);
      
      double[] val = new double[totalpoints];
      for(int i=0;i<totalpoints;i++) val[i]=1;

      for(int ord = 0;ord<=polynomialorder;ord++)
      {
        double sum = VectorMath.Sum(val);
        for(int i=0;i<=ord;i++)
          mat[ord-i,i] = sum;
        for(int i=0;i<totalpoints;i++)
          val[i] *= (i-leftpoints);
      }

      for(int ord = polynomialorder-1; ord>=0;ord--)
      {
        double sum = VectorMath.Sum(val);
        for(int i=0;i<=ord;i++)
          mat[polynomialorder-i,polynomialorder-ord+i] = sum;
        for(int i=0;i<totalpoints;i++)
          val[i] *= (i-leftpoints);
      }

      // now solve the equation
      ILuDecomposition decompose = mat.GetLuDecomposition();
      // ISingularValueDecomposition decompose = mat.GetSingularValueDecomposition();
      Matrix y = new Matrix(polynomialorder+1,1);
      y[derivativeorder,0] = 1;
      IMapackMatrix result = decompose.Solve(y);
    
      // to get the coefficients, the parameter have to be multiplied by i^j and summed up
      for(int i= -leftpoints;i<=rightpoints;i++)
      {
        double sum = 0;
        double x=1;
        for (int j=0;j<=polynomialorder;j++,x*=i)
          sum += result[j,0]*x;
        coefficients[i+leftpoints]=sum;
      }
    }

    /// <summary>Filter to apply to the middle of the array.</summary>
    double[][]   _middle;

    Tuple<double,long>[] rawPositionData;
    private int windowSize;
    //public enum NDerivative { smooth, first, second }

    /// <summary>
    /// This sets up a Savitzky-Golay filter.
    /// </summary>
    /// <param name="numberOfPoints">Number of points. Must be an odd number, otherwise it is rounded up.</param>
    /// <param name="derivativeOrder">Order of derivative you want to obtain. Set 0 for smothing.</param>
    /// <param name="polynomialOrder">Order of the fitting polynomial. Usual values are 2 or 4.</param>
    public SavitzkyGolay(int numberOfPoints, /*int derivativeOrder, */int polynomialOrder)
    {
        numberOfPoints = 1 + 2 * (numberOfPoints / 2);
        int numberOfSide = (numberOfPoints - 1) / 2;
        _middle = new double[3][];
        for (int i = 0; i < _middle.Length; i++)
            _middle[i] = new double[numberOfPoints];
        //numberOfPoints = 1+2*(numberOfPoints/2);

        windowSize = numberOfPoints;
        rawPositionData = new Tuple<double, long>[windowSize];
        for (int i = 0; i < windowSize; i++)
            rawPositionData[i] = new Tuple<double, long>(double.NaN, 0);

        for (int i = 0; i < _middle.Length; i++)
            GetCoefficients(numberOfSide, numberOfSide, i, polynomialOrder, VectorMath.ToVector(_middle[i]));
    }


    public IFilter UpdateVal(double newPosition, long newTime)
    {
        Tuple<double, long> newVal = new Tuple<double, long>(newPosition, newTime);
        for (int i = 0; i < windowSize - 1; i++)
            rawPositionData[i] = rawPositionData[i + 1];
        rawPositionData[windowSize - 1] = newVal;
        return (IFilter) this;
    }


    /// <summary>
    /// This applies the set-up filter to an array of numbers. The left and right side is special treated by
    /// applying Savitzky-Golay with appropriate adjusted left and right number of points.
    /// </summary>
    /// <param name="array">The array of numbers to filter.</param>
    /// <param name="result">The resulting array. Must not be identical to the input array!</param>

    public Tuple<double, long> GetNDerivative(NDerivative nd)
    {
        try
        {
            for (int i = 0; i < windowSize; i++)
                if (rawPositionData[i].Item1 == double.NaN)
                    return null;

            double[] middle = new double[0];
            double hsum = 0, h = 0;

            //average the h parameter
            for (int i = 0; i < rawPositionData.Length - 1; i++)
                hsum += rawPositionData[i + 1].Item2 - rawPositionData[i].Item2;

            h = hsum / (rawPositionData.Length - 1);
            h *= (1.0 / TimeSpan.TicksPerSecond);

            switch (nd)
            {
                case NDerivative.smooth:
                    middle = _middle[0];
                    break;
                case NDerivative.first:
                    middle = _middle[1];
                    break;
                case NDerivative.second:
                    middle = _middle[2];
                    break;
            }

            int filterPoints = middle.Length;
            int sidePoints = (filterPoints - 1) / 2;
            double result = 0;

            double sum = 0;
            for (int i = 0; i < filterPoints; i++)
                sum += rawPositionData[i].Item1 * middle[i];
            result = sum;

            switch (nd)
            {
                case NDerivative.first:
                    result *= 1.0 / (h);
                    break;
                case NDerivative.second:
                    result *= 2.0 / (Math.Pow(h, 2.0));
                    break;
            }



            return new Tuple<double, long>(result, rawPositionData[windowSize / 2].Item2);
        }
        catch (Exception e)
        {
            System.Diagnostics.Debug.WriteLine(e.ToString() + " In SGS");
            throw e;
        }
    }
  } 
} 
