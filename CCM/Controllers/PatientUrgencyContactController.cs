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
    public class PatientUrgencyContactController : BaseController
    {
        //private readonly ApplicationdbContect _db = new ApplicationdbContect();


        public async Task<PartialViewResult> _Index(int? patientId)
        {
            ViewBag.patientId = patientId;
            //if (User.IsInRole("Liaison"))
            //    ViewBag.ReviewId = HelperExtensions.ReviewTimeGet("ICD 10 Codes", patientId, User.Identity.GetUserId());
            var list = _db.PatientProfile_UrgencyContacts.Where(m => m.PatientId == patientId).OrderByDescending(x => x.PrimaryName);

            return PartialView(await list.ToListAsync());
        }

        public async Task<ActionResult> Create(int patientId)
        {
            var patient = _db.Patients.Find(patientId);
            var urgencyContact = patient?.UrgencyContactId != null
                               ? await _db.PatientProfile_UrgencyContacts.FindAsync(patient.UrgencyContactId)
                               : new PatientProfile_UrgencyContact { PatientId = patientId };

            ViewBag.PatientName = patient?.FirstName + " " + patient?.LastName;
            ViewBag.PatientId = patient?.Id;
            ViewBag.CcmStatus = patient?.CcmStatus;

            if (User.IsInRole("Liaison"))
                ViewBag.ReviewId = HelperExtensions.ReviewTimeGet("Urgency Contacts", patient?.Id, User.Identity.GetUserId());

            return View(urgencyContact);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Liaison, Admin")]
        public async Task<ActionResult> Create(PatientProfile_UrgencyContact urgencyContact)
        {
            if (HelperExtensions.isAllowedforEditingorAdd(urgencyContact.PatientId, CategoryCycleStatusHelper.GetPatientNewOrOldCycleByCategory(urgencyContact.PatientId, BillingCodeHelper.cmmBillingCatagoryid), User.Identity.GetUserId()) == false)
            {
                return RedirectToAction("Index", "CcmStatus", new { status = HelperExtensions.GetStatusRedirectionbyUser(User.Identity.GetUserId()), Message = "Cycle is locked." });

            }
            var patient = _db.Patients.Find(urgencyContact.PatientId);
            if (patient != null && ModelState.IsValid)
            {
                if (patient.UrgencyContactId != null)
                    _db.Entry(urgencyContact).State = EntityState.Modified;

                else
                {
                    _db.PatientProfile_UrgencyContacts.Add(urgencyContact);
                    await _db.SaveChangesAsync();

                    patient.UrgencyContactId = urgencyContact.Id;
                }

                patient.UpdatedBy = User.Identity.GetUserId();
                patient.UpdatedOn = DateTime.Now;
                _db.Entry(patient).State = EntityState.Modified;
                await _db.SaveChangesAsync();

                return RedirectToAction("Create", "PatientInsurance", new { patientId = patient.Id });

            }

            ViewBag.PatientName = patient?.FirstName + " " + patient?.LastName;
            ViewBag.PatientId = patient?.Id;
            ViewBag.CcmStatus = patient?.CcmStatus;

            return View(urgencyContact);

        }


        public async Task<PartialViewResult> _Create(int patientId)
        {
            ViewBag.ProfesionalCareProvider = new SelectList(_db.PatientProfile_ProfesionalCareProvider.ToList(), "Id", "CareProvider");
            ViewBag.DieseaseState = new SelectList(_db.PatientProfile_DieseaseState.ToList(), "Id", "DieseaseStateType");


            var patient = _db.Patients.Find(patientId);
            var urgencyContact =  new PatientProfile_UrgencyContact { PatientId = patientId };

            ViewBag.PatientName = patient?.FirstName + " " + patient?.LastName;
            ViewBag.PatientId = patient?.Id;
            ViewBag.CcmStatus = patient?.CcmStatus;

            //if (User.IsInRole("Liaison"))
            //    ViewBag.ReviewId = HelperExtensions.ReviewTimeGet("Urgency Contacts", patient?.Id, User.Identity.GetUserId());
            //patient?.UrgencyContactId != null
            //                   ? await _db.PatientProfile_UrgencyContacts.FindAsync(patient.UrgencyContactId)
            //                   :
            return PartialView(urgencyContact);
        }
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //[Authorize(Roles = "Liaison, Admin")]
        //public async Task<string> _Create(PatientProfile_UrgencyContact urgencyContact)
        //{
        //    if (HelperExtensions.isAllowedforEditingorAdd(urgencyContact.PatientId, CategoryCycleStatusHelper.GetPatientNewOrOldCycleByCategory(urgencyContact.PatientId), User.Identity.GetUserId()) == false)
        //    {
        //        return "Cycle is locked.";

        //    }
        //    var patient = _db.Patients.Find(urgencyContact.PatientId);
        //    if (patient != null && ModelState.IsValid)
        //    {
        //        if (patient.UrgencyContactId != null)
        //            _db.Entry(urgencyContact).State = EntityState.Modified;

        //        else
        //        {
        //            _db.PatientProfile_UrgencyContacts.Add(urgencyContact);
        //            await _db.SaveChangesAsync();

        //            patient.UrgencyContactId = urgencyContact.Id;
        //        }

        //        patient.UpdatedBy = User.Identity.GetUserId();
        //        patient.UpdatedOn = DateTime.Now;
        //        _db.Entry(patient).State = EntityState.Modified;
        //        await _db.SaveChangesAsync();
        //        return "True";
        //    }
        //    else
        //    {
        //        var errorList = ModelState.Values.SelectMany(m => m.Errors)
        //                         .Select(e => e.ErrorMessage)
        //                         .ToList();
        //        var errorstr = string.Join(",", errorList);
        //        return errorstr;
        //    }
        //    ViewBag.PatientName = patient?.FirstName + " " + patient?.LastName;
        //    ViewBag.PatientId = patient?.Id;
        //    ViewBag.CcmStatus = patient?.CcmStatus;

        //    return "False";
        //}


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Liaison, Admin,Sales")]
        public async Task<string> _Create([Bind(Include = "PatientId")] PatientProfile_UrgencyContact urgencyDetails, string[] PrimaryNameH, string[] PrimaryRelationshipH, string[] PrimaryMobilePhoneNumberH, string[] PrimaryMobilePhoneNumberH1,  string[] PrimaryHomePhoneNumberH, string[] PrimaryHomePhoneNumberH1, string[] PrimaryProfesionalCareProviderH, string[] PrimaryExpertiseH, string[] PrimaryHealthProxyAndCarplaneH, string[] PrimaryEmailH, string[] PrimaryIsShareCarePlanH, string[] UrgencyTypeH)
        {
            try
            {

           
            if (HelperExtensions.isAllowedforEditingorAdd(urgencyDetails.PatientId, CategoryCycleStatusHelper.GetPatientNewOrOldCycleByCategory(urgencyDetails.PatientId, BillingCodeHelper.cmmBillingCatagoryid), User.Identity.GetUserId()) == false)
            {
                return "Cycle is locked.";
            }
            //if (ModelState.IsValid)
            //{
            var alreadycodes = _db.PatientProfile_UrgencyContacts.AsNoTracking().Where(x => x.PatientId == urgencyDetails.PatientId).ToList();

            var patient = _db.Patients.Find(urgencyDetails.PatientId);

            for (int i = 0; i <= PrimaryNameH.Count() - 1; i++)
            {
                var alreadyitem = alreadycodes.Where(x => x.PrimaryName == PrimaryNameH[i]).FirstOrDefault();
                if (alreadyitem == null)
                {
                    PatientProfile_UrgencyContact UrgencyContactDetails = new PatientProfile_UrgencyContact();
                    UrgencyContactDetails.PrimaryName = PrimaryNameH[i];
                    UrgencyContactDetails.PrimaryRelationship = PrimaryRelationshipH[i];
                    UrgencyContactDetails.PrimaryMobilePhoneNumber = PrimaryMobilePhoneNumberH[i];
                    UrgencyContactDetails.PrimaryMobilePhoneNumber1 = PrimaryMobilePhoneNumberH1[i];
                    UrgencyContactDetails.PrimaryHomePhoneNumber = PrimaryHomePhoneNumberH[i];
                    UrgencyContactDetails.PrimaryHomePhoneNumber1 = PrimaryHomePhoneNumberH1[i];
                    UrgencyContactDetails.PrimaryProfesionalCareProvider = PrimaryProfesionalCareProviderH[i];
                    UrgencyContactDetails.PrimaryExpertise = PrimaryExpertiseH[i];
                    UrgencyContactDetails.PrimaryHealthProxyAndCarplane = string.IsNullOrEmpty(PrimaryHealthProxyAndCarplaneH[i]) ? false : Convert.ToBoolean(PrimaryHealthProxyAndCarplaneH[i]);
                    UrgencyContactDetails.PrimaryEmail = PrimaryEmailH[i];
                    //UrgencyContactDetails.PrimaryEmail = PrimaryEmailH[i];
                    UrgencyContactDetails.PrimaryIsShareCarePlan = string.IsNullOrEmpty(PrimaryIsShareCarePlanH[i]) ? false : Convert.ToBoolean(PrimaryIsShareCarePlanH[i]);
                    UrgencyContactDetails.ContactType = UrgencyTypeH[i];

                    if (patient != null)
                    {
                        UrgencyContactDetails.PatientId = patient.Id;
                        UrgencyContactDetails.Cycle = patient.Cycle;
                    }

                    _db.PatientProfile_UrgencyContacts.Add(UrgencyContactDetails);
                }
            }

            await _db.SaveChangesAsync();
            return "True";
            }
            catch (Exception ex)
            {
                return ex.Message+ex.InnerException.ToString();

            }
            //}
            //else
            //{
            //    var errorList = ModelState.Values.SelectMany(m => m.Errors)
            //                     .Select(e => e.ErrorMessage)
            //                     .ToList();
            //    var errorstr = string.Join(",", errorList);
            //    return errorstr;
            //}
            //return "False";
        }


        public async Task<PartialViewResult> _Edit(int? id, int patientId)
        {
            ViewBag.patientId = patientId;
            //if (User.IsInRole("Liaison"))
            //    ViewBag.ReviewId = HelperExtensions.ReviewTimeGet("ICD 10 Codes", patientId, User.Identity.GetUserId());
            if (id == null)
            {
                //return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                return PartialView("_BadRequest");
            }
            PatientProfile_UrgencyContact UrgencycontactDetail = await _db.PatientProfile_UrgencyContacts.FindAsync(id);
            if (UrgencycontactDetail == null)
            {
                return PartialView("_NotFound");
                //return HttpNotFound();
            }
            return PartialView(UrgencycontactDetail);
        }

        // POST: Icd/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Liaison, Admin,Sales")]
        public async Task<string> _Edit([Bind(Include = "Id,PatientId,PrimaryName,PrimaryRelationship,PrimaryMobilePhoneNumber,PrimaryMobilePhoneNumber1,PrimaryHomePhoneNumber,PrimaryHomePhoneNumber1,PrimaryProfesionalCareProvider,PrimaryExpertise,PrimaryHealthProxyAndCarplane,PrimaryEmail,PrimaryIsShareCarePlan,ContactType")] PatientProfile_UrgencyContact UrgencyContactDetails)
        {
            if (HelperExtensions.isAllowedforEditingorAdd(UrgencyContactDetails.PatientId, CategoryCycleStatusHelper.GetPatientNewOrOldCycleByCategory(UrgencyContactDetails.PatientId, BillingCodeHelper.cmmBillingCatagoryid), User.Identity.GetUserId()) == false)
            {
                return "Cycle is locked.";
            }
            if (ModelState.IsValid)
            {
                _db.Entry(UrgencyContactDetails).State = EntityState.Modified;
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
        }

        [Authorize(Roles = "Liaison, Admin,Sales")]
        public async Task<string> _Delete(int id)
        {
            PatientProfile_UrgencyContact urgencyDetails = await _db.PatientProfile_UrgencyContacts.FindAsync(id);
            if (HelperExtensions.isAllowedforEditingorAdd(urgencyDetails.PatientId, CategoryCycleStatusHelper.GetPatientNewOrOldCycleByCategory(urgencyDetails.PatientId, BillingCodeHelper.cmmBillingCatagoryid), User.Identity.GetUserId()) == false)
            {
                return "Cycle is locked.";
            }
            _db.PatientProfile_UrgencyContacts.Remove(urgencyDetails);
            await _db.SaveChangesAsync();
            return "True";
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