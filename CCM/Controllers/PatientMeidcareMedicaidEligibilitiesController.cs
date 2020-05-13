using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using CCM.Models;
using System.IO;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.Identity;
using CCM.Helpers;

namespace CCM.Controllers
{
    [Authorize(Roles = "Liaison, Admin, QAQC, PhysiciansGroup, LiaisonGroup,Sales")]
    public class PatientMeidcareMedicaidEligibilitiesController : BaseController
    {
        //private ApplicationdbContect _db = new ApplicationdbContect();

        // GET: PatientMeidcareMedicaidEligibilities
        public async Task<ActionResult> Index()
        {
            return View();
        }

        // GET: PatientMeidcareMedicaidEligibilities/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PatientMeidcareMedicaidEligibility patientMeidcareMedicaidEligibility = await _db.PatientMeidcareMedicaidEligibilities.FindAsync(id);
            if (patientMeidcareMedicaidEligibility == null)
            {
                return HttpNotFound();
            }
            return View(patientMeidcareMedicaidEligibility);
        }

        // GET: PatientMeidcareMedicaidEligibilities/Create
        public async Task<ActionResult> Create(int patientId)
        {
            var patient = await _db.Patients.FindAsync(patientId);
            ViewBag.PatientName = patient?.FirstName + " " + patient?.LastName;
            ViewBag.EnrollmentStatus = new SelectList(_db.EnrollmentStatuss.ToList(), "Id", "Name", patient.EnrollmentStatus);
            ViewBag.EnrollmentSubStatus = new SelectList(_db.EnrollmentSubStatuss.ToList(), "EnrollmentStatusID", "Name");
            ViewBag.EnrollemntStatusReson = new SelectList(_db.EnrollmentSubstatusReasons.ToList(), "Name", "Name");
            ViewBag.EnrollReason = _db.Patients.Where(x => x.Id == patientId).FirstOrDefault().EnrollmentStatusNotes;
            ViewBag.EnrollmentStatush = patient.EnrollmentStatus;
            ViewBag.EnrollmentSubStatush = patient.EnrollmentSubStatus;
            ViewBag.EnrollemntStatusResonh = patient.EnrollmentSubStatusReason;
            ViewBag.PatientId = patient.Id;
            if (User.IsInRole("Liaison"))
                ViewBag.ReviewId = HelperExtensions.ReviewTimeGet("Patient Medicare and medicaid eligibility", patient.Id, User.Identity.GetUserId());
            var model = _db.PatientMeidcareMedicaidEligibilities.Where(p => p.PatientId == patientId).FirstOrDefault();
            if (model != null)
            {
                return View(model);
            }
            return View(new PatientMeidcareMedicaidEligibility { PatientId = patientId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Liaison, Admin")]
        public async Task<ActionResult> Create([Bind(Exclude = "MedicareEligibiltySceenshot,MedicaidEligibiltySceenshot")] PatientMeidcareMedicaidEligibility patientMeidcareMedicaidEligibility, FormCollection form, string EnrollmentStatushiden, string EnrollmentSubStatushiden, string EnrollmentSubStatusReasonhiden, string EnrollmentStatusNotes)
        {
            if (HelperExtensions.isAllowedforEditingorAdd(patientMeidcareMedicaidEligibility.PatientId, CategoryCycleStatusHelper.GetPatientNewOrOldCycleByCategory(patientMeidcareMedicaidEligibility.PatientId, BillingCodeHelper.cmmBillingCatagoryid), User.Identity.GetUserId()) == false)
            {
                return RedirectToAction("Index", "CcmStatus", new { status = HelperExtensions.GetStatusRedirectionbyUser(User.Identity.GetUserId()), Message = "Cycle is locked." });
                //return "Cycle is locked.";
            }
            if (ModelState.IsValid)
            {
                var patient = _db.Patients.Where(x => x.Id == patientMeidcareMedicaidEligibility.PatientId).FirstOrDefault();
                patient.EnrollmentStatus = EnrollmentStatushiden == "" ? null : EnrollmentStatushiden;
                patient.EnrollmentSubStatus = EnrollmentSubStatushiden == "" ? null : EnrollmentSubStatushiden;
                if (patient?.EnrollmentSubStatus == "In-Active Enrolled")
                {
                    patient.EnrollmentSubStatusReason = EnrollmentSubStatusReasonhiden;
                }
                else
                {
                    patient.EnrollmentSubStatusReason = "";
                }
                patient.EnrollmentStatusNotes = EnrollmentStatusNotes;
                if (patient.CCMEnrolledOn == null)
                {
                    patient.CcmStatus = "Enrolled";
                    patient.CCMEnrolledOn = DateTime.Now;
                    patient.CCMEnrolledBy = User.Identity.GetUserId();
                    //HelperExtensions.UpdateCurrentMonthActivityfromCycleZeroToOne(patient.Id);
                    try
                    {


                        var reviewtimeccms = _db.ReviewTimeCcms.Where(x => x.PatientId == patient.Id && x.Cycle == 0).ToList().Where(x => x.StartTime.Date.Month == DateTime.Now.Month).ToList();
                        foreach (var reviewtimeccmitem in reviewtimeccms)
                        {
                            reviewtimeccmitem.Cycle = 1;
                            _db.Entry(reviewtimeccmitem).State = EntityState.Modified;
                            _db.SaveChanges();
                        }
                    }
                    catch (Exception ex)
                    {


                    }
                }
                _db.Entry(patient).State = EntityState.Modified;
                _db.SaveChanges();
                var postedImageFile = Request.Files["MedicareEligibiltySceenshot"];
                var postedImageFile1 = Request.Files["MedicaidEligibiltySceenshot"];

                var model = _db.PatientMeidcareMedicaidEligibilities.AsNoTracking().Where(p => p.PatientId == patientMeidcareMedicaidEligibility.PatientId).FirstOrDefault();
                if (postedImageFile?.ContentLength != 0 && postedImageFile?.InputStream != null)
                    using (var binary = new BinaryReader(postedImageFile.InputStream))
                    {
                        var imageData = binary.ReadBytes(postedImageFile.ContentLength);
                        if (imageData.Length > 0)
                            patientMeidcareMedicaidEligibility.MedicareEligibiltySceenshot = imageData;
                    }
                if (postedImageFile1?.ContentLength != 0 && postedImageFile1?.InputStream != null)
                    using (var binary = new BinaryReader(postedImageFile1.InputStream))
                    {
                        var imageData = binary.ReadBytes(postedImageFile1.ContentLength);
                        if (imageData.Length > 0)
                            patientMeidcareMedicaidEligibility.MedicaidEligibiltySceenshot = imageData;
                    }
                if (model == null)
                {
                    patientMeidcareMedicaidEligibility.CreatedOn = DateTime.Now;
                    patientMeidcareMedicaidEligibility.CreatedBy = User.Identity.GetUserId();

                    _db.PatientMeidcareMedicaidEligibilities.Add(patientMeidcareMedicaidEligibility);
                    await _db.SaveChangesAsync();
                }
                else
                {

                    patientMeidcareMedicaidEligibility.MedicaidEligibiltySceenshot = patientMeidcareMedicaidEligibility.MedicaidEligibiltySceenshot == null ? model.MedicaidEligibiltySceenshot : patientMeidcareMedicaidEligibility.MedicaidEligibiltySceenshot;
                    patientMeidcareMedicaidEligibility.MedicareEligibiltySceenshot = patientMeidcareMedicaidEligibility.MedicareEligibiltySceenshot == null ? model.MedicareEligibiltySceenshot : patientMeidcareMedicaidEligibility.MedicareEligibiltySceenshot;
                    patientMeidcareMedicaidEligibility.UpdatedOn = DateTime.Now;
                    patientMeidcareMedicaidEligibility.UpdatedBy = User.Identity.GetUserId();
                    _db.Entry(patientMeidcareMedicaidEligibility).State = EntityState.Modified;
                    await _db.SaveChangesAsync();
                }

                //return "True";
                return RedirectToAction("Details", "Patient", new { Id = patient.Id });
            }

            //return "False";
            return View(patientMeidcareMedicaidEligibility);
        }




        public async Task<PartialViewResult> _Create(int patientId)
        {
            var patient = await _db.Patients.FindAsync(patientId);
            ViewBag.PatientName = patient?.FirstName + " " + patient?.LastName;
            ViewBag.EnrollmentStatus = new SelectList(_db.EnrollmentStatuss.ToList(), "Id", "Name", patient.EnrollmentStatus);
            ViewBag.EnrollmentSubStatus = new SelectList(_db.EnrollmentSubStatuss.ToList(), "EnrollmentStatusID", "Name");
            ViewBag.EnrollemntStatusReson = new SelectList(_db.EnrollmentSubstatusReasons.ToList(), "Name", "Name");
            ViewBag.EnrollReason = _db.Patients.Where(x => x.Id == patientId).FirstOrDefault().EnrollmentStatusNotes;
            ViewBag.EnrollmentStatush = patient.EnrollmentStatus;
            ViewBag.EnrollmentSubStatush = patient.EnrollmentSubStatus;
            ViewBag.EnrollemntStatusResonh = patient.EnrollmentSubStatusReason;
            ViewBag.PatientId = patient.Id;
            //if (User.IsInRole("Liaison"))
            //    ViewBag.ReviewId = HelperExtensions.ReviewTimeGet("Patient Medicare and medicaid eligibility", patient.Id, User.Identity.GetUserId());
            var model = _db.PatientMeidcareMedicaidEligibilities.Where(p => p.PatientId == patientId).FirstOrDefault();
            if (model != null)
            {
                return PartialView(model);
            }
            return PartialView(new PatientMeidcareMedicaidEligibility { PatientId = patientId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Liaison, Admin,Sales")]
        public async Task<string> _Create([Bind(Exclude = "MedicareEligibiltySceenshot,MedicaidEligibiltySceenshot")] PatientMeidcareMedicaidEligibility patientMeidcareMedicaidEligibility, FormCollection form, string EnrollmentStatushiden, string EnrollmentSubStatushiden, string EnrollmentSubStatusReasonhiden, string EnrollmentStatusNotes)
        {
            if (HelperExtensions.isAllowedforEditingorAdd(patientMeidcareMedicaidEligibility.PatientId, CategoryCycleStatusHelper.GetPatientNewOrOldCycleByCategory(patientMeidcareMedicaidEligibility.PatientId, BillingCodeHelper.cmmBillingCatagoryid), User.Identity.GetUserId()) == false)
            {
                //return RedirectToAction("Index", "CcmStatus", new { status = HelperExtensions.GetStatusRedirectionbyUser(User.Identity.GetUserId()), Message = "Cycle is locked." });
                return "Cycle is locked.";
            }
            if (ModelState.IsValid)
            {
                var patient = _db.Patients.Where(x => x.Id == patientMeidcareMedicaidEligibility.PatientId).FirstOrDefault();
                patient.EnrollmentStatus = EnrollmentStatushiden == "" ? null : EnrollmentStatushiden;
                patient.EnrollmentSubStatus = EnrollmentSubStatushiden == "" ? null : EnrollmentSubStatushiden;
                if (patient?.EnrollmentSubStatus == "In-Active Enrolled")
                {
                    patient.EnrollmentSubStatusReason = EnrollmentSubStatusReasonhiden;
                }
                else
                {
                    patient.EnrollmentSubStatusReason = "";
                }
                patient.EnrollmentStatusNotes = EnrollmentStatusNotes;
                var OLDpatientdata = _db.Patients.AsNoTracking().Where(x => x.Id == patient.Id).FirstOrDefault();
                if (!User.IsInRole("Admin") && !User.IsInRole("LiaisonGroup"))
                {
                    if ((OLDpatientdata.EnrollmentStatus == "Enrolled" && OLDpatientdata.EnrollmentSubStatus == "Active Enrolled") && (patient.EnrollmentStatus != "Enrolled" && patient.EnrollmentSubStatus != "Active Enrolled"))
                    {
                        return  "Not allowed to change the enrollment status";
                    }
                }

                if (patient.CCMEnrolledOn == null)
                {
                    patient.CcmStatus = "Enrolled";
                    patient.CCMEnrolledOn = DateTime.Now;
                    patient.CCMEnrolledBy = User.Identity.GetUserId();
                    //HelperExtensions.UpdateCurrentMonthActivityfromCycleZeroToOne(patient.Id);
                    try
                    {


                        var reviewtimeccms = _db.ReviewTimeCcms.Where(x => x.PatientId == patient.Id && x.Cycle == 0).ToList().Where(x => x.StartTime.Date.Month == DateTime.Now.Month).ToList();
                        foreach (var reviewtimeccmitem in reviewtimeccms)
                        {
                            reviewtimeccmitem.Cycle = 1;
                            _db.Entry(reviewtimeccmitem).State = EntityState.Modified;
                            _db.SaveChanges();
                        }
                    }
                    catch (Exception ex)
                    {


                    }
                }
                _db.Entry(patient).State = EntityState.Modified;
                _db.SaveChanges();
                var postedImageFile = Request.Files["MedicareEligibiltySceenshot"];
                var postedImageFile1 = Request.Files["MedicaidEligibiltySceenshot"];

                var model = _db.PatientMeidcareMedicaidEligibilities.AsNoTracking().Where(p => p.PatientId == patientMeidcareMedicaidEligibility.PatientId).FirstOrDefault();
                if (postedImageFile?.ContentLength != 0 && postedImageFile?.InputStream != null)
                    using (var binary = new BinaryReader(postedImageFile.InputStream))
                    {
                        var imageData = binary.ReadBytes(postedImageFile.ContentLength);
                        if (imageData.Length > 0)
                            patientMeidcareMedicaidEligibility.MedicareEligibiltySceenshot = imageData;
                    }
                if (postedImageFile1?.ContentLength != 0 && postedImageFile1?.InputStream != null)
                    using (var binary = new BinaryReader(postedImageFile1.InputStream))
                    {
                        var imageData = binary.ReadBytes(postedImageFile1.ContentLength);
                        if (imageData.Length > 0)
                            patientMeidcareMedicaidEligibility.MedicaidEligibiltySceenshot = imageData;
                    }
                if (model == null)
                {
                    patientMeidcareMedicaidEligibility.CreatedOn = DateTime.Now;
                    patientMeidcareMedicaidEligibility.CreatedBy = User.Identity.GetUserId();

                    _db.PatientMeidcareMedicaidEligibilities.Add(patientMeidcareMedicaidEligibility);
                    await _db.SaveChangesAsync();
                }
                else
                {

                    patientMeidcareMedicaidEligibility.MedicaidEligibiltySceenshot = patientMeidcareMedicaidEligibility.MedicaidEligibiltySceenshot == null ? model.MedicaidEligibiltySceenshot : patientMeidcareMedicaidEligibility.MedicaidEligibiltySceenshot;
                    patientMeidcareMedicaidEligibility.MedicareEligibiltySceenshot = patientMeidcareMedicaidEligibility.MedicareEligibiltySceenshot == null ? model.MedicareEligibiltySceenshot : patientMeidcareMedicaidEligibility.MedicareEligibiltySceenshot;
                    patientMeidcareMedicaidEligibility.UpdatedOn = DateTime.Now;
                    patientMeidcareMedicaidEligibility.UpdatedBy = User.Identity.GetUserId();
                    _db.Entry(patientMeidcareMedicaidEligibility).State = EntityState.Modified;
                    await _db.SaveChangesAsync();
                }

                return "True";
            }

            return "False";
        }



        // GET: PatientMeidcareMedicaidEligibilities/Edit/5
        public async Task<ActionResult> Edit(int? patientId)
        {
            if (patientId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PatientMeidcareMedicaidEligibility patientMeidcareMedicaidEligibility = await _db.PatientMeidcareMedicaidEligibilities.FindAsync(patientId);
            if (patientMeidcareMedicaidEligibility == null)
            {
                return HttpNotFound();
            }
            return View(patientMeidcareMedicaidEligibility);
        }

        // POST: PatientMeidcareMedicaidEligibilities/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Liaison, Admin")]
        public async Task<ActionResult> Edit([Bind(Include = "Id,PatientId,MedicareEligibilty,MedicareEligibiltyNotes,MedicareEligibiltySceenshot,MedicaidEligibilty,MedicaidEligibiltyNotes,MedicaidEligibiltySceenshot,UpdatedOn,UpdatedBy,CreatedOn,CreatedBy")] PatientMeidcareMedicaidEligibility patientMeidcareMedicaidEligibility)
        {
            if (ModelState.IsValid)
            {
                _db.Entry(patientMeidcareMedicaidEligibility).State = EntityState.Modified;
                await _db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(patientMeidcareMedicaidEligibility);
        }

        // GET: PatientMeidcareMedicaidEligibilities/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PatientMeidcareMedicaidEligibility patientMeidcareMedicaidEligibility = await _db.PatientMeidcareMedicaidEligibilities.FindAsync(id);
            if (patientMeidcareMedicaidEligibility == null)
            {
                return HttpNotFound();
            }
            return View(patientMeidcareMedicaidEligibility);
        }

        // POST: PatientMeidcareMedicaidEligibilities/Delete/5
        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "Liaison, Admin")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            PatientMeidcareMedicaidEligibility patientMeidcareMedicaidEligibility = await _db.PatientMeidcareMedicaidEligibilities.FindAsync(id);
            _db.PatientMeidcareMedicaidEligibilities.Remove(patientMeidcareMedicaidEligibility);
            await _db.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Liaison, Admin")]
        public async Task<ActionResult> CheckEligibility(int PatientID, string DateFrom, string DateTo)

        {


            return View();
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
