using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CCM.Models.ViewModels
{
    public class EvaluationViewModel
    {
        public EvaluationViewModel()
        {
            this.MainQuestionViewModal = new List<MainQuestionViewModal>();
        }

        public string Id { get; set; }
        public string Name { get; set; }
        public string PatientId { get; set; }
        public string BillingCategoryId { get; set; }
        public List<MainQuestionViewModal> MainQuestionViewModal { get; set; }


    }
}