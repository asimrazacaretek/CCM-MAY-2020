using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CCM.Models.ViewModels
{
    public class PatientDeviceReading
    {
        DateTime? DateSearchStart { get; set; }
        DateTime? DateSearchEnd { get; set; }
        public int RPMServiceId { get; set; }
        public int PatientId { get; set; }
        public List<PatientDeviceReadingFullBO> PatientReadingList { get; set; }
    }


    public class PatientDeviceReadingFullBO {

        public int Id { get; set; }
        public int? Battery { get; set; }
        public Double? Blood_glucose_mgdl { get; set; }
        public string Reading_type { get; set; }
        public int? Reading_id { get; set; }
        public Double? Time_zone_offset { get; set; }
        public Double? Blood_glucose_mmol { get; set; }
        public string Device_model { get; set; }
        public DateTime? Date_recorded { get; set; }
        public DateTime? Date_received { get; set; }
        public Boolean Before_meal { get; set; }
        public string Device_id_RPMVendor { get; set; }
        public int? RPMServiceId { get; set; }
        public int? PatientId { get; set; }
        public int? DevicetId { get; set; }
        public int? IsActive { get; set; }
        public string ReasonForDeactivate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public string CreatedUserName { get; set; }
        public string ModifiedUserName { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string PatientName { get; set; }
        public string ServiceName { get; set; }
        public string BrandName { get; set; }
        public string BrandModel { get; set; }
        public string SerialNumber { get; set; }
        public int DeviceStatusId { get; set; }
    }
}