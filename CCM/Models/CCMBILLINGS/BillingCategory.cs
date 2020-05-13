using CCM.Models.DataModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace CCM.Models.CCMBILLINGS
{
    public class BillingCategory
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int BillingCategoryId { get; set; }
        public string Name { get; set; }
        public double MinimunMinutes { get; set; }

        public virtual List<BillingCodes> BillingCodes { get; set; }
        [ForeignKey("BillingPeriods")]
        public int? BillingPeriodsId { get; set; }
        public virtual BillingPeriods BillingPeriods { get; set; }

        public virtual List<Patients_BillingCategories> Patients_BillingCategories { get; set; } 
        public virtual List<Liaisons_BillingCategories> Liaisons_BillingCategories { get; set; }

        public virtual List<EnrollmentSubstatusReason> EnrollmentSubstatusReasons { get; set; }
        public virtual List<Evaluation> Evaluations { get; set; }






    }
}