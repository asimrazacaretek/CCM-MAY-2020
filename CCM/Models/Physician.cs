using CCM.Models.CCMBILLINGS;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace CCM.Models
{
    public class LiaisonGroup
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Group Name")]
        [Column(TypeName = "VARCHAR")]
        [StringLength(100)]
        public string GroupName { get; set; }

        [Required]
        [Display(Name = "Main Phone Number")]
        public string MainPhoneNumber { get; set; }

        [Required]
        [EmailAddress]
        [Column(TypeName = "VARCHAR")]
        [StringLength(100)]
        public string Email { get; set; }

       
    }
    public class LiaisonGroup_Liaison_Mapping
    {
        public int Id { get; set; }
        public int LiaisonId { get; set; }
        public int LiaisonGroupId { get; set; }
        [DataType(DataType.Date)]
        public DateTime? CreatedOn { get; set; }

        [Column(TypeName = "VARCHAR")]
        [StringLength(100)]
        public string CreatedBy { get; set; }

        [DataType(DataType.Date)]
        public DateTime? UpdatedOn { get; set; }

        [Column(TypeName = "VARCHAR")]
        [StringLength(100)]
        public string UpdatedBy { get; set; }

        public virtual Liaison Liaison { get; set; }
        public virtual LiaisonGroup LiaisonGroup { get; set; }


    }
    public class PhysiciansGroup
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Group Name")]
        [Column(TypeName = "VARCHAR")]
        [StringLength(100)]
        public string GroupName { get; set; }

        [Required]
        [Display(Name = "Main Phone Number")]
        public string MainPhoneNumber { get; set; }

        [Required]
        [EmailAddress]
        [Column(TypeName = "VARCHAR")]
        [StringLength(100)]
        public string Email { get; set; }

        [Required]
        [Display(Name = "NPI Number")]
        public int? NPI { get; set; }
    }
    
         public class PhysicianGroup_SalesStaff_Mapping
    {
        public int Id { get; set; }
        public int SaleStaffId { get; set; }
        public int PhysiciansGroupId { get; set; }
        [DataType(DataType.Date)]
        public DateTime? CreatedOn { get; set; }

        [Column(TypeName = "VARCHAR")]
        [StringLength(100)]
        public string CreatedBy { get; set; }

        [DataType(DataType.Date)]
        public DateTime? UpdatedOn { get; set; }

        [Column(TypeName = "VARCHAR")]
        [StringLength(100)]
        public string UpdatedBy { get; set; }

        public virtual SaleStaff SaleStaff { get; set; }
        public virtual PhysiciansGroup PhysiciansGroup { get; set; }


    }
    public class PhysicianGroup_Physician_Mapping
    {
        public int Id { get; set; }
        public int PhysicianId { get; set; }
        public int PhysiciansGroupId { get; set; }
        [DataType(DataType.Date)]
        public DateTime? CreatedOn { get; set; }

        [Column(TypeName = "VARCHAR")]
        [StringLength(100)]
        public string CreatedBy { get; set; }

        [DataType(DataType.Date)]
        public DateTime? UpdatedOn { get; set; }

        [Column(TypeName = "VARCHAR")]
        [StringLength(100)]
        public string UpdatedBy { get; set; }

        public virtual Physician Physician { get; set; }
        public virtual  PhysiciansGroup PhysiciansGroup { get; set; }


    }

    public class Physician
    {
        public int Id { get; set; }

        public string ContactId { get; set; }
        public string State { get; set; }
        public string SubSpecialties { get; set; }
       
        public string Specialty { get; set; }
        public string Fax { get; set; }

        [DataType(DataType.Date)]
        public DateTime? CreatedOn { get; set; }

        [Column(TypeName = "VARCHAR")]
        [StringLength(100)]
        public string CreatedBy { get; set; }

        [DataType(DataType.Date)]
        public DateTime? UpdatedOn { get; set; }

        [Column(TypeName = "VARCHAR")]
        [StringLength(100)]
        public string UpdatedBy { get; set; }

        [Required]
        [Display(Name = "First Name")]
        [Column(TypeName = "VARCHAR")]
        [StringLength(100)]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Last Name")]
        [Column(TypeName = "VARCHAR")]
        [StringLength(100)]
        public string LastName { get; set; }

        [Display(Name = "Address Line 1")]
        [Column(TypeName = "VARCHAR")]
        [StringLength(100)]
        public string Address1 { get; set; }

        [Display(Name = "Address Line 2")]
        [Column(TypeName = "VARCHAR")]
        [StringLength(100)]
        public string Address2 { get; set; }

        [Display(Name = "City")]
        [Column(TypeName = "VARCHAR")]
        [StringLength(100)]
        public string City { get; set; }

        [Display(Name = "Zipcode")]
        [DataType(DataType.PostalCode)]
        public string Zipcode { get; set; }

        [Required]
        [Display(Name = "Main Phone Number")]
        public string MainPhoneNumber { get; set; }

        [Display(Name = "Mobile Phone Number")]
        public string MobilePhoneNumber { get; set; }

        [Required]
        [EmailAddress]
        [Column(TypeName = "VARCHAR")]
        [StringLength(100)]
        public string Email { get; set; }

        [Display(Name = "NPI")]
        public int? NPI { get; set; }
        public bool isActive { get; set; } = true;
        [Display(Name = "Upload Photo")]
        public byte[] Photo { get; set; }
        public virtual List<Physician_CPTRates> PhysicianCPTRates { get; set; }
    }
    public class Physician_CPTRates
    {
        public int Id { get; set; }
        [DataType(DataType.Date)]
        public DateTime? CreatedOn { get; set; }

        [Column(TypeName = "VARCHAR")]
        [StringLength(100)]
        public string CreatedBy { get; set; }

        [DataType(DataType.Date)]
        public DateTime? UpdatedOn { get; set; }

        [Column(TypeName = "VARCHAR")]
        [StringLength(100)]
        public string UpdatedBy { get; set; }

        public int? PhysicianId { get; set; }
        public virtual Physician Physician { get; set; }
        [Column(TypeName = "VARCHAR")]
        [StringLength(100)]
        public string BillingCode { get; set; }

        public decimal? InvoiceRate { get; set; }
        public decimal? BillingRate { get; set; }
        [ForeignKey("BillingCodes")]
        public int? BillingCodeId { get; set; }

        public virtual BillingCodes BillingCodes { get; set; }
        public virtual List<BillingCycle> BillingCycles { get; set; }

    }
    public class PhysicianFinalCarePlanViewModel
    {
        public int PatientId { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string PhysicianName { get; set; }
        public string DOB { get; set; }

        public int? PhyscianID { get; set; }

        public string CarePlanShared { get; set; }

        public List<BillingCycle> BillingCycles { get; set; }
        public List<FinalCarePlanNotes> finalCarePlanNotes { get; set; }
       



    }
    public class CycleNameandLink
    {
        public string CycleName { get; set; }
        public string FinalCarePlanlink { get; set; }
    }
}