using CCM.Models.CCMBILLINGS.ViewModels;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Collections.Generic;
using System.Security.Principal;

namespace CCM.Models.ViewModels
{
    public class BulkChangeViewModel
    {
        public List<int> PatientsList { get; set; }
        public List<PatientBillingCategoryViewModel> EnrollementList { get; set; }
        public List<PatientBillingCategoryViewModel> PostCounselorTranslatorList { get; set; }
        public List<DeEnrollmentReasonViewModel> DeEnrollmentReason { get; set; }
        public string ChangeStatus { get; set; }
        public string EnrollmentStatus { get; set; }
        public string EnollmentSubStatus { get; set; }
        public string EnrollemntStatusReson { get; set; }
        public int? PreLiaisonId { get; set; }
        public int? PreTranslaterId { get; set; }
        public List<int> MigrateAppointmentIn_Id { get; set; }
        public string MigrateAppointment { get; set; }
       
    }
}