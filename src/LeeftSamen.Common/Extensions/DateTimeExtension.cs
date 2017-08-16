// <copyright file="DateTimeExtension.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

namespace LeeftSamen.Common.Extensions
{
    using InterfaceText;
    using System;
    using System.Globalization;

    /// <summary>
    /// The date time extension.
    /// </summary>
    public static class DateTimeExtension
    {
        /// <summary>
        /// The is fresh.
        /// </summary>
        /// <param name="dateTime">
        /// The date time.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public static bool IsFresh(this DateTime dateTime)
        {
            return (DateTime.Now - dateTime) <= TimeSpan.FromHours(24);
        }

        /// <summary>
        /// The is today.
        /// </summary>
        /// <param name="dateTime">
        /// The date time.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public static bool IsToday(this DateTime dateTime)
        {
            return dateTime.Date == DateTime.Today;
        }

        /// <summary>
        /// The is tomorrow.
        /// </summary>
        /// <param name="dateTime">
        /// The date time.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public static bool IsTomorrow(this DateTime dateTime)
        {
            return dateTime.Date == DateTime.Now.AddDays(1).Date;
        }

        /// <summary>
        /// The is yesterday.
        /// </summary>
        /// <param name="dateTime">
        /// The date time.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public static bool IsYesterday(this DateTime dateTime)
        {
            return dateTime.Date == DateTime.Now.AddDays(-1).Date;
        }

        /// <summary>
        /// Formats a date into the default format for the site
        /// </summary>
        /// <param name="dateTime">
        /// The dateTime to format
        /// </param>
        /// <returns>
        /// Date in format: 'dd MMM yyyy'
        /// </returns>
        public static string ToDateString(this DateTime dateTime)
        {
            return dateTime.ToString("dd MMM yyyy");
        }

        /// <summary>
        /// The to time back.
        /// </summary>
        /// <param name="dateTime">
        /// The date time.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string ToTimeAgoString(this DateTime dateTime)
        {
            var timeSpan = DateTime.Now - dateTime;
            if (timeSpan <= TimeSpan.FromSeconds(60))
            {
                return string.Format(timeSpan.Seconds == 1 ? Label.SecondAgo : Label.SecondsAgo, timeSpan.Seconds);
            }

            if (timeSpan <= TimeSpan.FromMinutes(60))
            {
                return string.Format(timeSpan.Minutes == 1 ? Label.MinuteAgo : Label.MinutesAgo, timeSpan.Minutes);
            }

            if (timeSpan <= TimeSpan.FromHours(24))
            {
                return string.Format(timeSpan.Hours == 1 ? Label.HourAgo : Label.HoursAgo, timeSpan.Hours);
            }

            if (timeSpan <= TimeSpan.FromDays(30))
            {
                return string.Format(timeSpan.Days == 1 ? Label.DayAgo : Label.DaysAgo, timeSpan.Days);
            }

            if (timeSpan <= TimeSpan.FromDays(365))
            {
                return string.Format(timeSpan.Days / 30 == 1 ? Label.MonthAgo : Label.MonthsAgo, timeSpan.Days / 30);
            }

            return string.Format(timeSpan.Days / 365 == 1 ? Label.YearAgo : Label.YearsAgo, timeSpan.Days / 365);
        }

        public static int WeekOfMonth(this DateTime dateTime)
        {
            var info = DateTimeFormatInfo.CurrentInfo;
            var cal = info.Calendar;
            var yearWeek1 = cal.GetWeekOfYear(dateTime, info.CalendarWeekRule, info.FirstDayOfWeek);
            var yearWeek2 = cal.GetWeekOfYear(
                new DateTime(dateTime.Year, dateTime.Month, 1),
                info.CalendarWeekRule,
                info.FirstDayOfWeek);
            if (yearWeek2 > yearWeek1)
            {
                yearWeek2 = yearWeek1 - 1;
            }

            return (yearWeek1 - yearWeek2) + 1;
        }
    }
}