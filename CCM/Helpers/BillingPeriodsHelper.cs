using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CCM.Models;
using CCM.Models.CCMBILLINGS;

namespace CCM.Helpers
{
    public static class BillingPeriodsHelper
    {
       
        private static readonly ApplicationdbContect _db = new ApplicationdbContect();
        private static List<BillingPeriods> BillingPeriodsList = _db.BillingPeriods.ToList();

        public static string MonthlyPeriod = "Monthly";
        public static string QuarterlyPeriod = "Quarterly";
        public static string SixMonthPeriod = "Every 6 Month";
        public static string YearlyPeriod = "Yearly / Every 12 Month";
        public static string OneTimePeriod = "One Time";

        public static int MonthlyPeriod_ID = BillingPeriodsList.Where(x => x.Name.ToLowerInvariant() == MonthlyPeriod.ToLowerInvariant()).Select(x => x.BillingPeriodsId).FirstOrDefault();
        public static int QuarterlyPeriod_ID = BillingPeriodsList.Where(x => x.Name.ToLowerInvariant() == QuarterlyPeriod.ToLowerInvariant()).Select(x => x.BillingPeriodsId).FirstOrDefault();
        public static int SixMonthPeriod_ID = BillingPeriodsList.Where(x => x.Name.ToLowerInvariant() == SixMonthPeriod.ToLowerInvariant()).Select(x => x.BillingPeriodsId).FirstOrDefault();
        public static int YearlyPeriod_ID = BillingPeriodsList.Where(x => x.Name.ToLowerInvariant() == YearlyPeriod.ToLowerInvariant()).Select(x => x.BillingPeriodsId).FirstOrDefault();
        public static int OneTimePeriod_ID = BillingPeriodsList.Where(x => x.Name.ToLowerInvariant() == OneTimePeriod.ToLowerInvariant()).Select(x => x.BillingPeriodsId).FirstOrDefault();

    }
}