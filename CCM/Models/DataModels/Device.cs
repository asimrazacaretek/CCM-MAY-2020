using CCM.Models.DataModels.Devices;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
namespace CCM.Models.DataModels
{

    public class Device
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [ForeignKey("Devices_Brand")]
        public int? Devices_BrandId { get; set; }
        public virtual Devices_Brand Devices_Brand { get; set; }
        [ForeignKey("RPMService")]
        public int? RPMServiceId { get; set; }
        public virtual RPMService  RPMService { get; set; }
        [ForeignKey("Device_BrandModel")]
        public int? Device_BrandModelId { get; set; }
        public virtual Device_BrandModel Device_BrandModel { get; set; }
        public string SerialNumber { get; set; }
        public DateTime? DatePurchase { get; set; }
        [ForeignKey("Device_Vendor")]
        public int? VendorId{ get; set; }
        public virtual Device_Vendor Device_Vendor { get; set; }
    
        public int DeviceStatusId { get; set; }
        public int IsActive { get; set; }
        public string ReasonForDeactivate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
    }
}