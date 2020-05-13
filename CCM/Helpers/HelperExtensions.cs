using CCM.Helpers;
using CCM.Models;
using CCM.Models.DataModels;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Drawing;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Text;
using System.Web;
using System.Web.Configuration;
using System.Web.Hosting;
using System.Web.Mvc;
using System.Web.Routing;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Rest.Api.V2010.Account.AvailablePhoneNumberCountry;

namespace CCM
{
    public static class HelperExtensions
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static OutgoingCallerIdResource OutgoingCallerIdResourceFromTwiliopathSid(string twiliopathSid)
        {
            TwilioClient.Init(ConfigurationManager.AppSettings["TwilioAccountSid"], ConfigurationManager.AppSettings["TwilioAuthToken"]);

            return OutgoingCallerIdResource.Fetch(
                pathSid: twiliopathSid
            );
        }

        private static readonly ApplicationdbContect Db = new ApplicationdbContect();

        public static string apiurl = WebConfigurationManager.AppSettings["apiUrl"];
        public static string apiSurvey = WebConfigurationManager.AppSettings["apiSurvey"];
        public static string apiSurveyType = WebConfigurationManager.AppSettings["apiSurveyTypes"];
        public static string apiSurveySections = WebConfigurationManager.AppSettings["apiSurveySection"];
        public static string apiGetSurveyQuestionSurveyId = WebConfigurationManager.AppSettings["apiGetSurveyQuestionSurveyId"];
        public static string apiGetSurveyQuestionSequenceMappingBySurvey = WebConfigurationManager.AppSettings["apiGetSurveyQuestionSequenceMappingBySurveyId"];
        public static string PatientSurvey = WebConfigurationManager.AppSettings["PatientSurvey"];
        public static string GetSurveyByPatientId = WebConfigurationManager.AppSettings["GetSurveyByPatientId"];

        public static string GetSurveyByPatientIdList = WebConfigurationManager.AppSettings["GetSurveyByPatientIdList"];
        public static string GetAllPatientsSurveysList = WebConfigurationManager.AppSettings["GetAllPatientsSurveysList"];
        public static string apiSurveyQuestions = WebConfigurationManager.AppSettings["apiSurveyQuestions"];
        public static string apisurveyQuestionSequenceMapping = WebConfigurationManager.AppSettings["apisurveyQuestionSequenceMapping"];
        public static string DeleteQuestionSequenceMapping = WebConfigurationManager.AppSettings["DeleteQuestionSequenceMapping"];
        public static string GetSurveyQuestionFlowBySurveyId = WebConfigurationManager.AppSettings["GetSurveyQuestionFlowBySurveyId"];
        public static string apiSurveyNextNodeByFlow = WebConfigurationManager.AppSettings["apiSurveyNextNodeByFlow"];
        public static string apiAllSurveysList = WebConfigurationManager.AppSettings["apiAllSurveysList"];
        public static string apiSurveyTypesBySurveyId = WebConfigurationManager.AppSettings["apiSurveyTypesBySurveyId"];
        public static string apiSurveySectionsBySurveyAndType = WebConfigurationManager.AppSettings["apiSurveySectionsBySurveyAndType"];
        public static string Encrypt(string clearText)
        {
            string EncryptionKey = "CCMHEALTH08102019";
            byte[] clearBytes = Encoding.Unicode.GetBytes(clearText);
            using (Aes encryptor = Aes.Create())
            {
                // here store hex value in byte array 
                // Refc2898DeruveBytes requried two paramertres one password and second salt 
                // hex convert to dec value
                // byte array store 8 bit value requied
                //0x49=73,0x76=118, 0x20=32,0x4d=77,0x65=101,0x64=100,0x76=118,

                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(clearBytes, 0, clearBytes.Length);
                        cs.Close();
                    }
                    clearText = Convert.ToBase64String(ms.ToArray());
                }
            }
            return clearText;
        }
        public static string Decrypt(string cipherText)
        {

            string EncryptionKey = "CCMHEALTH08102019";
            cipherText = cipherText.Replace(" ", "+");
            int mod4 = cipherText.Length % 4;
            if (mod4 > 0)
            {
                cipherText += new string('=', 4 - mod4);
            }
            byte[] cipherBytes = Convert.FromBase64String(cipherText);
            // byte[] cipherBytes = Encoding.ASCII.GetBytes(cipherText);
            using (Aes encryptor = Aes.Create())
            {

                // here store hex value in byte array 
                // Refc2898DeruveBytes requried two paramertres one password and second salt 
                // hex convert to dec value
                // byte array store 8 bit value requied
                //0x49=73,0x76=118, 0x20=32,0x4d=77,0x65=101,0x64=100,0x76=118,


                //Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 73, 118, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(cipherBytes, 0, cipherBytes.Length);
                        cs.Close();
                    }
                    cipherText = Encoding.Unicode.GetString(ms.ToArray());
                }
            }
            return cipherText;
        }

        public static ExpandoObject ToExpando(this object anonymousObject)
        {
            IDictionary<string, object> anonymousDictionary = new RouteValueDictionary(anonymousObject);
            IDictionary<string, object> expando = new ExpandoObject();
            foreach (var item in anonymousDictionary)
                expando.Add(item);
            return (ExpandoObject)expando;
        }

        public static string fileDirectoryForPatientConcent()
        {
            string currentYear = DateTime.Now.Year.ToString();
            string currentMonth = DateTime.Now.Month.ToString();
            string currentDate = DateTime.Now.ToString("MMddyyyy");

            string directory = HttpContext.Current.Server.MapPath("~/Patients_Concent_Images/");
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
            string subdirctory = HttpContext.Current.Server.MapPath("~/Patients_Concent_Images/" + currentYear + "/");
            if (!Directory.Exists(subdirctory))
            {
                Directory.CreateDirectory(subdirctory);
            }
            string subdirctoryMonth = HttpContext.Current.Server.MapPath("~/Patients_Concent_Images/" + currentYear + "/" + currentMonth + "/");
            if (!Directory.Exists(subdirctoryMonth))
            {
                Directory.CreateDirectory(subdirctoryMonth);
            }
            string subdirctoryDate = HttpContext.Current.Server.MapPath("~/Patients_Concent_Images/" + currentYear + "/" + currentMonth + "/" + currentDate + "CCM/");
            if (!Directory.Exists(subdirctoryDate))
            {
                Directory.CreateDirectory(subdirctoryDate);
            }
            return subdirctoryDate;
        }
        public static string fileDirectory()
        {
            string currentYear = DateTime.Now.Year.ToString();
            string currentMonth = DateTime.Now.Month.ToString();
            string currentDate = DateTime.Now.ToString("MMddyyyy");

            string directory = HttpContext.Current.Server.MapPath("~/Pat_Images/");
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
            string subdirctory = HttpContext.Current.Server.MapPath("~/Pat_Images/" + currentYear + "/");
            if (!Directory.Exists(subdirctory))
            {
                Directory.CreateDirectory(subdirctory);
            }
            string subdirctoryMonth = HttpContext.Current.Server.MapPath("~/Pat_Images/" + currentYear + "/" + currentMonth + "/");
            if (!Directory.Exists(subdirctoryMonth))
            {
                Directory.CreateDirectory(subdirctoryMonth);
            }
            string subdirctoryDate = HttpContext.Current.Server.MapPath("~/Pat_Images/" + currentYear + "/" + currentMonth + "/" + currentDate + "CCM/");
            if (!Directory.Exists(subdirctoryDate))
            {
                Directory.CreateDirectory(subdirctoryDate);
            }
            return subdirctoryDate;
        }
        public static string fileDirectory(DateTime date)
        {
            string currentYear = date.Year.ToString();
            string currentMonth = date.Month.ToString();
            string currentDate = date.ToString("MMddyyyy");

            string directory = HttpContext.Current.Server.MapPath("~/Pat_Images/");
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
            string subdirctory = HttpContext.Current.Server.MapPath("~/Pat_Images/" + currentYear + "/");
            if (!Directory.Exists(subdirctory))
            {
                Directory.CreateDirectory(subdirctory);
            }
            string subdirctoryMonth = HttpContext.Current.Server.MapPath("~/Pat_Images/" + currentYear + "/" + currentMonth + "/");
            if (!Directory.Exists(subdirctoryMonth))
            {
                Directory.CreateDirectory(subdirctoryMonth);
            }
            string subdirctoryDate = HttpContext.Current.Server.MapPath("~/Pat_Images/" + currentYear + "/" + currentMonth + "/" + currentDate + "CCM/");
            if (!Directory.Exists(subdirctoryDate))
            {
                Directory.CreateDirectory(subdirctoryDate);
            }
            return subdirctoryDate;
        }
        public static string convertbase64(string path)
        {
            string base64String = "";
            try
            {
                using (Image image = Image.FromFile(path))
                {
                    using (MemoryStream m = new MemoryStream())
                    {
                        image.Save(m, image.RawFormat);
                        byte[] imageBytes = m.ToArray();

                        // Convert byte[] to Base64 String
                        base64String = Convert.ToBase64String(imageBytes);
                    }
                }

            }
            catch (Exception ex) { base64String = ex.Message.ToString(); }
            return base64String;
        }

        public static void DuckCopyShallow(this Object dst, object src)
        {
            var srcT = src.GetType();
            var dstT = dst.GetType();
            foreach (var f in srcT.GetFields())
            {
                var dstF = dstT.GetField(f.Name);
                if (dstF == null)
                    continue;
                dstF.SetValue(dst, f.GetValue(src));
            }

            foreach (var f in srcT.GetProperties())
            {
                var dstF = dstT.GetProperty(f.Name);
                if (dstF == null || dstF.Name == "Id")
                    continue;

                dstF.SetValue(dst, f.GetValue(src, null), null);
            }
        }
        public static void WriteErrorLog(System.Exception ex)
        {
            try
            {


                string webPageName = Path.GetFileName(HttpContext.Current.Request.Path);
                string errorLogFilename = "ErrorLog_" + DateTime.Now.ToString("dd-MM-yyyy") + ".txt";
                string path = HostingEnvironment.MapPath("~/ErrorLogFiles/" + errorLogFilename);
                if (System.IO.File.Exists(path))
                {
                    using (StreamWriter stwriter = new StreamWriter(path, true))
                    {
                        stwriter.WriteLine("-------------------Error Log Start-----------as on " + DateTime.Now.ToString("hh:mm tt"));
                        stwriter.WriteLine("WebPage Name :" + webPageName);
                        stwriter.WriteLine("Message:" + ex.ToString());
                        if (ex.InnerException != null)
                        {
                            stwriter.WriteLine("Inner exception: " + ex.InnerException.ToString());
                        }
                        stwriter.WriteLine("-------------------End----------------------------");
                    }
                }
                else
                {
                    StreamWriter stwriter = System.IO.File.CreateText(path);
                    stwriter.WriteLine("-------------------Error Log Start-----------as on " + DateTime.Now.ToString("hh:mm tt"));
                    stwriter.WriteLine("WebPage Name :" + webPageName);
                    stwriter.WriteLine("Message: " + ex.ToString());
                    if (ex.InnerException != null)
                    {
                        stwriter.WriteLine("Inner exception: " + ex.InnerException.ToString());
                    }
                    stwriter.WriteLine("-------------------End----------------------------");
                    stwriter.Close();
                }
            }
            catch (Exception ex1)
            {


            }
        }
        public static string GetFirstName(this IIdentity user)
        {
            var claimsUser = (ClaimsIdentity)user;
            return claimsUser.Claims.FirstOrDefault(c => c.Type == "FirstName")?.Value;
        }
        //public static string GetUserID(this IIdentity user)
        //{
        //    var claimsUser = (ClaimsIdentity)user;
        //    return claimsUser.Claims.FirstOrDefault(c => c.Type == "Id")?.Value;
        //}

        public static string GetLastName(this IIdentity user)
        {
            var claimsUser = (ClaimsIdentity)user;
            return claimsUser.Claims.FirstOrDefault(c => c.Type == "LastName")?.Value;
        }
        public static string GetUserNamebyID(string userid)
        {
            try
            {
                using (ApplicationdbContect Db = new ApplicationdbContect())
                {
                    return Db.Users.Find(userid).FirstName + " " + Db.Users.Find(userid).LastName;
                }
            }
            catch (Exception ex)
            {

                return "";
            }


        }
        public static string GetUserRolebyID(string userid)
        {
            try
            {
                using (ApplicationdbContect Db = new ApplicationdbContect())
                {
                    var curruser = Db.Users.Find(userid);
                    if (curruser.Role == "Liaison")
                    {
                        var currentliaison = Db.Liaisons.Where(x => x.Id == curruser.CCMid).FirstOrDefault();
                        if (currentliaison.IsTranslator == false)
                        {
                            return "Counselor";
                        }
                        else
                        {
                            return "Translator";
                        }


                    }
                    else
                    {
                        return curruser.Role;
                    }

                }
            }
            catch (Exception ex)
            {

                return "";
            }


        }
        public static bool isTranslator(string userid)
        {
            try
            {
                using (ApplicationdbContect Db = new ApplicationdbContect())
                {
                    var curruser = Db.Users.Find(userid);
                    if (curruser.Role == "Liaison")
                    {
                        var liasion = Db.Liaisons.Where(x => x.Id == curruser.CCMid).FirstOrDefault();
                        if (liasion != null)
                        {
                            return liasion.IsTranslator;
                        }
                        else
                        {
                            return false;
                        }
                        //return Db.Liaisons.Where(x => x.Id == curruser.CCMid).FirstOrDefault().IsTranslator;

                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {

                return false;
            }
        }
        //public static int GetPatientCycle(Patient patient)
        //{
        //    if (patient == null)
        //    {
        //        return 0;
        //    }
        //    if (patient.CCMEnrolledOn == null)
        //    {
        //        //patient not enrolled in any category
        //        return 0;
        //    }
        //    else
        //    {
        //        if (patient.EnrollmentSubStatus == "Active Enrolled")//patient is enrolled in CMM
        //        {
        //            return (((DateTime.Now.Year - patient.CCMEnrolledOn.Value.Year) * 12) + DateTime.Now.Month - patient.CCMEnrolledOn.Value.Month) + 1;

        //        }
        //        else
        //        {
        //            using (ApplicationdbContect Db = new ApplicationdbContect())
        //            {
        //                var lastcycle = Db.CCMCycleStatuses.Where(x => x.PatientId == patient.Id).AsNoTracking().OrderByDescending(o => o.Id).FirstOrDefault();
        //                if (lastcycle != null)
        //                {
        //                    return lastcycle.Cycle;
        //                }
        //                else
        //                {
        //                    return 0;
        //                }
        //            }
        //        }

        //    }

        //}

        //public static int GetCategoryPatientCycle(int patientid, int? BillingcategoryId)
        //{
        //    try
        //    {
        //        using (ApplicationdbContect Db = new ApplicationdbContect())
        //        {
        //            var patient = Db.Patients.Where(x => x.Id == patientid).AsNoTracking().FirstOrDefault();
        //            if (patient.CCMEnrolledOn == null)
        //            {
        //                return 0;
        //            }
        //            else
        //            {
        //                if (patient.EnrollmentSubStatus == "Active Enrolled")
        //                {
        //                    if (BillingcategoryId != null)
        //                    {
        //                        var isonetime = BillingCodeHelper.IsOneTimeBillingPeriod((int)BillingcategoryId);
        //                        if (isonetime == true)
        //                        {
        //                            return 1;
        //                        }
        //                    }
        //                    return (((DateTime.Now.Year - patient.CCMEnrolledOn.Value.Year) * 12) + DateTime.Now.Month - patient.CCMEnrolledOn.Value.Month) + 1;
        //                }
        //                else
        //                {
        //                    var lastcycle = Db.CCMCycleStatuses.Where(x => x.PatientId == patient.Id).OrderByDescending(o => o.Id).AsNoTracking().FirstOrDefault();
        //                    if (lastcycle != null)
        //                    {
        //                        return lastcycle.Cycle;
        //                    }
        //                    else
        //                    {
        //                        return 0;
        //                    }
        //                }

        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {

        //        return 0;
        //    }


        //}
        //public static int GetPatientCycle(int patientid)
        //{
        //    try
        //    {

        //        using (ApplicationdbContect Db = new ApplicationdbContect())
        //        {
        //            var patient = Db.Patients.Where(x => x.Id == patientid).AsNoTracking().FirstOrDefault();
        //            if (patient != null)
        //            {
        //                if (patient.CCMEnrolledOn == null)
        //                {
        //                    return 0;
        //                }
        //                else
        //                {
        //                    if (patient.EnrollmentSubStatus == "Active Enrolled")
        //                    {
        //                        return (((DateTime.Now.Year - patient.CCMEnrolledOn.Value.Year) * 12) + DateTime.Now.Month - patient.CCMEnrolledOn.Value.Month) + 1;
        //                    }
        //                    else
        //                    {
        //                        var lastcycle = Db.CCMCycleStatuses.Where(x => x.PatientId == patient.Id).OrderByDescending(o => o.Id).AsNoTracking().FirstOrDefault();
        //                        if (lastcycle != null)
        //                        {
        //                            return lastcycle.Cycle;
        //                        }
        //                        else
        //                        {
        //                            return 0;
        //                        }
        //                    }

        //                }
        //            }
        //            else
        //            {
        //                return 0;
        //            }

        //        }
        //    }
        //    catch (Exception ex)
        //    {

        //        return 0;
        //    }


        //}
        public static string GetStatusRedirectionbyUser(string userid)
        {
            using (ApplicationdbContect Db = new ApplicationdbContect())
            {
                var user = Db.Users.Find(userid);
                return user.Role == "QAQC" ? "Clinical Sign-Off" : user.Role == "Liaison" ? "Enrolled" : user.Role == "PhysiciansGroup" ? "Enrolled" : user.Role == "Admin" ? "Enrolled" : user.Role == "Physician" ? "Enrolled" : "Enrolled";
            }
        }
        public static bool isAllowedforEditingorAdd(int patientId, int Cycle, string Userid)
        {
            using (ApplicationdbContect Db = new ApplicationdbContect())
            {
                try
                {

                    var user = Db.Users.Find(Userid);
                    if (user.Role == "Admin")
                    {
                        return true;
                    }
                }
                catch (Exception ex)
                {


                }

                var CCMCycleStatus = Db.CCMCycleStatuses.Where(x => x.PatientId == patientId && x.Cycle == Cycle).AsNoTracking().FirstOrDefault();
                if (CCMCycleStatus == null)
                {
                    return false;
                }
                else
                {
                    if (CCMCycleStatus.CCMStatus == "In Progress" || CCMCycleStatus.CCMStatus == "Enrolled" || CCMCycleStatus.CCMStatus == "Ready for Clinical Sign-Off")
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
        }
        public static bool CycleEntryForCurrentMonth()
        {
            using (ApplicationdbContect _db = new ApplicationdbContect())
            {
                AutomaticCycleStatusEntry entryforthismonth = _db.automaticCycleStatusEntries.Where(x => x.EntryMonth == DateTime.Now.Month && x.EntryYear == DateTime.Now.Year).FirstOrDefault();
                return entryforthismonth == null ? true : false;
            }

        }

        public static void UpdateCCMCycles(Patient currentPatient)
        {
            //var cycle = GetPatientCycle(currentPatient);
            //var s = GetCCMCycleStatus(currentPatient.Id, cycle, "", currentPatient.EnrollmentSubStatus);

        }
       
        //public static string GetCCMCycleStatus(int patientId, int Cycle, string Userid, string EnrollmentSubStatus = "")
        //{

        //    using (ApplicationdbContect Db = new ApplicationdbContect())
        //    {
        //        try
        //        {

        //            if (Cycle > 0)//means patient is enrolled in any billing category
        //            {

        //                var reviewtimeccms = Db.ReviewTimeCcms.Where(x => x.PatientId == patientId && x.BillingcategoryId == BillingCodeHelper.cmmBillingCatagoryid).ToList().Where(x => x.StartTime.Month == DateTime.Now.Month && x.StartTime.Year == DateTime.Now.Year).ToList();
        //                foreach (var item in reviewtimeccms)
        //                {
        //                    if (item.Cycle != Cycle)
        //                    {
        //                        item.Cycle = Cycle;
        //                        Db.Entry(item).State = EntityState.Modified;
        //                        Db.SaveChanges();
        //                    }
        //                }
        //                var finalcareplans = Db.FinalCarePlanNotes.Where(x => x.PatientId == patientId).ToList().Where(x => x.CarePlanCreatedOn.Value.Month == DateTime.Now.Month && x.CarePlanCreatedOn.Value.Year == DateTime.Now.Year && x.CarePlanCreatedOn != null).ToList();
        //                foreach (var item in finalcareplans)
        //                {
        //                    if (item.Cycle != Cycle)
        //                    {
        //                        item.Cycle = Cycle;
        //                        Db.Entry(item).State = EntityState.Modified;
        //                        Db.SaveChanges();
        //                    }
        //                }

        //            }


        //        }
        //        catch (Exception ex)
        //        {


        //        }
        //        var CCMCycleStatus = Db.CCMCycleStatuses.Where(x => x.PatientId == patientId && x.Cycle == Cycle).FirstOrDefault();
        //        if (CCMCycleStatus == null)
        //        {

        //            CCMCycleStatus cCMCycleStatus = new CCMCycleStatus();
        //            cCMCycleStatus.PatientId = patientId;
        //            cCMCycleStatus.Cycle = Cycle;
        //            cCMCycleStatus.RejectedCount = 0;
        //            if (Cycle == 0)
        //            {
        //                cCMCycleStatus.CCMStatus = "In Progress";
        //                cCMCycleStatus.CCMSubStatus = "";
        //            }
        //            else
        //            {
        //                cCMCycleStatus.CCMStatus = "Enrolled";
        //                cCMCycleStatus.CCMSubStatus = "";
        //            }
        //            if (EnrollmentSubStatus != "")
        //            {
        //                if (EnrollmentSubStatus != "Active Enrolled" && Cycle > 0)
        //                {
        //                    cCMCycleStatus.CCMStatus = "Expired";
        //                    cCMCycleStatus.CCMSubStatus = "";
        //                }
        //            }
        //            cCMCycleStatus.CreatedBy = Userid;
        //            cCMCycleStatus.CreatedOn = DateTime.Now;
        //            Db.CCMCycleStatuses.Add(cCMCycleStatus);
        //            Db.SaveChanges();
        //            return cCMCycleStatus.CCMStatus;
        //        }
        //        else
        //        {
        //            if (EnrollmentSubStatus != "")
        //            {


        //                if (EnrollmentSubStatus == "Active Enrolled")
        //                {
        //                    if (CCMCycleStatus.CCMStatus != "Claims Submission" && CCMCycleStatus.CCMStatus != "Clinical Sign-Off" && CCMCycleStatus.CCMStatus != "Ready for Clinical Sign-Off" && CCMCycleStatus.CCMStatus != "In Progress")
        //                    {
        //                        CCMCycleStatus.CCMStatus = "Enrolled";
        //                        CCMCycleStatus.UpdatedOn = DateTime.Now;
        //                        CCMCycleStatus.UpdatedBy = "";
        //                        Db.Entry(CCMCycleStatus).State = EntityState.Modified;
        //                        Db.SaveChanges();
        //                        return "Enrolled";
        //                    }

        //                }
        //                else
        //                {

        //                    if (CCMCycleStatus.CCMStatus != "Claims Submission" && CCMCycleStatus.CCMStatus != "Clinical Sign-Off" && CCMCycleStatus.CCMStatus != "Ready for Clinical Sign-Off" && CCMCycleStatus.CCMStatus != "In Progress")
        //                    {
        //                        CCMCycleStatus.CCMStatus = "Expired";
        //                        CCMCycleStatus.UpdatedOn = DateTime.Now;
        //                        CCMCycleStatus.UpdatedBy = "";
        //                        Db.Entry(CCMCycleStatus).State = EntityState.Modified;
        //                        Db.SaveChanges();
        //                        return "Expired";
        //                    }
        //                }
        //                var previouscycles = Db.CCMCycleStatuses.Where(x => x.PatientId == patientId && x.Cycle < Cycle && x.CCMStatus != "Claims Submission" && x.CCMStatus != "Clinical Sign-Off" && x.CCMStatus != "Ready for Clinical Sign-Off" && x.CCMStatus != "In Progress").ToList();
        //                foreach (var item in previouscycles)
        //                {
        //                    if (item.CCMStatus == "Enrolled")
        //                    {
        //                        item.CCMStatus = "Expired";
        //                        item.UpdatedOn = DateTime.Now;
        //                        item.UpdatedBy = "";
        //                        Db.Entry(item).State = EntityState.Modified;
        //                        Db.SaveChanges();
        //                    }
        //                }
        //            }
        //            else
        //            {
        //                var patientData = Db.Patients.Where(x => x.Id == patientId).FirstOrDefault();
        //                if (patientData.EnrollmentSubStatus == "Active Enrolled")
        //                {
        //                    if (CCMCycleStatus.CCMStatus != "Claims Submission" && CCMCycleStatus.CCMStatus != "Clinical Sign-Off" && CCMCycleStatus.CCMStatus != "Ready for Clinical Sign-Off" && CCMCycleStatus.CCMStatus != "In Progress")
        //                    {
        //                        CCMCycleStatus.CCMStatus = "Enrolled";
        //                        CCMCycleStatus.UpdatedOn = DateTime.Now;
        //                        CCMCycleStatus.UpdatedBy = "";
        //                        Db.Entry(CCMCycleStatus).State = EntityState.Modified;
        //                        Db.SaveChanges();
        //                        return "Enrolled";
        //                    }

        //                }
        //                else
        //                {

        //                    if (CCMCycleStatus.CCMStatus != "Claims Submission" && CCMCycleStatus.CCMStatus != "Clinical Sign-Off" && CCMCycleStatus.CCMStatus != "Ready for Clinical Sign-Off" && CCMCycleStatus.CCMStatus != "In Progress")
        //                    {
        //                        CCMCycleStatus.CCMStatus = "Expired";
        //                        CCMCycleStatus.UpdatedOn = DateTime.Now;
        //                        CCMCycleStatus.UpdatedBy = "";
        //                        Db.Entry(CCMCycleStatus).State = EntityState.Modified;
        //                        Db.SaveChanges();
        //                        return "Expired";
        //                    }
        //                }
        //                //var previouscycles = Db.CCMCycleStatuses.Where(x => x.PatientId == patientId && x.Cycle < Cycle && x.CCMStatus != "Claims Submission" && x.CCMStatus != "Clinical Sign-Off" && x.CCMStatus != "Ready for Clinical Sign-Off" && x.CCMStatus != "In Progress").ToList();
        //                //foreach (var item in previouscycles)
        //                //{
        //                //    if (item.CCMStatus == "Enrolled")
        //                //    {
        //                //        item.CCMStatus = "Expired";
        //                //        item.UpdatedOn = DateTime.Now;
        //                //        item.UpdatedBy = "";
        //                //        Db.Entry(item).State = EntityState.Modified;
        //                //        Db.SaveChanges();
        //                //    }
        //                //}
        //            }
        //            return CCMCycleStatus.CCMStatus;
        //        }


        //    }
        //}
       
      
        public static void UpdatecategoryStatus(int? BillingcategoryId, int PatientId, int Cycle, string CCMStatus, string Userid, string Reason, bool fromliaison = false, string SubmittedToLiaison = "")
        {
            using (ApplicationdbContect Db = new ApplicationdbContect())
            {
                Cycle = CategoryCycleStatusHelper.GetPatientNewOrOldCycleByCategory(PatientId, BillingcategoryId.GetInteger());
                var categoriesstatus = Db.CategoriesStatuses.Where(p => p.PatientId == PatientId && p.BillingCategoryId == BillingcategoryId).FirstOrDefault();

                if (categoriesstatus == null)
                {
                    categoriesstatus = new CategoriesStatuses();
                    categoriesstatus.PatientId = PatientId;
                    categoriesstatus.Cycle = Cycle;
                    categoriesstatus.BillingCategoryId = BillingcategoryId;
                    categoriesstatus.BillingCategoryName = Db.BillingCategories.Where(p => p.BillingCategoryId == BillingcategoryId).Select(p => p.Name).FirstOrDefault();
                    categoriesstatus.CreatedBy = Userid;
                    categoriesstatus.CreatedOn = DateTime.Now;


                    categoriesstatus.Status = CCMStatus;
                    categoriesstatus.SubStatus = "";

                    categoriesstatus.Notes = Reason;
                    Db.CategoriesStatuses.Add(categoriesstatus);
                    Db.SaveChanges();
                }
                categoriesstatus.Cycle = Cycle;
                categoriesstatus.Status = CCMStatus;
                categoriesstatus.SubStatus = "";
                categoriesstatus.Notes = Reason;

                if (CCMStatus == "Claims Submission")
                {
                    categoriesstatus.ApprovedBy = Userid;
                }
                if (CCMStatus == "Clinical Sign-Off")
                {

                    categoriesstatus.ClinicalSignOffDate = DateTime.Now;
                    // cCMCycleStatus.clinicalSignOffCounter += 1;
                    if (string.IsNullOrEmpty(categoriesstatus.SubmittedBy))
                    {
                        categoriesstatus.SubmittedBy = Userid;
                    }

                }
                else
                {
                    if (CCMStatus == "Claims Submission" && categoriesstatus.ClaimSubmissionDate == null)
                    {
                        categoriesstatus.ClaimSubmissionDate = DateTime.Now;
                    }
                    else
                    {
                        if (CCMStatus == "Reconciliation" && categoriesstatus.ReconciliationDate == null)
                        {
                            categoriesstatus.ReconciliationDate = DateTime.Now;

                        }
                        else
                        {
                            if (CCMStatus == "Enrolled" && !string.IsNullOrEmpty(Reason) && fromliaison == false)
                            {
                                categoriesstatus.RejectedDate = DateTime.Now;
                                categoriesstatus.RejectedBy = Userid;
                                try
                                {
                                    categoriesstatus.RejectedCount += 1;

                                    UpdateCycleStatusRejetionHistory(categoriesstatus, BillingcategoryId);
                                }
                                catch (Exception ex)
                                {


                                }

                            }
                            else
                            {
                                if (CCMStatus == "Enrolled" && !string.IsNullOrEmpty(Reason) && fromliaison == true)
                                {
                                    categoriesstatus.RejectedDatebyLiaison = DateTime.Now;
                                    categoriesstatus.RejectedbyLiaison = Userid;
                                    categoriesstatus.IsRejectedByLiaison = true;
                                    try
                                    {

                                    }
                                    catch (Exception ex)
                                    {


                                    }

                                }
                                else
                                {
                                    if (CCMStatus == "Ready for Clinical Sign-Off")
                                    {
                                        categoriesstatus.ReadyforClinicalSignOffDate = DateTime.Now;
                                        categoriesstatus.SubmittedByReadyforClinicalSignoff = Userid;
                                        categoriesstatus.SubmittedToReadyforClinicalSignoff = SubmittedToLiaison;
                                    }

                                }

                            }
                        }

                    }
                }
                categoriesstatus.UpdatedBy = Userid;
                categoriesstatus.UpdatedOn = DateTime.Now;
                Db.Entry(categoriesstatus).State = System.Data.Entity.EntityState.Modified;
                Db.SaveChanges();


            }
        }

        public static void UpdateCCMCycleStatus(int PatientId, int Cycle, string CCMStatus, string Userid, string Reason, bool fromliaison = false, string SubmittedToLiaison = "")
        {
            using (ApplicationdbContect Db = new ApplicationdbContect())
            {
                var cCMCycleStatus = Db.CCMCycleStatuses.Where(x => x.PatientId == PatientId && x.Cycle == Cycle).FirstOrDefault();
                if (cCMCycleStatus != null)
                {


                    cCMCycleStatus.CCMStatus = CCMStatus;
                    cCMCycleStatus.CCMSubStatus = "";
                    cCMCycleStatus.CCMNotes = Reason;
                    if (CCMStatus == "Claims Submission")
                    {
                        cCMCycleStatus.ApprovedBy = Userid;
                    }
                    if (CCMStatus == "Clinical Sign-Off")
                    {

                        cCMCycleStatus.CcmClinicalSignOffDate = DateTime.Now;
                        // cCMCycleStatus.clinicalSignOffCounter += 1;
                        if (string.IsNullOrEmpty(cCMCycleStatus.SubmittedBy))
                        {
                            cCMCycleStatus.SubmittedBy = Userid;
                        }

                    }
                    else
                    {
                        if (CCMStatus == "Claims Submission" && cCMCycleStatus.CcmClaimSubmissionDate == null)
                        {
                            cCMCycleStatus.CcmClaimSubmissionDate = DateTime.Now;
                        }
                        else
                        {
                            if (CCMStatus == "Reconciliation" && cCMCycleStatus.CcmReconciliationDate == null)
                            {
                                cCMCycleStatus.CcmReconciliationDate = DateTime.Now;
                            }
                            else
                            {
                                if (CCMStatus == "Enrolled" && !string.IsNullOrEmpty(Reason) && fromliaison == false)
                                {
                                    cCMCycleStatus.CcmRejectedDate = DateTime.Now;
                                    cCMCycleStatus.RejectedBy = Userid;
                                    try
                                    {
                                        cCMCycleStatus.RejectedCount += 1;

                                        UpdateCCMCycleStatusRejetionHistory(cCMCycleStatus);
                                    }
                                    catch (Exception ex)
                                    {


                                    }

                                }
                                else
                                {
                                    if (CCMStatus == "Enrolled" && !string.IsNullOrEmpty(Reason) && fromliaison == true)
                                    {
                                        cCMCycleStatus.CcmRejectedDatebyLiaison = DateTime.Now;
                                        cCMCycleStatus.RejectedbyLiaison = Userid;
                                        cCMCycleStatus.IsRejectedByLiaison = true;
                                        try
                                        {

                                        }
                                        catch (Exception ex)
                                        {


                                        }

                                    }
                                    else
                                    {
                                        if (CCMStatus == "Ready for Clinical Sign-Off")
                                        {
                                            cCMCycleStatus.CcmReadyforClinicalSignOffDate = DateTime.Now;
                                            cCMCycleStatus.SubmittedByReadyforClinicalSignoff = Userid;
                                            cCMCycleStatus.SubmittedToReadyforClinicalSignoff = SubmittedToLiaison;
                                        }

                                    }

                                }
                            }

                        }
                    }
                    cCMCycleStatus.UpdatedBy = Userid;
                    cCMCycleStatus.UpdatedOn = DateTime.Now;
                    Db.Entry(cCMCycleStatus).State = System.Data.Entity.EntityState.Modified;
                    Db.SaveChanges();

                }
            }
        }

        public static void UpdateCycleStatusRejetionHistory(CategoriesStatuses cCMCycleStatus, int? BillingcategoryId)
        {
            CCMCycleStatusRejectionHistory cCMCycleStatusRejectionHistory_obj = new CCMCycleStatusRejectionHistory();
            cCMCycleStatusRejectionHistory_obj.PatientId = (int)cCMCycleStatus.PatientId;
            cCMCycleStatusRejectionHistory_obj.rejectionDate = DateTime.Now;
            cCMCycleStatusRejectionHistory_obj.CCMStatus = cCMCycleStatus.Status;
            cCMCycleStatusRejectionHistory_obj.Cycle = (int)cCMCycleStatus.Cycle;
            cCMCycleStatusRejectionHistory_obj.SubmittedBy = cCMCycleStatus.SubmittedBy;
            cCMCycleStatusRejectionHistory_obj.BillingCategoryId = BillingcategoryId;
            using (ApplicationdbContect context = new ApplicationdbContect())
            {
                context.CCMCycleStatusRejectionHistory.Add(cCMCycleStatusRejectionHistory_obj);
                context.SaveChanges();
            }

        }
        public static void UpdateCCMCycleStatusRejetionHistory(CCMCycleStatus cCMCycleStatus)
        {
            CCMCycleStatusRejectionHistory cCMCycleStatusRejectionHistory_obj = new CCMCycleStatusRejectionHistory();
            cCMCycleStatusRejectionHistory_obj.PatientId = cCMCycleStatus.PatientId;
            cCMCycleStatusRejectionHistory_obj.rejectionDate = DateTime.Now;
            cCMCycleStatusRejectionHistory_obj.CCMStatus = cCMCycleStatus.CCMStatus;
            cCMCycleStatusRejectionHistory_obj.Cycle = cCMCycleStatus.Cycle;
            cCMCycleStatusRejectionHistory_obj.SubmittedBy = cCMCycleStatus.SubmittedBy;

            using (ApplicationdbContect context = new ApplicationdbContect())
            {
                context.CCMCycleStatusRejectionHistory.Add(cCMCycleStatusRejectionHistory_obj);
                context.SaveChanges();
            }

        }

        public static int ReviewTimeGet(string contollerName, int? patientId, string userId, int? BillingCategoryId = 0)
        {
            using (ApplicationdbContect Db = new ApplicationdbContect())
            {
                if (BillingCategoryId == 0)
                {

                    //int cycle = GetPatientCycle(patientId.Value);
                    int cycle = CategoryCycleStatusHelper.GetPatientNewOrOldCycleByCategory(patientId.Value, BillingCategoryId.GetInteger());
                    var ccmcyclestatus = Db.CCMCycleStatuses.AsNoTracking().Where(x => x.PatientId == patientId && x.Cycle == cycle).FirstOrDefault();
                    if (ccmcyclestatus != null)
                    {
                        if (ccmcyclestatus.CCMStatus == "Enrolled" || ccmcyclestatus.CCMStatus == "In Progress" || ccmcyclestatus.CCMStatus == "Ready for Clinical Sign-Off")
                        {

                            var review = new ReviewTimeCcm
                            {
                                PatientId = patientId,
                                UserId = userId,
                                Page = contollerName,
                                StartTime = DateTime.Now,
                                ReviewTime = TimeSpan.Zero,
                                Cycle = cycle
                            };

                            bool saveFailed;
                            do
                            {
                                saveFailed = false;

                                try
                                {
                                    Db.ReviewTimeCcms.Add(review);
                                    Db.SaveChanges();
                                }
                                catch (DbUpdateConcurrencyException ex)
                                {
                                    saveFailed = true;
                                    ex.Entries.Single().Reload();
                                }
                                catch (SqlException)
                                {
                                    saveFailed = true;
                                }
                                catch (InvalidOperationException)
                                {
                                    saveFailed = true;
                                }

                            } while (saveFailed);

                            return review.Id;
                        }
                        else
                        {
                            return 0;
                        }
                    }
                    else
                    {
                        return 0;
                    }
                }
                else
                {
                    // int cycle = GetCategoryPatientCycle(patientId.Value, BillingCategoryId);
                    int cycle = CategoryCycleStatusHelper.GetPatientNewOrOldCycleByCategory(patientId.Value, BillingCategoryId.GetInteger());
                    var ccmcyclestatus = Db.CategoriesStatuses.AsNoTracking().Where(x => x.PatientId == patientId && x.Cycle == cycle && x.BillingCategoryId== BillingCategoryId).FirstOrDefault();
                    if (ccmcyclestatus != null)
                    {
                        if (ccmcyclestatus.Status == "Enrolled" || ccmcyclestatus.Status == "In Progress" || ccmcyclestatus.Status == "Ready for Clinical Sign-Off")
                        {

                            var review = new ReviewTimeCcm
                            {
                                PatientId = patientId,
                                UserId = userId,
                                Page = contollerName,
                                StartTime = DateTime.Now,
                                ReviewTime = TimeSpan.Zero,
                                Cycle = cycle
                            };

                            bool saveFailed;
                            do
                            {
                                saveFailed = false;

                                try
                                {
                                    Db.ReviewTimeCcms.Add(review);
                                    Db.SaveChanges();
                                }
                                catch (DbUpdateConcurrencyException ex)
                                {
                                    saveFailed = true;
                                    ex.Entries.Single().Reload();
                                }
                                catch (SqlException)
                                {
                                    saveFailed = true;
                                }
                                catch (InvalidOperationException)
                                {
                                    saveFailed = true;
                                }

                            } while (saveFailed);

                            return review.Id;
                        }
                        else
                        {
                            return 0;
                        }
                    }
                    else
                    {
                        return 0;
                    }
                }
            }
        }
        public static int ReviewTimeGetForSales(string contollerName, string userId)
        {
            using (ApplicationdbContect Db = new ApplicationdbContect())
            {
                var review = new ReviewTimeCcm
                {
                    PatientId = 0,
                    UserId = userId,
                    Page = contollerName,
                    StartTime = DateTime.Now,
                    ReviewTime = TimeSpan.Zero,
                    Cycle = 0,
                    Activity = "Adding Patient"
                };

                bool saveFailed;
                do
                {
                    saveFailed = false;

                    try
                    {
                        Db.ReviewTimeCcms.Add(review);
                        Db.SaveChanges();
                    }
                    catch (DbUpdateConcurrencyException ex)
                    {
                        saveFailed = true;
                        ex.Entries.Single().Reload();
                    }
                    catch (SqlException)
                    {
                        saveFailed = true;
                    }
                    catch (InvalidOperationException)
                    {
                        saveFailed = true;
                    }

                } while (saveFailed);

                return review.Id;
            }
        }
        public static List<CountryTelCode> TelCodes { get; set; }
        public class CountryTelCode
        //*****************************************************************
        {
            public string Pfx { get; set; }
            public string Iso { get; set; }
            public int Priority { get; set; }

            public CountryTelCode(string pfx, string iso, int priority = 0)
            {
                Pfx = pfx;
                Iso = iso;
                Priority = priority;
            }
        }
        public static void InitTelCodes()
        //-----------------------------------------------------------------
        {

            TelCodes = new List<CountryTelCode>
            {
                  new CountryTelCode("+93", "AF"),
        new CountryTelCode("+355", "AL"),
        new CountryTelCode("+213", "DZ"),
        new CountryTelCode("+1-684", "AS"),
        new CountryTelCode("+376", "AD"),
        new CountryTelCode("+244", "AO"),
        new CountryTelCode("+1-264", "AI"),
        new CountryTelCode("+672", "AQ"),
        new CountryTelCode("+1-268", "AG"),
        new CountryTelCode("+54", "AR"),
        new CountryTelCode("+374", "AM"),
        new CountryTelCode("+297", "AW"),
        new CountryTelCode("+61", "AU"),
        new CountryTelCode("+43", "AT"),
        new CountryTelCode("+994", "AZ"),
        new CountryTelCode("+1-242", "BS"),
        new CountryTelCode("+973", "BH"),
        new CountryTelCode("+880", "BD"),
        new CountryTelCode("+1-246", "BB"),
        new CountryTelCode("+375", "BY"),
        new CountryTelCode("+32", "BE"),
        new CountryTelCode("+501", "BZ"),
        new CountryTelCode("+229", "BJ"),
        new CountryTelCode("+1-441", "BM"),
        new CountryTelCode("+975", "BT"),
        new CountryTelCode("+591", "BO"),
        new CountryTelCode("+387", "BA"),
        new CountryTelCode("+267", "BW"),
        new CountryTelCode("+55", "BR"),
        new CountryTelCode("+246", "IO"),
        new CountryTelCode("+1-284", "VG"),
        new CountryTelCode("+673", "BN"),
        new CountryTelCode("+359", "BG"),
        new CountryTelCode("+226", "BF"),
        new CountryTelCode("+257", "BI"),
        new CountryTelCode("+855", "KH"),
        new CountryTelCode("+237", "CM"),
        new CountryTelCode("+1", "CA"),
        new CountryTelCode("+238", "CV"),
        new CountryTelCode("+1-345", "KY"),
        new CountryTelCode("+236", "CF"),
        new CountryTelCode("+235", "TD"),
        new CountryTelCode("+56", "CL"),
        new CountryTelCode("+86", "CN"),
        new CountryTelCode("+61", "CX"),
        new CountryTelCode("+61", "CC"),
        new CountryTelCode("+57", "CO"),
        new CountryTelCode("+269", "KM"),
        new CountryTelCode("+682", "CK"),
        new CountryTelCode("+506", "CR"),
        new CountryTelCode("+385", "HR"),
        new CountryTelCode("+53", "CU"),
        new CountryTelCode("+599", "CW"),
        new CountryTelCode("+357", "CY"),
        new CountryTelCode("+420", "CZ"),
        new CountryTelCode("+243", "CD"),
        new CountryTelCode("+45", "DK"),
        new CountryTelCode("+253", "DJ"),
        new CountryTelCode("+1-767", "DM"),
        new CountryTelCode("+1-809", "DO"),
        new CountryTelCode("+1-829", "DO"),
        new CountryTelCode("+1-849", "DO"),
        new CountryTelCode("+670", "TL"),
        new CountryTelCode("+593", "EC"),
        new CountryTelCode("+20", "EG"),
        new CountryTelCode("+503", "SV"),
        new CountryTelCode("+240", "GQ"),
        new CountryTelCode("+291", "ER"),
        new CountryTelCode("+372", "EE"),
        new CountryTelCode("+251", "ET"),
        new CountryTelCode("+500", "FK"),
        new CountryTelCode("+298", "FO"),
        new CountryTelCode("+679", "FJ"),
        new CountryTelCode("+358", "FI"),
        new CountryTelCode("+33", "FR"),
        new CountryTelCode("+689", "PF"),
        new CountryTelCode("+241", "GA"),
        new CountryTelCode("+220", "GM"),
        new CountryTelCode("+995", "GE"),
        new CountryTelCode("+49", "DE"),
        new CountryTelCode("+233", "GH"),
        new CountryTelCode("+350", "GI"),
        new CountryTelCode("+30", "GR"),
        new CountryTelCode("+299", "GL"),
        new CountryTelCode("+1-473", "GD"),
        new CountryTelCode("+1-671", "GU"),
        new CountryTelCode("+502", "GT"),
        new CountryTelCode("+44-1481", "GG"),
        new CountryTelCode("+224", "GN"),
        new CountryTelCode("+245", "GW"),
        new CountryTelCode("+592", "GY"),
        new CountryTelCode("+509", "HT"),
        new CountryTelCode("+504", "HN"),
        new CountryTelCode("+852", "HK"),
        new CountryTelCode("+36", "HU"),
        new CountryTelCode("+354", "IS"),
        new CountryTelCode("+91", "IN"),
        new CountryTelCode("+62", "ID"),
        new CountryTelCode("+98", "IR"),
        new CountryTelCode("+964", "IQ"),
        new CountryTelCode("+353", "IE"),
        new CountryTelCode("+44-1624", "IM"),
        new CountryTelCode("+972", "IL"),
        new CountryTelCode("+39", "IT"),
        new CountryTelCode("+225", "CI"),
        new CountryTelCode("+1-876", "JM"),
        new CountryTelCode("+81", "JP"),
        new CountryTelCode("+44-1534", "JE"),
        new CountryTelCode("+962", "JO"),
        new CountryTelCode("+7", "KZ"),
        new CountryTelCode("+254", "KE"),
        new CountryTelCode("+686", "KI"),
        new CountryTelCode("+383", "XK"),
        new CountryTelCode("+965", "KW"),
        new CountryTelCode("+996", "KG"),
        new CountryTelCode("+856", "LA"),
        new CountryTelCode("+371", "LV"),
        new CountryTelCode("+961", "LB"),
        new CountryTelCode("+266", "LS"),
        new CountryTelCode("+231", "LR"),
        new CountryTelCode("+218", "LY"),
        new CountryTelCode("+423", "LI"),
        new CountryTelCode("+370", "LT"),
        new CountryTelCode("+352", "LU"),
        new CountryTelCode("+853", "MO"),
        new CountryTelCode("+389", "MK"),
        new CountryTelCode("+261", "MG"),
        new CountryTelCode("+265", "MW"),
        new CountryTelCode("+60", "MY"),
        new CountryTelCode("+960", "MV"),
        new CountryTelCode("+223", "ML"),
        new CountryTelCode("+356", "MT"),
        new CountryTelCode("+692", "MH"),
        new CountryTelCode("+222", "MR"),
        new CountryTelCode("+230", "MU"),
        new CountryTelCode("+262", "YT"),
        new CountryTelCode("+52", "MX"),
        new CountryTelCode("+691", "FM"),
        new CountryTelCode("+373", "MD"),
        new CountryTelCode("+377", "MC"),
        new CountryTelCode("+976", "MN"),
        new CountryTelCode("+382", "ME"),
        new CountryTelCode("+1-664", "MS"),
        new CountryTelCode("+212", "MA"),
        new CountryTelCode("+258", "MZ"),
        new CountryTelCode("+95", "MM"),
        new CountryTelCode("+264", "NA"),
        new CountryTelCode("+674", "NR"),
        new CountryTelCode("+977", "NP"),
        new CountryTelCode("+31", "NL"),
        new CountryTelCode("+599", "AN"),
        new CountryTelCode("+687", "NC"),
        new CountryTelCode("+64", "NZ"),
        new CountryTelCode("+505", "NI"),
        new CountryTelCode("+227", "NE"),
        new CountryTelCode("+234", "NG"),
        new CountryTelCode("+683", "NU"),
        new CountryTelCode("+850", "KP"),
        new CountryTelCode("+1-670", "MP"),
        new CountryTelCode("+47", "NO"),
        new CountryTelCode("+968", "OM"),
        new CountryTelCode("+92", "PK"),
        new CountryTelCode("+680", "PW"),
        new CountryTelCode("+970", "PS"),
        new CountryTelCode("+507", "PA"),
        new CountryTelCode("+675", "PG"),
        new CountryTelCode("+595", "PY"),
        new CountryTelCode("+51", "PE"),
        new CountryTelCode("+63", "PH"),
        new CountryTelCode("+64", "PN"),
        new CountryTelCode("+48", "PL"),
        new CountryTelCode("+351", "PT"),
        new CountryTelCode("+1-787", "PR"),
        new CountryTelCode("+1-939", "PR"),
        new CountryTelCode("+974", "QA"),
        new CountryTelCode("+242", "CG"),
        new CountryTelCode("+262", "RE"),
        new CountryTelCode("+40", "RO"),
        new CountryTelCode("+7", "RU"),
        new CountryTelCode("+250", "RW"),
        new CountryTelCode("+590", "BL"),
        new CountryTelCode("+290", "SH"),
        new CountryTelCode("+1-869", "KN"),
        new CountryTelCode("+1-758", "LC"),
        new CountryTelCode("+590", "MF"),
        new CountryTelCode("+508", "PM"),
        new CountryTelCode("+1-784", "VC"),
        new CountryTelCode("+685", "WS"),
        new CountryTelCode("+378", "SM"),
        new CountryTelCode("+239", "ST"),
        new CountryTelCode("+966", "SA"),
        new CountryTelCode("+221", "SN"),
        new CountryTelCode("+381", "RS"),
        new CountryTelCode("+248", "SC"),
        new CountryTelCode("+232", "SL"),
        new CountryTelCode("+65", "SG"),
        new CountryTelCode("+1-721", "SX"),
        new CountryTelCode("+421", "SK"),
        new CountryTelCode("+386", "SI"),
        new CountryTelCode("+677", "SB"),
        new CountryTelCode("+252", "SO"),
        new CountryTelCode("+27", "ZA"),
        new CountryTelCode("+82", "KR"),
        new CountryTelCode("+211", "SS"),
        new CountryTelCode("+34", "ES"),
        new CountryTelCode("+94", "LK"),
        new CountryTelCode("+249", "SD"),
        new CountryTelCode("+597", "SR"),
        new CountryTelCode("+47", "SJ"),
        new CountryTelCode("+268", "SZ"),
        new CountryTelCode("+46", "SE"),
        new CountryTelCode("+41", "CH"),
        new CountryTelCode("+963", "SY"),
        new CountryTelCode("+886", "TW"),
        new CountryTelCode("+992", "TJ"),
        new CountryTelCode("+255", "TZ"),
        new CountryTelCode("+66", "TH"),
        new CountryTelCode("+228", "TG"),
        new CountryTelCode("+690", "TK"),
        new CountryTelCode("+676", "TO"),
        new CountryTelCode("+1-868", "TT"),
        new CountryTelCode("+216", "TN"),
        new CountryTelCode("+90", "TR"),
        new CountryTelCode("+993", "TM"),
        new CountryTelCode("+1-649", "TC"),
        new CountryTelCode("+688", "TV"),
        new CountryTelCode("+1-340", "VI"),
        new CountryTelCode("+256", "UG"),
        new CountryTelCode("+380", "UA"),
        new CountryTelCode("+971", "AE"),
        new CountryTelCode("+44", "GB"),
        new CountryTelCode("+1", "US"),
        new CountryTelCode("+598", "UY"),
        new CountryTelCode("+998", "UZ"),
        new CountryTelCode("+678", "VU"),
        new CountryTelCode("+379", "VA"),
        new CountryTelCode("+58", "VE"),
        new CountryTelCode("+84", "VN"),
        new CountryTelCode("+681", "WF"),
        new CountryTelCode("+212", "EH"),
        new CountryTelCode("+967", "YE"),
        new CountryTelCode("+260", "ZM"),
        new CountryTelCode("+263", "ZW"),


            };

        }
        public static bool ToBoolean(this string value)
        {
            switch (value.ToLower())
            {
                case "true":
                    return true;
                case "t":
                    return true;
                case "1":
                    return true;
                case "0":
                    return false;
                case "false":
                    return false;
                case "f":
                    return false;
                default:
                    throw new InvalidCastException("You can't cast that value to a bool!");
            }
        }
        public static void LogError(string userName, string userId, string message, string stackTrace)
        {
            log.Error(Environment.NewLine + userName
                + "-------" + userId + Environment.NewLine + message + "-----" + stackTrace);

            AuditTrail aud = new AuditTrail()
            {
                createdDate = DateTime.Now,
                message = message,
                stackTrace = stackTrace,
                userId = userId
            };
            Db.AuditTrails.Add(aud);
            Db.SaveChanges();


        }
        public static Tuple<string, string, string> Appointments(int id)
        {
            string CouncelorAppointmentDate = "";
            string TranslatorAppointmentDate = "";
            string EnrollerAppointmentDate = "";

            var laisonID = Db.patientAppointments.Where(p => p.LiaisonID.Value != null).ToList().Select(X => X.LiaisonID);




            var translatorIds = Db.Liaisons.Where(p => (laisonID.Contains(p.Id) && p.IsTranslator == true)).ToList().Select(p => p.Id);


            var laisonsIds = Db.Liaisons.Where(p => (laisonID.Contains(p.Id) && p.IsTranslator == false)).ToList().Select(p => p.Id);

            var enrollerID = Db.patientAppointments.Where(p => p.SaleStaffID.Value != null).ToList().Select(X => X.SaleStaffID);

            //int? Id = item1.Id;
            var laisonspatientAppointmentids = Db.patientAppointments.Where(p => p.PatientID == id && p.LiaisonID != null).Select(p => p.ID).ToList();

            var lpatientAppointmentid = Db.patientAppointments.Where(p => p.PatientID == id && p.LiaisonID != null).Select(p => p.ID).FirstOrDefault();

            int? transid = Db.Patients.Where(p => p.Id.Equals(id) && p.TranslatorId != null).Select(p => p.TranslatorId).FirstOrDefault();

            int? Liasonid = Db.Patients.Where(p => p.Id.Equals(id) && p.LiaisonId != null).Select(p => p.LiaisonId).FirstOrDefault();



            if (Liasonid != null)
            {
                var lappoint = Db.patientAppointments.Where(p => p.LiaisonID == Liasonid && p.PatientID == id).Select(p => p.LiaisonID).ToList();


                if (lappoint != null)
                {
                    DateTime app = new DateTime();
                    foreach (var item2 in lappoint)
                    {
                        List<DateTime> appointments = Db.patientAppointments.Where(p => p.LiaisonID == item2.Value && p.PatientID == id).Select(p => p.StartTime).ToList();
                        appointments.Sort((a, b) => a.CompareTo(b));

                        List<DateTime> upcomingapplist = appointments.Where(p => p >= DateTime.Now).Select(p => p).Take(2).ToList();
                        string upcomings;
                        if (upcomingapplist != null && upcomingapplist.Count() != 0)
                        {
                            upcomings = string.Join(System.Environment.NewLine, upcomingapplist);
                            upcomings = "Upcomings :" + System.Environment.NewLine + upcomings + System.Environment.NewLine;

                        }
                        else
                        {
                            upcomings = "";
                        }
                        appointments.Sort((a, b) => b.CompareTo(a));
                        List<DateTime> pastapplist = appointments.Where(p => p < DateTime.Now).Select(p => p).Take(2).ToList();
                        string pastapp;
                        if (pastapplist != null && pastapplist.Count() != 0)
                        {
                            pastapplist.Sort((a, b) => a.CompareTo(b));
                            pastapp = string.Join(System.Environment.NewLine, pastapplist);
                            pastapp = "Past Appointments :" + System.Environment.NewLine + pastapp + System.Environment.NewLine;


                        }
                        else
                        {
                            pastapp = "";
                        }
                        CouncelorAppointmentDate = pastapp + System.Environment.NewLine + upcomings;

                        break;
                    }

                    //DateTime datecheck = new DateTime();
                    //if (app == datecheck)
                    //{
                    //    item1.CouncelorAppointmentDate = "";
                    //}
                    //else {
                    //item1.CouncelorAppointmentDate = Convert.ToString(app);
                    //}
                }
            }
            if (transid != null)
            {

                var tappoint = Db.patientAppointments.Where(p => p.LiaisonID == transid && p.PatientID == id && p.PatientID == id).Select(p => p.LiaisonID).ToList();

                if (tappoint != null)
                {
                    DateTime app = new DateTime();
                    foreach (var item2 in tappoint)
                    {
                        List<DateTime> appointments = Db.patientAppointments.Where(p => p.LiaisonID == item2.Value && p.PatientID == id).Select(p => p.StartTime).ToList();
                        appointments.Sort((a, b) => a.CompareTo(b));
                        List<DateTime> upcomingapplist = appointments.Where(p => p >= DateTime.Now).Select(p => p).Take(2).ToList();
                        string upcomings;
                        if (upcomingapplist != null && upcomingapplist.Count() != 0)
                        {
                            upcomings = string.Join(System.Environment.NewLine, upcomingapplist);
                            upcomings = "Upcomings :" + System.Environment.NewLine + upcomings + System.Environment.NewLine;

                        }
                        else
                        {
                            upcomings = "";
                        }
                        appointments.Sort((a, b) => b.CompareTo(a));
                        List<DateTime> pastapplist = appointments.Where(p => p < DateTime.Now).Select(p => p).Take(2).ToList();
                        string pastapp;
                        if (pastapplist != null && pastapplist.Count() != 0)
                        {
                            pastapplist.Sort((a, b) => a.CompareTo(b));
                            pastapp = string.Join(System.Environment.NewLine, pastapplist);
                            pastapp = "Past Appointments :" + System.Environment.NewLine + pastapp + System.Environment.NewLine;


                        }
                        else
                        {
                            pastapp = "";
                        }
                        TranslatorAppointmentDate = pastapp + System.Environment.NewLine + upcomings;

                        //app = appointments.Where(p => p >= DateTime.Today).Select(p => p).FirstOrDefault();
                        break;
                    }
                    //DateTime datecheck = new DateTime();
                    //if (app == datecheck)
                    //{
                    //    item1.TranslatorAppointmentDate = "";
                    //}
                    //else
                    //{
                    //    item1.TranslatorAppointmentDate = Convert.ToString(app);
                    //}
                }
            }

            var enrollerpatientsapp = Db.patientAppointments.Where(p => p.PatientID == id && p.SaleStaffID != null).Select(p => p.ID).FirstOrDefault();

            var eappoint = Db.patientAppointments.Where(p => p.PatientID == id && p.SaleStaffID != null).Select(p => p.SaleStaffID).ToList();

            if (eappoint != null)
            {
                DateTime app = new DateTime();
                foreach (var item2 in eappoint)
                {
                    List<DateTime> appointments = Db.patientAppointments.Where(p => p.SaleStaffID == item2.Value && p.PatientID == id).Select(p => p.StartTime).ToList();
                    appointments.Sort((a, b) => a.CompareTo(b));
                    List<DateTime> upcomingapplist = appointments.Where(p => p >= DateTime.Now).Select(p => p).Take(2).ToList();
                    string upcomings;
                    if (upcomingapplist != null && upcomingapplist.Count() != 0)
                    {
                        upcomings = string.Join(System.Environment.NewLine, upcomingapplist);
                        upcomings = "Upcomings :" + System.Environment.NewLine + upcomings + System.Environment.NewLine;

                    }
                    else
                    {
                        upcomings = "";
                    }
                    appointments.Sort((a, b) => b.CompareTo(a));
                    List<DateTime> pastapplist = appointments.Where(p => p < DateTime.Now).Select(p => p).Take(2).ToList();
                    string pastapp;
                    if (pastapplist != null && pastapplist.Count() != 0)
                    {
                        pastapplist.Sort((a, b) => a.CompareTo(b));
                        pastapp = string.Join(System.Environment.NewLine, pastapplist);
                        pastapp = "Past Appointments :" + System.Environment.NewLine + pastapp + System.Environment.NewLine;


                    }
                    else
                    {
                        pastapp = "";
                    }
                    EnrollerAppointmentDate = pastapp + System.Environment.NewLine + upcomings;

                    //app = appointments.Where(p => p >= DateTime.Today).Select(p => p).FirstOrDefault();
                    break;
                }
                //DateTime datecheck = new DateTime();
                //if (app == datecheck)
                //{
                //    item1.EnrollerAppointmentDate = "";
                //}
                //else
                //{
                //    item1.EnrollerAppointmentDate = Convert.ToString(app);
                //}
            }






            // Ticketing




            return new Tuple<string, string, string>(CouncelorAppointmentDate, TranslatorAppointmentDate, EnrollerAppointmentDate);


        }

        public static string GetResolutionById(int Id)
        {
            string resname = Db.ticketResolution.Where(p => p.id == Id).Select(p => p.resolutionName).FirstOrDefault();

            return "";
        }

        public static string GetPatientBillingCodes(int? BillingCycleId, int? BillingcategoryId)
        {
            string billingcode = "";
            if (BillingCycleId != null)
            {
                var billingCycle = Db.BillingCycles.Where(p => p.Id == BillingCycleId).Include(x => x.BillingCodes).Select(p => p).FirstOrDefault();
                if (billingCycle != null)
                {
                    if (billingCycle.BillingCodes != null)
                    {
                        var BillingCodes = billingCycle.BillingCodes.Where(p => p.BillingCategoryId == BillingcategoryId).ToList();
                        foreach (var item in BillingCodes)
                        {

                            billingcode = billingcode + System.Environment.NewLine + item.BillingCategory.Name + System.Environment.NewLine + item.Name;

                        }
                    }

                }

            }
            return billingcode;

        }



        public static void UpdateTwilioNumbers()
        {

            var count = Db.TwilioNumbersTable.Where(p => p.Status == false).Count();
            if (count < 3)
            {
                var limit = 3 - count;
                TwilioClient.Init(ConfigurationManager.AppSettings["TwilioAccountSid"], ConfigurationManager.AppSettings["TwilioAuthToken"]);
                var localAvailableNumbers = LocalResource.Read("US", areaCode: Convert.ToInt32(ConfigurationManager.AppSettings["TwilioAreaCodeForNumbers"]), limit: limit, voiceEnabled: true);

                var callerIDs = localAvailableNumbers.Select(p => new SelectListItem
                {
                    Value = p.PhoneNumber.ToString(),
                    Text = p.FriendlyName.ToString()
                });

                foreach (var item in callerIDs)
                {
                    TwilioNumbersTable twilioNumbers = new TwilioNumbersTable();
                    twilioNumbers.MobilePhoneNumber = item.Value;
                    twilioNumbers.FriendlyPhoneNumer = item.Text;
                    twilioNumbers.Status = false;
                    twilioNumbers.CreatedOn = DateTime.Now;
                    twilioNumbers.CreatedBy = HttpContext.Current.User.Identity.GetUserId();
                    Db.TwilioNumbersTable.Add(twilioNumbers);
                    Db.SaveChanges();


                }




            }


        }

        public static TimeSpan GetTotalReviewTime(int? patientId, int? BillingCategoryId)
        {

            try
            {

                var reviews = Db.ReviewTimeCcms.Where(r => r.PatientId == patientId && r.BillingcategoryId == BillingCategoryId).AsNoTracking().ToList();

                return reviews.Any()
                    ? reviews.Aggregate(TimeSpan.Zero, (sum, review) => sum + review.ReviewTime)
                    : TimeSpan.Zero;
            }
            catch (Exception ex)
            {


                var reviews = Db.ReviewTimeCcms.Where(r => r.PatientId == patientId && r.BillingcategoryId == BillingCategoryId).AsNoTracking().ToList();

                return reviews.Any()
                    ? reviews.Aggregate(TimeSpan.Zero, (sum, review) => sum + review.ReviewTime)
                    : TimeSpan.Zero;

            }
        }
        public static string GenerateGUID()
        {
            return Guid.NewGuid().ToString();
        }
    }
}