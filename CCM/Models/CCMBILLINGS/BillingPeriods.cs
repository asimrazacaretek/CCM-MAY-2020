using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace CCM.Models.CCMBILLINGS
{
    public class BillingPeriods
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
    
        public int BillingPeriodsId { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }







    }
}