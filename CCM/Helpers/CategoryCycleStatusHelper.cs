using CCM.Models;
using CCM.Models.DataModels;
using Microsoft.AspNet.Identity;
using System;
using System.Data.Entity;
using System.Linq;
using System.Security.Principal;

namespace CCM.Helpers
{
    public static class CategoryCycleStatusHelper
    {
        private static readonly ApplicationdbContect db = new ApplicationdbContect();
        public static IPrincipal User { get; set; }

        public static int GetPatientNewOrOldCycleByCategory(int patientid, int BillingCatagoryid)
      {
            try
            {

                using (ApplicationdbContect _db = new ApplicationdbContect())
                {

                    var patient = _db.Patients.Where(x => x.Id == patientid).AsNoTracking().FirstOrDefault();

                    if (patient != null)
                    {
                        if (patient.CCMEnrolledOn == null)
                        {
                            return 0;
                        }
                        else
                        {
                            if (patient.EnrollmentSubStatus == "Active Enrolled")
                            {
                                int cycle = (((DateTime.Now.Year - patient.CCMEnrolledOn.Value.Year) * 12) + DateTime.Now.Month - patient.CCMEnrolledOn.Value.Month) + 1;
                                //try
                                //{
                                //    patient.Cycle = cycle;
                                //    _db.Entry(patient).State = EntityState.Modified;
                                //    _db.SaveChanges();
                                //}
                                //catch { }


                                return cycle;
                            }
                            else
                            {
                                if (BillingCatagoryid == BillingCodeHelper.cmmBillingCatagoryid || BillingCatagoryid == 0)
                                {
                                    var lastcycle = _db.CCMCycleStatuses.Where(x => x.PatientId == patient.Id).OrderByDescending(o => o.Id).AsNoTracking().FirstOrDefault();
                                    if (lastcycle != null)
                                    {
                                        return lastcycle.Cycle;
                                    }
                                    else
                                    {
                                        return 0;
                                    }
                                }
                                else
                                {
                                    var lastcycle = _db.CategoriesStatuses.Where(x => x.PatientId == patient.Id && x.BillingCategoryId == BillingCatagoryid).OrderByDescending(o => o.Id).AsNoTracking().FirstOrDefault();
                                    if (lastcycle != null)
                                    {
                                        return lastcycle.Cycle.GetInteger();
                                    }
                                    else
                                    {
                                        return 0;
                                    }
                                }

                            }

                        }
                    }
                    else
                    {
                        return 0;
                    }
                }
            }
            catch (Exception ex)
            {

                return 0;
            }


        }

        public static string GetPatientNewOrOldCycleStatusbyCategory(int patientid, int billingCategoryId, int? cycle, bool UpdatePatientCycle = false)
        {

            using (ApplicationdbContect _db = new ApplicationdbContect())
            {
                if (cycle != null && UpdatePatientCycle)
                {
                    var patient = _db.Patients.Where(x => x.Id == patientid).FirstOrDefault();
                    patient.Cycle = cycle.GetInteger();
                    _db.Entry(patient).State = EntityState.Modified;
                    _db.SaveChanges();
                }

                if (billingCategoryId == BillingCodeHelper.cmmBillingCatagoryid)
                {
                    bool res = UpdatePatientReviewTimeCycleByCategory(patientid, billingCategoryId, cycle);
                    bool res1 = UpdatePatientFinalCarePlaneCycleByCategory(patientid, billingCategoryId, cycle);
                    return GetCCMCycleStatus(patientid, cycle);
                }
                else
                {
                    bool res = UpdatePatientReviewTimeCycleByCategory(patientid, billingCategoryId, cycle);
                    return GetothereCategoryCycleStatus(patientid, billingCategoryId, cycle);
                }
            }
        }


        #region Private Functions

        private static string GetothereCategoryCycleStatus(int patientid, int billingCategoryId, int? cycle)
        {
            string status = "";
            using (var _db = new ApplicationdbContect())
            {
                if (cycle == null)
                {
                    var patient = _db.Patients.Where(x => x.Id == patientid).AsNoTracking().FirstOrDefault();
                    if (patient != null)
                    {
                        cycle = patient.Cycle;
                    }
                }
                var billingCategory = _db.BillingCategories.Where(x => x.BillingCategoryId == billingCategoryId).AsNoTracking().FirstOrDefault();
                //for monthly billing cycle  
                if (billingCategory.BillingPeriodsId == BillingCodeHelper.MonthalyPeriodId)
                {
                    bool res = UpdatePatientReviewTimeCycleByCategory(patientid, billingCategoryId,cycle);
                    status = GetMonthlyCycleStatusforCategory(patientid, billingCategoryId,cycle);
                }
                //for one time
                if (billingCategory.BillingPeriodsId == BillingCodeHelper.OneTimePeriodId)
                {
                    bool res = UpdatePatientReviewTimeCycleByCategory(patientid, billingCategoryId,cycle);
                    status = GetOneTimeCycleStatusforCategory(patientid, billingCategoryId,cycle);

                }
            }
            return status;
        }

        private static string GetOneTimeCycleStatusforCategory(int patientid, int billingCategoryId, int? cycle)
        {
            string status = "";
            using (var _db = new ApplicationdbContect())
            {
                var patient = _db.Patients.Where(x => x.Id == patientid).AsNoTracking().FirstOrDefault();
                if (cycle == null)
                {

                    if (patient != null)
                    {
                        cycle = patient.Cycle;
                    }
                }
                int Cycle = cycle.GetInteger();
                if (patient != null)
                {
                    var categorystatus = _db.CategoriesStatuses.Where(x => x.PatientId == patientid && x.BillingCategoryId == billingCategoryId).OrderByDescending(x => x.Id).FirstOrDefault();
                    if (categorystatus == null)
                    {
                        CategoriesStatuses cateCycleStatus = new CategoriesStatuses();
                        cateCycleStatus.PatientId = patientid;
                        cateCycleStatus.Cycle = Cycle;
                        cateCycleStatus.RejectedCount = 0;
                        cateCycleStatus.BillingCategoryId = billingCategoryId;

                        if (Cycle == 0)
                        {
                            cateCycleStatus.Status = "In Progress";
                            cateCycleStatus.SubStatus = "";
                        }
                        else
                        {
                            cateCycleStatus.Status = "Enrolled";
                            cateCycleStatus.SubStatus = "";
                        }
                        if (patient.EnrollmentSubStatus != "")
                        {
                            if (patient.EnrollmentSubStatus != "Active Enrolled")
                            {
                                cateCycleStatus.Status = "Expired";
                                cateCycleStatus.SubStatus = "";
                            }
                        }
                        cateCycleStatus.CreatedBy = User.Identity.GetUserId();
                        cateCycleStatus.CreatedOn = DateTime.Now;
                        _db.CategoriesStatuses.Add(cateCycleStatus);
                        _db.SaveChanges();
                        status = cateCycleStatus.Status;
                    }
                    else
                    {
                        status = categorystatus.Status;
                    }
                }

                return status;
            }
        }

        private static string GetMonthlyCycleStatusforCategory(int patientId, int billingCategoryId, int? cycle)
        {
            using (var _db = new ApplicationdbContect())
            {
                var patient = _db.Patients.Where(x => x.Id == patientId).AsNoTracking().FirstOrDefault();
                if (cycle == null)
                {
                    
                    if (patient != null)
                    {
                        cycle = patient.Cycle;
                    }
                }
               
                int Cycle = cycle.GetInteger();
                var rpmCycleStatus = _db.CategoriesStatuses.Where(x => x.PatientId == patientId && x.Cycle == Cycle && x.BillingCategoryId == billingCategoryId).FirstOrDefault();
                if (rpmCycleStatus == null)
                {

                    CategoriesStatuses RPMCycleStatus = new CategoriesStatuses();
                    RPMCycleStatus.PatientId = patientId;
                    RPMCycleStatus.Cycle = Cycle;
                    RPMCycleStatus.RejectedCount = 0;
                    RPMCycleStatus.BillingCategoryId = billingCategoryId;

                    if (Cycle == 0)
                    {
                        RPMCycleStatus.Status = "In Progress";
                        RPMCycleStatus.SubStatus = "";
                    }
                    else
                    {
                        RPMCycleStatus.Status = "Enrolled";
                        RPMCycleStatus.SubStatus = "";
                    }
                    if (patient.EnrollmentSubStatus != "")
                    {
                        if (patient.EnrollmentSubStatus != "Active Enrolled" && Cycle > 0)
                        {
                            RPMCycleStatus.Status = "Expired";
                            RPMCycleStatus.SubStatus = "";
                        }
                    }
                    RPMCycleStatus.CreatedBy = User.Identity.GetUserId();
                    RPMCycleStatus.CreatedOn = DateTime.Now;
                    _db.CategoriesStatuses.Add(RPMCycleStatus);
                    _db.SaveChanges();
                    return RPMCycleStatus.Status;
                }
                else
                {
                    if (!string.IsNullOrEmpty(patient.EnrollmentSubStatus))
                    {


                        if (patient.EnrollmentSubStatus == "Active Enrolled")
                        {
                            if (rpmCycleStatus.Status != "Claims Submission" && rpmCycleStatus.Status != "Clinical Sign-Off" && rpmCycleStatus.Status != "Ready for Clinical Sign-Off" /*&& rpmCycleStatus.Status != "In Progress"*/)
                            {
                                rpmCycleStatus.Status = "Enrolled";
                                rpmCycleStatus.UpdatedOn = DateTime.Now;
                                rpmCycleStatus.UpdatedBy = "";
                                _db.Entry(rpmCycleStatus).State = EntityState.Modified;
                                _db.SaveChanges();
                                return "Enrolled";
                            }

                        }
                        else
                        {

                            if (rpmCycleStatus.Status != "Claims Submission" && rpmCycleStatus.Status != "Clinical Sign-Off" && rpmCycleStatus.Status != "Ready for Clinical Sign-Off" && rpmCycleStatus.Status != "In Progress")
                            {
                                rpmCycleStatus.Status = "Expired";
                                rpmCycleStatus.UpdatedOn = DateTime.Now;
                                rpmCycleStatus.UpdatedBy = "";
                                _db.Entry(rpmCycleStatus).State = EntityState.Modified;
                                _db.SaveChanges();
                                return "Expired";
                            }
                        }
                        var previouscycles = _db.CategoriesStatuses.Where(x => x.PatientId == patientId && x.Cycle < Cycle && x.Status != "Claims Submission" && x.Status != "Clinical Sign-Off" && x.Status != "Ready for Clinical Sign-Off" /*&& x.Status != "In Progress"*/).ToList();
                        foreach (var item in previouscycles)
                        {
                            if (item.Status == "Enrolled")
                            {
                                item.Status = "Expired";
                                item.UpdatedOn = DateTime.Now;
                                item.UpdatedBy = "";
                                _db.Entry(item).State = EntityState.Modified;
                                _db.SaveChanges();
                            }
                        }
                    }
                    else
                    {

                        if (patient.EnrollmentSubStatus == "Active Enrolled")
                        {
                            if (rpmCycleStatus.Status != "Claims Submission" && rpmCycleStatus.Status != "Clinical Sign-Off" && rpmCycleStatus.Status != "Ready for Clinical Sign-Off" /*&& rpmCycleStatus.Status != "In Progress"*/)
                            {
                                rpmCycleStatus.Status = "Enrolled";
                                rpmCycleStatus.UpdatedOn = DateTime.Now;
                                rpmCycleStatus.UpdatedBy = "";
                                _db.Entry(rpmCycleStatus).State = EntityState.Modified;
                                _db.SaveChanges();
                                return "Enrolled";
                            }

                        }
                        else
                        {

                            if (rpmCycleStatus.Status != "Claims Submission" && rpmCycleStatus.Status != "Clinical Sign-Off" && rpmCycleStatus.Status != "Ready for Clinical Sign-Off" /*&& rpmCycleStatus.Status != "In Progress"*/)
                            {
                                rpmCycleStatus.Status = "Expired";
                                rpmCycleStatus.UpdatedOn = DateTime.Now;
                                rpmCycleStatus.UpdatedBy = "";
                                _db.Entry(rpmCycleStatus).State = EntityState.Modified;
                                _db.SaveChanges();
                                return "Expired";
                            }
                        }
                        var previouscycles = _db.CategoriesStatuses.Where(x => x.PatientId == patientId && x.BillingCategoryId == billingCategoryId && x.Cycle < Cycle && x.Status != "Claims Submission" && x.Status != "Clinical Sign-Off" && x.Status != "Ready for Clinical Sign-Off" && x.Status != "In Progress").ToList();
                        foreach (var item in previouscycles)
                        {
                            if (item.Status == "Enrolled")
                            {
                                item.Status = "Expired";
                                item.UpdatedOn = DateTime.Now;
                                item.UpdatedBy = "";
                                _db.Entry(item).State = EntityState.Modified;
                                _db.SaveChanges();
                            }
                        }
                    }
                    return rpmCycleStatus.Status;
                }
            }
        }

        private static string GetCCMCycleStatus(int patientId, int? cycle)
        {
            using (var _db = new ApplicationdbContect())
            {
                var patient = _db.Patients.Where(x => x.Id == patientId).AsNoTracking().FirstOrDefault();
                if (cycle == null)
                {
                   
                    if (patient != null)
                    {
                        cycle = patient.Cycle;
                    }
                }
              
                int Cycle = cycle.GetInteger();
                var CCMCycleStatus = _db.CCMCycleStatuses.Where(x => x.PatientId == patientId && x.Cycle == Cycle).FirstOrDefault();
                if (CCMCycleStatus == null)
                {

                    CCMCycleStatus cCMCycleStatus = new CCMCycleStatus();
                    cCMCycleStatus.PatientId = patientId;
                    cCMCycleStatus.Cycle = Cycle;
                    cCMCycleStatus.RejectedCount = 0;
                    if (Cycle == 0)
                    {
                        cCMCycleStatus.CCMStatus = "In Progress";
                        cCMCycleStatus.CCMSubStatus = "";
                    }
                    else
                    {
                        cCMCycleStatus.CCMStatus = "Enrolled";
                        cCMCycleStatus.CCMSubStatus = "";
                    }
                    if (!string.IsNullOrEmpty(patient.EnrollmentSubStatus))
                    {
                        if (patient.EnrollmentSubStatus != "Active Enrolled" && Cycle > 0)
                        {
                            cCMCycleStatus.CCMStatus = "Expired";
                            cCMCycleStatus.CCMSubStatus = "";
                        }
                    }
                    cCMCycleStatus.CreatedBy = User.Identity.GetUserId();
                    cCMCycleStatus.CreatedOn = DateTime.Now;
                    _db.CCMCycleStatuses.Add(cCMCycleStatus);
                    _db.SaveChanges();
                    return cCMCycleStatus.CCMStatus;
                }
                else
                {
                    if (!string.IsNullOrEmpty(patient.EnrollmentSubStatus))
                    {


                        if (patient.EnrollmentSubStatus == "Active Enrolled")
                        {
                            if (CCMCycleStatus.CCMStatus != "Claims Submission" && CCMCycleStatus.CCMStatus != "Clinical Sign-Off" && CCMCycleStatus.CCMStatus != "Ready for Clinical Sign-Off" /*&& CCMCycleStatus.CCMStatus != "In Progress"*/)
                            {
                                CCMCycleStatus.CCMStatus = "Enrolled";
                                CCMCycleStatus.UpdatedOn = DateTime.Now;
                                CCMCycleStatus.UpdatedBy = "";
                                _db.Entry(CCMCycleStatus).State = EntityState.Modified;
                                _db.SaveChanges();
                                return "Enrolled";
                            }

                        }
                        else
                        {

                            if (CCMCycleStatus.CCMStatus != "Claims Submission" && CCMCycleStatus.CCMStatus != "Clinical Sign-Off" && CCMCycleStatus.CCMStatus != "Ready for Clinical Sign-Off" && CCMCycleStatus.CCMStatus != "In Progress")
                            {
                                CCMCycleStatus.CCMStatus = "Expired";
                                CCMCycleStatus.UpdatedOn = DateTime.Now;
                                CCMCycleStatus.UpdatedBy = "";
                                _db.Entry(CCMCycleStatus).State = EntityState.Modified;
                                _db.SaveChanges();
                                return "Expired";
                            }
                        }
                        var previouscycles = _db.CCMCycleStatuses.Where(x => x.PatientId == patientId && x.Cycle < Cycle && x.CCMStatus != "Claims Submission" && x.CCMStatus != "Clinical Sign-Off" && x.CCMStatus != "Ready for Clinical Sign-Off" && x.CCMStatus != "In Progress").ToList();
                        foreach (var item in previouscycles)
                        {
                            if (item.CCMStatus == "Enrolled")
                            {
                                item.CCMStatus = "Expired";
                                item.UpdatedOn = DateTime.Now;
                                item.UpdatedBy = "";
                                _db.Entry(item).State = EntityState.Modified;
                                _db.SaveChanges();
                            }
                        }
                    }
                    else
                    {

                        if (patient.EnrollmentSubStatus == "Active Enrolled")
                        {
                            if (CCMCycleStatus.CCMStatus != "Claims Submission" && CCMCycleStatus.CCMStatus != "Clinical Sign-Off" && CCMCycleStatus.CCMStatus != "Ready for Clinical Sign-Off" /*&& CCMCycleStatus.CCMStatus != "In Progress"*/)
                            {
                                CCMCycleStatus.CCMStatus = "Enrolled";
                                CCMCycleStatus.UpdatedOn = DateTime.Now;
                                CCMCycleStatus.UpdatedBy = "";
                                _db.Entry(CCMCycleStatus).State = EntityState.Modified;
                                _db.SaveChanges();
                                return "Enrolled";
                            }

                        }
                        else
                        {

                            if (CCMCycleStatus.CCMStatus != "Claims Submission" && CCMCycleStatus.CCMStatus != "Clinical Sign-Off" && CCMCycleStatus.CCMStatus != "Ready for Clinical Sign-Off" && CCMCycleStatus.CCMStatus != "In Progress")
                            {
                                CCMCycleStatus.CCMStatus = "Expired";
                                CCMCycleStatus.UpdatedOn = DateTime.Now;
                                CCMCycleStatus.UpdatedBy = "";
                                _db.Entry(CCMCycleStatus).State = EntityState.Modified;
                                _db.SaveChanges();
                                return "Expired";
                            }
                        }
                        var previouscycles = _db.CCMCycleStatuses.Where(x => x.PatientId == patientId && x.Cycle < Cycle && x.CCMStatus != "Claims Submission" && x.CCMStatus != "Clinical Sign-Off" && x.CCMStatus != "Ready for Clinical Sign-Off" && x.CCMStatus != "In Progress").ToList();
                        foreach (var item in previouscycles)
                        {
                            if (item.CCMStatus == "Enrolled")
                            {
                                item.CCMStatus = "Expired";
                                item.UpdatedOn = DateTime.Now;
                                item.UpdatedBy = "";
                                _db.Entry(item).State = EntityState.Modified;
                                _db.SaveChanges();
                            }
                        }

                    }
                    return CCMCycleStatus.CCMStatus;
                }
            }


        }

        private static bool UpdatePatientFinalCarePlaneCycleByCategory(int patientid, int billingCategoryId, int? cycle)
        {
            using (var _db = new ApplicationdbContect())
            {
                if (cycle == null)
                {
                    var patient = _db.Patients.Where(x => x.Id == patientid).AsNoTracking().FirstOrDefault();
                    if (patient != null)
                    {
                        cycle = patient.Cycle;
                    }
                }

                if (cycle > 0)
                {
                    var finalcareplans = _db.FinalCarePlanNotes.Where(x => x.PatientId == patientid).ToList().Where(x => x.CarePlanCreatedOn.Value.Month == DateTime.Now.Month && x.CarePlanCreatedOn.Value.Year == DateTime.Now.Year && x.CarePlanCreatedOn != null).ToList();
                    foreach (var item in finalcareplans)
                    {
                        if (item.Cycle != cycle.GetInteger())
                        {
                            item.Cycle = cycle.GetInteger();
                            _db.Entry(item).State = EntityState.Modified;
                            _db.SaveChanges();
                        }
                    }
                    return true;
                }
                return false;

            }
        }

        private static bool UpdatePatientReviewTimeCycleByCategory(int patientid, int billingCategoryId, int? cycle)
        {
            using (var _db = new ApplicationdbContect())
            {

                if (cycle == null)
                {
                    var patient = _db.Patients.Where(x => x.Id == patientid).AsNoTracking().FirstOrDefault();
                    if (patient != null)
                    {
                        cycle = patient.Cycle;
                    }
                }
                
                if (cycle > 0)
                {
                    var reviewtimeccms = _db.ReviewTimeCcms.Where(x => x.PatientId == patientid && x.BillingcategoryId == billingCategoryId).ToList().Where(x => x.StartTime.Month == DateTime.Now.Month && x.StartTime.Year == DateTime.Now.Year).ToList();
                    foreach (var item in reviewtimeccms)
                    {
                        if (item.Cycle != cycle.GetInteger())
                        {
                            item.Cycle =cycle.GetInteger();
                            _db.Entry(item).State = EntityState.Modified;
                            _db.SaveChanges();
                        }
                    }
                    return true;
                }
                return false;
            }
        }
        #endregion
    }
}