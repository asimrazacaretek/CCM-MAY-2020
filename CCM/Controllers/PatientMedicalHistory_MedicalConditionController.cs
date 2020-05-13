using System;
using CCM.Models;
using System.Web.Mvc;
using System.Data.Entity;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using System.Linq;
using CCM.Helpers;

namespace CCM.Controllers
{
    [Authorize(Roles = "Liaison, Admin, QAQC, PhysiciansGroup, LiaisonGroup,Sales")]
    public class PatientMedicalHistory_MedicalConditionController : BaseController
    {
       // private readonly ApplicationdbContect _db = new ApplicationdbContect();


        public async Task<ActionResult> Create(int patientId)
        {
            var patient = _db.Patients.Find(patientId);
            var medicalCondition = patient?.MedicalConditionId != null
                                 ? await _db.PatientMedicalHistory_MedicalConditions.FindAsync(patient.MedicalConditionId)
                                 : new PatientMedicalHistory_MedicalCondition { PatientId = patientId };

            ViewBag.PatientName = patient?.FirstName + " " + patient?.LastName;
            ViewBag.PatientId   = patient?.Id;
            ViewBag.CcmStatus   = patient?.CcmStatus;

            if (User.IsInRole("Liaison"))
                ViewBag.ReviewId = HelperExtensions.ReviewTimeGet("Medical Conditions", patient?.Id, User.Identity.GetUserId());

            return View(medicalCondition);
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Liaison, Admin")]
        public async Task<ActionResult> Create(PatientMedicalHistory_MedicalCondition medicalCondition)
        {
            if (HelperExtensions.isAllowedforEditingorAdd(medicalCondition.PatientId, CategoryCycleStatusHelper.GetPatientNewOrOldCycleByCategory(medicalCondition.PatientId, BillingCodeHelper.cmmBillingCatagoryid), User.Identity.GetUserId()) == false)
            {
                return RedirectToAction("Index", "CcmStatus", new { status = HelperExtensions.GetStatusRedirectionbyUser(User.Identity.GetUserId()), Message = "Cycle is locked." });
            }
            var patient = _db.Patients.Find(medicalCondition.PatientId);
            if (patient != null && ModelState.IsValid)
            {
                if (patient.MedicalConditionId != null)
                    _db.Entry(medicalCondition).State = EntityState.Modified;

                else
                {
                    _db.PatientMedicalHistory_MedicalConditions.Add(medicalCondition);
                    await _db.SaveChangesAsync();

                    patient.MedicalConditionId = medicalCondition.Id;
                }

                patient.UpdatedBy = User.Identity.GetUserId();
                patient.UpdatedOn = DateTime.Now;
                _db.Entry(patient).State = EntityState.Modified;
                await _db.SaveChangesAsync();

                return RedirectToAction("Create", "PatientFamilyHistory", new { patientId = patient.Id });

            }

            ViewBag.PatientName = patient?.FirstName + " " + patient?.LastName;
            ViewBag.PatientId   = patient?.Id;
            ViewBag.CcmStatus   = patient?.CcmStatus;

            return View(medicalCondition);

        }



        public async Task<PartialViewResult> _Create(int patientId)
        {
            var patient = _db.Patients.Find(patientId);
            var medicalCondition = patient?.MedicalConditionId != null
                                 ? await _db.PatientMedicalHistory_MedicalConditions.FindAsync(patient.MedicalConditionId)
                                 : new PatientMedicalHistory_MedicalCondition { PatientId = patientId };

            ViewBag.PatientName = patient?.FirstName + " " + patient?.LastName;
            ViewBag.PatientId = patient?.Id;
            ViewBag.CcmStatus = patient?.CcmStatus;

            //if (User.IsInRole("Liaison"))
            //    ViewBag.ReviewId = HelperExtensions.ReviewTimeGet("Medical Conditions", patient?.Id, User.Identity.GetUserId());

            return PartialView(medicalCondition);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Liaison, Admin,Sales")]
        public async Task<string> _Create(PatientMedicalHistory_MedicalCondition medicalCondition)
        {
            if (HelperExtensions.isAllowedforEditingorAdd(medicalCondition.PatientId, CategoryCycleStatusHelper.GetPatientNewOrOldCycleByCategory(medicalCondition.PatientId, BillingCodeHelper.cmmBillingCatagoryid), User.Identity.GetUserId()) == false)
            {
                return "Cycle is locked.";
            }
            var patient = _db.Patients.Find(medicalCondition.PatientId);
            if (patient != null && ModelState.IsValid)
            {
                if (patient.MedicalConditionId != null)
                    _db.Entry(medicalCondition).State = EntityState.Modified;

                else
                {
                    _db.PatientMedicalHistory_MedicalConditions.Add(medicalCondition);
                    await _db.SaveChangesAsync();

                    patient.MedicalConditionId = medicalCondition.Id;
                }

                patient.UpdatedBy = User.Identity.GetUserId();
                patient.UpdatedOn = DateTime.Now;
                _db.Entry(patient).State = EntityState.Modified;
                await _db.SaveChangesAsync();
                return "True";
            }
            else
            {
                var errorList = ModelState.Values.SelectMany(m => m.Errors)
                                 .Select(e => e.ErrorMessage)
                                 .ToList();
                var errorstr = string.Join(",", errorList);
                return errorstr;
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
