
using CCM.Helpers;
using CCM.Models;
using CCM.Models.DataModels;
using CCM.Models.DataModels.Devices;
using CCM.Models.ViewModels;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Mvc;
using CCM.Models.ENUMS_;

namespace CCM.Controllers
{
    public class RpmController : BaseController
    {
        PatientBloodSugarReadingViewModel mdoelbloodsuger = new PatientBloodSugarReadingViewModel();
        // GET: Rpm
        //private readonly ApplicationdbContect _db = new ApplicationdbContect();

        public ActionResult Index(int PatientId)
        {
            ViewBag.PatientId = PatientId;
            var Category = "RPM";
            ViewBag.Message = Category;
            ViewBag.BillingReviewId = HelperExtensions.ReviewTimeGet(Category, PatientId, User.Identity.GetUserId(), BillingCodeHelper.RPMBillingCatagoryid);
                ViewBag.RpmServices = _db.Patients.Where(x=>x.Id==PatientId).FirstOrDefault().Patients_Services.Where(x=>x.IsActive==(int)IsActiveStatus.Active).Select(y=>y.RPMService).Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.ServiceName }).ToList();
            ViewBag.CurrentWorkingTimeOnRpm = HelperExtensions.GetTotalReviewTime(PatientId, BillingCodeHelper.RPMBillingCatagoryid);
            return View();
        }
        public ActionResult BloodSugar(int PatientId)
        {
            ViewBag.PatientId = PatientId;
            DateTime baseDate = DateTime.Today;
            var thisMonthStart = baseDate.AddDays(1 - baseDate.Day);
            mdoelbloodsuger.PatientId = PatientId;
            mdoelbloodsuger.StartDate = thisMonthStart;
            mdoelbloodsuger.EndDate = baseDate;
            int RpmServiceId = RPMServices.BloodSugarID;
            mdoelbloodsuger.PatientLogBookList= _db.Database
                       .SqlQuery<PatientBloodSugarReadingLogBook>("GetPatientBloodSugarLogBook @RPMServiceId,@PatientId,@StartDate,@EndDate", 
                       new SqlParameter("RPMServiceId", RpmServiceId),
                       new SqlParameter("PatientId", PatientId),
                       new SqlParameter("StartDate", mdoelbloodsuger.StartDate.ToString()),
                       new SqlParameter("EndDate", mdoelbloodsuger.EndDate.ToString())).ToList();


            mdoelbloodsuger.DeviceReadingPatientDashboard = _db.Database
                      .SqlQuery<PatientBloodSugarReadingDashboard>("GetPatientBloodSugarDashboard @RPMServiceId,@PatientId,@StartDate,@EndDate",
                      new SqlParameter("RPMServiceId", RpmServiceId),
                      new SqlParameter("PatientId", PatientId),
                      new SqlParameter("StartDate", mdoelbloodsuger.StartDate.ToString()),
                      new SqlParameter("EndDate", mdoelbloodsuger.EndDate.ToString())).FirstOrDefault();
   

            return View(mdoelbloodsuger);
        }


        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult BloodSugarAjax(int PatientId, DateTime? StartDate = null, DateTime? EndDate = null)
        {
            if (StartDate !=null && EndDate != null)
            {

            int RpmServiceId = RPMServices.BloodSugarID; 
            mdoelbloodsuger.PatientLogBookList = _db.Database
                       .SqlQuery<PatientBloodSugarReadingLogBook>("GetPatientBloodSugarLogBook @RPMServiceId,@PatientId,@StartDate,@EndDate",
                       new SqlParameter("RPMServiceId", RpmServiceId),
                       new SqlParameter("PatientId", PatientId),
                       new SqlParameter("StartDate", StartDate.ToString()),
                       new SqlParameter("EndDate", EndDate.ToString())).ToList();


            mdoelbloodsuger.DeviceReadingPatientDashboard = _db.Database
                      .SqlQuery<PatientBloodSugarReadingDashboard>("GetPatientBloodSugarDashboard @RPMServiceId,@PatientId,@StartDate,@EndDate",
                      new SqlParameter("RPMServiceId", RpmServiceId),
                      new SqlParameter("PatientId", PatientId),
                      new SqlParameter("StartDate", StartDate.ToString()),
                      new SqlParameter("EndDate", EndDate.ToString())).FirstOrDefault();
            }
            else
            {
                ViewBag.PatientId = PatientId;
                DateTime baseDate = DateTime.Today;
                var thisMonthStart = baseDate.AddDays(1 - baseDate.Day);
                mdoelbloodsuger.PatientId = PatientId;
                mdoelbloodsuger.StartDate = thisMonthStart;
                mdoelbloodsuger.EndDate = baseDate;
                int RpmServiceId = RPMServices.BloodSugarID;
                mdoelbloodsuger.PatientLogBookList = _db.Database
                           .SqlQuery<PatientBloodSugarReadingLogBook>("GetPatientBloodSugarLogBook @RPMServiceId,@PatientId,@StartDate,@EndDate",
                           new SqlParameter("RPMServiceId", RpmServiceId),
                           new SqlParameter("PatientId", PatientId),
                           new SqlParameter("StartDate", mdoelbloodsuger.StartDate.ToString()),
                           new SqlParameter("EndDate", mdoelbloodsuger.EndDate.ToString())).ToList();


                mdoelbloodsuger.DeviceReadingPatientDashboard = _db.Database
                          .SqlQuery<PatientBloodSugarReadingDashboard>("GetPatientBloodSugarDashboard @RPMServiceId,@PatientId,@StartDate,@EndDate",
                          new SqlParameter("RPMServiceId", RpmServiceId),
                          new SqlParameter("PatientId", PatientId),
                          new SqlParameter("StartDate", mdoelbloodsuger.StartDate.ToString()),
                          new SqlParameter("EndDate", mdoelbloodsuger.EndDate.ToString())).FirstOrDefault();

            }
            // return Json(mdoelbloodsuger, JsonRequestBehavior.AllowGet);
            return View(mdoelbloodsuger);
            //return new JsonResult
            //{
            //    JsonRequestBehavior = JsonRequestBehavior.AllowGet,
            //    Data = new { PatientLogBookList = mdoelbloodsuger.PatientLogBookList, DeviceReadingPatientDashboard = mdoelbloodsuger.DeviceReadingPatientDashboard, RpmServiceId = RpmServiceId }
            //};



        }


        public ActionResult patients()
        {
            var RpmEnrolledPatients = _db.Patients_BillingCategories.Where(p => p.Status == true && p.BillingCategoryId == BillingCodeHelper.RPMBillingCatagoryid).Select(p=>p.Patients).ToList();
            return View();
        }


        public ActionResult RpmPatientsList(RpmQueFilterViewModel obj)

        {
            ViewBag.AvalilabeDevices = _db.Device.Where(x => x.IsActive == (int)IsActiveStatus.Active && x.DeviceStatusId == (int)DeviceStatus.ReadyToMap).ToList();



            var RpmEnrolledPatients = new List<RpmPatientsViewModel>();


            if (obj == null)
            {
                int zero = Convert.ToInt32("0");
                
                RpmEnrolledPatients = _db.Database.SqlQuery<RpmPatientsViewModel>("GetAllPatientForDeviceMapping @Devices_BrandId,@Device_BrandModelId,@VendorId,@RPMServiceId,@DeviceStatusId,@SerialNumber",
                            new SqlParameter("@Devices_BrandId", zero), new SqlParameter("@Device_BrandModelId", zero), new SqlParameter("@VendorId", zero), new SqlParameter("@RPMServiceId", zero), new SqlParameter("@DeviceStatusId", zero), new SqlParameter("@SerialNumber", "")).ToList();
                return View(RpmEnrolledPatients);
            }
            obj.BrandId = obj.BrandId != null ? obj.BrandId : 0;
            obj.DeviceCurrentStatus = obj.DeviceCurrentStatus != null ? obj.DeviceCurrentStatus : 0;
            obj.ModelId = obj.ModelId != null ? obj.ModelId : 0;
            obj.RpmServiceId = obj.RpmServiceId != null ? obj.RpmServiceId : 0;
            obj.SerialNumber = obj.SerialNumber != null ? obj.SerialNumber : string.Empty;
            obj.VendorId = obj.VendorId != null ? obj.VendorId : 0;



            RpmEnrolledPatients = _db.Database.SqlQuery<RpmPatientsViewModel>("GetAllPatientForDeviceMapping @Devices_BrandId,@Device_BrandModelId,@VendorId,@RPMServiceId,@DeviceStatusId,@SerialNumber",
                            new SqlParameter("@Devices_BrandId", Convert.ToInt32(obj.BrandId.ToString())), new SqlParameter("@Device_BrandModelId", Convert.ToInt32(obj.ModelId.ToString())), new SqlParameter("@VendorId", Convert.ToInt32(obj.VendorId.ToString())), new SqlParameter("@RPMServiceId", Convert.ToInt32(obj.RpmServiceId.ToString())), new SqlParameter("@DeviceStatusId", Convert.ToInt32(obj.DeviceCurrentStatus.ToString())), new SqlParameter("@SerialNumber",obj.SerialNumber)).ToList();
            //RpmEnrolledPatients = _db.Database.SqlQuery<RpmPatientsViewModel>("GetAllPatientForDeviceMapping @Devices_BrandId,@Device_BrandModelId,@VendorId,@RPMServiceId,@DeviceStatusId,@SerialNumber",
            //               new SqlParameter("@Devices_BrandId", Convert.ToInt32(obj.BrandId.ToString())), new SqlParameter("@Device_BrandModelId", Convert.ToInt32(obj.ModelId.ToString())), new SqlParameter("@VendorId", Convert.ToInt32(obj.VendorId.ToString())), new SqlParameter("@RPMServiceId", Convert.ToInt32(obj.RpmServiceId.ToString())), new SqlParameter("@DeviceStatusId", Convert.ToInt32(obj.DeviceCurrentStatus).ToString()), new SqlParameter("@SerialNumber", obj.DeviceCurrentStatus.ToString())).ToList();


            return View(RpmEnrolledPatients);
        }

        //public ActionResult RpmPatientsList(RpmQueFilterViewModel obj)

        //{
        //    ViewBag.AvalilabeDevices = _db.Device.Where(x => x.IsActive == (int)IsActiveStatus.Active && x.DeviceStatusId == (int)DeviceStatus.ReadyToMap).ToList();



        //    var RpmEnrolledPatients = new List<Patient>();


        //        RpmEnrolledPatients = _db.Patients_BillingCategories.Where(p => p.Status == true && p.BillingCategoryId == BillingCodeHelper.RPMBillingCatagoryid).Select(p => p.Patients).Distinct().ToList();
        //    if (obj == null)
        //    {

        //        return View(RpmEnrolledPatients);
        //    }
        //    else
        //    {
        //        if (obj.BrandId != null)
        //        {
        //            foreach (var item in RpmEnrolledPatients)
        //            {
        //               item.Patients_Services.RemoveAll(x => x.Device.Devices_BrandId != obj.BrandId);
        //            }

        //        }
        //        else if (obj.VendorId != null)
        //        {
        //            foreach (var item in RpmEnrolledPatients)
        //            {
        //                item.Patients_Services.RemoveAll(x => x.Device.VendorId != obj.VendorId);
        //            }
        //        }
        //        else if (obj.ModelId != null)
        //        {
        //            foreach (var item in RpmEnrolledPatients)
        //            {
        //                item.Patients_Services.RemoveAll(x => x.Device.Device_BrandModelId != obj.ModelId);
        //            }
        //        }

        //        else if (obj.SerialNumber != null)
        //        {
        //            foreach (var item in RpmEnrolledPatients)
        //            {
        //                item.Patients_Services.RemoveAll(x => !x.Device.SerialNumber.Contains(obj.SerialNumber));
        //            }
        //        }
        //        else if (obj.RpmServiceId != null)
        //        {
        //            foreach (var item in RpmEnrolledPatients)
        //            {
        //                item.Patients_Services.RemoveAll(x => x.Device.RPMServiceId!=obj.RpmServiceId);
        //            }
        //        }
        //        else if (obj.DeviceCurrentStatus != null)
        //        {
        //            foreach (var item in RpmEnrolledPatients)
        //            {
        //                item.Patients_Services.RemoveAll(x => x.Device.DeviceStatusId != obj.DeviceCurrentStatus);
        //            }
        //        }
        //    }


        //    return View(RpmEnrolledPatients);
        //}
        public ActionResult Filters()
        {
            ViewBag.IsAvtiveStatus = Enum.GetValues(typeof(IsActiveStatus)).Cast<IsActiveStatus>().Select(y => new SelectListItem { Text = y.ToString(), Value = ((int)y).ToString() }).ToList();
            ViewBag.DeviceStatus = Enum.GetValues(typeof(DeviceStatus)).Cast<DeviceStatus>().Select(y => new SelectListItem { Text = y.ToString(), Value = ((int)y).ToString() }).ToList();
            ViewBag.ModelNumber = _db.Device_BrandModels.Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() }).ToList();
            ViewBag.Vendors = _db.Device_Vendors.Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() }).ToList();
            ViewBag.Device_brands = _db.Devices_Brands.Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Name }).ToList();
            RpmQueFilterViewModel model = new RpmQueFilterViewModel();
            return View(model);
        }

        public JsonResult AttachDeviceToPatient(int? PatientId, int? serviceid,int? DeviceId,string Comments="",string attachments="")
        {
            List<Rpm_DeviceMappingAttachmentsViewModel> Attachments = JsonConvert.DeserializeObject<List<Rpm_DeviceMappingAttachmentsViewModel>>(attachments);

            var patient = _db.Patients.FirstOrDefault(x => x.Id == PatientId);
            if (patient != null)
            {
              var service=  patient.Patients_Services.FirstOrDefault(x => x.RPMServiceId == serviceid);


                if (service != null)
                {
                    if (service.IsAssigned == (int)IsActiveStatus.DeActive)
                    {

                        service.DeviceId = DeviceId;
                        service.CommentsOnAssigement = Comments;
                        service.UpdatedBy = User.Identity.GetUserId();
                        service.IsActive = (int)IsActiveStatus.Active;
                        service.IsAssigned = (int)IsActiveStatus.Active;
                        service.AssignedDate = DateTime.Now;

                        service.UpdatedOn = DateTime.Now;
                        _db.Entry(service).State = EntityState.Modified; 
                        var save = _db.SaveChanges();
                        if (save > 0)
                        {
                            var device = _db.Device.FirstOrDefault(x => x.Id == DeviceId);
                            device.DeviceStatusId = (int)DeviceStatus.AlreadyMapped;
                            _db.Entry(device).State = EntityState.Modified;
                            var saveDevice = _db.SaveChanges();
                            if (saveDevice > 0)
                            {
                                var History = new DeviceMappingHistory();
                                History.DevicetId = DeviceId;
                                History.PatientId = PatientId;
                                History.Type = "Attach";
                                History.RPMServiceId = serviceid;
                                History.Message = Comments;
                                History.DatePerformed = DateTime.Now;
                                History.CreatedBy = User.Identity.GetUserId();
                                _db.DeviceMappingHistory.Add(History);
                                var saveHistory = _db.SaveChanges();
                                if (saveHistory > 0)
                                {
                                    foreach (var item in Attachments)
                                    {
                                        var Attachment = new Rpm_DeviceMappingAttachments();
                                        Attachment.DeviceMappingHistoryId = History.Id;
                                        Attachment.Image = item.ImageData;
                                        Attachment.PatientId = PatientId;
                                        _db.Rpm_DeviceMappingAttachments.Add(Attachment);
                                        _db.SaveChanges();
                                    }
                                    return Json("saved", JsonRequestBehavior.AllowGet);
                                }
                                return Json("error", JsonRequestBehavior.AllowGet);
                            }

                            
                            return Json("error", JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            return Json("error", JsonRequestBehavior.AllowGet);
                        }
                    }
                    return Json("alreadymapped", JsonRequestBehavior.AllowGet);

                }
                return Json("error", JsonRequestBehavior.AllowGet);
            }
            return Json("error",JsonRequestBehavior.AllowGet);
        }
        public JsonResult DetachDeviceToPatient(int? PatientId, int? serviceid,int? DeviceId,string Comments="",string attachments="")
        {
            List<Rpm_DeviceMappingAttachmentsViewModel> Attachments = JsonConvert.DeserializeObject<List<Rpm_DeviceMappingAttachmentsViewModel>>(attachments);

            var patient = _db.Patients.FirstOrDefault(x => x.Id == PatientId);
            if (patient != null)
            {
              var service=  patient.Patients_Services.FirstOrDefault(x => x.RPMServiceId == serviceid &&x.IsActive==(int)IsActiveStatus.Active);
                if (service != null)
                {
                    if (service.Device != null)
                    {
                        service.UpdatedBy = User.Identity.GetUserId();
                        service.IsActive = (int)IsActiveStatus.Active;
                        service.IsAssigned = (int)IsActiveStatus.DeActive;
                        service.UpdatedOn = DateTime.Now;
                        _db.Entry(service).State = EntityState.Modified; 
                        var save = _db.SaveChanges();
                        if (save > 0)
                        {
                            var device = _db.Device.FirstOrDefault(x => x.Id == DeviceId);
                            device.DeviceStatusId = (int)DeviceStatus.ReadyToMap;
                           _db.Entry(device).State = EntityState.Modified;
                            var DetatchDevice = _db.SaveChanges();

                            if (DetatchDevice > 0)
                            {
                                var History = new DeviceMappingHistory();
                                History.DevicetId = DeviceId;
                                History.PatientId = PatientId;
                                History.Message = Comments;
                                History.Type = "Detached";
                                History.RPMServiceId = serviceid;
                                History.DatePerformed = DateTime.Now;
                                History.CreatedBy = User.Identity.GetUserId();
                                _db.DeviceMappingHistory.Add(History);
                              var saveHistory=  _db.SaveChanges();
                                if (saveHistory > 0)
                                {
                                    foreach (var item in Attachments)
                                    {
                                        var Attachment = new Rpm_DeviceMappingAttachments();
                                        Attachment.DeviceMappingHistoryId = History.Id;
                                        Attachment.Image = item.ImageData;
                                        Attachment.PatientId =  PatientId;
                                        _db.Rpm_DeviceMappingAttachments.Add(Attachment);
                                        _db.SaveChanges();
                                    }
                                    return Json("saved", JsonRequestBehavior.AllowGet);
                                }
                                return Json("error", JsonRequestBehavior.AllowGet);
                            }
                            return Json("error", JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            return Json("error", JsonRequestBehavior.AllowGet);
                        }
                    }
                    return Json("alreadymapped", JsonRequestBehavior.AllowGet);

                }
                return Json("error", JsonRequestBehavior.AllowGet);
            }
            return Json("error",JsonRequestBehavior.AllowGet);
        }

        public ActionResult LoadDetails(int? PatientId,int? ServiceId)
        {
            var Patient = _db.Patients.Where(x => x.Id == PatientId).FirstOrDefault();
            var Service = Patient.Patients_Services.Where(x => x.RPMServiceId == ServiceId && x.IsActive == (int)IsActiveStatus.Active).FirstOrDefault();
            ViewBag.Patient = Patient;
            ViewBag.Service = Service;
            var MappingHistory = _db.DeviceMappingHistory.Where(x => x.PatientId == PatientId && x.RPMServiceId == ServiceId).ToList();
            ViewBag.Attachments = _db.Rpm_DeviceMappingAttachments.Where(x => x.PatientId == PatientId).ToList();
            return View(MappingHistory);
        }
    }
}