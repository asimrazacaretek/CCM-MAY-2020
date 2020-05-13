using CCM.Helpers;
using CCM.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;


namespace CCM.Controllers
{
    [Authorize(Roles = "Liaison, Physician, PhysiciansGroup, Admin, QAQC, LiaisonGroup")]
    public class FinalCarePlanNotesController : BaseController
    {
        //private readonly ApplicationdbContect _db = new ApplicationdbContect();

        public async Task<ActionResult> Index(int patientId)
        {
            var patient = _db.Patients.Find(patientId);
            if (patient == null)
            {
                ViewBag.message = "Patient Not Found in the System- Please Contact Admin.";
                return View("Error");
            }


            ViewBag.PatientName = patient?.FirstName + " " + patient?.LastName;
            ViewBag.PatientId = patient?.Id;

            ViewBag.CcmStatus = patient?.CcmStatus;
            ViewBag.Cycle = patient.Cycle;
            if (User.IsInRole("Liaison"))
                ViewBag.ReviewId = HelperExtensions.ReviewTimeGet("Final Care Plan Notes", patient?.Id, User.Identity.GetUserId());


            return View(await _db.FinalCarePlanNotes.Where(fcp => fcp.PatientId == patientId).ToListAsync());
        }

        public async Task<PartialViewResult> _Index(int patientId)
        {
            var patient = _db.Patients.Find(patientId);
            if (patient == null)
            {
                ViewBag.message = "Patient Not Found in the System- Please Contact Admin.";
                return PartialView("Error");
            }


            ViewBag.PatientName = patient?.FirstName + " " + patient?.LastName;
            ViewBag.PatientId = patient?.Id;

            ViewBag.CcmStatus = patient?.CcmStatus;
            ViewBag.Cycle = patient.Cycle;
            //if (User.IsInRole("Liaison"))
            //    ViewBag.ReviewId = HelperExtensions.ReviewTimeGet("Final Care Plan Notes", patient?.Id, User.Identity.GetUserId());


            return PartialView(await _db.FinalCarePlanNotes.Where(fcp => fcp.PatientId == patientId).ToListAsync());
        }

        public async Task<ActionResult> Create(int patientId)
        {

            var patient = _db.Patients.Find(patientId);
            var currentcycle = CategoryCycleStatusHelper.GetPatientNewOrOldCycleByCategory(patient.Id, BillingCodeHelper.cmmBillingCatagoryid);
            if (currentcycle == 0)
            {
                return RedirectToAction("Details", "Patient", new { id = patient.Id, status = "Cannot generate final care plan until patient is enrolled." });
            }
            ViewBag.PatientName = patient?.FirstName + " " + patient?.LastName;
            ViewBag.PatientId = patient?.Id;
            ViewBag.CcmStatus = patient?.CcmStatus;
            if (User.IsInRole("Liaison"))
                ViewBag.ReviewId = HelperExtensions.ReviewTimeGet("Final Care Plan Notes", patient?.Id, User.Identity.GetUserId());
            var carePlanPatient = await _db.FinalCarePlanNotes.AsNoTracking().Where(p => p.PatientId == patientId).OrderByDescending(item => item.Id).ToListAsync();
            if (carePlanPatient.Count > 0)
            {
                var lastcareplan = carePlanPatient[0];
                return View(lastcareplan);
                //if (lastcareplan.CarePlanCreatedOn.Value.Month == DateTime.Now.Month && lastcareplan.CarePlanCreatedOn.Value.Year == DateTime.Now.Year)

                //{

                //}
                //else
                //{

                //}
            }
            return View(new FinalCarePlanNotes { PatientId = patientId ,Cycle=currentcycle});
        }

        public async Task<string> _Create(int patientId)
        {
            var patient = _db.Patients.Find(patientId);
            var currentcycle = CategoryCycleStatusHelper.GetPatientNewOrOldCycleByCategory(patient.Id, BillingCodeHelper.cmmBillingCatagoryid);
            if (currentcycle == 0)
            {
                return "Cannot generate final care plan until patient is enrolled.";
                //return PartialView("Details", "Patient", new { id = patient.Id, status = "Cannot generate final care plan until patient is enrolled." });
            }
            ViewBag.PatientName = patient?.FirstName + " " + patient?.LastName;
            ViewBag.PatientId = patient?.Id;
            ViewBag.CcmStatus = patient?.CcmStatus;
            //if (User.IsInRole("Liaison"))
            //    ViewBag.ReviewId = HelperExtensions.ReviewTimeGet("Final Care Plan Notes", patient?.Id, User.Identity.GetUserId());
            var carePlanPatient = await _db.FinalCarePlanNotes.AsNoTracking().Where(p => p.PatientId == patientId).OrderByDescending(item => item.Id).ToListAsync();
            if (carePlanPatient.Count > 0)
            {
                var lastcareplan = carePlanPatient[0];
                return ConvertViewToString("_Create", lastcareplan);
               // return PartialView(lastcareplan);
                //if (lastcareplan.CarePlanCreatedOn.Value.Month == DateTime.Now.Month && lastcareplan.CarePlanCreatedOn.Value.Year == DateTime.Now.Year)

                //{

                //}
                //else
                //{

                //}
            }
            return ConvertViewToString("_Create", new FinalCarePlanNotes { PatientId = patientId, Cycle = currentcycle });

            //return PartialView(new FinalCarePlanNotes { PatientId = patientId, Cycle = currentcycle });
        }
        private string ConvertViewToString(string viewName, object model)
        {
            try
            {


                ViewData.Model = model;
                using (StringWriter writer = new StringWriter())
                {
                    ViewEngineResult vResult = ViewEngines.Engines.FindPartialView(ControllerContext, viewName);
                    ViewContext vContext = new ViewContext(this.ControllerContext, vResult.View, ViewData, new TempDataDictionary(), writer);
                    vResult.View.Render(vContext, writer);
                    return writer.ToString();
                }
            }
            catch (Exception ed)
            {
                return "";

            }
        }
        [HttpPost, ValidateInput(false)]
        [Authorize(Roles = "Liaison, Admin, QAQC")]
        public async Task<ActionResult> Create(FinalCarePlanNotes finalCarePlanNotes)
        {
            ViewBag.Errors = "";
            var patient = await _db.Patients.FindAsync(finalCarePlanNotes.PatientId);
            ViewBag.PatientName = patient?.FirstName + ' ' + patient?.LastName;
            ViewBag.PatientId = patient?.Id;
            ViewBag.CcmStatus = patient?.CcmStatus;
            if (ModelState.IsValid)
            {


                //if (HelperExtensions.isAllowedforEditingorAdd(finalCarePlanNotes.PatientId, CategoryCycleStatusHelper.GetPatientNewOrOldCycleByCategory(finalCarePlanNotes.PatientId), User.Identity.GetUserId()) == false)
                //{
                //    return RedirectToAction("Index", "CcmStatus", new { status = HelperExtensions.GetStatusRedirectionbyUser(User.Identity.GetUserId()), Message = "Cycle is locked." });

                //}
                var user = _db.Users.Find(User.Identity.GetUserId()).FirstName + " " + _db.Users.Find(User.Identity.GetUserId()).LastName;
                Guid obj = Guid.NewGuid();

                patient.Cycle = CategoryCycleStatusHelper.GetPatientNewOrOldCycleByCategory(patient.Id, BillingCodeHelper.cmmBillingCatagoryid);
                var cycle = patient.Cycle;
                var carePlanPatient = _db.FinalCarePlanNotes.Where(p => p.PatientId == finalCarePlanNotes.PatientId).OrderByDescending(item => item.Id).ToList();
                bool isNewone = true;
                if (carePlanPatient.Count > 0)
                {
                    var lastcareplan = carePlanPatient[0];
                    if (lastcareplan.CarePlanCreatedOn.Value.Month == DateTime.Now.Month && lastcareplan.CarePlanCreatedOn.Value.Year == DateTime.Now.Year)

                    {
                        try
                        {
                            FinalCarePlanNotes_History finalCarePlanNotes_History = new FinalCarePlanNotes_History();
                          HelperExtensions.DuckCopyShallow( finalCarePlanNotes_History, lastcareplan);
                            _db.FinalCarePlanNotes_Histories.Add(finalCarePlanNotes_History);
                            _db.SaveChanges();
                        }
                        catch (Exception ex)
                        {


                        }
                        isNewone = false;
                        var carePlan = lastcareplan;
                        carePlan.PatientId = finalCarePlanNotes.PatientId;
                        carePlan.PatientSummary = finalCarePlanNotes.PatientSummary;
                        carePlan.HealthConcerns = finalCarePlanNotes.HealthConcerns;
                        carePlan.DrugUtillizationReview = finalCarePlanNotes.DrugUtillizationReview;
                        carePlan.NutritionGoals = finalCarePlanNotes.NutritionGoals;
                        carePlan.NutritionInterventions = finalCarePlanNotes.NutritionInterventions;
                        carePlan.NutritionReview = finalCarePlanNotes.NutritionReview;
                        carePlan.PhysicalActivityGoals = finalCarePlanNotes.PhysicalActivityGoals;
                        carePlan.PhysicalActivityInterventions = finalCarePlanNotes.PhysicalActivityInterventions;
                        carePlan.PhysicalActivityReview = finalCarePlanNotes.PhysicalActivityReview;
                        carePlan.SleepGoals = finalCarePlanNotes.SleepGoals;
                        carePlan.SleepInterventions = finalCarePlanNotes.SleepInterventions;
                        carePlan.SleepReview = finalCarePlanNotes.SleepReview;
                        carePlan.MentalResilienceGoals = finalCarePlanNotes.MentalResilienceGoals;
                        carePlan.MentalResilienceInterventions = finalCarePlanNotes.MentalResilienceInterventions;
                        carePlan.MentalResilienceReview = finalCarePlanNotes.MentalResilienceReview;
                        carePlan.ManagingMedicationGoals = finalCarePlanNotes.ManagingMedicationGoals;
                        carePlan.ManagingMedicationInterventions = finalCarePlanNotes.ManagingMedicationInterventions;
                        carePlan.ManagingMedicationReview = finalCarePlanNotes.ManagingMedicationReview;
                        carePlan.BadHabitsCessationGoals = finalCarePlanNotes.BadHabitsCessationGoals;
                        carePlan.BadHabitsCessationInterventions = finalCarePlanNotes.BadHabitsCessationInterventions;
                        carePlan.BadHabitsCessationReview = finalCarePlanNotes.BadHabitsCessationReview;
                        carePlan.SideEffectsHistory = finalCarePlanNotes.SideEffectsHistory;
                        carePlan.Allergies = finalCarePlanNotes.Allergies;
                        carePlan.VerbalEducationToPatient = finalCarePlanNotes.VerbalEducationToPatient;
                        carePlan.PatientComplianceManagement = finalCarePlanNotes.PatientComplianceManagement;
                        carePlan.PharmacyTreatmentGoals = finalCarePlanNotes.PharmacyTreatmentGoals;
                        carePlan.EducationSent = finalCarePlanNotes.EducationSent;
                        carePlan.LastHospitalized = finalCarePlanNotes.LastHospitalized;
                        carePlan.HospitalizedReason = finalCarePlanNotes.HospitalizedReason;
                        carePlan.HealthRating = finalCarePlanNotes.HealthRating;
                        carePlan.LastErVisit = finalCarePlanNotes.LastErVisit;
                        carePlan.ErReason = finalCarePlanNotes.ErReason;
                        carePlan.PerformTasksRating = finalCarePlanNotes.PerformTasksRating;
                        carePlan.CarePlanEditedBy = user;
                        carePlan.CarePlanEditedOn = DateTime.Now;



                        carePlan.HospitalizationLast30Days = finalCarePlanNotes.HospitalizationLast30Days;
                        carePlan.AbilityPerformIADL = finalCarePlanNotes.AbilityPerformIADL;
                        carePlan.AssessmentVisionHearingSpeech = finalCarePlanNotes.AssessmentVisionHearingSpeech;
                        carePlan.BloodPressureGlucoseReading = finalCarePlanNotes.BloodPressureGlucoseReading;
                        carePlan.LastBloodPressureGlucoseReading = finalCarePlanNotes.LastBloodPressureGlucoseReading;
                        carePlan.FallAmbulationAssessment = finalCarePlanNotes.FallAmbulationAssessment;
                        carePlan.MedicationReconciliation = finalCarePlanNotes.MedicationReconciliation;
                        carePlan.DiabeticScreening = finalCarePlanNotes.DiabeticScreening;
                        carePlan.TobaccoAlcoholScreening = finalCarePlanNotes.TobaccoAlcoholScreening;
                        carePlan.ColonCancerScreenDate = finalCarePlanNotes.ColonCancerScreenDate;
                        carePlan.MammogramScreenDate = finalCarePlanNotes.MammogramScreenDate;
                        carePlan.UrinaryIncontinence = finalCarePlanNotes.UrinaryIncontinence;
                        carePlan.Immunization = finalCarePlanNotes.Immunization;

                        carePlan.CriticalChestPain = finalCarePlanNotes.CriticalChestPain;
                        carePlan.CriticalBreathShortness = finalCarePlanNotes.CriticalBreathShortness;
                        //newone
                        carePlan.NewWound = finalCarePlanNotes.NewWound;
                        carePlan.Bleedinggums = finalCarePlanNotes.Bleedinggums;
                        carePlan.Nosebleed = finalCarePlanNotes.Nosebleed;
                        carePlan.Bloodinurine = finalCarePlanNotes.Bloodinurine;
                        carePlan.Isurinemalodorous = finalCarePlanNotes.Isurinemalodorous;
                        carePlan.Suddenincreasedswelling = finalCarePlanNotes.Suddenincreasedswelling;
                        carePlan.SkinAssesment = finalCarePlanNotes.SkinAssesment;
                        carePlan.Diaperchange = finalCarePlanNotes.Diaperchange;
                        carePlan.DMEsupply = finalCarePlanNotes.DMEsupply;
                        carePlan.isPatientBedbound = finalCarePlanNotes.isPatientBedbound;
                        //
                        carePlan.CriticalSeverePain = finalCarePlanNotes.CriticalSeverePain;
                        carePlan.CriticalMoodChange = finalCarePlanNotes.CriticalMoodChange;
                        carePlan.CriticalSpeechChange = finalCarePlanNotes.CriticalSpeechChange;
                        carePlan.CriticalHeadache = finalCarePlanNotes.CriticalHeadache;
                        carePlan.CriticalBloodSugar = finalCarePlanNotes.CriticalBloodSugar;
                        carePlan.CriticalBloodPressure = finalCarePlanNotes.CriticalBloodPressure;
                        _db.Entry(carePlan).State = EntityState.Modified;
                        _db.SaveChanges();
                    }

                }

                if (isNewone == true)
                {
                    var carePlan = new FinalCarePlanNotes();
                    carePlan.PatientId = finalCarePlanNotes.PatientId;
                    carePlan.PatientSummary = finalCarePlanNotes.PatientSummary;
                    carePlan.HealthConcerns = finalCarePlanNotes.HealthConcerns;
                    carePlan.DrugUtillizationReview = finalCarePlanNotes.DrugUtillizationReview;
                    carePlan.NutritionGoals = finalCarePlanNotes.NutritionGoals;
                    carePlan.NutritionInterventions = finalCarePlanNotes.NutritionInterventions;
                    carePlan.NutritionReview = finalCarePlanNotes.NutritionReview;
                    carePlan.PhysicalActivityGoals = finalCarePlanNotes.PhysicalActivityGoals;
                    carePlan.PhysicalActivityInterventions = finalCarePlanNotes.PhysicalActivityInterventions;
                    carePlan.PhysicalActivityReview = finalCarePlanNotes.PhysicalActivityReview;
                    carePlan.SleepGoals = finalCarePlanNotes.SleepGoals;
                    carePlan.SleepInterventions = finalCarePlanNotes.SleepInterventions;
                    carePlan.SleepReview = finalCarePlanNotes.SleepReview;
                    carePlan.MentalResilienceGoals = finalCarePlanNotes.MentalResilienceGoals;
                    carePlan.MentalResilienceInterventions = finalCarePlanNotes.MentalResilienceInterventions;
                    carePlan.MentalResilienceReview = finalCarePlanNotes.MentalResilienceReview;
                    carePlan.ManagingMedicationGoals = finalCarePlanNotes.ManagingMedicationGoals;
                    carePlan.ManagingMedicationInterventions = finalCarePlanNotes.ManagingMedicationInterventions;
                    carePlan.ManagingMedicationReview = finalCarePlanNotes.ManagingMedicationReview;
                    carePlan.BadHabitsCessationGoals = finalCarePlanNotes.BadHabitsCessationGoals;
                    carePlan.BadHabitsCessationInterventions = finalCarePlanNotes.BadHabitsCessationInterventions;
                    carePlan.BadHabitsCessationReview = finalCarePlanNotes.BadHabitsCessationReview;
                    carePlan.SideEffectsHistory = finalCarePlanNotes.SideEffectsHistory;
                    carePlan.Allergies = finalCarePlanNotes.Allergies;
                    carePlan.VerbalEducationToPatient = finalCarePlanNotes.VerbalEducationToPatient;
                    carePlan.PatientComplianceManagement = finalCarePlanNotes.PatientComplianceManagement;
                    carePlan.PharmacyTreatmentGoals = finalCarePlanNotes.PharmacyTreatmentGoals;
                    carePlan.EducationSent = finalCarePlanNotes.EducationSent;
                    carePlan.LastHospitalized = finalCarePlanNotes.LastHospitalized;
                    carePlan.HospitalizedReason = finalCarePlanNotes.HospitalizedReason;
                    carePlan.HealthRating = finalCarePlanNotes.HealthRating;
                    carePlan.LastErVisit = finalCarePlanNotes.LastErVisit;
                    carePlan.ErReason = finalCarePlanNotes.ErReason;
                    carePlan.PerformTasksRating = finalCarePlanNotes.PerformTasksRating;
                    carePlan.CarePlanCreatedBy = user;
                    carePlan.CarePlanCreatedOn = DateTime.Now;
                    carePlan.Version = obj;
                    carePlan.Cycle = cycle;


                    carePlan.HospitalizationLast30Days = finalCarePlanNotes.HospitalizationLast30Days;
                    carePlan.AbilityPerformIADL = finalCarePlanNotes.AbilityPerformIADL;
                    carePlan.AssessmentVisionHearingSpeech = finalCarePlanNotes.AssessmentVisionHearingSpeech;
                    carePlan.BloodPressureGlucoseReading = finalCarePlanNotes.BloodPressureGlucoseReading;
                    carePlan.LastBloodPressureGlucoseReading = finalCarePlanNotes.LastBloodPressureGlucoseReading;
                    carePlan.FallAmbulationAssessment = finalCarePlanNotes.FallAmbulationAssessment;
                    carePlan.MedicationReconciliation = finalCarePlanNotes.MedicationReconciliation;
                    carePlan.DiabeticScreening = finalCarePlanNotes.DiabeticScreening;
                    carePlan.TobaccoAlcoholScreening = finalCarePlanNotes.TobaccoAlcoholScreening;
                    carePlan.ColonCancerScreenDate = finalCarePlanNotes.ColonCancerScreenDate;
                    carePlan.MammogramScreenDate = finalCarePlanNotes.MammogramScreenDate;
                    carePlan.UrinaryIncontinence = finalCarePlanNotes.UrinaryIncontinence;
                    carePlan.Immunization = finalCarePlanNotes.Immunization;

                    carePlan.CriticalChestPain = finalCarePlanNotes.CriticalChestPain;
                    carePlan.CriticalBreathShortness = finalCarePlanNotes.CriticalBreathShortness;
                    //newone
                    carePlan.NewWound = finalCarePlanNotes.NewWound;
                    carePlan.Bleedinggums = finalCarePlanNotes.Bleedinggums;
                    carePlan.Nosebleed = finalCarePlanNotes.Nosebleed;
                    carePlan.Bloodinurine = finalCarePlanNotes.Bloodinurine;
                    carePlan.Isurinemalodorous = finalCarePlanNotes.Isurinemalodorous;
                    carePlan.Suddenincreasedswelling = finalCarePlanNotes.Suddenincreasedswelling;
                    carePlan.SkinAssesment = finalCarePlanNotes.SkinAssesment;
                    carePlan.Diaperchange = finalCarePlanNotes.Diaperchange;
                    carePlan.DMEsupply = finalCarePlanNotes.DMEsupply;
                    carePlan.isPatientBedbound = finalCarePlanNotes.isPatientBedbound;
                    //
                    carePlan.CriticalSeverePain = finalCarePlanNotes.CriticalSeverePain;
                    carePlan.CriticalMoodChange = finalCarePlanNotes.CriticalMoodChange;
                    carePlan.CriticalSpeechChange = finalCarePlanNotes.CriticalSpeechChange;
                    carePlan.CriticalHeadache = finalCarePlanNotes.CriticalHeadache;
                    carePlan.CriticalBloodSugar = finalCarePlanNotes.CriticalBloodSugar;
                    carePlan.CriticalBloodPressure = finalCarePlanNotes.CriticalBloodPressure;
                    patient.UpdatedBy = User.Identity.GetUserId();
                    patient.UpdatedOn = DateTime.Now;
                    patient.Cycle = cycle;
                    _db.Entry(patient).State = EntityState.Modified;
                    _db.FinalCarePlanNotes.Add(carePlan);
                    _db.SaveChanges();
                    try
                    {
                        FinalCarePlanNotes_History finalCarePlanNotes_History = new FinalCarePlanNotes_History();
                        HelperExtensions. DuckCopyShallow( finalCarePlanNotes_History, carePlan);
                        _db.FinalCarePlanNotes_Histories.Add(finalCarePlanNotes_History);
                        _db.SaveChanges();
                    }
                    catch (Exception ex)
                    {


                    }
                }

            }
            else
            {
                var errorList = ModelState.Values.SelectMany(m => m.Errors)
                                 .Select(e => e.ErrorMessage)
                                 .ToList();
                ViewBag.Errors = string.Join(", ", errorList);
                ViewBag.PatientName = patient?.FirstName + " " + patient?.LastName;
                ViewBag.PatientId = patient?.Id;
                ViewBag.CcmStatus = patient?.CcmStatus;
                if (User.IsInRole("Liaison"))
                    ViewBag.ReviewId = HelperExtensions.ReviewTimeGet("Final Care Plan Notes", patient?.Id, User.Identity.GetUserId());
                return View(finalCarePlanNotes);
            }


            return Redirect(Request.UrlReferrer.PathAndQuery);

            // return RedirectToAction("Index", new { patientId = patient.Id });
        }


        [HttpPost, ValidateInput(false)]
        [Authorize(Roles = "Liaison, Admin, QAQC")]
        public async Task<PartialViewResult> _Create(FinalCarePlanNotes finalCarePlanNotes)
        {
            ViewBag.Errors = "";
            var patient = await _db.Patients.FindAsync(finalCarePlanNotes.PatientId);
            ViewBag.PatientName = patient?.FirstName + ' ' + patient?.LastName;
            ViewBag.PatientId = patient?.Id;
            ViewBag.CcmStatus = patient?.CcmStatus;
            if (ModelState.IsValid)
            {
                //if (HelperExtensions.isAllowedforEditingorAdd(finalCarePlanNotes.PatientId, CategoryCycleStatusHelper.GetPatientNewOrOldCycleByCategory(finalCarePlanNotes.PatientId), User.Identity.GetUserId()) == false)
                //{
                //    return RedirectToAction("Index", "CcmStatus", new { status = HelperExtensions.GetStatusRedirectionbyUser(User.Identity.GetUserId()), Message = "Cycle is locked." });

                //}
                var user = _db.Users.Find(User.Identity.GetUserId()).FirstName + " " + _db.Users.Find(User.Identity.GetUserId()).LastName;
                Guid obj = Guid.NewGuid();

                patient.Cycle = CategoryCycleStatusHelper.GetPatientNewOrOldCycleByCategory(patient.Id, BillingCodeHelper.cmmBillingCatagoryid);
                var cycle = patient.Cycle;
                var carePlanPatient = _db.FinalCarePlanNotes.Where(p => p.PatientId == finalCarePlanNotes.PatientId).OrderByDescending(item => item.Id).ToList();
                bool isNewone = true;
                if (carePlanPatient.Count > 0)
                {
                    var lastcareplan = carePlanPatient[0];
                    if (lastcareplan.CarePlanCreatedOn.Value.Month == DateTime.Now.Month && lastcareplan.CarePlanCreatedOn.Value.Year == DateTime.Now.Year)

                    {
                        try
                        {
                            FinalCarePlanNotes_History finalCarePlanNotes_History = new FinalCarePlanNotes_History();
                            HelperExtensions.DuckCopyShallow(finalCarePlanNotes_History, lastcareplan);
                            _db.FinalCarePlanNotes_Histories.Add(finalCarePlanNotes_History);
                            _db.SaveChanges();
                        }
                        catch (Exception ex)
                        {


                        }
                        isNewone = false;
                        var carePlan = lastcareplan;
                        carePlan.PatientId = finalCarePlanNotes.PatientId;
                        carePlan.PatientSummary = finalCarePlanNotes.PatientSummary;
                        carePlan.HealthConcerns = finalCarePlanNotes.HealthConcerns;
                        carePlan.DrugUtillizationReview = finalCarePlanNotes.DrugUtillizationReview;
                        carePlan.NutritionGoals = finalCarePlanNotes.NutritionGoals;
                        carePlan.NutritionInterventions = finalCarePlanNotes.NutritionInterventions;
                        carePlan.NutritionReview = finalCarePlanNotes.NutritionReview;
                        carePlan.PhysicalActivityGoals = finalCarePlanNotes.PhysicalActivityGoals;
                        carePlan.PhysicalActivityInterventions = finalCarePlanNotes.PhysicalActivityInterventions;
                        carePlan.PhysicalActivityReview = finalCarePlanNotes.PhysicalActivityReview;
                        carePlan.SleepGoals = finalCarePlanNotes.SleepGoals;
                        carePlan.SleepInterventions = finalCarePlanNotes.SleepInterventions;
                        carePlan.SleepReview = finalCarePlanNotes.SleepReview;
                        carePlan.MentalResilienceGoals = finalCarePlanNotes.MentalResilienceGoals;
                        carePlan.MentalResilienceInterventions = finalCarePlanNotes.MentalResilienceInterventions;
                        carePlan.MentalResilienceReview = finalCarePlanNotes.MentalResilienceReview;
                        carePlan.ManagingMedicationGoals = finalCarePlanNotes.ManagingMedicationGoals;
                        carePlan.ManagingMedicationInterventions = finalCarePlanNotes.ManagingMedicationInterventions;
                        carePlan.ManagingMedicationReview = finalCarePlanNotes.ManagingMedicationReview;
                        carePlan.BadHabitsCessationGoals = finalCarePlanNotes.BadHabitsCessationGoals;
                        carePlan.BadHabitsCessationInterventions = finalCarePlanNotes.BadHabitsCessationInterventions;
                        carePlan.BadHabitsCessationReview = finalCarePlanNotes.BadHabitsCessationReview;
                        carePlan.SideEffectsHistory = finalCarePlanNotes.SideEffectsHistory;
                        carePlan.Allergies = finalCarePlanNotes.Allergies;
                        carePlan.VerbalEducationToPatient = finalCarePlanNotes.VerbalEducationToPatient;
                        carePlan.PatientComplianceManagement = finalCarePlanNotes.PatientComplianceManagement;
                        carePlan.PharmacyTreatmentGoals = finalCarePlanNotes.PharmacyTreatmentGoals;
                        carePlan.EducationSent = finalCarePlanNotes.EducationSent;
                        carePlan.LastHospitalized = finalCarePlanNotes.LastHospitalized;
                        carePlan.HospitalizedReason = finalCarePlanNotes.HospitalizedReason;
                        carePlan.HealthRating = finalCarePlanNotes.HealthRating;
                        carePlan.LastErVisit = finalCarePlanNotes.LastErVisit;
                        carePlan.ErReason = finalCarePlanNotes.ErReason;
                        carePlan.PerformTasksRating = finalCarePlanNotes.PerformTasksRating;
                        carePlan.CarePlanEditedBy = user;
                        carePlan.CarePlanEditedOn = DateTime.Now;



                        carePlan.HospitalizationLast30Days = finalCarePlanNotes.HospitalizationLast30Days;
                        carePlan.AbilityPerformIADL = finalCarePlanNotes.AbilityPerformIADL;
                        carePlan.AssessmentVisionHearingSpeech = finalCarePlanNotes.AssessmentVisionHearingSpeech;
                        carePlan.BloodPressureGlucoseReading = finalCarePlanNotes.BloodPressureGlucoseReading;
                        carePlan.LastBloodPressureGlucoseReading = finalCarePlanNotes.LastBloodPressureGlucoseReading;
                        carePlan.FallAmbulationAssessment = finalCarePlanNotes.FallAmbulationAssessment;
                        carePlan.MedicationReconciliation = finalCarePlanNotes.MedicationReconciliation;
                        carePlan.DiabeticScreening = finalCarePlanNotes.DiabeticScreening;
                        carePlan.TobaccoAlcoholScreening = finalCarePlanNotes.TobaccoAlcoholScreening;
                        carePlan.ColonCancerScreenDate = finalCarePlanNotes.ColonCancerScreenDate;
                        carePlan.MammogramScreenDate = finalCarePlanNotes.MammogramScreenDate;
                        carePlan.UrinaryIncontinence = finalCarePlanNotes.UrinaryIncontinence;
                        carePlan.Immunization = finalCarePlanNotes.Immunization;

                        carePlan.CriticalChestPain = finalCarePlanNotes.CriticalChestPain;
                        carePlan.CriticalBreathShortness = finalCarePlanNotes.CriticalBreathShortness;
                        //newone
                        carePlan.NewWound = finalCarePlanNotes.NewWound;
                        carePlan.Bleedinggums = finalCarePlanNotes.Bleedinggums;
                        carePlan.Nosebleed = finalCarePlanNotes.Nosebleed;
                        carePlan.Bloodinurine = finalCarePlanNotes.Bloodinurine;
                        carePlan.Isurinemalodorous = finalCarePlanNotes.Isurinemalodorous;
                        carePlan.Suddenincreasedswelling = finalCarePlanNotes.Suddenincreasedswelling;
                        carePlan.SkinAssesment = finalCarePlanNotes.SkinAssesment;
                        carePlan.Diaperchange = finalCarePlanNotes.Diaperchange;
                        carePlan.DMEsupply = finalCarePlanNotes.DMEsupply;
                        carePlan.isPatientBedbound = finalCarePlanNotes.isPatientBedbound;
                        //
                        carePlan.CriticalSeverePain = finalCarePlanNotes.CriticalSeverePain;
                        carePlan.CriticalMoodChange = finalCarePlanNotes.CriticalMoodChange;
                        carePlan.CriticalSpeechChange = finalCarePlanNotes.CriticalSpeechChange;
                        carePlan.CriticalHeadache = finalCarePlanNotes.CriticalHeadache;
                        carePlan.CriticalBloodSugar = finalCarePlanNotes.CriticalBloodSugar;
                        carePlan.CriticalBloodPressure = finalCarePlanNotes.CriticalBloodPressure;
                        _db.Entry(carePlan).State = EntityState.Modified;
                        _db.SaveChanges();
                    }

                }

                if (isNewone == true)
                {
                    var carePlan = new FinalCarePlanNotes();
                    carePlan.PatientId = finalCarePlanNotes.PatientId;
                    carePlan.PatientSummary = finalCarePlanNotes.PatientSummary;
                    carePlan.HealthConcerns = finalCarePlanNotes.HealthConcerns;
                    carePlan.DrugUtillizationReview = finalCarePlanNotes.DrugUtillizationReview;
                    carePlan.NutritionGoals = finalCarePlanNotes.NutritionGoals;
                    carePlan.NutritionInterventions = finalCarePlanNotes.NutritionInterventions;
                    carePlan.NutritionReview = finalCarePlanNotes.NutritionReview;
                    carePlan.PhysicalActivityGoals = finalCarePlanNotes.PhysicalActivityGoals;
                    carePlan.PhysicalActivityInterventions = finalCarePlanNotes.PhysicalActivityInterventions;
                    carePlan.PhysicalActivityReview = finalCarePlanNotes.PhysicalActivityReview;
                    carePlan.SleepGoals = finalCarePlanNotes.SleepGoals;
                    carePlan.SleepInterventions = finalCarePlanNotes.SleepInterventions;
                    carePlan.SleepReview = finalCarePlanNotes.SleepReview;
                    carePlan.MentalResilienceGoals = finalCarePlanNotes.MentalResilienceGoals;
                    carePlan.MentalResilienceInterventions = finalCarePlanNotes.MentalResilienceInterventions;
                    carePlan.MentalResilienceReview = finalCarePlanNotes.MentalResilienceReview;
                    carePlan.ManagingMedicationGoals = finalCarePlanNotes.ManagingMedicationGoals;
                    carePlan.ManagingMedicationInterventions = finalCarePlanNotes.ManagingMedicationInterventions;
                    carePlan.ManagingMedicationReview = finalCarePlanNotes.ManagingMedicationReview;
                    carePlan.BadHabitsCessationGoals = finalCarePlanNotes.BadHabitsCessationGoals;
                    carePlan.BadHabitsCessationInterventions = finalCarePlanNotes.BadHabitsCessationInterventions;
                    carePlan.BadHabitsCessationReview = finalCarePlanNotes.BadHabitsCessationReview;
                    carePlan.SideEffectsHistory = finalCarePlanNotes.SideEffectsHistory;
                    carePlan.Allergies = finalCarePlanNotes.Allergies;
                    carePlan.VerbalEducationToPatient = finalCarePlanNotes.VerbalEducationToPatient;
                    carePlan.PatientComplianceManagement = finalCarePlanNotes.PatientComplianceManagement;
                    carePlan.PharmacyTreatmentGoals = finalCarePlanNotes.PharmacyTreatmentGoals;
                    carePlan.EducationSent = finalCarePlanNotes.EducationSent;
                    carePlan.LastHospitalized = finalCarePlanNotes.LastHospitalized;
                    carePlan.HospitalizedReason = finalCarePlanNotes.HospitalizedReason;
                    carePlan.HealthRating = finalCarePlanNotes.HealthRating;
                    carePlan.LastErVisit = finalCarePlanNotes.LastErVisit;
                    carePlan.ErReason = finalCarePlanNotes.ErReason;
                    carePlan.PerformTasksRating = finalCarePlanNotes.PerformTasksRating;
                    carePlan.CarePlanCreatedBy = user;
                    carePlan.CarePlanCreatedOn = DateTime.Now;
                    carePlan.Version = obj;
                    carePlan.Cycle = cycle;


                    carePlan.HospitalizationLast30Days = finalCarePlanNotes.HospitalizationLast30Days;
                    carePlan.AbilityPerformIADL = finalCarePlanNotes.AbilityPerformIADL;
                    carePlan.AssessmentVisionHearingSpeech = finalCarePlanNotes.AssessmentVisionHearingSpeech;
                    carePlan.BloodPressureGlucoseReading = finalCarePlanNotes.BloodPressureGlucoseReading;
                    carePlan.LastBloodPressureGlucoseReading = finalCarePlanNotes.LastBloodPressureGlucoseReading;
                    carePlan.FallAmbulationAssessment = finalCarePlanNotes.FallAmbulationAssessment;
                    carePlan.MedicationReconciliation = finalCarePlanNotes.MedicationReconciliation;
                    carePlan.DiabeticScreening = finalCarePlanNotes.DiabeticScreening;
                    carePlan.TobaccoAlcoholScreening = finalCarePlanNotes.TobaccoAlcoholScreening;
                    carePlan.ColonCancerScreenDate = finalCarePlanNotes.ColonCancerScreenDate;
                    carePlan.MammogramScreenDate = finalCarePlanNotes.MammogramScreenDate;
                    carePlan.UrinaryIncontinence = finalCarePlanNotes.UrinaryIncontinence;
                    carePlan.Immunization = finalCarePlanNotes.Immunization;

                    carePlan.CriticalChestPain = finalCarePlanNotes.CriticalChestPain;
                    carePlan.CriticalBreathShortness = finalCarePlanNotes.CriticalBreathShortness;
                    //newone
                    carePlan.NewWound = finalCarePlanNotes.NewWound;
                    carePlan.Bleedinggums = finalCarePlanNotes.Bleedinggums;
                    carePlan.Nosebleed = finalCarePlanNotes.Nosebleed;
                    carePlan.Bloodinurine = finalCarePlanNotes.Bloodinurine;
                    carePlan.Isurinemalodorous = finalCarePlanNotes.Isurinemalodorous;
                    carePlan.Suddenincreasedswelling = finalCarePlanNotes.Suddenincreasedswelling;
                    carePlan.SkinAssesment = finalCarePlanNotes.SkinAssesment;
                    carePlan.Diaperchange = finalCarePlanNotes.Diaperchange;
                    carePlan.DMEsupply = finalCarePlanNotes.DMEsupply;
                    carePlan.isPatientBedbound = finalCarePlanNotes.isPatientBedbound;
                    //
                    carePlan.CriticalSeverePain = finalCarePlanNotes.CriticalSeverePain;
                    carePlan.CriticalMoodChange = finalCarePlanNotes.CriticalMoodChange;
                    carePlan.CriticalSpeechChange = finalCarePlanNotes.CriticalSpeechChange;
                    carePlan.CriticalHeadache = finalCarePlanNotes.CriticalHeadache;
                    carePlan.CriticalBloodSugar = finalCarePlanNotes.CriticalBloodSugar;
                    carePlan.CriticalBloodPressure = finalCarePlanNotes.CriticalBloodPressure;
                    patient.UpdatedBy = User.Identity.GetUserId();
                    patient.UpdatedOn = DateTime.Now;
                    patient.Cycle = cycle;
                    _db.Entry(patient).State = EntityState.Modified;
                    _db.FinalCarePlanNotes.Add(carePlan);
                    _db.SaveChanges();
                    try
                    {
                        FinalCarePlanNotes_History finalCarePlanNotes_History = new FinalCarePlanNotes_History();
                        HelperExtensions.DuckCopyShallow(finalCarePlanNotes_History, carePlan);
                        _db.FinalCarePlanNotes_Histories.Add(finalCarePlanNotes_History);
                        _db.SaveChanges();
                    }
                    catch (Exception ex)
                    {


                    }
                }

            }
            else
            {
                var errorList = ModelState.Values.SelectMany(m => m.Errors)
                                 .Select(e => e.ErrorMessage)
                                 .ToList();
                ViewBag.Errors = string.Join(", ", errorList);
                ViewBag.PatientName = patient?.FirstName + " " + patient?.LastName;
                ViewBag.PatientId = patient?.Id;
                ViewBag.CcmStatus = patient?.CcmStatus;
                if (User.IsInRole("Liaison"))
                    ViewBag.ReviewId = HelperExtensions.ReviewTimeGet("Final Care Plan Notes", patient?.Id, User.Identity.GetUserId());
                return PartialView(finalCarePlanNotes);
            }


              return PartialView(Request.UrlReferrer.PathAndQuery);

            // return RedirectToAction("Index", new { patientId = patient.Id });
        }



        [HttpPost, ValidateInput(false)]
        public string InsertFinalCarePlan(FinalCarePlanNotes finalCarePlanNotes)
        {
            var patient = _db.Patients.Find(finalCarePlanNotes.PatientId);
            if (ModelState.IsValid)
            {


                //if (HelperExtensions.isAllowedforEditingorAdd(finalCarePlanNotes.PatientId, CategoryCycleStatusHelper.GetPatientNewOrOldCycleByCategory(finalCarePlanNotes.PatientId), User.Identity.GetUserId()) == false)
                //{
                //    return "Not Allowed";

                //}
                var user = _db.Users.Find(User.Identity.GetUserId()).FirstName + " " + _db.Users.Find(User.Identity.GetUserId()).LastName;
                Guid obj = Guid.NewGuid();

                patient.Cycle = CategoryCycleStatusHelper.GetPatientNewOrOldCycleByCategory(patient.Id, BillingCodeHelper.cmmBillingCatagoryid);
                var cycle = patient.Cycle;
                var carePlanPatient = _db.FinalCarePlanNotes.Where(p => p.PatientId == finalCarePlanNotes.PatientId).OrderByDescending(item => item.Id).ToList();
                bool isNewone = true;
                if (carePlanPatient.Count > 0)
                {
                    var lastcareplan = carePlanPatient[0];
                    if (lastcareplan.CarePlanCreatedOn.Value.Month == DateTime.Now.Month && lastcareplan.CarePlanCreatedOn.Value.Year == DateTime.Now.Year)

                    {
                        try
                        {
                            FinalCarePlanNotes_History finalCarePlanNotes_History = new FinalCarePlanNotes_History();
                            HelperExtensions.DuckCopyShallow(finalCarePlanNotes_History, lastcareplan);
                            _db.FinalCarePlanNotes_Histories.Add(finalCarePlanNotes_History);
                            _db.SaveChanges();
                        }
                        catch (Exception ex)
                        {


                        }
                        isNewone = false;
                        var carePlan = lastcareplan;
                        carePlan.PatientId = finalCarePlanNotes.PatientId;
                        carePlan.PatientSummary = finalCarePlanNotes.PatientSummary;
                        carePlan.HealthConcerns = finalCarePlanNotes.HealthConcerns;
                        carePlan.DrugUtillizationReview = finalCarePlanNotes.DrugUtillizationReview;
                        carePlan.NutritionGoals = finalCarePlanNotes.NutritionGoals;
                        carePlan.NutritionInterventions = finalCarePlanNotes.NutritionInterventions;
                        carePlan.NutritionReview = finalCarePlanNotes.NutritionReview;
                        carePlan.PhysicalActivityGoals = finalCarePlanNotes.PhysicalActivityGoals;
                        carePlan.PhysicalActivityInterventions = finalCarePlanNotes.PhysicalActivityInterventions;
                        carePlan.PhysicalActivityReview = finalCarePlanNotes.PhysicalActivityReview;
                        carePlan.SleepGoals = finalCarePlanNotes.SleepGoals;
                        carePlan.SleepInterventions = finalCarePlanNotes.SleepInterventions;
                        carePlan.SleepReview = finalCarePlanNotes.SleepReview;
                        carePlan.MentalResilienceGoals = finalCarePlanNotes.MentalResilienceGoals;
                        carePlan.MentalResilienceInterventions = finalCarePlanNotes.MentalResilienceInterventions;
                        carePlan.MentalResilienceReview = finalCarePlanNotes.MentalResilienceReview;
                        carePlan.ManagingMedicationGoals = finalCarePlanNotes.ManagingMedicationGoals;
                        carePlan.ManagingMedicationInterventions = finalCarePlanNotes.ManagingMedicationInterventions;
                        carePlan.ManagingMedicationReview = finalCarePlanNotes.ManagingMedicationReview;
                        carePlan.BadHabitsCessationGoals = finalCarePlanNotes.BadHabitsCessationGoals;
                        carePlan.BadHabitsCessationInterventions = finalCarePlanNotes.BadHabitsCessationInterventions;
                        carePlan.BadHabitsCessationReview = finalCarePlanNotes.BadHabitsCessationReview;
                        carePlan.SideEffectsHistory = finalCarePlanNotes.SideEffectsHistory;
                        carePlan.Allergies = finalCarePlanNotes.Allergies;
                        carePlan.VerbalEducationToPatient = finalCarePlanNotes.VerbalEducationToPatient;
                        carePlan.PatientComplianceManagement = finalCarePlanNotes.PatientComplianceManagement;
                        carePlan.PharmacyTreatmentGoals = finalCarePlanNotes.PharmacyTreatmentGoals;
                        carePlan.EducationSent = finalCarePlanNotes.EducationSent;
                        carePlan.LastHospitalized = finalCarePlanNotes.LastHospitalized;
                        carePlan.HospitalizedReason = finalCarePlanNotes.HospitalizedReason;
                        carePlan.HealthRating = finalCarePlanNotes.HealthRating;
                        carePlan.LastErVisit = finalCarePlanNotes.LastErVisit;
                        carePlan.ErReason = finalCarePlanNotes.ErReason;
                        carePlan.PerformTasksRating = finalCarePlanNotes.PerformTasksRating;
                        carePlan.CarePlanEditedBy = user;
                        carePlan.CarePlanEditedOn = DateTime.Now;



                        carePlan.HospitalizationLast30Days = finalCarePlanNotes.HospitalizationLast30Days;
                        carePlan.AbilityPerformIADL = finalCarePlanNotes.AbilityPerformIADL;
                        carePlan.AssessmentVisionHearingSpeech = finalCarePlanNotes.AssessmentVisionHearingSpeech;
                        carePlan.BloodPressureGlucoseReading = finalCarePlanNotes.BloodPressureGlucoseReading;
                        carePlan.LastBloodPressureGlucoseReading = finalCarePlanNotes.LastBloodPressureGlucoseReading;
                        carePlan.FallAmbulationAssessment = finalCarePlanNotes.FallAmbulationAssessment;
                        carePlan.MedicationReconciliation = finalCarePlanNotes.MedicationReconciliation;
                        carePlan.DiabeticScreening = finalCarePlanNotes.DiabeticScreening;
                        carePlan.TobaccoAlcoholScreening = finalCarePlanNotes.TobaccoAlcoholScreening;
                        carePlan.ColonCancerScreenDate = finalCarePlanNotes.ColonCancerScreenDate;
                        carePlan.MammogramScreenDate = finalCarePlanNotes.MammogramScreenDate;
                        carePlan.UrinaryIncontinence = finalCarePlanNotes.UrinaryIncontinence;
                        carePlan.Immunization = finalCarePlanNotes.Immunization;

                        carePlan.CriticalChestPain = finalCarePlanNotes.CriticalChestPain;
                        carePlan.CriticalBreathShortness = finalCarePlanNotes.CriticalBreathShortness;
                        //newone
                        carePlan.NewWound = finalCarePlanNotes.NewWound;
                        carePlan.Bleedinggums = finalCarePlanNotes.Bleedinggums;
                        carePlan.Nosebleed = finalCarePlanNotes.Nosebleed;
                        carePlan.Bloodinurine = finalCarePlanNotes.Bloodinurine;
                        carePlan.Isurinemalodorous = finalCarePlanNotes.Isurinemalodorous;
                        carePlan.Suddenincreasedswelling = finalCarePlanNotes.Suddenincreasedswelling;
                        carePlan.SkinAssesment = finalCarePlanNotes.SkinAssesment;
                        carePlan.Diaperchange = finalCarePlanNotes.Diaperchange;
                        carePlan.DMEsupply = finalCarePlanNotes.DMEsupply;
                        carePlan.isPatientBedbound = finalCarePlanNotes.isPatientBedbound;
                        //
                        carePlan.CriticalSeverePain = finalCarePlanNotes.CriticalSeverePain;
                        carePlan.CriticalMoodChange = finalCarePlanNotes.CriticalMoodChange;
                        carePlan.CriticalSpeechChange = finalCarePlanNotes.CriticalSpeechChange;
                        carePlan.CriticalHeadache = finalCarePlanNotes.CriticalHeadache;
                        carePlan.CriticalBloodSugar = finalCarePlanNotes.CriticalBloodSugar;
                        carePlan.CriticalBloodPressure = finalCarePlanNotes.CriticalBloodPressure;
                        _db.Entry(carePlan).State = EntityState.Modified;
                        _db.SaveChanges();
                    }

                }

                if (isNewone == true)
                {
                    var carePlan = new FinalCarePlanNotes();
                    carePlan.PatientId = finalCarePlanNotes.PatientId;
                    carePlan.PatientSummary = finalCarePlanNotes.PatientSummary;
                    carePlan.HealthConcerns = finalCarePlanNotes.HealthConcerns;
                    carePlan.DrugUtillizationReview = finalCarePlanNotes.DrugUtillizationReview;
                    carePlan.NutritionGoals = finalCarePlanNotes.NutritionGoals;
                    carePlan.NutritionInterventions = finalCarePlanNotes.NutritionInterventions;
                    carePlan.NutritionReview = finalCarePlanNotes.NutritionReview;
                    carePlan.PhysicalActivityGoals = finalCarePlanNotes.PhysicalActivityGoals;
                    carePlan.PhysicalActivityInterventions = finalCarePlanNotes.PhysicalActivityInterventions;
                    carePlan.PhysicalActivityReview = finalCarePlanNotes.PhysicalActivityReview;
                    carePlan.SleepGoals = finalCarePlanNotes.SleepGoals;
                    carePlan.SleepInterventions = finalCarePlanNotes.SleepInterventions;
                    carePlan.SleepReview = finalCarePlanNotes.SleepReview;
                    carePlan.MentalResilienceGoals = finalCarePlanNotes.MentalResilienceGoals;
                    carePlan.MentalResilienceInterventions = finalCarePlanNotes.MentalResilienceInterventions;
                    carePlan.MentalResilienceReview = finalCarePlanNotes.MentalResilienceReview;
                    carePlan.ManagingMedicationGoals = finalCarePlanNotes.ManagingMedicationGoals;
                    carePlan.ManagingMedicationInterventions = finalCarePlanNotes.ManagingMedicationInterventions;
                    carePlan.ManagingMedicationReview = finalCarePlanNotes.ManagingMedicationReview;
                    carePlan.BadHabitsCessationGoals = finalCarePlanNotes.BadHabitsCessationGoals;
                    carePlan.BadHabitsCessationInterventions = finalCarePlanNotes.BadHabitsCessationInterventions;
                    carePlan.BadHabitsCessationReview = finalCarePlanNotes.BadHabitsCessationReview;
                    carePlan.SideEffectsHistory = finalCarePlanNotes.SideEffectsHistory;
                    carePlan.Allergies = finalCarePlanNotes.Allergies;
                    carePlan.VerbalEducationToPatient = finalCarePlanNotes.VerbalEducationToPatient;
                    carePlan.PatientComplianceManagement = finalCarePlanNotes.PatientComplianceManagement;
                    carePlan.PharmacyTreatmentGoals = finalCarePlanNotes.PharmacyTreatmentGoals;
                    carePlan.EducationSent = finalCarePlanNotes.EducationSent;
                    carePlan.LastHospitalized = finalCarePlanNotes.LastHospitalized;
                    carePlan.HospitalizedReason = finalCarePlanNotes.HospitalizedReason;
                    carePlan.HealthRating = finalCarePlanNotes.HealthRating;
                    carePlan.LastErVisit = finalCarePlanNotes.LastErVisit;
                    carePlan.ErReason = finalCarePlanNotes.ErReason;
                    carePlan.PerformTasksRating = finalCarePlanNotes.PerformTasksRating;
                    carePlan.CarePlanCreatedBy = user;
                    carePlan.CarePlanCreatedOn = DateTime.Now;
                    carePlan.Version = obj;
                    carePlan.Cycle = cycle;


                    carePlan.HospitalizationLast30Days = finalCarePlanNotes.HospitalizationLast30Days;
                    carePlan.AbilityPerformIADL = finalCarePlanNotes.AbilityPerformIADL;
                    carePlan.AssessmentVisionHearingSpeech = finalCarePlanNotes.AssessmentVisionHearingSpeech;
                    carePlan.BloodPressureGlucoseReading = finalCarePlanNotes.BloodPressureGlucoseReading;
                    carePlan.LastBloodPressureGlucoseReading = finalCarePlanNotes.LastBloodPressureGlucoseReading;
                    carePlan.FallAmbulationAssessment = finalCarePlanNotes.FallAmbulationAssessment;
                    carePlan.MedicationReconciliation = finalCarePlanNotes.MedicationReconciliation;
                    carePlan.DiabeticScreening = finalCarePlanNotes.DiabeticScreening;
                    carePlan.TobaccoAlcoholScreening = finalCarePlanNotes.TobaccoAlcoholScreening;
                    carePlan.ColonCancerScreenDate = finalCarePlanNotes.ColonCancerScreenDate;
                    carePlan.MammogramScreenDate = finalCarePlanNotes.MammogramScreenDate;
                    carePlan.UrinaryIncontinence = finalCarePlanNotes.UrinaryIncontinence;
                    carePlan.Immunization = finalCarePlanNotes.Immunization;

                    carePlan.CriticalChestPain = finalCarePlanNotes.CriticalChestPain;
                    carePlan.CriticalBreathShortness = finalCarePlanNotes.CriticalBreathShortness;
                    //newone
                    carePlan.NewWound = finalCarePlanNotes.NewWound;
                    carePlan.Bleedinggums = finalCarePlanNotes.Bleedinggums;
                    carePlan.Nosebleed = finalCarePlanNotes.Nosebleed;
                    carePlan.Bloodinurine = finalCarePlanNotes.Bloodinurine;
                    carePlan.Isurinemalodorous = finalCarePlanNotes.Isurinemalodorous;
                    carePlan.Suddenincreasedswelling = finalCarePlanNotes.Suddenincreasedswelling;
                    carePlan.SkinAssesment = finalCarePlanNotes.SkinAssesment;
                    carePlan.Diaperchange = finalCarePlanNotes.Diaperchange;
                    carePlan.DMEsupply = finalCarePlanNotes.DMEsupply;
                    carePlan.isPatientBedbound = finalCarePlanNotes.isPatientBedbound;
                    //
                    carePlan.CriticalSeverePain = finalCarePlanNotes.CriticalSeverePain;
                    carePlan.CriticalMoodChange = finalCarePlanNotes.CriticalMoodChange;
                    carePlan.CriticalSpeechChange = finalCarePlanNotes.CriticalSpeechChange;
                    carePlan.CriticalHeadache = finalCarePlanNotes.CriticalHeadache;
                    carePlan.CriticalBloodSugar = finalCarePlanNotes.CriticalBloodSugar;
                    carePlan.CriticalBloodPressure = finalCarePlanNotes.CriticalBloodPressure;
                    patient.UpdatedBy = User.Identity.GetUserId();
                    patient.UpdatedOn = DateTime.Now;
                    patient.Cycle = cycle;
                    _db.Entry(patient).State = EntityState.Modified;
                    _db.FinalCarePlanNotes.Add(carePlan);
                    _db.SaveChanges();
                    try
                    {
                        FinalCarePlanNotes_History finalCarePlanNotes_History = new FinalCarePlanNotes_History();
                        HelperExtensions.DuckCopyShallow(finalCarePlanNotes_History, carePlan);
                        _db.FinalCarePlanNotes_Histories.Add(finalCarePlanNotes_History);
                        _db.SaveChanges();
                    }
                    catch (Exception ex)
                    {


                    }
                }
            }
            else
            {
                var errorList = ModelState.Values.SelectMany(m => m.Errors)
                              .Select(e => e.ErrorMessage)
                              .ToList();
                var errorstr = string.Join(",", errorList);
                //return errorstr;
                return "False "+errorstr;
            }
            return "True";
        }

        public ActionResult IndividualCarePlan(Guid version, int patientId)
        {
            var patient = _db.Patients.Find(patientId);

            ViewBag.PatientName = patient?.FirstName + ' ' + patient?.LastName;
            ViewBag.PatientId = patient?.Id;
            ViewBag.CcmStatus = patient?.CcmStatus;
            if (User.IsInRole("Liaison"))
                ViewBag.ReviewId = HelperExtensions.ReviewTimeGet("Final Care Plan Notes", patient?.Id, User.Identity.GetUserId());
            var carePlan = _db.FinalCarePlanNotes.AsNoTracking().FirstOrDefault(m => m.Version == version);


            return View(carePlan);
        }
        public async Task<PartialViewResult> _IndividualCarePlan(Guid version, int patientId)
        {
            var patient = _db.Patients.Find(patientId);

            ViewBag.PatientName = patient?.FirstName + ' ' + patient?.LastName;
            ViewBag.PatientId = patient?.Id;
            ViewBag.CcmStatus = patient?.CcmStatus;
            //if (User.IsInRole("Liaison"))
            //    ViewBag.ReviewId = HelperExtensions.ReviewTimeGet("Final Care Plan Notes", patient?.Id, User.Identity.GetUserId());
            var carePlan = _db.FinalCarePlanNotes.AsNoTracking().FirstOrDefault(m => m.Version == version);


            return PartialView(carePlan);
        }


        public PartialViewResult PreviousCarePlan(int patientId, bool isNew,int cycle)
        {
            try
            {
                var patient = _db.Patients.Find(patientId);

                ViewBag.PatientName = patient?.FirstName + ' ' + patient?.LastName;
                ViewBag.PatientId = patient?.Id;
                ViewBag.CcmStatus = patient?.CcmStatus;
                var carePlanPatient = _db.FinalCarePlanNotes.AsNoTracking().Where(m => m.PatientId == patientId && m.Cycle< cycle).OrderByDescending(item => item.Id).ToList();

                //bool isNewone = true;
                if (carePlanPatient.Count > 0)
                {

                    return PartialView(carePlanPatient[0]);

                }
                else
                {

                    return PartialView(new FinalCarePlanNotes());
                }
                if (carePlanPatient.Count > 1)
                {

                    var lastcareplan = carePlanPatient[0];
                    if (lastcareplan.CarePlanCreatedOn.Value.Month == DateTime.Now.Month && lastcareplan.CarePlanCreatedOn.Value.Year == DateTime.Now.Year)

                    {
                        return PartialView(carePlanPatient[1]);
                    }
                    else
                    {
                        return PartialView(lastcareplan);
                    }
                }
                else
                {
                    if (carePlanPatient.Count > 0)
                    {
                        var lastcareplan = carePlanPatient[0];
                        if (lastcareplan.CarePlanCreatedOn.Value.Month == DateTime.Now.Month && lastcareplan.CarePlanCreatedOn.Value.Year == DateTime.Now.Year)

                        {
                            return PartialView(new FinalCarePlanNotes());
                        }
                        else
                        {
                            return PartialView(lastcareplan);
                        }
                    }
                    else
                    {
                        return PartialView(new FinalCarePlanNotes());
                    }

                }
            }
            catch (Exception ex)
            {
                return PartialView(new FinalCarePlanNotes());

            }
            //if (carePlan.Count > 1 && isNew == true)
            //{
            //    var secondlastcareplan = carePlan.OrderByDescending(m => m.Id).ToList();
            //    return PartialView(secondlastcareplan[0]);
            //}
            //else
            //{
            //    if (carePlan.Count > 1 && isNew == false)
            //    {
            //        var secondlastcareplan = carePlan.OrderByDescending(m => m.Id).ToList();
            //        return PartialView(secondlastcareplan[1]);
            //    }
            //    else
            //    {
            //        //if (carePlan.Count == 1 && isNew == false)
            //        //{
            //        //    var secondlastcareplan = carePlan.OrderByDescending(m => m.Id).ToList();
            //        //    return PartialView(secondlastcareplan[1]);
            //        //}
            //        return PartialView(new FinalCarePlanNotes());
            //    }

            //}

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