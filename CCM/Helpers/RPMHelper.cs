using CCM.Models;
using CCM.Models.DataModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace CCM.Helpers
{
    public static class RPMHelper
    {
        private static readonly ApplicationdbContect _db = new ApplicationdbContect();
        public static RPMService GetRPMServivceById(this int ServiceID)
        {
            return _db.RPMServices.Where(s =>s.Id==ServiceID).FirstOrDefault();
        }

        internal static string GetRPMCycleStatus(int patientId, int Cycle, string Userid, string EnrollmentSubStatus = "")
        {
            using (ApplicationdbContect Db = new ApplicationdbContect())
            {
                try
                {

                    if (Cycle > 0)//means patient is enrolled in any billing category
                    {

                        var reviewtimeccms = Db.ReviewTimeCcms.Where(x => x.PatientId == patientId && x.BillingcategoryId == BillingCodeHelper.RPMBillingCatagoryid).ToList().Where(x => x.StartTime.Month == DateTime.Now.Month && x.StartTime.Year == DateTime.Now.Year).ToList();
                        foreach (var item in reviewtimeccms)
                        {
                            if (item.Cycle != Cycle)
                            {
                                item.Cycle = Cycle;
                                Db.Entry(item).State = EntityState.Modified;
                                Db.SaveChanges();
                            }
                        }
                        //var finalcareplans = Db.FinalCarePlanNotes.Where(x => x.PatientId == patientId).ToList().Where(x => x.CarePlanCreatedOn.Value.Month == DateTime.Now.Month && x.CarePlanCreatedOn.Value.Year == DateTime.Now.Year && x.CarePlanCreatedOn != null).ToList();
                        //foreach (var item in finalcareplans)
                        //{
                        //    if (item.Cycle != Cycle)
                        //    {
                        //        item.Cycle = Cycle;
                        //        Db.Entry(item).State = EntityState.Modified;
                        //        Db.SaveChanges();
                        //    }
                        //}

                    }


                }
                catch (Exception ex)
                {


                }
                var rpmCycleStatus = Db.CategoriesStatuses.Where(x => x.PatientId == patientId && x.Cycle == Cycle && x.BillingCategoryId==BillingCodeHelper.RPMBillingCatagoryid).FirstOrDefault();
                if (rpmCycleStatus == null)
                {

                    CategoriesStatuses RPMCycleStatus = new CategoriesStatuses();
                    RPMCycleStatus.PatientId = patientId;
                    RPMCycleStatus.Cycle = Cycle;
                    RPMCycleStatus.RejectedCount = 0;
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
                    if (EnrollmentSubStatus != "")
                    {
                        if (EnrollmentSubStatus != "Active Enrolled" && Cycle > 0)
                        {
                            RPMCycleStatus.Status = "Expired";
                            RPMCycleStatus.SubStatus = "";
                        }
                    }
                    RPMCycleStatus.CreatedBy = Userid;
                    RPMCycleStatus.CreatedOn = DateTime.Now;
                    Db.CategoriesStatuses.Add(RPMCycleStatus);
                    Db.SaveChanges();
                    return RPMCycleStatus.Status;
                }
                else
                {
                    if (EnrollmentSubStatus != "")
                    {


                        if (EnrollmentSubStatus == "Active Enrolled")
                        {
                            if (rpmCycleStatus.Status != "Claims Submission" && rpmCycleStatus.Status != "Clinical Sign-Off" && rpmCycleStatus.Status != "Ready for Clinical Sign-Off" /*&& rpmCycleStatus.Status != "In Progress"*/)
                            {
                                rpmCycleStatus.Status = "Enrolled";
                                rpmCycleStatus.UpdatedOn = DateTime.Now;
                                rpmCycleStatus.UpdatedBy = "";
                                Db.Entry(rpmCycleStatus).State = EntityState.Modified;
                                Db.SaveChanges();
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
                                Db.Entry(rpmCycleStatus).State = EntityState.Modified;
                                Db.SaveChanges();
                                return "Expired";
                            }
                        }
                        var previouscycles = Db.CategoriesStatuses.Where(x => x.PatientId == patientId && x.Cycle < Cycle && x.Status != "Claims Submission" && x.Status != "Clinical Sign-Off" && x.Status != "Ready for Clinical Sign-Off" /*&& x.CCMStatus != "In Progress"*/).ToList();
                        foreach (var item in previouscycles)
                        {
                            if (item.Status == "Enrolled")
                            {
                                item.Status = "Expired";
                                item.UpdatedOn = DateTime.Now;
                                item.UpdatedBy = "";
                                Db.Entry(item).State = EntityState.Modified;
                                Db.SaveChanges();
                            }
                        }
                    }
                    else
                    {
                        var patientData = Db.Patients.Where(x => x.Id == patientId).FirstOrDefault();
                        if (patientData.EnrollmentSubStatus == "Active Enrolled")
                        {
                            if (rpmCycleStatus.Status != "Claims Submission" && rpmCycleStatus.Status != "Clinical Sign-Off" && rpmCycleStatus.Status != "Ready for Clinical Sign-Off" /*&& rpmCycleStatus.Status != "In Progress"*/)
                            {
                                rpmCycleStatus.Status = "Enrolled";
                                rpmCycleStatus.UpdatedOn = DateTime.Now;
                                rpmCycleStatus.UpdatedBy = "";
                                Db.Entry(rpmCycleStatus).State = EntityState.Modified;
                                Db.SaveChanges();
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
                                Db.Entry(rpmCycleStatus).State = EntityState.Modified;
                                Db.SaveChanges();
                                return "Expired";
                            }
                        }
                        //var previouscycles = Db.CCMCycleStatuses.Where(x => x.PatientId == patientId && x.Cycle < Cycle && x.CCMStatus != "Claims Submission" && x.CCMStatus != "Clinical Sign-Off" && x.CCMStatus != "Ready for Clinical Sign-Off" && x.CCMStatus != "In Progress").ToList();
                        //foreach (var item in previouscycles)
                        //{
                        //    if (item.CCMStatus == "Enrolled")
                        //    {
                        //        item.CCMStatus = "Expired";
                        //        item.UpdatedOn = DateTime.Now;
                        //        item.UpdatedBy = "";
                        //        Db.Entry(item).State = EntityState.Modified;
                        //        Db.SaveChanges();
                        //    }
                        //}
                    }
                    return rpmCycleStatus.Status;
                }


            }
        }

        internal static int GetRPMCycle(int Patientid)
        {
            try
            {

                using (ApplicationdbContect Db = new ApplicationdbContect())
                {
                    var patient = Db.Patients.Where(x => x.Id == Patientid).AsNoTracking().FirstOrDefault();
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
                                return (((DateTime.Now.Year - patient.CCMEnrolledOn.Value.Year) * 12) + DateTime.Now.Month - patient.CCMEnrolledOn.Value.Month) + 1;
                            }
                            else
                            {
                                var lastcycle = Db.CategoriesStatuses.Where(x => x.PatientId == patient.Id).OrderByDescending(o => o.Id).AsNoTracking().FirstOrDefault();
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
    }
}