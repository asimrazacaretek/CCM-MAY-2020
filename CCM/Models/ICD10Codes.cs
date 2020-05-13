using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CCM.Models
{
    public class Icd10Codes
    {
        public int Id { get; set; }
        public string Code10 { get; set; }
        public string Code9 { get; set; }
        public int PatientId { get; set; }
        public string DiseaseState { get; set; }
        public string DiseaseType { get; set; }

        public string DiseaseHistory { get; set; }
        public DateTime? DateCreated { get; set; }

    }
}