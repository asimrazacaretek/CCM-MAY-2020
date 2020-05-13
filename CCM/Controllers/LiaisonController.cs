using CCM.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Rest.Api.V2010.Account.AvailablePhoneNumberCountry;
using CCM.Helpers;
using CCM.Models.CCMBILLINGS;

namespace CCM.Controllers
{
    [RequireHttps]
    [Authorize(Roles = "Liaison, Admin, PhysiciansGroup, LiaisonGroup")]
    public class LiaisonController : BaseController
    {
        private ApplicationUserManager _userManager;

        public LiaisonController()
        {
        }

        public LiaisonController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        //private readonly ApplicationdbContect _db = new ApplicationdbContect();



        // GET: Liaisons
        [Authorize(Roles = "Admin, PhysiciansGroup, LiaisonGroup")]
        public async Task<ActionResult> Index()
        {
            return View((await _db.Liaisons.ToListAsync())
                                  .Select(liaison => new LiaisonViewModel
                                  {
                                      Liaison = liaison,
                                      LastLogin = _db.LoginHistories.AsNoTracking().OrderByDescending(h => h.LoginDateTime)
                                                     .FirstOrDefault(l => l.UserId == liaison.UserId)?.LoginDateTime
                                  }).ToList()
                       );
        }

        // GET: Liaisons/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var liaison = await _db.Liaisons.FindAsync(id);
            if (liaison == null)
            {
                return HttpNotFound();
            }
            return View(liaison);
        }

        // GET: Liaisons/Create
        [Authorize(Roles = "Admin")]
        public ActionResult Create()
        {
            ViewBag.DayList = WeekDaysHelper.GetClinincalTiming();

            ViewBag.BillingCategories = _db.BillingCategories.ToList();
            //TwilioClient.Init(ConfigurationManager.AppSettings["TwilioAccountSid"], ConfigurationManager.AppSettings["TwilioAuthToken"]);
            //var localAvailableNumbers = LocalResource.Read("US", areaCode: Convert.ToInt32(ConfigurationManager.AppSettings["TwilioAreaCodeForNumbers"]), limit: 3, voiceEnabled: true);
            HelperExtensions.UpdateTwilioNumbers();
            var callerIDs = _db.TwilioNumbersTable.Where(p=>p.Status==false).Select(p => new SelectListItem
            {
                Value = p.MobilePhoneNumber.ToString(),
                Text = p.FriendlyPhoneNumer.ToString()
            });
            ViewBag.TwilioAvailableNumbers = callerIDs;
            return View();
        }

        // POST: Liaisons/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Create([Bind(Exclude = "UserPhoto")] Liaison liaison, string[] StartTime, string[] EndTime, string[] WeekDays, bool[] isHoliday, string[] billingCategory)
        {
            ViewBag.BillingCategories = _db.BillingCategories.ToList();
            //these variables are are create method parameters
            //decimal CPT99490Billing, decimal CPT99491Billing, decimal CPT99487Billing, decimal CPT99489Billing,
            if (ModelState.IsValid)
            {
                int[] BillingCatagoriesIntList = Array.ConvertAll(billingCategory, int.Parse);
                #region getting twiloCallerID
                if (liaison.TwilioCallerId != null)
                {
                   
                    TwilioClient.Init(ConfigurationManager.AppSettings["TwilioAccountSid"], ConfigurationManager.AppSettings["TwilioAuthToken"]);
                    Twilio.Types.PhoneNumber phoneNumber = new Twilio.Types.PhoneNumber(liaison.TwilioCallerId);
                    try
                    {
                        var incomingPhoneNumber = IncomingPhoneNumberResource.Create(
                            friendlyName: liaison.FirstName + " " + liaison.LastName,
                            phoneNumber: phoneNumber,
                            voiceMethod: Twilio.Http.HttpMethod.Post,
                           voiceUrl: new Uri("https://ccmhealth.us/Voice/Receive?CallerName=" + liaison.FirstName + liaison.LastName)
                            //voiceReceiveMode:IncomingPhoneNumberResource.VoiceReceiveModeEnum.Voice,
                            //voiceApplicationSid: ConfigurationManager.AppSettings["TwilioTwimlAppSid"]


                            );
                        liaison.TwiliopathSid = incomingPhoneNumber.Sid;
                        try
                        {
                            var twiliotable = _db.TwilioNumbersTable.Where(p => p.MobilePhoneNumber == liaison.TwilioCallerId).FirstOrDefault();
                            twiliotable.Status = true;
                            twiliotable.UpdatedOn = DateTime.Now;
                            twiliotable.UpdatedBy = User.Identity.GetUserId();
                            _db.Entry(twiliotable).State = EntityState.Modified;
                            _db.SaveChanges();
                            liaison.TwilioNumbersTableId = twiliotable.Id;

                        }
                        catch (Exception ex)
                        {


                        }

                    }
                    catch (Twilio.Exceptions.RestException ex)
                    {

                        ViewBag.DayList = WeekDaysHelper.GetClinincalTiming();
                        ViewBag.Message = "Twilio Number error. Reason: " + ex.Message;
                        //TwilioClient.Init(ConfigurationManager.AppSettings["TwilioAccountSid"], ConfigurationManager.AppSettings["TwilioAuthToken"]);
                        //var localAvailableNumbers1 = LocalResource.Read("US", areaCode: Convert.ToInt32(ConfigurationManager.AppSettings["TwilioAreaCodeForNumbers"]), limit: 3, voiceEnabled: true);

                        //var callerIDs1 = localAvailableNumbers1.Select(p => new SelectListItem
                        //{
                        //    Value = p.PhoneNumber.ToString(),
                        //    Text = p.FriendlyName.ToString()
                        //});
                        HelperExtensions.UpdateTwilioNumbers();
                        var callerIDs1 = _db.TwilioNumbersTable.Where(p => p.Status == false).Select(p => new SelectListItem
                        {
                            Value = p.MobilePhoneNumber.ToString(),
                            Text = p.FriendlyPhoneNumer.ToString()
                        });
                        ViewBag.TwilioAvailableNumbers = callerIDs1;
                        return View(liaison);

                    }
                }
                #endregion

                #region creating new user account
                var existingUser = await UserManager.FindByNameAsync(liaison.Email);
                if (existingUser != null)
                {
                    string sExistingId = existingUser.Id;
                    ViewBag.Message = "Email Already Exists: " + liaison.Email + "! Liaison Portal Not Created!.";

                    ViewBag.DayList = WeekDaysHelper.GetClinincalTiming();

                    //TwilioClient.Init(ConfigurationManager.AppSettings["TwilioAccountSid"], ConfigurationManager.AppSettings["TwilioAuthToken"]);
                    //var localAvailableNumbers1 = LocalResource.Read("US", areaCode: Convert.ToInt32(ConfigurationManager.AppSettings["TwilioAreaCodeForNumbers"]), limit: 3, voiceEnabled: true);


                    //var callerIDs1 = localAvailableNumbers1.Select(p => new SelectListItem
                    //{
                    //    Value = p.PhoneNumber.ToString(),
                    //    Text = p.FriendlyName.ToString()
                    //});
                    HelperExtensions.UpdateTwilioNumbers();
                    var callerIDs1 = _db.TwilioNumbersTable.Where(p => p.Status == false).Select(p => new SelectListItem
                    {
                        Value = p.MobilePhoneNumber.ToString(),
                        Text = p.FriendlyPhoneNumer.ToString()
                    });
                    ViewBag.TwilioAvailableNumbers = callerIDs1;
                    return View(liaison);
                    // await UserManager.DeleteAsync(existingUser);

                }

                ApplicationUser user = new ApplicationUser { UserName = liaison.Email, Email = liaison.Email };
                string password = LiasionHelper.GenrateLiasionPassword(liaison.LastName.ToLower());
                var result = await UserManager.CreateAsync(user, password);
                #endregion

                if (result.Succeeded)
                {

                    await UserManager.AddToRoleAsync(user.Id, "Liaison");


                    liaison.UserId = user.Id;
                    liaison.CreatedOn = DateTime.Now;
                    liaison.CreatedBy = User.Identity.GetUserId();


                    var postedImageFile = Request.Files["UserPhoto"];

                    if (postedImageFile?.ContentLength != 0 && postedImageFile?.InputStream != null)
                        using (var binary = new BinaryReader(postedImageFile.InputStream))
                        {
                            var imageData = binary.ReadBytes(postedImageFile.ContentLength);
                            if (imageData.Length > 0)
                                liaison.UserPhoto = imageData;
                        }

                    liaison.TwilioAccountSID = ConfigurationManager.AppSettings["TwilioAccountSid"];
                    liaison.TwilioAuthToken = ConfigurationManager.AppSettings["TwilioAuthToken"];
                    liaison.TwilioTwimlAppSid = ConfigurationManager.AppSettings["TwilioTwimlAppSid"];
                    List<Liaisons_BillingCategories> liaisons_BillingCategoriesList = new List<Liaisons_BillingCategories>();

                    try
                    {
                        foreach (var item in BillingCatagoriesIntList)
                        {

                            Liaisons_BillingCategories laisonCategories = new Liaisons_BillingCategories();
                            //laisonCategories.LiaisonId = liaison.Id;
                            laisonCategories.Status = true;
                            laisonCategories.CreatedOn = DateTime.Now;
                            laisonCategories.CreatedBy = User.Identity.GetUserId();
                            laisonCategories.EnrolledOn = DateTime.Now;
                            laisonCategories.BillingCategoryId = item;

                            // laisonCategories.Liaison = liaison;
                            laisonCategories.BillingCategory = _db.BillingCategories.Where(p => p.BillingCategoryId == item).Select(p => p).FirstOrDefault();
                            liaisons_BillingCategoriesList.Add(laisonCategories);

                        }
                        //_db.Liaisons_BillingCategories.AddRange(liaisons_BillingCategoriesList);
                        //await _db.SaveChangesAsync();

                    }
                    catch (Exception ex)
                    {

                    }
                    liaison.Liaisons_BillingCategories = liaisons_BillingCategoriesList;
                    _db.Liaisons.Add(liaison);
                    _db.SaveChanges();


                    //try
                    //{


                    //    Liaison_CPTRates liasion_CPTRatesCPT99490 = new Liaison_CPTRates();
                    //    liasion_CPTRatesCPT99490.BillingCode = "CPT99490";
                    //    liasion_CPTRatesCPT99490.SalaryRate = CPT99490Billing;

                    //    liasion_CPTRatesCPT99490.CreatedBy = User.Identity.GetUserId();
                    //    liasion_CPTRatesCPT99490.CreatedOn = DateTime.Now;
                    //    liasion_CPTRatesCPT99490.LiaisonId = liaison.Id;
                    //    //CPT99491
                    //    Liaison_CPTRates liasion_CPTRatesCPT99491 = new Liaison_CPTRates();
                    //    liasion_CPTRatesCPT99491.BillingCode = "CPT99491";
                    //    liasion_CPTRatesCPT99491.SalaryRate = CPT99491Billing;

                    //    liasion_CPTRatesCPT99491.CreatedBy = User.Identity.GetUserId();
                    //    liasion_CPTRatesCPT99491.CreatedOn = DateTime.Now;
                    //    liasion_CPTRatesCPT99491.LiaisonId = liaison.Id;
                    //    //CPT99487
                    //    Liaison_CPTRates liasion_CPTRatesCPT99487 = new Liaison_CPTRates();
                    //    liasion_CPTRatesCPT99487.BillingCode = "CPT99487";
                    //    liasion_CPTRatesCPT99487.SalaryRate = CPT99487Billing;

                    //    liasion_CPTRatesCPT99487.CreatedBy = User.Identity.GetUserId();
                    //    liasion_CPTRatesCPT99487.CreatedOn = DateTime.Now;
                    //    liasion_CPTRatesCPT99487.LiaisonId = liaison.Id;
                    //    //CPT99489
                    //    Liaison_CPTRates liasion_CPTRatesCPT99489 = new Liaison_CPTRates();
                    //    liasion_CPTRatesCPT99489.BillingCode = "CPT99489";
                    //    liasion_CPTRatesCPT99489.SalaryRate = CPT99489Billing;

                    //    liasion_CPTRatesCPT99489.CreatedBy = User.Identity.GetUserId();
                    //    liasion_CPTRatesCPT99489.CreatedOn = DateTime.Now;
                    //    liasion_CPTRatesCPT99489.LiaisonId = liaison.Id;

                    //    _db.Liaison_CPTRates.Add(liasion_CPTRatesCPT99490);
                    //    _db.Liaison_CPTRates.Add(liasion_CPTRatesCPT99491);
                    //    _db.Liaison_CPTRates.Add(liasion_CPTRatesCPT99487);
                    //    _db.Liaison_CPTRates.Add(liasion_CPTRatesCPT99489);
                    //    _db.SaveChanges();
                    //}
                    //catch (Exception ex)
                    //{


                    //}

                    user.Role = "Liaison";
                    user.CCMid = liaison.Id;
                    user.FirstName = liaison.FirstName;
                    user.LastName = liaison.LastName;
                    user.PhoneNumber = liaison.MobilePhoneNumber;

                    _db.Entry(user).State = EntityState.Modified;
                    await _db.SaveChangesAsync();
                    //LiasionTimings
                    List<DoctorTiming> list = new List<DoctorTiming>();
                    for (int i = 0; i < StartTime.Count(); i++)
                    {
                        if (isHoliday[i] == false)
                        {
                            DoctorTiming ct = new DoctorTiming();
                            string one = DateTime.Now.ToString(WeekDaysHelper.DateFormat);
                            ct.StartTime = Convert.ToDateTime(one + " " + StartTime[i], CultureInfo.InvariantCulture);
                            ct.EndTime = Convert.ToDateTime(one + " " + EndTime[i], CultureInfo.InvariantCulture);
                            ct.ClinicID = 1;
                            ct.IsDeleted = false;
                            ct.WeekDayName = WeekDays[i];
                            ct.LiaisonID = liaison.Id;
                            list.Add(ct);

                        }
                    }
                    _db.doctorTimings.AddRange(list);
                    _db.SaveChanges();
                    //
                    ViewBag.Message = "Liaison Portal Created.";
                    ViewBag.Username = liaison.Email;
                    ViewBag.Password = password;

                    ViewBag.DayList = WeekDaysHelper.GetClinincalTiming();

                    //TwilioClient.Init(ConfigurationManager.AppSettings["TwilioAccountSid"], ConfigurationManager.AppSettings["TwilioAuthToken"]);
                    //var localAvailableNumbers1 = LocalResource.Read("US", areaCode: Convert.ToInt32(ConfigurationManager.AppSettings["TwilioAreaCodeForNumbers"]), limit: 3, voiceEnabled: true);

                    //var callerIDs1 = localAvailableNumbers1.Select(p => new SelectListItem
                    //{
                    //    Value = p.PhoneNumber.ToString(),
                    //    Text = p.FriendlyName.ToString()
                    //});
                    HelperExtensions.UpdateTwilioNumbers();
                    var callerIDs1 = _db.TwilioNumbersTable.Where(p => p.Status == false).Select(p => new SelectListItem
                    {
                        Value = p.MobilePhoneNumber.ToString(),
                        Text = p.FriendlyPhoneNumer.ToString()
                    });
                    ViewBag.TwilioAvailableNumbers = callerIDs1;
                    return View();
                }

                string err = "";
                foreach (string e in result.Errors)
                {
                    err += e;
                    err += " ";
                }

                ViewBag.DayList = WeekDaysHelper.GetClinincalTiming();
                ViewBag.Message = "Unable To Create Liaison Portal. Reason: " + err;
                //TwilioClient.Init(ConfigurationManager.AppSettings["TwilioAccountSid"], ConfigurationManager.AppSettings["TwilioAuthToken"]);
                //var localAvailableNumbers = LocalResource.Read("US", areaCode: Convert.ToInt32(ConfigurationManager.AppSettings["TwilioAreaCodeForNumbers"]), limit: 3, voiceEnabled: true);

                //var callerIDs = localAvailableNumbers.Select(p => new SelectListItem
                //{
                //    Value = p.PhoneNumber.ToString(),
                //    Text = p.FriendlyName.ToString()
                //});
                HelperExtensions.UpdateTwilioNumbers();
                var callerIDs = _db.TwilioNumbersTable.Where(p => p.Status == false).Select(p => new SelectListItem
                {
                    Value = p.MobilePhoneNumber.ToString(),
                    Text = p.FriendlyPhoneNumer.ToString()
                });
                ViewBag.TwilioAvailableNumbers = callerIDs;
                var BillingCategories = _db.BillingCategories.ToList();

                return View(liaison);
            }



            ViewBag.DayList = WeekDaysHelper.GetClinincalTiming();

            //ViewBag.Message = "Email Already Exists! Liaison Portal Not Created!.";
            ViewBag.Message = "Invalid model state; Liaison Portal Not Created";



            return View(liaison);
        }


        // GET: Liaisons/Edit/5
        public async Task<ActionResult> Edit(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var liaison = await _db.Liaisons.FirstOrDefaultAsync(l => l.UserId == userId);

            if (liaison == null)
            {
                return HttpNotFound();
            }

            ViewBag.CCMEnrolledCount = await _db.Patients.CountAsync(p => p.EnrollmentStatus == "Enrolled" &&
                                                                          p.CcmStatus == "Enrolled" &&
                                                                          p.Liaison.UserId == liaison.UserId);

            ViewBag.CCMBilledCount = await _db.Patients.CountAsync(p => p.EnrollmentStatus == "Enrolled" &&
                                                                        p.CcmStatus == "Claims Submission" &&
                                                                        p.Liaison.UserId == liaison.UserId);
            //try
            //{


            //    ViewBag.CPT99490Billing = _db.Liaison_CPTRates.Where(x => x.LiaisonId == liaison.Id && x.BillingCode == "CPT99490").FirstOrDefault().SalaryRate;
            //    ViewBag.CPT99491Billing = _db.Liaison_CPTRates.Where(x => x.LiaisonId == liaison.Id && x.BillingCode == "CPT99491").FirstOrDefault().SalaryRate;
            //    ViewBag.CPT99487Billing = _db.Liaison_CPTRates.Where(x => x.LiaisonId == liaison.Id && x.BillingCode == "CPT99487").FirstOrDefault().SalaryRate;
            //    ViewBag.CPT99489Billing = _db.Liaison_CPTRates.Where(x => x.LiaisonId == liaison.Id && x.BillingCode == "CPT99489").FirstOrDefault().SalaryRate;
            //}
            //catch (Exception ex)
            //{


            //}

            var doctortiming = _db.doctorTimings.Where(x => x.LiaisonID == liaison.Id).ToList();

            ViewBag.ClinicTimining = WeekDaysHelper.GetClinicalTimingWithViewModel(doctortiming);

            //TwilioClient.Init(ConfigurationManager.AppSettings["TwilioAccountSid"], ConfigurationManager.AppSettings["TwilioAuthToken"]);
            //var localAvailableNumbers = LocalResource.Read("US", limit: 3, voiceEnabled: true, areaCode: Convert.ToInt32(ConfigurationManager.AppSettings["TwilioAreaCodeForNumbers"]));

            //var callerIDs = localAvailableNumbers.Select(p => new SelectListItem
            //{
            //    Value = p.PhoneNumber.ToString(),
            //    Text = p.FriendlyName.ToString()
            //});
            HelperExtensions.UpdateTwilioNumbers();
            var callerIDs = _db.TwilioNumbersTable.Where(p => p.Status == false).Select(p => new SelectListItem
            {
                Value = p.MobilePhoneNumber.ToString(),
                Text = p.FriendlyPhoneNumer.ToString()
            });


            ViewBag.TwilioAvailableNumbers = callerIDs;
            ViewBag.BillingCategories = _db.BillingCategories.ToList();
            return View(liaison);
        }

        // POST: Liaisons/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Edit([Bind(Exclude = "UserPhoto, Resume")]Liaison liaison, string[] StartTime, string[] EndTime, string[] WeekDays, bool[] isHoliday, int[] IDs, string CallerID1, string[] billingCategory)
        {

            // decimal CPT99490Billing, decimal CPT99491Billing, decimal CPT99487Billing, decimal CPT99489Billing,
            if (ModelState.IsValid)
            {

                if (!string.IsNullOrEmpty(CallerID1))
                {

                   var newtwilioid = _db.TwilioNumbersTable.Where(p => p.MobilePhoneNumber == CallerID1).Select(p => p.Id).FirstOrDefault();
                    if (liaison.isActive == true)
                    {

                    //if (liaison.TwilioNumbersTableId != newtwilioid)
                    //{
                    //    var twiliotable = _db.TwilioNumbersTable.Where(p => p.Id== liaison.TwilioNumbersTableId).FirstOrDefault();

                    //        if (twiliotable != null)
                    //        {
                    //        twiliotable.UpdatedOn = DateTime.Now;
                    //        twiliotable.UpdatedBy = User.Identity.GetUserId();
                    //        twiliotable.Status = false;
                    //    _db.Entry(twiliotable).State = EntityState.Modified;
                    //    _db.SaveChanges();


                    //        }
                    //    var newtwilio = _db.TwilioNumbersTable.Where(p => p.MobilePhoneNumber == CallerID1).FirstOrDefault();

                    //    liaison.TwilioNumbersTableId = newtwilio.Id;
                    //        newtwilio.UpdatedOn = DateTime.Now;
                    //        newtwilio.UpdatedBy = User.Identity.GetUserId();
                    //        newtwilio.Status = true;
                    //    _db.Entry(newtwilio).State = EntityState.Modified;
                    //    _db.SaveChanges();

                    //}
                 

                        var newtwilio = _db.TwilioNumbersTable.Where(p => p.MobilePhoneNumber == CallerID1).FirstOrDefault();
                        liaison.TwilioNumbersTableId = newtwilio.Id;
                            newtwilio.UpdatedOn = DateTime.Now;
                            newtwilio.UpdatedBy = User.Identity.GetUserId();
                        newtwilio.Status = true;
                        _db.Entry(newtwilio).State = EntityState.Modified;
                        _db.SaveChanges();
                  

                    }

                  
                    var liaisonalready = await _db.Liaisons.AsNoTracking().FirstOrDefaultAsync(l => l.Id == liaison.Id);
                    var calleridalready = liaisonalready.TwilioCallerId;
                    if (!string.IsNullOrEmpty(calleridalready))
                    {
                        try
                        {
                            TwilioClient.Init(liaisonalready.TwilioAccountSID, liaisonalready.TwilioAuthToken);
                            IncomingPhoneNumberResource.Delete(pathSid: liaisonalready.TwiliopathSid);
                        }
                        catch (Exception ex)
                        {


                        }
                    }
                    TwilioClient.Init(ConfigurationManager.AppSettings["TwilioAccountSid"], ConfigurationManager.AppSettings["TwilioAuthToken"]);
                    Twilio.Types.PhoneNumber phoneNumber = new Twilio.Types.PhoneNumber(CallerID1);

                    try
                    {
                        var incomingPhoneNumber = IncomingPhoneNumberResource.Create(
                            friendlyName: liaison.FirstName + " " + liaison.LastName,
                            phoneNumber: phoneNumber,
                            voiceMethod: Twilio.Http.HttpMethod.Post,
                            voiceUrl: new Uri("https://ccmhealth.us/Voice/Receive?CallerName=" + liaison.FirstName + liaison.LastName)
                        //    voiceReceiveMode: IncomingPhoneNumberResource.VoiceReceiveModeEnum.Voice,
                        //voiceApplicationSid: ConfigurationManager.AppSettings["TwilioTwimlAppSid"]

                            );
                        liaison.TwilioCallerId = CallerID1;
                        liaison.TwiliopathSid = incomingPhoneNumber.Sid;
                    }
                    catch (Twilio.Exceptions.RestException ex)
                    {
                        //try
                        //{


                        //    ViewBag.CPT99490Billing = _db.Liaison_CPTRates.Where(x => x.LiaisonId == liaison.Id && x.BillingCode == "CPT99490").FirstOrDefault().SalaryRate;
                        //    ViewBag.CPT99491Billing = _db.Liaison_CPTRates.Where(x => x.LiaisonId == liaison.Id && x.BillingCode == "CPT99491").FirstOrDefault().SalaryRate;
                        //    ViewBag.CPT99487Billing = _db.Liaison_CPTRates.Where(x => x.LiaisonId == liaison.Id && x.BillingCode == "CPT99487").FirstOrDefault().SalaryRate;
                        //    ViewBag.CPT99489Billing = _db.Liaison_CPTRates.Where(x => x.LiaisonId == liaison.Id && x.BillingCode == "CPT99489").FirstOrDefault().SalaryRate;
                        //}
                        //catch (Exception ex1)
                        //{


                        //}

                        ViewBag.DayList = WeekDaysHelper.GetClinincalTiming();
                        ViewBag.Message = "Twilio Number error. Reason: " + ex.Message;
                        //TwilioClient.Init(ConfigurationManager.AppSettings["TwilioAccountSid"], ConfigurationManager.AppSettings["TwilioAuthToken"]);
                        //var localAvailableNumbers1 = LocalResource.Read("US", areaCode: Convert.ToInt32(ConfigurationManager.AppSettings["TwilioAreaCodeForNumbers"]), limit: 3, voiceEnabled: true);

                        //var callerIDs1 = localAvailableNumbers1.Select(p => new SelectListItem
                        //{
                        //    Value = p.PhoneNumber.ToString(),
                        //    Text = p.FriendlyName.ToString()
                        //});
                        HelperExtensions.UpdateTwilioNumbers();
                        var callerIDs1 = _db.TwilioNumbersTable.Where(p => p.Status == false).Select(p => new SelectListItem
                        {
                            Value = p.MobilePhoneNumber.ToString(),
                            Text = p.FriendlyPhoneNumer.ToString()
                        });

                        ViewBag.TwilioAvailableNumbers = callerIDs1;
                        return View(liaison);

                    }
                }


                //if (liaison.isActive == false)
                //{
                //    var twilionumstatuschange = _db.TwilioNumbersTable.Where(p => p.Id == liaison.TwilioNumbersTableId).FirstOrDefault();
                //    if (twilionumstatuschange != null)
                //    {

                //        twilionumstatuschange.Status = false;
                //        _db.Entry(twilionumstatuschange).State = EntityState.Modified;
                //        _db.SaveChanges();

                //    }
                //}
                // Convert the user uploaded Photo as Byte Array before saving to DB 
                HttpPostedFileBase userPhoto = Request.Files["UserPhoto"];

                if (userPhoto?.ContentLength != 0 && userPhoto?.InputStream != null)
                    using (var binary = new BinaryReader(userPhoto.InputStream))
                    {
                        var imageData = binary.ReadBytes(userPhoto.ContentLength);
                        liaison.UserPhoto = imageData;
                    }

                else
                {
                    var caller = await _db.Liaisons.AsNoTracking().FirstOrDefaultAsync(l => l.Id == liaison.Id);
                    if (caller.UserPhoto != null)
                        liaison.UserPhoto = caller.UserPhoto;
                }



                // Convert the user uploaded Resume as Byte Array before saving to DB 
                HttpPostedFileBase resume = Request.Files["Resume"];

                if (resume?.ContentLength != 0 && resume?.InputStream != null)
                    using (var binary = new BinaryReader(resume.InputStream))
                    {
                        var imageData = binary.ReadBytes(resume.ContentLength);
                        liaison.Resume = imageData;
                    }

                else
                {
                    var caller = await _db.Liaisons.AsNoTracking().FirstOrDefaultAsync(l => l.Id == liaison.Id);
                    if (caller.Resume != null)
                        liaison.Resume = caller.Resume;
                }

                liaison.TwilioAccountSID = ConfigurationManager.AppSettings["TwilioAccountSid"];
                liaison.TwilioAuthToken = ConfigurationManager.AppSettings["TwilioAuthToken"];
                liaison.TwilioTwimlAppSid = ConfigurationManager.AppSettings["TwilioTwimlAppSid"];

                liaison.UpdatedOn = DateTime.Now;
                liaison.UpdatedBy = User.Identity.GetUserId();
                //liaison.Liaisons_BillingCategories = new List<Liaisons_BillingCategories>();
                List<Liaisons_BillingCategories> UpdatedBillingCategoryList = new List<Liaisons_BillingCategories>();
                try
                {
                    var oldBillingCategory = _db.Liaisons_BillingCategories.AsNoTracking().Where(x => x.LiaisonId == liaison.Id && x.Status==true).ToList();

                    if (billingCategory != null)
                    {

                        int[] CurrentSelectedBillingCategory = Array.ConvertAll(billingCategory, int.Parse);


                        foreach (var item in CurrentSelectedBillingCategory)
                        {

                            if (oldBillingCategory.FirstOrDefault(x => x.BillingCategoryId == item) == null)
                            {
                                //the current selected category is new one
                                Liaisons_BillingCategories laisonCategories = new Liaisons_BillingCategories();
                                laisonCategories.LiaisonId = liaison.Id;
                                laisonCategories.Status = true;
                                laisonCategories.CreatedOn = DateTime.Now;
                                laisonCategories.CreatedBy = User.Identity.GetUserId();
                                laisonCategories.EnrolledOn = DateTime.Now;
                                laisonCategories.BillingCategoryId = item;

                                //laisonCategories.Liaison = liaison;
                                //laisonCategories.BillingCategory = _db.BillingCategories.Where(p => p.BillingCategoryId == item).Select(p => p).FirstOrDefault();
                                //_db.Entry(laisonCategories).State = EntityState.Added;
                                //UpdatedBillingCategoryList.Add(laisonCategories);
                                _db.Liaisons_BillingCategories.Add(laisonCategories);
                                _db.SaveChanges();
                            }

                        }
                        //liaison.Liaisons_BillingCategories = UpdatedBillingCategoryList;
                        //_db.SaveChanges();
                        foreach (var i in oldBillingCategory)
                        {
                            if (!CurrentSelectedBillingCategory.Contains((int)i.BillingCategoryId))
                            {
                                Liaisons_BillingCategories bilingcatagory = _db.Liaisons_BillingCategories.Where(x => x.BillingCategoryId == i.BillingCategoryId && x.LiaisonId==liaison.Id && x.Status == true).FirstOrDefault();
                                bilingcatagory.Status = false;
                                _db.Entry(bilingcatagory).State = EntityState.Modified;
                                _db.SaveChanges();
                            }

                        }
                        //  liaison.Liaisons_BillingCategories = UpdatedBillingCategoryList;

                    }
                    else {
                        if (oldBillingCategory.Count()>0) {
                            foreach (var i in oldBillingCategory)
                            {
                                Liaisons_BillingCategories bilingcatagory = _db.Liaisons_BillingCategories.Where(x => x.BillingCategoryId == i.BillingCategoryId && x.LiaisonId==liaison.Id &&  x.Status == true).FirstOrDefault();
                                bilingcatagory.Status = false;
                                _db.Entry(bilingcatagory).State = EntityState.Modified;
                                _db.SaveChanges();
                            }
                        }
                    }


                }
                catch (Exception ex)
                {

                }
                liaison.Liaisons_BillingCategories = UpdatedBillingCategoryList;
                _db.Entry(liaison).State = EntityState.Modified;
                _db.SaveChanges();


                // var liasioncptalreadyexists = _db.Liaison_CPTRates.Where(x => x.LiaisonId == liaison.Id).FirstOrDefault();
                //if (liasioncptalreadyexists != null)
                // {
                //try
                //{

                //    //CPT99490
                //    var CPTRatesCPT99490 = _db.Liaison_CPTRates.Where(x => x.LiaisonId == liaison.Id && x.BillingCode == "CPT99490").FirstOrDefault();

                //    CPTRatesCPT99490.SalaryRate = CPT99490Billing;

                //    CPTRatesCPT99490.UpdatedBy = User.Identity.GetUserId();
                //    CPTRatesCPT99490.UpdatedOn = DateTime.Now;
                //    //CPT99491
                //    var CPTRatesCPT99491 = _db.Liaison_CPTRates.Where(x => x.LiaisonId == liaison.Id && x.BillingCode == "CPT99491").FirstOrDefault();

                //    CPTRatesCPT99491.SalaryRate = CPT99491Billing;

                //    CPTRatesCPT99491.UpdatedBy = User.Identity.GetUserId();
                //    CPTRatesCPT99491.UpdatedOn = DateTime.Now;

                //    //CPT99487
                //    var CPTRatesCPT99487 = _db.Liaison_CPTRates.Where(x => x.LiaisonId == liaison.Id && x.BillingCode == "CPT99487").FirstOrDefault();

                //    CPTRatesCPT99487.SalaryRate = CPT99487Billing;

                //    CPTRatesCPT99487.UpdatedBy = User.Identity.GetUserId();
                //    CPTRatesCPT99487.UpdatedOn = DateTime.Now;

                //    //CPT99489
                //    var CPTRatesCPT99489 = _db.Liaison_CPTRates.Where(x => x.LiaisonId == liaison.Id && x.BillingCode == "CPT99489").FirstOrDefault();

                //    CPTRatesCPT99489.SalaryRate = CPT99489Billing;

                //    CPTRatesCPT99489.UpdatedBy = User.Identity.GetUserId();
                //    CPTRatesCPT99489.UpdatedOn = DateTime.Now;


                //    _db.Entry(CPTRatesCPT99490).State = EntityState.Modified;
                //    _db.Entry(CPTRatesCPT99491).State = EntityState.Modified;
                //    _db.Entry(CPTRatesCPT99487).State = EntityState.Modified;
                //    _db.Entry(CPTRatesCPT99489).State = EntityState.Modified;
                //    _db.SaveChanges();
                //}
                //catch (Exception ex)
                //{


                //}
                //}
                // else
                //{
                //try
                //{


                //    Liaison_CPTRates liasion_CPTRatesCPT99490 = new Liaison_CPTRates();
                //    liasion_CPTRatesCPT99490.BillingCode = "CPT99490";
                //    liasion_CPTRatesCPT99490.SalaryRate = CPT99490Billing;

                //    liasion_CPTRatesCPT99490.CreatedBy = User.Identity.GetUserId();
                //    liasion_CPTRatesCPT99490.CreatedOn = DateTime.Now;
                //    liasion_CPTRatesCPT99490.LiaisonId = liaison.Id;
                //    //CPT99491
                //    Liaison_CPTRates liasion_CPTRatesCPT99491 = new Liaison_CPTRates();
                //    liasion_CPTRatesCPT99491.BillingCode = "CPT99491";
                //    liasion_CPTRatesCPT99491.SalaryRate = CPT99491Billing;

                //    liasion_CPTRatesCPT99491.CreatedBy = User.Identity.GetUserId();
                //    liasion_CPTRatesCPT99491.CreatedOn = DateTime.Now;
                //    liasion_CPTRatesCPT99491.LiaisonId = liaison.Id;
                //    //CPT99487
                //    Liaison_CPTRates liasion_CPTRatesCPT99487 = new Liaison_CPTRates();
                //    liasion_CPTRatesCPT99487.BillingCode = "CPT99487";
                //    liasion_CPTRatesCPT99487.SalaryRate = CPT99487Billing;

                //    liasion_CPTRatesCPT99487.CreatedBy = User.Identity.GetUserId();
                //    liasion_CPTRatesCPT99487.CreatedOn = DateTime.Now;
                //    liasion_CPTRatesCPT99487.LiaisonId = liaison.Id;
                //    //CPT99489
                //    Liaison_CPTRates liasion_CPTRatesCPT99489 = new Liaison_CPTRates();
                //    liasion_CPTRatesCPT99489.BillingCode = "CPT99489";
                //    liasion_CPTRatesCPT99489.SalaryRate = CPT99489Billing;

                //    liasion_CPTRatesCPT99489.CreatedBy = User.Identity.GetUserId();
                //    liasion_CPTRatesCPT99489.CreatedOn = DateTime.Now;
                //    liasion_CPTRatesCPT99489.LiaisonId = liaison.Id;

                //    _db.Liaison_CPTRates.Add(liasion_CPTRatesCPT99490);
                //    _db.Liaison_CPTRates.Add(liasion_CPTRatesCPT99491);
                //    _db.Liaison_CPTRates.Add(liasion_CPTRatesCPT99487);
                //    _db.Liaison_CPTRates.Add(liasion_CPTRatesCPT99489);
                //    _db.SaveChanges();
                //}
                //catch (Exception ex)
                //{


                //}

                //}
                try
                {

                    var doctortimings = _db.doctorTimings.Where(x => x.LiaisonID == liaison.Id).ToList();
                    _db.doctorTimings.RemoveRange(doctortimings);
                    //LiaisonTimings
                    List<DoctorTiming> list = new List<DoctorTiming>();
                    for (int i = 0; i < WeekDays.Count(); i++)
                    {
                        if (isHoliday[i] == false)
                        {
                            DoctorTiming ct = new DoctorTiming();

                            string one = DateTime.Now.ToString(WeekDaysHelper.DateFormat);
                            try
                            {

                                ct.StartTime = Convert.ToDateTime(one + " " + StartTime[i]);
                                ct.EndTime = Convert.ToDateTime(one + " " + EndTime[i]);
                                ct.ClinicID = 1;

                                ct.IsDeleted = false;
                                ct.WeekDayName = WeekDays[i];
                                ct.LiaisonID = liaison.Id;
                                list.Add(ct);

                            }
                            catch (Exception)
                            {
                                ct.ClinicID = 1;
                                ct.StartTime = Convert.ToDateTime(one + " " + WeekDaysHelper.ClinicStartTime);
                                ct.IsDeleted = false;
                                ct.WeekDayName = WeekDays[i];
                                ct.EndTime = Convert.ToDateTime(one + " " + WeekDaysHelper.ClinicEndTime);

                                ct.LiaisonID = liaison.Id;
                                list.Add(ct);
                            }
                        }
                    }
                    _db.doctorTimings.AddRange(list);
                    _db.SaveChanges();

                    //
                }
                catch (Exception ex)
                {


                }
                return RedirectToAction("Index");
            }

            ViewBag.CCMEnrolledCount = await _db.Patients.CountAsync(p => p.EnrollmentStatus == "Enrolled" && p.UpdatedBy == liaison.UserId);
            ViewBag.CCMBilledCount = await _db.Patients.CountAsync(p => p.EnrollmentStatus == "Enrolled" && p.CcmClaimSubmissionDate != null && p.CCMEnrolledBy == liaison.UserId);
            //try
            //{


            //    ViewBag.CPT99490Billing = _db.Liaison_CPTRates.Where(x => x.LiaisonId == liaison.Id && x.BillingCode == "CPT99490").FirstOrDefault().SalaryRate;
            //    ViewBag.CPT99491Billing = _db.Liaison_CPTRates.Where(x => x.LiaisonId == liaison.Id && x.BillingCode == "CPT99491").FirstOrDefault().SalaryRate;
            //    ViewBag.CPT99487Billing = _db.Liaison_CPTRates.Where(x => x.LiaisonId == liaison.Id && x.BillingCode == "CPT99487").FirstOrDefault().SalaryRate;
            //    ViewBag.CPT99489Billing = _db.Liaison_CPTRates.Where(x => x.LiaisonId == liaison.Id && x.BillingCode == "CPT99489").FirstOrDefault().SalaryRate;
            //}
            //catch (Exception ex)
            //{


            //}

            var doctortiming = _db.doctorTimings.Where(x => x.LiaisonID == liaison.Id).ToList();
            ViewBag.ClinicTimining = WeekDaysHelper.GetClinicalTimingWithViewModel(doctortiming);
            return View(liaison);
        }


        // GET: Liaisons/Delete/5
        [Authorize(Roles = "Admin")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Liaison liaison = _db.Liaisons.Find(id);
            if (liaison == null)
            {
                return HttpNotFound();
            }
            return View(liaison);
        }

        // POST: Liaisons/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public ActionResult DeleteConfirmed(int id)
        {
            var liasioncptrates = _db.Liaison_CPTRates.Where(item => item.LiaisonId == id).ToList();
            _db.Liaison_CPTRates.RemoveRange(liasioncptrates);
            var liasiontimings = _db.doctorTimings.Where(x => x.LiaisonID == id).ToList();
            _db.doctorTimings.RemoveRange(liasiontimings);
            Liaison liaison = _db.Liaisons.Find(id);

            if (!string.IsNullOrEmpty(liaison.TwilioCallerId))
            {
                    var twilionumber = _db.TwilioNumbersTable.Where(p => p.MobilePhoneNumber == liaison.TwilioCallerId).FirstOrDefault();
                twilionumber.Status = false;
                _db.Entry(twilionumber).State = EntityState.Modified;
                _db.SaveChanges();
                TwilioClient.Init(liaison.TwilioAccountSID, liaison.TwilioAuthToken);
                IncomingPhoneNumberResource.Delete(pathSid: liaison.TwiliopathSid);
            }
            string email = liaison.Email;

            _db.Liaisons.Remove(liaison);
            _db.SaveChanges();

            var existingUser = UserManager.FindByName(email);
            if (existingUser != null)
            {
                UserManager.Delete(existingUser);
            }


            return RedirectToAction("Index");
        }


        public async Task<FileContentResult> UserPhotos(string userId)
        {
            if (!string.IsNullOrEmpty(userId))
            {
                var liaison = await _db.Liaisons.FirstOrDefaultAsync(u => u.UserId == userId);

                if (liaison?.UserPhoto != null && liaison.UserPhoto.Length > 0)
                    return new FileContentResult(liaison.UserPhoto, "image/jpeg");
            }

            //if there is no photo chosen then use a Stock (default) photo
            var fileName = HttpContext.Server.MapPath(@"~/dashboard/assets/img/MegaAidLogo.jpg");

            //convert imported image into byte file that can be read using FileStream and BinaryReader
            var fileInfo = new FileInfo(fileName);
            var imageSize = fileInfo.Length;
            var fStream = new FileStream(fileName, FileMode.Open, FileAccess.Read);
            var bReader = new BinaryReader(fStream);
            var imageData = bReader.ReadBytes((int)imageSize);

            return File(imageData, "image/jpeg");
        }
        public ActionResult LiaisonWorkLoad()
        {
            return View();
        }
        public PartialViewResult ScheduleList(DateTime date)
        {

            var results1 = _db.Liaisons.AsNoTracking().Select(x => new { LiaisonID = x.Id, FirstName = x.FirstName, LastName = x.LastName, isTranslator = x.IsTranslator }).ToList();
            var results = _db.patientAppointments.AsNoTracking().ToList().Where(x => x.StartTime.Date == date.Date).OrderBy(x => x.StartTime);

            var newresults = results1.Select(x1 => new LiaisonSchedulelist
            {
                LiaisonName = x1.FirstName + " " + x1?.LastName + (x1.isTranslator == true ? " (Translator)" : " (Counsler)"),


                Appointmentlst = string.Join("\n", results.Where(x => x.LiaisonID == x1.LiaisonID).Select(x => "<p class='pworkload'>" + x.Subject + "</p> booked From: <p class='pworkload'>" + x.StartTime.ToString("hh:mm tt") + " To: " + x.EndTime.ToString("hh:mm tt") + "</p>").ToList())

            }).Distinct().ToList();
            newresults = newresults.GroupBy(i => i.LiaisonName).Select(i => i.FirstOrDefault()).ToList();
            return PartialView(newresults);
        }
        public ActionResult Resume(int liaisonId)
        {
            var resume = _db.Liaisons.Find(liaisonId)?.Resume;
            var ms = resume != null ? new MemoryStream(resume) : null;

            return new FileStreamResult(ms, "application/pdf");
        }
        public ActionResult VoiceMails()
        {
            var user = _db.Users.Find(User.Identity.GetUserId());
            if (User.IsInRole("Liaison"))
            {
                var userid = User.Identity.GetUserId();
                var liaisons = _db.Liaisons.AsNoTracking().Where(x => x.UserId == userid).Select(p => new SelectListItem
                {
                    Value = p.Id.ToString(),
                    Text = p.FirstName + " " + p.LastName
                });
                ViewBag.Liaisons = new SelectList(liaisons.OrderBy(l => l.Text), "Value", "Text");
                return View();
            }


            else if (User.IsInRole("Admin"))
            {
                var liaisons = _db.Liaisons.AsNoTracking().Select(p => new SelectListItem
                {
                    Value = p.Id.ToString(),
                    Text = p.FirstName + " " + p.LastName
                });



                ViewBag.Liaisons = new SelectList(liaisons.OrderBy(l => l.Text), "Value", "Text");


                return View();
            }


            else if (User.IsInRole("LiaisonGroup"))
            {


                List<int> physicianids = new List<int>();
                var liasionids = _db.LiaisonGroup_Liaison_Mappings.AsNoTracking().Where(x => x.LiaisonGroupId == user.CCMid).Select(x => x.LiaisonId).ToList();
                var group = _db.Users.Find(User.Identity.GetUserId());
                var liaisons = _db.Liaisons.AsNoTracking().Where(p => liasionids.Contains(p.Id)).Select(p => new SelectListItem
                {
                    Value = p.Id.ToString(),
                    Text = p.FirstName + " " + p.LastName
                });


                ViewBag.Liaisons = new SelectList(liaisons.OrderBy(l => l.Text), "Value", "Text");

                return View();
            }
            return View();
        }
        public PartialViewResult _VoiceMailData(string ID1, DateTime From, DateTime To)
        {
            if (ID1 != "")
            {
                int ID = Convert.ToInt32(ID1);
                var liasionnumber = _db.Liaisons.AsNoTracking().Where(x => x.Id == ID).FirstOrDefault()?.TwilioCallerId;
                if (!string.IsNullOrEmpty(liasionnumber))
                {
                    liasionnumber = liasionnumber.Replace("+1", "");
                }
                if (!string.IsNullOrEmpty(liasionnumber))
                {
                    var Voicecalls = _db.voiceMailHistories.AsNoTracking().Where(x => x.LiaisonId == ID).ToList().Where(x => x.StartTime != null && x.StartTime.Value.Date >= From.Date && x.StartTime.Value.Date <= To.Date).OrderByDescending(x => x.StartTime).ToList().Select(x => new CallHistoryViewModel
                    {
                        PatientID = x.PatientID,
                        StartTime = x.StartTime,
                        To = x.To,
                        From = x.From,
                        Duration = x.Duration,
                        Status = x.Status,
                        RecordingURL = x.RecordingURL,
                        TwilioCallId = x.TwilioCallId,
                        isVoiceMail = true

                    }).ToList();
                    var calls = _db.CallHistories.AsNoTracking().Where(x => (x.LiaisonId == ID) && x.Direction == "Incoming").ToList().Where(x => x.StartTime != null && x.StartTime.Value.Date >= From.Date && x.StartTime.Value.Date <= To.Date).OrderByDescending(x => x.StartTime).ToList().Select(x => new CallHistoryViewModel
                    {
                        PatientID = x.PatientID,
                        StartTime = x.StartTime,
                        To = x.To,
                        From = x.From,
                        Duration = x.Duration,
                        Status = x.Status,
                        RecordingURL = x.RecordingURL,
                        TwilioCallId = x.TwilioCallId,
                        isVoiceMail = false

                    }).ToList();
                    Voicecalls.AddRange(calls);
                    Voicecalls = Voicecalls.OrderByDescending(x => x.StartTime).ToList();
                    return PartialView(Voicecalls);
                }
            }
            else
            {
                if (User.IsInRole("Admin"))
                {

                    var Voicecalls = _db.voiceMailHistories.AsNoTracking().ToList().Where(x => x.StartTime != null && x.StartTime.Value.Date >= From.Date && x.StartTime.Value.Date <= To.Date).OrderByDescending(x => x.StartTime).ToList().Select(x => new CallHistoryViewModel
                    {
                        PatientID = x.PatientID,
                        StartTime = x.StartTime,
                        To = x.To,
                        From = x.From,
                        Duration = x.Duration,
                        Status = x.Status,
                        RecordingURL = x.RecordingURL,
                        TwilioCallId = x.TwilioCallId,
                        isVoiceMail = true

                    }).ToList();
                    // var liasonnumbers = _db.Liaisons.AsNoTracking().Where(x => x.TwilioCallerId != null && x.TwilioCallerId != "").Select(x => x.TwilioCallerId).ToList();
                    // var liasonnumbers = _db.Liaisons.AsNoTracking().Select(x => x.Id.ToString()).ToList();
                    // liasonnumbers.Add(ConfigurationManager.AppSettings["TwilioCallerId"]);
                    var calls = _db.CallHistories.AsNoTracking().ToList().Where(x => x.StartTime != null && x.StartTime.Value.Date >= From.Date && x.StartTime.Value.Date <= To.Date && x.Direction == "Incoming").OrderByDescending(x => x.StartTime).ToList().Select(x => new CallHistoryViewModel
                    {
                        PatientID = x.PatientID,
                        StartTime = x.StartTime,
                        To = x.To,
                        From = x.From,
                        Duration = x.Duration,
                        Status = x.Status,
                        RecordingURL = x.RecordingURL,
                        TwilioCallId = x.TwilioCallId,
                        isVoiceMail = false

                    }).ToList();
                    Voicecalls.AddRange(calls);
                    Voicecalls = Voicecalls.OrderByDescending(x => x.StartTime).ToList();
                    return PartialView(Voicecalls);
                }
            }
            return PartialView(new List<CallHistoryViewModel>());

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