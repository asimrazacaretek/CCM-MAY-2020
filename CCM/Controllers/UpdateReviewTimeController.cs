using CCM.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CCM.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UpdateReviewTimeController : BaseController
    {
        //private readonly ApplicationdbContect _db = new ApplicationdbContect();
        // GET: UpdateReviewTime
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Index(int? id)
        {


            var ReviewTime = _db.ReviewTimeCcms.Where(x => x.Id == id).FirstOrDefault();


            

            return View(ReviewTime);
        }

        public ActionResult UpdateReviewT(int ?id ,int? time)
        {

            int idd = Convert.ToInt32(id);
            int timee = Convert.ToInt32(time);

            //var UpdateReviewTime = _db.Database.SqlQuery("USP_UpdateReviewTime @UpTIME ,@UpR_ID", new SqlParameter("UpTIME", timee),
            //    new SqlParameter("UpR_ID", idd));
            var uprid = new SqlParameter("@UpR_ID", idd);
            var uptime = new SqlParameter("@UpTIME", timee);
            _db.Database.ExecuteSqlCommand("EXEC USP_UpdateReviewTime @UpTIME,@UpR_ID", uptime, uprid);
            var updatetime = _db.ReviewTimeCcms.Where(x => x.Id == id).FirstOrDefault();
            ViewBag.UpdateReviewTime = updatetime;
            // var ReviewTime = _db.ReviewTimeCcms.Where(x => x.Id == id).FirstOrDefault();
            //return View(updatetime);
            return PartialView("_UpdateTime",updatetime);
        }
    }
      
    }