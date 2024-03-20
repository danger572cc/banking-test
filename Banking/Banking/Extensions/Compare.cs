using System;

namespace Banking.Extensions
{
    public static class Compare
    {
        public static bool StringEqual(string a, string b)
        {
            if (a == null || b == null)
                return (a == null && b == null);

            if (string.IsNullOrWhiteSpace(a) || string.IsNullOrWhiteSpace(b))
                return (string.IsNullOrWhiteSpace(a) && string.IsNullOrWhiteSpace(b));


            return a.Trim().Equals(b.Trim(), StringComparison.OrdinalIgnoreCase);
        }
    }
}
