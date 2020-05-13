using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace CCM.Models.CCMBILLINGS
{
    public class Liaisons_BillingCategories
    {
        [Key]
        public int Id { get; set; }

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

       
        public virtual BillingCategory BillingCategory { get; set; }
        public virtual Liaison Liaison { get; set; }

    }
}