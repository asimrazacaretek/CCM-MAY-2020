using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CCM.Models.CCMBILLINGS.ViewModels
{
    public class PatientBillingCategoryViewModelEdit
    {
        public string BillingcategoryId { get; set; }
        public string DeEnrollmentReason { get; set; }

        public string LiaisonId { get; set; }
        public string TranslatorId { get; set; }
    }
}