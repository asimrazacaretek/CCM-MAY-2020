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
    public class PatientLifestyle_NutritionalSupplementController : BaseController
    {
        //private readonly ApplicationdbContect _db = new ApplicationdbContect();



        public async Task<ActionResult> Create(int patientId)
        {
            var patient     = _db.Patients.Find(patientId);
            var supplements = patient?.NutritionalSupplementId != null
                            ? await _db.PatientLifestyle_NutritionalSupplements.FindAsync(patient.NutritionalSupplementId)
                            : new PatientLifestyle_NutritionalSupplement { PatientId = patientId };

            ViewBag.PatientName = patient?.FirstName + " " + patient?.LastName;
            ViewBag.PatientId   = patient?.Id;
            ViewBag.CcmStatus   = patient?.CcmStatus;

            if (User.IsInRole("Liaison"))
                ViewBag.ReviewId = HelperExtensions.ReviewTimeGet("Nutritional Supplements", patient?.Id, User.Identity.GetUserId());

            return View(supplements);
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Liaison, Admin")]
        public async Task<ActionResult> Create(PatientLifestyle_NutritionalSupplement supplement)
        {
            if (HelperExtensions.isAllowedforEditingorAdd(supplement.PatientId, CategoryCycleStatusHelper.GetPatientNewOrOldCycleByCategory(supplement.PatientId, BillingCodeHelper.cmmBillingCatagoryid), User.Identity.GetUserId()) == false)
            {
                return RedirectToAction("Index", "CcmStatus", new { status = HelperExtensions.GetStatusRedirectionbyUser(User.Identity.GetUserId()), Message = "Cycle is locked." });
            }
            var patient = await _db.Patients.FindAsync(supplement.PatientId);
            if (ModelState.IsValid && patient != null)
            {
                if (patient.NutritionalSupplementId != null)
                    _db.Entry(supplement).State = EntityState.Modified;

                else
                {
                    _db.PatientLifestyle_NutritionalSupplements.Add(supplement);
                    await _db.SaveChangesAsync();

                    patient.NutritionalSupplementId = supplement.Id;
                }

                patient.UpdatedOn = DateTime.Now;
                patient.UpdatedBy = User.Identity.GetUserId();
                patient.CcmStatus = "Claims Submission";
                patient.CcmClaimSubmissionDate = DateTime.Now;
                _db.Entry(patient).State = EntityState.Modified;
                await _db.SaveChangesAsync();


                return RedirectToAction("Index", "CcmStatus", new { status = "Enrolled" });
            }

            ViewBag.PatientName = patient?.FirstName + " " + patient?.LastName;
            ViewBag.PatientId   = patient?.Id;
            ViewBag.CcmStatus   = patient?.CcmStatus;

            return View(supplement);
        }




        public async Task<PartialViewResult> _Create(int patientId)
        {
            var patient = _db.Patients.Find(patientId);
            var supplements = patient?.NutritionalSupplementId != null
                            ? await _db.PatientLifestyle_NutritionalSupplements.FindAsync(patient.NutritionalSupplementId)
                            : new PatientLifestyle_NutritionalSupplement { PatientId = patientId };

            ViewBag.PatientName = patient?.FirstName + " " + patient?.LastName;
            ViewBag.PatientId = patient?.Id;
            ViewBag.CcmStatus = patient?.CcmStatus;

            //if (User.IsInRole("Liaison"))
            //    ViewBag.ReviewId = HelperExtensions.ReviewTimeGet("Nutritional Supplements", patient?.Id, User.Identity.GetUserId());

            return PartialView(supplements);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Liaison, Admin,Sales")]
        public async Task<string> _Create(PatientLifestyle_NutritionalSupplement supplement)
        {
            if (HelperExtensions.isAllowedforEditingorAdd(supplement.PatientId, CategoryCycleStatusHelper.GetPatientNewOrOldCycleByCategory(supplement.PatientId, BillingCodeHelper.cmmBillingCatagoryid), User.Identity.GetUserId()) == false)
            {
                //return RedirectToAction("Index", "CcmStatus", new { status = HelperExtensions.GetStatusRedirectionbyUser(User.Identity.GetUserId()), Message = "Cycle is locked." });

                return "Cycle is locked.";
            }
            var patient = await _db.Patients.FindAsync(supplement.PatientId);
            if (ModelState.IsValid && patient != null)
            {
                if (patient.NutritionalSupplementId != null)
                    _db.Entry(supplement).State = EntityState.Modified;

                else
                {
                    _db.PatientLifestyle_NutritionalSupplements.Add(supplement);
                    await _db.SaveChangesAsync();

                    patient.NutritionalSupplementId = supplement.Id;
                }

                patient.UpdatedOn = DateTime.Now;
                patient.UpdatedBy = User.Identity.GetUserId();
                patient.CcmStatus = "Claims Submission";
                patient.CcmClaimSubmissionDate = DateTime.Now;
                _db.Entry(patient).State = EntityState.Modified;
                await _db.SaveChangesAsync();

                return "True";
                //return RedirectToAction("Index", "CcmStatus", new { status = "Enrolled" });
            }

            ViewBag.PatientName = patient?.FirstName + " " + patient?.LastName;
            ViewBag.PatientId = patient?.Id;
            ViewBag.CcmStatus = patient?.CcmStatus;

            return "False";
            //return View(supplement);
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