using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using CCM.Models;

namespace CCM.Models.CCMBILLINGS
{
    public class Patients_BillingCategories
    {
        public int Id { get; set; }
        [ForeignKey("Patients")]
        public int? PatientId { get; set; }
        [ForeignKey("BillingCategory")]
        public int? BillingCategoryId { get; set; }
        [ForeignKey("Liaison")]
        public int? LiaisonId { get; set; }

        public bool Status { get; set; }
        public int? CycleId { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public string UpdatedBy { get; set; }

        public DateTime? CreatedOn { get; set; }

        public string CreatedBy { get; set; }
        public DateTime? EnrolledOn { get; set; }
        public DateTime? DeEnrolledOn { get; set; }
        public string DeEnrollmentReason { get; set; }
        public bool IsTranslator { get; set; }
        public int? TranslatorId { get; set; }

        public virtual Patient Patients { get; set; }
        public virtual BillingCategory BillingCategory { get; set; }
        public virtual Liaison Liaison { get; set; }






    }
}