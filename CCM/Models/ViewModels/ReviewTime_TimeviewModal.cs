using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CCM.Models.ViewModels
{
    public class ReviewTime_TimeviewModal
    {
        public int? BillingCategoryId { get; set; }
        public TimeSpan Time { get; set; }
    }
}