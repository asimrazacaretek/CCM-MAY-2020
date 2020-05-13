using System;
using CCM.Models;
using System.Web.Mvc;
using System.Data.Entity;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using System.Data.Entity.Migrations;
using System.Linq;
using CCM.Helpers;

namespace CCM.Controllers
{
    [Authorize(Roles = "Liaison, Admin, QAQC")]
    public class CarePlanController : BaseController
    {
        
        public async Task<ActionResult> Create(int id)
        {
            var patient = _db.Patients.Find(id);
            if (patient == null)
            {
                ViewBag.message = "Patient Not Found.";
                return View("Error");
            }

            var physician = await _db.Physicians.FindAsync(patient.PhysicianId);
            ViewBag.PhysicianName = physician?.FirstName + " " + physician?.LastName;
            ViewBag.PatientName = patient.FirstName + " " + patient.LastName;
            ViewBag.PatientId = patient.Id;
            ViewBag.CcmStatus = patient.CcmStatus;

            if (User.IsInRole("Liaison"))
                ViewBag.ReviewId = HelperExtensions.ReviewTimeGet("Care Plan", patient.Id, User.Identity.GetUserId());

            return View( await _db.CarePlans.FirstOrDefaultAsync(cp => cp.PatientId == id) 
                      ?? new CarePlan { PatientId = id/*, Cycle = patient.Cycle*/ });
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Liaison, Admin")]
        public async Task<ActionResult> Create(CarePlan carePlan)
        {
            try
            {


                CategoryCycleStatusHelper.User = User;
                if (HelperExtensions.isAllowedforEditingorAdd(carePlan.PatientId, CategoryCycleStatusHelper.GetPatientNewOrOldCycleByCategory(carePlan.PatientId,BillingCodeHelper.cmmBillingCatagoryid), User.Identity.GetUserId()) == false)
                {
                    return RedirectToAction("Index", "CcmStatus", new { status = HelperExtensions.GetStatusRedirectionbyUser(User.Identity.GetUserId()), Message = "Cycle is locked." });

                }
                var patient = await _db.Patients.FirstOrDefaultAsync(p => p.Id == carePlan.PatientId);
                if (patient != null && ModelState.IsValid)
                {
                    _db.Set<CarePlan>().AddOrUpdate(carePlan);

                    patient.UpdatedBy = User.Identity.GetUserId();
                    patient.UpdatedOn = DateTime.Now;
                    _db.Entry(patient).State = EntityState.Modified;
                    await _db.SaveChangesAsync();
                }

                var physician = await _db.Physicians.FindAsync(patient?.PhysicianId);
                ViewBag.PhysicianName = physician?.FirstName + ' ' + physician?.LastName;
                ViewBag.PatientName = patient?.FirstName + ' ' + patient?.LastName;
                ViewBag.PatientId = patient?.Id;
                ViewBag.CcmStatus = patient?.CcmStatus;

                return View(carePlan);
            }
            catch (Exception ex)
            {
                CarePlan carePlan1 = new CarePlan();
                ViewBag.PhysicianName = "Error";
                ViewBag.PatientName = "Error";
                ViewBag.PatientId = 0;
                ViewBag.CcmStatus = "Error";
                log.Error(Environment.NewLine + User.Identity.GetUserName() + "-------" + User.Identity.GetUserId() + Environment.NewLine + ex.Message + "-----" + ex.StackTrace);
                return View(carePlan1);
                /*return ex.Message + "------------------" + ex.StackTrace;*/
            }
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