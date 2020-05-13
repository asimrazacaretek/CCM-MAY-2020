using CCM.Models;
using Microsoft.AspNet.Identity;
using Rotativa;
using Rotativa.Options;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web.Configuration;
using System.Web.Mvc;

namespace CCM.Controllers
{
    [Authorize]
    public class FinalCarePlanController : BaseController
    {
        //private readonly ApplicationdbContect _db = new ApplicationdbContect();


        private async Task<FinalCarePlanViewModel> FinalCarePlan(int? patientId)
        {
            var patient = await _db.Patients.FindAsync(patientId);
            var doctorVisit = await _db.DoctorVisits.AsNoTracking().Where(dv => dv.PatientId == patient.Id).OrderByDescending(dv => dv.VisitDate).FirstOrDefaultAsync();
            var version = _db.FinalCarePlanNotes.AsNoTracking().Where(x => x.PatientId == patientId).OrderByDescending(x => x.CarePlanCreatedOn).FirstOrDefault()?.Version;
            var results = patient != null
                           ? new FinalCarePlanViewModel
                           {
                               CCMEnrolledOn = patient.CCMEnrolledOn,
                               PatientId = patient.Id,
                               FinalCarePlanPdf = patient.FinalCarePlanPdf,
                               LiaisonName = patient.Liaison?.FirstName + " " + patient.Liaison?.LastName,
                               PhysicianName = patient.Physician?.FirstName + " " + patient.Physician?.LastName,
                               LastVisited = doctorVisit?.VisitDate,
                               NextAppointment = doctorVisit?.NextAppointment,
                               PhysicianSpeciality = patient.Physician.Specialty,
                               PatientPhoto = patient.Photo,
                               LiaisonPhoto = patient.Liaison?.UserPhoto,
                               PhysicianPhoto = patient.Physician?.Photo,

                               PatientName = patient.Prefix + " " + patient.FirstName + " "
                                                    + patient.MiddleName + " " + patient.LastName,
                               HomePhoneNumber = patient.HomePhoneNumber,
                               MobilePhoneNumber = patient.MobilePhoneNumber,
                               Address = patient.Address1 + " " + patient.Address2 + "\n" +
                                                      patient.City + ", " + patient.State + " - " + patient.Zipcode,
                               BirthDate = patient.BirthDate,
                               Email = patient.Email,
                               Gender = patient.Gender ?? "",
                               preferredLangugae = patient.PreferredLanguage,
                               MedicationRx = await _db.PatientMedicalHistory_MedicationRxes.AsNoTracking().Where(m => m.PatientId == patient.Id).ToListAsync(),
                               SecondaryDoctors = await _db.SecondaryDoctors.AsNoTracking().Where(m => m.PatientId == patient.Id).ToListAsync(),
                               UrgencyContact = await _db.PatientProfile_UrgencyContacts.FindAsync(patient.UrgencyContactId),
                               FinalCarePlan = await _db.FinalCarePlanNotes.AsNoTracking().FirstOrDefaultAsync(fcp => fcp.PatientId == patient.Id && fcp.Version == version),
                               Icd10Codes = await _db.Icd10Codes.AsNoTracking().Where(m => m.PatientId == patient.Id).ToListAsync(),
                               lstUrgencyContact = await _db.PatientProfile_UrgencyContacts.AsNoTracking().Where(m => m.PatientId == patient.Id).ToListAsync(),
                               jsonresult = await HospitalGraph(patient.Id)
                           }
                            : null;
            return results;
        }

        [HttpGet]
        public async Task<JsonResult> HospitalGraph(int? id)
        {
            var DateFrom = DateTime.Now.AddMonths(-11);
            var DateTo = DateTime.Now;
            var datediff = DateFrom - DateTo;
            int startmonth = DateFrom.Month;
            int endmonth = DateTo.Month;
            int startyear = DateFrom.Year;
            int endyear = DateTo.Year;
            if (endyear > startyear)
            {
                endmonth = 12;
            }
            var totalmonths = Math.Abs((DateFrom.Month - DateTo.Month) + 12 * (DateFrom.Year - DateTo.Year));
            List<string> StartEndMonth = new List<string>();
            List<string> lstMonths = new List<string>() { "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec" };
          
            var _PatientProfile_HospitalDetails = _db.patientProfile_Hospitalvisits.Where(x => x.PatientId == id && x.AdmitDate != null).ToList().Where(x => x.AdmitDate.Date >= DateFrom.Date && x.AdmitDate <= DateTo.Date).ToList();
            if (endyear > startyear)
            {
                endmonth = 12;
            }
            List<GraphData> GraphDatas = new List<GraphData>();
            for (int ii = startyear; ii <= endyear; ii++)
            {
                if (ii > startyear && ii == endyear)
                {
                    startmonth = 1;
                    endmonth = DateTo.Month;
                }
                else
                {
                    if (ii > startyear && ii < endyear)
                    {
                        startmonth = 1;
                        endmonth = 12;
                    }
                }

                for (int jj = startmonth; jj <= endmonth; jj++)
                {
                    GraphData monthNames = new GraphData();
                    monthNames.Month = jj;
                    monthNames.Year = ii;
                    monthNames.TotalCost = _PatientProfile_HospitalDetails.Where(x => x.AdmitDate.Year == ii && x.AdmitDate.Month == jj).Sum(x => x.TotalDays * x.Rate);
                    monthNames.TotalVisit = _PatientProfile_HospitalDetails.Where(x => x.AdmitDate.Year == ii && x.AdmitDate.Month == jj).Count();
                    monthNames.Average = monthNames.TotalVisit != 0 ? monthNames.TotalCost / monthNames.TotalVisit : 0;
                    GraphDatas.Add(monthNames);
                }
            }

            //var sss = new[] { new { Field1 = "aa", Field2 = 1 } };
            //Patient p;
            DateTime firstDay = new DateTime(endyear, 1, 1);
            var lastyear = _PatientProfile_HospitalDetails.Where(x => x.AdmitDate <= DateTo).Where(x => x.AdmitDate >= DateFrom).Select(x => x.TotalDays).Sum();
            var countdays= _PatientProfile_HospitalDetails.Where(x => x.AdmitDate <= DateTo).Where(x => x.AdmitDate >= DateFrom).Select(x => x.TotalDays).Count();
            var jantocurrent = _PatientProfile_HospitalDetails.Where(x => x.AdmitDate <= DateTo).Where(x => x.AdmitDate >= firstDay).Select(x => x.TotalDays).Sum();

            var varlastyear= lastyear * 1500;
            jantocurrent = jantocurrent * 1500;
            
            var result = new { Result = GraphDatas, JanToCurrent = jantocurrent, totalday=countdays,last365days= varlastyear };
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        private async Task<FinalCarePlanViewModel> FinalCarePlanForVersion(int? patientId, Guid? version)
        {
            //Same as above, but this function takes care plan version, not just patient ID
            return FinalCarePlanForVersionData(patientId, version);
        }
        private FinalCarePlanViewModel FinalCarePlanForVersionData(int? patientId, Guid? version)
        {
            //Same as above, but this function takes care plan version, not just patient ID
            try
            {


                var patient = _db.Patients.Find(patientId);
                var doctorVisit = _db.DoctorVisits.AsNoTracking().Where(dv => dv.PatientId == patient.Id).OrderByDescending(dv => dv.VisitDate).FirstOrDefault();
                var results = patient != null
                               ? new FinalCarePlanViewModel
                               {
                                   CCMEnrolledOn = patient.CCMEnrolledOn,
                                   PatientId = patient.Id,
                                   FinalCarePlanPdf = patient.FinalCarePlanPdf,
                                   LiaisonName = patient.Liaison?.FirstName + " " + patient.Liaison?.LastName,
                                   PhysicianName = patient.Physician?.FirstName + " " + patient.Physician?.LastName,
                                   LastVisited = doctorVisit?.VisitDate,
                                   NextAppointment = doctorVisit?.NextAppointment,
                                   PhysicianSpeciality = patient.Physician.Specialty,
                                   PatientPhoto = patient.Photo,
                                   LiaisonPhoto = patient.Liaison?.UserPhoto,
                                   PhysicianPhoto = patient.Physician?.Photo,

                                   PatientName = patient.Prefix + " " + patient.FirstName + " "
                                                    + patient.MiddleName + " " + patient.LastName,
                                   HomePhoneNumber = patient.HomePhoneNumber,
                                   MobilePhoneNumber = patient.MobilePhoneNumber,
                                   Address = patient.Address1 + " " + patient.Address2 + "\n" +
                                                      patient.City + ", " + patient.State + " - " + patient.Zipcode,
                                   BirthDate = patient.BirthDate,
                                   Email = patient.Email,
                                   Gender = patient.Gender ?? "",
                                   preferredLangugae = patient.PreferredLanguage,
                                   MedicationRx = _db.PatientMedicalHistory_MedicationRxes.AsNoTracking().Where(m => m.PatientId == patient.Id).ToList(),
                                   SecondaryDoctors = _db.SecondaryDoctors.AsNoTracking().Where(m => m.PatientId == patient.Id).ToList(),
                                   UrgencyContact = _db.PatientProfile_UrgencyContacts.Find(patient.UrgencyContactId),
                                   FinalCarePlan = _db.FinalCarePlanNotes.AsNoTracking().FirstOrDefault(fcp => fcp.PatientId == patient.Id && fcp.Version == version),
                                   Icd10Codes = _db.Icd10Codes.AsNoTracking().Where(m => m.PatientId == patient.Id).ToList(),
                                   lstUrgencyContact = _db.PatientProfile_UrgencyContacts.AsNoTracking().Where(m => m.PatientId == patient.Id).ToList(),

                                   //PatientId = patient.Id,
                                   //FinalCarePlanPdf = patient.FinalCarePlanPdf,
                                   //LiaisonName = patient.Liaison?.FirstName + " " + patient.Liaison?.LastName,
                                   //PhysicianName = patient.Physician?.FirstName + " " + patient.Physician?.LastName,
                                   //LastVisited = doctorVisit?.VisitDate,
                                   //NextAppointment = doctorVisit?.NextAppointment,
                                   //PatientPhoto = patient.Photo,
                                   //LiaisonPhoto = patient.Liaison?.UserPhoto,
                                   //PhysicianPhoto = patient.Physician?.Photo,
                                   //PatientName = patient.Prefix + " " + patient.FirstName + " "
                                   //                     + patient.MiddleName + " " + patient.LastName,
                                   //HomePhoneNumber = patient.HomePhoneNumber,
                                   //MobilePhoneNumber = patient.MobilePhoneNumber,
                                   //Address = patient.Address1 + " " + patient.Address2 + "\n" +
                                   //                       patient.City + ", " + patient.State + " - " + patient.Zipcode,
                                   //BirthDate = patient.BirthDate,
                                   //Email = patient.Email,
                                   //Gender = patient.Gender ?? "",
                                   //preferredLangugae = patient.PreferredLanguage,
                                   //MedicationRx = _db.PatientMedicalHistory_MedicationRxes.AsNoTracking().Where(m => m.PatientId == patient.Id).ToList(),
                                   //SecondaryDoctors = _db.SecondaryDoctors.AsNoTracking().Where(m => m.PatientId == patient.Id).ToList(),
                                   //UrgencyContact = _db.PatientProfile_UrgencyContacts.Find(patient.UrgencyContactId),
                                   //FinalCarePlan = _db.FinalCarePlanNotes.AsNoTracking().FirstOrDefault(fcp => (fcp.PatientId == patient.Id) && (fcp.Version == version)),
                                   //Icd10Codes = _db.Icd10Codes.AsNoTracking().Where(x => x.PatientId == patient.Id).ToList()
                               }
                                : null;
                return results;
            }
            catch (Exception ex)
            {

                return new FinalCarePlanViewModel();
            }
        }


        public async Task<ActionResult> Patient(int? patientId)
        {
            if (await FinalCarePlan(patientId) != null)
                return View(await FinalCarePlan(patientId));

            ViewBag.Message = "Patient Not Found.";
            return View("Error");
        }


        public async Task<ActionResult> View4Pdf(int? patientId)
        {
            if (await FinalCarePlan(patientId) != null)
                return View(await FinalCarePlan(patientId));

            ViewBag.Message = "Patient Not Found!";
            return View("Error");
        }
        public async Task<ActionResult> _View4Pdf(int? patientId)
        {
            if (await FinalCarePlan(patientId) != null)
                return View(await FinalCarePlan(patientId));

            ViewBag.Message = "Patient Not Found!";
            return View("Error");
        }

        private string ConvertViewToString(string viewName, object model)
        {
            try
            {


                ViewData.Model = model;
                using (StringWriter writer = new StringWriter())
                {
                    ViewEngineResult vResult = ViewEngines.Engines.FindPartialView(ControllerContext, viewName);
                    ViewContext vContext = new ViewContext(this.ControllerContext, vResult.View, ViewData, new TempDataDictionary(), writer);
                    vResult.View.Render(vContext, writer);
                    return writer.ToString();
                }
            }
            catch (Exception ed)
            {
                return "";

            }
        }
        public static Byte[] PdfSharpConvert(String html)
        {
            Byte[] res = null;
            using (MemoryStream ms = new MemoryStream())
            {
                //var pdf = TheArtOfDev.HtmlRenderer.PdfSharp.PdfGenerator.GeneratePdf(html, PdfSharp.PageSize.A4);
                //pdf.Save(ms);
                res = ms.ToArray();
            }
            return res;
        }
        public async Task<string> GetHTMLForPDF(int ? patientId)
        {
            var str = ConvertViewToString("_View4Pdf", await FinalCarePlan(patientId));
            return str;
        }
        public async Task<ActionResult> GenerateFinalCarePdf(int? patientId)
        {
            var patient = await _db.Patients.FindAsync(patientId);
            if (patient != null && await FinalCarePlan(patientId) != null)
            {
                var footer = "--footer-center \"Last Updated on " + DateTime.Now.ToString("MM/dd/yyyy @ hh:mmtt ") +
                             "- Page: [page]/[toPage]\"" + " --footer-font-size \"8\" --footer-spacing \"6\" --footer-font-name \"times\"";
                
                var generatePdf = new ViewAsPdf("_View4Pdf", await FinalCarePlan(patientId))
                {
                    FileName = "CCM_Health_" + patient.FirstName + "_" + patient.LastName + ".pdf",
                    //MinimumFontSize =10,
                    CustomSwitches = footer,
                    IsLowQuality = false,

                    PageOrientation = Rotativa.Options.Orientation.Portrait,
                    
                    PageSize = Rotativa.Options.Size.A5,
                    PageMargins = new Margins(5, 5, 5,5),
                    //PageMargins = new Margins(3, 3,3, 3),
                    //PageMargins = { Left = 20, Bottom = 20, Right = 20, Top = 20 }
                };

                patient.FinalCarePlanPdf = generatePdf.BuildPdf(ControllerContext);
                _db.Entry(patient).State = EntityState.Modified;
                await _db.SaveChangesAsync();

                return RedirectToAction("ViewPdf", new { patientId = patientId });
            }

            ViewBag.Message = "Patient Not Found!";
            return View("Error");
        }

        public async Task<ActionResult> GenerateFinalCarePdfForVersion(int? patientId, Guid? version)
        {
            //Same as above, but this function takes a care plan version

            var patient = await _db.Patients.FindAsync(patientId);
            if (patient != null && await FinalCarePlanForVersion(patientId, version) != null)
            {
                var footer = "--footer-center \"Last Updated on " + DateTime.Now.ToString("MM/dd/yyyy @ hh:mmtt ") +
                             "- Page: [page]/[toPage]\"" + " --footer-font-size \"8\" --footer-spacing \"6\" --footer-font-name \"times\"";

                var generatePdf = new ViewAsPdf("_View4Pdf", await FinalCarePlanForVersion(patientId, version))
                {
                    FileName = "CCM_Health_" + patient.FirstName + "_" + patient.LastName + ".pdf",
                    //MinimumFontSize = 19,
                    CustomSwitches = footer,
                    IsLowQuality = false,
                    PageOrientation = Rotativa.Options.Orientation.Portrait,

                    PageSize = Rotativa.Options.Size.A5,
                    PageMargins = new Margins(5, 5, 5, 5),
                    //PageMargins = new Margins(3, 3, 12, 3)
                };
                try
                {
                    patient.FinalCarePlanPdf = generatePdf.BuildPdf(ControllerContext);
                    _db.Entry(patient).State = EntityState.Modified;
                    await _db.SaveChangesAsync();
                }
                catch (Exception ex)
                {


                }


                return RedirectToAction("ViewPdf", new { patientId = patientId });
            }

            ViewBag.Message = "Patient Not Found!";
            return View("Error");
        }
        [HttpPost]
        public string EmailCarePlan(int PatientID, int Cycle)
        {
            try
            {
                var patient = _db.Patients.Find(PatientID);
                var doctor = _db.Physicians.Find(patient.PhysicianId);
                var preText = "Hello " + patient.FirstName + ",<br />" +
                              "This is an automated message from Dr. " + doctor?.LastName + "'s office:<br /><br />";
                var body = preText + "<br /><br />" +

                              "Thank you." + "<br/>" +
                              "My CCM Health." + "<br/><br/>" +

                              "<small>This email is system generated and is not monitored by customer service. Please do not reply to this email. " +
                              "<br />If you need any assistance, please call our customer service at (212) 920-4500.</small>";
                List<string> toemails = new List<string>();
                using (var message = new MailMessage())
                {
                    message.From = new MailAddress(WebConfigurationManager.AppSettings["SmtpSender"]);
                    var additionalproviders = _db.SecondaryDoctors.AsNoTracking().Where(x => x.PatientId == patient.Id && x.Email != null && x.Email != "" && x.IsShareCarePlan == true).ToList();
                    foreach (var item in additionalproviders)
                    {
                        message.To.Add(item.Email);
                        toemails.Add(item.Email);
                    }
                    var urgencycontacts = _db.PatientProfile_UrgencyContacts.AsNoTracking().Where(x => x.PatientId == patient.Id).ToList();
                    if (urgencycontacts.Count>0)
                    {
                        foreach(var urgencycontact in urgencycontacts)
                        {
                            if (!string.IsNullOrEmpty(urgencycontact.PrimaryEmail))
                            {
                                if (urgencycontact.PrimaryIsShareCarePlan == true)
                                {
                                    message.To.Add(urgencycontact.PrimaryEmail);
                                    toemails.Add(urgencycontact.PrimaryEmail);
                                }
                            }
                        }
                        
                    }
                    if (!string.IsNullOrEmpty(doctor.Email))
                    {
                        message.To.Add(doctor.Email);
                        toemails.Add(doctor.Email);
                    }
                       
                    if (!string.IsNullOrEmpty(patient.Email))
                    {
                        message.To.Add(patient.Email);
                        toemails.Add(patient.Email);
                    }
                    message.Subject = "My CCM Health - Email from Dr. " + doctor?.LastName + "'s office:";
                    message.Body = body;
                    message.IsBodyHtml = true;
                    var versionforFCP = _db.FinalCarePlanNotes.Where(x => x.PatientId == PatientID && x.Cycle == Cycle).FirstOrDefault().Version;

                    if (1 == 1)
                    {
                        var footer = "--footer-center \"Last Updated on " + DateTime.Now.ToString("MM/dd/yyyy @ hh:mmtt ") +
                                     "- Page: [page]/[toPage]\"" + " --footer-font-size \"8\" --footer-spacing \"6\" --footer-font-name \"times\"";
                        string Filename = "CCM_Health_" + patient.FirstName + "_" + patient.LastName + "_Cycle" + patient.Cycle + ".pdf";
                        var generatePdf = new ViewAsPdf("View4Pdf", FinalCarePlanForVersionData(PatientID, versionforFCP))
                        {
                            FileName = Filename,
                            MinimumFontSize = 19,
                            CustomSwitches = footer,
                            IsLowQuality = false,

                            PageSize = Size.A4,
                            PageMargins = new Margins(3, 3, 12, 3)
                        };
                        message.Attachments.Add(new Attachment(new MemoryStream(generatePdf.BuildPdf(ControllerContext)), Filename));
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
                    CarePlanSharedHistory carePlanSharedHistory = new CarePlanSharedHistory
                    {
                        PatientId = patient.Id,
                        Cycle = Cycle,
                        EmailSentTo = String.Join(",", toemails),
                        CreatedBy = User.Identity.GetUserId(),
                        CreatedOn = DateTime.Now,
                    };
                    _db.carePlanSharedHistories.Add(carePlanSharedHistory);
                    _db.SaveChanges();
                    return "True";
                }
            }
            catch (Exception ex)
            {
                return ex.Message+ex.InnerException;

            }
            return "True";
        }

        [HttpPost]
        public string EmailCarePlanInd(int PatientID, int Cycle,string EmailAddress,int ReceiverID,string ReceiverType, string ReceiverName = "")
        {
            try
            {
                var patient = _db.Patients.Find(PatientID);
                var doctor = _db.Physicians.Find(patient.PhysicianId);
                var preText = "Hello " + patient.FirstName + ",<br />" +
                              "This is an automated message from Dr. " + doctor?.LastName + "'s office:<br /><br />";
                var body = preText + "<br /><br />" +

                              "Thank you." + "<br/>" +
                              "My CCM Health." + "<br/><br/>" +

                              "<small>This email is system generated and is not monitored by customer service. Please do not reply to this email. " +
                              "<br />If you need any assistance, please call our customer service at (212) 920-4500.</small>";
                List<string> toemails = new List<string>();
                using (var message = new MailMessage())
                {
                    message.From = new MailAddress(WebConfigurationManager.AppSettings["SmtpSender"]);
                    message.To.Add(EmailAddress);
                    toemails.Add(EmailAddress);
                    message.Subject = "My CCM Health - Email from Dr. " + doctor?.LastName + "'s office:";
                    message.Body = body;
                    message.IsBodyHtml = true;
                    var versionforFCP = _db.FinalCarePlanNotes.Where(x => x.PatientId == PatientID && x.Cycle == Cycle).FirstOrDefault().Version;

                    if (1 == 1)
                    {
                        var footer = "--footer-center \"Last Updated on " + DateTime.Now.ToString("MM/dd/yyyy @ hh:mmtt ") +
                                     "- Page: [page]/[toPage]\"" + " --footer-font-size \"8\" --footer-spacing \"6\" --footer-font-name \"times\"";
                        string Filename = "CCM_Health_" + patient.FirstName + "_" + patient.LastName + "_Cycle" + patient.Cycle + ".pdf";
                        var generatePdf = new ViewAsPdf("_View4Pdf", FinalCarePlanForVersionData(PatientID, versionforFCP))
                        {
                            FileName=Filename,
                            CustomSwitches = footer,
                            IsLowQuality = false,
                            PageOrientation = Rotativa.Options.Orientation.Portrait,

                            PageSize = Rotativa.Options.Size.A5,
                            PageMargins = new Margins(5, 5, 5, 5),
                        };
                        message.Attachments.Add(new Attachment(new MemoryStream(generatePdf.BuildPdf(ControllerContext)), Filename));
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
                    CarePlanSharedHistory carePlanSharedHistory = new CarePlanSharedHistory
                    {
                        PatientId = patient.Id,
                        Cycle = Cycle,
                        EmailSentTo = String.Join(",", toemails),
                        CreatedBy = User.Identity.GetUserId(),
                        CreatedOn = DateTime.Now,
                        ReceiverName=ReceiverName,
                        ReceiverId=ReceiverID,
                        ReceiverType=ReceiverType
                    };
                    _db.carePlanSharedHistories.Add(carePlanSharedHistory);
                    _db.SaveChanges();
                    return "True";
                }
            }
            catch (Exception ex)
            {
                return ex.Message + ex.InnerException;

            }
            return "True";
        }
        public async Task<ActionResult> DeleteFinalCarePlanForVersion(int? patientId, Guid? version)
        {

            //var fcpn = _db.FinalCarePlanNotes.Find(version);
            var fcpn = _db.FinalCarePlanNotes.FirstOrDefault(m => (m.PatientId == patientId) && (m.Version == version));
            if (fcpn != null)
            {
                _db.FinalCarePlanNotes.Remove(fcpn);
                _db.SaveChanges();
            }

            //https://localhost:44356/FinalCarePlanNotes?patientId=713
            //return RedirectToAction("Index", "FinalCarePlanNotes", new { patientId = patientId });

            //https://localhost:44356/PatientProfile/Create?patientId=713
            return RedirectToAction("Create", "PatientProfile", new { patientId = patientId });

        }


        public async Task<bool> _DeleteFinalCarePlanForVersion(int? patientId, Guid? version)
        {
            bool IsDelete = false;

            //var fcpn = _db.FinalCarePlanNotes.Find(version);
            var fcpn = _db.FinalCarePlanNotes.FirstOrDefault(m => (m.PatientId == patientId) && (m.Version == version));
            if (fcpn != null)
            {
                _db.FinalCarePlanNotes.Remove(fcpn);
                _db.SaveChanges();
                IsDelete = true;
            }

            //https://localhost:44356/FinalCarePlanNotes?patientId=713
            //return RedirectToAction("Index", "FinalCarePlanNotes", new { patientId = patientId });

            //https://localhost:44356/PatientProfile/Create?patientId=713
          /*  return RedirectToAction("Create", "PatientProfile", new { patientId = patientId })*/;
            return IsDelete;
        }

       
        public async Task<ActionResult> ViewPdf(int? patientId)
        {
            var patient = await _db.Patients.FindAsync(patientId);
            if (patient == null)
            {
                ViewBag.Message = "Patient Not Found!";
                return View("Error");
            }

            var binaryData = patient.FinalCarePlanPdf;
            if (binaryData == null)
            {
                ViewBag.Message = "This Patient Does Not Have Care Plan PDF.";
                return View("Error");
            }

            var ms = new MemoryStream(binaryData);

            return new FileStreamResult(ms, "application/pdf");
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