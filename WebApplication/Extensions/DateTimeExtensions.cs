using System;
using K9.Globalisation;

namespace K9.Base.WebApplication.Extensions
{
    public static class DateTimeExtensions
    {

        public static string ToHumanReadableString(this DateTime value)
        {
            var now = DateTime.Now;
            var elapsed = now.Subtract(value);
            var duration = value.Subtract(now);
            var isInPast = value < now;
            var timeSpanText = isInPast ? Dictionary.TimespanAgo : Dictionary.TimespanFuture;
            var briefTimeText = isInPast ? Dictionary.JustNow : Dictionary.Shortly;
            var timespan = isInPast ? elapsed : duration;
            var oneDaySpan = isInPast ? Dictionary.Yesterday : Dictionary.Tomorrow;
      
            if (timespan.TotalSeconds < 60)
            {
                return briefTimeText;
            }
            if (timespan.TotalMinutes < 2)
            {
                return string.Format(timeSpanText, 1, Dictionary.Minute.ToLower());
            }
            if (timespan.TotalHours < 1)
            {
                return String.Format(timeSpanText, (int)timespan.TotalMinutes,
                    Dictionary.Minutes.ToLower());
            }
            if ((int)timespan.TotalHours == 1)
            {
                return string.Format(timeSpanText, 1, Dictionary.Hour.ToLower());
            }
            if ((int)timespan.TotalDays < 1)
            {
                return string.Format(timeSpanText, (int)timespan.TotalHours, Dictionary.Hours.ToLower());
            }
            if ((int)timespan.TotalDays < 2)
            {
                return oneDaySpan;
            }
            if ((int)timespan.TotalDays < 7)
            {
                return string.Format(timeSpanText, (int)timespan.TotalDays, Dictionary.Days.ToLower());
            }
            if ((int)timespan.TotalDays == 7)
            {
                return string.Format(timeSpanText, 1, Dictionary.Week.ToLower());
            }
            if ((int)timespan.TotalDays < 14)
            {
                return string.Format(timeSpanText, (int)timespan.TotalDays, Dictionary.Days.ToLower());
            }
            if ((int)timespan.TotalDays == 14)
            {
                return string.Format(timeSpanText, 2, Dictionary.Weeks.ToLower());
            }
            if ((int)timespan.TotalDays < 21)
            {
                return string.Format(timeSpanText, (int)timespan.TotalDays, Dictionary.Days.ToLower());
            }
            if ((int)timespan.TotalDays == 21)
            {
                return string.Format(timeSpanText, 3, Dictionary.Weeks.ToLower());
            }
            if (timespan.TotalDays < (isInPast ? now.Subtract(now.AddMonths(-1)).TotalDays : now.AddMonths(1).Subtract(now).TotalDays))
            {
                return string.Format(timeSpanText, (int)timespan.TotalDays, Dictionary.Days.ToLower());
            }
            if (timespan.TotalDays < (isInPast ? now.Subtract(now.AddMonths(-2)).TotalDays : now.AddMonths(2).Subtract(now).TotalDays))
            {
                return string.Format(timeSpanText, 1, Dictionary.Month.ToLower());
            }
            if (timespan.TotalDays < (isInPast ? now.Subtract(now.AddMonths(-3)).TotalDays : now.AddMonths(3).Subtract(now).TotalDays))
            {
                return string.Format(timeSpanText, 2, Dictionary.Months.ToLower());
            }
            if (timespan.TotalDays < (isInPast ? now.Subtract(now.AddMonths(-4)).TotalDays : now.AddMonths(4).Subtract(now).TotalDays))
            {
                return string.Format(timeSpanText, 3, Dictionary.Months.ToLower());
            }
            if (timespan.TotalDays < (isInPast ? now.Subtract(now.AddMonths(-5)).TotalDays : now.AddMonths(5).Subtract(now).TotalDays))
            {
                return string.Format(timeSpanText, 4, Dictionary.Months.ToLower());
            }
            if (timespan.TotalDays < (isInPast ? now.Subtract(now.AddMonths(-6)).TotalDays : now.AddMonths(6).Subtract(now).TotalDays))
            {
                return string.Format(timeSpanText, 5, Dictionary.Months.ToLower());
            }
            if (timespan.TotalDays < (isInPast ? now.Subtract(now.AddMonths(-7)).TotalDays : now.AddMonths(7).Subtract(now).TotalDays))
            {
                return string.Format(timeSpanText, 6, Dictionary.Months.ToLower());
            }
            if (timespan.TotalDays < (isInPast ? now.Subtract(now.AddMonths(-8)).TotalDays : now.AddMonths(8).Subtract(now).TotalDays))
            {
                return string.Format(timeSpanText, 7, Dictionary.Months.ToLower());
            }
            if (timespan.TotalDays < (isInPast ? now.Subtract(now.AddMonths(-9)).TotalDays : now.AddMonths(9).Subtract(now).TotalDays))
            {
                return string.Format(timeSpanText, 8, Dictionary.Months.ToLower());
            }
            if (timespan.TotalDays < (isInPast ? now.Subtract(now.AddMonths(-10)).TotalDays : now.AddMonths(10).Subtract(now).TotalDays))
            {
                return string.Format(timeSpanText, 9, Dictionary.Months.ToLower());
            }
            if (timespan.TotalDays < (isInPast ? now.Subtract(now.AddMonths(-11)).TotalDays : now.AddMonths(11).Subtract(now).TotalDays))
            {
                return string.Format(timeSpanText, 10, Dictionary.Months.ToLower());
            }
            if (timespan.TotalDays < (isInPast ? now.Subtract(now.AddMonths(-12)).TotalDays : now.AddMonths(12).Subtract(now).TotalDays))
            {
                return string.Format(timeSpanText, 11, Dictionary.Months.ToLower());
            }
            if (timespan.TotalDays < (isInPast ? now.Subtract(now.AddYears(-1).AddMonths(-6)).TotalDays : now.AddYears(1).AddMonths(6).Subtract(now).TotalDays))
            {
                return string.Format(timeSpanText, 1, Dictionary.Year.ToLower());
            }
            if (timespan.TotalDays < (isInPast ? now.Subtract(now.AddYears(-2)).TotalDays : now.AddYears(2).Subtract(now).TotalDays))
            {
                return string.Format(timeSpanText, 18, Dictionary.Months.ToLower());
            }
            {
                return string.Format(timeSpanText, now.GetYearsElapsedSince(value),
                    Dictionary.Years.ToLower());
            }
        }

        public static int GetYearsElapsedSince(this DateTime value, DateTime dateToCompare)
        {
            if (dateToCompare < value)
            {
                return value.Year - dateToCompare.Year - (dateToCompare.Day > value.Day ? 1 : 0);
            }
            return dateToCompare.Year - value.Year - (dateToCompare.Day < value.Day ? 1 : 0);
        }
    }

}
