using CCM.Helpers;
using CCM.Models;
using CCM.Models.DataModels;
using Microsoft.AspNet.Identity;
using System;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;


namespace CCM.Controllers
{
    [Authorize]
    [RequireHttps]
    public class CcmFlowController : BaseController
    {
        //private readonly ApplicationdbContect _db = new ApplicationdbContect();
        //private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);


        [Authorize(Roles = "Liaison, Admin")]
        public bool AssignCycleStatusBulk(string Status, int[] Patients, int[] Cycles, string Notes)
        {
            try
            {
                if (Patients.Count() > 0)
                {
                    for (int i = 0; i <= Patients.Count() - 1; i++)
                    {
                        int patientid = Patients[i];
                        int Cycle = Cycles[i];
                        var cCMCycleStatus = _db.CCMCycleStatuses.AsNoTracking().Where(x => x.PatientId == patientid && x.Cycle == Cycle).FirstOrDefault();
                        cCMCycleStatus.CCMStatus = Status;
                        cCMCycleStatus.CCMNotes = Notes;
                        var CCMStatus = Status;
                        var Userid = User.Identity.GetUserId();
                        if (Status == "Clinical Sign-Off" || Status == "Enrolled")
                        {

                            var billingcycle = _db.BillingCycles.Where(x => x.PatientId == patientid && x.Cycle == Cycle).FirstOrDefault();
                            if (billingcycle != null)
                            {
                                _db.BillingCycles.Remove(billingcycle);
                            }

                        }

                        if (CCMStatus == "Clinical Sign-Off")
                        {
                            cCMCycleStatus.CcmClinicalSignOffDate = DateTime.Now;
                        }
                        cCMCycleStatus.UpdatedOn = DateTime.Now;
                        cCMCycleStatus.UpdatedBy = User.Identity.GetUserId();
                        _db.Entry(cCMCycleStatus).State = EntityState.Modified;

                    }
                    _db.SaveChanges();
                }

            }
            catch (Exception ex)
            {
                log.Error(Environment.NewLine + User.Identity.GetUserName() + "-------" + User.Identity.GetUserId() + Environment.NewLine + ex.Message + "-----" + ex.StackTrace);

                return false;
            }
            return true;
        }
        [Authorize(Roles = "Liaison, Admin")]
        public ActionResult ClinicalSignOff(int? patientId, int BillingcategoryId = 0)
        {
            try
            {
                var patient = _db.Patients.Where(x => x.Id == patientId).FirstOrDefault();
                var categorystatus = _db.CategoriesStatuses.Where(p => p.BillingCategoryId == BillingcategoryId && p.PatientId == patientId).FirstOrDefault();
                if (BillingcategoryId == BillingCodeHelper.cmmBillingCatagoryid)
                {
                    CategoryCycleStatusHelper.User = User;
                    var patientccmcycle = CategoryCycleStatusHelper.GetPatientNewOrOldCycleByCategory(patient.Id, BillingCodeHelper.cmmBillingCatagoryid);
                    var patientccmcyclestatus = CategoryCycleStatusHelper.GetPatientNewOrOldCycleStatusbyCategory(patient.Id, BillingCodeHelper.cmmBillingCatagoryid, patientccmcycle);

                    if (patient != null)
                    {
                        var totaltimespent = TotalReviewTime(patient.Id, patientccmcycle);
                        //if (totaltimespent.TotalMinutes < 20)
                        //{
                        //    return RedirectToAction("Details", "Patient", new { id = patient.Id, status = "Time is less than 20 minutes." });
                        //}
                        var finalcareplan = _db.FinalCarePlanNotes.Where(x => x.PatientId == patientId && x.Cycle == patientccmcycle).FirstOrDefault();
                        if (finalcareplan == null)
                        {
                            var finalcareplans = _db.FinalCarePlanNotes.Where(x => x.PatientId == patientId).OrderByDescending(x => x.CarePlanCreatedOn).ToList();
                            if (finalcareplans.Count > 0)
                            {
                                var lastcareplan = finalcareplans[0];
                                if (lastcareplan.CarePlanCreatedOn.Value.Month == DateTime.Now.Month && lastcareplan.CarePlanCreatedOn.Value.Year == DateTime.Now.Year)

                                {
                                    lastcareplan.Cycle = patientccmcycle;
                                    _db.Entry(lastcareplan).State = EntityState.Modified;
                                    _db.SaveChanges();
                                }
                                else
                                {
                                    return RedirectToAction("Details", "Patient", new { id = patient.Id, status = "Cannot submit this cycle for billing untill final care plan not generated." }); ;
                                }
                            }
                            else
                            {
                                return RedirectToAction("Details", "Patient", new { id = patient.Id, status = "Cannot submit this cycle for billing untill final care plan not generated." });
                            }


                        }
                        if (patient.EnrollmentSubStatus != "Active Enrolled")
                        {
                            return RedirectToAction("Details", "Patient", new { id = patient.Id, status = "Patient is not Active Enrolled." });
                        }
                        if ((patientccmcyclestatus == "Enrolled" || patientccmcyclestatus == "Ready for Clinical Sign-Off"))
                        {
                            HelperExtensions.UpdateCCMCycleStatus(patient.Id, patientccmcycle, "Clinical Sign-Off", User.Identity.GetUserId(), "");
                        }
                        else
                        {
                            return RedirectToAction("Details", "Patient", new { id = patient.Id, status = "Cycle is locked." });
                        }

                        patient.CcmStatus = "Clinical Sign-Off";
                        patient.CcmClinicalSignOffDate = DateTime.Now;
                        patient.UpdatedOn = DateTime.Now;
                        patient.UpdatedBy = User.Identity.GetUserId();


                        var ccmCycleStatusObj = _db.CCMCycleStatuses.Where(x => (x.PatientId == patientId && EntityFunctions.TruncateTime(x.CreatedOn).Value.Month == DateTime.Now.Month)).FirstOrDefault();
                        if (ccmCycleStatusObj != null)
                        {
                            //ccmCycleStatusObj.clinicalSignOffQueCounter += 1;
                        }
                        _db.Entry(ccmCycleStatusObj).State = EntityState.Modified;
                        _db.Entry(patient).State = EntityState.Modified;
                        _db.SaveChanges();
                    }

                    return RedirectToAction("Index", "CcmStatus", new { status = "Clinical Sign-Off" });
                }
                else
                {
                    CategoryCycleStatusHelper.User = User;
                    var patientccmcycle = CategoryCycleStatusHelper.GetPatientNewOrOldCycleByCategory(patient.Id, BillingcategoryId);
                    var patientccmcyclestatus = CategoryCycleStatusHelper.GetPatientNewOrOldCycleStatusbyCategory(patient.Id,BillingcategoryId, patientccmcycle);

                    if (patient.EnrollmentSubStatus != "Active Enrolled")
                    {
                        return RedirectToAction("Details", "Patient", new { id = patient.Id, status = "Patient is not Active Enrolled." });
                    }
                    if ((patientccmcyclestatus == "Enrolled" || patientccmcyclestatus == "Ready for Clinical Sign-Off"))
                    {



                        HelperExtensions.UpdatecategoryStatus(BillingcategoryId, patient.Id, patientccmcycle, "Clinical Sign-Off", User.Identity.GetUserId(), "");
                    }
                    else
                    {
                        return RedirectToAction("Details", "Patient", new { id = patient.Id, status = "Cycle is locked." });
                    }

                    return RedirectToAction("Index", "CcmStatus", new { status = "Clinical Sign-Off", BillingcategoryId = BillingcategoryId });
                }


            }
            catch (Exception ex)
            {

                log.Error(Environment.NewLine + User.Identity.GetUserName() + "-------" + User.Identity.GetUserId() + Environment.NewLine + ex.Message + "-----" + ex.StackTrace);
                return RedirectToAction("Index", "CcmStatus", new { status = "Clinical Sign-Off" });
                /*return ex.Message + "------------------" + ex.StackTrace;*/
            }
        }

        [Authorize(Roles = "Liaison, Admin")]
        public ActionResult ReadyClinicalSignOff(int? patientId, int? BillingcategoryId)
        {
            try
            {


                var patient = _db.Patients.Where(x => x.Id == patientId).FirstOrDefault();
                if (patient != null)
                {

                    if (BillingcategoryId == BillingCodeHelper.cmmBillingCatagoryid)
                    {
                        CategoryCycleStatusHelper.User = User;
                        var patientccmcycle = CategoryCycleStatusHelper.GetPatientNewOrOldCycleByCategory(patient.Id, BillingcategoryId.GetInteger());
                        var patientccmcyclestatus = CategoryCycleStatusHelper.GetPatientNewOrOldCycleStatusbyCategory(patient.Id, BillingcategoryId.GetInteger(), patientccmcycle);


                        //var totaltimespent = TotalReviewTime(patient.Id, patientccmcycle);
                        //if (totaltimespent.TotalMinutes < 20)
                        //{
                        //    return RedirectToAction("Details", "Patient", new { id = patient.Id, status = "Time is less than 20 minutes." });
                        //}
                        //var finalcareplan = _db.FinalCarePlanNotes.Where(x => x.PatientId == patientId && x.Cycle == patientccmcycle).FirstOrDefault();
                        //if (finalcareplan == null)
                        //{
                        //    var finalcareplans = _db.FinalCarePlanNotes.Where(x => x.PatientId == patientId).OrderByDescending(x => x.CarePlanCreatedOn).ToList();
                        //    if (finalcareplans.Count > 0)
                        //    {
                        //        var lastcareplan = finalcareplans[0];
                        //        if (lastcareplan.CarePlanCreatedOn.Value.Month == DateTime.Now.Month && lastcareplan.CarePlanCreatedOn.Value.Year == DateTime.Now.Year)

                        //        {
                        //            lastcareplan.Cycle = patientccmcycle;
                        //            _db.Entry(lastcareplan).State = EntityState.Modified;
                        //            _db.SaveChanges();
                        //        }
                        //        else
                        //        {
                        //            return RedirectToAction("Details", "Patient", new { id = patient.Id, status = "Cannot submit this cycle for billing untill final care plan not generated." }); ;
                        //        }
                        //    }
                        //    else
                        //    {
                        //        return RedirectToAction("Details", "Patient", new { id = patient.Id, status = "Cannot submit this cycle for billing untill final care plan not generated." });
                        //    }


                        //}
                        if (patient.EnrollmentSubStatus != "Active Enrolled")
                        {
                            return RedirectToAction("Details", "Patient", new { id = patient.Id, status = "Patient is not Active Enrolled." });
                        }
                        if (patientccmcyclestatus == "Enrolled")
                        {
                            var liasionid = _db.Users.AsNoTracking().Where(x => x.CCMid == patient.LiaisonId && x.Role == "Liaison").FirstOrDefault()?.Id;
                            HelperExtensions.UpdateCCMCycleStatus(patient.Id, patientccmcycle, "Ready for Clinical Sign-Off", User.Identity.GetUserId(), "", false, liasionid);
                        }
                        else
                        {
                            return RedirectToAction("Details", "Patient", new { id = patient.Id, status = "Cycle is locked." });
                        }

                        patient.CcmStatus = "Ready for Clinical Sign-Off";
                        patient.CcmClinicalSignOffDate = DateTime.Now;
                        patient.UpdatedOn = DateTime.Now;
                        patient.UpdatedBy = User.Identity.GetUserId(); ;
                        _db.Entry(patient).State = EntityState.Modified;
                        _db.SaveChanges();
                    }
                    else
                    {
                        CategoryCycleStatusHelper.User = User;
                        var patientccmcycle = CategoryCycleStatusHelper.GetPatientNewOrOldCycleByCategory(patient.Id,BillingcategoryId.GetInteger());
                        var patientccmcyclestatus = CategoryCycleStatusHelper.GetPatientNewOrOldCycleStatusbyCategory(patient.Id,BillingcategoryId.GetInteger(), patientccmcycle);

                        if (patient.EnrollmentSubStatus != "Active Enrolled")
                        {
                            return RedirectToAction("Details", "Patient", new { id = patient.Id, status = "Patient is not Active Enrolled." });
                        }
                        if (patientccmcyclestatus == "Enrolled")
                        {
                            var liasionid = _db.Users.AsNoTracking().Where(x => x.CCMid == patient.LiaisonId && x.Role == "Liaison").FirstOrDefault()?.Id;

                            HelperExtensions.UpdatecategoryStatus(BillingcategoryId, patient.Id, patientccmcycle, "Ready for Clinical Sign-Off", User.Identity.GetUserId(), "", false, liasionid);


                        }
                        else
                        {
                            return RedirectToAction("Details", "Patient", new { id = patient.Id, status = "Cycle is locked." });
                        }
                    }
                }

                return RedirectToAction("Index", "CcmStatus", new { status = "Enrolled", BillingcategoryId = BillingcategoryId });
            }
            catch (Exception ex)
            {

                log.Error(Environment.NewLine + User.Identity.GetUserName() + "-------" + User.Identity.GetUserId() + Environment.NewLine + ex.Message + "-----" + ex.StackTrace);
                return RedirectToAction("Index", "CcmStatus", new { status = "Enrolled" });
                /*return ex.Message + "------------------" + ex.StackTrace;*/
            }
        }
        [Authorize(Roles = "Admin, Liaison")]
        public ActionResult BacktoProgessbyLiaison(int? patientId, int Cycle, string Reason, string FeedBack, int? BillingcategoryId)
        {
            try
            {

                var patient = _db.Patients.Where(x => x.Id == patientId).FirstOrDefault();
                patient.EnrollmentNotes = FeedBack;
                _db.Entry(patient).State = EntityState.Modified;
                _db.SaveChanges();
                var patientccmcycle = Cycle;
                CategoryCycleStatusHelper.User = User;
                var patientccmcyclestatus = CategoryCycleStatusHelper.GetPatientNewOrOldCycleStatusbyCategory(patient.Id,BillingCodeHelper.cmmBillingCatagoryid, patientccmcycle);
                if (patient != null)
                {
                    if (BillingcategoryId == BillingCodeHelper.cmmBillingCatagoryid)
                    {

                        if (patientccmcyclestatus == "Ready for Clinical Sign-Off")
                        {
                            HelperExtensions.UpdateCCMCycleStatus(patient.Id, patientccmcycle, "Enrolled", User.Identity.GetUserId(), Reason, true);
                        }
                        else
                        {
                            return RedirectToAction("Details", "Patient", new { id = patient.Id, status = "Not Allowed." });
                        }
                    }
                    else
                    {
                        CategoryCycleStatusHelper.User = User;
                        var patientcyclestatus = CategoryCycleStatusHelper.GetPatientNewOrOldCycleStatusbyCategory(patient.Id, BillingcategoryId.GetInteger(), patientccmcycle);


                        if (patientcyclestatus == "Ready for Clinical Sign-Off")
                        {

                            HelperExtensions.UpdatecategoryStatus(BillingcategoryId, patient.Id, patientccmcycle, "Enrolled", User.Identity.GetUserId(), Reason, true);


                        }
                        else
                        {
                            return RedirectToAction("Details", "Patient", new { id = patient.Id, status = "Not Allowed." });
                        }
                    }

                }
                return RedirectToAction("Index", "CcmStatus", new { status = "Ready for Clinical Sign-Off", BillingcategoryId = BillingcategoryId });
            }
            catch (Exception ex)
            {

                log.Error(Environment.NewLine + User.Identity.GetUserName() + "-------" + User.Identity.GetUserId() + Environment.NewLine + ex.Message + "-----" + ex.StackTrace);
                return RedirectToAction("Index", "CcmStatus", new { status = "Ready for Clinical Sign-Off" });

                /*return ex.Message + "------------------" + ex.StackTrace;*/
            }
        }
        [Authorize(Roles = "Admin, QAQC")]
        public ActionResult BacktoProgess(int? patientId, int Cycle, string Reason, string FeedBack, int? BillingcategoryId)
        {
            try
            {
                var patient = _db.Patients.Where(x => x.Id == patientId).FirstOrDefault();
                patient.EnrollmentNotes = FeedBack;
                _db.Entry(patient).State = EntityState.Modified;
                _db.SaveChanges();
                var patientccmcycle = Cycle;
                CategoryCycleStatusHelper.User = User;
                var patientccmcyclestatus = CategoryCycleStatusHelper.GetPatientNewOrOldCycleStatusbyCategory(patient.Id, BillingCodeHelper.cmmBillingCatagoryid, patientccmcycle);
                if (patient != null)
                {
                    if (BillingcategoryId == BillingCodeHelper.cmmBillingCatagoryid)
                    {

                        if (patientccmcyclestatus == "Clinical Sign-Off")
                        {
                            HelperExtensions.UpdateCCMCycleStatus(patient.Id, patientccmcycle, "Enrolled", User.Identity.GetUserId(), Reason);
                        }
                        else
                        {
                            return RedirectToAction("Details", "Patient", new { id = patient.Id, status = "Not Allowed." });
                        }
                    }
                    else
                    {
                        CategoryCycleStatusHelper.User = User;
                        var patientcyclestatus = CategoryCycleStatusHelper.GetPatientNewOrOldCycleStatusbyCategory(patient.Id,BillingcategoryId.GetInteger(), patientccmcycle);


                        if (patientcyclestatus == "Clinical Sign-Off")
                        {
                            HelperExtensions.UpdatecategoryStatus(BillingcategoryId, patient.Id, patientccmcycle, "Enrolled", User.Identity.GetUserId(), Reason);
                        }
                        else
                        {
                            return RedirectToAction("Details", "Patient", new { id = patient.Id, status = "Not Allowed." });
                        }

                    }


                }
                return RedirectToAction("Index", "CcmStatus", new { status = "Clinical Sign-Off", BillingcategoryId = BillingcategoryId });
            }
            catch (Exception ex)
            {

                log.Error(Environment.NewLine + User.Identity.GetUserName() + "-------" + User.Identity.GetUserId() + Environment.NewLine + ex.Message + "-----" + ex.StackTrace);
                return RedirectToAction("Index", "CcmStatus", new { status = "Clinical Sign-Off" });
                /*return ex.Message + "------------------" + ex.StackTrace;*/
            }
        }
        [Authorize(Roles = "Admin, QAQC")]
        public ActionResult ClaimsSubmission(int? patientId, int? BillingcategoryId, int Cycle = 0)
        {
            try
            {


                var patient = _db.Patients.Where(x => x.Id == patientId).FirstOrDefault();
                if (patient == null)
                {
                    ViewBag.Message = "Patient Not Found!";
                    return View("Error");
                }

                //var patientccmcycle = CategoryCycleStatusHelper.GetPatientNewOrOldCycleByCategory(patient);
                var patientccmcycle = Cycle;
                CategoryCycleStatusHelper.User = User;
                var patientccmcyclestatus = CategoryCycleStatusHelper.GetPatientNewOrOldCycleStatusbyCategory(patient.Id, BillingCodeHelper.cmmBillingCatagoryid, patientccmcycle);
                if (BillingcategoryId == BillingCodeHelper.cmmBillingCatagoryid)
                {

                    if (patient != null)
                    {
                        if (patientccmcyclestatus == "Clinical Sign-Off")
                        {
                            HelperExtensions.UpdateCCMCycleStatus(patient.Id, patientccmcycle, "Claims Submission", User.Identity.GetUserId(), "");
                        }
                        else
                        {
                            return RedirectToAction("Details", "Patient", new { id = patient.Id, status = "Not Allowed." });
                        }
                    }
                    if (string.IsNullOrEmpty(patient.CcmBillingCode) || string.IsNullOrEmpty(patient.CcmBillingCode2))
                    {
                        ViewBag.Message = "No Billing Codes Found for Claims Submission!";
                        return View("Error");
                    }

                    patient.CcmStatus = "Claims Submission";
                    patient.CcmClaimSubmissionDate = DateTime.Now;
                    patient.UpdatedOn = DateTime.Now;
                    patient.UpdatedBy = User.Identity.GetUserId();
                    _db.Entry(patient).State = EntityState.Modified;
                    _db.SaveChanges();

                }
                else
                {
                    CategoryCycleStatusHelper.User = User;
                    var patientcyclestatus = CategoryCycleStatusHelper.GetPatientNewOrOldCycleStatusbyCategory(patient.Id, BillingcategoryId.GetInteger(), patientccmcycle);
                    if (patient != null)
                    {
                        if (patientcyclestatus == "Clinical Sign-Off")
                        {
                            HelperExtensions.UpdatecategoryStatus(BillingcategoryId, patient.Id, patientccmcycle, "Claims Submission", User.Identity.GetUserId(), "");


                        }
                        else
                        {
                            return RedirectToAction("Details", "Patient", new { id = patient.Id, status = "Not Allowed." });
                        }
                    }




                }
                return RedirectToAction("Index", "CcmStatus", new { status = "Claims Submission", BillingcategoryId = BillingcategoryId });
            }
            catch (Exception ex)
            {

                log.Error(Environment.NewLine + User.Identity.GetUserName() + "-------" + User.Identity.GetUserId() + Environment.NewLine + ex.Message + "-----" + ex.StackTrace);
                return RedirectToAction("Index", "CcmStatus", new { status = "Claims Submission" });
                /*return ex.Message + "------------------" + ex.StackTrace;*/
            }
        }

        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Reconciliation(int? patientId)
        {
            try
            {


                var patient = await _db.Patients.FindAsync(patientId);
                if (patient != null)
                {
                    patient.CcmStatus = "Enrolled";
                    patient.CcmReconciliationDate = DateTime.Now;
                    patient.UpdatedOn = DateTime.Now;
                    patient.UpdatedBy = User.Identity.GetUserId();
                    _db.Entry(patient).State = EntityState.Modified;
                    await _db.SaveChangesAsync();
                }

                return RedirectToAction("Reconciliation", "CcmStatus");
            }
            catch (Exception ex)
            {

                log.Error(Environment.NewLine + User.Identity.GetUserName() + "-------" + User.Identity.GetUserId() + Environment.NewLine + ex.Message + "-----" + ex.StackTrace);
                return RedirectToAction("Reconciliation", "CcmStatus");
                /*return ex.Message + "------------------" + ex.StackTrace;*/
            }
        }


        [Authorize(Roles = "Admin")]
        public ActionResult GenerateInvoice(int? patientId)
        {
            return View();
        }

        public string GetCCMCycle(int patientId, int Cycle)
        {

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

                cCMCycleStatus.CreatedBy = User.Identity.GetUserId();
                cCMCycleStatus.CreatedOn = DateTime.Now;
                _db.CCMCycleStatuses.Add(cCMCycleStatus);
                _db.SaveChanges();
                return cCMCycleStatus.CCMStatus;
            }
            else
            {
                return CCMCycleStatus.CCMStatus;
            }
            //return HelperExtensions.GetCCMCycleStatus(patientId, Cycle, User.Identity.GetUserId());
        }
        public TimeSpan TotalReviewTime(int patientId, int Cycle)
        {
            try
            {

                var ccmId = BillingCodeHelper.cmmBillingCatagoryid;
                var reviews = _db.ReviewTimeCcms.Where(r => r.PatientId == patientId && r.Cycle == Cycle && r.BillingcategoryId == ccmId).AsNoTracking().ToList();

                return reviews.Any()
                    ? reviews.Aggregate(TimeSpan.Zero, (sum, review) => sum + review.ReviewTime)
                    : TimeSpan.Zero;
            }
            catch (Exception ex)
            {
                var ccmId = BillingCodeHelper.cmmBillingCatagoryid;
                log.Error(Environment.NewLine + User.Identity.GetUserName() + "-------" + User.Identity.GetUserId() + Environment.NewLine + ex.Message + "-----" + ex.StackTrace);

                var reviews = _db.ReviewTimeCcms.Where(r => r.PatientId == patientId && r.Cycle == Cycle && r.BillingcategoryId == ccmId).AsNoTracking().ToList();

                return reviews.Any()
                    ? reviews.Aggregate(TimeSpan.Zero, (sum, review) => sum + review.ReviewTime)
                    : TimeSpan.Zero;

                /*return ex.Message + "------------------" + ex.StackTrace;*/
            }
        }

        public JsonResult CheckHospitalVisit(int? patientId)
        {
            var date = DateTime.Now;
            var firstDayOfMonth = new DateTime(date.Year, date.Month, 1);
            var count = _db.patientProfile_Hospitalvisits.Where(p => p.PatientId == patientId && (p.CreatedOn >= firstDayOfMonth || p.UpdatedOn >= firstDayOfMonth)).Count();
            if (count > 0)
            {

                return Json("Visited", JsonRequestBehavior.AllowGet);
            }
            else
            {
                var checkisnotvisited = _db.Patient_HospitalVisit_NotAdmitted.Where(p => p.PatientId == patientId && (p.CreatedOn >= firstDayOfMonth || p.UpdatedOn >= firstDayOfMonth) && p.Status == true).Count();
                if (checkisnotvisited > 0)
                {
                    return Json("Visited", JsonRequestBehavior.AllowGet);
                }
                else
                {

                    return Json("NotVisited", JsonRequestBehavior.AllowGet);
                }

            }


        }
        public JsonResult CheckAdditionProviders(int? patientId)
        {
            var date = DateTime.Now;
            var firstDayOfMonth = new DateTime(date.Year, date.Month, 1);
            var count = _db.SecondaryDoctors.Where(p => p.PatientId == patientId && (p.CreatedOn >= firstDayOfMonth || p.UpdatedOn >= firstDayOfMonth)).Count();
            if (count > 0)
            {

                return Json("Visited", JsonRequestBehavior.AllowGet);
            }
            else
            {
                var checkisnotvisited = _db.Patient_NoAdditionalProvider.Where(p => p.PatientId == patientId && (p.CreatedOn >= firstDayOfMonth || p.UpdatedOn >= firstDayOfMonth) && p.Status == true).Count();
                if (checkisnotvisited > 0)
                {
                    return Json("Visited", JsonRequestBehavior.AllowGet);
                }
                else
                {

                    return Json("NotVisited", JsonRequestBehavior.AllowGet);
                }

            }


        }
        public JsonResult PatientNotVisited(int? patientId, bool status)
        {
            var alreadyExist = _db.Patient_HospitalVisit_NotAdmitted.Where(p => p.PatientId == patientId).FirstOrDefault();

            if (alreadyExist == null)
            {

                Patient_HospitalVisit_NotAdmitted patient_HospitalVisit_NotAdmitted = new Patient_HospitalVisit_NotAdmitted();
                patient_HospitalVisit_NotAdmitted.PatientId = patientId;
                patient_HospitalVisit_NotAdmitted.PatientNotAdmitted = status;
                patient_HospitalVisit_NotAdmitted.Status = status;
                patient_HospitalVisit_NotAdmitted.CreatedBy = User.Identity.GetUserId();
                patient_HospitalVisit_NotAdmitted.CreatedOn = DateTime.Now;
                _db.Patient_HospitalVisit_NotAdmitted.Add(patient_HospitalVisit_NotAdmitted);
                _db.SaveChanges();
            }
            else
            {
                alreadyExist.Status = status;
                alreadyExist.PatientNotAdmitted = status;
                alreadyExist.UpdatedOn = DateTime.Now;
                alreadyExist.UpdatedBy = User.Identity.GetUserId();
                _db.Entry(alreadyExist).State = EntityState.Modified;
                _db.SaveChanges();


            }



            return Json("Saved", JsonRequestBehavior.AllowGet);
        }


        public JsonResult PatientNoSecondaryDoctor(int? patientId, bool status)
        {
            var alreadyExist = _db.Patient_NoAdditionalProvider.Where(p => p.PatientId == patientId).FirstOrDefault();

            if (alreadyExist == null)
            {

                Patient_NoAdditionalProvider Patient_NoAdditionalProvider = new Patient_NoAdditionalProvider();
                Patient_NoAdditionalProvider.PatientId = patientId;
                Patient_NoAdditionalProvider.PatientNoSecondaryDoctor = status;
                Patient_NoAdditionalProvider.Status = status;
                Patient_NoAdditionalProvider.CreatedBy = User.Identity.GetUserId();
                Patient_NoAdditionalProvider.CreatedOn = DateTime.Now;
                _db.Patient_NoAdditionalProvider.Add(Patient_NoAdditionalProvider);
                _db.SaveChanges();
            }
            else
            {
                alreadyExist.Status = status;
                alreadyExist.PatientNoSecondaryDoctor = status;
                alreadyExist.UpdatedOn = DateTime.Now;
                alreadyExist.UpdatedBy = User.Identity.GetUserId();
                _db.Entry(alreadyExist).State = EntityState.Modified;
                _db.SaveChanges();


            }



            return Json("Saved", JsonRequestBehavior.AllowGet);
        }






        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}