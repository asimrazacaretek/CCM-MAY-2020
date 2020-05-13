using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CCM.Models.DataModels;

namespace CCM.Models
{
    public class PatientDeviceReadingsRequest
    {
        public int Id { get; set; }
        public string Message { get; set; }
        public string Type { get; set; }
        public DateTime? DatePerformed { get; set; }
        public int? RPMServiceId { get; set; }
        public int? PatientId { get; set; }
        public int? DevicetId { get; set; }
        public string CreatedBy { get; set; }
        public string SerialNumber { get; set; }
    }
}