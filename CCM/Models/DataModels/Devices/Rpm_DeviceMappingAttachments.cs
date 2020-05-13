using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace CCM.Models.DataModels.Devices
{
    public class Rpm_DeviceMappingAttachments
    {
        [Key]
        public int id { get; set; }
        public string Image { get; set; }
        [ForeignKey("DeviceMappingHistory")]
        public int DeviceMappingHistoryId { get; set; }
        public virtual DeviceMappingHistory DeviceMappingHistory { get; set; }

        [ForeignKey("Patient")]
        public int? PatientId { get; set; }
        public virtual Patient Patient { get; set; }
    }
}