using System;

namespace Pegasus.Extensions
{
    public static class DateTimeExtensions
    {
        public static string ToTaskDate(this DateTime value)
        {
            if (DateTime.Today == value.Date)
            {
                return value.ToString("t");
            }
            return value.ToString("MMM dd");
        }
    }
}
