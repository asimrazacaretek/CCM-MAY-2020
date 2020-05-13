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
    public class PatientLifestyle_LifeStressController : BaseController
    {
        //private readonly ApplicationdbContect _db = new ApplicationdbContect();



        public async Task<ActionResult> Create(int patientId)
        {
            var patient = _db.Patients.Find(patientId);
            var stress  = patient?.LifeStressId != null
                        ? await _db.PatientLifestyle_LifeStresses.FindAsync(patient.LifeStressId)
                        : new PatientLifestyle_LifeStress { PatientId = patientId };

            ViewBag.Coping_StressId = new SelectList(_db.PatientLifestyle_LifeStress_CopingStresses, "Id", "Type");
            ViewBag.LifeStressId    = new SelectList(_db.PatientLifestyle_LifeStress_Stresses, "Id", "Type");
            ViewBag.workStressId    = new SelectList(_db.PatientLifestyle_LifeStress_Stresses, "Id", "Type");

            ViewBag.PatientName = patient?.FirstName + " " + patient?.LastName;
            ViewBag.PatientId   = patient?.Id;
            ViewBag.CcmStatus   = patient?.CcmStatus;

            if (User.IsInRole("Liaison"))
                ViewBag.ReviewId = HelperExtensions.ReviewTimeGet("Life Stress", patient?.Id, User.Identity.GetUserId());

            return View(stress);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Liaison, Admin")]
        public async Task<ActionResult> Create(PatientLifestyle_LifeStress lifeStress)
        {
            if (HelperExtensions.isAllowedforEditingorAdd(lifeStress.PatientId, CategoryCycleStatusHelper.GetPatientNewOrOldCycleByCategory(lifeStress.PatientId, BillingCodeHelper.cmmBillingCatagoryid), User.Identity.GetUserId()) == false)
            {
                return RedirectToAction("Index", "CcmStatus", new { status = HelperExtensions.GetStatusRedirectionbyUser(User.Identity.GetUserId()), Message = "Cycle is locked." });
            }
            var patient = _db.Patients.Find(lifeStress.PatientId);
            if (patient != null && ModelState.IsValid)
            {
                if (patient.LifeStressId != null)
                    _db.Entry(lifeStress).State = EntityState.Modified;

                else
                {
                    _db.PatientLifestyle_LifeStresses.Add(lifeStress);
                    await _db.SaveChangesAsync();

                    patient.LifeStressId = lifeStress.Id;
                }

                patient.UpdatedBy = User.Identity.GetUserId();
                patient.UpdatedOn = DateTime.Now;
                _db.Entry(patient).State = EntityState.Modified;
                await _db.SaveChangesAsync();

                return RedirectToAction("Create", "PatientLifestyle_NutritionalSupplement", new { patientId = patient?.Id });
            }

            ViewBag.Coping_StressId = new SelectList(_db.PatientLifestyle_LifeStress_CopingStresses, "Id", "Type", lifeStress.Coping_StressId);
            ViewBag.LifeStressId    = new SelectList(_db.PatientLifestyle_LifeStress_Stresses, "Id", "Type", lifeStress.LifeStressId);
            ViewBag.workStressId    = new SelectList(_db.PatientLifestyle_LifeStress_Stresses, "Id", "Type", lifeStress.workStressId);

            ViewBag.PatientName = patient?.FirstName + " " + patient?.LastName;
            ViewBag.PatientId   = patient?.Id;
            ViewBag.CcmStatus   = patient?.CcmStatus;
            
            return View(lifeStress);
        }



        public async Task<PartialViewResult> _Create(int patientId)
        {
            var patient = _db.Patients.Find(patientId);
            var stress = patient?.LifeStressId != null
                        ? await _db.PatientLifestyle_LifeStresses.FindAsync(patient.LifeStressId)
                        : new PatientLifestyle_LifeStress { PatientId = patientId };

            ViewBag.Coping_StressId = new SelectList(_db.PatientLifestyle_LifeStress_CopingStresses, "Id", "Type");
            ViewBag.LifeStressId = new SelectList(_db.PatientLifestyle_LifeStress_Stresses, "Id", "Type");
            ViewBag.workStressId = new SelectList(_db.PatientLifestyle_LifeStress_Stresses, "Id", "Type");

            ViewBag.PatientName = patient?.FirstName + " " + patient?.LastName;
            ViewBag.PatientId = patient?.Id;
            ViewBag.CcmStatus = patient?.CcmStatus;

            //if (User.IsInRole("Liaison"))
            //    ViewBag.ReviewId = HelperExtensions.ReviewTimeGet("Life Stress", patient?.Id, User.Identity.GetUserId());

            return PartialView(stress);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Liaison, Admin,Sales")]
        public async Task<string> _Create(PatientLifestyle_LifeStress lifeStress)
        {
            if (HelperExtensions.isAllowedforEditingorAdd(lifeStress.PatientId, CategoryCycleStatusHelper.GetPatientNewOrOldCycleByCategory(lifeStress.PatientId, BillingCodeHelper.cmmBillingCatagoryid), User.Identity.GetUserId()) == false)
            {
                //return RedirectToAction("Index", "CcmStatus", new { status = HelperExtensions.GetStatusRedirectionbyUser(User.Identity.GetUserId()), Message = "Cycle is locked." });
                return "Cycle is locked.";
            }
            var patient = _db.Patients.Find(lifeStress.PatientId);
            if (patient != null && ModelState.IsValid)
            {
                if (patient.LifeStressId != null)
                    _db.Entry(lifeStress).State = EntityState.Modified;

                else
                {
                    _db.PatientLifestyle_LifeStresses.Add(lifeStress);
                    await _db.SaveChangesAsync();

                    patient.LifeStressId = lifeStress.Id;
                }

                patient.UpdatedBy = User.Identity.GetUserId();
                patient.UpdatedOn = DateTime.Now;
                _db.Entry(patient).State = EntityState.Modified;
                await _db.SaveChangesAsync();

                return "True";
                //return RedirectToAction("Create", "PatientLifestyle_NutritionalSupplement", new { patientId = patient?.Id });
            }

            ViewBag.Coping_StressId = new SelectList(_db.PatientLifestyle_LifeStress_CopingStresses, "Id", "Type", lifeStress.Coping_StressId);
            ViewBag.LifeStressId = new SelectList(_db.PatientLifestyle_LifeStress_Stresses, "Id", "Type", lifeStress.LifeStressId);
            ViewBag.workStressId = new SelectList(_db.PatientLifestyle_LifeStress_Stresses, "Id", "Type", lifeStress.workStressId);

            ViewBag.PatientName = patient?.FirstName + " " + patient?.LastName;
            ViewBag.PatientId = patient?.Id;
            ViewBag.CcmStatus = patient?.CcmStatus;

            return "False";
            //return View(lifeStress);
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