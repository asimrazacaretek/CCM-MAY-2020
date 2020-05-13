using System;
using CCM.Models;
using System.Linq;
using System.Web.Mvc;
using System.Data.Entity;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using CCM.Helpers;

namespace CCM.Controllers
{
    [Authorize(Roles = "Liaison, Admin, QAQC, PhysiciansGroup, LiaisonGroup,Sales")]
    public class DoctorVisitController : BaseController
    {
        //private readonly ApplicationdbContect _db = new ApplicationdbContect();
     
        public async Task<ActionResult> Create(int patientId)
        {
            var patient = await _db.Patients.FindAsync(patientId);
            ViewBag.PatientName = patient?.FirstName + " " + patient?.LastName;
            ViewBag.PatientId = patient.Id;
            if (User.IsInRole("Liaison"))
                ViewBag.ReviewId = HelperExtensions.ReviewTimeGet("Doctor Visits", patient.Id, User.Identity.GetUserId());
            return View(new DoctorVisit { PatientId = patientId });
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Liaison, Admin")]
        public async Task<ActionResult> Create(DoctorVisit visit)
        {
            
            if (HelperExtensions.isAllowedforEditingorAdd(visit.PatientId, CategoryCycleStatusHelper.GetPatientNewOrOldCycleByCategory(visit.PatientId, BillingCodeHelper.cmmBillingCatagoryid), User.Identity.GetUserId()) == false)
            {
                return RedirectToAction("Index", "CcmStatus", new { status = HelperExtensions.GetStatusRedirectionbyUser(User.Identity.GetUserId()), Message = "Cycle is locked." });

            }
            var patient = await _db.Patients.FindAsync(visit.PatientId);
            ViewBag.PatientName = patient?.FirstName + " " + patient?.LastName;

            if (patient != null)
            {
                _db.DoctorVisits.Add(visit);

                patient.UpdatedBy = User.Identity.GetUserId();
                patient.UpdatedOn = DateTime.Now;

                _db.Entry(patient).State = EntityState.Modified;
                await _db.SaveChangesAsync();

                return RedirectToAction("ListDoctorVisits", new { patientId = visit.PatientId });
            }

            return View(new DoctorVisit { PatientId = visit.PatientId });
        }


        public async Task<PartialViewResult> _Create(int patientId)
        {
            var patient = await _db.Patients.FindAsync(patientId);
            ViewBag.PatientName = patient?.FirstName + " " + patient?.LastName;
            ViewBag.PatientId = patient.Id;

            ViewBag.AdditionalProviders = new SelectList(_db.SecondaryDoctors.Where(x=>x.PatientId == patientId).ToList(), "Id", "FullName");

            //if (User.IsInRole("Liaison"))
            //    ViewBag.ReviewId = HelperExtensions.ReviewTimeGet("Doctor Visits", patient.Id, User.Identity.GetUserId());
            return PartialView(new DoctorVisit { PatientId = patientId });
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Liaison, Admin,Sales")]
        public async Task<string> _Create(DoctorVisit visit)
        {
            try
            {
                if (HelperExtensions.isAllowedforEditingorAdd(visit.PatientId, CategoryCycleStatusHelper.GetPatientNewOrOldCycleByCategory(visit.PatientId,BillingCodeHelper.cmmBillingCatagoryid), User.Identity.GetUserId()) == false)
                {
                    //return RedirectToAction("Index", "CcmStatus", new { status = HelperExtensions.GetStatusRedirectionbyUser(User.Identity.GetUserId()), Message = "Cycle is locked." });
                    return "Cycle is locked.";

                }
                var patient = await _db.Patients.FindAsync(visit.PatientId);
                ViewBag.PatientName = patient?.FirstName + " " + patient?.LastName;

                if (patient != null)
                {
                    int a = Convert.ToInt32(visit.AdditionalProviders);
                    var additonal = _db.SecondaryDoctors.AsNoTracking().Where(x => x.Id == a).FirstOrDefault()?.FullName;
                    visit.AdditionalProviders = additonal;
                    _db.DoctorVisits.Add(visit);

                    patient.UpdatedBy = User.Identity.GetUserId();
                    patient.UpdatedOn = DateTime.Now;

                    _db.Entry(patient).State = EntityState.Modified;
                    await _db.SaveChangesAsync();

                    return "True";
                }

                return "False";
            }
            catch(Exception ex) { return ex.Message; }
            //return View(new DoctorVisit { PatientId = visit.PatientId });
        }

        
        public async Task<ActionResult> ListDoctorVisits(int? patientId)
        {
            var patient = await _db.Patients.FindAsync(patientId);

            ViewBag.PatientId   = patient?.Id;
            ViewBag.PatientName = patient?.FirstName + " " + patient?.LastName;
            if (User.IsInRole("Liaison"))
                ViewBag.ReviewId = HelperExtensions.ReviewTimeGet("Doctor Visits", patient.Id, User.Identity.GetUserId());
            return View(await _db.DoctorVisits.Where(v => v.PatientId == patientId).ToListAsync());
        }

        public async Task<PartialViewResult> _ListDoctorVisits(int? patientId)
        {
            var patient = await _db.Patients.FindAsync(patientId);

            ViewBag.PatientId = patient?.Id;
            ViewBag.PatientName = patient?.FirstName + " " + patient?.LastName;
            //if (User.IsInRole("Liaison"))
            //    ViewBag.ReviewId = HelperExtensions.ReviewTimeGet("Doctor Visits", patient.Id, User.Identity.GetUserId());
            return PartialView(await _db.DoctorVisits.Where(v => v.PatientId == patientId).ToListAsync());
        }

        public async Task<PartialViewResult> _DoctorVisitAndAditionalProvider(int? patientId)
        {
            return PartialView(await _db.DoctorVisits.Where(v => v.PatientId == patientId).ToListAsync());
        }

        public async Task<PartialViewResult> _PatientProfileConsent(int? patientId)
        {
            return PartialView(await _db.DoctorVisits.Where(v => v.PatientId == patientId).ToListAsync());
        }

        [Authorize(Roles = "Liaison, Admin")]
        public async Task<ActionResult> Delete(int id)
        {
            var visit  = await _db.DoctorVisits.FindAsync(id);
            if (visit != null)
            {
                _db.DoctorVisits.Remove(visit);

                var patient  = await _db.Patients.FindAsync(visit.PatientId);
                if (patient != null)
                {
                    patient.UpdatedOn = DateTime.Now;
                    patient.UpdatedBy = User.Identity.GetUserId();
                }

                await _db.SaveChangesAsync();

                return RedirectToAction("ListDoctorVisits", new { patientId = visit.PatientId });
            }

            ViewBag.Message = "Doctor's Visit Record Not Found!";
            return View("Error");
        }

        [Authorize(Roles = "Liaison, Admin,Sales" +
            "")]
        public async Task<string> _Delete(int id)
        {
            var visit = await _db.DoctorVisits.FindAsync(id);
            if (visit != null)
            {
                _db.DoctorVisits.Remove(visit);

                var patient = await _db.Patients.FindAsync(visit.PatientId);
                if (patient != null)
                {
                    patient.UpdatedOn = DateTime.Now;
                    patient.UpdatedBy = User.Identity.GetUserId();
                }

                await _db.SaveChangesAsync();
                return "True";
                //return PartialView("_ListDoctorVisits", new { patientId = visit.PatientId });
            }

            ViewBag.Message = "Doctor's Visit Record Not Found!";
            return "Doctor's Visit Record Not Found!";
            //return PartialView("_Error");
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