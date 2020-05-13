using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace CCM.Models.DataModels
{
    public class Patient_HospitalVisit_NotAdmitted
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public int? PatientId { get; set; }

        public bool PatientNotAdmitted { get; set; }


        public Nullable<bool> Status { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string CreatedBy { get; set; }

        public DateTime? UpdatedOn { get; set; }
        public string UpdatedBy { get; set; }

    }
}