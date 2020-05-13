using CCM.Models.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CCM.Models.ViewModels
{
    public class DevicesListViewModel
    {

        public string DeviceNameSearch { get; set; }
        public string ModelNumberSearch { get; set; }
        public string SerialNumberSearch { get; set; }
        public string VendorNameSearch { get; set; }
        public string DeviceTypeIdSearch { get; set; }
        public string DeviceStatusIdSearch { get; set; }
        public string IsActiveSearch { get; set; }
        public DateTime? DatepurchaseSearch { get; set; }
        public DateTime? startDate { get; set; }
        public DateTime? enddate { get; set; }
        public List<DevicesListFullBo> devicelistFullBo { get; set; }

      
 
        // Here you need to add List of complex bo
    }

    public class DevicesCreateViewModel
    {
        public DateTime? enddate { get; set; }
        public Device device { get; set; }

    }

        public class DevicesListFullBo
    {
        public int Id { get; set; }
        public string DeviceName { get; set; }
        public int? RPMServiceId { get; set; }
        public string DeviceTypeName { get; set; }
        public string ModelNumber { get; set; }
        public string SerialNumber { get; set; }
        public DateTime? Datepurchase { get; set; }
        public string VendorName { get; set; }
        public int? DeviceStatusId { get; set; }
        public string CurrentDeviceStatus { get; set; }
        public int? IsActive { get; set; }
        public string ReasonForDeactivate { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedUserName { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public string ModifiedUserName { get; set; }
        public DateTime? ModifiedDate { get; set; }
    }

    public class DevicesTypesList
    {
        public int RPMServiceId { get; set; }
        public string ServiceName { get; set; }
    }




    }