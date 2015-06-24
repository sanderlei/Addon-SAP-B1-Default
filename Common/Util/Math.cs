using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common.Util
{
    public class Math
    {
        public static double RoundUp(double value, int decimalPlaces)
        {
            double multiplier = System.Math.Pow(10, Convert.ToDouble(decimalPlaces));
            return System.Math.Ceiling(value * multiplier) / multiplier;
        }
    }
}
