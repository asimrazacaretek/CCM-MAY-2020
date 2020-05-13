using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CCM.Models.ViewModels
{
    public class RpmPatientsViewModel
    {
      
        public int Id { get; set; }
        public int? RPMServiceId { get; set; }
        public int? PatientId { get; set; }
        public string PatientName { get; set; }
        public string Gender { get; set; }
        public DateTime? BirthDate { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zipcode { get; set; }
        public string ServiceName { get; set; }
        public DateTime? EnrolledIn { get; set; }
        public int? DeviceId { get; set; }
        public string BrandName { get; set; }
        public string BrandModel { get; set; }
        public int? DeviceStatusId { get; set; }
        public string SerialNumber { get; set; }
        public int? IsAssigned { get; set; }
      
      
    }
}