using CCM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CCM.Controllers
{
    public class ErrorController : BaseController
    {
        // GET: Error
        public ActionResult exception()
        {
           
            //AlertModel ar = (AlertModel)TempData["AlertMessage"];
            //Session.Add("Alert", ar);
            return View();
        }
    }
}