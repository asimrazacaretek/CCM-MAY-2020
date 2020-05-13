using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CCM.Models.ViewModels
{
    public class TotalPatientsViewModel
    {

        public TotalPatientsViewModel()
        {
            this.TotalPatientsCategoryViewModel = new List<TotalPatientsCategoryViewModel>();
        }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int? Id { get; set; }

        public int? Cycle { get; set; }

        public DateTime BirthDate { get; set; }

        public string BirthDatestr { get; set; }

        public string Gender { get; set; }

        public string PreferredLanguage { get; set; }

        public DateTime? AppointmentDate { get; set; }
        

        public string AppointmentDateStr { get; set; }
        public string EnrollmentStatus { get; set; }
        public int? LiaisonId { get; set; }

        public string liaisonFirstName { get; set; }
        public string liaisonLastName { get; set; }
        public DateTime? liaisonassignedon { get; set; }
        public DateTime? enrolledon { get; set; }
        public string enrolledonstr { get; set; }
        public string DocFirstName { get; set; }
        public string DocLastName { get; set; }
        public string enrollmentsubstatus { get; set; }
        public string callingstatus { get; set; }
        public string emrnumber { get; set; }
        public string emrtype { get; set; }
        public string picassochecked { get; set; }
        public DateTime? picssodate { get; set; }
        public string capitated { get; set; }
        public DateTime? capitatedfrom { get; set; }
        public DateTime? capitatedto { get; set; }
        public int PhysicianId { get; set; }
        public string MainPhoneNumber { get; set; }
        public string note { get; set; }
        public string insuranceid { get; set; }
        public string insurancename { get; set; }
        public int? TranslatorId { get; set; }

        public string Translator { get; set; }
        public DateTime? TranslatorAssignedOn { get; set; }
        public string TranslatorAssignedOnstr { get; set; }
        public int? Patients_PreLiaisonId { get; set; }
        public int? preliaisonId { get; set; }
        public int? pretranslatorId { get; set; }

        public Nullable< bool> prestatus { get; set; }
        public int? PostLiaisonId { get; set; }

        public string LiaisonName { get; set; }


        public string PreLiaisonName { get; set; }
        public string PreTranslatorName { get; set; }
        public int? BillingCategoryId { get; set; }
        public string BillingCategoryName { get; set; }
        public string PhyGroupName { get; set; }
        public List<TotalPatientsCategoryViewModel> TotalPatientsCategoryViewModel { get; set; }


















    }
}