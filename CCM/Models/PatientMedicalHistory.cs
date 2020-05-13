using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CCM.Models
{
    public class PatientMedicalHistory_MedicalStatus
    {
        public int Id { get; set; }
        public int Cycle { get; set; }
        public int PatientId { get; set; }

        [Display(Name = "Age*")]
        public int Age { get; set; }

        //[Required]
        [Display(Name = "Weight*")]
        //[Range(0.01, 400.00, ErrorMessage = "Please, Enter a valid weight.")]
        public double Weight { get; set; }

        //[Required]
        [Display(Name = "Height*")]
        //[Range(0.01, 9.00, ErrorMessage = "Please, Enter a valid height.")]
        public double Height { get; set; }

        [Display(Name = "Waist Size")]
        public double? WaistSize { get; set; }

        [Display(Name = "BMI (Body Mass Index, If Known)")]
        public double? BMI { get; set; }

        [Display(Name = "Blood Type")]
        public string BloodType { get; set; }

        [Display(Name = "General Health*")]
        public string GeneralHealth { get; set; }

        [Display(Name = "Energy Level*")]
        public string EnergyLevel { get; set; }

        [Display(Name = "Are You Currently Using Hormones*")]
        public bool? IsUsingHormones { get; set; }

        [Display(Name = "Primary Care Physician's Name*")]
        public string PrimaryPhysician { get; set; }

        [Display(Name = "Physician's Phone Number")]
        [DataType(DataType.PhoneNumber)]
        [RegularExpression(@"^([0-9]{10})$", ErrorMessage = "Invalid Phone Number.")]
        [Range(1000000000, 9999999999, ErrorMessage = "Invalid Phone Number")]
        public string PhysicianPhoneNumber { get; set; }

        [Display(Name = "Other Physicans I am Currently Seeing")]
        public string OtherPhysician { get; set; }


    }

    public class PatientMedicalHistory_MedicalCondition
    {
        public int Id { get; set; }
        public int Cycle { get; set; }
        public int PatientId { get; set; }

        [Column(TypeName = "VARCHAR")]
        [StringLength(100)]
        public string UpdatedBy { get; set; }

        [Display(Name = "Heart Disease")]
        public bool HeartDisease { get; set; }

        [Display(Name = "Colitis")]
        public bool Colitis { get; set; }

        [Display(Name = "Stroke")]
        public bool Stroke { get; set; }

        [Display(Name = "Irritable Bowel")]
        public bool IrritableBowel { get; set; }

        [Display(Name = "High Blood Pressure")]
        public bool HighBP { get; set; }

        [Display(Name = "Ulcers")]
        public bool Ulcers { get; set; }

        [Display(Name = "Blood Clotting Problems")]
        public bool BloodClotting { get; set; }

        [Display(Name = "Gallbladder Issues")]
        public bool GallBladderIssues { get; set; }

        [Display(Name = "Vericose Veins")]
        public bool VericoseVeins { get; set; }

        [Display(Name = "Fractures")]
        public bool Fractures { get; set; }

        [Display(Name = "Diabetes or Insulin Resistance")]
        public bool Diabetes { get; set; }

        [Display(Name = "High Cholesterol or Triglycerides")]
        public bool HighCholesterol { get; set; }

        [Display(Name = "Arthritis")]
        public bool Arthritis { get; set; }

        [Display(Name = "Fibromyalgia")]
        public bool Fibromyalgia { get; set; }

        [Display(Name = "Osteoporosis")]
        public bool Osteoporosis { get; set; }

        [Display(Name = "Chronic Farigue")]
        public bool ChronicFatigue { get; set; }

        [Display(Name = "Epilepsy")]
        public bool Epilepsy { get; set; }

        [Display(Name = "Depression")]
        public bool Depression { get; set; }

        [Display(Name = "Kidney Trouble")]
        public bool KidneyTrouble { get; set; }

        [Display(Name = "Headaches/Migraines")]
        public bool Headaches { get; set; }

        [Display(Name = "Eye Disorder")]
        public bool EyeDisorder { get; set; }

        [Display(Name = "Eating Disorder")]
        public bool EatingDisorder { get; set; }

        [Display(Name = "Thyroid Disorder")]
        public bool ThyroidDisorder { get; set; }

        [Display(Name = "Hormone-Related Issues")]
        public bool HormoneIssues { get; set; }

        [Display(Name = "Adult Mumps")]
        public bool AdultMumps { get; set; }

        [Display(Name = "Impaired Liver Function")]
        public bool ImpairedLiver { get; set; }

        [Display(Name = "Lung Condition")]
        public bool LungCondition { get; set; }

        [Display(Name = "Type")]
        public string LungConditionType { get; set; }

        [Display(Name = "Cancer")]
        public bool Cancer { get; set; }

        [Display(Name = "Type")]
        public string CancerType { get; set; }

        [Display(Name = "Autoimmune Disorder")]
        public bool AutoimmuneDisorder { get; set; }

        [Display(Name = "Type")]
        public string AutoImmuneType { get; set; }

        [Display(Name = "Vasectomy")]
        public bool Vasectomy { get; set; }

        [Display(Name = "Orchitis")]
        public bool Orchitis { get; set; }

        [Display(Name = "Other Testicular Problems")]
        public bool OtherTesticularProblems { get; set; }
        public string TesticularProblems { get; set; }
    }

    public class PatientMedicalHistory_FamilyHistory
    {
        public int Id { get; set; }
        public int Cycle { get; set; }
        public int PatientId { get; set; }

        [Display(Name = "Diabetes")]
        public string Diabetes { get; set; }

        [Display(Name = "Breast Cancer")]
        public string BreastCancer { get; set; }

        [Display(Name = "Ovarian Cancer")]
        public string OvarianCancer { get; set; }

        [Display(Name = "Cervical Cancer")]
        public string CervicalCancer { get; set; }

        [Display(Name = "Prostrate Cancer")]
        public string ProstrateCancer { get; set; }

        [Display(Name = "Heart Disease")]
        public string HeartDisease { get; set; }

        [Display(Name = "Stroke")]
        public string Stroke { get; set; }

        [Display(Name = "Osteoporosis")]
        public string Osteoporosis { get; set; }

        [Display(Name = "Autoimmune Disorder (Include Type)")]
        public string AutoimmuneDisorder { get; set; }

        [Display(Name = "Thyroid Disorder")]
        public string ThyroidDisorder { get; set; }
    }

    public class PatientMedicalHistory_Allergy
    {
        public int Id { get; set; }
        public int Cycle { get; set; }
        public int PatientId { get; set; }

        [Display(Name = "Medication")]
        public string Medication1 { get; set; }

        [Display(Name = "Response")]
        public string MedicationResponse1 { get; set; }

        [Display(Name = "Food, etc.")]
        public string Food1 { get; set; }

        [Display(Name = "Response")]
        public string FoodResponse1 { get; set; }

        public string Medication2 { get; set; }
        public string MedicationResponse2 { get; set; }
        public string Food2 { get; set; }
        public string FoodResponse2 { get; set; }

        public string Medication3 { get; set; }
        public string MedicationResponse3 { get; set; }
        public string Food3 { get; set; }
        public string FoodResponse3 { get; set; }
    }

    public class PatientMedicalHistory_GeneralCondition
    {
        public int Id { get; set; }
        public int PatientId { get; set; }
        public DateTime? Date { get; set; }
        public string BloodPressure { get; set; }
        public string Glucose { get; set; }
        public string HeartRate { get; set; }

        [Required]
        [Display(Name = "Height*")]
        [Range(0.01, 9.00, ErrorMessage = "Please, Enter a valid height.")]
        public double Height { get; set; }
        [Required]
        [Display(Name = "Weight*")]
        [Range(0.01, 400.00, ErrorMessage = "Please, Enter a valid weight.")]
        public double Weight { get; set; }

        [Display(Name = "BMI (Body Mass Index, If Known)")]
        public double? BMI { get; set; }
    }

    public class PatientMedicalHistory_MedicationOTC
    {
        public int Id { get; set; }
        public int Cycle { get; set; }
        public int PatientId { get; set; }

        [Display(Name = "Pain Reliever")]
        public bool PainReliever { get; set; }
        public string PainRelieverNote { get; set; }
        public bool? PainRelieverFrequency { get; set; } // true == Regularly, false == Occasionally, null == No Answer

        [Display(Name = "Cold or Sinus Products")]
        public bool ColdProducts { get; set; }
        public string ColdProductsNote { get; set; }
        public bool? ColdProductsFrequency { get; set; }

        [Display(Name = "Diet Aids")]
        public bool DietAids { get; set; }
        public string DietAidsNote { get; set; }
        public bool? DietAidsFrequency { get; set; }

        [Display(Name = "Sleep Aids")]
        public bool SleepAids { get; set; }
        public string SleepAidsNote { get; set; }
        public bool? SleepAidsFrequency { get; set; }

        [Display(Name = "Laxatives")]
        public bool Laxatives { get; set; }
        public string LaxativesNote { get; set; }
        public bool? LaxativesFrequency { get; set; }

        [Display(Name = "Antacids/Acid Blockers")]
        public bool Antacids { get; set; }
        public string AntacidsNote { get; set; }
        public bool? AntacidsFrequency { get; set; }

        [Display(Name = "Anti Diarrheal")]
        public bool AntiDiarrheal { get; set; }
        public string AntiDiarrhealNote { get; set; }
        public bool? AntiDiarrhealFrequency { get; set; }

        [Display(Name = "Other")]
        public bool Other { get; set; }
        public bool? OtherFrequency { get; set; }
        public string Others { get; set; }
    }
    public class PatientMedicalHistory_MedicationRxViewModel
    {
        public int Id { get; set; }
        public int PatientId { get; set; }

        [Display(Name = "Medication/Strength")]
        public string DrugName { get; set; }

        [Display(Name = "Daily Dose")]
        public string DailyDose { get; set; }

        [Display(Name = "Date Started")]
        public DateTime? StartDate { get; set; }

        [Display(Name = "Date Discontinued")]
        public DateTime? EndDate { get; set; }

        [Display(Name = "Purpose of Use")]
        public string UseReason { get; set; }

        [Display(Name = "Reason For Stopping")]
        public string StopReason { get; set; }

        [Display(Name = "Issues Identified")]
        public bool? IssuesIdentified { get; set; }

        [Display(Name = "Additional Comments")]
        public string Comments { get; set; }

        public string Route { get; set; }
        public string RxCuis { get; set; }
        public string RateQuantity { get; set; }

        [Display(Name = "Dose Repetition Time")]
        public string DoseRepetitionTime { get; set; }
        [Display(Name = "Prescribe by")]
        public string PrescribeBy { get; set; }
        [Display(Name = "For How Long")]
        public string ForHowLong { get; set; }

        [Display(Name = "Are you taking medicine properly?")]
        public bool IsTakenMedicineProperly { get; set; }
        [Display(Name = "How many time medicine used?")]
        public string TakenMedicineProperly { get; set; }
        [Display(Name = "Is Permanent Medicine")]
        public bool IsPermanentMedicine { get; set; }
        [Display(Name = "Is Medicine Discontinued")]
        public bool IsMedicineDiscontinued { get; set; }
    }
    public class PatientMedicalHistory_MedicationRx
    {
        public int Id { get; set; }
        public int PatientId { get; set; }

        [Display(Name = "Medication/Strength")]
        public string DrugName { get; set; }

        [Display(Name = "Daily Dose")]
        public string DailyDose { get; set; }

        [Display(Name = "Date Started")]
        public DateTime? StartDate { get; set; }

        [Display(Name = "Date Discontinued")]
        public DateTime? EndDate { get; set; }

        [Display(Name = "Purpose of Use")]
        public string UseReason { get; set; }

        [Display(Name = "Reason For Stopping")]
        public string StopReason { get; set; }

        [Display(Name = "Issues Identified")]
        public bool? IssuesIdentified { get; set; }

        [Display(Name = "Additional Comments")]
        public string Comments { get; set; }

        public string Route { get; set; }
        public string RxCuis { get; set; }
        public string RateQuantity { get; set; }

        [Display(Name= "Dose Repetition Time")]
        public string DoseRepetitionTime { get; set; }
        [Display(Name= "Prescribe by")]
        public string PrescribeBy { get; set; }
        [Display(Name= "For How Long")]
        public string ForHowLong { get; set; }

        [Display(Name = "Are you taking medicine properly?")]
        public bool IsTakenMedicineProperly { get; set; }
        [Display(Name = "How many time medicine used?")]
        public string TakenMedicineProperly { get; set; }
        [Display(Name= "Is Permanent Medicine")]
        public bool IsPermanentMedicine { get; set; }
        [Display(Name= "Is Medicine Discontinued")]
        public bool IsMedicineDiscontinued { get; set; }
    }

    public class PatientDrugViewModel
    {
        public string PatientNo { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string RxNo { get; set; }
        public string BirthDate { get; set; }
        public string DateFilled { get; set; }
        public string Doctor { get; set; }
        public string DrugName { get; set; }
        public string RequestedRefill { get; set; }
        public string DidYouGetRx { get; set; }
        public string BodyPartsAffected { get; set; }
        public string Quantity { get; set; }
        public string QuantityCalculated { get; set; }
        public string NewRxFromBilling { get; set; }
        public string Validated { get; set; }
        public string ValidationDate { get; set; }
    }
}