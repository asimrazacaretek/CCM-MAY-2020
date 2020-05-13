using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace CCM.Models.DataModels
{
    public class G0506_SecondaryInsurance
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int Id { get; set; }
        public int? MedicareNo { get; set; }
        
        public int? MedicaidNo { get; set; }
        public int? OtherInsuranceNo { get; set; }

        public bool Status { get; set; }
        public string CreatedBy { get; set; }   
        public DateTime? CreatedOn { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }


    }
}