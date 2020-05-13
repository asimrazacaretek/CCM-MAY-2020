using System;
using CCM.Models;
using System.Web.Mvc;
using System.Data.Entity;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using CCM.Helpers;

namespace CCM.Controllers
{
    [Authorize(Roles = "Liaison, Admin, QAQC, PhysiciansGroup, LiaisonGroup,Sales")]
    public class PatientFamilyHistoryController : BaseController
    {
        //private readonly ApplicationdbContect _db = new ApplicationdbContect();



        public async Task<ActionResult> Create(int patientId)
        {
            var patient       = _db.Patients.Find(patientId);
            var familyHistory = patient?.FamilyHistoryId != null
                              ? await _db.PatientMedicalHistory_FamilyHistories.FindAsync(patient.FamilyHistoryId)
                              : new PatientMedicalHistory_FamilyHistory { PatientId = patientId };

            ViewBag.PatientName = patient?.FirstName + " " + patient?.LastName;
            ViewBag.PatientId   = patient?.Id;
            ViewBag.CcmStatus   = patient?.CcmStatus;

            if (User.IsInRole("Liaison"))
                ViewBag.ReviewId = HelperExtensions.ReviewTimeGet("Family History", patient?.Id, User.Identity.GetUserId());

            return View(familyHistory);
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Liaison, Admin")]
        public async Task<ActionResult> Create(PatientMedicalHistory_FamilyHistory familyHistory)
        {
            if (HelperExtensions.isAllowedforEditingorAdd(familyHistory.PatientId, CategoryCycleStatusHelper.GetPatientNewOrOldCycleByCategory(familyHistory.PatientId, BillingCodeHelper.cmmBillingCatagoryid), User.Identity.GetUserId()) == false)
            {
                return RedirectToAction("Index", "CcmStatus", new { status = HelperExtensions.GetStatusRedirectionbyUser(User.Identity.GetUserId()), Message = "Cycle is locked." });
            }
            var patient = _db.Patients.Find(familyHistory.PatientId);
            if (patient != null && ModelState.IsValid)
            {
                if (patient.FamilyHistoryId != null)
                    _db.Entry(familyHistory).State = EntityState.Modified;

                else
                {
                    _db.PatientMedicalHistory_FamilyHistories.Add(familyHistory);
                    await _db.SaveChangesAsync();

                    patient.FamilyHistoryId = familyHistory.Id;
                }

                patient.UpdatedBy = User.Identity.GetUserId();
                patient.UpdatedOn = DateTime.Now;
                _db.Entry(patient).State = EntityState.Modified;
                await _db.SaveChangesAsync();

                return RedirectToAction("Create", "PatientAllergy", new { patientId = patient.Id });
            }

            ViewBag.PatientName = patient?.FirstName + " " + patient?.LastName;
            ViewBag.PatientId   = patient?.Id;
            ViewBag.CcmStatus   = patient?.CcmStatus;

            return View(familyHistory);
        }



        public async Task<PartialViewResult> _Create(int patientId)
        {
            var patient = _db.Patients.Find(patientId);
            var familyHistory = patient?.FamilyHistoryId != null
                              ? await _db.PatientMedicalHistory_FamilyHistories.FindAsync(patient.FamilyHistoryId)
                              : new PatientMedicalHistory_FamilyHistory { PatientId = patientId };

            ViewBag.PatientName = patient?.FirstName + " " + patient?.LastName;
            ViewBag.PatientId = patient?.Id;
            ViewBag.CcmStatus = patient?.CcmStatus;

            //if (User.IsInRole("Liaison"))
            //    ViewBag.ReviewId = HelperExtensions.ReviewTimeGet("Family History", patient?.Id, User.Identity.GetUserId());

            return PartialView(familyHistory);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Liaison, Admin,Sales")]
        public async Task<string> _Create(PatientMedicalHistory_FamilyHistory familyHistory)
        {
            if (HelperExtensions.isAllowedforEditingorAdd(familyHistory.PatientId, CategoryCycleStatusHelper.GetPatientNewOrOldCycleByCategory(familyHistory.PatientId, BillingCodeHelper.cmmBillingCatagoryid), User.Identity.GetUserId()) == false)
            {
                //return RedirectToAction("Index", "CcmStatus", new { status = HelperExtensions.GetStatusRedirectionbyUser(User.Identity.GetUserId()), Message = "Cycle is locked." });
                return "Cycle is locked.";
            }
            var patient = _db.Patients.Find(familyHistory.PatientId);
            if (patient != null && ModelState.IsValid)
            {
                if (patient.FamilyHistoryId != null)
                    _db.Entry(familyHistory).State = EntityState.Modified;

                else
                {
                    _db.PatientMedicalHistory_FamilyHistories.Add(familyHistory);
                    await _db.SaveChangesAsync();

                    patient.FamilyHistoryId = familyHistory.Id;
                }

                patient.UpdatedBy = User.Identity.GetUserId();
                patient.UpdatedOn = DateTime.Now;
                _db.Entry(patient).State = EntityState.Modified;
                await _db.SaveChangesAsync();

                return "True";
                //return RedirectToAction("Create", "PatientAllergy", new { patientId = patient.Id });
            }

            ViewBag.PatientName = patient?.FirstName + " " + patient?.LastName;
            ViewBag.PatientId = patient?.Id;
            ViewBag.CcmStatus = patient?.CcmStatus;

            return "False";
            //return View(familyHistory);
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