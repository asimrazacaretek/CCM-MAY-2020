using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CCM.Models
{
    public class PatientsforQues
    {
        public int DaysinQue { get; set; }
        public string Color { get; set; }
        public string dateColor { get; set; }

        public DateTime? DateEntered { get; set; }
        public TimeSpan? ReviewTime { get; set; }
        //   ReviewTime =new List<ReviewTimeCcm>(),
        public int Id { get; set; }
        public int Cycle { get; set; }

        public DateTime? BirthDate { get; set; }
        public string Gender { get; set; }
        public string PreferredLanguage { get; set; }
        public DateTime? AppointmentDate { get; set; }
        public string PatientName { get; set; }
        public string EnrollmentStatus { get; set; }

        public string EnrollmentSubStatus { get; set; }
        public int LiaisonId { get; set; }
        public string liaisonFirstName { get; set; }
        public string liaisonLastName { get; set; }
        public string DocFirstName { get; set; }
        public string DocLastName { get; set; }

        public string CMMReviewLink { get; set; }
        public string CcmStatus { get; set; }
        public string Status { get; set; }
        public DateTime? CcmReconciliationDate { get; set; }
        public DateTime? CcmClaimSubmissionDate { get; set; }
        public DateTime? CcmClinicalSignOffDate { get; set; }
        public DateTime? ClinicalSignOffDate { get; set; }
        public DateTime? CcmReadyforClinicalSignOffDate { get; set; }
        public string ApprovedBy { get; set; }
        public string RejectedBy { get; set; }
        public DateTime? CCMEnrolledOn { get; set; }
        public DateTime? CycleCreatedON { get; set; }
        public string UserRole { get; set; }
        public int PhysicianId { get; set; }
        public string MainPhoneNumber { get; set; }
        public DateTime? LiasionAssignedOn { get; set; }
        public string callingstatus { get; set; }
        public string emrnumber { get; set; }
        public string emrtype { get; set; }
        public string picassochecked { get; set; }
        public DateTime? picssodate { get; set; }

        public string capitated { get; set; }
        public DateTime? capitatedfrom { get; set; }
        public DateTime? capitatedto { get; set; }
        public string insuranceid { get; set; }
        public string insurancename { get; set; }
        public DateTime? lastupdatedate { get; set; }
        public string ccmcyclenotes { get; set; }

        public string address { get; set; }
        public string note { get; set; }
        public string CCMUpdatedBy { get; set; }
        public DateTime? CCMUpdatedOn { get; set; }
        public string MedicaidIdNumber { get; set; }
        public string MedicareIdNumber { get; set; }
        public string OtherInsuranceIdNumber { get; set; }
        public string BillingCodes { get; set; }
        public string BillingCode1 { get; set; }
        public string BillingCode2 { get; set; }

   
        public int? BillingCodeId { get; set; }
        public string BillingCodeName { get; set; }

        public string CategoryName { get; set; }
        public int? BillingCategoryId { get; set; }

        public int? BillingCycleId { get; set; }


        public int TotalCountCarePlanTobeShared { get; set; }
        public int TotalCareplanShared { get; set; }
        public int TotalSharedRemining = 0;

        public string SubmittedBy { get; set; }

        public int? TranslatorId { get; set; }
        public DateTime? TranslatorAssignedOn { get; set; }

        public string TranslatorAssignedBy { get; set; }

        public string TranslatorName { get; set; }
        public string PhysicianGroupName { get; set; }

        public DateTime? CcmRejectedDatebyLiaison { get; set; }
        public string RejectedbyLiaison { get; set; }

        public bool IsRejectedByLiaison { get; set; }
    }
}