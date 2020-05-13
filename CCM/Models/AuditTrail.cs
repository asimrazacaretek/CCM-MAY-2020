using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CCM.Models
{
    public class AuditTrail
    {
        public int Id { get; set; }

        public DateTime? createdDate { get; set; }

        public string userId { get; set; }

        public string message { get; set; }

        public string stackTrace { get; set; }

        

    }
}