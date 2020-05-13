using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CCM.Models.DataModels
{
    public class HospitalReasons
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool Status { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public string UpdatedBy { get; set; }

    }
}