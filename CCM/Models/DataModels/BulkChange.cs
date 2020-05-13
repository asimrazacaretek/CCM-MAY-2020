using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CCM.Models.DataModels
{
    public class BulkChange
    {
        [Key]
        public int id { get; set; }
        public string Title { get; set; }
        public DateTime CreatedOn { get; set; }
        public string CreatedBy { get; set; }
       
        public List<BulkChangesLog> bulkChangesLogs  { get; set; }
    }
}