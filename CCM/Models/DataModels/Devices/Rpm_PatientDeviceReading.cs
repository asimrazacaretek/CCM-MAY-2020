using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace CCM.Models.DataModels.Devices
{
    public class Rpm_PatientDeviceReading
    {
        [Key]
        public int Id { get; set; }
        public int? Battery { get; set; }
        public double? Blood_glucose_mgdl { get; set; }
        public string Reading_type { get; set; }
        public int? Reading_id { get; set; }
        public double? Time_zone_offset { get; set; }
        public double? Blood_glucose_mmol { get; set; }
        public string Device_model { get; set; }
        public DateTime? Date_recorded { get; set; }
        public DateTime? Date_received { get; set; }
        public bool? Before_meal { get; set; }
        public string Device_id_RPMVendor { get; set; }
        [ForeignKey("RPMService")]
        public int? RPMServiceId { get; set; }
        public virtual RPMService RPMService { get; set; }
        [ForeignKey("Patient")]
        public int? PatientId { get; set; }
        public virtual Patient Patient { get; set; }
        [ForeignKey("Device")]
        public int? DevicetId { get; set; }
        public virtual Device Device { get; set; }
        public int? IsActive { get; set; }
        public string ReasonForDeactivate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
    }
}