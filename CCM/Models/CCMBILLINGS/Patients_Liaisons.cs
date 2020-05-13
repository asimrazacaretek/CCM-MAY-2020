using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using CCM.Models;

namespace CCM.Models.CCMBILLINGS
{
    public class Patients_Liaisons
    {
        public int Id { get; set; }
        [ForeignKey("Patients")]
        public int? PatientId { get; set; }
        [ForeignKey("Liaisons")]
        public int? LiaisonId { get; set; }
        public Patient Patients { get; set; }
        public Liaison Liaisons { get; set; }




    }
}