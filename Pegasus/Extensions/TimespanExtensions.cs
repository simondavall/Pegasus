using System;

namespace Pegasus.Extensions
{

    public static class TimeSpanExtensions
    {
        private const double ApproxNumberOfDaysInYear = 365;
        private const double ApproxNumberOfDaysInMonth = 30;

        public static int ApproxYears(this TimeSpan timespan)
        {
            return (int)(timespan.Days / ApproxNumberOfDaysInYear);
        }

        public static int ApproxMonths(this TimeSpan timespan)
        {
            return (int)(timespan.Days / ApproxNumberOfDaysInMonth);
        }
    }
}
