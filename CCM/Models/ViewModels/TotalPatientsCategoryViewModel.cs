using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CCM.Models.ViewModels
{
    public class TotalPatientsCategoryViewModel
    {
        public int? LiaisonId { get; set; }
        public int? TranslatorId { get; set; }

        public int? CategoryId { get; set; }
        public string LiaisonName { get; set; }
        public string TranslatorName { get; set; }
        public string CategoryName { get; set; }




    }
}