using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CCM.Models;
using System.Data.Entity;
using System.Net;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using CCM.Models.DataModels;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using System.IO;
using System.Text;
using CCM.Helpers;

namespace CCM.Controllers
{
    public class DeviceTypeController : BaseController
    {
        // GET: Billing
        //private Application_dbContect _db = new Application_dbContect();

        // GET: BillingCategories
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> DeviceTypeIndex()
        {
            var model = new RPMService();
            var devicetypes = await _db.RPMServices.ToListAsync();
            ViewBag.devicetypes = devicetypes;
            return View(model);

        }

        //Test  Running Method************************************************************
        public class GetMeterclass
        {
            public string api_key { get; set; }
        }
        public class GetMeterReadingByIDsclass
        {
            public string api_key { get; set; }
            public string[] meter_ids { get; set; }
       
    }
        public class GetMeterReadingdetailsclass
        {
            public string api_key { get; set; }
            public string date_start { get; set; }
            public string date_end { get; set; }
            public string ingest_date_start { get; set; }
            public string ingest_date_end { get; set; }
            public string[] meter_ids { get; set; }
            public string[] reading_type { get; set; }

        }

        public async Task<ActionResult> Index()
        {
            /// Reading Class Start *****************************************************************************
            GetMeterReadingdetailsclass GetMeterReadingdetailsclassObj = new GetMeterReadingdetailsclass();
            GetMeterReadingdetailsclassObj.api_key = "F1AC8019-3BA0-4DFC-A677-27F155B0A012-1584719321";
            GetMeterReadingdetailsclassObj.date_start = "2020-03-01T00:00:00";
            GetMeterReadingdetailsclassObj.date_end = "2020-04-01T00:00:00";
            GetMeterReadingdetailsclassObj.ingest_date_start = "2020-03-01T00:00:00";
            GetMeterReadingdetailsclassObj.ingest_date_end = "2020-04-01T00:00:00";
            // GetMeterReadingdetailsclassObj.meter_ids = new string[] { "5276711", "5276729", "9999996", "9999997" };
            GetMeterReadingdetailsclassObj.meter_ids = new string[] { "9999997" };
            GetMeterReadingdetailsclassObj.reading_type = new string[] { "blood_glucose", "blood_pressure", "weight" };
            string json = JsonConvert.SerializeObject(GetMeterReadingdetailsclassObj);
            string response = await CCMRequestRestAPI.GetResponsePostRequest("https://api.iglucose.com/readings/", json);
            /// Reading Class Object End  *****************************************************************************



            /// json With Array List Object Start *****************************************************************************
            //GetMeterReadingByIDsclass MeterReadingByIDsclassObj = new GetMeterReadingByIDsclass();
            //MeterReadingByIDsclassObj.api_key = "F1AC8019-3BA0-4DFC-A677-27F155B0A012-1584719321";
            //MeterReadingByIDsclassObj.meter_ids = new string[] { "5276711", "234324", "234" };
            //string jsonWithArrayList = JsonConvert.SerializeObject(MeterReadingByIDsclassObj);

            /// json With Array List Object End  *****************************************************************************

            //  Post Request with Simple  Json Object Start  ******************************************************************
            //GetMeterclass meterobj = new GetMeterclass();
            //meterobj.api_key = "F1AC8019-3BA0-4DFC-A677-27F155B0A012-1584719321";
            //string json = JsonConvert.SerializeObject(meterobj);
           // string response = await CCMRequestRestAPI.GetResponsePostRequest("http://api.iglucose.com/v1/meters/get", json);

           
            // *********
            // For future use ******
            //**********
            //string response = await CCMRequestRestAPI.GetResponse("", "https://reqres.in/api/users?page=2");
            //string response = await CCMRequestRestAPI.GetResponse("https://reqres.in/", "/api/users?page=2");
            // ListBO = JsonConvert.DeserializeObject<List<ListBO>>(Response);
            // ListBO = ListBO.OrderByDescending(x => x.CreatedOn).ToList();
            return View();
        } 


        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> DeviceTypeDetails(int? id)

        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RPMService devicetype = await _db.RPMServices.FindAsync(id);
            if (devicetype == null)
            {
                return HttpNotFound();
            }
            return View(devicetype);
        }

        // GET: RPMService/Create
        // POST: RPMService/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<JsonResult> DeviceTypeCreate(RPMService obj)
        {

            if (obj.Id == 0)
            {
                var model = new RPMService();
                model.ServiceName = obj.ServiceName;
                model.IsActive = obj.IsActive;
                model.ReasonForDeactivate = obj.ReasonForDeactivate;
                if (Session["UserID"] != null) {
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
    }
}