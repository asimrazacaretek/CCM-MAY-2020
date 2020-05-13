using CCM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
namespace CCM.Helpers
{
    public static class BillingCodeHelper
    {
        private static readonly ApplicationdbContect _db = new ApplicationdbContect();
        public static string ccmEnroll = "Enrolled";
        public static string ccm = "ccm";
        public static string G0506 = "G0506 INITIAL VISIT";
        public static string RPM = "RPM";
        private static string onetime = "one time";
        private static string Monthly = "Monthly";
        private static string SixMonth = "Every 6 Month";
        private static string Yearly = "Yearly / Every 12 Month";
        private static string Quarterly = "Quarterly";
  
        public static int cmmBillingCatagoryid = _db.BillingCategories.Where(x => x.Name.ToLower() == ccm || x.Name.StartsWith(ccm) || x.Name.Contains(ccm)).Select(x => x.BillingCategoryId).FirstOrDefault();
        public static int G0506BillingCatagoryid = _db.BillingCategories.Where(x => x.Name.ToLower() == G0506 || x.Name.StartsWith(G0506) || x.Name.Contains(G0506)).Select(x => x.BillingCategoryId).FirstOrDefault();
        public static int RPMBillingCatagoryid = _db.BillingCategories.Where(x => x.Name.ToLower() == RPM || x.Name.StartsWith(RPM) || x.Name.Contains(RPM)).Select(x => x.BillingCategoryId).FirstOrDefault();


        public static int MonthalyPeriodId = _db.BillingPeriods.Where(x => x.Name.ToLower() == Monthly || x.Name.StartsWith(Monthly) || x.Name.Contains(Monthly)).Select(x => x.BillingPeriodsId).FirstOrDefault();
        public static int OneTimePeriodId = _db.BillingPeriods.Where(x => x.Name.ToLower() == onetime || x.Name.StartsWith(onetime) || x.Name.Contains(onetime)).Select(x => x.BillingPeriodsId).FirstOrDefault();

        public static string GetBillingCatagoryNameById(this int billingcategoryId)
        {
            return _db.BillingCategories.Where(p => p.BillingCategoryId == billingcategoryId).Select(p => p.Name).FirstOrDefault();
        }
       
        public static int GetBillingCatagoryIdByName(this string billingcategoryName)
        {
            return _db.BillingCategories.Where(p => p.Name == billingcategoryName).Select(p => p.BillingCategoryId).FirstOrDefault();
        }
        public static bool IsOneTimeBillingPeriod(int BillingCatgoryId)
        {

           
            var Billingcategory = _db.BillingCategories.Where(p => p.BillingCategoryId == BillingCatgoryId).FirstOrDefault();
            if (Billingcategory != null)
            {
                if (Billingcategory.BillingPeriodsId == OneTimePeriodId)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }

            return false;
        }

        
    }
}