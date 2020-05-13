using CCM.Models.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CCM.Models.ViewModels
{
    public class G0506FormDataViewModel
    {
        //public string FullName { get; set; }
        //public int? PrimaryCareProviderId { get; set; }
        public G0506FormDataViewModel()
        {
            this.G0506_PatientsInfo = new G0506_PatientsInfo();
            this.G0506_AdditionalProviders = new G0506_AdditionalProviders();
            this.G0506_PrimaryInsurance = new G0506_PrimaryInsurance();
            this.G0506_SecondaryInsurance = new G0506_SecondaryInsurance();
            this.UrgencyContactList = new List<PatientProfile_UrgencyContact>();
            this.EvaluationViewModel = new EvaluationViewModel();
        }


        public G0506_PatientsInfo G0506_PatientsInfo { get; set; }
        public G0506_AdditionalProviders G0506_AdditionalProviders { get; set; }

        public G0506_PrimaryInsurance G0506_PrimaryInsurance { get; set; }
        public G0506_SecondaryInsurance G0506_SecondaryInsurance { get; set; }

        public List<PatientProfile_UrgencyContact> UrgencyContactList { get; set; }

        public EvaluationViewModel EvaluationViewModel { get; set; }




    }
}