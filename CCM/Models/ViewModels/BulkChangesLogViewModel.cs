using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CCM.Models.ViewModels
{
    public class BulkChangesLogViewModel
    {

        public int? PatientId { get; set; }
        public string ResultMassage { get; set; }

        public int Status { get; set; }
        //public Patient Patient { get; set; }
        public string CreatedOn { get; set; }
        public string Createdby { get; set; }
    }
}