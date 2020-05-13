using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CCM.Models.DataModels
{
    public class BulkChangesLog
    {
        [Key]
        public int Id { get; set; }
        public int? PatientId { get; set; }
        //public Patient Patient { get; set; }
        public string ResultMessage { get; set; }
        public int Status { get; set; }
        public DateTime CreatedOn { get; set; }
        public string Createdby { get; set; }
        public int? BulkChangeId { get; set; }
        public BulkChange BulkChange { get; set; }
    }

}