using System;
using CCM.Models;
using System.Web.Mvc;
using System.Data.Entity;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using log4net.Appender;
using CCM.Helpers;

namespace CCM.Controllers
{
    //////////////////////////////////////////////////revert to chnegeset 513
    [Authorize(Roles = "Liaison, Admin, QAQC, PhysiciansGroup, LiaisonGroup,Sales")]
    public class PatientWorkAndRelationshipController : BaseController
    {
        //private readonly ApplicationdbContect _db = new ApplicationdbContect();

        // log4net declaration
        //private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);


        public async Task<ActionResult> Create(int patientId)
        {
            var patient = _db.Patients.Find(patientId);
            var workRelationship = patient?.WorkAndRelationshipId != null
                                 ? await _db.PatientLifestyle_WorkAndRelationships.FindAsync(patient.WorkAndRelationshipId)
                                 : new PatientLifestyle_WorkAndRelationship { PatientId = patientId };

            ViewBag.Employment_StatusId   = new SelectList(_db.PatientLifestyle_WorkAndRelationship_EmploymentStatuses, "Id", "Type");
            ViewBag.Relationship_StatusId = new SelectList(_db.PatientLifestyle_WorkAndRelationship_RelationshipStatuses, "Id", "Type");
            ViewBag.TravelRequirementId   = new SelectList(_db.PatientLifestyle_WorkAndRelationship_Travels, "Id", "Type");

            ViewBag.PatientName = patient?.FirstName + " " + patient?.LastName;
            ViewBag.PatientId   = patient?.Id;
            ViewBag.CcmStatus   = patient?.CcmStatus;

            if (User.IsInRole("Liaison"))
                ViewBag.ReviewId = HelperExtensions.ReviewTimeGet("Work & Relationship Status", patient?.Id, User.Identity.GetUserId());

            return View(workRelationship);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Liaison, Admin")]
        public async Task<ActionResult> Create(PatientLifestyle_WorkAndRelationship workRelationship)
        {
            if (HelperExtensions.isAllowedforEditingorAdd(workRelationship.PatientId, CategoryCycleStatusHelper.GetPatientNewOrOldCycleByCategory(workRelationship.PatientId, BillingCodeHelper.cmmBillingCatagoryid), User.Identity.GetUserId()) == false)
            {
                return RedirectToAction("Index", "CcmStatus", new { status = HelperExtensions.GetStatusRedirectionbyUser(User.Identity.GetUserId()), Message = "Cycle is locked." });
            }
            var patient  = _db.Patients.Find(workRelationship.PatientId);
            if (patient != null && ModelState.IsValid)
            {
                if (patient.WorkAndRelationshipId != null) { 
                     workRelationship.Id =(int) patient.WorkAndRelationshipId;
                    _db.Entry(workRelationship).State = EntityState.Modified;
                    }
                else
                {
                    _db.PatientLifestyle_WorkAndRelationships.Add(workRelationship);
                    await _db.SaveChangesAsync();

                    patient.WorkAndRelationshipId = workRelationship.Id;
                }

                patient.UpdatedBy = User.Identity.GetUserId();
                patient.UpdatedOn = DateTime.Now;
                _db.Entry(patient).State = EntityState.Modified;
                await _db.SaveChangesAsync();

                return RedirectToAction("Create", "PatientLifestyle_DietAndHabit", new { patientId = patient.Id });
            }

            ViewBag.Employment_StatusId   = new SelectList(_db.PatientLifestyle_WorkAndRelationship_EmploymentStatuses, "Id", "Type", workRelationship.Employment_StatusId);
            ViewBag.Relationship_StatusId = new SelectList(_db.PatientLifestyle_WorkAndRelationship_RelationshipStatuses, "Id", "Type", workRelationship.Relationship_StatusId);
            ViewBag.TravelRequirementId   = new SelectList(_db.PatientLifestyle_WorkAndRelationship_Travels, "Id", "Type", workRelationship.TravelRequirementId);

            ViewBag.PatientName = patient?.FirstName + " " + patient?.LastName;
            ViewBag.PatientId   = patient?.Id;
            ViewBag.CcmStatus   = patient?.CcmStatus;

            return View(workRelationship);

        }



        public async Task<PartialViewResult> _Create(int patientId)
        {
            var patient = _db.Patients.Find(patientId);
            var workRelationship = patient?.WorkAndRelationshipId != null
                                 ? await _db.PatientLifestyle_WorkAndRelationships.FindAsync(patient.WorkAndRelationshipId)
                                 : new PatientLifestyle_WorkAndRelationship { PatientId = patientId };

            ViewBag.Employment_StatusId = new SelectList(_db.PatientLifestyle_WorkAndRelationship_EmploymentStatuses, "Id", "Type");
            ViewBag.Relationship_StatusId = new SelectList(_db.PatientLifestyle_WorkAndRelationship_RelationshipStatuses, "Id", "Type");
            ViewBag.TravelRequirementId = new SelectList(_db.PatientLifestyle_WorkAndRelationship_Travels, "Id", "Type");

            ViewBag.PatientName = patient?.FirstName + " " + patient?.LastName;
            ViewBag.PatientId = patient?.Id;
            ViewBag.CcmStatus = patient?.CcmStatus;

            //if (User.IsInRole("Liaison"))
            //    ViewBag.ReviewId = HelperExtensions.ReviewTimeGet("Work & Relationship Status", patient?.Id, User.Identity.GetUserId());

            return PartialView(workRelationship);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Liaison, Admin,Sales")]
        public async Task<string> _Create(PatientLifestyle_WorkAndRelationship workRelationship)
        {

            try
            {


            if (HelperExtensions.isAllowedforEditingorAdd(workRelationship.PatientId, CategoryCycleStatusHelper.GetPatientNewOrOldCycleByCategory(workRelationship.PatientId, BillingCodeHelper.cmmBillingCatagoryid), User.Identity.GetUserId()) == false)
            {
                return "Cycle is locked.";

            }
            var patient = _db.Patients.Find(workRelationship.PatientId);
            if (patient != null && ModelState.IsValid)
            {
                    if (patient.WorkAndRelationshipId != null)
                    {
                        workRelationship.Id =(int) patient.WorkAndRelationshipId;
                        _db.Entry(workRelationship).State = EntityState.Modified;
                        _db.SaveChanges();
                    }

                    else
                    {

                        // facing issue here  (An error occurred while updating the entries. See the inner exception for details.)
                       
                        _db.PatientLifestyle_WorkAndRelationships.Add(workRelationship);
                       _db.SaveChanges();

                        patient.WorkAndRelationshipId = workRelationship.Id;
                    }

                patient.UpdatedBy = User.Identity.GetUserId();
                patient.UpdatedOn = DateTime.Now;
                _db.Entry(patient).State = EntityState.Modified;
                await _db.SaveChangesAsync();
                
                return "True";
            }

            ViewBag.Employment_StatusId = new SelectList(_db.PatientLifestyle_WorkAndRelationship_EmploymentStatuses, "Id", "Type", workRelationship.Employment_StatusId);
            ViewBag.Relationship_StatusId = new SelectList(_db.PatientLifestyle_WorkAndRelationship_RelationshipStatuses, "Id", "Type", workRelationship.Relationship_StatusId);
            ViewBag.TravelRequirementId = new SelectList(_db.PatientLifestyle_WorkAndRelationship_Travels, "Id", "Type", workRelationship.TravelRequirementId);

            ViewBag.PatientName = patient?.FirstName + " " + patient?.LastName;
            ViewBag.PatientId = patient?.Id;
            ViewBag.CcmStatus = patient?.CcmStatus;
            
            return "False";

            }
            catch (Exception ex)
            {
                
                log.Error( Environment.NewLine + User.Identity.GetUserId() + "-------------" + ex.Message + "-------------" + ex.StackTrace + "--------------------------------------------------END----------------------------------------------");
                return "False";
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