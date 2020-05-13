using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace CCM.Models.CCMBILLINGS.ViewModels
{
    public class Patients_PreLiaisons
    {
        public int Id { get; set; }
        
        public int? LiaisonId { get; set; }
        public int? TranslatorId { get; set; }
        public bool Status { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public string UpdatedBy { get; set; }

        

    }
}