using CCM.Models.CCMBILLINGS;
using CCM.Models.DataModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace CCM.Models
{
    public class LiaisonSchedulelist
    {
      public string   LiaisonName { get; set; }
        public string Appointmentlst { get; set; }

        public string firstslot { get; set; }
        public string secondslot { get; set; }
        public string secondslot1 { get; set; }
        public string thirdslot { get; set; }
        public string fourthslot { get; set; }
        public string fourthslot1 { get; set; }
        public string fifthslot { get; set; }
        public string sixslot { get; set; }
        public string sixslot1 { get; set; }
        public string seventslot { get; set; }
        public string eightslot { get; set; }
        public string eightslot1 { get; set; }
        public string ninthslot { get; set; }
        public string tenthslot { get; set; }
        public string tenthslot1 { get; set; }
        public string eleventhslot { get; set; }
        public string twelvthslot { get; set; }
        public string twelvthslot1 { get; set; }
        public string thirteenthslot { get; set; }
        public string fourteenthslot { get; set; }
        public string fourteenthslot1 { get; set; }
        public string fifteenslot { get; set; }
        public string sixteenthslot { get; set; }
        public string sixteenthslot1 { get; set; }
        public string seventeenthslot { get; set; }
        public string eigtheenthslot { get; set; }
        public string eigtheenthslot1 { get; set; }
        public string ninteenthslot { get; set; }
        public string twenteethslot { get; set; }
        public string twenteethslot1 { get; set; }
        public string tweentyoneslot { get; set; }
        public string tweentysecondslot { get; set; }
        public string tweentysecondslot1 { get; set; }
        public string tweentythirdslot { get; set; }
        public string tweentyfourththslot { get; set; }
        public string tweentyfourththslot1 { get; set; }

        public string tweeentyfifthslot { get; set; }
        public string tweentysixslot { get; set; }
        public string tweentysixslot1 { get; set; }
        public string tweentysevenslot { get; set; }
        public string tweentyeightslot { get; set; }
        public string tweentyeightslot1 { get; set; }
        public string tweentynineslot { get; set; }
        public string thirtyslot { get; set; }
        public string thirtyslot1 { get; set; }
        public string thirtyoneslot { get; set; }
        public string thirtytwothslot { get; set; }
        public string thirtytwothslot1 { get; set; }

    }
    public class PatientsViewModelAppointment
    {
        public string Description { get; set; }
        public int Id { get; set; }
    }
    public  class Liaison
    {
        public int Id { get; set; }

        [Column(TypeName = "VARCHAR")]
        [StringLength(50)]
        public string UserId { get; set; }

        [Display(Name = "Date Entered")]
        [DataType(DataType.Date)]
        public DateTime? CreatedOn { get; set; } 

        [Display(Name = "Created By")]
        [Column(TypeName = "VARCHAR")]
        [StringLength(100)]
        public string CreatedBy { get; set; }

        [Display(Name = "Date Updated")]
        [DataType(DataType.Date)]
        public DateTime? UpdatedOn { get; set; }

        [Display(Name = "Updated By")]
        [Column(TypeName = "VARCHAR")]
        [StringLength(100)]
        public string UpdatedBy { get; set; }

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

        [Display(Name = "Gender")]
        [Column(TypeName = "VARCHAR")]
        [StringLength(50)]
        public string Gender { get; set; }

        [Required]
        [Display(Name = "Mobile Phone Number")]
        [RegularExpression(@"^([0-9]{10})$", ErrorMessage = "Invalid Phone Number.")]
        [Range(1000000000, 9999999999, ErrorMessage = "Invalid Phone Number")]
        public string MobilePhoneNumber { get; set; }

        [Required]
        [EmailAddress]
        [Column(TypeName = "VARCHAR")]
        [StringLength(100)]
        public string Email { get; set; }

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

        [Display(Name = "State")]
        [Column(TypeName = "VARCHAR")]
        [StringLength(100)]
        public string State { get; set; }

        [Display(Name = "Zipcode")]
        [DataType(DataType.PostalCode)]
        public string Zipcode { get; set; }

        [Required]
        [Display(Name = "Liaison Pay Rate")]
        public decimal PayRate { get; set; }

        [Required]
        [Display(Name = "G0506 Pay Rate")]
        public decimal PayRateG0506 { get; set; }

        [Required]
        [Display(Name = "99490 Pay Rate")]
        public decimal PayRate99490 { get; set; }

        [Required]
        [Display(Name = "99487 Pay Rate")]
        public decimal PayRate99487 { get; set; }

        [Required]
        [Display(Name = "99489 Pay Rate")]
        public decimal PayRate99489 { get; set; }

        [Display(Name = "About Me")]
        [Column(TypeName = "VARCHAR")]
        public string AboutMe { get; set; }

        [Display(Name = "Primary Language")]
        [Column(TypeName = "VARCHAR")]
        [StringLength(100)]
        public string PrimaryLanguage { get; set; }

        [Display(Name = "Secondary Language")]
        [Column(TypeName = "VARCHAR")]
        [StringLength(100)]
        public string SecondaryLanguage { get; set; }

        [Display(Name = "Tertiary Language")]
        [Column(TypeName = "VARCHAR")]
        [StringLength(100)]
        public string TertiaryLanguage { get; set; }

        [Display(Name = "Skill Level")]
        [Column(TypeName = "VARCHAR")]
        [StringLength(100)]
        public string SkillLevel { get; set; }

        [Display(Name = "Upload Photo")]
        public byte[] UserPhoto { get; set; }

        [Display(Name = "Upload Resume")]
        public byte[] Resume { get; set; }

        public bool isActive { get; set; } = true;
        [Column(TypeName = "VARCHAR")]
        [StringLength(100)]
        public string TwilioAccountSID { get; set; }
        [Column(TypeName = "VARCHAR")]
        [StringLength(100)]
        public string TwilioAuthToken { get; set; }
        [Column(TypeName = "VARCHAR")]
        [StringLength(20)]
        public string TwilioCallerId { get; set; }
        [Column(TypeName = "VARCHAR")]
        [StringLength(100)]
        public string TwilioTwimlAppSid { get; set; }

        public string TwiliopathSid { get; set; }

        public bool IsTranslator { get; set; } = false;


        [ForeignKey("TwilioNumbersTable")]
        public int? TwilioNumbersTableId { get; set; }

        public virtual TwilioNumbersTable TwilioNumbersTable { get; set; }

        public virtual List<Liaison_CPTRates> LiaisonCPTRates { get; set; }     
        public virtual List<Liaisons_BillingCategories> Liaisons_BillingCategories { get; set; }  
        public virtual List<Patients_BillingCategories> Patients_BillingCategories { get; set; }

      

    }
    public class LiaisonDropdown
    {
        public string SId { get; set; }
        public string SName { get; set; }
    }
    public class Liaison_CPTRates
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

      
      
        public int? LiaisonId { get; set; }
        public virtual Liaison Liaison { get; set; }
        [Column(TypeName = "VARCHAR")]
        [StringLength(100)]
        public string BillingCode { get; set; }

        public decimal? SalaryRate { get; set; }
        [ForeignKey("BillingCodes")]
        public int? BillingCodeId { get; set; }

        public virtual BillingCodes BillingCodes { get; set; }
        public virtual List<BillingCycle> BillingCycles { get; set; }



    }
    public class LiaisonViewModel
    {
        public Liaison   Liaison   { get; set; }
        public DateTime? LastLogin { get; set; }
    }

    public class LoginHistory
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public DateTime LoginDateTime { get; set; }

        public DateTime? LogOutDateTime { get; set; }
    }

    public class QualityControl
    {
        public int Id { get; set; }

        [Column(TypeName = "VARCHAR")]
        [StringLength(50)]
        public string UserId { get; set; }

        [Display(Name = "Date Entered")]
        [DataType(DataType.Date)]
        public DateTime? CreatedOn { get; set; }

        [Display(Name = "Created By")]
        [Column(TypeName = "VARCHAR")]
        [StringLength(100)]
        public string CreatedBy { get; set; }

        [Display(Name = "Date Updated")]
        [DataType(DataType.Date)]
        public DateTime? UpdatedOn { get; set; }

        [Display(Name = "Updated By")]
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

        [Display(Name = "Gender")]
        [Column(TypeName = "VARCHAR")]
        [StringLength(50)]
        public string Gender { get; set; }

        [Required]
        [Display(Name = "Mobile Phone Number")]
        
        public string MobilePhoneNumber { get; set; }

        [Required]
        [EmailAddress]
        [Column(TypeName = "VARCHAR")]
        [StringLength(100)]
        public string Email { get; set; }

        [Display(Name = "Address")]
        [Column(TypeName = "VARCHAR")]
        [StringLength(100)]
        public string Address { get; set; }

      

        [Display(Name = "City")]
        [Column(TypeName = "VARCHAR")]
        [StringLength(100)]
        public string City { get; set; }
        
    }

    public class SaleStaff
    {
        public int Id { get; set; }

        [Column(TypeName = "VARCHAR")]
        [StringLength(50)]
        public string UserId { get; set; }

        [Display(Name = "Date Entered")]
        [DataType(DataType.Date)]
        public DateTime? CreatedOn { get; set; }

        [Display(Name = "Created By")]
        [Column(TypeName = "VARCHAR")]
        [StringLength(100)]
        public string CreatedBy { get; set; }

        [Display(Name = "Date Updated")]
        [DataType(DataType.Date)]
        public DateTime? UpdatedOn { get; set; }

        [Display(Name = "Updated By")]
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

        [Display(Name = "Gender")]
        [Column(TypeName = "VARCHAR")]
        [StringLength(50)]
        public string Gender { get; set; }

        [Required]
        [Display(Name = "Mobile Phone Number")]

        public string MobilePhoneNumber { get; set; }

        [Required]
        [EmailAddress]
        [Column(TypeName = "VARCHAR")]
        [StringLength(100)]
        public string Email { get; set; }

        [Display(Name = "Address")]
        [Column(TypeName = "VARCHAR")]
        [StringLength(100)]
        public string Address { get; set; }



        [Display(Name = "City")]
        [Column(TypeName = "VARCHAR")]
        [StringLength(100)]
        public string City { get; set; }

    }
    public class Reasons
    {
        public int Id { get; set; }
        public string ReasonText { get; set; }
        public string Module { get; set; }
    }


}