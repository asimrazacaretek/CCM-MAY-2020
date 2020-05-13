using System;
using CCM.Models;
using System.Web.Mvc;
using System.Data.Entity;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using CCM.Helpers;

namespace CCM.Controllers
{
    [Authorize(Roles = "Liaison, Admin, QAQC, PhysiciansGroup, LiaisonGroup,,Sales")]
    public class PatientAllergyController : BaseController
    {
        //private readonly ApplicationdbContect _db = new ApplicationdbContect();


        public async Task<ActionResult> Create(int patientId)
        {
            var patient = _db.Patients.Find(patientId);
            var allergy = patient?.AllergyId != null
                        ? await _db.PatientMedicalHistory_Allergies.FindAsync(patient.AllergyId)
                        : new PatientMedicalHistory_Allergy { PatientId = patientId };

            ViewBag.PatientName = patient?.FirstName + " " + patient?.LastName;
            ViewBag.PatientId   = patient?.Id;
            ViewBag.CcmStatus   = patient?.CcmStatus;

            if (User.IsInRole("Liaison"))
                ViewBag.ReviewId = HelperExtensions.ReviewTimeGet("Allergies", patient?.Id, User.Identity.GetUserId());

            return View(allergy);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Liaison, Admin")]
        public async Task<ActionResult> Create(PatientMedicalHistory_Allergy allergy)
        {
            if (HelperExtensions.isAllowedforEditingorAdd(allergy.PatientId, CategoryCycleStatusHelper.GetPatientNewOrOldCycleByCategory(allergy.PatientId, BillingCodeHelper.cmmBillingCatagoryid), User.Identity.GetUserId()) == false)
            {
                return RedirectToAction("Index", "CcmStatus", new { status = HelperExtensions.GetStatusRedirectionbyUser(User.Identity.GetUserId()), Message = "Cycle is locked." });
            }
            var patient  = _db.Patients.Find(allergy.PatientId);
            if (patient != null && ModelState.IsValid)
            {
                if (patient.AllergyId != null)
                    _db.Entry(allergy).State = EntityState.Modified;

                else
                {
                    _db.PatientMedicalHistory_Allergies.Add(allergy);
                    await _db.SaveChangesAsync();

                    patient.AllergyId = allergy.Id;
                }

                patient.UpdatedBy = User.Identity.GetUserId();
                patient.UpdatedOn = DateTime.Now;
                _db.Entry(patient).State = EntityState.Modified;
                await _db.SaveChangesAsync();

                return RedirectToAction("Patient", "CurrentMedication", new { patientId = patient.Id });
            }

            ViewBag.PatientName = patient?.FirstName + " " + patient?.LastName;
            ViewBag.PatientId   = patient?.Id;
            ViewBag.CcmStatus   = patient?.CcmStatus;

            return View(allergy);
        }



        public async Task<PartialViewResult> _Create(int patientId)
        {
            var patient = _db.Patients.Find(patientId);
            var allergy = patient?.AllergyId != null
                        ? await _db.PatientMedicalHistory_Allergies.FindAsync(patient.AllergyId)
                        : new PatientMedicalHistory_Allergy { PatientId = patientId };

            ViewBag.PatientName = patient?.FirstName + " " + patient?.LastName;
            ViewBag.PatientId = patient?.Id;
            ViewBag.CcmStatus = patient?.CcmStatus;

            //if (User.IsInRole("Liaison"))
            //    ViewBag.ReviewId = HelperExtensions.ReviewTimeGet("Allergies", patient?.Id, User.Identity.GetUserId());

            return PartialView(allergy);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Liaison, Admin,Sales")]
        public async Task<string> _Create(PatientMedicalHistory_Allergy allergy)
        {
            if (HelperExtensions.isAllowedforEditingorAdd(allergy.PatientId, CategoryCycleStatusHelper.GetPatientNewOrOldCycleByCategory(allergy.PatientId, BillingCodeHelper.cmmBillingCatagoryid), User.Identity.GetUserId()) == false)
            {
                
                return "Cycle is locked.";
            }
            var patient = _db.Patients.Find(allergy.PatientId);
            if (patient != null && ModelState.IsValid)
            {
                if (patient.AllergyId != null)
                    _db.Entry(allergy).State = EntityState.Modified;

                else
                {
                    _db.PatientMedicalHistory_Allergies.Add(allergy);
                    await _db.SaveChangesAsync();

                    patient.AllergyId = allergy.Id;
                }

                patient.UpdatedBy = User.Identity.GetUserId();
                patient.UpdatedOn = DateTime.Now;
                _db.Entry(patient).State = EntityState.Modified;
                await _db.SaveChangesAsync();

                return "True";
              
            }

            ViewBag.PatientName = patient?.FirstName + " " + patient?.LastName;
            ViewBag.PatientId = patient?.Id;
            ViewBag.CcmStatus = patient?.CcmStatus;

            return "False";
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