using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace CCM.Models.DataModels.Devices
{
    public class DeviceMappingHistory
    {
        [Key]
        public int Id { get; set; }
        public string Message { get; set; }
        public string Type { get; set; }
        public DateTime DatePerformed { get; set; }

        [ForeignKey("RPMService")]
        public int? RPMServiceId { get; set; }
        public virtual RPMService RPMService { get; set; }

        [ForeignKey("Patient")]
        public int? PatientId { get; set; }
        public virtual Patient Patient { get; set; }

        [ForeignKey("Device")]
        public int? DevicetId { get; set; }
        public virtual Device Device { get; set; }
        public string CreatedBy { get; set; }
        public int? IsSyncFormlive { get; set; }
    }
}