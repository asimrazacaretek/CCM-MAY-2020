using System;
using CCM.Models;
using System.Linq;
using System.Web.Mvc;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using CCM.Helpers;

namespace CCM.Controllers
{
    [Authorize(Roles = "Liaison, Admin, QAQC, PhysiciansGroup, LiaisonGroup,Sales")]
    public class CurrentMedicationController : BaseController
    {
        //private readonly ApplicationdbContect _db = new ApplicationdbContect();


        public async Task<ActionResult> Patient(int patientId)
        {
            var patient = _db.Patients.Find(patientId);

            ViewBag.PatientName = patient?.FirstName + " " + patient?.LastName;
            ViewBag.PatientId   = patient?.Id;
            ViewBag.CcmStatus   = patient?.CcmStatus;

            if (User.IsInRole("Liaison"))
                ViewBag.ReviewId = HelperExtensions.ReviewTimeGet("Current Medications", patient?.Id, User.Identity.GetUserId());

            return View( patient?.MedicationOtcId != null
                       ? await _db.PatientMedicalHistory_MedicationOTCs.FindAsync(patient.MedicationOtcId)
                       : new PatientMedicalHistory_MedicationOTC { PatientId = patientId }
                       );
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Liaison, Admin")]
        public async Task<string> Patient(PatientMedicalHistory_MedicationOTC medicationOtc)
        {
            if (HelperExtensions.isAllowedforEditingorAdd(medicationOtc.PatientId, CategoryCycleStatusHelper.GetPatientNewOrOldCycleByCategory(medicationOtc.PatientId, BillingCodeHelper.cmmBillingCatagoryid), User.Identity.GetUserId()) == false)
            {
                //return RedirectToAction("Index", "CcmStatus", new { status = HelperExtensions.GetStatusRedirectionbyUser(User.Identity.GetUserId()), Message = "Cycle is locked." });

                return "Cycle is locked.";
            }
            var patient  = _db.Patients.Find(medicationOtc.PatientId);
            if (patient != null && ModelState.IsValid)
            {
                if (patient.MedicationOtcId != null)
                    _db.Entry(medicationOtc).State = EntityState.Modified;

                else
                {
                    _db.PatientMedicalHistory_MedicationOTCs.Add(medicationOtc);
                    await _db.SaveChangesAsync();

                    patient.MedicationOtcId = medicationOtc.Id;
                }

                patient.UpdatedBy = User.Identity.GetUserId();
                patient.UpdatedOn = DateTime.Now;
                _db.Entry(patient).State = EntityState.Modified;
                await _db.SaveChangesAsync();


                return "True";
                //return RedirectToAction("Create", "PatientWorkAndRelationship", new { patientId = patient.Id });
            }

            ViewBag.PatientName = patient?.FirstName + " " + patient?.LastName;
            ViewBag.PatientId   = patient?.Id;
            ViewBag.CcmStatus   = patient?.CcmStatus;

            return "False";
            //return View(medicationOtc);
        }


        public async Task<PartialViewResult> _Patient(int patientId)
        {
            var patient = _db.Patients.Find(patientId);

            ViewBag.PatientName = patient?.FirstName + " " + patient?.LastName;
            ViewBag.PatientId = patient?.Id;
            ViewBag.CcmStatus = patient?.CcmStatus;

            if (User.IsInRole("Liaison"))
                ViewBag.ReviewId = HelperExtensions.ReviewTimeGet("Current Medications", patient?.Id, User.Identity.GetUserId());

            return PartialView(patient?.MedicationOtcId != null
                       ? await _db.PatientMedicalHistory_MedicationOTCs.FindAsync(patient.MedicationOtcId)
                       : new PatientMedicalHistory_MedicationOTC { PatientId = patientId }
                       );
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Liaison, Admin")]
        public async Task<string> _Patient(PatientMedicalHistory_MedicationOTC medicationOtc)
        {
            if (HelperExtensions.isAllowedforEditingorAdd(medicationOtc.PatientId, CategoryCycleStatusHelper.GetPatientNewOrOldCycleByCategory(medicationOtc.PatientId, BillingCodeHelper.cmmBillingCatagoryid), User.Identity.GetUserId()) == false)
            {
                //return RedirectToAction("Index", "CcmStatus", new { status = HelperExtensions.GetStatusRedirectionbyUser(User.Identity.GetUserId()), Message = "Cycle is locked." });

                return "Cycle is locked.";
            }
            var patient = _db.Patients.Find(medicationOtc.PatientId);
            if (patient != null && ModelState.IsValid)
            {
                if (patient.MedicationOtcId != null)
                    _db.Entry(medicationOtc).State = EntityState.Modified;

                else
                {
                    _db.PatientMedicalHistory_MedicationOTCs.Add(medicationOtc);
                    await _db.SaveChangesAsync();

                    patient.MedicationOtcId = medicationOtc.Id;
                }

                patient.UpdatedBy = User.Identity.GetUserId();
                patient.UpdatedOn = DateTime.Now;
                _db.Entry(patient).State = EntityState.Modified;
                await _db.SaveChangesAsync();


                return "True";
                //return RedirectToAction("Create", "PatientWorkAndRelationship", new { patientId = patient.Id });
            }

            ViewBag.PatientName = patient?.FirstName + " " + patient?.LastName;
            ViewBag.PatientId = patient?.Id;
            ViewBag.CcmStatus = patient?.CcmStatus;

            return "False";
            //return View(medicationOtc);
        }



        public async Task<PartialViewResult> _GeneralConditionsPartial(int patientId)
        {
            return PartialView( await _db.PatientMedicalHistory_GeneralConditions.FirstOrDefaultAsync(gc => gc.PatientId == patientId)
                             ?? new PatientMedicalHistory_GeneralCondition { PatientId = patientId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Liaison, Admin")]
        public async Task<string> _GeneralConditions(PatientMedicalHistory_GeneralCondition conditions)
        {
            if (HelperExtensions.isAllowedforEditingorAdd(conditions.PatientId, CategoryCycleStatusHelper.GetPatientNewOrOldCycleByCategory(conditions.PatientId,BillingCodeHelper.cmmBillingCatagoryid), User.Identity.GetUserId()) == false)
            {
                //return RedirectToAction("Index", "CcmStatus", new { status = HelperExtensions.GetStatusRedirectionbyUser(User.Identity.GetUserId()), Message = "Cycle is locked." });
                return "Cycle is locked.";
            }
            var patient = await _db.Patients.FindAsync(conditions.PatientId);
            if (patient != null)
            {
                patient.UpdatedBy = User.Identity.GetUserId();
                patient.UpdatedOn = DateTime.Now;
                if (string.IsNullOrEmpty(conditions.Date.ToString()))
                    conditions.Date = DateTime.Now;
                var gconditions = _db.PatientMedicalHistory_GeneralConditions.AsNoTracking().Where(x => x.PatientId == conditions.PatientId).FirstOrDefault();
                if (gconditions == null)
                {
                    _db.PatientMedicalHistory_GeneralConditions.Add(conditions);
                }
                else
                {
                    _db.Entry(conditions).State = EntityState.Modified;

                }
                //_db.Set<PatientMedicalHistory_GeneralCondition>().AddOrUpdate(conditions);
                await _db.SaveChangesAsync();
                return "True";
            }

            ViewBag.PatientName = patient?.FirstName + " " + patient?.LastName;
            ViewBag.PatientId = patient?.Id;
            ViewBag.CcmStatus = patient?.CcmStatus;
            ViewBag.CurrentPage = "Medical History";

            //return RedirectToAction("Patient", "CurrentMedication", new { patientId = conditions.PatientId });
            return "False";
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Liaison, Admin")]
        public async Task<ActionResult> GeneralConditions(PatientMedicalHistory_GeneralCondition conditions)
        {
            if (HelperExtensions.isAllowedforEditingorAdd(conditions.PatientId, CategoryCycleStatusHelper.GetPatientNewOrOldCycleByCategory(conditions.PatientId, BillingCodeHelper.cmmBillingCatagoryid), User.Identity.GetUserId()) == false)
            {
                return RedirectToAction("Index", "CcmStatus", new { status = HelperExtensions.GetStatusRedirectionbyUser(User.Identity.GetUserId()), Message = "Cycle is locked." });

            }
            var patient  = await _db.Patients.FindAsync(conditions.PatientId);
            if (patient != null)
            {
                patient.UpdatedBy = User.Identity.GetUserId();
                patient.UpdatedOn = DateTime.Now;
                var gconditions = _db.PatientMedicalHistory_GeneralConditions.Where(x => x.PatientId == conditions.PatientId).FirstOrDefault();
                if (gconditions == null)
                {
                    _db.PatientMedicalHistory_GeneralConditions.Add(conditions);
                }
                else
                {
                    _db.Entry(conditions).State = EntityState.Modified;

                }
                //_db.Set<PatientMedicalHistory_GeneralCondition>().AddOrUpdate(conditions);
                await _db.SaveChangesAsync();
            }

            ViewBag.PatientName = patient?.FirstName + " " + patient?.LastName;
            ViewBag.PatientId   = patient?.Id;
            ViewBag.CcmStatus   = patient?.CcmStatus;
            ViewBag.CurrentPage = "Medical History";

            return RedirectToAction("Patient", "CurrentMedication", new { patientId = conditions.PatientId });
        }


      
        public async Task<PartialViewResult> _MedicationsListPartial(int patientId)
        {
            //var medications = _db.PatientMedicalHistory_MedicationRxes.Where(m => m.PatientId == patientId).Include(.ToListAsync();
          
            var medication = (from m in _db.PatientMedicalHistory_MedicationRxes
                              join d in _db.MedicationDoseRepetitionTimes on m.DoseRepetitionTime equals d.Id.ToString() into drpt
                              from d in drpt.DefaultIfEmpty()
                              join d1 in _db.MedicationDoseRepetitionTimes on m.TakenMedicineProperly equals d1.Id.ToString() into drpt1
                              from d1 in drpt1.DefaultIfEmpty()
                              where m.PatientId == patientId 
                              select new PatientMedicalHistory_MedicationRxViewModel
                              {
                                  Comments = m.Comments,
                                  DailyDose = m.DailyDose,
                                  DoseRepetitionTime = d.DoseRepetitionTime ?? "",
                                  DrugName = m.DrugName,
                                  EndDate = m.EndDate,
                                  ForHowLong = m.ForHowLong,
                                  Id = m.Id,
                                  IsMedicineDiscontinued = m.IsMedicineDiscontinued,
                                  IsPermanentMedicine = m.IsPermanentMedicine,
                                  IssuesIdentified = m.IssuesIdentified,
                                  IsTakenMedicineProperly = m.IsTakenMedicineProperly,
                                  PatientId = m.PatientId,
                                  PrescribeBy = m.PrescribeBy,
                                  RateQuantity = m.RateQuantity,
                                  Route = m.Route,
                                  RxCuis = m.RxCuis,
                                  StartDate = m.StartDate,
                                  StopReason = m.StopReason,
                                  TakenMedicineProperly = d1.DoseRepetitionTime ?? "",
                                  UseReason = m.UseReason

                              }).ToList();

            return PartialView(medication);
            /*return PartialView(await _db.PatientMedicalHistory_MedicationRxes.Where(m => m.PatientId == patientId).ToListAsync())*/
            ;
        }
        public async Task<PartialViewResult> MedicationListPartialNew(int patientId)
        {
            var medication = (from m in _db.PatientMedicalHistory_MedicationRxes
                              join d in _db.MedicationDoseRepetitionTimes on m.DoseRepetitionTime equals d.Id.ToString() into drpt
                              from d in drpt.DefaultIfEmpty()
                              join d1 in _db.MedicationDoseRepetitionTimes on m.TakenMedicineProperly equals d1.Id.ToString() into drpt1
                              from d1 in drpt1.DefaultIfEmpty()
                              where m.PatientId == patientId
                              select new PatientMedicalHistory_MedicationRxViewModel
                              {
                                  Comments = m.Comments,
                                  DailyDose = m.DailyDose,
                                  DoseRepetitionTime = d.DoseRepetitionTime??"",
                                  DrugName = m.DrugName,
                                  EndDate = m.EndDate,
                                  ForHowLong = m.ForHowLong,
                                  Id = m.Id,
                                  IsMedicineDiscontinued = m.IsMedicineDiscontinued,
                                  IsPermanentMedicine = m.IsPermanentMedicine,
                                  IssuesIdentified = m.IssuesIdentified,
                                  IsTakenMedicineProperly = m.IsTakenMedicineProperly,
                                  PatientId = m.PatientId,
                                  PrescribeBy = m.PrescribeBy,
                                  RateQuantity = m.RateQuantity,
                                  Route = m.Route,
                                  RxCuis = m.RxCuis,
                                  StartDate = m.StartDate,
                                  StopReason = m.StopReason,
                                  TakenMedicineProperly = d1.DoseRepetitionTime??"",
                                  UseReason = m.UseReason

                              }).ToList();

            return PartialView(medication);
           // return PartialView(await _db.PatientMedicalHistory_MedicationRxes.Where(m => m.PatientId == patientId).ToListAsync());
        }

        public PartialViewResult _AddMedicationPartial(int patientId)
        {
            var physicians = _db.Physicians.Select(p => new SelectListItem { Value = p.Id.ToString(), Text = p.FirstName + " " + p.LastName });
            //ViewBag.Physicians = new SelectList(physicians.OrderBy(p => p.Text).ToList(), "Value", "Text");//, patient.PhysicianId
            //ViewBag.MedicineRepitinDose = new SelectList(_db.MedicationDoseRepetitionTimes.ToList(), "Id", "DoseRepetitionTime");
            //ViewBag.MedicationRoute = new SelectList(_db.MedicationRoutes.ToList(), "Id", "RouteItme");

            ViewBag.Physicians = new SelectList(physicians.OrderBy(p => p.Text).ToList(), "Text", "Text");
            ViewBag.MedicineRepitinDose = new SelectList(_db.MedicationDoseRepetitionTimes.ToList(), "Id", "DoseRepetitionTime");
            ViewBag.MedicationRoute = new SelectList(_db.MedicationRoutes.ToList(), "RouteItme", "RouteItme");
            return PartialView(new PatientMedicalHistory_MedicationRx { PatientId = patientId });
        }

        public PartialViewResult _EditMedicationPartial(int medicationId)
        {
            var physicians = _db.Physicians.Select(p => new SelectListItem { Value = p.Id.ToString(), Text = p.FirstName + " " + p.LastName });
            ViewBag.Physicians = new SelectList(physicians.OrderBy(p => p.Text).ToList(), "Text", "Text");//, patient.PhysicianId
            ViewBag.MedicineRepitinDose = new SelectList(_db.MedicationDoseRepetitionTimes.ToList(), "Id", "DoseRepetitionTime");
            ViewBag.MedicationRoute = new SelectList(_db.MedicationRoutes.ToList(), "RouteItme", "RouteItme");

            PatientMedicalHistory_MedicationRx model = _db.PatientMedicalHistory_MedicationRxes.Where(m => m.Id == medicationId).First();


            //PatientMedicalHistory_MedicationRx model = new PatientMedicalHistory_MedicationRx();
            //model.DrugName = model1.DrugName;
            //model.DailyDose = model1.DailyDose;
            //model.RxCuis = model1.RxCuis;
            //model.RateQuantity = model1.RateQuantity;
            //model.Route = model1.Route;
            //model.UseReason = model1.UseReason;
            //model.StartDate = model1.StartDate;
            //model.DoseRepetitionTime = model1.DoseRepetitionTime;
            //model.IssuesIdentified = model1.IssuesIdentified;
            //model.PrescribeBy = model1.PrescribeBy;
            //model.Comments = model1.Comments;
            //model.ForHowLong = model1.ForHowLong;
            //model.IsTakenMedicineProperly = model1.IsTakenMedicineProperly;
            //model.TakenMedicineProperly = model1.TakenMedicineProperly;
            return PartialView(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Liaison, Admin")]
        public async Task<ActionResult> AddMedication(PatientMedicalHistory_MedicationRx medicationRx)
        {
            if (HelperExtensions.isAllowedforEditingorAdd(medicationRx.PatientId, CategoryCycleStatusHelper.GetPatientNewOrOldCycleByCategory(medicationRx.PatientId, BillingCodeHelper.cmmBillingCatagoryid), User.Identity.GetUserId()) == false)
            {
                return RedirectToAction("Index", "CcmStatus", new { status = HelperExtensions.GetStatusRedirectionbyUser(User.Identity.GetUserId()), Message = "Cycle is locked." });

            }
            var patient  = _db.Patients.Find(medicationRx.PatientId);
            if (patient != null && ModelState.IsValid)
            {
                patient.UpdatedBy = User.Identity.GetUserId();
                patient.UpdatedOn = DateTime.Now;


                medicationRx.EndDate = DateTime.Now;
                _db.PatientMedicalHistory_MedicationRxes.Add(medicationRx);
                await _db.SaveChangesAsync();
            }

            ViewBag.PatientName = patient?.FirstName + " " + patient?.LastName;
            ViewBag.PatientId   = patient?.Id;
            ViewBag.CcmStatus   = patient?.CcmStatus;
            ViewBag.CurrentPage = "Medical History";

            return RedirectToAction("Patient", "CurrentMedication", new { patientId = medicationRx.PatientId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Liaison, Admin,Sales")]
        public async Task<string> _AddMedication(PatientMedicalHistory_MedicationRx medicationRx)
        {
            if (HelperExtensions.isAllowedforEditingorAdd(medicationRx.PatientId, CategoryCycleStatusHelper.GetPatientNewOrOldCycleByCategory(medicationRx.PatientId, BillingCodeHelper.cmmBillingCatagoryid), User.Identity.GetUserId()) == false)
            {
                //return RedirectToAction("Index", "CcmStatus", new { status = HelperExtensions.GetStatusRedirectionbyUser(User.Identity.GetUserId()), Message = "Cycle is locked." });

                return "Cycle is locked.";
            }
            var patient = _db.Patients.Find(medicationRx.PatientId);
            if (patient != null && ModelState.IsValid)
            {
                patient.UpdatedBy = User.Identity.GetUserId();
                patient.UpdatedOn = DateTime.Now;


                medicationRx.EndDate = DateTime.Now;
                _db.PatientMedicalHistory_MedicationRxes.Add(medicationRx);
                await _db.SaveChangesAsync();
                return "True";
            }

            ViewBag.PatientName = patient?.FirstName + " " + patient?.LastName;
            ViewBag.PatientId = patient?.Id;
            ViewBag.CcmStatus = patient?.CcmStatus;
            ViewBag.CurrentPage = "Medical History";

            // return RedirectToAction("Patient", "CurrentMedication", new { patientId = medicationRx.PatientId });
            return "False";
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Liaison, Admin")]
        public async Task<ActionResult> EditMedication(PatientMedicalHistory_MedicationRx medicationRx)
        {
            if (HelperExtensions.isAllowedforEditingorAdd(medicationRx.PatientId, CategoryCycleStatusHelper.GetPatientNewOrOldCycleByCategory(medicationRx.PatientId, BillingCodeHelper.cmmBillingCatagoryid), User.Identity.GetUserId()) == false)
            {
                return RedirectToAction("Index", "CcmStatus", new { status = HelperExtensions.GetStatusRedirectionbyUser(User.Identity.GetUserId()), Message = "Cycle is locked." });

            }
            var patient = _db.Patients.Find(medicationRx.PatientId);
            if (patient != null && ModelState.IsValid)
            {
                patient.UpdatedBy = User.Identity.GetUserId();
                patient.UpdatedOn = DateTime.Now;
                
                _db.Entry(medicationRx).State = EntityState.Modified;
                
                await _db.SaveChangesAsync();
            }

            ViewBag.PatientName = patient?.FirstName + " " + patient?.LastName;
            ViewBag.PatientId = patient?.Id;
            ViewBag.CcmStatus = patient?.CcmStatus;
            ViewBag.CurrentPage = "Medical History";
            
            return RedirectToAction("Patient", "CurrentMedication", new { patientId = medicationRx.PatientId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Liaison, Admin,Sales")]
        public async Task<string> _EditMedication(PatientMedicalHistory_MedicationRx medicationRx)
        {
            if (HelperExtensions.isAllowedforEditingorAdd(medicationRx.PatientId, CategoryCycleStatusHelper.GetPatientNewOrOldCycleByCategory(medicationRx.PatientId, BillingCodeHelper.cmmBillingCatagoryid), User.Identity.GetUserId()) == false)
            {
                //return RedirectToAction("Index", "CcmStatus", new { status = HelperExtensions.GetStatusRedirectionbyUser(User.Identity.GetUserId()), Message = "Cycle is locked." });
                return "Cycle is locked.";

            }
            var patient = _db.Patients.Find(medicationRx.PatientId);
            if (patient != null && ModelState.IsValid)
            {
                patient.UpdatedBy = User.Identity.GetUserId();
                patient.UpdatedOn = DateTime.Now;

                _db.Entry(medicationRx).State = EntityState.Modified;

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
            ViewBag.PatientName = patient?.FirstName + " " + patient?.LastName;
            ViewBag.PatientId = patient?.Id;
            ViewBag.CcmStatus = patient?.CcmStatus;
            ViewBag.CurrentPage = "Medical History";

            //return RedirectToAction("Patient", "CurrentMedication", new { patientId = medicationRx.PatientId });
        }

        [Authorize(Roles = "Liaison, Admin")]
        public async Task<ActionResult> DeleteRx(int id)
        {
            var medication  = await _db.PatientMedicalHistory_MedicationRxes.FindAsync(id);
            if (medication != null)
            {
                _db.PatientMedicalHistory_MedicationRxes.Remove(medication);
                await _db.SaveChangesAsync();
            }

            var patient = await _db.Patients.FindAsync(medication?.PatientId);
            ViewBag.PatientName = patient?.FirstName + " " + patient?.LastName;
            ViewBag.PatientId   = patient?.Id;
            ViewBag.CcmStatus   = patient?.CcmStatus;

            return RedirectToAction("Patient", "CurrentMedication", new { patientId = medication?.PatientId });
        }

        [Authorize(Roles = "Liaison, Admin,Sales")]
        public async Task<string> _DeleteRx(int id)
        {
            var medication = await _db.PatientMedicalHistory_MedicationRxes.FindAsync(id);
            if (medication != null)
            {
                _db.PatientMedicalHistory_MedicationRxes.Remove(medication);
                await _db.SaveChangesAsync();
                return "True";
            }


            return "False";
            //var patient = await _db.Patients.FindAsync(medication?.PatientId);
            //ViewBag.PatientName = patient?.FirstName + " " + patient?.LastName;
            //ViewBag.PatientId = patient?.Id;
            //ViewBag.CcmStatus = patient?.CcmStatus;

            //return RedirectToAction("Patient", "CurrentMedication", new { patientId = medication?.PatientId });
        }

        public async Task<ActionResult> EditRx(int id)
        {
            
            var medication = await _db.PatientMedicalHistory_MedicationRxes.FindAsync(id);
            /*
            if (medication != null)
            {
                _db.PatientMedicalHistory_MedicationRxes.Remove(medication);
                await _db.SaveChangesAsync();
            }

            var patient = await _db.Patients.FindAsync(medication?.PatientId);
            ViewBag.PatientName = patient?.FirstName + " " + patient?.LastName;
            ViewBag.PatientId = patient?.Id;
            ViewBag.CcmStatus = patient?.CcmStatus;
            */

            return RedirectToAction("Patient", "CurrentMedication", new { patientId = medication?.PatientId, medicationId = id });
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