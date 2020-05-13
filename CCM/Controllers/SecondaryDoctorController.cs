using CCM.Models;
using System.Linq;
using System.Web.Mvc;
using System.Data.Entity;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using System;
using CCM.Helpers;

namespace CCM.Controllers
{
    public class SecondaryDoctorController : BaseController
    {
        //private readonly ApplicationdbContect _db = new ApplicationdbContect();

        public async Task<ActionResult> Index(int patientId)
        {
            var patient = await _db.Patients.FindAsync(patientId);
            ViewBag.PatientId = patient?.Id;
            ViewBag.PatientName = patient?.FirstName + " " + patient?.LastName;
            ViewBag.PrimaryPhysician = "";
            
            if (User.IsInRole("Liaison"))
                ViewBag.ReviewId = HelperExtensions.ReviewTimeGet("Secondary Doctors", patient.Id, User.Identity.GetUserId());
            if (patient.PhysicianId != null)
            {
                var physcian = _db.Physicians.Where(x => x.Id == patient.PhysicianId).FirstOrDefault();
                ViewBag.PrimaryPhysician = physcian?.FirstName + " " + physcian?.LastName;

            }

            return View(await _db.SecondaryDoctors.AsNoTracking().Where(sd => sd.PatientId == patientId).ToListAsync());
        }

        public async Task<PartialViewResult> _Index(int patientId)
        {
            var patient = await _db.Patients.FindAsync(patientId);
            ViewBag.PatientId = patient?.Id;
            ViewBag.PatientName = patient?.FirstName + " " + patient?.LastName;
            ViewBag.PrimaryPhysician = "";

            //if (User.IsInRole("Liaison"))
            //    ViewBag.ReviewId = HelperExtensions.ReviewTimeGet("Secondary Doctors", patient.Id, User.Identity.GetUserId());
            if (patient.PhysicianId != null)
            {
                var physcian = _db.Physicians.Where(x => x.Id == patient.PhysicianId).FirstOrDefault();
                ViewBag.PrimaryPhysician = physcian?.FirstName + " " + physcian?.LastName;

            }

            return PartialView(await _db.SecondaryDoctors.AsNoTracking().Where(sd => sd.PatientId == patientId).ToListAsync());
        }


        public async Task<ActionResult> Create(int patientId)
        {
            var patient = await _db.Patients.FindAsync(patientId);
            ViewBag.PatientName = patient?.FirstName + " " + patient?.LastName;
            ViewBag.PatientId = patient.Id;
            if (User.IsInRole("Liaison"))
                ViewBag.ReviewId = HelperExtensions.ReviewTimeGet("Secondary Doctors", patient.Id, User.Identity.GetUserId());

            return View(new SecondaryDoctor { PatientId = patientId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Liaison, Admin")]
        public async Task<ActionResult> Create(SecondaryDoctor secondaryDoctor)
        {
            if (HelperExtensions.isAllowedforEditingorAdd(secondaryDoctor.PatientId, CategoryCycleStatusHelper.GetPatientNewOrOldCycleByCategory(secondaryDoctor.PatientId, BillingCodeHelper.cmmBillingCatagoryid), User.Identity.GetUserId()) == false)
            {
                return RedirectToAction("Index", "CcmStatus", new { status = HelperExtensions.GetStatusRedirectionbyUser(User.Identity.GetUserId()), Message = "Cycle is locked." });

            }
            if (ModelState.IsValid)
            {
                _db.SecondaryDoctors.Add(secondaryDoctor);
                await _db.SaveChangesAsync();
                return RedirectToAction("Index", new { patientId = secondaryDoctor.PatientId });
            }

            return View(secondaryDoctor);
        }

        public async Task<PartialViewResult> _Create(int patientId)
        {
            var patient = await _db.Patients.FindAsync(patientId);
            ViewBag.PatientName = patient?.FirstName + " " + patient?.LastName;
            ViewBag.PatientId = patient.Id;
            //if (User.IsInRole("Liaison"))
            //    ViewBag.ReviewId = HelperExtensions.ReviewTimeGet("Secondary Doctors", patient.Id, User.Identity.GetUserId());

            return PartialView(new SecondaryDoctor { PatientId = patientId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Liaison, Admin,Sales")]
        public async Task<string> _Create(SecondaryDoctor secondaryDoctor)
        {
            if (HelperExtensions.isAllowedforEditingorAdd(secondaryDoctor.PatientId, CategoryCycleStatusHelper.GetPatientNewOrOldCycleByCategory(secondaryDoctor.PatientId,BillingCodeHelper.cmmBillingCatagoryid), User.Identity.GetUserId()) == false)
            {
                //return RedirectToAction("Index", "CcmStatus", new { status = HelperExtensions.GetStatusRedirectionbyUser(User.Identity.GetUserId()), Message = "Cycle is locked." });
                return "Cycle is locked.";
            }
            if (ModelState.IsValid)
            {
                secondaryDoctor.CreatedOn = DateTime.Now;
                secondaryDoctor.CreatedBy = User.Identity.GetUserId();
                secondaryDoctor.Status = true;


                _db.SecondaryDoctors.Add(secondaryDoctor);
                await _db.SaveChangesAsync();
                return "True";
                //return RedirectToAction("Index", new { patientId = secondaryDoctor.PatientId });
            }

            return "False";
            //return View(secondaryDoctor);
        }

        [Authorize(Roles = "Liaison, Admin")]
        public async Task<ActionResult> Delete(int? id)
        {
            var secondaryDoctor = await _db.SecondaryDoctors.FindAsync(id);
            if (secondaryDoctor != null)
            {
                _db.SecondaryDoctors.Remove(secondaryDoctor);
                await _db.SaveChangesAsync();
            }

            return RedirectToAction("Index", new { patientId = secondaryDoctor?.PatientId });
        }

        [Authorize(Roles = "Liaison, Admin,Sales")]
        public async Task<string> _Delete(int? id)
        {
            var secondaryDoctor = await _db.SecondaryDoctors.FindAsync(id);
            if (secondaryDoctor != null)
            {
                _db.SecondaryDoctors.Remove(secondaryDoctor);
                await _db.SaveChangesAsync();
                return "True";
            }
            else
                return "False";
            //return RedirectToAction("Index", new { patientId = secondaryDoctor?.PatientId });
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