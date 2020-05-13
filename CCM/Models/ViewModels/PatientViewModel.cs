using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CCM.Models.ViewModels
{
    public class PatientViewModel
    {
        public int DaysinQue { get; set; } = 0;
        public string Color = "";
        public string dateColor = "";
        public string DateEntered = "";
        public string ReviewTime = "";
        public string ageStatus = "";
        public int Id;
        public int Cycle;
        public string Cyclestr = "";
        public string BirthDate = "";
        public string Gender = "";
        public string PreferredLanguage = "";
        public string AppointmentDate = "";
        public string CouncelorAppointmentDate = "";
        public string TranslatorAppointmentDate = "";
        public string EnrollerAppointmentDate = "";
        public string PatientName = "";
        public string EnrollmentStatus = "";
        public string EnrollmentSubStatus = "";
        public string Category = "";

        public string LiaisonId;
        public string liaisonName = "";
        public string liaisonLastName = "";
        public string DocFirstName = "";
        public string DocLastName = "";
        public string UserRole = "";
        public string CMMReviewLink;
        public string CcmStatus = "";
        public string CcmReconciliationDate = "";
        public string CcmClaimSubmissionDate = "";
        public string CcmClinicalSignOffDate = "";
        public string CcmBillingCode = "";
        public string CcmBillingCode2 = "";
        public string CcmEnrolledOn = "";
        public string LiasionAssignedOn = "";
        public string language = "";
        public string note = "";
        public string Detailslink = "";
        public string Deletelink = "";
        public string CallingStatus = "";
        public string emrnumber = "";
        public string emrtype = "";
        public string medicareeligibility = "";
        public string medicaideligibility = "";
        public string capitated = "";
        public string capitatedfrom = "";
        public string capitatedto = "";
        public string insurancename = "";
        public string insuranceid = "";
        public string LastUpdatedDate = "";
        public string ccmnotes = "";
        public string Activitytext = "";
        public string ICD10Codes = "";
        public string Address = "";
        public string QAQCName = "";
        public string CCMUpdatedOn = "";
        public string medicareid = "";
        public string medicaidid = "";
        public string otherinsuranceid = "";
        public string approvedby = "";
        public string rejectedby = "";
        public int TotalCountCarePlanTobeShared = 0;
        public int TotalCareplanShared = 0;
        public int TotalSharedRemining = 0;
        public List<CPTCODES> cPTCODEs = new List<CPTCODES>();
        public List<ActiveWorkQueTotals> ActiveWorkQueTotals = new List<ActiveWorkQueTotals>();
        public string activeworkquechart = "";
        public string SubmittedBy { get; set; }
        public string TranslatorName { get; set; }

        public string PhysicianGroup { get; set; }

        public DateTime? CcmRejectedDatebyLiaison { get; set; }
        public string RejectedbyLiaison { get; set; }

        public bool IsRejectedByLiaison { get; set; }
    }
}