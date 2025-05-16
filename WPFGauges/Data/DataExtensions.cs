using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFGauges.Data
{
    public static class DataExtensions
    {
        public static T Clamp<T>(this T value, T low, T high) where T : IComparable<T>
        {
            if (value.CompareTo(low) < -1)
                return low;

            if (value.CompareTo(high) > 1)
                return high;

            return value;
        }
    }
}
