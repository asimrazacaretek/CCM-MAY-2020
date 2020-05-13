using CCM.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web.Configuration;
using System.Web.Mvc;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;


namespace CCM.Controllers
{
    [Authorize(Roles = "Liaison, Physician, PhysiciansGroup, Admin, LiaisonGroup, QAQC,Sales")]
    public class ContactPatientController : BaseController
    {
        //private readonly ApplicationdbContect _db = new ApplicationdbContect();
        public async Task<ActionResult> ReceiveCalls()
        {
            return View();
        }
        public async Task<PartialViewResult> ReceiveCallsNew()
        {
            return PartialView();
        }
        
        public  string GetNamefromNumber(string CallSid)
        {
            string patientid = "";
            try
            {

                var callHistory = new CallHistory();
                try
                {

                    callHistory.From = CallSid;
                    callHistory.From = callHistory.From.Replace("+1", "");
                    patientid = callHistory.From;
                  var  patient = _db.Patients.AsNoTracking().Where(x => (x.MobilePhoneNumber != null && x.MobilePhoneNumber.Replace("", "").Contains(callHistory.From)) || (x.WorkPhoneNumber != null && x.WorkPhoneNumber.Replace(" ", "").Contains(callHistory.From)) || (x.EmergencyNumber != null && x.EmergencyNumber.Replace(" ", "").Contains(callHistory.From)) || (x.HomePhoneNumber != null && x.HomePhoneNumber.Replace(" ", "").Contains(callHistory.From)) || (x.CaretakerPhoneNumber != null && x.CaretakerPhoneNumber.Replace(" ", "").Contains(callHistory.From))).FirstOrDefault();

                    if (patient != null)
                    {
                        patientid = patient.FirstName + " " + patient.LastName;
                        return patientid;
                        
                    }
                    else
                    {
                        var patientprofile = _db.PatientProfile_Contact.AsNoTracking().Where(x => (x.CellPhoneNumber != null && x.CellPhoneNumber.Replace(" ", "").Contains(callHistory.From)) || (x.CellPhoneNumber1 != null && x.CellPhoneNumber1.Replace(" ", "").Contains(callHistory.From)) || (x.CellPhoneNumber2 != null && x.CellPhoneNumber2.Replace(" ", "").Contains(callHistory.From)) || (x.EmergencyNumber != null && x.EmergencyNumber.Replace(" ", "").Contains(callHistory.From)) || (x.EmergencyNumber1 != null && x.EmergencyNumber1.Replace(" ", "").Contains(callHistory.From)) || (x.EmergencyNumber2 != null && x.EmergencyNumber2.Replace(" ", "").Contains(callHistory.From)) || (x.HomePhoneNumber != null && x.HomePhoneNumber.Replace(" ", "").Contains(callHistory.From)) || (x.HomePhoneNumber1 != null && x.HomePhoneNumber1.Replace(" ", "").Contains(callHistory.From)) || (x.HomePhoneNumber2 != null && x.HomePhoneNumber2.Replace(" ", "").Contains(callHistory.From)) || (x.WorkPhoneNumber != null && x.WorkPhoneNumber.Replace(" ", "").Contains(callHistory.From)) || (x.WorkPhoneNumber1 != null && x.WorkPhoneNumber1.Replace(" ", "").Contains(callHistory.From)) || (x.WorkPhoneNumber2 != null && x.WorkPhoneNumber2.Replace(" ", "").Contains(callHistory.From))).FirstOrDefault();

                        if (patientprofile != null)
                        {
                            var patientidnew = patientprofile.PatientId;
                            var patientnew = _db.Patients.AsNoTracking().Where(x => x.Id == patientidnew).FirstOrDefault();
                            patientid = patientnew.FirstName + " " + patientnew.LastName;
                            return patientid;
                        }
                        else
                        {
                           var patienturgencycontact = _db.PatientProfile_UrgencyContacts.AsNoTracking().Where(x => (x.PrimaryHomePhoneNumber != null && x.PrimaryHomePhoneNumber.Replace(" ", "").Contains(callHistory.From)) || (x.PrimaryHomePhoneNumber1 != null && x.PrimaryHomePhoneNumber1.Replace(" ", "").Contains(callHistory.From)) || (x.PrimaryHomePhoneNumber2 != null && x.PrimaryHomePhoneNumber2.Replace(" ", "").Contains(callHistory.From)) || (x.PrimaryMobilePhoneNumber != null && x.PrimaryMobilePhoneNumber.Replace(" ", "").Contains(callHistory.From)) || (x.PrimaryMobilePhoneNumber1 != null && x.PrimaryMobilePhoneNumber1.Replace(" ", "").Contains(callHistory.From)) || (x.PrimaryMobilePhoneNumber2 != null && x.PrimaryMobilePhoneNumber2.Replace(" ", "").Contains(callHistory.From)) || (x.SecondaryHomePhoneNumber != null && x.SecondaryHomePhoneNumber.Replace(" ", "").Contains(callHistory.From)) || (x.SecondaryHomePhoneNumber1 != null && x.SecondaryHomePhoneNumber1.Replace(" ", "").Contains(callHistory.From)) || (x.SecondaryHomePhoneNumber2 != null && x.SecondaryHomePhoneNumber2.Replace(" ", "").Contains(callHistory.From)) || (x.SecondaryMobilePhoneNumber != null && x.SecondaryMobilePhoneNumber.Replace(" ", "").Contains(callHistory.From)) || (x.SecondaryMobilePhoneNumber1 != null && x.SecondaryMobilePhoneNumber1.Replace(" ", "").Contains(callHistory.From)) || (x.SecondaryMobilePhoneNumber2 != null && x.SecondaryMobilePhoneNumber2.Replace(" ", "").Contains(callHistory.From))).FirstOrDefault();
                            if(patienturgencycontact != null)
                            {
                                var patientidnew = patienturgencycontact.PatientId;
                                var patientnew = _db.Patients.AsNoTracking().Where(x => x.Id == patientidnew).FirstOrDefault();
                                patientid = patientnew.FirstName + " " + patientnew.LastName;
                                return patientid;
                            }
                            else
                            {
                                var patientcallhistory = _db.CallHistories.AsNoTracking().Where(x => x.To.Replace(" ", "").Contains(callHistory.From)).FirstOrDefault();
                                if (patientcallhistory != null)
                                {
                                    var patientidnew = patientcallhistory.PatientID;
                                    if (patientidnew > 0)
                                    {
                                        var patientnew = _db.Patients.AsNoTracking().Where(x => x.Id == patientidnew).FirstOrDefault();
                                        patientid = patientnew.FirstName + " " + patientnew.LastName;
                                        return patientid;
                                    }
                                   
                                }
                            }
                            
                        }

                    }
                    
                }
                catch (Exception ex)
                {


                }

            }
            catch (Exception ex)
            {

                
            }
            return patientid;
        }
        public string GetNamefromNumberNew(string CallSid)
        {
            string patientid = "";
            try
            {

                var callHistory = new CallHistory();
                try
                {

                    callHistory.From = CallSid;
                    callHistory.From = callHistory.From.Replace("+1", "");
                    patientid = callHistory.From;
                    var patient = _db.Patients.AsNoTracking().Where(x => (x.MobilePhoneNumber != null && x.MobilePhoneNumber.Replace("", "").Contains(callHistory.From)) || (x.WorkPhoneNumber != null && x.WorkPhoneNumber.Replace(" ", "").Contains(callHistory.From)) || (x.EmergencyNumber != null && x.EmergencyNumber.Replace(" ", "").Contains(callHistory.From)) || (x.HomePhoneNumber != null && x.HomePhoneNumber.Replace(" ", "").Contains(callHistory.From)) || (x.CaretakerPhoneNumber != null && x.CaretakerPhoneNumber.Replace(" ", "").Contains(callHistory.From))).FirstOrDefault();

                    if (patient != null)
                    {
                        patientid = patient.FirstName + " " + patient.LastName;
                        return patientid+"||"+patient.Id;

                    }
                    else
                    {
                        var patientprofile = _db.PatientProfile_Contact.AsNoTracking().Where(x => (x.CellPhoneNumber != null && x.CellPhoneNumber.Replace(" ", "").Contains(callHistory.From)) || (x.CellPhoneNumber1 != null && x.CellPhoneNumber1.Replace(" ", "").Contains(callHistory.From)) || (x.CellPhoneNumber2 != null && x.CellPhoneNumber2.Replace(" ", "").Contains(callHistory.From)) || (x.EmergencyNumber != null && x.EmergencyNumber.Replace(" ", "").Contains(callHistory.From)) || (x.EmergencyNumber1 != null && x.EmergencyNumber1.Replace(" ", "").Contains(callHistory.From)) || (x.EmergencyNumber2 != null && x.EmergencyNumber2.Replace(" ", "").Contains(callHistory.From)) || (x.HomePhoneNumber != null && x.HomePhoneNumber.Replace(" ", "").Contains(callHistory.From)) || (x.HomePhoneNumber1 != null && x.HomePhoneNumber1.Replace(" ", "").Contains(callHistory.From)) || (x.HomePhoneNumber2 != null && x.HomePhoneNumber2.Replace(" ", "").Contains(callHistory.From)) || (x.WorkPhoneNumber != null && x.WorkPhoneNumber.Replace(" ", "").Contains(callHistory.From)) || (x.WorkPhoneNumber1 != null && x.WorkPhoneNumber1.Replace(" ", "").Contains(callHistory.From)) || (x.WorkPhoneNumber2 != null && x.WorkPhoneNumber2.Replace(" ", "").Contains(callHistory.From))).FirstOrDefault();

                        if (patientprofile != null)
                        {
                            var patientidnew = patientprofile.PatientId;
                            var patientnew = _db.Patients.AsNoTracking().Where(x => x.Id == patientidnew).FirstOrDefault();
                            patientid = patientnew.FirstName + " " + patientnew.LastName;
                            return patientid + "||" + patientnew.Id;
                        }
                        else
                        {
                            var patienturgencycontact = _db.PatientProfile_UrgencyContacts.AsNoTracking().Where(x => (x.PrimaryHomePhoneNumber != null && x.PrimaryHomePhoneNumber.Replace(" ", "").Contains(callHistory.From)) || (x.PrimaryHomePhoneNumber1 != null && x.PrimaryHomePhoneNumber1.Replace(" ", "").Contains(callHistory.From)) || (x.PrimaryHomePhoneNumber2 != null && x.PrimaryHomePhoneNumber2.Replace(" ", "").Contains(callHistory.From)) || (x.PrimaryMobilePhoneNumber != null && x.PrimaryMobilePhoneNumber.Replace(" ", "").Contains(callHistory.From)) || (x.PrimaryMobilePhoneNumber1 != null && x.PrimaryMobilePhoneNumber1.Replace(" ", "").Contains(callHistory.From)) || (x.PrimaryMobilePhoneNumber2 != null && x.PrimaryMobilePhoneNumber2.Replace(" ", "").Contains(callHistory.From)) || (x.SecondaryHomePhoneNumber != null && x.SecondaryHomePhoneNumber.Replace(" ", "").Contains(callHistory.From)) || (x.SecondaryHomePhoneNumber1 != null && x.SecondaryHomePhoneNumber1.Replace(" ", "").Contains(callHistory.From)) || (x.SecondaryHomePhoneNumber2 != null && x.SecondaryHomePhoneNumber2.Replace(" ", "").Contains(callHistory.From)) || (x.SecondaryMobilePhoneNumber != null && x.SecondaryMobilePhoneNumber.Replace(" ", "").Contains(callHistory.From)) || (x.SecondaryMobilePhoneNumber1 != null && x.SecondaryMobilePhoneNumber1.Replace(" ", "").Contains(callHistory.From)) || (x.SecondaryMobilePhoneNumber2 != null && x.SecondaryMobilePhoneNumber2.Replace(" ", "").Contains(callHistory.From))).FirstOrDefault();
                            if (patienturgencycontact != null)
                            {
                                var patientidnew = patienturgencycontact.PatientId;
                                var patientnew = _db.Patients.AsNoTracking().Where(x => x.Id == patientidnew).FirstOrDefault();
                                patientid = patientnew.FirstName + " " + patientnew.LastName;
                                return patientid + "||" + patientnew.Id;
                            }
                            else
                            {
                                var patientcallhistory = _db.CallHistories.AsNoTracking().Where(x => x.To.Replace(" ", "").Contains(callHistory.From)).FirstOrDefault();
                                if (patientcallhistory != null)
                                {
                                    var patientidnew = patientcallhistory.PatientID;
                                    if (patientidnew > 0)
                                    {
                                        var patientnew = _db.Patients.AsNoTracking().Where(x => x.Id == patientidnew).FirstOrDefault();
                                        patientid = patientnew.FirstName + " " + patientnew.LastName;
                                        return patientid + "||" + patientnew.Id;
                                    }

                                }
                            }

                        }

                    }

                }
                catch (Exception ex)
                {


                }

            }
            catch (Exception ex)
            {


            }
            return patientid;
        }
        public string TestCall(string To,string From)
        {
           
            TwilioClient.Init(ConfigurationManager.AppSettings["TwilioAccountSid"], ConfigurationManager.AppSettings["TwilioAuthToken"]);

            var to = new PhoneNumber(To);
            var from = new PhoneNumber(From);
            var call = CallResource.Create(to, from,
                url: new Uri("https://72ea4792.ngrok.io/Voice?to="+ To));
            return call.Sid;
        }

        [Authorize(Roles = "Liaison, Admin,Sales")]
        public async Task<ActionResult> Index(int? patientId)
        {
            ViewBag.eMessage = TempData["eMessage"];
            ViewBag.LiasionName = "Admin";
            var patient = await _db.Patients.FindAsync(patientId);
            if (patient != null)
            {
                var istranslator = HelperExtensions.isTranslator(User.Identity.GetUserId());
                if (istranslator == true)
                {
                    ViewBag.LiasionID = patient.TranslatorId;
                    try
                    {
                        var liasion = _db.Liaisons.AsNoTracking().Where(x => x.Id == patient.TranslatorId).FirstOrDefault();
                        if (liasion != null)
                        {
                            ViewBag.CallerId = liasion.TwilioCallerId ?? "";
                            ViewBag.LiasionName = liasion.FirstName + liasion.LastName;
                        }

                    }
                    catch (Exception ex)
                    {


                    }
                }
                else
                {
                    ViewBag.LiasionID = patient.LiaisonId;
                    try
                    {
                        var liasion = _db.Liaisons.AsNoTracking().Where(x => x.Id == patient.LiaisonId).FirstOrDefault();
                        if (liasion != null)
                        {
                            ViewBag.CallerId = liasion.TwilioCallerId ?? "";
                            ViewBag.LiasionName = liasion.FirstName + liasion.LastName;
                        }

                    }
                    catch (Exception ex)
                    {


                    }
                }
            }
            //    ViewBag.LiasionID = patient.LiaisonId;
            //try
            //{
            //    var liasion = _db.Liaisons.AsNoTracking().Where(x => x.Id == patient.LiaisonId).FirstOrDefault();
            //    if (liasion != null)
            //    {
            //        ViewBag.CallerId =liasion.TwilioCallerId??"";
            //        ViewBag.LiasionName = liasion.FirstName + liasion.LastName;
            //    }
               
            //}
            //catch (Exception ex)
            //{

              
            //}
           
            return View(new ContactPatientViewModel
                {
                    PatientId = patientId,
                    PatientProfile_Contact = patient.Contact,
                    PatientProfile_UrgencyContact = patient.UrgencyContact,
                    PatientProfile_UrgencyContacts=_db.PatientProfile_UrgencyContacts.AsNoTracking().Where(x=>x.PatientId==patientId).ToList(),
                    secondaryDoctors = _db.SecondaryDoctors.Where(x => x.PatientId == patientId).AsNoTracking().ToList(),
                    MobilePhoneNumber = patient.MobilePhoneNumber,
                    HomePhoneNumber = patient.HomePhoneNumber,
                    WorkPhoneNumber = patient.WorkPhoneNumber,
                    EmergencyPhoneNumber = patient.EmergencyNumber,
                    CareTakerPhoneNumber = patient.CaretakerPhoneNumber,
                    FullName = patient.FirstName + ' ' + patient.LastName,
                    EmailTo = patient.Email
                });

            ViewBag.Message = "Patient Not Found!";
            return View("Error");
        }
        [Authorize(Roles = "Liaison, Admin")]
        public async Task<PartialViewResult> IndexNew(int? patientId)
        {
            ViewBag.LiasionName = "Admin";
            var patient = await _db.Patients.FindAsync(patientId);
            
            if (patient != null)
            {
                if(patient.EnrollmentSubStatus == "Active Enrolled")
                {

                var istranslator = HelperExtensions.isTranslator(User.Identity.GetUserId());
                if (istranslator == true)
                {
                    ViewBag.LiasionID = patient.TranslatorId;
                    try
                    {
                        var liasion = _db.Liaisons.AsNoTracking().Where(x => x.Id == patient.TranslatorId).FirstOrDefault();
                        if (liasion != null)
                        {
                            ViewBag.CallerId = liasion.TwilioCallerId ?? "";
                            ViewBag.LiasionName = liasion.FirstName + liasion.LastName;
                        }

                    }
                    catch (Exception ex)
                    {


                    }
                }
                else
                {
                    ViewBag.LiasionID = patient.LiaisonId;
                    try
                    {
                        var liasion = _db.Liaisons.AsNoTracking().Where(x => x.Id == patient.LiaisonId).FirstOrDefault();
                        if (liasion != null)
                        {
                            ViewBag.CallerId = liasion.TwilioCallerId ?? "";
                            ViewBag.LiasionName = liasion.FirstName + liasion.LastName;
                        }

                    }
                    catch (Exception ex)
                    {


                    }
                }
                }

                if(patient.EnrollmentSubStatus!= "Active Enrolled")
                {
                    var istranslator = HelperExtensions.isTranslator(User.Identity.GetUserId());
                    if (istranslator == true)
                    {
                        if (patient.Patients_PreLiaisonsId != null)
                        {

                        ViewBag.LiasionID = patient.Patients_PreLiaisons.TranslatorId;
                        }
                        try
                        {
                            if (patient.Patients_PreLiaisonsId != null)
                            {


                                var liasion = _db.Liaisons.AsNoTracking().Where(x => x.Id == patient.Patients_PreLiaisons.TranslatorId).FirstOrDefault();
                                if (liasion != null)
                                {
                                    ViewBag.CallerId = liasion.TwilioCallerId ?? "";
                                    ViewBag.LiasionName = liasion.FirstName + liasion.LastName;
                                }
                            }

                        }
                        catch (Exception ex)
                        {


                        }
                    }
                    else
                    {
                        if (patient.Patients_PreLiaisonsId != null)
                        {

                            ViewBag.LiasionID = patient.Patients_PreLiaisons.LiaisonId;
                        }
                        try
                        {
                            if (patient.Patients_PreLiaisonsId != null)
                            {

                                var liasion = _db.Liaisons.AsNoTracking().Where(x => x.Id == patient.Patients_PreLiaisons.LiaisonId).FirstOrDefault();
                                if (liasion != null)
                                {
                                    ViewBag.CallerId = liasion.TwilioCallerId ?? "";
                                    ViewBag.LiasionName = liasion.FirstName + liasion.LastName;
                                }
                            }
                        }
                        catch (Exception ex)
                        {


                        }
                    }
                }
            }



            
                
            
            ViewBag.eMessage = TempData["eMessage"];

           
            if (patient != null)
                return PartialView(new ContactPatientViewModel
                {
                    PatientId = patientId,
                    PatientProfile_Contact = patient.Contact,
                    PatientProfile_UrgencyContact = patient.UrgencyContact,
                    PatientProfile_UrgencyContacts = _db.PatientProfile_UrgencyContacts.AsNoTracking().Where(x => x.PatientId == patientId).ToList(),
                    secondaryDoctors = _db.SecondaryDoctors.Where(x => x.PatientId == patientId).AsNoTracking().ToList(),
                    MobilePhoneNumber = patient.MobilePhoneNumber,
                    HomePhoneNumber = patient.HomePhoneNumber,
                    WorkPhoneNumber = patient.WorkPhoneNumber,
                    EmergencyPhoneNumber = patient.EmergencyNumber,
                    CareTakerPhoneNumber = patient.CaretakerPhoneNumber,
                    FullName = patient.FirstName + ' ' + patient.LastName,
                    EmailTo = patient.Email
                });

            ViewBag.Message = "Patient Not Found!";
            return PartialView("Error");
        }
        [HttpPost]
        public string SendSignatureLink(int? patientId, string phoneNumber, string messageBody)
        {
            var patient = _db.Patients.Find(patientId);
            if (patient == null)
                return "Error: Patient Not Found!";

            try
            {
                TwilioClient.Init(ConfigurationManager.AppSettings["TwilioAccountSid"], ConfigurationManager.AppSettings["TwilioAuthToken"]);
                var callerid = ConfigurationManager.AppSettings["TwilioCallerId"];

                var user = _db.Users.Find(User.Identity.GetUserId());
                if (user.Role == "Liaison")
                {
                    var liasioncallerid = _db.Liaisons.AsNoTracking().Where(x => x.Id == user.CCMid).FirstOrDefault().TwilioCallerId;
                    if (!string.IsNullOrEmpty(liasioncallerid))
                    {
                        callerid = liasioncallerid;
                    }
                }
               
                var to = new PhoneNumber(phoneNumber);

                var message = MessageResource.Create(to,
                                                     from: new PhoneNumber(callerid),
                                                     body:messageBody);

                _db.TextHistories.Add(new TextHistory
                {
                    PatientId = patient.Id,
                    DateTime = DateTime.Now,
                    FromUser = User.Identity.GetUserId(),
                    Message = messageBody,
                    ToPhoneNumber = phoneNumber,
                    TwilioSid = message.Sid,
                    TwilioCallerId = callerid,
                    TwilioStatus = message.Status.ToString()
                });
                _db.SaveChanges();

                return "True";
            }

            catch (Exception e)
            {
                return "Error: " + e.Message;
            }
        }
        [HttpPost]
        public string SendTextMessage(int? patientId, string phoneNumber, string messageBody)
        {
            var patient =  _db.Patients.Find(patientId);
            if (patient == null)
                return "Error: Patient Not Found!";

            try
            {
                TwilioClient.Init(ConfigurationManager.AppSettings["TwilioAccountSid"], ConfigurationManager.AppSettings["TwilioAuthToken"]);
                var callerid = ConfigurationManager.AppSettings["TwilioCallerId"];

                var user = _db.Users.Find(User.Identity.GetUserId());
                if (user.Role == "Liaison")
                {
                    var liasioncallerid = _db.Liaisons.AsNoTracking().Where(x => x.Id == user.CCMid).FirstOrDefault().TwilioCallerId;
                    if (!string.IsNullOrEmpty(liasioncallerid))
                    {
                        callerid = liasioncallerid;
                    }
                }
                var doctor =  _db.Physicians.Find(patient.PhysicianId);
                var preText = "Automated message from Dr. " + doctor?.LastName + ":\n\n";
                var to = new PhoneNumber(phoneNumber);

                var message = MessageResource.Create(to,
                                                     from: new PhoneNumber(callerid),
                                                     body: preText + messageBody);

                _db.TextHistories.Add(new TextHistory
                {
                    PatientId = patient.Id,
                    DateTime = DateTime.Now,
                    FromUser = User.Identity.GetUserId(),
                    Message = messageBody,
                    ToPhoneNumber = phoneNumber,
                    TwilioSid = message.Sid,
                    TwilioCallerId = callerid,
                    TwilioStatus = message.Status.ToString()
                });
                 _db.SaveChanges();

                return "Success: Text Message Sent.";
            }

            catch (Exception e)
            {
                return "Error: " + e.Message;
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SendEmail(ContactPatientViewModel viewModel)
        {
            var patient = await _db.Patients.FindAsync(viewModel.PatientId);
            if (patient == null)
            {
                ViewBag.Message = "Patient Not Found!";
                return View("Error");
            }

            try
            {
                var doctor = await _db.Physicians.FindAsync(patient.PhysicianId);
                var preText = "Hello " + patient.FirstName + ",<br />" +
                              "This is an automated message from Dr. " + doctor?.LastName + "'s office:<br /><br />";
                var body = preText + viewModel.EmailMessage + "<br /><br />" +

                              "Thank you." + "<br/>" +
                              "My CCM Health." + "<br/><br/>" +

                              "<small>This email is system generated and is not monitored by customer service. Please do not reply to this email. " +
                              "<br />If you need any assistance, please call our customer service at (212) 920-4500.</small>";

                using (var message = new MailMessage(WebConfigurationManager.AppSettings["SmtpSender"], viewModel.EmailTo))
                {
                    message.Subject = "My CCM Health - Email from Dr. " + doctor?.LastName + "'s office:";
                    message.Body = body;
                    message.IsBodyHtml = true;

                    foreach (var attachment in viewModel.Attachments)
                    {
                        if (attachment != null)
                        {
                            var fileName = Path.GetFileName(attachment.FileName);
                            message.Attachments.Add(new Attachment(attachment.InputStream, fileName));
                        }
                    }

                    var smtp = new SmtpClient
                    {
                        Host = WebConfigurationManager.AppSettings["SmtpHost"],
                        Port = Convert.ToInt32(WebConfigurationManager.AppSettings["SmtpPort"]),
                        EnableSsl = true,
                        DeliveryMethod = SmtpDeliveryMethod.Network,
                        UseDefaultCredentials = false,
                        Credentials = new NetworkCredential(WebConfigurationManager.AppSettings["SmtpSender"], WebConfigurationManager.AppSettings["SmtpPassword"])
                    };

                    smtp.Send(message);
                    _db.EmailHistories.Add(new EmailHistory
                    {
                        PatientId = patient.Id,
                        DateTime = DateTime.Now,
                        FromUser = User.Identity.GetUserId(),
                        Message = viewModel.EmailMessage,
                        ToEmail = viewModel.EmailTo,

                        AttachmentsCount = viewModel.Attachments.Count()
                    });
                    await _db.SaveChangesAsync();
                    TempData["eMessage"] = "Success: Email Sent.";
                }

                return RedirectToAction("Index", new { patientId = viewModel.PatientId });
            }

            catch (Exception e)
            {
                TempData["eMessage"] = "Error: " + e.Message;
                return RedirectToAction("Index", new { patientId = viewModel.PatientId });
            }
        }
        public async Task<string> SendSignatureLinkEmail(int? patientId, string Emailaddress, string messageBody)
        {
            var patient = await _db.Patients.FindAsync(patientId);
            if (patient == null)
            {
                
                return "Patient Not Found!";
            }

            try
            {

                var body = messageBody;
                using (var message = new MailMessage(WebConfigurationManager.AppSettings["SmtpSender"], Emailaddress))
                {
                    message.Subject = "Email link for signature from CCM Health";
                    message.Body = body;
                    message.IsBodyHtml = true;
                    var smtp = new SmtpClient
                    {
                        Host = WebConfigurationManager.AppSettings["SmtpHost"],
                        Port = Convert.ToInt32(WebConfigurationManager.AppSettings["SmtpPort"]),
                        EnableSsl = true,
                        DeliveryMethod = SmtpDeliveryMethod.Network,
                        UseDefaultCredentials = false,
                        Credentials = new NetworkCredential(WebConfigurationManager.AppSettings["SmtpSender"], WebConfigurationManager.AppSettings["SmtpPassword"])
                    };

                    smtp.Send(message);
                    _db.EmailHistories.Add(new EmailHistory
                    {
                        PatientId = patient.Id,
                        DateTime = DateTime.Now,
                        FromUser = User.Identity.GetUserId(),
                        Message = messageBody,
                        ToEmail = Emailaddress,

                        AttachmentsCount =0
                    });
                    await _db.SaveChangesAsync();
                    TempData["eMessage"] = "Success: Email Sent.";
                }

                return "True";
            }

            catch (Exception e)
            {
                return "Error: " + e.Message;
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<string> SendEmail1(ContactPatientViewModel viewModel)
        {
            var patient = await _db.Patients.FindAsync(viewModel.PatientId);
            if (patient == null)
            {
                ViewBag.Message = "Patient Not Found!";
                return "Patient Not found";
            }

            try
            {
                var doctor = await _db.Physicians.FindAsync(patient.PhysicianId);
                var preText = "Hello " + patient.FirstName + ",<br />" +
                              "This is an automated message from Dr. " + doctor?.LastName + "'s office:<br /><br />";
                var body = preText + viewModel.EmailMessage + "<br /><br />" +

                              "Thank you." + "<br/>" +
                              "My CCM Health." + "<br/><br/>" +

                              "<small>This email is system generated and is not monitored by customer service. Please do not reply to this email. " +
                              "<br />If you need any assistance, please call our customer service at (212) 920-4500.</small>";

                using (var message = new MailMessage(WebConfigurationManager.AppSettings["SmtpSender"], viewModel.EmailTo))
                {
                    message.Subject = "My CCM Health - Email from Dr. " + doctor?.LastName + "'s office:";
                    message.Body = body;
                    message.IsBodyHtml = true;

                    foreach (var attachment in viewModel.Attachments)
                    {
                        if (attachment != null)
                        {
                            var fileName = Path.GetFileName(attachment.FileName);
                            message.Attachments.Add(new Attachment(attachment.InputStream, fileName));
                        }
                    }

                    var smtp = new SmtpClient
                    {
                        Host = WebConfigurationManager.AppSettings["SmtpHost"],
                        Port = Convert.ToInt32(WebConfigurationManager.AppSettings["SmtpPort"]),
                        EnableSsl = true,
                        DeliveryMethod = SmtpDeliveryMethod.Network,
                        UseDefaultCredentials = false,
                        Credentials = new NetworkCredential(WebConfigurationManager.AppSettings["SmtpSender"], WebConfigurationManager.AppSettings["SmtpPassword"])
                    };

                    smtp.Send(message);
                    _db.EmailHistories.Add(new EmailHistory
                    {
                        PatientId = patient.Id,
                        DateTime = DateTime.Now,
                        FromUser = User.Identity.GetUserId(),
                        Message = viewModel.EmailMessage,
                        ToEmail = viewModel.EmailTo,

                        AttachmentsCount = viewModel.Attachments.Count()
                    });
                    await _db.SaveChangesAsync();
                    
                }

                return "True";
            }

            catch (Exception e)
            {


                return "False";
            }
        }
        [HttpPost]
        public async Task<ActionResult> SaveCallNotes(string Notes, int PatientID)
        {
            var patient = _db.Patients.Where(x => x.Id == PatientID).FirstOrDefault();
            patient.CallingNote = Notes;
            _db.Entry(patient).State = EntityState.Modified;
            _db.SaveChanges();
            return Content("Success: Note Saved.");
        }
        [HttpPost]
        public async Task<bool> SaveCallRecordings(string callSid, string To, int PatientID)
        {
            try
            {


                TwilioClient.Init(ConfigurationManager.AppSettings["TwilioAccountSid"], ConfigurationManager.AppSettings["TwilioAuthToken"]);
                var calls = await CallResource.ReadAsync( parentCallSid: (callSid));
                if (calls.Any())
                {
                    foreach (var call in calls)
                    {
                        var callHistory = new CallHistory()
                        {
                            StartTime = call?.StartTime,
                            EndTime = call?.EndTime,
                            Status = call?.Status.ToString(),
                            Duration = call?.StartTime != null && call.EndTime != null
                                                         ? (TimeSpan)(call.EndTime - call.StartTime)
                                                         : TimeSpan.Zero,
                            To = call?.To,
                            From = call?.From,
                            PatientID = PatientID,
                            TwilioCallId = call?.ParentCallSid

                        };

                        var recordings = await RecordingResource.ReadAsync(callSid: call?.ParentCallSid);
                        if (recordings.Any())
                        {
                            foreach (var recording in recordings)
                            {
                                callHistory.RecordingURL = recording?.Uri;
                            }
                            var src = callHistory.RecordingURL.Substring(0, callHistory.RecordingURL.IndexOf('.'));
                            var fullSource = "https://api.twilio.com" + src + ".mp3";
                            WebClient webClient = new WebClient();
                            string physicalPath = Server.MapPath("~/Recordings/" + callHistory.TwilioCallId + ".mp3");
                            webClient.DownloadFile(fullSource, physicalPath);
                        }


                        _db.CallHistories.Add(callHistory);
                       await _db.SaveChangesAsync();
                    }

                }
            }
            catch (Exception ex)
            {


            }
            return true;
        }
        public async Task<bool> SaveDatatoDB()
        {
            try
            {
                var starttime = Convert.ToDateTime(ConfigurationManager.AppSettings["TwilioStartDate"]);
                var endtime = Convert.ToDateTime(ConfigurationManager.AppSettings["TwilioEndDate"]);

                TwilioClient.Init(ConfigurationManager.AppSettings["TwilioAccountSid"], ConfigurationManager.AppSettings["TwilioAuthToken"]);

                var calls = await CallResource.ReadAsync(pathAccountSid: ConfigurationManager.AppSettings["TwilioAccountSid"], from: new PhoneNumber(ConfigurationManager.AppSettings["TwilioFromNumber"]),status:CallResource.StatusEnum.Completed);
                // var calls = callnew.ToList();


                // var totalcount = calls.Count();


                if (calls.Any())
                {

                    foreach (var call in calls)
                    {
                        try
                        {
                            var patient = _db.Patients.AsNoTracking().Where(x => x.MobilePhoneNumber.Contains(call.To.Replace("+1","")) || x.WorkPhoneNumber.Contains(call.To.Replace("+1", "")) || x.EmergencyNumber.Contains(call.To.Replace("+1", "")) || x.HomePhoneNumber.Contains(call.To.Replace("+1", ""))).FirstOrDefault();
                            int PatientId = 0;
                            if (patient != null)
                            {
                                PatientId = patient.Id;
                            }

                            var callHistory = new CallHistory()
                            {
                                StartTime = call?.StartTime,
                                EndTime = call?.EndTime,
                                Status = call?.Status.ToString(),
                                Duration = call?.StartTime != null && call.EndTime != null
                                                             ? (TimeSpan)(call.EndTime - call.StartTime)
                                                             : TimeSpan.Zero,
                                To = call?.To,
                                From = call?.From,
                                TwilioCallId = call?.ParentCallSid,
                                PatientID=PatientId

                            };

                            var recordings = await RecordingResource.ReadAsync(callSid: call?.ParentCallSid);
                            //if (!recordings.Any())
                            //    continue;
                            var alreadyexists = _db.CallHistories.AsNoTracking().Where(x => x.TwilioCallId == callHistory.TwilioCallId).ToList();
                            if (alreadyexists.Count == 0)
                            {
                                if (recordings.Any())
                                {
                                    foreach (var recording in recordings)
                                        callHistory.RecordingURL = recording?.Uri;
                                    var src = callHistory.RecordingURL.Substring(0, callHistory.RecordingURL.IndexOf('.'));
                                    var fullSource = "https://api.twilio.com" + src + ".mp3";
                                    WebClient webClient = new WebClient();
                                    string physicalPath = Server.MapPath("~/Recordings/" + callHistory.TwilioCallId + ".mp3");
                                    webClient.DownloadFile(fullSource, physicalPath);
                                }
                                   
                                _db.CallHistories.Add(callHistory);
                                _db.SaveChanges();
                                //  _db.CallHistories.RemoveRange(alreadyexists);
                            }
                        }
                        catch (Exception)
                        {

                           
                        }
                    }

                }
                return true;
            }
            catch (Exception ex)
            {
                return true;

            }
        }
        public PartialViewResult _CallHistoryPartial(int? patientId,DateTime? From,DateTime? To, int LiasionID = 0)
        {
            var callHistories = new List<CallHistory>();

            //var patient = await _db.Patients.FindAsync(patientId);
            //if (patient == null)
            //    return PartialView(callHistories);

            //var phoneNumbers = new List<string>();

            //if (!string.IsNullOrEmpty(patient.MobilePhoneNumber))
            //    phoneNumbers.Add(patient.MobilePhoneNumber);
            //if (!string.IsNullOrEmpty(patient.HomePhoneNumber))
            //    phoneNumbers.Add(patient.HomePhoneNumber);
            //if (!string.IsNullOrEmpty(patient.WorkPhoneNumber))
            //    phoneNumbers.Add(patient.WorkPhoneNumber);
            //if (!string.IsNullOrEmpty(patient.EmergencyNumber))
            //    phoneNumbers.Add(patient.EmergencyNumber);

            //if (phoneNumbers.Count <= 0)
            //    return PartialView(callHistories);


            //// TwilioClient.Init(ConfigurationManager.AppSettings["TwilioAccountSid"], ConfigurationManager.AppSettings["TwilioAuthToken"]);
            //List<string> formatedPhoneNumbers = new List<string>();
            //foreach (var number in phoneNumbers)
            //{
            //    var to = number.Length == 11 && number.Substring(0) == "1"
            //           ? "+" + number : number.Length == 10
            //           ? "+1" + number : number;
            //    formatedPhoneNumbers.Add(to);

            //}
            // var calls =  _db.CallHistories.Where(x => formatedPhoneNumbers.Contains(x.To)).ToList();
            if (LiasionID > 0)
            {
                //var patientids = _db.Patients.AsNoTracking().Where(x => x.LiaisonId == LiasionID).ToList().Select(x => x.Id).ToList();
                //var istranlator = _db.Liaisons.Where(x => x.Id == LiasionID).FirstOrDefault().IsTranslator;
                //if (istranlator == true)
                //{
                //    patientids = _db.Patients.AsNoTracking().Where(x => x.TranslatorId == LiasionID).ToList().Select(x => x.Id).ToList();
                //}
                //var calls = _db.CallHistories.AsNoTracking().Where(x => patientids.Contains(x.PatientID)).ToList();
                var calls = _db.CallHistories.AsNoTracking().Where(x => x.LiaisonId==LiasionID).ToList();
                var calls1=calls.Where(x=>x.StartTime !=null && x.StartTime.Value.Date>=From.Value.Date && x.StartTime.Value.Date <= To.Value.Date).OrderByDescending(x => x.StartTime).ToList()
                .Select(x => new CallHistoryViewModel
                 {Id=x.Id,
                     PatientID = x.PatientID,
                     StartTime = x.StartTime,
                     To = x.To,
                     From=x.From,
                     Duration = x.Duration,
                     Status = x.Status,
                     RecordingURL = x.RecordingURL,
                     TwilioCallId = x.TwilioCallId,
                     isVoiceMail = false

                 }).ToList();
                return PartialView("_CallHistoryPartial", calls1);
            }
            else
            {
                var calls = _db.CallHistories.AsNoTracking().Where(x => x.PatientID == patientId).ToList().OrderByDescending(x => x.StartTime).ToList().Select(x => new CallHistoryViewModel {
                    Id=x.Id,
                    PatientID =x.PatientID,
                    StartTime=x.StartTime,
                    To=x.To,
                    From = x.From,
                    Duration =x.Duration,
                    Status=x.Status,
                    RecordingURL=x.RecordingURL,
                    TwilioCallId=x.TwilioCallId,
                    isVoiceMail=false

                }).ToList();
                var Voicecalls = _db.voiceMailHistories.AsNoTracking().Where(x => x.PatientID == patientId).ToList().OrderByDescending(x => x.StartTime).ToList().Select(x => new CallHistoryViewModel
                {Id=x.Id,
                    PatientID = x.PatientID,
                    StartTime = x.StartTime,
                    To = x.To,
                    From = x.From,
                    Duration = x.Duration,
                    Status = x.Status,
                    RecordingURL = x.RecordingURL,
                    TwilioCallId = x.TwilioCallId,
                    isVoiceMail=true

                }).ToList();
                calls.AddRange(Voicecalls);
                calls = calls.OrderByDescending(x => x.StartTime).ToList();
                return PartialView("_CallHistoryPartial", calls);
            }
           
            return PartialView("_CallHistoryPartial", callHistories);
        }
        public async Task<PartialViewResult> _CallHistoryPartial1(int? patientId, DateTime? From, DateTime? To, int LiasionID = 0)
        {
            var callHistories = new List<CallHistory>();

          
            if (LiasionID > 0)
            {
                //var patientids = _db.Patients.AsNoTracking().Where(x => x.LiaisonId == LiasionID).ToList().Select(x => x.Id).ToList();
                //var istranlator = _db.Liaisons.Where(x => x.Id == LiasionID).FirstOrDefault().IsTranslator;
                //if (istranlator == true)
                //{
                //    patientids = _db.Patients.AsNoTracking().Where(x => x.TranslatorId == LiasionID).ToList().Select(x => x.Id).ToList();
                //}
                //var calls = _db.CallHistories.AsNoTracking().Where(x => patientids.Contains(x.PatientID)).ToList();
                var calls = _db.CallHistories.AsNoTracking().Where(x => x.LiaisonId==LiasionID).ToList();
                var calls1 = calls.Where(x => x.StartTime.Value.Date >= From.Value.Date && x.StartTime.Value.Date <= To.Value.Date).OrderByDescending(x => x.StartTime).ToList();
                return PartialView("_CallHistoryPartial1", calls1);
            }
            else
            {
                var calls = _db.CallHistories.AsNoTracking().Where(x => x.PatientID == patientId).ToList().OrderByDescending(x => x.StartTime).ToList();
                return PartialView("_CallHistoryPartial1", calls);
            }

            return PartialView("_CallHistoryPartial", callHistories);
        }
        //public async Task<PartialViewResult> _CallHistoryPartial(int? patientId)
        //{
        //    var callHistories = new List<CallHistory>();

        //    var patient =  await _db.Patients.FindAsync(patientId);
        //    if (patient == null)
        //        return PartialView(callHistories);

        //    var phoneNumbers = new List<string>();

        //    if (!string.IsNullOrEmpty(patient.MobilePhoneNumber))
        //        phoneNumbers.Add(patient.MobilePhoneNumber);
        //    if (!string.IsNullOrEmpty(patient.HomePhoneNumber))
        //        phoneNumbers.Add(patient.HomePhoneNumber);
        //    if (!string.IsNullOrEmpty(patient.WorkPhoneNumber))
        //        phoneNumbers.Add(patient.WorkPhoneNumber);
        //    if (!string.IsNullOrEmpty(patient.EmergencyNumber))
        //        phoneNumbers.Add(patient.EmergencyNumber);

        //    if (phoneNumbers.Count <= 0)
        //        return PartialView(callHistories);


        //    TwilioClient.Init(ConfigurationManager.AppSettings["TwilioAccountSid"], ConfigurationManager.AppSettings["TwilioAuthToken"]);

        //    foreach (var number in phoneNumbers)
        //    {
        //        var to = number.Length == 11 && number.Substring(0) == "1"
        //               ? "+"  + number : number.Length == 10
        //               ? "+1" + number : number;

        //        var calls = await CallResource.ReadAsync(to: new PhoneNumber(to));
        //        if (!calls.Any())
        //            continue;

        //        foreach (var call in calls)
        //        {
        //            var callHistory = new CallHistory()
        //            {
        //                StartTime = call?.StartTime,
        //                EndTime   = call?.EndTime,
        //                Status    = call?.Status.ToString(),
        //                Duration  = call?.StartTime != null && call.EndTime != null
        //                          ? (TimeSpan)(call.EndTime - call.StartTime)
        //                          : TimeSpan.Zero,
        //                To        = number
        //            };

        //            var recordings = await RecordingResource.ReadAsync(callSid: call?.ParentCallSid);
        //            if (!recordings.Any())
        //                continue;

        //            foreach (var recording in recordings)
        //                callHistory.RecordingURL = recording?.Uri;

        //            callHistories.Add(callHistory);
        //        }
        //    }

        //    return PartialView("_CallHistoryPartial", callHistories);
        //}

        public async Task<PartialViewResult> _TextHistoryPartial(int? patientId, DateTime? From, DateTime? To, int LiasionID = 0)
        {
            if (LiasionID > 0)
            {

                //var patientids = _db.Patients.AsNoTracking().Where(x => x.LiaisonId == LiasionID).ToList().Select(x => x.Id).ToList();
                //var istranlator = _db.Liaisons.Where(x => x.Id == LiasionID).FirstOrDefault().IsTranslator;
                //if (istranlator == true)
                //{
                //    patientids = _db.Patients.AsNoTracking().Where(x => x.TranslatorId == LiasionID).ToList().Select(x => x.Id).ToList();
                //}
                var userid = _db.Users.Where(x => x.Role == "" && x.CCMid == LiasionID).FirstOrDefault().Id;
                var calls = _db.TextHistories.AsNoTracking().Where(x => x.FromUser==userid).ToList().Where(x => x.DateTime.Date >= From.Value.Date && x.DateTime.Date <= To.Value.Date).OrderByDescending(x => x.DateTime).ToList();
                return PartialView(calls);
            }
            else
            {
                return PartialView(await _db.TextHistories.Where(th => th.PatientId == patientId).ToListAsync());
            }
        }
        public async Task<PartialViewResult> _TextHistoryPartial1(int? patientId, DateTime? From, DateTime? To, int LiasionID = 0)
        {
            if (LiasionID > 0)
            {

                //var patientids = _db.Patients.AsNoTracking().Where(x => x.LiaisonId == LiasionID && x.TranslatorId==null).ToList().Select(x => x.Id).ToList();
                //var istranlator = _db.Liaisons.Where(x => x.Id == LiasionID).FirstOrDefault().IsTranslator;
                //if (istranlator == true)
                //{
                //    patientids = _db.Patients.AsNoTracking().Where(x => x.TranslatorId == LiasionID).ToList().Select(x => x.Id).ToList();
                //}
                var userid = _db.Users.Where(x => x.Role == "" && x.CCMid == LiasionID).FirstOrDefault().Id;
                var calls = _db.TextHistories.AsNoTracking().Where(x => x.FromUser==userid).ToList().Where(x => x.DateTime.Date >= From.Value.Date && x.DateTime.Date <= To.Value.Date).OrderByDescending(x => x.DateTime).ToList();
                return PartialView(calls);
            }
            else
            {
                return PartialView(await _db.TextHistories.Where(th => th.PatientId == patientId).ToListAsync());
            }
        }
        public async Task<PartialViewResult> _EmailHistoryPartial(int? patientId, DateTime? From, DateTime? To, int LiasionID = 0)
        {
            if (LiasionID > 0)
            {
                //var patientids = _db.Patients.AsNoTracking().Where(x => x.LiaisonId == LiasionID).ToList().Select(x => x.Id).ToList();
                //var istranlator = _db.Liaisons.Where(x => x.Id == LiasionID).FirstOrDefault().IsTranslator;
                //if (istranlator == true)
                //{
                //    patientids = _db.Patients.AsNoTracking().Where(x => x.TranslatorId == LiasionID).ToList().Select(x => x.Id).ToList();
                //}
                //var calls = _db.EmailHistories.AsNoTracking().Where(x => patientids.Contains(x.PatientId)).ToList().Where(x => x.DateTime.Date >= From.Value.Date && x.DateTime.Date <= To.Value.Date).OrderByDescending(x => x.DateTime).ToList();
                var userid = _db.Users.Where(x => x.Role == "" && x.CCMid == LiasionID).FirstOrDefault().Id;
                var calls = _db.EmailHistories.AsNoTracking().Where(x => x.FromUser==userid).ToList().Where(x => x.DateTime.Date >= From.Value.Date && x.DateTime.Date <= To.Value.Date).OrderByDescending(x => x.DateTime).ToList();
                return PartialView(calls);
            }
            else
            {
                return PartialView(await _db.EmailHistories.Where(th => th.PatientId == patientId).ToListAsync());
            }
        }
        public async Task<PartialViewResult> _EmailHistoryPartial1(int? patientId, DateTime? From, DateTime? To, int LiasionID = 0)
        {
            if (LiasionID > 0)
            {
                //var patientids = _db.Patients.AsNoTracking().Where(x => x.LiaisonId == LiasionID).ToList().Select(x => x.Id).ToList();
                //var istranlator = _db.Liaisons.Where(x => x.Id == LiasionID).FirstOrDefault().IsTranslator;
                //if (istranlator == true)
                //{
                //    patientids = _db.Patients.AsNoTracking().Where(x => x.TranslatorId == LiasionID).ToList().Select(x => x.Id).ToList();
                //}
                var userid = _db.Users.Where(x => x.Role == "" && x.CCMid == LiasionID).FirstOrDefault().Id;
                var calls = _db.EmailHistories.AsNoTracking().Where(x => x.FromUser==userid).ToList().Where(x => x.DateTime.Date >= From.Value.Date && x.DateTime.Date <= To.Value.Date).OrderByDescending(x => x.DateTime).ToList();
                return PartialView(calls);
            }
            else
            {
                return PartialView(await _db.EmailHistories.Where(th => th.PatientId == patientId).ToListAsync());
            }
        }




        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}