using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CCM.Models.ViewModels
{
    public class EnrollmentListViewModel
    {
        public int BillingcatagoryId { get; set; }
        public string EnrollmentStatus { get; set; }
        public string EnrollmentSubStatus { get; set; }
        public string EnrollemntStatusReson { get; set; }
    }
}