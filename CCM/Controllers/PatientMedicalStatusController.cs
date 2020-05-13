using System;
using CCM.Models;
using System.Web.Mvc;
using System.Data.Entity;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using System.Linq;
using System.Collections.Generic;
using CCM.Helpers;

namespace CCM.Controllers
{
    [Authorize(Roles = "Liaison, Admin, QAQC, PhysiciansGroup, LiaisonGroup,Sales")]
    public class PatientMedicalStatusController : BaseController
    {
       // private readonly ApplicationdbContect _db = new ApplicationdbContect();


        public async Task<ActionResult> Create(int patientId)
        {
            var patient       = _db.Patients.Find(patientId);
            var medicalStatus = patient?.MedicalStatusId != null
                              ? await _db.PatientMedicalHistory_MedicalStatuses.FindAsync(patient.MedicalStatusId)
                              : new PatientMedicalHistory_MedicalStatus { PatientId = patientId };
            medicalStatus.Age = (int.Parse(DateTime.Now.ToString("yyyyMMdd")) -
                                 int.Parse(patient?.BirthDate.ToString("yyyyMMdd"))) / 10000;
            medicalStatus.PrimaryPhysician = patient?.Physician.FirstName + " " + patient?.Physician.LastName;
            medicalStatus.PhysicianPhoneNumber = patient?.Physician.MainPhoneNumber;

            ViewBag.PatientName = patient?.FirstName + " " + patient?.LastName;
            ViewBag.PatientId   = patient?.Id;
            ViewBag.CcmStatus   = patient?.CcmStatus;

            if (User.IsInRole("Liaison"))
                ViewBag.ReviewId = HelperExtensions.ReviewTimeGet("General Medical Status", patient?.Id, User.Identity.GetUserId());

            return View(medicalStatus);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Liaison, Admin")]
        public async Task<ActionResult> Create(PatientMedicalHistory_MedicalStatus medicalStatus)
        {
            if (HelperExtensions.isAllowedforEditingorAdd(medicalStatus.PatientId, CategoryCycleStatusHelper.GetPatientNewOrOldCycleByCategory(medicalStatus.PatientId, BillingCodeHelper.cmmBillingCatagoryid), User.Identity.GetUserId()) == false)
            {
                return RedirectToAction("Index", "CcmStatus", new { status = HelperExtensions.GetStatusRedirectionbyUser(User.Identity.GetUserId()), Message = "Cycle is locked." });
            }
            var patient = _db.Patients.Find(medicalStatus.PatientId);
            if (patient != null && ModelState.IsValid)
            {
                if (patient.MedicalStatusId != null)
                    _db.Entry(medicalStatus).State = EntityState.Modified;

                else
                {
                    _db.PatientMedicalHistory_MedicalStatuses.Add(medicalStatus);
                    await _db.SaveChangesAsync();

                    patient.MedicalStatusId = medicalStatus.Id;
                }

                patient.UpdatedBy = User.Identity.GetUserId();
                patient.UpdatedOn = DateTime.Now;
                _db.Entry(patient).State = EntityState.Modified;
                await _db.SaveChangesAsync();

                return RedirectToAction("Create", "PatientMedicalHistory_MedicalCondition", new { patientId = patient.Id });
               
            }

            ViewBag.PatientName = patient?.FirstName + " " + patient?.LastName;
            ViewBag.PatientId   = patient?.Id;
            ViewBag.CcmStatus   = patient?.CcmStatus;

            return View(medicalStatus);
        }


        public async Task<PartialViewResult> _Create(int patientId)
        {
            var patient = _db.Patients.Find(patientId);
            var medicalStatus = patient?.MedicalStatusId != null
                              ? await _db.PatientMedicalHistory_MedicalStatuses.FindAsync(patient.MedicalStatusId)
                              : new PatientMedicalHistory_MedicalStatus { PatientId = patientId };
            medicalStatus.Age = (int.Parse(DateTime.Now.ToString("yyyyMMdd")) -
                                 int.Parse(patient?.BirthDate.ToString("yyyyMMdd"))) / 10000;
            medicalStatus.PrimaryPhysician = patient?.Physician.FirstName + " " + patient?.Physician.LastName;
            medicalStatus.PhysicianPhoneNumber = patient?.Physician.MainPhoneNumber;

            ViewBag.PatientName = patient?.FirstName + " " + patient?.LastName;
            ViewBag.PatientId = patient?.Id;
            ViewBag.CcmStatus = patient?.CcmStatus;

            //if (User.IsInRole("Liaison"))
            //    ViewBag.ReviewId = HelperExtensions.ReviewTimeGet("General Medical Status", patient?.Id, User.Identity.GetUserId());

            return PartialView(medicalStatus);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Liaison, Admin,Sales")]
        public async Task<string> _Create(PatientMedicalHistory_MedicalStatus medicalStatus)
        {
            if (HelperExtensions.isAllowedforEditingorAdd(medicalStatus.PatientId, CategoryCycleStatusHelper.GetPatientNewOrOldCycleByCategory(medicalStatus.PatientId, BillingCodeHelper.cmmBillingCatagoryid), User.Identity.GetUserId()) == false)
            {
                //return RedirectToAction("Index", "CcmStatus", new { status = HelperExtensions.GetStatusRedirectionbyUser(User.Identity.GetUserId()), Message = "Cycle is locked." });
                return "Cycle is locked.";
            }
            var patient = _db.Patients.Find(medicalStatus.PatientId);
            if (patient != null && ModelState.IsValid)
            {
                if (patient.MedicalStatusId != null)
                    _db.Entry(medicalStatus).State = EntityState.Modified;

                else
                {
                    _db.PatientMedicalHistory_MedicalStatuses.Add(medicalStatus);
                    await _db.SaveChangesAsync();

                    patient.MedicalStatusId = medicalStatus.Id;
                }

                patient.UpdatedBy = User.Identity.GetUserId();
                patient.UpdatedOn = DateTime.Now;
                _db.Entry(patient).State = EntityState.Modified;
                await _db.SaveChangesAsync();

                //return RedirectToAction("Create", "PatientMedicalHistory_MedicalCondition", new { patientId = patient.Id });
                return "True";
            }
            else
            {
                var errorList = ModelState.Values.SelectMany(m => m.Errors)
                                 .Select(e => e.ErrorMessage)
                                 .ToList();
                 if(errorList.Contains("Invalid Phone Number")&& errorList.Contains("Invalid Phone Number"))
                {
                var val= errorList.IndexOf("Invalid Phone Number");
                errorList.RemoveAt(val);

                }
                var errorstr = string.Join(",", errorList);
                return errorstr;
            }
            ViewBag.PatientName = patient?.FirstName + " " + patient?.LastName;
            ViewBag.PatientId = patient?.Id;
            ViewBag.CcmStatus = patient?.CcmStatus;

            return "False";
            //return View(medicalStatus);
        }


        public async Task<PartialViewResult> _MedicalHistory(int patientId)
        {
            var patient = _db.Patients.Find(patientId);
            var medicalStatus = patient?.MedicalStatusId != null
                              ? await _db.PatientMedicalHistory_MedicalStatuses.FindAsync(patient.MedicalStatusId)
                              : new PatientMedicalHistory_MedicalStatus { PatientId = patientId };
            medicalStatus.Age = (int.Parse(DateTime.Now.ToString("yyyyMMdd")) -
                                 int.Parse(patient?.BirthDate.ToString("yyyyMMdd"))) / 10000;
            medicalStatus.PrimaryPhysician = patient?.Physician.FirstName + " " + patient?.Physician.LastName;
            medicalStatus.PhysicianPhoneNumber = patient?.Physician.MainPhoneNumber;

            return PartialView(medicalStatus);
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