using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq.Expressions;
using System.Collections.Specialized;
using CCM.Models.CCMBILLINGS;
using CCM.Models.CCMBILLINGS.ViewModels;
using CCM.Models.DataModels;

namespace CCM.Models
{
    public class GroupchatdetailsViewModel
    {
        public string Message { get; set; }
        public DateTime timesent { get; set;
        }
        public string sender { get; set; }

        public string senderclass { get; set; }
        public int LastID { get; set; }
public int TotalMessages { get; set; }
        public bool isNew { get; set; }

        public string userid { get; set; }

    }
    public class GroupChatTotalCounts
    {
        public int GroupChatId { get; set; }
        public int TotalCount { get; set; }
    }
    public class GroupchatViewModel
    {
        public GroupChat GroupChat { get; set; }
        public List<UsersViewModel> usersViewModels { get; set; }

        public int TotalCount { get; set; }
    }
    //public class UsersViewModel
    //{
    //    public string Id { get; set; }
    //    public string Name { get; set; }
    //}
    public class GroupChat
    {
        public int Id { get; set; }
        public string ChatName { get; set; }
        [Display(Name = "Date Created")]
        [DataType(DataType.Date)]
        public DateTime CreatedOn { get; set; }
       
        public string CreatedBy { get; set; }

        [Display(Name = "Date Updated")]
        [DataType(DataType.Date)]
        public DateTime? UpdatedOn { get; set; }

        public string UpdatedBy { get; set; }

        public string ChatStatus { get; set; }

        public bool isTicket { get; set; }

        public string TicketNum { get; set; }
        public string TicketTitle { get; set; }



        public string Department { get; set; }

       


    }
    public class GroupChatParticipants
    {
        public int Id { get; set; }
        public int GroupChatId { get; set; }

        public string UserId { get; set; }

        public bool isCreater { get; set; }


    }
    public class GroupChatDetails
    {
        public int Id { get; set; }
        public int GroupChatId { get; set; }

        public string Message { get; set; }

        [Display(Name = "Date Created")]
        [DataType(DataType.Date)]
        public DateTime? CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        
       


    }
    public class GroupChatNewMessage
    {
        public int Id { get; set; }
        public int GroupChatDetailsId { get; set; }

        public int MessageId { get; set; }

       public string ReceivedBy { get; set; }
        public DateTime ReceivedOn { get; set; }
        public bool isNew { get; set; }



    }
    public class AutomaticCycleStatusEntry
    {
        public int Id { get; set; }
        public int EntryMonth { get; set; }
        public int EntryYear { get; set; }


    }
    public class CarePlanSharedHistory
    {
        public int Id { get; set; }
       
        [Display(Name = "Date Created")]
        [DataType(DataType.Date)]
        public DateTime? CreatedOn { get; set; }
      [Display(Name = "Created By")]
        [Column(TypeName = "VARCHAR")]
        [StringLength(100)]
        public string CreatedBy { get; set; }
        public int PatientId { get; set; }

        public int Cycle { get; set; }

        public string EmailSentTo { get; set; }

        public string ReceiverName { get; set; }

        public int ReceiverId { get; set; }
        public string ReceiverType { get; set; }

    }
    public class PatientNotes
    {
        public int Id { get; set; }
        public string Notes { get; set; }
        [Display(Name = "Date Created")]
        [DataType(DataType.Date)]
        public DateTime? CreatedOn { get; set; }

        [Display(Name = "Created By")]
        [Column(TypeName = "VARCHAR")]
        [StringLength(100)]
        public string CreatedBy { get; set; }
        [StringLength(100)]
        public string Module { get; set; }
        public int PatientId { get; set; }
        public bool isToShowinPopup { get; set; }
    }
    public class CallHistoryandReviewTimes
    {
        public string TotalCallMinutes { get; set; }
        public string TotalCallAttempts { get; set; }
        public string CarePlanSubmittedMin { get; set; }
        public string CarePlanClass { get; set; }
        public string AppointmentClass { get; set; }
        public string AppointmentDate { get; set; }

        public string TotalActivityTime { get; set; }
        public string PatientID { get; set; }
        public string patientIdandname { get; set; }
        public string CounslerName { get; set; }
        public List<CallHistory> callHistories { get; set; }
        public List<ReviewTimeCcm> reviewTimeCcms { get; set; }
    }


    //
    public class Patient
    {
        public int Id { get; set; }

        [Display(Name = "Date Updated")]
        [DataType(DataType.Date)]
        public DateTime? UpdatedOn { get; set; }

        [Display(Name = "Updated By")]
        [Column(TypeName = "VARCHAR")]
        [StringLength(100)]
        public string UpdatedBy { get; set; }

        [Display(Name = "Date Created")]
        [DataType(DataType.Date)]
        public DateTime? CreatedOn { get; set; }

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
        public int? BestContactTime { get; set; }
      
        [Display(Name = "Mobile Phone Number")]
        [RegularExpression(@"^([0-9]{10})$", ErrorMessage = "Invalid Phone Number.")]
        public string MobilePhoneNumber { get; set; }

        [Display(Name = "Allow Text")]
        public bool? AllowText { get; set; }

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
        public bool? AllowEmail { get; set; }


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
        public bool? DeliveryPermisison { get; set; }

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
        public DateTime? CCMEnrolledOn { get; set; }

        [Display(Name = "CCM Enrolled By")]
        [Column(TypeName = "VARCHAR")]
        [StringLength(100)]
        public string CCMEnrolledBy { get; set; }

        public string EnrollmentStatus { get; set; }
        public string CcmStatus { get; set; }
        public DateTime? CcmClinicalSignOffDate { get; set; }
        public DateTime? CcmClaimSubmissionDate { get; set; }
        public DateTime? CcmReconciliationDate { get; set; }
        public DateTime? AppointmentDate { get; set; }
        public string CcmBillingCode { get; set; }
        public string CcmBillingCode2 { get; set; }
        public byte[] FinalCarePlanPdf { get; set; }
        public int Cycle { get; set; }

        [Display(Name = "Councler Assigned Date")]
        [DataType(DataType.Date)]
        public DateTime? LiasionAssignedOn { get; set; }
        public string LiasionAssignedBy { get; set; }

        [Column(TypeName = "VARCHAR")]
        [StringLength(100)]
        public string CallingStatus { get; set; }


        public string CallingNote { get; set; }
        // Foreign Keys
        //[Display(Name = "Pre-Counselor")]
        public int? LiaisonId { get; set; }

        [Required]
        [Display(Name = "Patient's Physician")]
        public int? PhysicianId { get; set; }

        [Display(Name = "Chronic Condition 1")]
        public int? PatientChronicCondition1Id { get; set; }

        [Display(Name = "Chronic Condition 2")]
        public int? PatientChronicCondition2Id { get; set; }

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
        public DateTime? PicassoCheckedOn { get; set; }
        [Column(TypeName = "VARCHAR")]
        [StringLength(5)]
        public string CapitatedPatient { get; set; } = "No";
        [Display(Name = "Capitated From")]
        [DataType(DataType.Date)]
        public DateTime? CapitatedFrom { get; set; }
        [Display(Name = "Capitated To")]
        [DataType(DataType.Date)]
        public DateTime? CapitatedTo { get; set; }

        //[Display(Name = "Pre-Translator")]
        public int? TranslatorId { get; set; }
        [Display(Name = "Translator Assigned Date")]
        [DataType(DataType.Date)]
        public DateTime? TranslatorAssignedOn { get; set; }
        public string TranslatorAssignedBy { get; set; }

        [Display(Name = "Patient Signature")]
        public byte[] Signature { get; set; }

        // My Profile Foreign Keys start here
        public int? ProfileId { get; set; }
        public int? ContactId { get; set; }
        public int? AddressId { get; set; }
        public int? UrgencyContactId { get; set; }
        public int? InsuranceId { get; set; }

        // My Medical History Foreign Keys start here
        public int? MedicalStatusId { get; set; }
        public int? MedicalConditionId { get; set; }
        public int? FamilyHistoryId { get; set; }
        public int? AllergyId { get; set; }
        public int? MedicationOtcId { get; set; }

        // My Lifestyle Foreign Key
        public int? WorkAndRelationshipId { get; set; }
        public int? DietAndHabitId { get; set; }
        public int? LifeStressId { get; set; }
        public int? NutritionalSupplementId { get; set; }
        [ForeignKey("Patients_PreLiaisons")]
        public int? Patients_PreLiaisonsId { get; set; }





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

     

        public virtual List<Patients_BillingCategories> Patients_BillingCategories { get; set; }
        public virtual List<Patients_Services> Patients_Services { get; set; }

        public virtual Patients_PreLiaisons Patients_PreLiaisons { get; set; }

    }

    public class patients_historyviewmodel
    {
        public string EnrollmentStatus { get; set; }
        public string EnrollmentSubStatus { get; set; }
        public string EnrollmentSubStatusReason { get; set; }
        public int LiaisonId { get; set; }
        public int? PhysicianId { get; set; }

        public DateTime? UpdatedOn { get; set; }
        public long ROWNUMBER { get; set; }
    }
    public class Patients_History
    {
        public int Id { get; set; }





        [Display(Name = "Date Updated")]
        [DataType(DataType.Date)]
        public DateTime? UpdatedOn { get; set; }

        [Display(Name = "Updated By")]
        [Column(TypeName = "VARCHAR")]
        [StringLength(100)]
        public string UpdatedBy { get; set; }

        [Display(Name = "Date Created")]
        [DataType(DataType.Date)]
        public DateTime? CreatedOn { get; set; }

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
        public string FirstName { get; set; }

        [Display(Name = "Middle Name")]
        [Column(TypeName = "VARCHAR")]
        [StringLength(100)]
        public string MiddleName { get; set; }

        [Required]
        [Display(Name = "Last Name")]
        [Column(TypeName = "VARCHAR")]
        [StringLength(100)]
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
        public int? BestContactTime { get; set; }

        [Display(Name = "Mobile Phone Number")]
        [RegularExpression(@"^([0-9]{10})$", ErrorMessage = "Invalid Phone Number.")]
        [Range(1000000000, 9999999999, ErrorMessage = "Invalid Phone Number")]
        public string MobilePhoneNumber { get; set; }

        [Display(Name = "Allow Text")]
        public bool? AllowText { get; set; }

        [Display(Name = "Work Phone Number")]
        [RegularExpression(@"^([0-9]{10})$", ErrorMessage = "Invalid Phone Number.")]
        [Range(1000000000, 9999999999, ErrorMessage = "Invalid Phone Number")]
        public string WorkPhoneNumber { get; set; }

        [Display(Name = "Home Phone Number")]
        [RegularExpression(@"^([0-9]{10})$", ErrorMessage = "Invalid Phone Number.")]
        [Range(1000000000, 9999999999, ErrorMessage = "Invalid Phone Number")]
        public string HomePhoneNumber { get; set; }

        [Display(Name = "Emergency Contact Number")]
        [RegularExpression(@"^([0-9]{10})$", ErrorMessage = "Invalid Phone Number.")]
        [Range(1000000000, 9999999999, ErrorMessage = "Invalid Phone Number")]
        public string EmergencyNumber { get; set; }

        [EmailAddress]
        [Column(TypeName = "VARCHAR")]
        [StringLength(100)]
        public string Email { get; set; }

        [Display(Name = "Allow Email")]
        public bool? AllowEmail { get; set; }


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
        [RegularExpression(@"^([0-9]{10})$", ErrorMessage = "Invalid Phone Number.")]
        [DataType(DataType.PostalCode)]
        public string Zipcode { get; set; }

        [Display(Name = "Building Type")]
        [Column(TypeName = "VARCHAR")]
        [StringLength(50)]
        public string BuildingType { get; set; }

        [Display(Name = "Delivery Permission")]
        public bool? DeliveryPermisison { get; set; }

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
        [RegularExpression(@"^([0-9]{10})$", ErrorMessage = "Invalid Phone Number.")]
        [Range(1000000000, 9999999999, ErrorMessage = "Invalid Phone Number")]
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
        public DateTime? CCMEnrolledOn { get; set; }

        [Display(Name = "CCM Enrolled By")]
        [Column(TypeName = "VARCHAR")]
        [StringLength(100)]
        public string CCMEnrolledBy { get; set; }

        public string EnrollmentStatus { get; set; }
        public string CcmStatus { get; set; }
        public DateTime? CcmClinicalSignOffDate { get; set; }
        public DateTime? CcmClaimSubmissionDate { get; set; }
        public DateTime? CcmReconciliationDate { get; set; }
        public DateTime? AppointmentDate { get; set; }
        public string CcmBillingCode { get; set; }
        public string CcmBillingCode2 { get; set; }
        public byte[] FinalCarePlanPdf { get; set; }
        public int Cycle { get; set; }

        [Display(Name = "Councler Assigned Date")]
        [DataType(DataType.Date)]
        public DateTime? LiasionAssignedOn { get; set; }
        public string LiasionAssignedBy { get; set; }

        [Column(TypeName = "VARCHAR")]
        [StringLength(100)]
        public string CallingStatus { get; set; }


        public string CallingNote { get; set; }
        // Foreign Keys
        [Display(Name = "Liaison")]
        public int? LiaisonId { get; set; }

        [Required]
        [Display(Name = "Patient's Physician")]
        public int? PhysicianId { get; set; }

        [Display(Name = "Chronic Condition 1")]
        public int? PatientChronicCondition1Id { get; set; }

        [Display(Name = "Chronic Condition 2")]
        public int? PatientChronicCondition2Id { get; set; }

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
        public DateTime? PicassoCheckedOn { get; set; }
        [Column(TypeName = "VARCHAR")]
        [StringLength(5)]
        public string CapitatedPatient { get; set; } = "No";
        [Display(Name = "Capitated From")]
        [DataType(DataType.Date)]
        public DateTime? CapitatedFrom { get; set; }
        [Display(Name = "Capitated To")]
        [DataType(DataType.Date)]
        public DateTime? CapitatedTo { get; set; }

        public int? TranslatorId { get; set; }
        [Display(Name = "Translator Assigned Date")]
        [DataType(DataType.Date)]
        public DateTime? TranslatorAssignedOn { get; set; }
        public string TranslatorAssignedBy { get; set; }


        [Display(Name = "Patient Signature")]
        public byte[] Signature { get; set; }
        // My Profile Foreign Keys start here
        public int? ProfileId { get; set; }
        public int? ContactId { get; set; }
        public int? AddressId { get; set; }
        public int? UrgencyContactId { get; set; }
        public int? InsuranceId { get; set; }

        // My Medical History Foreign Keys start here
        public int? MedicalStatusId { get; set; }
        public int? MedicalConditionId { get; set; }
        public int? FamilyHistoryId { get; set; }
        public int? AllergyId { get; set; }
        public int? MedicationOtcId { get; set; }

        // My Lifestyle Foreign Key
        public int? WorkAndRelationshipId { get; set; }
        public int? DietAndHabitId { get; set; }
        public int? LifeStressId { get; set; }
        public int? NutritionalSupplementId { get; set; }
        public int? Patients_PreLiaisonsId { get; set; }

    }
    public class PatientMeidcareMedicaidEligibility
    {
        public int Id { get; set; }

        public int PatientId { get; set; }
        [Column(TypeName = "VARCHAR")]
        [StringLength(5)]
        public string MedicareEligibilty { get; set; }
        public string MedicareEligibiltyNotes { get; set; }
        [Display(Name = "Upload Screenshot")]
        public byte[] MedicareEligibiltySceenshot { get; set; }
        [Column(TypeName = "VARCHAR")]
        [StringLength(5)]
        public string MedicaidEligibilty { get; set; }
        public string MedicaidEligibiltyNotes { get; set; }
        [Display(Name = "Upload Screenshot")]
        public byte[] MedicaidEligibiltySceenshot { get; set; }
        [Display(Name = "Date Updated")]
        [DataType(DataType.Date)]
        public DateTime? UpdatedOn { get; set; }

        [Display(Name = "Updated By")]
        [Column(TypeName = "VARCHAR")]
        [StringLength(100)]
        public string UpdatedBy { get; set; }

        [Display(Name = "Date Created")]
        [DataType(DataType.Date)]
        public DateTime? CreatedOn { get; set; }

        [Display(Name = "Created By")]
        [Column(TypeName = "VARCHAR")]
        [StringLength(100)]
        public string CreatedBy { get; set; }

    }
    public class CCMCycleStatus
    {
        public int Id { get; set; }
        public int PatientId { get; set; }
        [Column(TypeName = "VARCHAR")]
        [StringLength(50)]
        public string CCMStatus { get; set; }

        [Column(TypeName = "VARCHAR")]
        [StringLength(50)]
        public string CCMSubStatus { get; set; }

        public int Cycle { get; set; }

        [Display(Name = "Date Updated")]
        [DataType(DataType.Date)]
        public DateTime? UpdatedOn { get; set; }

        [Display(Name = "Updated By")]
        [Column(TypeName = "VARCHAR")]
        [StringLength(100)]
        public string UpdatedBy { get; set; }

        [Display(Name = "Date Created")]
        [DataType(DataType.Date)]
        public DateTime? CreatedOn { get; set; }

        [Display(Name = "Created By")]
        [Column(TypeName = "VARCHAR")]
        [StringLength(100)]
        public string CreatedBy { get; set; }

        [Display(Name = "Reconciliation Date")]
        [DataType(DataType.Date)]
        public DateTime? CcmReconciliationDate { get; set; }

        [Display(Name = "Claim Submission Date")]
        [DataType(DataType.Date)]
        public DateTime? CcmClaimSubmissionDate { get; set; }

        [Display(Name = "Clinical SignOff Date")]
        [DataType(DataType.Date)]
        public DateTime? CcmClinicalSignOffDate { get; set; }

        [Display(Name = "Careplan Rejected Date")]
        [DataType(DataType.Date)]
        public DateTime? CcmRejectedDate { get; set; }

        public int RejectedCount { get; set; } = 0;

        public string CCMNotes { get; set; }

        [Display(Name = "Approved By")]
        [Column(TypeName = "VARCHAR")]
        [StringLength(100)]
        public string ApprovedBy { get; set; }

        [Display(Name = "Rejected By")]
        [Column(TypeName = "VARCHAR")]
        [StringLength(100)]
        public string RejectedBy { get; set; }

        [Display(Name = "Submitted By")]
        [Column(TypeName = "VARCHAR")]
        [StringLength(100)]
        public string SubmittedBy { get; set; }

        [Display(Name = "Ready for Clinical Signoff Date")]
        [DataType(DataType.Date)]
        public DateTime? CcmReadyforClinicalSignOffDate { get; set; }

        [Display(Name = "Submitted By Ready For Clinical Signoff")]
        [Column(TypeName = "VARCHAR")]
        [StringLength(100)]
        public string SubmittedByReadyforClinicalSignoff { get; set; }

        public bool IsRejectedByLiaison { get; set; } = false;
        [Column(TypeName = "VARCHAR")]
        [StringLength(100)]
        public string RejectedbyLiaison { get; set; }

        [Display(Name = "Rejected Date")]
        [DataType(DataType.Date)]
        public DateTime? CcmRejectedDatebyLiaison { get; set; }

        [Display(Name = "Submitted To Ready For Clinical Signoff")]
        
        public string SubmittedToReadyforClinicalSignoff { get; set; }

       // public int? clinicalSignOffQueCounter { get; set; }

        //public int clinicalSignOffCounter { get; set; }

        //[Display(Name = "Expired Date")]
        //[DataType(DataType.Date)]
        //public DateTime? CcmExpiredCycleDate { get; set; }


    }

    public class CCMCycleStatusRejectionHistory {

        public int Id  { get; set; }
        public int PatientId { get; set; }

        [Column(TypeName = "VARCHAR")]
        [StringLength(50)]
        public string CCMStatus { get; set; }
        public int Cycle { get; set; }

        [Display(Name = "Date Updated")]
        [DataType(DataType.Date)]
        public DateTime? rejectionDate { get; set; }

        [Display(Name = "Submitted By")]
        [Column(TypeName = "VARCHAR")]
        [StringLength(100)]
        public string SubmittedBy { get; set; }

        [ForeignKey("BillingCategory")]
        public int? BillingCategoryId { get; set; }
        public virtual BillingCategory BillingCategory { get; set; }



    }
    public class BillingCycle
    {
        public int Id { get; set; }
        public int PatientId { get; set; }

        public int Cycle { get; set; }
        [Column(TypeName = "VARCHAR")]
        [StringLength(50)]
        public string BillingCode1 { get; set; }
        [Column(TypeName = "VARCHAR")]
        [StringLength(50)]
        public string BillingCode2 { get; set; }

        [Display(Name = "Date Updated")]
        [DataType(DataType.Date)]
        public DateTime? UpdatedOn { get; set; }

        [Display(Name = "Updated By")]
        [Column(TypeName = "VARCHAR")]
        [StringLength(100)]
        public string UpdatedBy { get; set; }

        [Display(Name = "Date Created")]
        [DataType(DataType.Date)]
        public DateTime? CreatedOn { get; set; }

        [Display(Name = "Created By")]
        [Column(TypeName = "VARCHAR")]
        [StringLength(100)]
        public string CreatedBy { get; set; }

        public decimal BillingCode1Rate { get; set; }
        public decimal BillingCode2Rate { get; set; }

        public decimal BillingCode1PhysicianRateInvoice { get; set; }
        public decimal BillingCode2PhysicianRateInvoice { get; set; }

        public decimal BillingCode1PhysicianRateBilling { get; set; }
        public decimal BillingCode2PhysicianRateBilling { get; set; }

        public ICollection<BillingCycleDetails> BillingCycleDetails { get; set; }

        public ICollection<BillingCycleComments> BillingCycleComments { get; set; }
        public virtual List<BillingCodes> BillingCodes { get; set; }
        public virtual List<Liaison_CPTRates> Liaison_CPTRates { get; set; }
        public virtual List<Physician_CPTRates> Physician_CPTRates { get; set; }




    }
    public class BillingCycleDetails
    {
        public int Id { get; set; }
        public int BillingCycleId { get; set; }

        public int RecordingID { get; set; }


        [Display(Name = "Date Updated")]
        [DataType(DataType.Date)]
        public DateTime? UpdatedOn { get; set; }

        [Display(Name = "Updated By")]
        [Column(TypeName = "VARCHAR")]
        [StringLength(100)]
        public string UpdatedBy { get; set; }

        [Display(Name = "Date Created")]
        [DataType(DataType.Date)]
        public DateTime? CreatedOn { get; set; }

        [Display(Name = "Created By")]
        [Column(TypeName = "VARCHAR")]
        [StringLength(100)]
        public string CreatedBy { get; set; }

        public bool isDeleted { get; set; } = false;

        public BillingCycle BillingCycle { get; set; }
    }
    public class BillingCycleComments
    {
        public int Id { get; set; }
        public int BillingCycleId { get; set; }

        public string Comments { get; set; }

        [Column(TypeName = "VARCHAR")]
        [StringLength(50)]
        public string Status { get; set; }
        [Display(Name = "Date Updated")]
        [DataType(DataType.Date)]
        public DateTime? UpdatedOn { get; set; }

        [Display(Name = "Updated By")]
        [Column(TypeName = "VARCHAR")]
        [StringLength(100)]
        public string UpdatedBy { get; set; }

        [Display(Name = "Date Created")]
        [DataType(DataType.Date)]
        public DateTime? CreatedOn { get; set; }


        [Display(Name = "Created By")]
        [Column(TypeName = "VARCHAR")]
        [StringLength(100)]
        public string CreatedBy { get; set; }



        public BillingCycle BillingCycle { get; set; }
    }
    public class PatientListItem
    {
        public int Id { get; set; }

        public DateTime? UpdatedOn { get; set; }


        public string UpdatedBy { get; set; }


        public DateTime? CreatedOn { get; set; }


        public string CreatedBy { get; set; }



        public string Prefix { get; set; }


        public string FirstName { get; set; }


        public string MiddleName { get; set; }


        public string LastName { get; set; }


        public string Suffix { get; set; }


        public DateTime BirthDate { get; set; }


        public string Gender { get; set; }

        public string PreferredLanguage { get; set; }


        public string OtherLanguage { get; set; }



        //public byte[] Photo { get; set; }


        //public byte[] PhotoEmrRecords { get; set; }


        //public byte[] PhotoEmrRecords2 { get; set; }


        //public byte[] PhotoEmrRecords3 { get; set; }


        //public byte[] PhotoEmrRecords4 { get; set; }


        //public byte[] PhotoEmrRecords5 { get; set; }


        //public byte[] PhotoEmrRecords6 { get; set; }



        public int? BestContactTime { get; set; }


        public string MobilePhoneNumber { get; set; }


        public bool? AllowText { get; set; }


        public string WorkPhoneNumber { get; set; }


        public string HomePhoneNumber { get; set; }


        public string EmergencyNumber { get; set; }


        public string Email { get; set; }


        public bool? AllowEmail { get; set; }



        public string Address1 { get; set; }


        public string Address2 { get; set; }


        public string City { get; set; }


        public string State { get; set; }


        public string Zipcode { get; set; }


        public string BuildingType { get; set; }


        public bool? DeliveryPermisison { get; set; }


        public string DeliveryInstruction { get; set; }


        // Insurance

        public string MedicareIdNumber { get; set; }


        public string MedicaidIdNumber { get; set; }


        public string OtherInsuranceIdNumber { get; set; }


        // Care Taker

        public string CaretakerFirstName { get; set; }


        public string CaretakerLastName { get; set; }


        public string CaretakerPhoneNumber { get; set; }


        public string CaretakerRelationship { get; set; }



        public string Notes { get; set; }


        public string EnrollmentNotes { get; set; }


        public DateTime? CCMEnrolledOn { get; set; }


        public string CCMEnrolledBy { get; set; }

        public string EnrollmentStatus { get; set; }
        public string CcmStatus { get; set; }
        public DateTime? CcmClinicalSignOffDate { get; set; }
        public DateTime? CcmClaimSubmissionDate { get; set; }
        public DateTime? CcmReconciliationDate { get; set; }
        public DateTime? AppointmentDate { get; set; }
        public string CcmBillingCode { get; set; }
        public string CcmBillingCode2 { get; set; }
        //public byte[] FinalCarePlanPdf { get; set; }
        public int Cycle { get; set; }


        // Foreign Keys

        public int? LiaisonId { get; set; }

        public string LiaisonFullName { get; set; }


        public int? PhysicianId { get; set; }

        public string PhysicianFullName { get; set; }


        public int? PatientChronicCondition1Id { get; set; }


        public int? PatientChronicCondition2Id { get; set; }

        // My Profile Foreign Keys start here
        public int? ProfileId { get; set; }
        public int? ContactId { get; set; }
        public int? AddressId { get; set; }
        public int? UrgencyContactId { get; set; }
        public int? InsuranceId { get; set; }

        // My Medical History Foreign Keys start here
        public int? MedicalStatusId { get; set; }
        public int? MedicalConditionId { get; set; }
        public int? FamilyHistoryId { get; set; }
        public int? AllergyId { get; set; }
        public int? MedicationOtcId { get; set; }

        // My Lifestyle Foreign Key
        public int? WorkAndRelationshipId { get; set; }
        public int? DietAndHabitId { get; set; }
        public int? LifeStressId { get; set; }
        public int? NutritionalSupplementId { get; set; }




        public static Expression<Func<Patient, PatientListItem>> Projection => x => new PatientListItem()
        {
            Id = x.Id,
            Address1 = x.Address1,
            Address2 = x.Address2,
            AddressId = x.AddressId,
            AllergyId = x.AllergyId,
            AllowEmail = x.AllowEmail,
            AllowText = x.AllowText,
            AppointmentDate = x.AppointmentDate,
            BestContactTime = x.BestContactTime,
            BirthDate = x.BirthDate,
            BuildingType = x.BuildingType,
            CCMEnrolledBy = x.CCMEnrolledBy,
            CCMEnrolledOn = x.CCMEnrolledOn,
            CaretakerFirstName = x.CaretakerFirstName,
            CaretakerLastName = x.CaretakerLastName,
            CaretakerPhoneNumber = x.CaretakerPhoneNumber,
            CaretakerRelationship = x.CaretakerRelationship,
            CcmBillingCode = x.CcmBillingCode,
            CcmBillingCode2 = x.CcmBillingCode2,
            CcmClaimSubmissionDate = x.CcmClaimSubmissionDate,
            CcmClinicalSignOffDate = x.CcmClinicalSignOffDate,
            CcmReconciliationDate = x.CcmReconciliationDate,
            CcmStatus = x.CcmStatus,
            City = x.City,
            ContactId = x.ContactId,
            CreatedBy = x.CreatedBy,
            CreatedOn = x.CreatedOn,
            Cycle = x.Cycle,
            DeliveryInstruction = x.DeliveryInstruction,
            DeliveryPermisison = x.DeliveryPermisison,
            DietAndHabitId = x.DietAndHabitId,
            Email = x.Email,
            EmergencyNumber = x.EmergencyNumber,
            EnrollmentNotes = x.EnrollmentNotes,
            EnrollmentStatus = x.EnrollmentStatus,
            FamilyHistoryId = x.FamilyHistoryId,
            FirstName = x.FirstName,
            Gender = x.Gender,
            HomePhoneNumber = x.HomePhoneNumber,
            InsuranceId = x.InsuranceId,
            LastName = x.LastName,
            LiaisonId = x.LiaisonId,
            LiaisonFullName = x.Liaison.FirstName ?? "" + " " + x.Liaison.FirstName ?? "",
            LifeStressId = x.LifeStressId,
            MedicaidIdNumber = x.MedicaidIdNumber,
            MedicalConditionId = x.MedicalConditionId,
            MedicalStatusId = x.MedicalStatusId,
            MedicareIdNumber = x.MedicareIdNumber,
            MedicationOtcId = x.MedicationOtcId,
            MiddleName = x.MiddleName,
            MobilePhoneNumber = x.MobilePhoneNumber,
            Notes = x.Notes,
            NutritionalSupplementId = x.NutritionalSupplementId,
            OtherInsuranceIdNumber = x.OtherInsuranceIdNumber,
            OtherLanguage = x.OtherLanguage,
            PatientChronicCondition1Id = x.PatientChronicCondition1Id,
            PatientChronicCondition2Id = x.PatientChronicCondition2Id,
            PhysicianId = x.PhysicianId,
            PhysicianFullName = x.Physician.FirstName ?? "" + " " + x.Physician.LastName ?? "",
            PreferredLanguage = x.PreferredLanguage,
            Prefix = x.Prefix,
            ProfileId = x.ProfileId,
            State = x.State,
            Suffix = x.Suffix,
            UpdatedBy = x.UpdatedBy,
            UpdatedOn = x.UpdatedOn,
            UrgencyContactId = x.UrgencyContactId,
            WorkAndRelationshipId = x.WorkAndRelationshipId,
            WorkPhoneNumber = x.WorkPhoneNumber,
            Zipcode = x.Zipcode
        };
    }

    public class DoctorVisitViewModel
    {
        public int Id { get; set; }
        public int PatientId { get; set; }

        [Required]
        [Display(Name = "Visit Date")]
        public DateTime VisitDate { get; set; }

        [Display(Name = "Chief Complaint")]
        public string VisitReason { get; set; }

        [Display(Name = "Next Appointment")]
        public DateTime NextAppointment { get; set; }

        [Display(Name = "Email")]
        public string Email { get; set; }

        [Display(Name = "Additional Providers")]
        public string AdditionalProviders { get; set; }

    }
    public class DoctorVisit
    {
        public int Id { get; set; }
        public int PatientId { get; set; }

        [Required]
        [Display(Name = "Visit Date")]
        public DateTime VisitDate { get; set; }

        [Display(Name = "Chief Complaint")]
        public string VisitReason { get; set; }

        [Display(Name = "Next Appointment")]
        public DateTime NextAppointment { get; set; }

        [Display(Name = "Email")]
        public string Email { get; set; }

        [Display(Name = "Additional Providers")]
        public string AdditionalProviders { get; set; }
    }

    public class SecondaryDoctor
    {
        public int Id { get; set; }
        public int PatientId { get; set; }

        [Required]
        public string FullName { get; set; }
        public string Speciality { get; set; }

        [Display(Name = "Last Visited (mm/dd/yyyy)")]
        public string LastVisit { get; set; }

        [Display(Name = "Next Appointment (mm/dd/yyyy)")]
        public string NextAppointment { get; set; }

        public string DoctorType { get; set; }
        [Display(Name = "CCM Provider")]
        public bool isCCMProvider { get; set; }
        [Display(Name = "Phone Number")]
        
        public string MobilePhoneNumber { get; set; }

        [Display(Name = "Email")]
        public string Email { get; set; }

        [Display(Name = "Share Care Plan")]
        public bool? IsShareCarePlan { get; set; }

        [Display(Name = "NPI")]
        public int? NPI { get; set; }

        [Required]
        [Display(Name = "Main Phone Number")]
        public string MainPhoneNumber { get; set; }

        [Display(Name = "Address Line 1")]
        [Column(TypeName = "VARCHAR")]
        [StringLength(100)]
        public string Address1 { get; set; }

        [Display(Name = "Address Line 2")]
        [Column(TypeName = "VARCHAR")]
        [StringLength(100)]
        public string Address2 { get; set; }
        public Nullable< bool> Status { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string CreatedBy { get; set; }

        public DateTime? UpdatedOn { get; set; }
        public string UpdatedBy { get; set; }

    }

    public class CcdaLabResult
    {
        public int Id { get; set; }
        public int PatientId { get; set; }

        public string Name { get; set; }
        public string TestValue { get; set; }
        public DateTime? Date { get; set; }
    }

    public class CcdaVital
    {
        public int Id { get; set; }
        public int PatientId { get; set; }

        public string Date { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
        public string Unit { get; set; }
    }

    public class CcdaProcedure
    {
        public int Id { get; set; }
        public int PatientId { get; set; }

        public string Name { get; set; }
        public string Date { get; set; }
    }

    public class CcdaProblem
    {
        public int Id { get; set; }
        public int PatientId { get; set; }

        public string Name { get; set; }
        public string Status { get; set; }
    }

    public class CcdaAllergy
    {
        public int Id { get; set; }
        public int PatientId { get; set; }

        public string StartDate { get; set; }
        public string Name { get; set; }
        public string Severity { get; set; }
    }


    public class Patient_ChronicCondition1
    {
        public int Id { get; set; }

        [Column(TypeName = "VARCHAR")]
        [StringLength(100)]
        public string ChronicCondition1Type { get; set; }
    }

    public class Patient_ChronicCondition2
    {
        public int Id { get; set; }

        [Column(TypeName = "VARCHAR")]
        [StringLength(100)]
        public string ChronicCondition2Type { get; set; }
    }

    public class EnrollmentStatus
    {
        public int Id { get; set; }
        [Column(TypeName = "VARCHAR")]
        [StringLength(100)]
        public string Name { get; set; }

        public int OrderBy { get; set; }

    }
    public class EnrollmentSubStatus
    {
        public int Id { get; set; }
        [Column(TypeName = "VARCHAR")]
        [StringLength(100)]
        public string Name { get; set; }
        [ForeignKey("EnrollmentStatus")]
        public int EnrollmentStatusID { get; set; }

        public int OrderBy { get; set; }
        public virtual EnrollmentStatus EnrollmentStatus { get; set; }

    }

    public class EnrollmentSubstatusReason
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int Id { get; set; }
        [Column(TypeName = "VARCHAR")]
        [StringLength(100)]
        public string Name { get; set; }
        //[ForeignKey("BillingCategoryId")]
        public int? BillingCategoryId { get; set; }
        
        public  virtual BillingCategory BillingCategory { get; set; }

    }

    public class MedicationDoseRepetitionTime
    {
        public int Id { get; set; }
        [Column (TypeName = "VARCHAR")]
        [StringLength(100)]
        public string DoseRepetitionTime { get; set; }
    }

    public class MedicationRoute
    {
        public int Id { get; set; }
        [Column(TypeName = "VARCHAR")]
        [StringLength(100)]
        public string RouteItme { get; set; }
    }

    
    
}