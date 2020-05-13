using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CCM.Models.ViewModels
{
    public class RpmQueFilterViewModel
    {
        public RpmQueFilterViewModel()
        {
            this.BrandId = 0;
            this.DeviceCurrentStatus = 0;
            this.ModelId = 0;
            this.RpmServiceId = 0;
            this.SerialNumber = "";
            this.VendorId = 0;
        }
        public int? BrandId { get; set; }
        public int? VendorId { get; set; }
        public int? ModelId { get; set; }
        public string SerialNumber { get; set; }
        public int? RpmServiceId { get; set; }
        public int? DeviceCurrentStatus { get; set; }
    }
}