using CCM.Helpers;
using CCM.Models;
using CCM.Models.DataModels;
using CCM.Models.DataModels.Devices;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace CCM.Controllers
{
    public class RpmServiceController : BaseController
    {
       
        // GET: Billing
        //private ApplicationdbContect _db = new ApplicationdbContect();

        // GET: BillingCategories
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult>Index()
        {
            var model = new RPMService();
            var devicetypes = await _db.RPMServices.ToListAsync();
            ViewBag.RPMServices = devicetypes;
            return View(model);

        }
            
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<ActionResult> DeviceMapping(int? PatientId)
        {
            var patient = _db.Patients.FirstOrDefault(x => x.Id == PatientId);
            var devicetypes = await _db.RPMServices.ToListAsync();
            //var devicetypes = _db.RPMServices.Where(x=>x.Patients_Services.FirstOrDefault().PatientId==PatientId).ToList();
            devicetypes = devicetypes != null ? devicetypes :new List<RPMService>();

            return View(devicetypes);
        }  
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<ActionResult> AssignDevice(int? PatientId, int? ServiceId)
        {
            var Devices = await _db.Device.Where(x => x.RPMServiceId == ServiceId &&x.DeviceStatusId==(int)IsActiveStatus.Active && x.IsActive==(int)IsActiveStatus.Active).ToListAsync();
            return View(Devices);

        }

        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Service()
        {
            var model = new RPMService();
            ViewBag.IsAvtiveStatus = Enum.GetValues(typeof(IsActiveStatus)).Cast<IsActiveStatus>().Select(y => new SelectListItem { Text = y.ToString(), Value = ((int)y).ToString() }).ToList();
            var devicetypes = await _db.RPMServices.ToListAsync();
            ViewBag.devicetypes = devicetypes;
            return View(model);

        }


        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<JsonResult> AddService(RPMService obj)
        {

            if (obj.Id == 0)
            {
                var model = new RPMService();
                model.ServiceName = obj.ServiceName;
                model.IsActive = obj.IsActive;
                model.ReasonForDeactivate = obj.ReasonForDeactivate;
                if (Session["UserID"] != null)
                {
                    model.CreatedBy = Session["UserID"].ToString();
                    model.ModifiedBy = Session["UserID"].ToString();
                }
                model.CreatedDate = DateTime.Now;
                model.ModifiedDate = DateTime.Now;
                _db.RPMServices.Add(model);
                var save = await _db.SaveChangesAsync();
                if (save > 0)
                    return Json("added", JsonRequestBehavior.AllowGet);
                return Json("error", JsonRequestBehavior.AllowGet);
            }
            else
            {
                var device = await _db.RPMServices.FirstOrDefaultAsync(x => x.Id == obj.Id);
                device.ServiceName = obj.ServiceName;
                device.IsActive = obj.IsActive;
                device.ReasonForDeactivate = obj.ReasonForDeactivate;
                if (Session["UserID"] != null)
                {
                    device.ModifiedBy = Session["UserID"].ToString();
                }
                device.ModifiedDate = DateTime.Now;
                _db.Entry(device).State = EntityState.Modified;
                var save = await _db.SaveChangesAsync();
                if (save > 0)
                    return Json("updated", JsonRequestBehavior.AllowGet);
                return Json("error", JsonRequestBehavior.AllowGet);
            }

        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Brands()
        {
            var model = new Devices_Brand();
            ViewBag.IsAvtiveStatus = Enum.GetValues(typeof(IsActiveStatus)).Cast<IsActiveStatus>().Select(y => new SelectListItem { Text = y.ToString(), Value = ((int)y).ToString() }).ToList();
            var DevicesBrands = await _db.Devices_Brands.ToListAsync();
            ViewBag.DevicesBrands = DevicesBrands;
            return View(model);

        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<JsonResult> AddBrands(Devices_Brand obj)
        {

            if (obj.Id == 0)
            {
                var exsist = _db.Devices_Brands.FirstOrDefault(x => x.Name == obj.Name);
                if (exsist != null)
                {
                    return Json("exists", JsonRequestBehavior.AllowGet);
                }
                var model = new Devices_Brand();
                model.Name = obj.Name;
                model.Description = obj.Description;
                model.IsActive = obj.IsActive;
                model.CreatedBy =  User.Identity.GetUserId();
                model.CreatedDate = DateTime.Now;
                _db.Devices_Brands.Add(model);
                var save = await _db.SaveChangesAsync();
                if (save > 0)
                    return Json("added", JsonRequestBehavior.AllowGet);
                return Json("error", JsonRequestBehavior.AllowGet);
            }
            else
            {
                var device = await _db.Devices_Brands.FirstOrDefaultAsync(x => x.Id == obj.Id);
                device.Name = obj.Name;
                device.Description = obj.Description;

                device.IsActive = obj.IsActive;
          
                 device.ModifiedBy = User.Identity.GetUserId();

                device.ModifiedDate = DateTime.Now;
                _db.Entry(device).State = EntityState.Modified;
                var save = await _db.SaveChangesAsync();
                if (save > 0)
                    return Json("updated", JsonRequestBehavior.AllowGet);
                return Json("error", JsonRequestBehavior.AllowGet);
            }

        }


        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> DeviceModels()
        {

            var model = new Device_BrandModel();
            ViewBag.IsAvtiveStatus = Enum.GetValues(typeof(IsActiveStatus)).Cast<IsActiveStatus>().Select(y => new SelectListItem { Text = y.ToString(), Value = ((int)y).ToString() }).ToList();
          
            ViewBag.Devices_brands = _db.Devices_Brands.Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() }).ToList();
            ViewBag.Devices_Vendors = _db.Device_Vendors.Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() }).ToList();
            ViewBag.RpmServices = _db.RPMServices.Select(x => new SelectListItem { Text = x.ServiceName, Value = x.Id.ToString() }).ToList();
    
            var Device_BrandModels = await _db.Device_BrandModels.ToListAsync();
            ViewBag.DeviceBrandModels = Device_BrandModels;
            return View(model);

        }


        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<JsonResult> AddDeviceModels(Device_BrandModel obj)
        {

            if (obj.Id == 0)
            {
                var exsist = _db.Device_BrandModels.FirstOrDefault(x => x.Name == obj.Name && x.Devices_BrandId==obj.Devices_BrandId);
                if (exsist != null)
                {
                    return Json("exists", JsonRequestBehavior.AllowGet);
                }
                var model = new Device_BrandModel();
                model.Name = obj.Name;
                model.Devices_BrandId = obj.Devices_BrandId;
                model.Device_VendorId = obj.Device_VendorId;
                model.RPMServiceId = obj.RPMServiceId;
                model.CreatedBy = User.Identity.GetUserId();
                model.CreatedDate = DateTime.Now;
                _db.Device_BrandModels.Add(model);
                var save = await _db.SaveChangesAsync();
                if (save > 0)
                    return Json("added", JsonRequestBehavior.AllowGet);
                return Json("error", JsonRequestBehavior.AllowGet);
            }
            else
            {
                var model = await _db.Device_BrandModels.FirstOrDefaultAsync(x => x.Id == obj.Id);
                model.Name = obj.Name;
                model.Devices_BrandId = obj.Devices_BrandId;
                model.Device_VendorId = obj.Device_VendorId;
                model.RPMServiceId = obj.RPMServiceId;
                model.ModifiedBy = User.Identity.GetUserId();

                model.ModifiedDate = DateTime.Now;
                _db.Entry(model).State = EntityState.Modified;
                var save = await _db.SaveChangesAsync();
                if (save > 0)
                    return Json("updated", JsonRequestBehavior.AllowGet);
                return Json("error", JsonRequestBehavior.AllowGet);
            }

        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Vendors()
        {
            var model = new Device_Vendor();
            ViewBag.IsAvtiveStatus = Enum.GetValues(typeof(IsActiveStatus)).Cast<IsActiveStatus>().Select(y => new SelectListItem { Text = y.ToString(), Value = ((int)y).ToString() }).ToList();
            var Device_Vendor = await _db.Device_Vendors.ToListAsync();
            ViewBag.DevicesBrands = Device_Vendor;
            return View(model);

        }


        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<JsonResult> AddUpdateVendors(Device_Vendor obj)
        {

            if (obj.Id == 0)
            {
                var exsist = _db.Device_Vendors.FirstOrDefault(x => x.Name == obj.Name);
                if (exsist != null)
                {
                    return Json("exists", JsonRequestBehavior.AllowGet);
                }
                var model = new Device_Vendor();
                model.Name = obj.Name;
                model.Address = obj.Address;
                model.Description = obj.Description;
                model.IsActive = obj.IsActive.GetInteger();
                model.PhoneNumber = obj.PhoneNumber;
                model.CreatedDate = DateTime.Now;
                model.CreatedBy = User.Identity.GetUserId();
                _db.Device_Vendors.Add(model);
                var save = await _db.SaveChangesAsync();
                if (save > 0)
                    return Json("added", JsonRequestBehavior.AllowGet);
                return Json("error", JsonRequestBehavior.AllowGet);
            }
            else
            {
                var model = await _db.Device_Vendors.FirstOrDefaultAsync(x => x.Id == obj.Id);
                model.Name = obj.Name;
                model.Address = obj.Address;
                model.Description = obj.Description;
                model.IsActive = obj.IsActive.GetInteger();
                model.PhoneNumber = obj.PhoneNumber;

                model.ModifiedBy = User.Identity.GetUserId();

                model.ModifiedDate = DateTime.Now;
                _db.Entry(model).State = EntityState.Modified;
                var save = await _db.SaveChangesAsync();
                if (save > 0)
                    return Json("updated", JsonRequestBehavior.AllowGet);
                return Json("error", JsonRequestBehavior.AllowGet);
            }

        }

    }


}