using CCM.Helpers;
using CCM.Models.DataModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace CCM.Models.RPM
{
    public class RPMModel
    {
        internal static void UpdateRPMCycles(Patient currentPatient)
        {
            
            var cycle = RPMHelper.GetRPMCycle(currentPatient.Id);
            var s = GetUpdateRPMCycleStatus(currentPatient.Id, cycle, "", currentPatient.EnrollmentSubStatus);
        }

        private static object GetUpdateRPMCycleStatus(int patientId, int Cycle, string Userid, string EnrollmentSubStatus)
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
                var RMPCyclesStatus = Db.CategoriesStatuses.Where(x => x.PatientId == patientId && x.Cycle == Cycle && x.BillingCategoryId==BillingCodeHelper.RPMBillingCatagoryid).FirstOrDefault();
                if (RMPCyclesStatus == null)
                {
                    CategoriesStatuses rpmStatuses = new CategoriesStatuses();
                    rpmStatuses.PatientId = patientId;
                    rpmStatuses.Cycle = Cycle;
                    rpmStatuses.RejectedCount = 0;
                   
                    if (Cycle == 0)
                    {
                        rpmStatuses.Status = "In Progress";
                        rpmStatuses.SubStatus = "";
                    }
                    else
                    {
                        rpmStatuses.Status = "Enrolled";
                        rpmStatuses.SubStatus = "";
                    }
                    if (EnrollmentSubStatus != "")
                    {
                        if (EnrollmentSubStatus != "Active Enrolled" && Cycle > 0)
                        {
                            rpmStatuses.Status = "Expired";
                            rpmStatuses.SubStatus = "";
                        }
                    }
                    rpmStatuses.CreatedBy = Userid;
                    rpmStatuses.CreatedOn = DateTime.Now;
                    Db.CategoriesStatuses.Add(rpmStatuses);
                    Db.SaveChanges();
                    return rpmStatuses.Status;
                }
                else
                {
                    if (EnrollmentSubStatus != "")
                    {


                        if (EnrollmentSubStatus == "Active Enrolled")
                        {
                            if (RMPCyclesStatus.Status != "Claims Submission" && RMPCyclesStatus.Status != "Clinical Sign-Off" && RMPCyclesStatus.Status != "Ready for Clinical Sign-Off" /*&& RMPCyclesStatus.Status != "In Progress"*/)
                            {
                                RMPCyclesStatus.Status = "Enrolled";
                                RMPCyclesStatus.UpdatedOn = DateTime.Now;
                                RMPCyclesStatus.UpdatedBy = "";
                                Db.Entry(RMPCyclesStatus).State = EntityState.Modified;
                                Db.SaveChanges();
                                return "Enrolled";
                            }

                        }
                        else
                        {

                            if (RMPCyclesStatus.Status != "Claims Submission" && RMPCyclesStatus.Status != "Clinical Sign-Off" && RMPCyclesStatus.Status != "Ready for Clinical Sign-Off" /*&& RMPCyclesStatus.Status != "In Progress"*/)
                            {
                                RMPCyclesStatus.Status = "Expired";
                                RMPCyclesStatus.UpdatedOn = DateTime.Now;
                                RMPCyclesStatus.UpdatedBy = "";
                                Db.Entry(RMPCyclesStatus).State = EntityState.Modified;
                                Db.SaveChanges();
                                return "Expired";
                            }
                        }
                        var previouscycles = Db.CategoriesStatuses.Where(x => x.PatientId == patientId && x.BillingCategoryId==BillingCodeHelper.RPMBillingCatagoryid && x.Cycle < Cycle && x.Status != "Claims Submission" && x.Status != "Clinical Sign-Off" && x.Status != "Ready for Clinical Sign-Off" /*&& x.Status != "In Progress"*/).ToList();
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
                            if (RMPCyclesStatus.Status != "Claims Submission" && RMPCyclesStatus.Status != "Clinical Sign-Off" && RMPCyclesStatus.Status != "Ready for Clinical Sign-Off" /*&& RMPCyclesStatus.Status != "In Progress"*/)
                            {
                                RMPCyclesStatus.Status = "Enrolled";
                                RMPCyclesStatus.UpdatedOn = DateTime.Now;
                                RMPCyclesStatus.UpdatedBy = "";
                                Db.Entry(RMPCyclesStatus).State = EntityState.Modified;
                                Db.SaveChanges();
                                return "Enrolled";
                            }

                        }
                        else
                        {

                            if (RMPCyclesStatus.Status != "Claims Submission" && RMPCyclesStatus.Status != "Clinical Sign-Off" && RMPCyclesStatus.Status != "Ready for Clinical Sign-Off" /*&& RMPCyclesStatus.Status != "In Progress"*/)
                            {
                                RMPCyclesStatus.Status = "Expired";
                                RMPCyclesStatus.UpdatedOn = DateTime.Now;
                                RMPCyclesStatus.UpdatedBy = "";
                                Db.Entry(RMPCyclesStatus).State = EntityState.Modified;
                                Db.SaveChanges();
                                return "Expired";
                            }
                        }
                        //var previouscycles = Db.CategoriesStatuses.Where(x => x.PatientId == patientId && x.Cycle < Cycle && x.Status != "Claims Submission" && x.Status != "Clinical Sign-Off" && x.CCMStatus != "Ready for Clinical Sign-Off" && x.CCMStatus != "In Progress").ToList();
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
                    return RMPCyclesStatus.Status;
                }


            }
        }
    }
}