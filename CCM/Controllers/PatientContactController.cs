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
    public class PatientContactController : BaseController
    {
        //private readonly ApplicationdbContect _db = new ApplicationdbContect();



        public async Task<ActionResult> Create(int patientId)
        {
            var patient = await _db.Patients.FindAsync(patientId);
            var contact = patient?.ContactId != null
                        ? await _db.PatientProfile_Contact.FindAsync(patient.ContactId)
                        : new PatientProfile_Contact
                          {
                              PatientId           = patientId,
                              CellPhoneNumber     = patient?.MobilePhoneNumber,
                              CellPhonePermission = true,
                              HomePhoneNumber     = patient?.HomePhoneNumber,
                              WorkPhoneNumber     = patient?.WorkPhoneNumber,
                              Email               = patient?.Email,
                              EmailPermission     = true
                          };

            ViewBag.PatientName = patient?.FirstName + " " + patient?.LastName;
            ViewBag.PatientId   = patient?.Id;
            ViewBag.CcmStatus   = patient?.CcmStatus;

            if (User.IsInRole("Liaison"))
                ViewBag.ReviewId = HelperExtensions.ReviewTimeGet("Contacts", patient?.Id, User.Identity.GetUserId());

            return View(contact);
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Liaison, Admin")]
        public async Task<ActionResult> Create(PatientProfile_Contact contact)
        {
            if (HelperExtensions.isAllowedforEditingorAdd(contact.PatientId, CategoryCycleStatusHelper.GetPatientNewOrOldCycleByCategory(contact.PatientId, BillingCodeHelper.cmmBillingCatagoryid), User.Identity.GetUserId()) == false)
            {
                return RedirectToAction("Index", "CcmStatus", new { status = HelperExtensions.GetStatusRedirectionbyUser(User.Identity.GetUserId()), Message = "Cycle is locked." });
            }
            var patient  = await _db.Patients.FindAsync(contact.PatientId);
            if (patient != null && ModelState.IsValid)
            {
                patient.MobilePhoneNumber = contact.CellPhoneNumber;
                patient.AllowText         = contact.CellPhonePermission;
                patient.WorkPhoneNumber   = contact.WorkPhoneNumber;
                patient.HomePhoneNumber   = contact.HomePhoneNumber;
                patient.Email             = contact.Email;
                patient.AllowEmail        = contact.EmailPermission;

                if (patient.ContactId != null)
                    _db.Entry(contact).State = EntityState.Modified;

                else
                {
                    _db.PatientProfile_Contact.Add(contact);
                    await _db.SaveChangesAsync();

                    patient.ContactId = contact.Id;
                }

                patient.UpdatedBy = User.Identity.GetUserId();
                patient.UpdatedOn = DateTime.Now;
                _db.Entry(patient).State = EntityState.Modified;
                await _db.SaveChangesAsync();

                return RedirectToAction("Create", "PatientAddress", new { patientId = patient?.Id });
                
            }

            ViewBag.PatientName = patient?.FirstName + " " + patient?.LastName;
            ViewBag.PatientId   = patient?.Id;
            ViewBag.CcmStatus   = patient?.CcmStatus;

            return View(contact);
           
        }



        public async Task<PartialViewResult> _Create(int patientId)
        {
            var patient = await _db.Patients.FindAsync(patientId);
            var contact = patient?.ContactId != null
                        ? await _db.PatientProfile_Contact.FindAsync(patient.ContactId)
                        : new PatientProfile_Contact
                        {
                            PatientId = patientId,
                            CellPhoneNumber = patient?.MobilePhoneNumber,
                            CellPhonePermission = true,
                            HomePhoneNumber = patient?.HomePhoneNumber,
                            WorkPhoneNumber = patient?.WorkPhoneNumber,
                            Email = patient?.Email,
                            EmailPermission = true
                        };

            ViewBag.PatientName = patient?.FirstName + " " + patient?.LastName;
            ViewBag.PatientId = patient?.Id;
            ViewBag.CcmStatus = patient?.CcmStatus;

            //if (User.IsInRole("Liaison"))
            //    ViewBag.ReviewId = HelperExtensions.ReviewTimeGet("Contacts", patient?.Id, User.Identity.GetUserId());

            return PartialView(contact);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Liaison, Admin,Sales")]
        public async Task<string> _Create(PatientProfile_Contact contact)
        {
            if (HelperExtensions.isAllowedforEditingorAdd(contact.PatientId, CategoryCycleStatusHelper.GetPatientNewOrOldCycleByCategory(contact.PatientId, BillingCodeHelper.cmmBillingCatagoryid), User.Identity.GetUserId()) == false)
            {
                //return RedirectToAction("Index", "CcmStatus", new { status = HelperExtensions.GetStatusRedirectionbyUser(User.Identity.GetUserId()), Message = "Cycle is locked." });
                return "Cycle is locked.";

            }
            var patient = await _db.Patients.FindAsync(contact.PatientId);
            if (patient != null && ModelState.IsValid)
            {
                patient.MobilePhoneNumber = contact.CellPhoneNumber;
                patient.AllowText = contact.CellPhonePermission;
                patient.WorkPhoneNumber = contact.WorkPhoneNumber;
                patient.HomePhoneNumber = contact.HomePhoneNumber;
                patient.Email = contact.Email;
                patient.AllowEmail = contact.EmailPermission;

                if (patient.ContactId != null)
                    _db.Entry(contact).State = EntityState.Modified;

                else
                {
                    _db.PatientProfile_Contact.Add(contact);
                    await _db.SaveChangesAsync();

                    patient.ContactId = contact.Id;
                }

                patient.UpdatedBy = User.Identity.GetUserId();
                patient.UpdatedOn = DateTime.Now;
                _db.Entry(patient).State = EntityState.Modified;
                await _db.SaveChangesAsync();

                //return RedirectToAction("Create", "PatientAddress", new { patientId = patient?.Id });
                return "True";
            }
            else
            {
                var errorList = ModelState.Values.SelectMany(m => m.Errors)
                                 .Select(e => e.ErrorMessage)
                                 .ToList();
                var errorstr =  string.Join(",", errorList);
                return errorstr;
            }
            ViewBag.PatientName = patient?.FirstName + " " + patient?.LastName;
            ViewBag.PatientId = patient?.Id;
            ViewBag.CcmStatus = patient?.CcmStatus;

            //return View(contact);
            return "False";
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
