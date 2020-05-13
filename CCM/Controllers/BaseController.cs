using CCM.Helpers;
using CCM.Models;
using CCM.Models.BackGroundJob;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CCM.Controllers
{
    public class BaseController : Controller
    {
        protected readonly ApplicationdbContect _db = new ApplicationdbContect();

        // log4net declaration
        protected static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);


        public BaseController()
        {
            CategoryCycleStatusHelper.User = User;
        }
        protected string GetUserId()
        {
            return User.Identity.GetUserId();
        }
    }
}