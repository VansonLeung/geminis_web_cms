using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;

namespace WebApplication2.Helpers
{
    public static class DateTimeExtensions
    {
        public static DateTime GetServerTime()
        {
            return DateTime.Now;
        }

        public static DateTime StringToDateTime(string str)
        {
            return DateTime.ParseExact(str, "MM/dd/yyyy h:mm tt", new CultureInfo("en-US"));
        }
        public static String DateTimeToString(DateTime? dt)
        {
            var datetime = dt.GetValueOrDefault();
            if (datetime == null)
            {
                return "";
            }

            return datetime.ToString("MM/dd/yyyy h:mm tt", new CultureInfo("en-US"));
        }

        public static String DateTimeToStringBR(DateTime? dt)
        {
            if (!dt.HasValue)
            {
                return "";
            }

            var datetime = dt.GetValueOrDefault();
            if (datetime == null)
            {
                return "";
            }

            return datetime.ToString("yyyy/MM/dd\nHH:mm:ss", new CultureInfo("en-US"));
        }


        private static int DateValue(this DateTime dt)
        {
            return dt.Year * 372 + (dt.Month - 1) * 31 + dt.Day - 1;
        }

        public static int YearsBetween(this DateTime dt, DateTime dt2)
        {
            return dt.MonthsBetween(dt2) / 12;
        }

        public static int YearsBetween(this DateTime dt, DateTime dt2, bool includeLastDay)
        {
            return dt.MonthsBetween(dt2, includeLastDay) / 12;
        }

        public static int YearsBetween(this DateTime dt, DateTime dt2, bool includeLastDay, out int excessMonths)
        {
            int months = dt.MonthsBetween(dt2, includeLastDay);
            excessMonths = months % 12;
            return months / 12;
        }

        public static int MonthsBetween(this DateTime dt, DateTime dt2)
        {
            int months = (dt2.DateValue() - dt.DateValue()) / 31;
            return Math.Abs(months);
        }

        public static int MonthsBetween(this DateTime dt, DateTime dt2, bool includeLastDay)
        {
            if (!includeLastDay) return dt.MonthsBetween(dt2);
            int days;
            if (dt2 >= dt)
                days = dt2.AddDays(1).DateValue() - dt.DateValue();
            else
                days = dt.AddDays(1).DateValue() - dt2.DateValue();
            return days / 31;
        }

        public static int WeeksBetween(this DateTime dt, DateTime dt2)
        {
            return dt.DaysBetween(dt2) / 7;
        }

        public static int WeeksBetween(this DateTime dt, DateTime dt2, bool includeLastDay)
        {
            return dt.DaysBetween(dt2, includeLastDay) / 7;
        }

        public static int WeeksBetween(this DateTime dt, DateTime dt2, bool includeLastDay, out int excessDays)
        {
            int days = dt.DaysBetween(dt2, includeLastDay);
            excessDays = days % 7;
            return days / 7;
        }

        public static int DaysBetween(this DateTime dt, DateTime dt2)
        {
            return (dt2.Date - dt.Date).Duration().Days;
        }

        public static int DaysBetween(this DateTime dt, DateTime dt2, bool includeLastDay)
        {
            int days = dt.DaysBetween(dt2);
            if (!includeLastDay) return days;
            return days + 1;
        }
    }

}