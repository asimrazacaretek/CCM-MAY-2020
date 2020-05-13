using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CCM.Models
{
    public class NewTotalPatientsViewModel
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int Cycle { get; set; }
        public DateTime BirthDate { get; set; }
        public string Gender { get; set; }
        public string MyProperty { get; set; }
        public string PreferredLanguage { get; set; }
        public Nullable<DateTime> AppointmentDate { get; set; }
        public string AppointmentDateStr { get; set; }
        public string EnrollmentStatus { get; set; }
        public Nullable<int> LiaisonId { get; set; }
        public Nullable<DateTime> LiasionAssignedOn { get; set; }
        public Nullable<DateTime> CCMEnrolledOn { get; set; }
        public string DocFirstName { get; set; }
        public string DocLastName { get; set; }
        public string EnrollmentSubStatus { get; set; }
        public string CallingStatus { get; set; }
        public string EMRNumber { get; set; }
        public string EMRType { get; set; }
        public string PicassonChecked { get; set; }
        public Nullable<DateTime> PicassoCheckedOn { get; set; }
        public string CapitatedPatient { get; set; }
        public Nullable<DateTime> CapitatedFrom { get; set; }
        public Nullable<DateTime> CapitatedTo { get; set; }
        public int PhysicianId { get; set; }
        public string MainPhoneNumber { get; set; }
        public string EnrollmentNotes { get; set; }
        public string PrimaryIdNumber { get; set; }
        public string PrimaryName { get; set; }
        public int? TranslatorId { get; set; }
        public string Translator { get; set; }

        public Nullable<DateTime> TranslatorAssignedOn { get; set; }
        public Nullable<int> Patients_PreLiaisonsId { get; set; }
        public Nullable<int> PreLiaisonId { get; set; }
        public Nullable<int> PreTranslatorId { get; set; }

        public Nullable<bool> PreStatus { get; set; }

        public string PreLiaisonName { get; set; }
        public string PreTranslatorName { get; set; }
        public int? BillingCategoryId { get; set; }


    }
}