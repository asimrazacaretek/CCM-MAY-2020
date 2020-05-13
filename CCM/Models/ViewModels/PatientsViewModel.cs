using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq.Expressions;

namespace CCM.Models.ViewModels
{
  
        public class PatientsViewModel
        {
            public int Id { get; set; }

            [Display(Name = "Date Updated")]
            [DataType(DataType.Date)]
            public DateTime UpdatedOn { get; set; }

            [Display(Name = "Updated By")]
            [Column(TypeName = "VARCHAR")]
            [StringLength(100)]
            public string UpdatedBy { get; set; }

            [Display(Name = "Date Created")]
            [DataType(DataType.Date)]
            public DateTime CreatedOn { get; set; }

            [Display(Name = "Created By")]
            [Column(TypeName = "VARCHAR")]
            [StringLength(100)]
            public string CreatedBy { get; set; }


            [Display(Name = "Prefix")]
            [Column(TypeName = "VARCHAR")]
            [StringLength(50)]
            public string Prefix { get; set; }

            [Required]
            [Display(Name = "First Name")]
            [Column(TypeName = "VARCHAR")]
            [StringLength(100)]
            [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Use Alphabets For Name")]
            public string FirstName { get; set; }

            [Display(Name = "Middle Name")]
            [Column(TypeName = "VARCHAR")]
            [StringLength(100)]

            public string MiddleName { get; set; }

            [Required]
            [Display(Name = "Last Name")]
            [Column(TypeName = "VARCHAR")]
            [StringLength(100)]
            [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Use Alphabets for Last Name")]
            public string LastName { get; set; }

            [Display(Name = "Suffix")]
            [Column(TypeName = "VARCHAR")]
            [StringLength(50)]
            public string Suffix { get; set; }

            [Required]
            [Display(Name = "Date of Birth")]
            [DataType(DataType.Date)]
            public DateTime BirthDate { get; set; }

            [Display(Name = "Gender")]
            public string Gender { get; set; }

            [Column(TypeName = "VARCHAR")]
            [StringLength(100)]
            [Display(Name = "Preferred Language")]
            public string PreferredLanguage { get; set; }

            [Column(TypeName = "VARCHAR")]
            [StringLength(100)]
            [Display(Name = "Please, specify other preferred language")]
            public string OtherLanguage { get; set; }


            [Display(Name = "Upload Photo")]
            public byte[] Photo { get; set; }

            [Display(Name = "EMR Records")]
            public byte[] PhotoEmrRecords { get; set; }

            [Display(Name = "EMR Records 2")]
            public byte[] PhotoEmrRecords2 { get; set; }

            [Display(Name = "EMR Records 3")]
            public byte[] PhotoEmrRecords3 { get; set; }

            [Display(Name = "EMR Records 4")]
            public byte[] PhotoEmrRecords4 { get; set; }

            [Display(Name = "EMR Records 5")]
            public byte[] PhotoEmrRecords5 { get; set; }

            [Display(Name = "EMR Records 6")]
            public byte[] PhotoEmrRecords6 { get; set; }

            //New
            [Display(Name = "EMR Records 7")]
            public byte[] PhotoEmrRecords7 { get; set; }

            [Display(Name = "EMR Records 8")]
            public byte[] PhotoEmrRecords8 { get; set; }

            [Display(Name = "EMR Records 9")]
            public byte[] PhotoEmrRecords9 { get; set; }

            [Display(Name = "EMR Records 10")]
            public byte[] PhotoEmrRecords10 { get; set; }

            [Display(Name = "EMR Records 11")]
            public byte[] PhotoEmrRecords11 { get; set; }

            [Display(Name = "EMR Records 12")]
            public byte[] PhotoEmrRecords12 { get; set; }

            [Display(Name = "EMR Records 13")]
            public byte[] PhotoEmrRecords13 { get; set; }

            [Display(Name = "EMR Records 14")]
            public byte[] PhotoEmrRecords14 { get; set; }

            [Display(Name = "EMR Records 15")]
            public byte[] PhotoEmrRecords15 { get; set; }
            //



            [Display(Name = "Best Time To Contact")]
            public int BestContactTime { get; set; }

            [Display(Name = "Mobile Phone Number")]
            [RegularExpression(@"^([0-9]{10})$", ErrorMessage = "Invalid Phone Number.")]
            public string MobilePhoneNumber { get; set; }

            [Display(Name = "Allow Text")]
            public bool AllowText { get; set; }

            [Display(Name = "Work Phone Number")]
            [RegularExpression(@"^([0-9]{10})$", ErrorMessage = "Invalid Work Phone Number.")]
            public string WorkPhoneNumber { get; set; }

            [Display(Name = "Home Phone Number")]
            [RegularExpression(@"^([0-9]{10})$", ErrorMessage = "Invalid Home Phone Number.")]
            [Range(1000000000, 9999999999, ErrorMessage = "Invalid Home Phone Number")]
            public string HomePhoneNumber { get; set; }

            [Display(Name = "Emergency Contact Number")]
            [RegularExpression(@"^([0-9]{10})$", ErrorMessage = "Invalid Emergency Number.")]
            [Range(1000000000, 9999999999, ErrorMessage = "Invalid Emergency Number")]
            public string EmergencyNumber { get; set; }

            [EmailAddress]
            [Column(TypeName = "VARCHAR")]
            [StringLength(100)]
            public string Email { get; set; }

            [Display(Name = "Allow Email")]
            public bool AllowEmail { get; set; }


            [Display(Name = "Address 1")]
            [Column(TypeName = "VARCHAR")]
            [StringLength(100)]
            public string Address1 { get; set; }

            [Display(Name = "Floor/Apt/Suite")]
            [Column(TypeName = "VARCHAR")]
            [StringLength(100)]
            public string Address2 { get; set; }

            [Display(Name = "City")]
            [Column(TypeName = "VARCHAR")]
            [StringLength(50)]
            public string City { get; set; }

            [Display(Name = "State")]
            [Column(TypeName = "VARCHAR")]
            [StringLength(50)]
            public string State { get; set; }

            [Display(Name = "Zipcode")]
            [DataType(DataType.PostalCode)]
            public string Zipcode { get; set; }

            [Display(Name = "Building Type")]
            [Column(TypeName = "VARCHAR")]
            [StringLength(50)]
            public string BuildingType { get; set; }

            [Display(Name = "Delivery Permission")]
            public bool DeliveryPermisison { get; set; }

            [Display(Name = "Delivery Instruction")]
            [Column(TypeName = "VARCHAR")]
            public string DeliveryInstruction { get; set; }


            // Insurance
            [Column(TypeName = "VARCHAR")]
            [StringLength(100)]
            [Display(Name = "Medicare ID#")]
            public string MedicareIdNumber { get; set; }

            [Column(TypeName = "VARCHAR")]
            [StringLength(100)]
            [Display(Name = "Medicaid ID#")]
            public string MedicaidIdNumber { get; set; }

            [Column(TypeName = "VARCHAR")]
            [StringLength(100)]
            [Display(Name = "Other Insurance ID#")]
            public string OtherInsuranceIdNumber { get; set; }


            // Care Taker
            [Display(Name = "Caretaker's First Name")]
            [Column(TypeName = "VARCHAR")]
            [StringLength(100)]
            public string CaretakerFirstName { get; set; }

            [Display(Name = "Caretaker's Last Name")]
            [Column(TypeName = "VARCHAR")]
            [StringLength(100)]
            public string CaretakerLastName { get; set; }

            [Display(Name = "Caretaker's Phone Number")]
            [RegularExpression(@"^([0-9]{10})$", ErrorMessage = "Invalid Caretaker Phone Number.")]
            [Range(1000000000, 9999999999, ErrorMessage = "Invalid Caretaker Phone Number")]
            public string CaretakerPhoneNumber { get; set; }

            [Display(Name = "Caretaker's Relationship")]
            [Column(TypeName = "VARCHAR")]
            [StringLength(100)]
            public string CaretakerRelationship { get; set; }


            [Display(Name = "Notes")]
            public string Notes { get; set; }

            [Display(Name = "Enrollment Notes")]
            public string EnrollmentNotes { get; set; }

            [Display(Name = "CCM Enrolled Date")]
            [DataType(DataType.Date)]
            public DateTime CCMEnrolledOn { get; set; }

            [Display(Name = "CCM Enrolled By")]
            [Column(TypeName = "VARCHAR")]
            [StringLength(100)]
            public string CCMEnrolledBy { get; set; }

            public string EnrollmentStatus { get; set; }
            public string CcmStatus { get; set; }
            public DateTime CcmClinicalSignOffDate { get; set; }
            public DateTime CcmClaimSubmissionDate { get; set; }
            public DateTime CcmReconciliationDate { get; set; }
            public DateTime AppointmentDate { get; set; }
            public string CcmBillingCode { get; set; }
            public string CcmBillingCode2 { get; set; }
            public byte[] FinalCarePlanPdf { get; set; }
            public int Cycle { get; set; }

            [Display(Name = "Councler Assigned Date")]
            [DataType(DataType.Date)]
            public DateTime LiasionAssignedOn { get; set; }
            public string LiasionAssignedBy { get; set; }

            [Column(TypeName = "VARCHAR")]
            [StringLength(100)]
            public string CallingStatus { get; set; }


            public string CallingNote { get; set; }
            // Foreign Keys
            [Display(Name = "Liaison")]
            public int LiaisonId { get; set; }

            [Required]
            [Display(Name = "Patient's Physician")]
            public int PhysicianId { get; set; }

            [Display(Name = "Chronic Condition 1")]
            public int PatientChronicCondition1Id { get; set; }

            [Display(Name = "Chronic Condition 2")]
            public int PatientChronicCondition2Id { get; set; }

            [Column(TypeName = "VARCHAR")]
            [StringLength(100)]
            public string EnrollmentSubStatus { get; set; }

            [Column(TypeName = "VARCHAR")]
            [StringLength(100)]
            public string EnrollmentSubStatusReason { get; set; }
            [Required]
            [Column(TypeName = "VARCHAR")]
            [StringLength(100)]
            public string EMRNumber { get; set; } = "";
            [Column(TypeName = "VARCHAR")]
            [StringLength(100)]
            public string EMRType { get; set; }

            public string EnrollmentStatusNotes { get; set; }
            [Column(TypeName = "VARCHAR")]
            [StringLength(5)]
            public string PicassoChecked { get; set; } = "No";
            [Display(Name = "Picasso Checked Date")]
            [DataType(DataType.Date)]
            public DateTime PicassoCheckedOn { get; set; }
            [Column(TypeName = "VARCHAR")]
            [StringLength(5)]
            public string CapitatedPatient { get; set; } = "No";
            [Display(Name = "Capitated From")]
            [DataType(DataType.Date)]
            public DateTime CapitatedFrom { get; set; }
            [Display(Name = "Capitated To")]
            [DataType(DataType.Date)]
            public DateTime CapitatedTo { get; set; }

            [Display(Name = "Translator")]
            public int? TranslatorId { get; set; }
            [Display(Name = "Translator Assigned Date")]
            [DataType(DataType.Date)]
            public DateTime TranslatorAssignedOn { get; set; }
            public string TranslatorAssignedBy { get; set; }

            [Display(Name = "Patient Signature")]
            public byte[] Signature { get; set; }

            // My Profile Foreign Keys start here
            public int ProfileId { get; set; }
            public int ContactId { get; set; }
            public int AddressId { get; set; }
            public int UrgencyContactId { get; set; }
            public int InsuranceId { get; set; }

            // My Medical History Foreign Keys start here
            public int MedicalStatusId { get; set; }
            public int MedicalConditionId { get; set; }
            public int FamilyHistoryId { get; set; }
            public int AllergyId { get; set; }
            public int MedicationOtcId { get; set; }

            // My Lifestyle Foreign Key
            public int WorkAndRelationshipId { get; set; }
            public int DietAndHabitId { get; set; }
            public int LifeStressId { get; set; }
            public int NutritionalSupplementId { get; set; }




            // Reference Tables
            public virtual Physician Physician { get; set; }
            public virtual Liaison Liaison { get; set; }
            public virtual Patient_ChronicCondition1 PatientChronicCondition1 { get; set; }
            public virtual Patient_ChronicCondition2 PatientChronicCondition2 { get; set; }

            // My Profile Reference Tables
            public virtual PatientProfile Profile { get; set; }
            public virtual PatientProfile_Contact Contact { get; set; }
            public virtual PatientProfile_Address Address { get; set; }
            public virtual PatientProfile_UrgencyContact UrgencyContact { get; set; }
            public virtual PatientProfile_Insurance Insurance { get; set; }

            // My Medical History Reference Tables
            public virtual PatientMedicalHistory_MedicalStatus MedicalStatus { get; set; }
            public virtual PatientMedicalHistory_MedicalCondition MedicalCondition { get; set; }
            public virtual PatientMedicalHistory_FamilyHistory FamilyHistory { get; set; }
            public virtual PatientMedicalHistory_Allergy Allergy { get; set; }
            public virtual PatientMedicalHistory_MedicationOTC MedicationOtc { get; set; }
            public virtual PatientMedicalHistory_GeneralCondition GeneralCondition { get; set; }

            // My Lifestyle Reference Tables
            public virtual PatientLifestyle_WorkAndRelationship WorkAndRelationship { get; set; }
            public virtual PatientLifestyle_DietAndHabit DietAndHabit { get; set; }
            public virtual PatientLifestyle_LifeStress LifeStress { get; set; }
            public virtual PatientLifestyle_NutritionalSupplement NutritionalSupplement { get; set; }

            public virtual PatientMeidcareMedicaidEligibility PatientMeidcareMedicaidEligibility { get; set; }

            public virtual ICollection<BillingCycle> BillingCycles { get; set; }

            public virtual ICollection<CCMCycleStatus> CCMCycleStatuses { get; set; }
        }

    
}