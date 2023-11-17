using System;
using System.Collections.Generic;
using System.Text;

namespace ArchiveFlow.Utilities
{
    public static class Guard
    {
        public static void AgainstNull<T>(string paramName, T value) 
        {
            if (value == null)
            {
                throw new ArgumentNullException($"{paramName} can not be null.");
            }
        }

        public static void AgainsNullOrWhiteSpace(string paramName, string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException($"{paramName} can not be null or white space.");
            }
        }

        public static void AgainstSmallerThan<T>(string paramName, T value, T compare)
        {
            if (Comparer<T>.Default.Compare(value, compare) < 0)
            {
                throw new ArgumentOutOfRangeException($"{paramName} can not be smaller than {compare}.");
            }
        }

        public static void ShouldStartWith(string paramName, string value, string start)
        {
            if (!value.StartsWith(start))
            {
                throw new ArgumentException($"{paramName} should start with '{start}'.");
            }
        }
    }
}
