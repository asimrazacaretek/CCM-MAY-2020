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
using System.Data.SqlClient;
using CCM.Models.ViewModels;

namespace CCM.Controllers
{
    public class PatientDeviceReadingController : BaseController
    {
        PatientDeviceReading model = new PatientDeviceReading();

           // GET: PatientDeviceReading
           [Authorize(Roles = "Admin")]
        public ActionResult Index()
        {
            //model.PatientReadingList=_db.Database.SqlQuery<PatientDeviceReadingFullBO>("GetPatientReadingsApiData @PatientId,@RPMServiceId,@DateSearchStart,@DateSearchEnd",
            //                   new SqlParameter("@PatientId", 0), new SqlParameter("@RPMServiceId", 0),
            //                   new SqlParameter("@DateSearchStart", String.Empty), new SqlParameter("@DateSearchEnd", String.Empty)
            //                   ).ToList();
            

            int DefaultValue = Convert.ToInt32("0");
            model.PatientReadingList = _db.Database.SqlQuery<PatientDeviceReadingFullBO>("GetPatientReadingsApiData @PatientId, @RPMServiceId",
                          new SqlParameter("@PatientId", DefaultValue), new SqlParameter("@RPMServiceId", DefaultValue)).ToList();
            return View(model);
        }

        [Authorize(Roles = "Admin")]
        public ActionResult IndexFiler(PatientDeviceReading PatientDeviceReadingObj)
        {
           
            return View(PatientDeviceReadingObj);

        }
    }
}