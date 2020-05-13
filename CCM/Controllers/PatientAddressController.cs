using System;
using CCM.Models;
using System.Web.Mvc;
using System.Data.Entity;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using CCM.Helpers;

namespace CCM.Controllers
{
    [Authorize(Roles = "Liaison, Admin, QAQC, PhysiciansGroup, LiaisonGroup, Sales")]
    public class PatientAddressController : BaseController
    {
       // private readonly ApplicationdbContect _db = new ApplicationdbContect();



        public async Task<ActionResult> Create(int patientId)
        {
            var patient = await _db.Patients.FindAsync(patientId);
            var patientAddress = patient?.AddressId != null
                               ? await _db.PatientProfile_Addresses.FindAsync(patient.AddressId)
                               : new PatientProfile_Address
                                 {
                                     PatientId           = patientId,
                                     Address1            = patient?.Address1,
                                     Address2            = patient?.Address2,
                                     City                = patient?.City,
                                     State               = patient?.State,
                                     Zip                 = patient?.Zipcode,
                                     BuildingType        = patient?.BuildingType,
                                     DeliveryPermisison  = patient?.DeliveryPermisison,
                                     DeliveryInstruction = patient?.DeliveryInstruction
                                 };

            ViewBag.PatientName = patient?.FirstName + " " + patient?.LastName;
            ViewBag.PatientId   = patient?.Id;
            ViewBag.CcmStatus   = patient?.CcmStatus;

            if (User.IsInRole("Liaison"))
                ViewBag.ReviewId = HelperExtensions.ReviewTimeGet("Address", patient?.Id, User.Identity.GetUserId());

            return View(patientAddress);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Liaison, Admin")]
        public async Task<ActionResult> Create(PatientProfile_Address address)
        {
            if (HelperExtensions.isAllowedforEditingorAdd(address.PatientId, CategoryCycleStatusHelper.GetPatientNewOrOldCycleByCategory(address.PatientId, BillingCodeHelper.cmmBillingCatagoryid), User.Identity.GetUserId()) == false)
            {
                return RedirectToAction("Index", "CcmStatus", new { status = HelperExtensions.GetStatusRedirectionbyUser(User.Identity.GetUserId()), Message = "Cycle is locked." });
            }
            var patient  = await _db.Patients.FindAsync(address.PatientId);
            if (patient != null && ModelState.IsValid)
            {
                patient.Address1            = address.Address1;
                patient.Address2            = address.Address2;
                patient.City                = address.City;
                patient.State               = address.State;
                patient.Zipcode             = address.Zip;
                patient.BuildingType        = address.BuildingType;
                patient.DeliveryPermisison  = address.DeliveryPermisison;
                patient.DeliveryInstruction = address.DeliveryInstruction;

                if (patient.AddressId != null)
                    _db.Entry(address).State = EntityState.Modified;

                else
                {
                    _db.PatientProfile_Addresses.Add(address);
                    await _db.SaveChangesAsync();

                    patient.AddressId = address.Id;
                }

                patient.UpdatedBy = User.Identity.GetUserId();
                patient.UpdatedOn = DateTime.Now;
                _db.Entry(patient).State = EntityState.Modified;
                await _db.SaveChangesAsync();

                return RedirectToAction("Create", "PatientUrgencyContact", new { patientId = patient.Id });
               
            }

            var sameAddress = new PatientProfile_Address
            {
                PatientId           = address.PatientId,
                Address1            = patient?.Address1,
                Address2            = patient?.Address2,
                City                = patient?.City,
                State               = patient?.State,
                Zip                 = patient?.Zipcode,
                BuildingType        = patient?.BuildingType,
                DeliveryPermisison  = patient?.DeliveryPermisison,
                DeliveryInstruction = patient?.DeliveryInstruction
            };

            ViewBag.PatientName = patient?.FirstName + " " + patient?.LastName;
            ViewBag.PatientId   = patient?.Id;
            ViewBag.CcmStatus   = patient?.CcmStatus;

            return View(sameAddress);
        }




        public async Task<PartialViewResult> _Create(int patientId)
        {
            var patient = await _db.Patients.FindAsync(patientId);
            var patientAddress = patient?.AddressId != null
                               ? await _db.PatientProfile_Addresses.FindAsync(patient.AddressId)
                               : new PatientProfile_Address
                               {
                                   PatientId = patientId,
                                   Address1 = patient?.Address1,
                                   Address2 = patient?.Address2,
                                   City = patient?.City,
                                   State = patient?.State,
                                   Zip = patient?.Zipcode,
                                   BuildingType = patient?.BuildingType,
                                   DeliveryPermisison = patient?.DeliveryPermisison,
                                   DeliveryInstruction = patient?.DeliveryInstruction
                               };

            ViewBag.PatientName = patient?.FirstName + " " + patient?.LastName;
            ViewBag.PatientId = patient?.Id;
            ViewBag.CcmStatus = patient?.CcmStatus;

            //if (User.IsInRole("Liaison"))
            //    ViewBag.ReviewId = HelperExtensions.ReviewTimeGet("Address", patient?.Id, User.Identity.GetUserId());

            return PartialView(patientAddress);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Liaison, Admin,Sales")]
        public async Task<string> _Create(PatientProfile_Address address)
        {
            if (HelperExtensions.isAllowedforEditingorAdd(address.PatientId, CategoryCycleStatusHelper.GetPatientNewOrOldCycleByCategory(address.PatientId, BillingCodeHelper.cmmBillingCatagoryid), User.Identity.GetUserId()) == false)
            {
                return "Cycle is locked.";
            }
            var patient = await _db.Patients.FindAsync(address.PatientId);
            if (patient != null && ModelState.IsValid)
            {
                patient.Address1 = address.Address1;
                patient.Address2 = address.Address2;
                patient.City = address.City;
                patient.State = address.State;
                patient.Zipcode = address.Zip;
                patient.BuildingType = address.BuildingType;
                patient.DeliveryPermisison = address.DeliveryPermisison;
                patient.DeliveryInstruction = address.DeliveryInstruction;

                if (patient.AddressId != null)
                    _db.Entry(address).State = EntityState.Modified;

                else
                {
                    _db.PatientProfile_Addresses.Add(address);
                    await _db.SaveChangesAsync();

                    patient.AddressId = address.Id;
                }

                patient.UpdatedBy = User.Identity.GetUserId();
                patient.UpdatedOn = DateTime.Now;
                _db.Entry(patient).State = EntityState.Modified;
                await _db.SaveChangesAsync();

                return "True";
            }

            var sameAddress = new PatientProfile_Address
            {
                PatientId = address.PatientId,
                Address1 = patient?.Address1,
                Address2 = patient?.Address2,
                City = patient?.City,
                State = patient?.State,
                Zip = patient?.Zipcode,
                BuildingType = patient?.BuildingType,
                DeliveryPermisison = patient?.DeliveryPermisison,
                DeliveryInstruction = patient?.DeliveryInstruction
            };

            ViewBag.PatientName = patient?.FirstName + " " + patient?.LastName;
            ViewBag.PatientId = patient?.Id;
            ViewBag.CcmStatus = patient?.CcmStatus;

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
