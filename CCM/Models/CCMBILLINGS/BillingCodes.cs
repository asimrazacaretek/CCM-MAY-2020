using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace CCM.Models.CCMBILLINGS
{
    public class BillingCodes
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public double MinimunMinutes { get; set; }
        public string Description { get; set; }


        [ForeignKey("BillingCategory")]
        public int? BillingCategoryId { get; set; }
        public virtual BillingCategory BillingCategory { get; set; }

        public virtual List<BillingCycle> BillingCycles { get; set; }




    }
}