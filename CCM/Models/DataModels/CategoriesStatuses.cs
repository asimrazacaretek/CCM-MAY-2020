using CCM.Models.CCMBILLINGS;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace CCM.Models.DataModels
{
    public class CategoriesStatuses
    {

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]

        public int Id { get; set; }
        public int? PatientId { get; set; }
        [Column(TypeName = "VARCHAR")]
        [StringLength(50)]
        public string Status { get; set; }

        [Column(TypeName = "VARCHAR")]
        [StringLength(50)]
        public string SubStatus { get; set; }

        public int? Cycle { get; set; }

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
        public DateTime? ReconciliationDate { get; set; }

        [Display(Name = "Claim Submission Date")]
        [DataType(DataType.Date)]
        public DateTime? ClaimSubmissionDate { get; set; }

        [Display(Name = "Clinical SignOff Date")]
        [DataType(DataType.Date)]
        public DateTime? ClinicalSignOffDate { get; set; }

        [Display(Name = "Careplan Rejected Date")]
        [DataType(DataType.Date)]
        public DateTime? RejectedDate { get; set; }

        public int RejectedCount { get; set; } = 0;

        public string Notes { get; set; }

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
        public DateTime? ReadyforClinicalSignOffDate { get; set; }

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
        public DateTime? RejectedDatebyLiaison { get; set; }

        [Display(Name = "Submitted To Ready For Clinical Signoff")]

        public string SubmittedToReadyforClinicalSignoff { get; set; }
        public string BillingCategoryName { get; set; }


        [ForeignKey("BillingCategory")]
        public int? BillingCategoryId { get; set; }
        public virtual BillingCategory BillingCategory { get; set; }




    }
}