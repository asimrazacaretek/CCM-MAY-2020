using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CCM.Models.ViewModels
{
    public class DeviceFilterViewModel
    {
        public DateTime? CreatedStartDate { get; set; }
        public DateTime? CreatedEndDate { get; set; }
        public DateTime? DatePurchase { get; set; }
        public int? BrandId { get; set; }
        public int? VendorId { get; set; }
        public int? ModelId { get; set; }
        public string SerialNumber { get; set; }
        public int? RpmServiceId { get; set; }
        public int? DeviceCurrentStatus { get; set; }
    }
}