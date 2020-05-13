using System;
using CCM.Models;
using System.Linq;
using System.Web.Mvc;
using System.Data.Entity;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq.Dynamic;
using CCM.Models.ViewModels;
using CCM.Helpers;

namespace CCM.Controllers
{
    [RequireHttps]
    [Authorize(Roles = "Liaison, Physician, PhysiciansGroup, Admin, LiaisonGroup,Sales")]
    public class EnrollmentStatusController : BaseController
    {
        //private readonly ApplicationdbContect _db = new ApplicationdbContect();



        public async Task<ActionResult> Index(string userId, string status, DateTime? date, string substatus)
        {
            return RedirectToAction("TotalPatients", "Patient", new { userId = userId, status = status, date = date, substatus = substatus });
            //below code is unreachable comment by Arslan Saleem 
            //ViewBag.EnrollmentStauses = _db.EnrollmentStatuss.ToList();
            //ViewBag.EnrollmentSubStatuses = _db.EnrollmentSubStatuss.ToList();
            ////var today      = DateTime.Today;
            ////var patients   = date != null && date >= today ? _db.Patients.AsNoTracking().Where(p => DbFunctions.TruncateTime(p.AppointmentDate) == date)
            ////               : date != null && date <  today ? _db.Patients.AsNoTracking().Where(p => DbFunctions.TruncateTime(p.AppointmentDate) < today)
            ////               : !string.IsNullOrEmpty(status) && status == "Left Voice Message"
            ////               ? _db.Patients.AsNoTracking().Where(p => (p.EnrollmentStatus == "Left Voice Message 1" ||
            ////                                                         p.EnrollmentStatus == "Left Voice Message 2" ||
            ////                                                         p.EnrollmentStatus == "Left Voice Message 3"))
            ////               : _db.Patients.AsNoTracking().Where(p =>  p.EnrollmentStatus == status);
            //var user = string.IsNullOrEmpty(userId)
            //               ? _db.Users.Find(User.Identity.GetUserId())
            //               : _db.Users.Find(userId);

            //ViewBag.UserId = userId;
            //ViewBag.Status = date != null ? "Calls Due (" + (date == DateTime.Today ? "Today" : "Tomorrow") + ")"
            //               : string.IsNullOrEmpty(status) ? "Unknown"
            //               : status;
            //ViewBag.StatusStr = status;
            //ViewBag.SubStatus = substatus == null ? "" : substatus;
            //ViewBag.DateStr = date;
            //ViewBag.Owner = user.Role == "Liaison" || user.Role == "PhysiciansGroup" ? user.FirstName
            //               : user.Role == "Physician" ? "Dr. " + user.LastName
            //               : "Admin";
            //ViewBag.UserRole = user.Role;
            //return View("Index1");
            ////return View( user.Role == "Liaison"
            ////           ? await patients.Where(p => p.LiaisonId == user.CCMid).ToListAsync()
            ////           : user.Role == "Physician"
            ////           ? await patients.Where(p => p.PhysicianId == user.CCMid).ToListAsync()
            ////           : user.Role == "PhysiciansGroup"
            ////           ? await patients.Where(p => p.Physician.MainPhoneNumber == user.PhoneNumber).ToListAsync()
            ////           : await patients.ToListAsync());
        }


        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> NotAssigned()
        {
            ViewBag.EnrollmentStauses = _db.EnrollmentStatuss.ToList();
            ViewBag.EnrollmentSubStatuses = _db.EnrollmentSubStatuss.ToList();
            var liaison = _db.Liaisons.Select(p => new SelectListItem
            {
                Value = p.Id.ToString(),
                Text = p.FirstName + " " + p.LastName
            });

            ViewBag.Liaisons = new SelectList(liaison.OrderBy(l => l.Text), "Value", "Text");
            ViewBag.Status = "Not Assigned";
            ViewBag.Owner = "Admin";

            return View(await _db.Patients.AsNoTracking().Where(p => p.LiaisonId == null).ToListAsync());
        }



        [HttpPost]
        public ActionResult PatientNo(int? patientId)
        {
            Session["PatientNo"] = patientId;

            return RedirectToAction("AssignLiaison", "EnrollmentStatus", new { patientId = Session["PatientNo"] });
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Sales,LiaisonGroup")]
        public bool AssignTranslatorToAllPatients(int? translatorId, int[] Patients)
        {
            try
            {
                if (Patients.Count() > 0)
                {
                    foreach (var patientid in Patients)
                    {
                        var patient = _db.Patients.Where(x => x.Id == patientid).FirstOrDefault();
                        patient.TranslatorId = translatorId;
                        patient.TranslatorAssignedBy = User.Identity.GetUserId();
                        patient.TranslatorAssignedOn = DateTime.Now;
                        //patient.EnrollmentStatus = "Not Enrolled";
                        //patient.EnrollmentSubStatus = "Assigned but No Activity";
                        patient.UpdatedOn = DateTime.Now;
                        patient.UpdatedBy = User.Identity.GetUserId();
                        _db.Entry(patient).State = EntityState.Modified;

                    }
                    _db.SaveChanges();
                }

            }
            catch /*(Exception ex)*/
            {

                return false;
            }
            return true;
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Sales,LiaisonGroup")]
        public string AssignLiaisonToAllPatients(int? liaisonId, int? translatorId, int[] Patients, string EnrolmentStatus, string EnrollmentSubStatus, string EnrollmentReason, string istoUpdateStatus, string istoUpdateAppointments,string isliaisonChange,string istranslatorChange)
        {
            try
            {
                string msg = "";
                List<string> list = new List<string>();
                var patientappoinment = 0;

                if (EnrollmentSubStatus != "In-Active Enrolled")
                {
                    EnrollmentReason = "";
                }
             
                //if (istoUpdateAppointments != "Yes" && istoUpdateStatus != "Yes")
                //    msg = "Liaison have been updated without Status and Patient's have no Appointments.";

                var liasionschedule = _db.doctorSchedules.Where(x => x.LiaisonID == liaisonId).OrderByDescending(x => x.CreateDate).FirstOrDefault();
                if (liasionschedule != null)
                {
                }
                var translatorschedule = _db.doctorSchedules.Where(x => x.LiaisonID == translatorId).OrderByDescending(x => x.CreateDate).FirstOrDefault();
                if (translatorschedule != null)
                {
                }

                foreach (var patientid in Patients)
                {
                    var patient = _db.Patients.Where(x => x.Id == patientid).FirstOrDefault();
                    var oldliaison = patient.LiaisonId;
                    var oldtranslattor = patient.TranslatorId;
                    if (istoUpdateAppointments != "Yes")
                    {
                        if (isliaisonChange == "Yes")
                        {
                            patient.LiaisonId = liaisonId;
                            patient.LiasionAssignedBy = User.Identity.GetUserId();
                            patient.LiasionAssignedOn = DateTime.Now;
                        }
                        if (istranslatorChange == "Yes")
                        {
                            patient.TranslatorId = translatorId;
                            patient.TranslatorAssignedBy = User.Identity.GetUserId();
                            patient.TranslatorAssignedOn = DateTime.Now;
                        }
                        if (istoUpdateStatus == "Yes" )
                        {
                            if (!User.IsInRole("Admin") && !User.IsInRole("LiaisonGroup"))
                            {
                                if ((patient.EnrollmentStatus == "Enrolled" && patient.EnrollmentSubStatus == "Active Enrolled") && (EnrolmentStatus != "Enrolled" && EnrollmentSubStatus != "Active Enrolled"))
                                {
                                   
                                }
                                else
                                {
                                    patient.EnrollmentStatus = EnrolmentStatus;
                                    patient.EnrollmentSubStatus = EnrollmentSubStatus;
                                    patient.EnrollmentSubStatusReason = EnrollmentReason;

                                }
                            }
                            else
                            {
                                patient.EnrollmentStatus = EnrolmentStatus;
                                patient.EnrollmentSubStatus = EnrollmentSubStatus;
                                patient.EnrollmentSubStatusReason = EnrollmentReason;
                            }
                          
                        }
                        //Messages
                        if(isliaisonChange=="Yes" && istranslatorChange!="Yes" && istoUpdateStatus!="Yes")
                            msg = "Liaison has been changed successfully.!";
                        else if(isliaisonChange == "Yes" && istranslatorChange == "Yes" && istoUpdateStatus != "Yes")
                            msg = "Liaison & Translator has been changed successfully.!";
                        else if (isliaisonChange == "Yes" && istranslatorChange != "Yes" && istoUpdateStatus == "Yes")
                            msg = "Liaison & Status has been changed successfully.!";
                        else if(isliaisonChange == "Yes" && istranslatorChange == "Yes" && istoUpdateStatus == "Yes")
                            msg = "Liaison ,Status & Translator has been changed successfully.!";
                        else if(isliaisonChange != "Yes" && istranslatorChange == "Yes" && istoUpdateStatus != "Yes")
                            msg = "Translator has been changed successfully.!";
                        else if (isliaisonChange != "Yes" && istranslatorChange == "Yes" && istoUpdateStatus == "Yes")
                            msg = "Translator & Status has been changed successfully.!";
                        else if (isliaisonChange != "Yes" && istranslatorChange != "Yes" && istoUpdateStatus == "Yes")
                            msg = "Status has been changed successfully.!";
                    }
                    else if(istoUpdateAppointments == "Yes")
                    {
                        if (isliaisonChange != "Yes" && istranslatorChange != "Yes")
                        {
                            msg = "Please select Liaison/Translator First";
                        }
                        else if(isliaisonChange == "Yes" || istranslatorChange == "Yes")
                        {
                            int countliasiontranslator = 0;
                            if (isliaisonChange == "Yes")
                            {
                                if (liasionschedule != null)
                                {
                                    if (oldliaison != null)
                                    {
                                        var pa = _db.patientAppointments.Where(x => x.PatientID == patientid  && x.LiaisonID == oldliaison && x.StartTime > DateTime.Now).ToList();
                                        if (pa.Count > 0)
                                        {
                                            foreach (var ptime in pa)
                                            {
                                                patientappoinment = 1;
                                                ptime.LiaisonID = liaisonId.Value;
                                                ptime.UpdateOn = DateTime.Now;
                                                ptime.UpdatedBy = 0;
                                                _db.Entry(ptime).State = EntityState.Modified;
                                                _db.SaveChanges();
                                            }
                                            msg = "Liaison has been changed along with Appointments successfully.!";
                                        }
                                    }
                                    else
                                    {
                                        msg = "Liaison has been changed successfully.No Liaison was found so No Appointments Migrates.!";
                                    }
                                    if (istoUpdateStatus == "Yes")
                                    {
                                        if (!User.IsInRole("Admin") && !User.IsInRole("LiaisonGroup"))
                                        {
                                            if ((patient.EnrollmentStatus == "Enrolled" && patient.EnrollmentSubStatus == "Active Enrolled") && (EnrolmentStatus != "Enrolled" && EnrollmentSubStatus != "Active Enrolled"))
                                            {
                                                msg = "Liaison has been changed along with appointments successfully.But the status is not changed as this user is not allowed to change the status!";
                                            }
                                            else
                                            {
                                                patient.EnrollmentStatus = EnrolmentStatus;
                                                patient.EnrollmentSubStatus = EnrollmentSubStatus;
                                                patient.EnrollmentSubStatusReason = EnrollmentReason;
                                                msg = "Liaison & Status has been changed along with appointments successfully.!";
                                            }
                                        }
                                        else
                                        {
                                            patient.EnrollmentStatus = EnrolmentStatus;
                                            patient.EnrollmentSubStatus = EnrollmentSubStatus;
                                            patient.EnrollmentSubStatusReason = EnrollmentReason;
                                            msg = "Liaison & Status has been changed along with appointments successfully.!";
                                        }
                                        
                                    }
                                    patient.LiaisonId = liaisonId;
                                    patient.LiasionAssignedBy = User.Identity.GetUserId();
                                    patient.LiasionAssignedOn = DateTime.Now;
                                    countliasiontranslator = 1;
                                }
                                else { msg = "No Activity performed, Liaison schedule is not created, please create schedule first.!"; }
                            }
                            if (istranslatorChange == "Yes")
                            {
                                if (translatorschedule != null)
                                {
                                    if (oldtranslattor != null)
                                    {
                                        var pa = _db.patientAppointments.Where(x => x.PatientID == patientid && x.LiaisonID == oldtranslattor && x.StartTime > DateTime.Now).ToList();
                                        if (pa.Count > 0)
                                        {
                                            foreach (var ptime in pa)
                                            {
                                                patientappoinment = 1;
                                                ptime.LiaisonID = translatorId.Value;
                                                ptime.UpdateOn = DateTime.Now;
                                                ptime.UpdatedBy = 0;
                                                _db.Entry(ptime).State = EntityState.Modified;
                                                _db.SaveChanges();
                                            }
                                            msg = "Translator has been changed along with Appointments successfully.!";
                                        }
                                        else
                                        {
                                            msg = "Translator has been changed successfully.No Appointments was found from patients!";
                                        }
                                    }
                                    else { msg = "Translator has been changed successfully.No Translator was found so No Appointments Migrates.!"; }
                                    if (istoUpdateStatus == "Yes")
                                    {
                                        patient.EnrollmentStatus = EnrolmentStatus;
                                        patient.EnrollmentSubStatus = EnrollmentSubStatus;
                                        patient.EnrollmentSubStatusReason = EnrollmentReason;
                                        msg = "Translator & Status has been changed along with appointments successfully.!";
                                    }
                                    patient.TranslatorId = translatorId;
                                    patient.TranslatorAssignedBy = User.Identity.GetUserId();
                                    patient.TranslatorAssignedOn = DateTime.Now;
                                    countliasiontranslator = 1;
                                }
                                else { msg = "No Activity performed, Translator schedule is not created, please create schedule first.!"; }
                            }
                            //Messages
                            if (istranslatorChange=="Yes" && isliaisonChange=="Yes" && translatorschedule == null && liasionschedule == null)
                                msg = "No Activity performed, Liaison/Translator schedule is not created, please create schedule first.!";
                            else if(countliasiontranslator==2)
                                msg = " Liaison & Translator has been changed along with appointments successfully.!";
                            if (istranslatorChange == "Yes" && isliaisonChange == "Yes" && translatorschedule != null && liasionschedule != null)
                                msg = "No Activity performed, Liaison/Translator schedule is not created, please create schedule first.!";
                        }
                    }

                    patient.UpdatedOn = DateTime.Now;
                    patient.UpdatedBy = User.Identity.GetUserId();
                    _db.Entry(patient).State = EntityState.Modified;
                }

                _db.SaveChanges();
                //if (list.Count > 0)
                //{
                //    var res = list.GroupBy(x => x).Select(group => new
                //    {
                //        Name = "PatientId " + group.Key + " (" + group.Count().ToString() + " appointments)"
                //    }).ToList();
                    

                //    if (istoUpdateStatus != "Yes")
                //        msg = "Liaison have been updated with Appointments. '" + Environment.NewLine + string.Join(",", res.Select(x => x.Name).ToArray()) + "' have not shifted due to time conflict, please shift them manually. No Status changed";
                //    else if (istoUpdateStatus == "Yes" && istoUpdateAppointments == "Yes")
                //        msg = "Liaison & Status have been updated with Appointments.'" + Environment.NewLine + string.Join(",", res.Select(x => x.Name).ToArray()) + "' have not shifted due to time conflict, please shift them manually.";

                //    //msg = msg + Environment.NewLine + string.Join(Environment.NewLine, list);
                //}
                return msg;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        [HttpPost]
        [Authorize(Roles = "Admin,Sales")]
        public bool AssignLanguageToAllPatients(string Language, int[] Patients)
        {
            try
            {
                if (Patients.Count() > 0)
                {
                    foreach (var patientid in Patients)
                    {
                        var patient = _db.Patients.Where(x => x.Id == patientid).FirstOrDefault();
                        patient.PreferredLanguage = Language;
                        patient.UpdatedOn = DateTime.Now;
                        patient.UpdatedBy = User.Identity.GetUserId();
                        _db.Entry(patient).State = EntityState.Modified;

                    }
                    _db.SaveChanges();
                }
            }
            catch /*(Exception ex)*/
            {

                return false;
            }
            return true;
        }
        [HttpPost]
        [Authorize(Roles = "Admin,Sales")]
        public bool AssignStatusToAllPatients(string EnrolmentStatus, string EnrollmentSubStatus, string EnrollmentReason, string EnrollmentStatusnote, int[] Patients)
        {
            try
            {
                if (EnrollmentSubStatus != "In-Active Enrolled")
                {
                    EnrollmentReason = "";
                }
                if (Patients.Count() > 0)
                {
                    foreach (var patientid in Patients)
                    {
                        var patient = _db.Patients.Where(x => x.Id == patientid).FirstOrDefault();
                        patient.EnrollmentStatus = EnrolmentStatus;
                        patient.EnrollmentSubStatus = EnrollmentSubStatus;
                        patient.EnrollmentSubStatusReason = EnrollmentReason;
                        patient.EnrollmentStatusNotes = EnrollmentStatusnote;
                        patient.UpdatedBy = User.Identity.GetUserId();
                        patient.UpdatedOn = DateTime.Now;
                        if (EnrollmentSubStatus == "Active Enrolled")
                        {
                            patient.CcmStatus = "Enrolled";

                            if (patient.CCMEnrolledOn == null)
                            {
                                patient.CCMEnrolledOn = DateTime.Now;
                                patient.CCMEnrolledBy = User.Identity.GetUserId();
                                //HelperExtensions.UpdateCurrentMonthActivityfromCycleZeroToOne(patient.Id);
                                try
                                {


                                    var reviewtimeccms = _db.ReviewTimeCcms.Where(x => x.PatientId == patient.Id && x.Cycle == 0).ToList().Where(x => x.StartTime.Date.Month == DateTime.Now.Month).ToList();
                                    foreach (var reviewtimeccmitem in reviewtimeccms)
                                    {
                                        reviewtimeccmitem.Cycle = 1;
                                        _db.Entry(reviewtimeccmitem).State = EntityState.Modified;
                                        _db.SaveChanges();
                                    }
                                }
                                catch/* (Exception ex)*/
                                {


                                }
                            }



                        }
                        //patient.CCMEnrolledBy = User.Identity.GetUserId();
                        //patient.CCMEnrolledOn = DateTime.Now;
                        _db.Entry(patient).State = EntityState.Modified;
                        _db.SaveChanges();
                        patient.Cycle = CategoryCycleStatusHelper.GetPatientNewOrOldCycleByCategory(patient.Id, BillingCodeHelper.cmmBillingCatagoryid);
                        //HelperExtensions.GetCCMCycleStatus(patient.Id, patient.Cycle, User.Identity.GetUserId(),patient.EnrollmentSubStatus);
                        CategoryCycleStatusHelper.GetPatientNewOrOldCycleStatusbyCategory(patient.Id, BillingCodeHelper.cmmBillingCatagoryid, patient.Cycle);

                    }

                }

            }
            catch /*(Exception ex)*/
            {

                return false;
            }
            return true;
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Sales")]
        public JsonResult AssignLiaison(int? liaisonId)
        {
            var patientId = (int)Session["PatientNo"];

            var patient = _db.Patients.Find(patientId);
            var liaison = _db.Liaisons.Find(liaisonId);

            //if (patient != null && liaison != null)
            try
            {
                patient.Liaison = liaison;
                patient.LiaisonId = liaisonId;
                patient.LiasionAssignedBy = User.Identity.GetUserId();
                patient.LiasionAssignedOn = DateTime.Now;
                patient.EnrollmentStatus = "Not Enrolled";
                patient.EnrollmentSubStatus = "Assigned but No Activity";
                _db.SaveChanges();
            }
            catch (DbEntityValidationException e)
            {
                foreach (var eve in e.EntityValidationErrors)
                {
                    Debug.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State);
                    foreach (var ve in eve.ValidationErrors)
                    {
                        Debug.WriteLine("- Property: \"{0}\", Value: \"{1}\", Error: \"{2}\"",
                            ve.PropertyName,
                            eve.Entry.CurrentValues.GetValue<object>(ve.PropertyName),
                            ve.ErrorMessage);
                    }
                }
                throw;
            }

            return new JsonResult
            {
                JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                Data = new { result = "Assigned", PatientName = patient.FirstName, LiaisonName = liaison.FirstName, PatientId = patientId }
            };



            //}

            //if (liaison == null && patient != null)
            //{
            //    var liaisonName = patient.Liaison?.FirstName;
            //    patient.Liaison = null;
            //    patient.LiaisonId = null;
            //    _db.SaveChanges();

            //    return Json(new { Action = "Unassigned", PatientName = patient.FirstName, LiaisonName = liaisonName }, JsonRequestBehavior.AllowGet);
            //}

            //return Json(new { Action = "Error", PatientId = patientId, LiaisonId = liaisonId }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult LoadDrugData(string status, string userId, string date1, string substatus)
        {
            DateTime? date = null;
            if (date1 != null)
            {
                date = Convert.ToDateTime(date1);

            }
            var draw = Request.Form.GetValues("draw")?.FirstOrDefault();
            var start = Request.Form.GetValues("start")?.FirstOrDefault();
            var length = Request.Form.GetValues("length")?.FirstOrDefault();
            var sortColumn = Request.Form.GetValues("columns[" + Request.Form.GetValues("order[0][column]")?.FirstOrDefault() + "][name]")?.FirstOrDefault();
            var sortColumnDir = Request.Form.GetValues("order[0][dir]")?.FirstOrDefault();
            string searchValue = Request.Form.GetValues("search[value]")?.FirstOrDefault();


            //Paging Size (10,20,50,100)    
            int pageSize = length != null ? Convert.ToInt32(length) : 0;
            int skip = start != null ? Convert.ToInt32(start) : 0;
            int recordsTotal = 0;
            var user = string.IsNullOrEmpty(userId)
                           ? _db.Users.Find(User.Identity.GetUserId())
                           : _db.Users.Find(userId);

            List<PatientViewModel> lstPatients = new List<PatientViewModel>();
            var today = DateTime.Today;
            var patients = date != null && date >= today ? _db.Patients.AsNoTracking().Where(p => DbFunctions.TruncateTime(p.AppointmentDate) == date)
                           : date != null && date < today ? _db.Patients.AsNoTracking().Where(p => DbFunctions.TruncateTime(p.AppointmentDate) < today)
                           : !string.IsNullOrEmpty(status) && status == "Left Voice Message"
                           ? _db.Patients.AsNoTracking().Where(p => (p.CallingStatus == "Left Voice Message 1" ||
                                                                     p.CallingStatus == "Left Voice Message 2" ||
                                                                     p.CallingStatus == "Left Voice Message 3"))
                           : !string.IsNullOrEmpty(status) && status == "Left Voice Message 1"
                           ? _db.Patients.AsNoTracking().Where(p => (p.CallingStatus == "Left Voice Message 1"))
                           : !string.IsNullOrEmpty(status) && status == "Left Voice Message 2"
                           ? _db.Patients.AsNoTracking().Where(p => (p.CallingStatus == "Left Voice Message 2"))
                           : !string.IsNullOrEmpty(status) && status == "Left Voice Message 3"
                           ? _db.Patients.AsNoTracking().Where(p => (p.CallingStatus == "Left Voice Message 3"))
                      : !string.IsNullOrEmpty(substatus) ? _db.Patients.AsNoTracking().Where(p => (p.EnrollmentSubStatus == substatus))
                                                                     : _db.Patients.AsNoTracking().Where(p => p.EnrollmentStatus == status)
                           ;
            //This is getting the Data
            var dataView = (from p in patients
                                //join pe in _db.PatientMeidcareMedicaidEligibilities on p.Id equals pe.PatientId into ps
                                //from pe in ps.DefaultIfEmpty()
                                //join pe1 in _db.CCMCycleStatuses on p.Id equals pe1.PatientId into ps1
                                //from pe1 in ps1.DefaultIfEmpty()
                                //join pbc in _db.BillingCycles on p.Id equals pbc.PatientId into pbc1
                                //from pbc in pbc1.DefaultIfEmpty()



                            select new
                            {
                                DaysinQue = 0,
                                Color = "",
                                dateColor = "",

                                DateEntered = DateTime.Now,
                                ReviewTime = DateTime.Now,
                                p.Id,
                                p.Cycle,
                                p.BirthDate,
                                Gender = p.Gender ?? "",
                                PreferredLanguage = p.PreferredLanguage ?? "",
                                p.AppointmentDate,
                                PatientName = p.FirstName + ", " + p.LastName,
                                EnrollmentStatus = p.EnrollmentStatus ?? "",
                                EnrollmentSubStatus = p.EnrollmentSubStatus ?? "",
                                p.LiaisonId,
                                liaisonFirstName = p.Liaison.FirstName ?? "",
                                liaisonLastName = p.Liaison.LastName ?? "",
                                DocFirstName = p.Physician.FirstName ?? "",
                                DocLastName = p.Physician.LastName ?? "",

                                CMMReviewLink = "",
                                p.CcmStatus,
                                p.CcmReconciliationDate,
                                p.CcmClaimSubmissionDate,
                                p.CcmClinicalSignOffDate,
                                p.CcmBillingCode,
                                p.CcmBillingCode2,
                                p.CCMEnrolledOn,

                                UserRole = user.Role.ToString(),
                                p.PhysicianId,
                                p.Physician.MainPhoneNumber,
                                p.LiasionAssignedOn,
                                p.EnrollmentNotes,
                                p.Notes,
                                p.OtherLanguage,
                                callingstatus = p.CallingStatus ?? "",
                                emrnumber = p.EMRNumber == null ? "" : p.EMRNumber,
                                emrtype = p.EMRType == null ? "" : p.EMRType
                            }).AsQueryable();




            dataView = user.Role == "Liaison"
                       ? dataView.Where(p => p.LiaisonId == user.CCMid).AsQueryable()
                       : user.Role == "Physician"
                       ? dataView.Where(p => p.PhysicianId == user.CCMid).AsQueryable()
                       : user.Role == "PhysiciansGroup"
                       ? dataView.Where(p => p.MainPhoneNumber == user.PhoneNumber).AsQueryable()
                       : dataView.AsQueryable();


            foreach (var item in dataView)
            {
                var age = (int.Parse(DateTime.Today.ToString("yyyyMMdd")) - int.Parse(item?.BirthDate.ToString("yyyyMMdd"))) / 10000;
                var ageStatus = item.EnrollmentStatus == "Deceased" ? "Deceased" : age + " Years";
                var color = ageStatus == "Deceased" ? "text-danger" : "";


                PatientViewModel item1 = new PatientViewModel();

                item1.Color = color;
                item1.ageStatus = ageStatus;




                item1.Cycle = item.Cycle;
                if (User.IsInRole("Admin"))
                {
                    if (item.LiaisonId == null)
                    {
                        item1.liaisonName = "Not Assigned";
                    }
                    else
                    {
                        item1.liaisonName = item?.liaisonFirstName + " " + item?.liaisonLastName;
                    }



                }
                item1.language = item?.OtherLanguage ?? item?.PreferredLanguage;
                if (item1.language == null)
                {
                    item1.language = "";

                }
                item1.PatientName = item.PatientName;
                item1.CallingStatus = item.callingstatus == null ? "" : item.callingstatus;
                item1.Gender = item.Gender == null ? "" : item.Gender;


                item1.BirthDate = item.BirthDate.ToString("MM/dd/yyyy") + " <span class='" + color + "'> " + ageStatus + " </span>";
                item1.note = string.IsNullOrEmpty(item?.EnrollmentNotes) ? item?.Notes : item?.EnrollmentNotes;

                if (item1.note == null)
                {
                    item1.note = "";
                }
                if (item.AppointmentDate != null)
                {
                    item1.AppointmentDate = item.AppointmentDate.Value.ToString("MM/dd/yyyy");
                }


                item1.DocFirstName = item.DocFirstName + " " + item?.DocLastName;

                item1.Id = item.Id;
                item1.CcmStatus = item.CcmStatus == null ? "" : item.CcmStatus;

                item1.EnrollmentStatus = item.EnrollmentStatus == null ? "" : item.EnrollmentStatus;
                item1.CcmEnrolledOn = item.CCMEnrolledOn == null ? "" : item.CCMEnrolledOn.ToString();
                item1.LiasionAssignedOn = item.LiasionAssignedOn == null ? "" : item.LiasionAssignedOn.ToString();
                item1.emrnumber = item.emrnumber == null ? "" : item.emrnumber;
                item1.emrtype = item.emrtype == null ? "" : item.emrtype;
                if (item.EnrollmentSubStatus != null && item.EnrollmentSubStatus != "")
                {
                    item1.EnrollmentSubStatus = item.EnrollmentSubStatus;
                }
                item1.UserRole = item.UserRole;
                item1.Detailslink = "Patient/Details?id=" + item1.Id.ToString();
                if (item.UserRole == "Admin")
                {
                    item1.Deletelink = "Patient/Delete?id=" + item1.Id.ToString();
                }

                lstPatients.Add(item1);
            }
            if (sortColumn == "")
            {
                sortColumn = "Id";
            }
            //SORT
            if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
            {
                lstPatients = lstPatients.OrderBy(sortColumn + " " + sortColumnDir).ToList();
            }
            //Search  
            try
            {
                if (!string.IsNullOrEmpty(searchValue) && !string.IsNullOrWhiteSpace(searchValue))
                {
                    bool isDateTimeSearch = false;

                    // Apply search   
                    if (isDateTimeSearch == false)
                    {
                        lstPatients = lstPatients.Where(p => p.PatientName.ToString().ToLower().Contains(searchValue.ToLower()) ||

                                                                             p.EnrollmentStatus.ToLower() == (searchValue.ToLower()) ||
                                                                              p.EnrollmentSubStatus.ToLower() == (searchValue.ToLower()) ||
                                                                               p.CcmEnrolledOn.ToLower() == (searchValue.ToLower()) ||
                                                                                p.LiasionAssignedOn.ToLower() == (searchValue.ToLower()) ||
                                                                             p.DocFirstName.ToLower().Contains(searchValue.ToLower()) ||

                                                                             p.liaisonName.ToLower().Contains(searchValue.ToLower()) ||

                                                                             p.Gender.ToLower() == (searchValue.ToLower()) ||

                                                                             p.language.ToLower().Contains(searchValue.ToLower()) ||

                                                                             p.BirthDate.ToLower().Contains(searchValue.ToLower()) ||
                                                                             p.AppointmentDate.ToLower().Contains(searchValue.ToLower()) ||
                                                                            p.CallingStatus.ToLower() == searchValue.ToLower() ||
                                                                             p.CcmStatus.ToLower().Contains(searchValue.ToLower()) ||
                                                                             p.note.ToLower().Contains(searchValue.ToLower()) ||
                                                                             p.emrnumber.ToLower().Contains(searchValue.ToLower()) ||
                                                                            p.emrtype.ToLower().Contains(searchValue.ToLower())



                                              ).ToList();
                    }


                }



                //total number of rows count     


            }
            catch /*(Exception ex)*/
            {


            }
            recordsTotal = lstPatients.Count();
            //Paging     
            if (pageSize == -1)
            {
                pageSize = recordsTotal;
            }
            lstPatients = lstPatients.Skip(skip).Take(pageSize).ToList();

            var jsonResult = Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = lstPatients, JsonRequestBehavior.AllowGet });
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
            ////Returning Json Data    
            //return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = lstPatients, JsonRequestBehavior.AllowGet });

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