using CCM.Models.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CCM.Models.ViewModels
{
    public class BulkChangesViewModel
    {
        public int id { get; set; }
        public string Title { get; set; }
        public string CreatedOn { get; set; }
        public string CreatedBy { get; set; }

        public List<BulkChangesLog> bulkChangesLogs { get; set; }
    }
}