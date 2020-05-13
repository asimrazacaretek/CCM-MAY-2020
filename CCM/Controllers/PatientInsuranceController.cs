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
    public class PatientInsuranceController : BaseController
    {
       // private readonly ApplicationdbContect _db = new ApplicationdbContect();


        public async Task<ActionResult> Create(int patientId)
        {
            var patient   = _db.Patients.Find(patientId);
            var insurance = patient?.InsuranceId != null
                          ? await _db.PatientProfile_Insurance.FindAsync(patient.InsuranceId)
                          : new PatientProfile_Insurance { PatientId = patientId };

            ViewBag.PatientName = patient?.FirstName + " " + patient?.LastName;
            ViewBag.PatientId   = patient?.Id;
            ViewBag.CcmStatus   = patient?.CcmStatus;
            ViewBag.MedicareID = patient?.MedicareIdNumber;
            ViewBag.MedicaidID = patient?.MedicaidIdNumber;
            ViewBag.OtherInsuranceID = patient?.OtherInsuranceIdNumber;

            if (User.IsInRole("Liaison"))
                ViewBag.ReviewId = HelperExtensions.ReviewTimeGet("Insurance", patient?.Id, User.Identity.GetUserId());

            return View(insurance);
        }
            
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Liaison, Admin")]
        public async Task<ActionResult> Create(PatientProfile_Insurance insurance,string MedicareIdNumber,string MedicaidIdNumber,string OtherInsuranceIdNumber)
        {
            if (HelperExtensions.isAllowedforEditingorAdd(insurance.PatientId, CategoryCycleStatusHelper.GetPatientNewOrOldCycleByCategory(insurance.PatientId, BillingCodeHelper.cmmBillingCatagoryid), User.Identity.GetUserId()) == false)
            {
                return RedirectToAction("Index", "CcmStatus", new { status = HelperExtensions.GetStatusRedirectionbyUser(User.Identity.GetUserId()), Message = "Cycle is locked." });

            }
            var patient = _db.Patients.Find(insurance.PatientId);
            if (patient != null && ModelState.IsValid)
            {
                if (patient.InsuranceId != null)
                    _db.Entry(insurance).State = EntityState.Modified;

                else
                {
                    _db.PatientProfile_Insurance.Add(insurance);
                    await _db.SaveChangesAsync();

                    patient.InsuranceId = insurance.Id;
                }
                //
                patient.MedicaidIdNumber = MedicaidIdNumber;
                patient.MedicareIdNumber = MedicareIdNumber;
                patient.OtherInsuranceIdNumber = OtherInsuranceIdNumber;
                //
                patient.UpdatedBy = User.Identity.GetUserId();
                patient.UpdatedOn = DateTime.Now;
                _db.Entry(patient).State = EntityState.Modified;
                await _db.SaveChangesAsync();

             
                return RedirectToAction("Create", "PatientMedicalStatus", new { patientId = patient.Id });
            }

            ViewBag.PatientName = patient?.FirstName + " " + patient?.LastName;
            ViewBag.PatientId   = patient?.Id;
            ViewBag.CcmStatus   = patient?.CcmStatus;

             return View(insurance);
        }


        public async Task<PartialViewResult> _Create(int patientId)
        {
            var patient = _db.Patients.Find(patientId);
            var insurance = patient?.InsuranceId != null
                          ? await _db.PatientProfile_Insurance.FindAsync(patient.InsuranceId)
                          : new PatientProfile_Insurance { PatientId = patientId };

            ViewBag.PatientName = patient?.FirstName + " " + patient?.LastName;
            ViewBag.PatientId = patient?.Id;
            ViewBag.CcmStatus = patient?.CcmStatus;
            ViewBag.MedicareID = patient?.MedicareIdNumber;
            ViewBag.MedicaidID = patient?.MedicaidIdNumber;
            ViewBag.OtherInsuranceID = patient?.OtherInsuranceIdNumber;

            //if (User.IsInRole("Liaison"))
            //    ViewBag.ReviewId = HelperExtensions.ReviewTimeGet("Insurance", patient?.Id, User.Identity.GetUserId());

            return PartialView(insurance);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Liaison, Admin,Sales")]
        public async Task<string> _Create(PatientProfile_Insurance insurance, string MedicareIdNumber, string MedicaidIdNumber, string OtherInsuranceIdNumber)
        {
            if (HelperExtensions.isAllowedforEditingorAdd(insurance.PatientId, CategoryCycleStatusHelper.GetPatientNewOrOldCycleByCategory(insurance.PatientId, BillingCodeHelper.cmmBillingCatagoryid), User.Identity.GetUserId()) == false)
            {
                //return RedirectToAction("Index", "CcmStatus", new { status = HelperExtensions.GetStatusRedirectionbyUser(User.Identity.GetUserId()), Message = "Cycle is locked." });
                return "Cycle is locked.";

            }
            var patient = _db.Patients.Find(insurance.PatientId);
            if (patient != null && ModelState.IsValid)
            {
                if (patient.InsuranceId != null)
                    _db.Entry(insurance).State = EntityState.Modified;

                else
                {
                    _db.PatientProfile_Insurance.Add(insurance);
                    await _db.SaveChangesAsync();

                    patient.InsuranceId = insurance.Id;
                }
                //
                patient.MedicaidIdNumber = MedicaidIdNumber;
                patient.MedicareIdNumber = MedicareIdNumber;
                patient.OtherInsuranceIdNumber = OtherInsuranceIdNumber;
                //
                patient.UpdatedBy = User.Identity.GetUserId();
                patient.UpdatedOn = DateTime.Now;
                _db.Entry(patient).State = EntityState.Modified;
                await _db.SaveChangesAsync();

                return "True";
                //return RedirectToAction("Create", "PatientMedicalStatus", new { patientId = patient.Id });
            }

            ViewBag.PatientName = patient?.FirstName + " " + patient?.LastName;
            ViewBag.PatientId = patient?.Id;
            ViewBag.CcmStatus = patient?.CcmStatus;

            return "False";
            //return View(insurance);
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
