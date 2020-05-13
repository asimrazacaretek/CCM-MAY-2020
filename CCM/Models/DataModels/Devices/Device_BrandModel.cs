using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace CCM.Models.DataModels.Devices
{
    public class Device_BrandModel
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }

        [ForeignKey("Devices_Brand")]
        public int? Devices_BrandId { get; set; }
        public virtual Devices_Brand Devices_Brand { get; set; }
        [ForeignKey("Device_Vendor")]
        public int? Device_VendorId { get; set; }
        public virtual Device_Vendor Device_Vendor { get; set; }
        [ForeignKey("RPMService")]
        public int? RPMServiceId { get; set; }
        public virtual RPMService RPMService { get; set; }

    }
}