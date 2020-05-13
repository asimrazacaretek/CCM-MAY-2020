using CCM.Models.CCMBILLINGS;
using CCM.Models.DataModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace CCM.Models
{
    public class PatientProfile
    {
        public int Id { get; set; }
        public int Cycle { get; set; }
        public int PatientId { get; set; }

        [Column(TypeName = "VARCHAR")]
        [StringLength(50)]
        [Display(Name = "Prefix")]
        public string Prefix { get; set; }

        [Column(TypeName = "VARCHAR")]
        [StringLength(100)]
        [Display(Name = "First Name*")]
        public string FirstName { get; set; }

        [Column(TypeName = "VARCHAR")]
        [StringLength(100)]
        [Display(Name = "Middle Name")]
        public string MiddleName { get; set; }
        
        [Column(TypeName = "VARCHAR")]
        [StringLength(100)]
        [Display(Name = "Last Name*")]
        public string LastName { get; set; }

        [Column(TypeName = "VARCHAR")]
        [StringLength(50)]
        [Display(Name = "Suffix")]
        public string Suffix { get; set; }

        [Display(Name = "Date Of Birth* (MM/DD/YYYY)")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? BirthDate { get; set; }

        [Column(TypeName = "VARCHAR")]
        [StringLength(50)]
        [Display(Name = "Gender")]
        public string Gender { get; set; }

        [Column(TypeName = "VARCHAR")]
        [StringLength(50)]
        [Display(Name = "Preferred Language")]
        public string PreferredLanguage { get; set; }

        [Column(TypeName = "VARCHAR")]
        [StringLength(100)]
        [Display(Name = "Please, specify other preferred language")]
        public string OtherLanguage { get; set; }
    }

    public class PatientProfile_Contact
    {
        public int Id { get; set; }
        public int Cycle { get; set; }
        public int PatientId { get; set; }

        [Display(Name = "Cell Phone Number 1*")]
        [RegularExpression(@"^([0-9]{10})$", ErrorMessage = "Invalid Cell Phone Number 1.")]
        [Range(1000000000, 9999999999, ErrorMessage = "Invalid Cell Phone Number 1")]
        public string CellPhoneNumber { get; set; }
        [Display(Name = "Cell Phone Number 2")]
        [RegularExpression(@"^([0-9]{10})$", ErrorMessage = "Invalid Cell Phone Number 2.")]
        [Range(1000000000, 9999999999, ErrorMessage = "Invalid Cell Phone Number 2")]
        [Column(TypeName = "VARCHAR")]
        [StringLength(15)]
        public string CellPhoneNumber1 { get; set; }

        [Display(Name = "Cell Phone Number 3")]
        [RegularExpression(@"^([0-9]{10})$", ErrorMessage = "Invalid Cell Phone Number 3.")]
        [Range(1000000000, 9999999999, ErrorMessage = "Invalid Cell Phone Number 3")]
        [Column(TypeName = "VARCHAR")]
        [StringLength(15)]
        public string CellPhoneNumber2 { get; set; }

        [Display(Name = "Can We Call This Number?*")]
        public bool CellPhonePermission { get; set; }

        [Display(Name = "Best Way to Contact")]
        public bool? CellPhoneContactWay { get; set; } 
        // call = true, text = false, null = not selected

        [Display(Name = "Best Time to Call")]
        public bool? CellPhoneCallTime { get; set; } 
        // Daytime = true, Evening = false

        [Display(Name = "Home Phone Number 1")]
        [RegularExpression(@"^([0-9]{10})$", ErrorMessage = "Home Phone Number 1.")]
        [Range(1000000000, 9999999999, ErrorMessage = "Home Phone Number 1")]
        public string HomePhoneNumber { get; set; }

        [Display(Name = "Home Phone Number 2")]
        [RegularExpression(@"^([0-9]{10})$", ErrorMessage = "Home Phone Number 2.")]
        [Range(1000000000, 9999999999, ErrorMessage = "Home Phone Number 2")]
        [Column(TypeName = "VARCHAR")]
        [StringLength(15)]
        public string HomePhoneNumber1 { get; set; }

        [Display(Name = "Home Phone Number 3")]
        [RegularExpression(@"^([0-9]{10})$", ErrorMessage = "Home Phone Number 3.")]
        [Range(1000000000, 9999999999, ErrorMessage = "Home Phone Number 3")]
        [Column(TypeName = "VARCHAR")]
        [StringLength(15)]
        public string HomePhoneNumber2 { get; set; }

        [Display(Name = "Can We Call This Number?")]
        public bool HomePhonePermission { get; set; }

        [Display(Name = "Best Time to Call")]
        public bool? HomePhoneCallTime { get; set; }

        [Display(Name = "Work Phone Number 1")]
        [RegularExpression(@"^([0-9]{10})$", ErrorMessage = "Work Phone Number 1.")]
        [Range(1000000000, 9999999999, ErrorMessage = "Work Phone Number 1")]
       
        public string WorkPhoneNumber { get; set; }

        [Display(Name = "Work Phone Number 2")]
        [RegularExpression(@"^([0-9]{10})$", ErrorMessage = "Work Phone Number 2.")]
        [Range(1000000000, 9999999999, ErrorMessage = "Work Phone Number 2")]
        [Column(TypeName = "VARCHAR")]
        [StringLength(15)]
        public string WorkPhoneNumber1 { get; set; }

        [Display(Name = "Work Phone Number 3")]
        [RegularExpression(@"^([0-9]{10})$", ErrorMessage = "Work Phone Number 3.")]
        [Range(1000000000, 9999999999, ErrorMessage = "Work Phone Number 3")]
        [Column(TypeName = "VARCHAR")]
        [StringLength(15)]
        public string WorkPhoneNumber2 { get; set; }
        [Display(Name = "Emergency Contact Number 1")]
        [RegularExpression(@"^([0-9]{10})$", ErrorMessage = "Emergency Contact Number 1.")]
        [Range(1000000000, 9999999999, ErrorMessage = "Emergency Contact Number 1")]
        [Column(TypeName = "VARCHAR")]
        [StringLength(15)]
        public string EmergencyNumber { get; set; }
        [Display(Name = "Emergency Contact Number 2")]
        [RegularExpression(@"^([0-9]{10})$", ErrorMessage = "Emergency Contact Number 2.")]
        [Range(1000000000, 9999999999, ErrorMessage = "Emergency Contact Number 2")]
        [Column(TypeName = "VARCHAR")]
        [StringLength(15)]
        public string EmergencyNumber1 { get; set; }

        [Display(Name = "Emergency Contact Number 3")]
        [RegularExpression(@"^([0-9]{10})$", ErrorMessage = "Emergency Contact Number 2.")]
        [Range(1000000000, 9999999999, ErrorMessage = "Emergency Contact Number 2")]
        [Column(TypeName = "VARCHAR")]
        [StringLength(15)]
        public string EmergencyNumber2 { get; set; }

        [Display(Name = "Can We Call This Number?")]
        public bool WorkPhonePermission { get; set; }

        [Display(Name = "Best Time to Call")]
        public bool? WorkPhoneCallTime { get; set; }

        [Display(Name = "Personal Email Address*")]
        [EmailAddress]
        public string Email { get; set; }

        [Display(Name = "Can We Email You?*")]
        public bool EmailPermission { get; set; }

       
    }

    public class PatientProfile_Address
    {
        public int Id { get; set; }
        public int Cycle { get; set; }
        public int PatientId { get; set; }

        [Required]
        [Display(Name = "Address Line 1*")]
        public string Address1 { get; set; }

        [Display(Name = "Address Line 2")]
        public string Address2 { get; set; }

        [Required]
        [Display(Name = "City*")]
        public string City { get; set; }

        [Required]
        [Display(Name = "State*")]
        public string State { get; set; }

        [Required]
        [Display(Name = "Zip*")]
        public string Zip { get; set; }

        [Display(Name = "Building Type")]
        public string BuildingType { get; set; }

        [Display(Name = "Delivery Permission")]
        public bool? DeliveryPermisison { get; set; }

        [Display(Name = "Delivery Instruction")]
        public string DeliveryInstruction { get; set; }
    }

    public class PatientProfile_UrgencyContact
    {
        public int Id { get; set; }
        public int Cycle { get; set; }
        public int PatientId { get; set; }

        [Display(Name = "Name*")]
        public string PrimaryName { get; set; }

        [Display(Name = "Relationship*")]
        public string PrimaryRelationship { get; set; }

        [Display(Name = "Cell Phone Number 1*")]
        [RegularExpression(@"^([0-9]{10})$", ErrorMessage = "Invalid Cell Phone Number 1.")]
        [Range(1000000000, 9999999999, ErrorMessage = "Invalid Cell Phone Number 1")]
        public string PrimaryMobilePhoneNumber { get; set; }
        [Display(Name = "Cell Phone Number 2")]
        [RegularExpression(@"^([0-9]{10})$", ErrorMessage = "Invalid Cell Phone Number 2.")]
        [Range(1000000000, 9999999999, ErrorMessage = "Invalid Cell Phone Number 2")]
        [Column(TypeName = "VARCHAR")]
        [StringLength(15)]
        public string PrimaryMobilePhoneNumber1 { get; set; }
        [Display(Name = "Cell Phone Number 3")]
        [RegularExpression(@"^([0-9]{10})$", ErrorMessage = "Invalid Cell Phone Number 3.")]
        [Range(1000000000, 9999999999, ErrorMessage = "Invalid Cell Phone Number 3")]
        [Column(TypeName = "VARCHAR")]
        [StringLength(15)]
        public string PrimaryMobilePhoneNumber2 { get; set; }
        [Display(Name = "Home Phone Number")]
        [RegularExpression(@"^([0-9]{10})$", ErrorMessage = "Invalid Home Phone Number.")]
        [Range(1000000000, 9999999999, ErrorMessage = "Invalid Home Phone Number")]
        public string PrimaryHomePhoneNumber { get; set; }

        [Display(Name = "Work Phone Number")]
        [RegularExpression(@"^([0-9]{10})$", ErrorMessage = "Invalid Work Phone Number.")]
        [Range(1000000000, 9999999999, ErrorMessage = "Invalid Work Phone Number")]
        [Column(TypeName = "VARCHAR")]
        [StringLength(15)]
        public string PrimaryHomePhoneNumber1 { get; set; }

        [Display(Name = "Home Phone Number 3")]
        [RegularExpression(@"^([0-9]{10})$", ErrorMessage = "Invalid Home Phone Number 3.")]
        [Range(1000000000, 9999999999, ErrorMessage = "Invalid Home Phone Number 3")]
        [Column(TypeName = "VARCHAR")]
        [StringLength(15)]
        public string PrimaryHomePhoneNumber2 { get; set; }

        [Display(Name= "Provider Type")]
        public string PrimaryProfesionalCareProvider { get; set; }
        [Display(Name = "Expertise / Skillset")]
        public string PrimaryExpertise { get; set; }
        [Display(Name= "Health Proxy and Carplane")]
        public bool PrimaryHealthProxyAndCarplane { get; set; }
        [Display(Name= "Diesease State")]
        public string PrimaryDieseaseState { get; set; }
        [Display(Name = "Email")]
        public string PrimaryEmail { get; set; }
        [Display(Name = "Share Care Plan")]
        public bool? PrimaryIsShareCarePlan { get; set; }

        [Display(Name = "Name")]
        public string SecondaryName { get; set; }

        [Display(Name = "Relationship")]
        public string SecondaryRelationship { get; set; }

        [Display(Name = "Cell Phone Number 1")]
        [RegularExpression(@"^([0-9]{10})$", ErrorMessage = "Invalid Cell Phone Number 1.")]
        [Range(1000000000, 9999999999, ErrorMessage = "Invalid Cell Phone Number 1")]
        public string SecondaryMobilePhoneNumber { get; set; }
        [Display(Name = "Cell Phone Number 2")]
        [RegularExpression(@"^([0-9]{10})$", ErrorMessage = "Invalid Cell Phone Number 2.")]
        [Range(1000000000, 9999999999, ErrorMessage = "Invalid Cell Phone Number 2")]
        [Column(TypeName = "VARCHAR")]
        [StringLength(15)]
        public string SecondaryMobilePhoneNumber1 { get; set; }

        [Display(Name = "Cell Phone Number 3")]
        [RegularExpression(@"^([0-9]{10})$", ErrorMessage = "Invalid Cell Phone Number 3.")]
        [Range(1000000000, 9999999999, ErrorMessage = "Invalid Cell Phone Number 3")]
        [Column(TypeName = "VARCHAR")]
        [StringLength(15)]
        public string SecondaryMobilePhoneNumber2 { get; set; }

        [Display(Name = "Home Phone Number 1")]
        [RegularExpression(@"^([0-9]{10})$", ErrorMessage = "Invalid Home Phone Number 1.")]
        [Range(1000000000, 9999999999, ErrorMessage = "Invalid Cell Phone Number 1")]
        public string SecondaryHomePhoneNumber { get; set; }

        [Display(Name = "Home Phone Number 2")]
        [RegularExpression(@"^([0-9]{10})$", ErrorMessage = "Invalid Home Phone Number 2.")]
        [Range(1000000000, 9999999999, ErrorMessage = "Invalid Home Phone Number 2")]
        [Column(TypeName = "VARCHAR")]
        [StringLength(15)]
        public string SecondaryHomePhoneNumber1 { get; set; }

        [Display(Name = "Home Phone Number 3")]
        [RegularExpression(@"^([0-9]{10})$", ErrorMessage = "Invalid Home Phone Number 3.")]
        [Range(1000000000, 9999999999, ErrorMessage = "Invalid Home Phone Number 3")]
        [Column(TypeName = "VARCHAR")]
        [StringLength(15)]
        public string SecondaryHomePhoneNumber2 { get; set; }


        [Display(Name = "Profesional Care Provider or Family Member")]
        public string SecondaryProfesionalCareProvider { get; set; }
        [Display(Name = "Expertise / Skillset")]
        public string SecondaryExpertise { get; set; }
        [Display(Name = "Proxy and Carplane")]
        public bool SecondaryHealthProxyAndCarplane { get; set; }
        [Display(Name = "Diesease State")]
        public string SecondaryDieseaseState { get; set; }
        [Display(Name = "Email")]
        public string SecondaryEmail { get; set; }
        [Display(Name = "Share Care Plan")]
        public bool? SecondaryIsShareCarePlan { get; set; } = false;

        public string ContactType { get; set; }
    }
    
    public class PatientProfile_Insurance
    {
        public int Id { get; set; }
        public int Cycle { get; set; }
        public int PatientId { get; set; }

        [Display(Name = "Name")]
        public string PrimaryName { get; set; }

        [Display(Name = "Plan Name")]
        public string PrimaryPlanName { get; set; }

        [Display(Name = "Insurance ID Number")]
        public string PrimaryIdNumber { get; set; }

        [Display(Name = "Group Number")]
        public string PrimaryGroupNumber { get; set; }

        [Display(Name = "Plan Code")]
        public string PrimaryPlanCode { get; set; }

        [Display(Name = "Rx: Bin")]
        public string PrimaryRxBin { get; set; }

        [Display(Name = "Rx: PCN")]
        public string PrimaryRxPcn { get; set; }

        [Display(Name = "Rx: Group")]
        public string PrimaryRxGroup { get; set; }

        [Display(Name = "Relation To Insured")]
        public string PrimaryRelation { get; set; }

        [Display(Name = "HMO Insurance Name")]
        [Column(TypeName = "VARCHAR")]
        [StringLength(100)]
        public string HMOInsuranceName { get; set; }

        [Display(Name = "HMO Insurance ID")]
        [Column(TypeName = "VARCHAR")]
        [StringLength(100)]
        public string HMOInsuranceID { get; set; }


    }


    public class PatientProfile_ProfesionalCareProvider
    {
        public int Id { get; set; }
        [Column(TypeName = "VARCHAR")]
        [StringLength(100)]
        public string CareProvider { get; set; }
    }

    public class PatientProfile_Consent
    {
        public int Id { get; set; }
        public int PatientId { get; set; }

        [Column(TypeName = "VARCHAR")]
        [StringLength(250)]
        [Display(Name = "Consent")]
        public string Consent { get; set; }

        [Display(Name = "Patient Signature")]
        public byte[] Signature { get; set; }

        [Column(TypeName = "VARCHAR")]
        [StringLength(100)]
        [Display(Name = "Created by*")]
        public string CreatedBy { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Created on")]
        public DateTime CreatedOn { get; set; }

        [Column(TypeName = "VARCHAR")]
        [StringLength(100)]
        [Display(Name = "Updated by")]
        public string UpdatedBy { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Updated on")]
        public DateTime UpdatedOn { get; set; }
        public string Note { get; set; }

        public string filePath { get; set; }

        public string fileName { get; set; }

        [ForeignKey("BillingCategory")]
        public int? BillingCategoryId { get; set; }
        public BillingCategory BillingCategory { get; set; }
    }

    public class PatientProfile_ConsentTemplate
    {
        public int Id { get; set; }

        [Column(TypeName = "VARCHAR")]
        [StringLength(250)]
        [Display(Name = "Consent Template")]
        public string ConsentTemplate { get; set; }

        [Column(TypeName = "VARCHAR")]
        [StringLength(100)]
        [Display(Name = "Type")]
        public string Type { get; set; }

    }

    public class PatientProfile_DieseaseState
    {
        public int Id { get; set; }
        [Column(TypeName = "VARCHAR")]
        [StringLength(100)]
        public string DieseaseStateType { get; set; }
    }

    //Hospitals 
    public class Hospitals
    {
        public int Id { get; set; }

        [Column(TypeName = "VARCHAR")]
        [StringLength(250)]
        [Display(Name = "Hospital Name")]
        public string HospitalName { get; set; }

        [Column(TypeName = "VARCHAR")]
        [StringLength(100)]
        [Display(Name = "City")]
        public string City { get; set; }

        [Column(TypeName = "VARCHAR")]
        [StringLength(50)]
        [Display(Name = "Country")]
        public string Country { get; set; }

        [Column(TypeName = "VARCHAR")]
        [StringLength(50)]
        [Display(Name = "State")]
        public string State { get; set; }
    }
    public class PatientProfile_HospitalDetails
    {
        public int Id { get; set; }
        public int PatientId { get; set; }

        public int HospitalsId { get; set; }

        [Column(TypeName = "VARCHAR")]
        [StringLength(250)]
        [Display(Name = "Hospital Name")]
        public string HospitalName { get; set; }

        [Column(TypeName = "VARCHAR")]
        [StringLength(250)]
        [Display(Name = "City")]
        public string City { get; set; }

        [Column(TypeName = "VARCHAR")]
        [StringLength(50)]
        [Display(Name = "Country")]
        public string Country { get; set; }

        [Column(TypeName = "VARCHAR")]
        [StringLength(50)]
        [Display(Name = "State")]
        public string State { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Date of Admission")]
        public DateTime AdmitDate { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Date of Discharge")]
        public DateTime DischargeDate { get; set; }

        public int Rate { get; set; }

        public int TotalDays { get; set; }

        [Column(TypeName = "VARCHAR")]
        [StringLength(250)]
        [Display(Name = "Reason For Admission")]
        public string Reason { get; set; }

        [Column(TypeName = "VARCHAR")]
        [StringLength(250)]
        [Display(Name = "Department")]
        public string Department { get; set; }

        public int? DepartmentId { get; set; }

        [Column(TypeName = "VARCHAR")]
        [StringLength(250)]
        [Display(Name = "ICD-10-Codes")]
        public string ICD10Codes { get; set; }

        [Column(TypeName = "VARCHAR")]
        [StringLength(250)] 
        [Display(Name = "Full Name")]
        public string FullName { get; set; }

        [Column(TypeName = "VARCHAR")]
        [StringLength(100)]
        [Display(Name = "NPI")]
        public string NPI { get; set; }

        [Column(TypeName = "VARCHAR")]
        [StringLength(50)]
        [Display(Name = "Phone")]
        public string Phone { get; set; }

        [Column(TypeName = "VARCHAR")]
        [StringLength(50)]
        [Display(Name = "Type Of Stay")]
        public string StayType { get; set; }


        [Column(TypeName = "VARCHAR")]
        [StringLength(100)]
        [Display(Name = "Created by*")]
        public string CreatedBy { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Created on")]
        public DateTime CreatedOn { get; set; }

        [Column(TypeName = "VARCHAR")]
        [StringLength(100)]
        [Display(Name = "Updated by")]
        public string UpdatedBy { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Updated on")]
        public DateTime UpdatedOn { get; set; }

        public Patient Patient { get; set; }
        public Hospitals Hospitals { get; set; }
    }
    public class Department
    {
        public int Id { get; set; }
        public string DepartmentName { get; set; }
    }

    
    ///////New Model for Live Search Of Hospitals
    public class PatientProfile_Hospitalvisits
    {
        public int Id { get; set; }
        public int PatientId { get; set; }

        [Column(TypeName = "VARCHAR")]
        [StringLength(250)]
        [Display(Name = "Hospital Name")]
        public string HospitalName { get; set; }

        [Column(TypeName = "VARCHAR")]
        [StringLength(250)]
        [Display(Name = "HNPI")]
        public string HNPI { get; set; }

        [Column(TypeName = "VARCHAR")]
        [StringLength(250)]
        [Display(Name = "HType")]
        public string HType { get; set; }

        [Column(TypeName = "VARCHAR")]
        [StringLength(250)]
        [Display(Name = "HAddress")]
        public string HAddress { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Date of Admission")]
        public DateTime AdmitDate { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Date of Discharge")]
        public DateTime DischargeDate { get; set; }

        public int Rate { get; set; }

        public int TotalDays { get; set; }

        [Column(TypeName = "VARCHAR")]
        [StringLength(250)]
        [Display(Name = "Reason For Admission")]
        public string Reason { get; set; }

        [Column(TypeName = "VARCHAR")]
        [StringLength(250)]
        [Display(Name = "Department")]
        public string Department { get; set; }

        [Column(TypeName = "VARCHAR")]
        
        [Display(Name = "ICD-10-Codes")]
        public string ICD10Codes { get; set; }

        [Column(TypeName = "VARCHAR")]
        [StringLength(250)]
        [Display(Name = "Full Name")]
        public string FullName { get; set; }

        [Column(TypeName = "VARCHAR")]
        [StringLength(100)]
        [Display(Name = "NPI")]
        public string NPI { get; set; }

        [Column(TypeName = "VARCHAR")]
        [StringLength(50)]
        [Display(Name = "Phone")]
        public string Phone { get; set; }

        [Column(TypeName = "VARCHAR")]
        [StringLength(50)]
        [Display(Name = "Type Of Stay")]
        public string StayType { get; set; }


        [Column(TypeName = "VARCHAR")]
        [StringLength(100)]
        [Display(Name = "Created by*")]
        public string CreatedBy { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Created on")]
        public DateTime CreatedOn { get; set; }

        [Column(TypeName = "VARCHAR")]
        [StringLength(100)]
        [Display(Name = "Updated by")]
        public string UpdatedBy { get; set; }
        public string OtherProcedure { get; set; }


        [DataType(DataType.Date)]
        [Display(Name = "Updated on")]
        public DateTime UpdatedOn { get; set; }

        public Patient Patient { get; set; }


        [ForeignKey("HospitalReasons")]
        public int? HospitalReasonsId { get; set; }

        public virtual HospitalReasons HospitalReasons { get; set; }

        [ForeignKey("HospitalDepartments")]
        public int? HospitalDepartmentsId { get; set; }

        public virtual HospitalDepartments HospitalDepartments { get; set; }

        [ForeignKey("HospitalProcedures")]
        public int? HospitalProceduresId { get; set; }

        public virtual HospitalProcedures HospitalProcedures { get; set; }





    }


    public class Patient_Images
    {
        public int Id { get; set; }
        public int PatientId { get; set; }

        [Column(TypeName = "VARCHAR")]
        public string FileName { get; set; }
        [Column(TypeName = "VARCHAR")]
        public string FilePath { get; set; }
        [Column(TypeName = "VARCHAR")]
        public string ImgType { get; set; }
        public string MimeType { get; set; }

        public bool IsDelete { get; set; }

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
}