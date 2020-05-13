using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CCM.Models
{
    public class CarePlanforButtons
    {
        public int Cycle { get; set; }
        public DateTime? CreatedMonth { get; set; }
    }
    public class SharingHistory
    {
        public string sharedby { get; set; }
        public string EmailAddress { get; set; }
        public DateTime? shareddate { get; set; }
    }
    public class ShareCarePlanViewAll
    {
        public string EmailAddress { get; set; }
        public string Name { get; set; }

        public string sharedby { get; set; }
        public DateTime? shareddate { get; set; }
        public int Cycle { get; set; }
        public int PatientId { get; set; }

        
    }
    public class ShareCarePlanView
    {
        public string EmailAddress { get; set; }
        public string Name { get; set; }
        public string SectionName { get; set; }
        public string Relation { get; set; }
        public bool AlreadySentEmail { get; set; } = false;
        public int Cycle { get; set; }
        public int PatientId { get; set; }
        public int ReceiverId { get; set; }
        public string ReceiverType { get; set; }
        public List<SharingHistory> sharingHistories { get; set; }
    }
    public class CarePlan
    {
        public int Id { get; set; }
        public int PatientId { get; set; }

        public string NoteToPhysician { get; set; }
        public string Allergy { get; set; }
        public string SurgicalHistory { get; set; }
        public string FamilyHistory { get; set; }
        public string HPI { get; set; }
        public string CurrentRequest { get; set; }
        public string Plan { get; set; }
        public string Reminders { get; set; }
        public string Medication { get; set; }
        public string Lab { get; set; }
        public string SocialHistory { get; set; }
    }

    public class FinalCarePlanNotes
    {
        public int Id { get; set; }
        public int PatientId { get; set; }
        public DateTime? CarePlanCreatedOn { get; set; }
        public string CarePlanCreatedBy { get; set; }
        public string CarePlanEditedBy { get; set; }
        public DateTime? CarePlanEditedOn { get; set; }
        public Guid Version { get; set; }
        public int Cycle { get; set; }
       
        public string PatientSummary { get; set; }
        public string HealthConcerns { get; set; }

        public string DrugUtillizationReview { get; set; }

        public string NutritionGoals { get; set; }
        public string NutritionInterventions { get; set; }
        public string NutritionReview { get; set; }

        public string PhysicalActivityGoals { get; set; }
        public string PhysicalActivityInterventions { get; set; }
        public string PhysicalActivityReview { get; set; }

        public string SleepGoals { get; set; }
        public string SleepInterventions { get; set; }
        public string SleepReview { get; set; }

        public string MentalResilienceGoals { get; set; }
        public string MentalResilienceInterventions { get; set; }
        public string MentalResilienceReview { get; set; }

        public string ManagingMedicationGoals { get; set; }
        public string ManagingMedicationInterventions { get; set; }
        public string ManagingMedicationReview { get; set; }

        public string BadHabitsCessationGoals { get; set; }
        public string BadHabitsCessationInterventions { get; set; }
        public string BadHabitsCessationReview { get; set; }

        public string SideEffectsHistory { get; set; }
        public string Allergies { get; set; }
        public string LastHospitalized { get; set; }
        public string HospitalizedReason { get; set; }
        public string LastErVisit { get; set; }
        public string ErReason { get; set; }
        public string HealthRating { get; set; }
        public string PerformTasksRating { get; set; }
        public string VerbalEducationToPatient { get; set; }
        public string EducationSent { get; set; }
        public string PatientComplianceManagement { get; set; }
        public string PharmacyTreatmentGoals { get; set; }


        //Added on request from Valerie Lam Van on 6/19/2018:

        public string HospitalizationLast30Days { get; set; }  // Hospitalization in last 30 days
        public string AbilityPerformIADL { get; set; }   //Ability to perform IADL's and ADL's
        public string AssessmentVisionHearingSpeech { get; set; }  // Sensory assessment on vision, hearing and speech

        public string BloodPressureGlucoseReading { get; set; }  //Blood pressure and glucose reading 
        public string LastBloodPressureGlucoseReading { get; set; }  //Last Blood pressure and glucose reading 
        public string FallAmbulationAssessment { get; set; }  //Fall and Ambulation assessment

        public string MedicationReconciliation { get; set; }  //Medication Reconciliation
        public string DiabeticScreening { get; set; }   // Diabetic screening: Retinal Eye Exam, Podiatry Exam and last A1C result
        public string TobaccoAlcoholScreening { get; set; }  //Tobacco and Alcohol Screening

        public DateTime? ColonCancerScreenDate { get; set; }   //Colon Cancer Screen: Date
        public DateTime? MammogramScreenDate { get; set; }  //Mammogram Screen: Date

        public string UrinaryIncontinence { get; set; } //Urinary Incontinence
        public string Immunization { get; set; }  //Immunization

        [Display(Name = "Chest Pain")]
        public bool CriticalChestPain { get; set; }
        [Display(Name = "Shortness of breath")]
        public bool CriticalBreathShortness { get; set; }
        //Newone 11-04-2019
        [Display(Name = "New wound")]
        public bool NewWound { get; set; }
        [Display(Name = "Bleeding gums")]
        public bool Bleedinggums { get; set; }
        [Display(Name = "Nose bleed")]
        public bool Nosebleed { get; set; }
        [Display(Name = "Blood in urine")]
        public bool Bloodinurine { get; set; }
        [Display(Name = "Is urine malodorous")]
        public bool Isurinemalodorous { get; set; }
        [Display(Name = "Sudden increased swelling")]
        public bool Suddenincreasedswelling { get; set; }
        [Column(TypeName = "VARCHAR")]
        [StringLength(5)]
        public string SkinAssesment { get; set; }
        [Column(TypeName = "VARCHAR")]
        [StringLength(5)]
        public string Diaperchange { get; set; }

        public string DMEsupply { get; set; }
        [Column(TypeName = "VARCHAR")]
        [StringLength(5)]
        public string isPatientBedbound { get; set; }
        //
        [Display(Name = "Severe Pain")]
        public bool CriticalSeverePain { get; set; }
        [Display(Name = "Change in Awareness or Mood")]
        public bool CriticalMoodChange { get; set; }
        [Display(Name = "Change in Speech")]
        public bool CriticalSpeechChange { get; set; }
        [Display(Name = "Sudden and Severe Headache")]
        public bool CriticalHeadache { get; set; }
        [Display(Name = "Blood Sugar > 300")]
        public bool CriticalBloodSugar { get; set; }
        [Display(Name = "Blood Pressure > 180/90")]
        public bool CriticalBloodPressure { get; set; }



    }
    public class FinalCarePlanNotes_History
    {
        public int Id { get; set; }
        public int PatientId { get; set; }
        public DateTime? CarePlanCreatedOn { get; set; }
        public string CarePlanCreatedBy { get; set; }
        public string CarePlanEditedBy { get; set; }
        public DateTime? CarePlanEditedOn { get; set; }
        public Guid Version { get; set; }
        public int Cycle { get; set; }
        [Required]
        public string PatientSummary { get; set; }
        public string HealthConcerns { get; set; }

        public string DrugUtillizationReview { get; set; }

        public string NutritionGoals { get; set; }
        public string NutritionInterventions { get; set; }
        public string NutritionReview { get; set; }

        public string PhysicalActivityGoals { get; set; }
        public string PhysicalActivityInterventions { get; set; }
        public string PhysicalActivityReview { get; set; }

        public string SleepGoals { get; set; }
        public string SleepInterventions { get; set; }
        public string SleepReview { get; set; }

        public string MentalResilienceGoals { get; set; }
        public string MentalResilienceInterventions { get; set; }
        public string MentalResilienceReview { get; set; }

        public string ManagingMedicationGoals { get; set; }
        public string ManagingMedicationInterventions { get; set; }
        public string ManagingMedicationReview { get; set; }

        public string BadHabitsCessationGoals { get; set; }
        public string BadHabitsCessationInterventions { get; set; }
        public string BadHabitsCessationReview { get; set; }

        public string SideEffectsHistory { get; set; }
        public string Allergies { get; set; }
        public string LastHospitalized { get; set; }
        public string HospitalizedReason { get; set; }
        public string LastErVisit { get; set; }
        public string ErReason { get; set; }
        public string HealthRating { get; set; }
        public string PerformTasksRating { get; set; }
        public string VerbalEducationToPatient { get; set; }
        public string EducationSent { get; set; }
        public string PatientComplianceManagement { get; set; }
        public string PharmacyTreatmentGoals { get; set; }


        //Added on request from Valerie Lam Van on 6/19/2018:

        public string HospitalizationLast30Days { get; set; }  // Hospitalization in last 30 days
        public string AbilityPerformIADL { get; set; }   //Ability to perform IADL's and ADL's
        public string AssessmentVisionHearingSpeech { get; set; }  // Sensory assessment on vision, hearing and speech

        public string BloodPressureGlucoseReading { get; set; }  //Blood pressure and glucose reading 
        public string LastBloodPressureGlucoseReading { get; set; }  //Last Blood pressure and glucose reading 
        public string FallAmbulationAssessment { get; set; }  //Fall and Ambulation assessment

        public string MedicationReconciliation { get; set; }  //Medication Reconciliation
        public string DiabeticScreening { get; set; }   // Diabetic screening: Retinal Eye Exam, Podiatry Exam and last A1C result
        public string TobaccoAlcoholScreening { get; set; }  //Tobacco and Alcohol Screening

        public DateTime? ColonCancerScreenDate { get; set; }   //Colon Cancer Screen: Date
        public DateTime? MammogramScreenDate { get; set; }  //Mammogram Screen: Date

        public string UrinaryIncontinence { get; set; } //Urinary Incontinence
        public string Immunization { get; set; }  //Immunization

        [Display(Name = "Chest Pain")]
        public bool CriticalChestPain { get; set; }
        [Display(Name = "Shortness of breath")]
        public bool CriticalBreathShortness { get; set; }
        //Newone 11-04-2019
        [Display(Name = "New wound")]
        public bool NewWound { get; set; }
        [Display(Name = "Bleeding gums")]
        public bool Bleedinggums { get; set; }
        [Display(Name = "Nose bleed")]
        public bool Nosebleed { get; set; }
        [Display(Name = "Blood in urine")]
        public bool Bloodinurine { get; set; }
        [Display(Name = "Is urine malodorous")]
        public bool Isurinemalodorous { get; set; }
        [Display(Name = "Sudden increased swelling")]
        public bool Suddenincreasedswelling { get; set; }
        [Column(TypeName = "VARCHAR")]
        [StringLength(5)]
        public string SkinAssesment { get; set; }
        [Column(TypeName = "VARCHAR")]
        [StringLength(5)]
        public string Diaperchange { get; set; }

        public string DMEsupply { get; set; }
        [Column(TypeName = "VARCHAR")]
        [StringLength(5)]
        public string isPatientBedbound { get; set; }
        //
        [Display(Name = "Severe Pain")]
        public bool CriticalSeverePain { get; set; }
        [Display(Name = "Change in Awareness or Mood")]
        public bool CriticalMoodChange { get; set; }
        [Display(Name = "Change in Speech")]
        public bool CriticalSpeechChange { get; set; }
        [Display(Name = "Sudden and Severe Headache")]
        public bool CriticalHeadache { get; set; }
        [Display(Name = "Blood Sugar > 300")]
        public bool CriticalBloodSugar { get; set; }
        [Display(Name = "Blood Pressure > 180/90")]
        public bool CriticalBloodPressure { get; set; }



    }

    public class FinalCarePlanViewModel
    {
        public int PatientId { get; set; }
        public byte[] FinalCarePlanPdf { get; set; }

        [Display(Name = "Patient's Counselor")]
        public string LiaisonName { get; set; }

        [Display(Name = "Pateint's Physician")]
        public string PhysicianName { get; set; }
        public DateTime? LastVisited { get; set; }
        public DateTime? NextAppointment { get; set; }

        public string PhysicianSpeciality { get; set; }

        public byte[] PatientPhoto { get; set; }
        public byte[] LiaisonPhoto { get; set; }
        public byte[] PhysicianPhoto { get; set; }

        [Display(Name = "Patient's Full Name")]
        public string PatientName { get; set; }
        public string Address { get; set; }
        public string Email { get; set; }

        [Display(Name = "Date of Birth")]
        public DateTime BirthDate { get; set; }

        [Display(Name = "Home Phone Number")]
        public string HomePhoneNumber { get; set; }

        [Display(Name = "Mobile Phone Number")]
        public string MobilePhoneNumber { get; set; }

        
        public string Gender { get; set; }

        public string preferredLangugae { get; set; }
        public DateTime? CCMEnrolledOn { get; set; }
        public FinalCarePlanNotes FinalCarePlan { get; set; }
        public List<SecondaryDoctor> SecondaryDoctors { get; set; }
        public PatientProfile_UrgencyContact UrgencyContact { get; set; }
        public List<PatientProfile_UrgencyContact> lstUrgencyContact { get; set; }
        public List<PatientMedicalHistory_MedicationRx> MedicationRx { get; set; }
        public List<Icd10Codes> Icd10Codes { get; set; }
        public System.Web.Mvc.ActionResult jsonresult { get; set; }
    }
}