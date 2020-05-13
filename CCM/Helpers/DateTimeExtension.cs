using Nager.Date;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CCM.Helpers
{
    public static class DateTimeExtension
    {
        public static DateTime AddWorkdays(this DateTime originalDate, double workDays)
        {
            DateTime tmpDate = originalDate;
            while (workDays > 0)
            {
                tmpDate = tmpDate.AddDays(1);
                if (tmpDate.DayOfWeek < DayOfWeek.Saturday &&
                    tmpDate.DayOfWeek > DayOfWeek.Sunday &&
                    !DateTimeExtension.IsHoliday(tmpDate))
                    workDays--;
            }
            return tmpDate;
        }
   
        
        //install-package Nager.Date
        public static bool IsHoliday(DateTime? d)
        {
            DateTime date = d.Value;
            if (DateSystem.IsPublicHoliday(date, CountryCode.US))
            {
                return true;
            }
            return false;
        }


        // give days difference exclude start and end Date
        public static double TotalDaysLeftInMinutes(DateTime? startDate, DateTime? endDate, Boolean excludeWeekends)
        {
            startDate = startDate.Value.AddDays(1);
            double count = 0;
            for (DateTime? index = startDate; index.Value.Date < endDate.Value.Date; index = index.Value.AddDays(1))
            {
                if (excludeWeekends && index.Value.DayOfWeek != DayOfWeek.Sunday && index.Value.DayOfWeek != DayOfWeek.Saturday)
                {
                    bool excluded = false; 
                    if (DateTimeExtension.IsHoliday(index))
                    {
                        excluded = true;
                    }

                    if (!excluded)
                    {
                        count++;
                    }
                }
            }

            return (count*480);
        }

        // give method to total days and it return days into minutes
        public static double GetWorkingHoursMinutesFromTotalNumberOfDays(int? days)
        {
            if (days != null)
            {
                double val = Convert.ToDouble(days * 480);
                return val;
            }
            return 0;
        }

       // give startdate and it will give you a remaining minute from office close time. 
        public static double timedifferenceInMinutes(DateTime offDateTime, DateTime? nowDate)
        {
            var totalMints = (offDateTime - nowDate).Value.TotalMinutes;
            if (totalMints > 0)
            {
                return totalMints;
            }
            return 0;
            //return String.Format("{0} days, {1} hours, {2} minutes, {3} seconds",
            //    span.Days, span.Hours, span.Minutes, span.Seconds);
        }

        // give method to total hours and it return hours into minutes
        public static double getTotalMinutesFromHours(int hours)
        {
            return hours * 60;
        }

        public static double timedifferenceHourse { get; set; }
        public static double gettimedifference
        {
            get { return (timedifferenceHourse*480); }
            set { timedifferenceHourse = value; }
        }

        public static double ConvertToUnixTimestamp(DateTime date)
        {
            DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            TimeSpan diff = date.ToUniversalTime() - origin;
            return Math.Floor(diff.TotalSeconds);
        }

        public static string ConvertDateIntoString(DateTime? date)
        {
            if (date != null)
            {
               return String.Format("{0:G}", date);
            }
            return "---";

        }
    }
}