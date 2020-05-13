using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CCM.Models.CCMBILLINGS.ViewModels
{
    public class BillingViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public string BillingCategory { get; set; }
        public double? Minutes { get; set; }

    }
}