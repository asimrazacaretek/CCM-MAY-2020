using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace CCM.Models.DataModels
{
    public class PatientProfile_Hospitalvisits_History
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Nullable<int> Id { get; set; }
        public int? PatientId { get; set; }

        public string HospitalName { get; set; }

  
        public string HNPI { get; set; }

        public string HType { get; set; }


        public string HAddress { get; set; }

        public DateTime AdmitDate { get; set; }


        public DateTime DischargeDate { get; set; }

        public int Rate { get; set; }

        public int TotalDays { get; set; }

        public string Reason { get; set; }

        public string Department { get; set; }


        public string ICD10Codes { get; set; }

        public string FullName { get; set; }

        public string NPI { get; set; }


        public string Phone { get; set; }

    
        public string StayType { get; set; }


        public string CreatedBy { get; set; }

        public DateTime CreatedOn { get; set; }

        public string UpdatedBy { get; set; }
        public string OtherProcedure { get; set; }

        public DateTime UpdatedOn { get; set; }

        public int? HospitalReasonsId { get; set; }

        public int? HospitalDepartmentsId { get; set; }


        public int? HospitalProceduresId { get; set; }




    }
}