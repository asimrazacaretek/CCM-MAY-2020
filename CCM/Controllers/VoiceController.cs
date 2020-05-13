using CCM.Models;
using Microsoft.AspNet.Identity;
using System.Data.Entity;
using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using Twilio.TwiML;
using Twilio.TwiML.Voice;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using Twilio.AspNet.Mvc;
using Twilio;
using System.Net;
using Twilio.Rest.Api.V2010.Account;

namespace CCM.Controllers
{

   
    public class VoiceController : TwilioController
    {
        private readonly ApplicationdbContect _db = new ApplicationdbContect();
       


        public void WriteErrorLog(System.Exception ex)
        {
            try
            {


                string webPageName = Path.GetFileName(Request.Path);
                string errorLogFilename = "ErrorLog_" + DateTime.Now.ToString("dd-MM-yyyy") + ".txt";
                string path = Server.MapPath("~/ErrorLogFiles/" + errorLogFilename);
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
        [HttpPost]
        public async Task<ActionResult> Index(string to)
        {
            var from = Request["From"];
            var to1 = Request["To"];
            var callsid = Request["CallSid"];
            var patientid= Request["PatientID"];
            var liaisonId = Request["LiaisonID"];
            //var fromcode = Request["CallerCountry"];
            //var tocode = Request["ToCountry"];
            //var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(_db));
            //ApplicationUser currentUser = UserManager.FindById(User.Identity.GetUserId());
            var callerid = ConfigurationManager.AppSettings["TwilioCallerId"];
            try
            {
                var callHistory = new CallHistory()
                {


                    To = to,
                    From = from,
                    PatientID =Convert.ToInt32(patientid),
                    TwilioCallId = callsid,
                    Direction="Outgoing",
                    LiaisonId=Convert.ToInt32(liaisonId)

                };
                _db.CallHistories.Add(callHistory);
                _db.SaveChanges();

            }
            catch (Exception ex)
            {

                WriteErrorLog(ex);
            }
            try
            {
                var Calleridnew = Request["From"];
                if (!string.IsNullOrEmpty(Calleridnew))
                {
                    callerid = Calleridnew;
                }
            }
            catch (Exception ex)
            {
                WriteErrorLog(ex);
              
            }
            if (!string.IsNullOrEmpty(to))
            {
                return Content(new VoiceResponse()
                              .Dial(new Dial(callerId: callerid,
                                  record: "Record-from-ringing", recordingStatusCallback: Url.ActionUri("SaveRecordings", "Voice"), recordingStatusCallbackMethod: Twilio.Http.HttpMethod.Post).Number(to)).ToString(), "text/xml");
            }
            else
            {
                var response = new VoiceResponse();
                response.Say("Thanks for calling!");
                return Content(response.ToString(), "text/xml");
            }

        }
        [HttpPost]
        public  ActionResult Receive(string CallerName)
        {
            //CallerName = CallerName.Replace("+1", "");
            //CallerName = "Phonenum" + CallerName;
            var from = Request["From"];
            var to = Request["To"];
            var callsid = Request["CallSid"];
            var callaction= Request["action"];
            var digits = Request["Digits"];
            var steps= Request["step"]; 
            var fromcode= Request["CallerCountry"];
            var tocode= Request["ToCountry"];
            var callerId = ConfigurationManager.AppSettings["TwilioCallerId"];
            try
            {
                var liasionid = _db.Liaisons.AsNoTracking().Where(x => x.TwilioCallerId == to).FirstOrDefault()?.Id;
                var callHistory = new CallHistory()
                {


                    To = to.Replace(HelperExtensions.TelCodes.Where(x=>x.Iso==tocode).FirstOrDefault().Pfx,""),
                    From = from.Replace(HelperExtensions.TelCodes.Where(x => x.Iso == fromcode).FirstOrDefault().Pfx, ""),
                    Status="Incoming/Not Answer",
                    StartTime=DateTime.Now,
                    TwilioCallId = callsid,
                    Direction="Incoming",
                    LiaisonId=liasionid??0

                };
                _db.CallHistories.Add(callHistory);
                _db.SaveChanges();

            }
            catch (Exception ex)
            {

                WriteErrorLog(ex);
            }
            //test
            
                    var response = new VoiceResponse();
            if (Request["Digits"] == "0")
            {
                var dial = new Dial(callerId: to, record: "Record-from-answer",action: Url.ActionUri("VoiceMail", "Voice"),recordingStatusCallback:Url.ActionUri("SaveRecordings", "Voice"),recordingStatusCallbackMethod:Twilio.Http.HttpMethod.Post);
                dial.Client(CallerName);
               
                response.Dial(dial);

                return TwiML(response);
            }
            else
            {
              //  var dial = new Dial(callerId: to, record: "Record-from-answer");
                var dial = new Dial( record: "Record-from-ringing",  recordingStatusCallback: Url.ActionUri("SaveRecordings", "Voice"), recordingStatusCallbackMethod: Twilio.Http.HttpMethod.Post);
                dial.Client(CallerName);
                //dial.Action = new Uri("/Voice/SaveRecordings");
                //dial.Method = Twilio.Http.HttpMethod.Post;
                //var gather = new Gather(action: Url.ActionUri("Receive", "Voice",""), numDigits: 1, method: Twilio.Http.HttpMethod.Post, timeout: 5);
                //gather.Say("Thank you for calling " +
                //           "Press 0 to make a call.", voice: Say.VoiceEnum.Woman, loop: 2);
                response.Append(new Say("Thank you for calling " +
                           "Please wait we are connecting your call.", voice: Say.VoiceEnum.Woman, loop: 1)).Append(dial).Append(new Say("Please record voice mail after beep if not answer",voice:Say.VoiceEnum.Woman)).Append(new Record(action:Url.ActionUri("VoiceMail","Voice"),method:Twilio.Http.HttpMethod.Post,timeout:3,playBeep:true));
                return TwiML(response);
               
            }
            
           

            //var response = new VoiceResponse();
            //response.Say("Thanks for calling!");

            //return Content(response.ToString(), "text/xml");
        }
        [HttpPost]
        public async System.Threading.Tasks.Task<ActionResult> VoiceMail()
        {
            var recordingurl = Request["RecordingUrl"];
            var callsid = Request["CallSid"];
            var recordingsid = Request["RecordingSid"];
            var fromcode = Request["CallerCountry"];
            var tocode = Request["ToCountry"];
            try
            {


                TwilioClient.Init(ConfigurationManager.AppSettings["TwilioAccountSid"], ConfigurationManager.AppSettings["TwilioAuthToken"]);
                var calls = await CallResource.ReadAsync(parentCallSid: (callsid));

                if (calls.Any())
                {
                    int? patientid = 0;
                    foreach (var call in calls)
                    {


                        var callHistory = _db.CallHistories.AsNoTracking().Where(x => x.TwilioCallId == call.ParentCallSid).FirstOrDefault();
                       
                        try
                        {
                            //callHistory.From = callHistory.From.Replace("+1", "");
                            patientid = _db.Patients.AsNoTracking().Where(x => (x.MobilePhoneNumber != null && x.MobilePhoneNumber.Replace(" ","").Contains(callHistory.From)) || (x.WorkPhoneNumber != null && x.WorkPhoneNumber.Replace(" ","").Contains(callHistory.From)) || (x.EmergencyNumber != null && x.EmergencyNumber.Replace(" ","").Contains(callHistory.From)) || (x.HomePhoneNumber != null && x.HomePhoneNumber.Replace(" ","").Contains(callHistory.From)) || (x.CaretakerPhoneNumber != null && x.CaretakerPhoneNumber.Replace(" ","").Contains(callHistory.From))).FirstOrDefault()?.Id;
                            if (patientid == null)
                            {
                                patientid = _db.PatientProfile_Contact.AsNoTracking().Where(x => (x.CellPhoneNumber != null && x.CellPhoneNumber.Replace(" ","").Contains(callHistory.From)) || (x.CellPhoneNumber1 != null && x.CellPhoneNumber1.Replace(" ","").Contains(callHistory.From)) || (x.CellPhoneNumber2 != null && x.CellPhoneNumber2.Replace(" ","").Contains(callHistory.From)) || (x.EmergencyNumber != null && x.EmergencyNumber.Replace(" ","").Contains(callHistory.From)) || (x.EmergencyNumber1 != null && x.EmergencyNumber1.Replace(" ","").Contains(callHistory.From)) || (x.EmergencyNumber2 != null && x.EmergencyNumber2.Replace(" ","").Contains(callHistory.From)) || (x.HomePhoneNumber != null && x.HomePhoneNumber.Replace(" ","").Contains(callHistory.From)) || (x.HomePhoneNumber1 != null && x.HomePhoneNumber1.Replace(" ","").Contains(callHistory.From)) || (x.HomePhoneNumber2 != null && x.HomePhoneNumber2.Replace(" ","").Contains(callHistory.From)) || (x.WorkPhoneNumber != null && x.WorkPhoneNumber.Replace(" ","").Contains(callHistory.From)) || (x.WorkPhoneNumber1 != null && x.WorkPhoneNumber1.Replace(" ","").Contains(callHistory.From)) || (x.WorkPhoneNumber2 != null && x.WorkPhoneNumber2.Replace(" ","").Contains(callHistory.From))).FirstOrDefault()?.PatientId;
                                if (patientid == null)
                                {
                                    patientid = _db.PatientProfile_UrgencyContacts.AsNoTracking().Where(x => (x.PrimaryHomePhoneNumber != null && x.PrimaryHomePhoneNumber.Replace(" ","").Contains(callHistory.From)) || (x.PrimaryHomePhoneNumber1 != null && x.PrimaryHomePhoneNumber1.Replace(" ","").Contains(callHistory.From)) || (x.PrimaryHomePhoneNumber2 != null && x.PrimaryHomePhoneNumber2.Replace(" ","").Contains(callHistory.From)) || (x.PrimaryMobilePhoneNumber != null && x.PrimaryMobilePhoneNumber.Replace(" ","").Contains(callHistory.From)) || (x.PrimaryMobilePhoneNumber1 != null && x.PrimaryMobilePhoneNumber1.Replace(" ","").Contains(callHistory.From)) || (x.PrimaryMobilePhoneNumber2 != null && x.PrimaryMobilePhoneNumber2.Replace(" ","").Contains(callHistory.From)) || (x.SecondaryHomePhoneNumber != null && x.SecondaryHomePhoneNumber.Replace(" ","").Contains(callHistory.From)) || (x.SecondaryHomePhoneNumber1 != null && x.SecondaryHomePhoneNumber1.Replace(" ","").Contains(callHistory.From)) || (x.SecondaryHomePhoneNumber2 != null && x.SecondaryHomePhoneNumber2.Replace(" ","").Contains(callHistory.From)) || (x.SecondaryMobilePhoneNumber != null && x.SecondaryMobilePhoneNumber.Replace(" ","").Contains(callHistory.From)) || (x.SecondaryMobilePhoneNumber1 != null && x.SecondaryMobilePhoneNumber1.Replace(" ","").Contains(callHistory.From)) || (x.SecondaryMobilePhoneNumber2 != null && x.SecondaryMobilePhoneNumber2.Replace(" ","").Contains(callHistory.From))).FirstOrDefault()?.PatientId;
                                    if (patientid == null)
                                    {
                                        patientid = _db.CallHistories.AsNoTracking().Where(x => x.To.Replace(" ","").Contains(callHistory.From)).FirstOrDefault()?.PatientID;
                                    }
                                }
                            }
                            if (patientid == null)
                            {
                                patientid = 0;
                            }
                        }
                        catch (Exception ex)
                        {


                        }
                        var callHistory1 = new VoiceMailHistory()
                        {
                            StartTime = call?.StartTime,
                            EndTime = call?.EndTime,
                            Status = call?.Status.ToString(),
                            Duration = call?.StartTime != null && call.EndTime != null
                                                         ? (TimeSpan)(call.EndTime - call.StartTime)
                                                         : TimeSpan.Zero,
                            To = callHistory?.To,
                            From = callHistory?.From,
                            PatientID =patientid.Value,
                            TwilioCallId = call?.ParentCallSid,
                            LiaisonId=callHistory.LiaisonId

                        };
                        try
                        {

                       
                        callHistory1.RecordingURL = recordingurl;
                        var src = recordingurl;
                        var fullSource = src + ".mp3";
                        WebClient webClient = new WebClient();
                        string physicalPath = Server.MapPath("~/VoiceMails/" + callHistory1.TwilioCallId + ".mp3");
                        webClient.DownloadFile(fullSource, physicalPath);
                        }
                        catch (Exception ex)
                        {

                            WriteErrorLog(ex);
                        }
                        _db.voiceMailHistories.Add(callHistory1);
                        _db.SaveChanges();
                    }

                }
            }
            catch (Exception ex)
            {
                WriteErrorLog(ex);

            }
            var response = new VoiceResponse();
            response.Append(new Hangup());
            return TwiML(response);

        }
        [HttpPost]
        public async System.Threading.Tasks.Task SaveRecordings()
        {
            var recordingurl = Request["RecordingUrl"];
            var callsid = Request["CallSid"];
            var callfrom = Request["From"];
            var recordingsid = Request["RecordingSid"];
            var fromcode = Request["CallerCountry"];
            var tocode = Request["ToCountry"];
            try
            {

               
                TwilioClient.Init(ConfigurationManager.AppSettings["TwilioAccountSid"], ConfigurationManager.AppSettings["TwilioAuthToken"]);
                var calls = await CallResource.ReadAsync(parentCallSid: (callsid));
               
                if (calls.Any())
                {
                    int? patientid=0;
                    foreach (var call in calls)
                    {
                        
                        

                        var callHistory = _db.CallHistories.Where(x => x.TwilioCallId == call.ParentCallSid).FirstOrDefault();
                        if (callHistory !=null)
                        {
                            
                            try
                            {
                               callHistory.From = callHistory.From.Replace("+1", "");
                                patientid = _db.Patients.AsNoTracking().Where(x => (x.MobilePhoneNumber != null && x.MobilePhoneNumber.Replace("","").Contains(callHistory.From)) || (x.WorkPhoneNumber != null && x.WorkPhoneNumber.Replace(" ","").Contains(callHistory.From)) || (x.EmergencyNumber != null && x.EmergencyNumber.Replace(" ","").Contains(callHistory.From)) || (x.HomePhoneNumber != null && x.HomePhoneNumber.Replace(" ","").Contains(callHistory.From)) || (x.CaretakerPhoneNumber != null && x.CaretakerPhoneNumber.Replace(" ","").Contains(callHistory.From))).FirstOrDefault()?.Id;
                                if (patientid == null)
                                {
                                    patientid = _db.PatientProfile_Contact.AsNoTracking().Where(x => (x.CellPhoneNumber != null && x.CellPhoneNumber.Replace(" ","").Contains(callHistory.From)) || (x.CellPhoneNumber1 != null && x.CellPhoneNumber1.Replace(" ","").Contains(callHistory.From)) || (x.CellPhoneNumber2 != null && x.CellPhoneNumber2.Replace(" ","").Contains(callHistory.From)) || (x.EmergencyNumber != null && x.EmergencyNumber.Replace(" ","").Contains(callHistory.From)) || (x.EmergencyNumber1 != null && x.EmergencyNumber1.Replace(" ","").Contains(callHistory.From)) || (x.EmergencyNumber2 != null && x.EmergencyNumber2.Replace(" ","").Contains(callHistory.From)) || (x.HomePhoneNumber != null && x.HomePhoneNumber.Replace(" ","").Contains(callHistory.From)) || (x.HomePhoneNumber1 != null && x.HomePhoneNumber1.Replace(" ","").Contains(callHistory.From)) || (x.HomePhoneNumber2 != null && x.HomePhoneNumber2.Replace(" ","").Contains(callHistory.From)) || (x.WorkPhoneNumber != null && x.WorkPhoneNumber.Replace(" ","").Contains(callHistory.From)) || (x.WorkPhoneNumber1 != null && x.WorkPhoneNumber1.Replace(" ","").Contains(callHistory.From)) || (x.WorkPhoneNumber2 != null && x.WorkPhoneNumber2.Replace(" ","").Contains(callHistory.From))).FirstOrDefault()?.PatientId;
                                    if (patientid == null)
                                    {
                                        patientid = _db.PatientProfile_UrgencyContacts.AsNoTracking().Where(x => (x.PrimaryHomePhoneNumber != null && x.PrimaryHomePhoneNumber.Replace(" ","").Contains(callHistory.From)) || (x.PrimaryHomePhoneNumber1 != null && x.PrimaryHomePhoneNumber1.Replace(" ","").Contains(callHistory.From)) || (x.PrimaryHomePhoneNumber2 != null && x.PrimaryHomePhoneNumber2.Replace(" ","").Contains(callHistory.From)) || (x.PrimaryMobilePhoneNumber != null && x.PrimaryMobilePhoneNumber.Replace(" ","").Contains(callHistory.From)) || (x.PrimaryMobilePhoneNumber1 != null && x.PrimaryMobilePhoneNumber1.Replace(" ","").Contains(callHistory.From)) || (x.PrimaryMobilePhoneNumber2 != null && x.PrimaryMobilePhoneNumber2.Replace(" ","").Contains(callHistory.From)) || (x.SecondaryHomePhoneNumber != null && x.SecondaryHomePhoneNumber.Replace(" ","").Contains(callHistory.From)) || (x.SecondaryHomePhoneNumber1 != null && x.SecondaryHomePhoneNumber1.Replace(" ","").Contains(callHistory.From)) || (x.SecondaryHomePhoneNumber2 != null && x.SecondaryHomePhoneNumber2.Replace(" ","").Contains(callHistory.From)) || (x.SecondaryMobilePhoneNumber != null && x.SecondaryMobilePhoneNumber.Replace(" ","").Contains(callHistory.From)) || (x.SecondaryMobilePhoneNumber1 != null && x.SecondaryMobilePhoneNumber1.Replace(" ","").Contains(callHistory.From)) || (x.SecondaryMobilePhoneNumber2 != null && x.SecondaryMobilePhoneNumber2.Replace(" ","").Contains(callHistory.From))).FirstOrDefault()?.PatientId;
                                        if (patientid == null)
                                        {
                                            patientid = _db.CallHistories.AsNoTracking().Where(x => x.To.Replace(" ","").Contains(callHistory.From)).FirstOrDefault()?.PatientID;
                                        }
                                    }
                                }
                                if (patientid == null)
                                {
                                    patientid = 0;
                                }
                            }
                            catch (Exception ex)
                            {
                                WriteErrorLog(ex);

                            }
                            callHistory.StartTime = call?.StartTime;
                            callHistory.EndTime = call?.EndTime;
                            callHistory.Status = call?.Status.ToString();
                            callHistory.Duration = call?.StartTime != null && call.EndTime != null
                                                         ? (TimeSpan)(call.EndTime - call.StartTime)
                                                         : TimeSpan.Zero;

                            callHistory.PatientID = callHistory.PatientID > 0 ? callHistory.PatientID : patientid.Value;
                            
                            

                        };

                        //var recordings = await RecordingResource.ReadAsync(callSid: call?.ParentCallSid);
                        //if (recordings.Any())
                        //{
                        //    foreach (var recording in recordings)
                        //        callHistory.RecordingURL = recording?.Uri;
                        //    var src = callHistory.RecordingURL.Substring(0, callHistory.RecordingURL.IndexOf('.'));
                        //    var fullSource = "https://api.twilio.com" + src + ".mp3";
                        //    WebClient webClient = new WebClient();
                        //    string physicalPath = Server.MapPath("~/Recordings/" + callHistory.TwilioCallId + ".mp3");
                        //    webClient.DownloadFile(fullSource, physicalPath);
                        //}
                        try
                        {
                            callHistory.RecordingURL = recordingurl;
                            var src = recordingurl;
                            var fullSource = src + ".mp3";
                            WebClient webClient = new WebClient();
                            string physicalPath = Server.MapPath("~/Recordings/" + callHistory.TwilioCallId + ".mp3");
                            webClient.DownloadFile(fullSource, physicalPath);
                        }
                        catch (Exception ex)
                        {
                            WriteErrorLog(ex);
                           
                        }
                       

                        _db.Entry(callHistory).State=EntityState.Modified;
                        _db.SaveChanges();
                    }

                }
            }
            catch (Exception ex)
            {
                WriteErrorLog(ex);

            }
            
        }

    }
}