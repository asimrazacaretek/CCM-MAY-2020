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
    public class PatientLifestyle_DietAndHabitController : BaseController
    {
        //private readonly ApplicationdbContect _db = new ApplicationdbContect();


        public async Task<ActionResult> Create(int patientId)
        {
            var patient   = _db.Patients.Find(patientId);
            var dietHabit = patient?.DietAndHabitId != null
                          ? await _db.PatientLifestyle_DietAndHabits.FindAsync(patient.DietAndHabitId)
                          : new PatientLifestyle_DietAndHabit { PatientId = patientId };

            ViewBag.AlcoholId   = new SelectList(_db.PatientLifestyle_DietAndHabit_AlcoholFrequencies, "id", "Type");
            ViewBag.PatientName = patient?.FirstName + " " + patient?.LastName;
            ViewBag.PatientId   = patient?.Id;
            ViewBag.CcmStatus   = patient?.CcmStatus;

            if (User.IsInRole("Liaison"))
                ViewBag.ReviewId = HelperExtensions.ReviewTimeGet("Diet & Habits", patient?.Id, User.Identity.GetUserId());

            return View(dietHabit);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Liaison, Admin")]
        public async Task<ActionResult> Create(PatientLifestyle_DietAndHabit dietHabit)
        {
            if (HelperExtensions.isAllowedforEditingorAdd(dietHabit.PatientId, CategoryCycleStatusHelper.GetPatientNewOrOldCycleByCategory(dietHabit.PatientId, BillingCodeHelper.cmmBillingCatagoryid), User.Identity.GetUserId()) == false)
            {
                return RedirectToAction("Index", "CcmStatus", new { status = HelperExtensions.GetStatusRedirectionbyUser(User.Identity.GetUserId()), Message = "Cycle is locked." });
            }
            var patient  = _db.Patients.Find(dietHabit.PatientId);
            if (patient != null && ModelState.IsValid)
            {
                if (patient.DietAndHabitId != null)
                    _db.Entry(dietHabit).State = EntityState.Modified;

                else
                {
                    _db.PatientLifestyle_DietAndHabits.Add(dietHabit);
                    await _db.SaveChangesAsync();

                    patient.DietAndHabitId = dietHabit.Id;
                }

                patient.UpdatedBy = User.Identity.GetUserId();
                patient.UpdatedOn = DateTime.Now;
                _db.Entry(patient).State = EntityState.Modified;
                await _db.SaveChangesAsync();

                return RedirectToAction("Create", "PatientLifestyle_LifeStress", new { patientId = patient.Id });
            }

            ViewBag.AlcoholId   = new SelectList(_db.PatientLifestyle_DietAndHabit_AlcoholFrequencies, "id", "Type", dietHabit.AlcoholId);
            ViewBag.PatientName = patient?.FirstName + " " + patient?.LastName;
            ViewBag.PatientId   = patient?.Id;
            ViewBag.CcmStatus   = patient?.CcmStatus;

            return View(dietHabit);
        }



        public async Task<PartialViewResult> _Create(int patientId)
        {
            var patient = _db.Patients.Find(patientId);
            var dietHabit = patient?.DietAndHabitId != null
                          ? await _db.PatientLifestyle_DietAndHabits.FindAsync(patient.DietAndHabitId)
                          : new PatientLifestyle_DietAndHabit { PatientId = patientId };

            ViewBag.AlcoholId = new SelectList(_db.PatientLifestyle_DietAndHabit_AlcoholFrequencies, "id", "Type");
            ViewBag.PatientName = patient?.FirstName + " " + patient?.LastName;
            ViewBag.PatientId = patient?.Id;
            ViewBag.CcmStatus = patient?.CcmStatus;

            //if (User.IsInRole("Liaison"))
            //    ViewBag.ReviewId = HelperExtensions.ReviewTimeGet("Diet & Habits", patient?.Id, User.Identity.GetUserId());

            return PartialView(dietHabit);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Liaison, Admin,Sales")]
        public async Task<string> _Create(PatientLifestyle_DietAndHabit dietHabit)
        {
            if (HelperExtensions.isAllowedforEditingorAdd(dietHabit.PatientId, CategoryCycleStatusHelper.GetPatientNewOrOldCycleByCategory(dietHabit.PatientId, BillingCodeHelper.cmmBillingCatagoryid), User.Identity.GetUserId()) == false)
            {
                return "Cycle is locked.";
            }
            var patient = _db.Patients.Find(dietHabit.PatientId);
            if (patient != null && ModelState.IsValid)
            {
                if (patient.DietAndHabitId != null)
                    _db.Entry(dietHabit).State = EntityState.Modified;

                else
                {
                    _db.PatientLifestyle_DietAndHabits.Add(dietHabit);
                    await _db.SaveChangesAsync();

                    patient.DietAndHabitId = dietHabit.Id;
                }

                patient.UpdatedBy = User.Identity.GetUserId();
                patient.UpdatedOn = DateTime.Now;
                _db.Entry(patient).State = EntityState.Modified;
                await _db.SaveChangesAsync();

                return "True";
            }

            ViewBag.AlcoholId = new SelectList(_db.PatientLifestyle_DietAndHabit_AlcoholFrequencies, "id", "Type", dietHabit.AlcoholId);
            ViewBag.PatientName = patient?.FirstName + " " + patient?.LastName;
            ViewBag.PatientId = patient?.Id;
            ViewBag.CcmStatus = patient?.CcmStatus;

            return "False";
        }

        public async Task<PartialViewResult> _LifeStyleAndHabits(int patientId)
        {
            var patient = _db.Patients.Find(patientId);
            var dietHabit = patient?.DietAndHabitId != null
                          ? await _db.PatientLifestyle_DietAndHabits.FindAsync(patient.DietAndHabitId)
                          : new PatientLifestyle_DietAndHabit { PatientId = patientId };

            return PartialView(dietHabit);
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