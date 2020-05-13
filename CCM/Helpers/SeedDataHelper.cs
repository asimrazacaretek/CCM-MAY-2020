using CCM.Models;
using CCM.Models.CCMBILLINGS;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CCM.Helpers
{
    public static class SeedDataHelper
    {
        private static readonly ApplicationdbContect _db = new ApplicationdbContect();

        public static List<string> BillingPeriodList = new List<string>()
        {
            BillingPeriodsHelper.MonthlyPeriod,
            BillingPeriodsHelper.QuarterlyPeriod,
            BillingPeriodsHelper.SixMonthPeriod,
            BillingPeriodsHelper.YearlyPeriod,
            BillingPeriodsHelper.OneTimePeriod
        };

        internal static void SeedData()
        {
            //Seed Billing Periods
            if (_db.BillingPeriods.ToList().Count() < BillingPeriodList.Count())
            {
                foreach (var i in BillingPeriodList)
                {
                    var billingPeriod = _db.BillingPeriods.Where(x => x.Name == i).FirstOrDefault();
                    if (billingPeriod == null)
                    {
                        _db.BillingPeriods.Add(new BillingPeriods()
                        {
                            Name = i,
                            Description = i
                        });
                        _db.SaveChanges();
                    }
                }
            }
           
        }

        internal static void DuplicateData()
        {

            int patientsCount = _db.Patients.Count();
            int skip = 0;
            int take = 5;
            try
            {
                if (patientsCount < 90000)
                {

                    for (int i = skip; i <= 90000; i = +skip)
                    {
                        try
                        {
                            var patients = _db.Patients.OrderBy(x => x.Id).Skip(skip).Take(take).ToList();
                            List<Patient> list = new List<Patient>();
                            foreach (var p in patients)
                            {
                                Patient patient = new Patient();
                                patient = p;
                                list.Add(patient);
                            }
                            _db.Patients.AddRange(list);
                            _db.SaveChanges();
                        }
                        catch (Exception e)
                        {
                            throw e;
                        }
                        skip = skip + take;
                        //i = skip;
                        //take = take + take;
                        //patientsCount = _db.Patients.Count();
                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}