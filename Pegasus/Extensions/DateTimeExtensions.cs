using System;

namespace Pegasus.Extensions
{
    public static class DateTimeExtensions
    {
        public static string FormattedDate(this DateTime value, string format = null)
        {
            if (string.IsNullOrEmpty(format))
            {
                return value.ToLocalTime().ToString("dd MMM yy, HH:mm");
            }
            else
            {
                return value.ToLocalTime().ToString(format);
            }
        }
        public static string LapsedTime(this DateTime value)
        {
            var timeNow = DateTime.Now.ToUniversalTime();
            var lapsedTime = timeNow - value.ToLocalTime().ToUniversalTime();

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

        public static string ToTaskDate(this DateTime value)
        {
            if (DateTime.Today == value.Date.ToLocalTime())
            {
                return value.ToString("t");
            }
            return value.ToString("MMM dd");
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
