using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace CCM.Models.DataModels
{
    public class G0506_PrimaryInsurance
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string PlanName { get; set; }
        public int? InsuranceId { get; set; }
        public int? GroupNumber { get; set; }
        public string PlanCode { get; set; }
        public string RxBin { get; set; }
        public string RxPCN { get; set; }
        public string RxGroup { get; set; }
        public string RelatedtoInsurance { get; set; }

        public bool Status { get; set; }

        public string CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }


    }
}