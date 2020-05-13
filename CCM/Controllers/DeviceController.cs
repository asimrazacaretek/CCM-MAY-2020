using CCM.Helpers;
using CCM.Models;
using CCM.Models.DataModels;
using CCM.Models.DataModels.Devices;
using CCM.Models.ENUMS_;
using CCM.Models.ViewModels;
using Microsoft.Ajax.Utilities;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace CCM.Controllers
{
    public class DeviceController : BaseController
    {
        // if (User.IsInRole("Admin"))
        //private Application_dbContect _db = new Application_dbContect();
        // GET: Device
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public ActionResult Index()
        {
            var model = new Device();
            //for Creation Modal
            ViewBag.IsAvtiveStatus = Enum.GetValues(typeof(IsActiveStatus)).Cast<IsActiveStatus>().Select(y => new SelectListItem { Text = y.ToString(), Value = ((int)y).ToString() }).ToList();
            ViewBag.DeviceStatus = Enum.GetValues(typeof(DeviceStatus)).Cast<DeviceStatus>().Select(y => new SelectListItem { Text = y.ToString(), Value = ((int)y).ToString() }).ToList();
            ViewBag.ModelNumber = _db.Device_BrandModels.ToList();
            ViewBag.Vendors = _db.Device_Vendors.Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() }).ToList();
            ViewBag.Device_brands = _db.Devices_Brands.Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Name }).ToList();
            //for Table
            ViewBag.Devices = _db.Device.ToList();
            return View(model);
        }
















        [Authorize(Roles = "Admin")]
        public ActionResult DeviceIndex()
        {
            string currentuserId = User.Identity.GetUserId();
            DevicesListViewModel model = new DevicesListViewModel();

            model.devicelistFullBo = new List<DevicesListFullBo>();

            //model.devicelistFullBo = db.Database.SqlQuery<DevicesListFullBo>("GetAllDevices @DeviceName,@ModelNumber,@SerialNumber,@VendorName,@RPMServiceId,@DeviceStatusId,@IsActive,@Datepurchase,@DateCreatedStart,@DateCreatedEnd",
            //                 new SqlParameter("@DeviceName", string.Empty), new SqlParameter("@ModelNumber", string.Empty), new SqlParameter("@SerialNumber", string.Empty)
            //                 , new SqlParameter("@VendorName", string.Empty), new SqlParameter("@RPMServiceId", Convert.ToInt32(0))
            //                 , new SqlParameter("@DeviceStatusId", Convert.ToInt32(0)), new SqlParameter("@IsActive", Convert.ToInt32(-1)), new SqlParameter("@Datepurchase", DBNull.Value)
            //                 , new SqlParameter("@DateCreatedStart", DBNull.Value), new SqlParameter("@DateCreatedEnd", DBNull.Value)).ToList();

            ViewBag.Devices = _db.Device.ToList();
            return View(model);

        }
        [HttpPost]
        public ActionResult _DeviceListView(DevicesListViewModel obj)
        {

         
            var DatepurchaseSearch = (obj.DatepurchaseSearch == null) ? (object)DBNull.Value : (object)Convert.ToDateTime(obj.DatepurchaseSearch);
            var StartDateSearch = (obj.startDate == null) ? (object)DBNull.Value : (object)Convert.ToDateTime(obj.startDate);
            var EnddDteSearch = (obj.enddate == null) ? (object)DBNull.Value : (object)Convert.ToDateTime(obj.enddate);
            obj = NullAssingValuesToRequestSP(obj);
            obj.devicelistFullBo = _db.Database.SqlQuery<DevicesListFullBo>("GetAllDevices @DeviceName,@ModelNumber,@SerialNumber,@VendorName,@RPMServiceId,@DeviceStatusId,@IsActive,@Datepurchase,@DateCreatedStart,@DateCreatedEnd",
                            new SqlParameter("@DeviceName", obj.DeviceNameSearch), new SqlParameter("@ModelNumber", obj.ModelNumberSearch), new SqlParameter("@SerialNumber", obj.SerialNumberSearch)
                            , new SqlParameter("@VendorName",obj.VendorNameSearch), new SqlParameter("@RPMServiceId", Convert.ToInt32(obj.DeviceTypeIdSearch))
                            , new SqlParameter("@DeviceStatusId", Convert.ToInt32(obj.DeviceStatusIdSearch)), new SqlParameter("@IsActive", Convert.ToInt32(obj.IsActiveSearch)), new SqlParameter("@Datepurchase", DatepurchaseSearch)
                            , new SqlParameter("@DateCreatedStart", StartDateSearch), new SqlParameter("@DateCreatedEnd", EnddDteSearch)).ToList();

            ViewBag.Devices = _db.Device.ToList();
            return PartialView(obj.devicelistFullBo);
        }

        private DevicesListViewModel NullAssingValuesToRequestSP(DevicesListViewModel obj) {
            if (obj.DeviceNameSearch == null) {
                obj.DeviceNameSearch = string.Empty;
            }
            if (obj.ModelNumberSearch == null)
            {
                obj.ModelNumberSearch = string.Empty;
            }
            if (obj.SerialNumberSearch == null)
            {
                obj.SerialNumberSearch = string.Empty;
            }
            if (obj.VendorNameSearch == null)
            {
                obj.VendorNameSearch = string.Empty;
            }
            if (obj.DeviceTypeIdSearch == null)
            {
                obj.DeviceTypeIdSearch = "0";
            }
            if (obj.DeviceStatusIdSearch == null)
            {
                obj.DeviceStatusIdSearch = "0";
            }
            if (obj.IsActiveSearch == null)
            {
                obj.IsActiveSearch = "0";
            }

            return obj;
        }

        //[HttpPost]
        public ActionResult _DeviceCreate()
        {

            //  string currentuserId = User.Identity.GetUserId();

            //obj.device = new Device();
            var model = new Device();
            ViewBag.IsavtiveStatus = 1;
            ViewBag.IsAvtiveStatus = Enum.GetValues(typeof(IsActiveStatus)).Cast<IsActiveStatus>().Select(y => new SelectListItem { Text = y.ToString(), Value = ((int)y).ToString() }).ToList();
            ViewBag.DeviceStatus = Enum.GetValues(typeof(DeviceStatus)).Cast<DeviceStatus>().Select(y => new SelectListItem { Text = y.ToString(), Value = ((int)y).ToString() }).ToList();
         
            ViewBag.ModelNumber = _db.Device_BrandModels.ToList();
            ViewBag.Vendors = _db.Device_Vendors.Select(x=>new SelectListItem { Text=x.Name,Value=x.Id.ToString()}).ToList();


            ViewBag.Device_brands = _db.Devices_Brands.Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Name }).ToList();
            return View(model);
            //return PartialView(obj);
        }


        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<JsonResult> DeviceCreate(Device obj)
        {

            List<string> SerialNumbers = new List<string>();
            if (obj.SerialNumber != null)
            {
                SerialNumbers = obj.SerialNumber.Split(',').ToList();
            }

            if (obj.Id == 0)
            {
                //var exsist = db.Device_BrandModels.FirstOrDefault(x => x.Name == obj.Name && x.Devices_BrandId == obj.Devices_BrandId);
                //if (exsist != null)
                //{
                //    return Json("exists", JsonRequestBehavior.AllowGet);
                //}
                int save=0;
                foreach (var item in SerialNumbers)
                {
                    obj.SerialNumber = item;
                    obj.CreatedBy = User.Identity.GetUserId();
                    obj.CreatedDate = DateTime.Now;
                    _db.Device.Add(obj);
                     save = await _db.SaveChangesAsync();
                }

            
                if (save > 0)
                    return Json("added", JsonRequestBehavior.AllowGet);
                return Json("0", JsonRequestBehavior.AllowGet);
            }
            else
            {
                int save = 0;
                foreach (var item in SerialNumbers)
                {
                    var model = await _db.Device.FirstOrDefaultAsync(x => x.Id == obj.Id && x.SerialNumber==item);

                    if (model != null)
                    {
                        model.DatePurchase = obj.DatePurchase;
                        model.DeviceStatusId = obj.DeviceStatusId;
                        model.Devices_BrandId = obj.Devices_BrandId; ;
                        model.Device_BrandModelId = obj.Device_BrandModelId;
                        model.VendorId = obj.VendorId;
                        model.IsActive = obj.IsActive;
                        model.RPMServiceId = obj.RPMServiceId;
                        model.ReasonForDeactivate = obj.ReasonForDeactivate;
                        model.SerialNumber = item;

                        model.ModifiedBy = User.Identity.GetUserId();

                        model.ModifiedDate = DateTime.Now;
                        _db.Entry(model).State = EntityState.Modified;
                         save = await _db.SaveChangesAsync();
                    }
                    else
                    {
                        obj.SerialNumber = item;
                        obj.CreatedBy = User.Identity.GetUserId();
                        obj.CreatedDate = DateTime.Now;
                        _db.Device.Add(obj);
                        save = await _db.SaveChangesAsync();
                    }
                }
            
                if (save > 0)
                    return Json("updated", JsonRequestBehavior.AllowGet);
                return Json("error", JsonRequestBehavior.AllowGet);
            }









            //List<string> SerialNumbers = new List<string>();
            //if (obj.device.SerialNumber != null)
            //{
            //    SerialNumbers = obj.device.SerialNumber.Split(',').ToList();
            //}

            //var objsd = new Device(); 

            //if (obj.device.ReasonForDeactivate == null)
            //{
            //    obj.device.ReasonForDeactivate = "";
            //}
            //if (Session["UserID"] != null)
            //{
            //    obj.device.CreatedBy = Session["UserID"].ToString();
            //    obj.device.ModifiedBy = Session["UserID"].ToString();
            //}

            //obj.device.CreatedDate = DateTime.Now;
            //obj.device.ModifiedDate = DateTime.Now;
            //int result = 0;
            //int i = 0;
            //foreach (var item in SerialNumbers)
            //{
            //    i++;
            //    var serialNumber = item;
            //    var deviceName = obj.device.DeviceName.ToString() + "-" + i;
            //    result = await _db.Database.SqlQuery<int>("InsertUpdateDeviceWithHistory @Id,@DeviceName,@DeviceTypeId,@ModelNumber,@SerialNumber,@Datepurchase,@VendorName,@DeviceStatusId,@IsActive,@ReasonForDeactivate,@CreatedBy,@CreatedDate,@ModifiedBy,@ModifiedDate", 
            //                  new SqlParameter("@Id", obj.device.Id),new SqlParameter("@DeviceName", deviceName), new SqlParameter("@DeviceTypeId", obj.device.RPMServiceId)
            //                , new SqlParameter("@ModelNumber", obj.device.ModelNumber), new SqlParameter("@SerialNumber", item), new SqlParameter("@Datepurchase", obj.device.DatePurchase)
            //                , new SqlParameter("@VendorName", obj.device.VendorName), new SqlParameter("@DeviceStatusId", obj.device.DeviceStatusId), new SqlParameter("@IsActive", obj.device.IsActive)
            //                , new SqlParameter("@ReasonForDeactivate", obj.device.ReasonForDeactivate), new SqlParameter("@CreatedBy", obj.device.CreatedBy), new SqlParameter("@CreatedDate", obj.device.CreatedDate)
            //                , new SqlParameter("@ModifiedBy", obj.device.ModifiedBy), new SqlParameter("@ModifiedDate", obj.device.ModifiedDate)).FirstOrDefaultAsync();
            //}
            //return Json(result, JsonRequestBehavior.AllowGet);
            return Json("", JsonRequestBehavior.AllowGet);
           
        }

       public ActionResult Filters()
        {
            ViewBag.IsAvtiveStatus = Enum.GetValues(typeof(IsActiveStatus)).Cast<IsActiveStatus>().Select(y => new SelectListItem { Text = y.ToString(), Value = ((int)y).ToString() }).ToList();
            ViewBag.DeviceStatus = Enum.GetValues(typeof(DeviceStatus)).Cast<DeviceStatus>().Select(y => new SelectListItem { Text = y.ToString(), Value = ((int)y).ToString() }).ToList();
            ViewBag.ModelNumber = _db.Device_BrandModels.Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() }).ToList();
            ViewBag.Vendors = _db.Device_Vendors.Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() }).ToList();
            ViewBag.Device_brands = _db.Devices_Brands.Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Name }).ToList();
            DeviceFilterViewModel model = new DeviceFilterViewModel();
            return View(model);
        }

        public ActionResult LoadDevices(DeviceFilterViewModel obj)
        {
            var devicesList = new List<Device>();
            devicesList = _db.Device.ToList();
            
            if (obj == null)
            {
                return View(devicesList);
            }
            else
            {
                if (obj.CreatedStartDate != null)
                {
                    devicesList = devicesList.Where(x => x.CreatedDate >= obj.CreatedStartDate).ToList();
                }
                else if (obj.CreatedEndDate != null)
                {
                    devicesList = devicesList.Where(x => x.CreatedDate <= obj.CreatedStartDate).ToList();
                }
                else if (obj.DatePurchase != null)
                {
                    devicesList = devicesList.Where(x => x.DatePurchase >= obj.DatePurchase).ToList();
                }
                else if (obj.BrandId != null)
                {
                    devicesList = devicesList.Where(x => x.Devices_BrandId == obj.BrandId).ToList();
                }
                else if (obj.VendorId != null)
                {
                    devicesList = devicesList.Where(x => x.VendorId == obj.VendorId).ToList();
                }
                else if (obj.ModelId != null)
                {
                    devicesList = devicesList.Where(x => x.Device_BrandModelId == obj.ModelId).ToList();
                }
             
                else if (obj.SerialNumber != null)
                {
                    devicesList = devicesList.Where(x => x.SerialNumber.ToLower() == obj.SerialNumber || x.SerialNumber.StartsWith(obj.SerialNumber) || x.SerialNumber.Contains(obj.SerialNumber)).ToList();
                }
                else if (obj.RpmServiceId != null)
                {
                    devicesList = devicesList.Where(x => x.RPMServiceId == obj.RpmServiceId).ToList();
                }
                else if (obj.DeviceCurrentStatus != null)
                {
                    devicesList = devicesList.Where(x => x.DeviceStatusId == obj.DeviceCurrentStatus).ToList();
                }
            }
            return View(devicesList);
        }


    }
}