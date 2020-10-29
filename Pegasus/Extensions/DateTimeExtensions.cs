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

        public static string LapsedTime(this DateTime value)
        {
            var timeNow = DateTime.Now.ToUniversalTime();
            var lapsedTime = timeNow - value.ToUniversalTime();

            if (lapsedTime < timeNow - timeNow.AddMinutes(-1))
            {
                return "just now";
            }
            if (lapsedTime < timeNow - timeNow.AddHours(-1))
            {
                return FormatLapsedTime(lapsedTime.Minutes, "min");
            }
            if (lapsedTime < timeNow - timeNow.AddDays(-1))
            {
                return FormatLapsedTime(lapsedTime.Hours, "hr");
            }
            if (lapsedTime < timeNow - timeNow.AddMonths(-1))
            {
                return FormatLapsedTime(lapsedTime.Days, "day");
            }
            if (lapsedTime < timeNow - timeNow.AddYears(-1))
            {
                return FormatLapsedTime(lapsedTime.ApproxMonths(), "mth");
            }

            return FormatLapsedTime(lapsedTime.ApproxYears(), "yr");
        }

        private static string FormatLapsedTime(int lapsedTime, string timeUnit)
        {
            return $"{lapsedTime} {timeUnit}{Pluralize(lapsedTime)} ago";
        }

        private static string Pluralize(int value)
        {
            return value > 1 ? "s" : string.Empty;
        }
    }
}
