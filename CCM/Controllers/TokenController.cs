using CCM.Models;
using Microsoft.AspNet.Identity;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web.Mvc;
using Twilio.Jwt;
using Twilio.Jwt.Client;

namespace CCM.Controllers
{
    [RequireHttps]

    public class TokenController : BaseController
    {
        //private readonly ApplicationdbContect _db = new ApplicationdbContect();
        public ActionResult Index(string CallerName)
        {
            try
            {
                if (CallerName == "empty")
                {
                    if (User.IsInRole("Liaison"))
                    {
                        var userid = User.Identity.GetUserId();
                        var liasion = _db.Liaisons.AsNoTracking().Where(x => x.UserId == userid).FirstOrDefault();
                        if (liasion != null)
                        {

                            CallerName = liasion.FirstName + liasion.LastName;

                        }
                    }
                    else
                    {
                        CallerName = "Admin";
                    }
                }
            }
            catch (System.Exception ex)
            {
                HelperExtensions.WriteErrorLog(ex);

            }
            ////CallerName = CallerName.Replace("+1", "");
            ////CallerName = "Phonenum" + CallerName;
            //var identity = CallerName;
            try
            {
                if (User.IsInRole("Liaison"))
                {
                    var token = new ClientCapability(
              ConfigurationManager.AppSettings["TwilioAccountSid"],
              ConfigurationManager.AppSettings["TwilioAuthToken"],
              scopes: new HashSet<IScope> { new OutgoingClientScope(
                    ConfigurationManager.AppSettings["TwilioTwimlAppSid"]),new IncomingClientScope(CallerName)  }
          ).ToJwt();
                    //  return Content(token, "application/jwt");
                    return Json(new { token }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    var token = new ClientCapability(
             ConfigurationManager.AppSettings["TwilioAccountSid"],
             ConfigurationManager.AppSettings["TwilioAuthToken"],
             scopes: new HashSet<IScope> { new OutgoingClientScope(
                    ConfigurationManager.AppSettings["TwilioTwimlAppSid"])}
         ).ToJwt();
                    //  return Content(token, "application/jwt");
                    return Json(new { token }, JsonRequestBehavior.AllowGet);
                }

            }
            catch (System.Exception ex)
            {

                return Json(ex.Message + ex.InnerException);
            }



        }
        public ActionResult Tokenforincomming(string CallerName)
        {
            try
            {


                if (CallerName == "empty")
                {
                    if (User.IsInRole("Liaison"))
                    {
                        var userid = User.Identity.GetUserId();
                        var liasion = _db.Liaisons.AsNoTracking().Where(x => x.UserId == userid).FirstOrDefault();
                        if (liasion != null)
                        {

                            CallerName = liasion.FirstName + liasion.LastName;

                        }
                    }
                    else
                    {
                        CallerName = "Admin";
                    }
                }
            }
            catch (System.Exception ex)
            {


            }

            var identity = CallerName;

            var token = new ClientCapability(
                ConfigurationManager.AppSettings["TwilioAccountSid"],
                ConfigurationManager.AppSettings["TwilioAuthToken"],
                scopes: new HashSet<IScope> { new IncomingClientScope(CallerName) }
            ).ToJwt();
            //  return Content(token, "application/jwt");
            return Json(new { identity, token }, JsonRequestBehavior.AllowGet);
        }
    }
}