using CCM.Helpers;
using CCM.Models.RPM;
using System;
using System.Linq;

namespace CCM.Models.BackGroundJob
{
    public static class CatagoryCyclesStatusUpdate
    {
        internal static void UpdateCategoryCycle()
        {
            bool CycleEntryForCurrentMonth = HelperExtensions.CycleEntryForCurrentMonth();
            if (CycleEntryForCurrentMonth)
            {
                using (var _db = new ApplicationdbContect())
                {
                    try
                    {
                        var patients = _db.Patients.AsNoTracking().AsQueryable();
                        foreach (var currentPatient in patients)
                        {
                            try
                            {
                                var MonthlyBillingCategory = currentPatient.Patients_BillingCategories.Where(x => x.Status = true && x.BillingCategory.BillingPeriods.BillingPeriodsId == BillingPeriodsHelper.MonthlyPeriod_ID).ToList();
                                MonthlyBillingCategory.ForEach(x =>
                                {
                                    UpdatePatientCycleAndCycleStatus(currentPatient.Id, x.Id);
                                });
                            }
                            catch (Exception e)
                            {
                                HelperExtensions.WriteErrorLog(e);
                            }
                            
                        }
                        _db.automaticCycleStatusEntries.Add(new AutomaticCycleStatusEntry { EntryMonth = DateTime.Now.Month, EntryYear = DateTime.Now.Year });
                        _db.SaveChanges();
                    }
                    catch (Exception ex)
                    {

                        HelperExtensions.WriteErrorLog(ex);
                    }
                }
            }

        }

        private static void UpdatePatientCycleAndCycleStatus(int patientId, int BillingCategoryId)
        {
            int cycle = CategoryCycleStatusHelper.GetPatientNewOrOldCycleByCategory(patientId, BillingCategoryId);
            string cyclestatus = CategoryCycleStatusHelper.GetPatientNewOrOldCycleStatusbyCategory(patientId, BillingCategoryId, cycle,true);
        }
    }
}