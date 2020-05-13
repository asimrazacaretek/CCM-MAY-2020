using CCM.Helpers;
using CCM.Models;
using CCM.Models.CCMBILLINGS.ViewModels;
using CCM.Models.ViewModels;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Linq.Dynamic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Mvc;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace CCM.Controllers
{



    [RequireHttps]
    [Authorize(Roles = "Liaison, Physician, PhysiciansGroup, Admin, QAQC, LiaisonGroup,Sales")]
    public class CcmStatusController : BaseController
    {

        //private readonly ApplicationdbContect _db = new ApplicationdbContect();
        //private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public async Task<ActionResult> Index(string userId, string status, string substatus, string Message = "", bool forcareplan = false, string forTranslator = "", bool fromDashBaord = false, int? BillingcategoryId = null)
        {
            var liasionsList = _db.Liaisons.ToList();
            var PhysiciansList = _db.Physicians.ToList();
            var PhysiciansGroupList = _db.PhysiciansGroup.ToList();
            ViewBag.BillingcategoryId = BillingcategoryId;
            if (BillingcategoryId != null)
            {

                try
                {

                    ViewBag.EnrollmentStauses = _db.EnrollmentStatuss.AsNoTracking().ToList();
                    ViewBag.EnrollmentSubStatuses = _db.EnrollmentSubStatuss.AsNoTracking().ToList();
                    //var patients = _db.Patients.AsNoTracking().Where(p => p.EnrollmentStatus == "Enrolled" && p.CcmStatus == status);
                    var user = string.IsNullOrEmpty(userId)
                                   ? _db.Users.Find(User.Identity.GetUserId())
                                   : _db.Users.Find(userId);

                    ViewBag.UserId = userId;
                    ViewBag.fromDashBaord = fromDashBaord;
                    ViewBag.forTranslator = forTranslator;
                    ViewBag.ForShareCarePlan = forcareplan;
                    ViewBag.Status = string.IsNullOrEmpty(status) ? "Unknown" : status;
                    ViewBag.SubStatus = substatus == null ? "" : substatus;
                    ViewBag.Owner = user.Role == "Liaison" || user.Role == "PhysiciansGroup" ? user.FirstName
                                   : user.Role == "Physician" ? "Dr. " + user.LastName
                                   : "Admin";
                    ViewBag.UserRole = user.Role;
                    ViewBag.Message = Message;
                    var liaisons = liasionsList.Where(x => x.IsTranslator == false).Select(p => new SelectListItem
                    {
                        Value = p.Id.ToString(),
                        Text = p.FirstName + " " + p.LastName
                    });
                    var translator = liasionsList.Where(x => x.IsTranslator == true).Select(p => new SelectListItem
                    {
                        Value = p.Id.ToString(),
                        Text = p.FirstName + " " + p.LastName
                    });

                    var physicians = PhysiciansList.Select(p => new SelectListItem
                    {
                        Value = p.Id.ToString(),
                        Text = p.FirstName + " " + p.LastName
                    });

                    var physiciansGroups = PhysiciansGroupList.Select(p => new SelectListItem
                    {
                        Value = p.Id.ToString(),
                        Text = p.GroupName
                    });
                    if (User.IsInRole("PhysiciansGroup"))
                    {


                        List<int> physicianids = new List<int>();
                        physicianids = _db.physicianGroup_Physician_Mappings.AsNoTracking().Where(x => x.PhysiciansGroupId == user.CCMid).Select(x => x.PhysicianId).ToList();
                        var group = _db.Users.Find(User.Identity.GetUserId());
                        physicians = PhysiciansList.Where(p => physicianids.Contains(p.Id))
                                                                     .Select(p => new SelectListItem
                                                                     {
                                                                         Value = p.Id.ToString(),
                                                                         Text = p.FirstName + " " + p.LastName
                                                                     });


                        var liasionids = _db.Patients.AsNoTracking().Where(x => physicianids.Contains(x.PhysicianId.Value) && x.LiaisonId != null).Select(x => x.LiaisonId).Distinct().ToList();
                        liaisons = liasionsList.Where(p => liasionids.Contains(p.Id) && p.IsTranslator == false).Select(p => new SelectListItem
                        {
                            Value = p.Id.ToString(),
                            Text = p.FirstName + " " + p.LastName
                        });

                        var translatorids = _db.Patients.AsNoTracking().Where(x => physicianids.Contains(x.PhysicianId.Value) && x.TranslatorId != null).Select(x => x.TranslatorId).Distinct().ToList();
                        translator = liasionsList.Where(p => translatorids.Contains(p.Id) && p.IsTranslator == true).Select(p => new SelectListItem
                        {
                            Value = p.Id.ToString(),
                            Text = p.FirstName + " " + p.LastName
                        });
                        physiciansGroups = PhysiciansGroupList.Where(x => x.Id == user.CCMid).Select(p => new SelectListItem
                        {
                            Value = p.Id.ToString(),
                            Text = p.GroupName

                        });


                        //var liasionids = _db.Patients.AsNoTracking().Where(x => physicianids.Contains(x.PhysicianId.Value) && x.LiaisonId != null).Select(x => x.LiaisonId).Distinct().ToList();
                        //liaisons = _db.Liaisons.AsNoTracking().Where(p => liasionids.Contains(p.Id)).Select(p => new SelectListItem
                        //{
                        //    Value = p.Id.ToString(),
                        //    Text = p.FirstName + " " + p.LastName
                        //});

                    }
                    if (User.IsInRole("LiaisonGroup"))
                    {
                        List<int> physicianids = new List<int>();
                        var liasionids = _db.LiaisonGroup_Liaison_Mappings.AsNoTracking().Where(x => x.LiaisonGroupId == user.CCMid).Select(x => x.LiaisonId).ToList();
                        var group = _db.Users.Find(User.Identity.GetUserId());

                        liaisons = liasionsList.Where(p => liasionids.Contains(p.Id) && p.IsTranslator == false).Select(p => new SelectListItem
                        {
                            Value = p.Id.ToString(),
                            Text = p.FirstName + " " + p.LastName
                        });
                        translator = liasionsList.Where(p => liasionids.Contains(p.Id) && p.IsTranslator == true).Select(p => new SelectListItem
                        {
                            Value = p.Id.ToString(),
                            Text = p.FirstName + " " + p.LastName
                        });
                        physicianids = _db.Patients.AsNoTracking().Where(x => liasionids.Contains(x.LiaisonId.Value) && x.LiaisonId != null).Select(x => x.PhysicianId.Value).Distinct().ToList();
                        physicians = PhysiciansList.Where(p => physicianids.Contains(p.Id))
                        .Select(p => new SelectListItem
                        {
                            Value = p.Id.ToString(),
                            Text = p.FirstName + " " + p.LastName
                        });


                        //liaisons = _db.Liaisons.AsNoTracking().Where(p => liasionids.Contains(p.Id)).Select(p => new SelectListItem
                        //{
                        //    Value = p.Id.ToString(),
                        //    Text = p.FirstName + " " + p.LastName
                        //});
                        //physicianids = _db.Patients.AsNoTracking().Where(x => liasionids.Contains(x.LiaisonId.Value) && x.LiaisonId != null).Select(x => x.PhysicianId.Value).Distinct().ToList();
                        //physicians = _db.Physicians.AsNoTracking().Where(p => physicianids.Contains(p.Id))
                        //                                             .Select(p => new SelectListItem
                        //                                             {
                        //                                                 Value = p.Id.ToString(),
                        //                                                 Text = p.FirstName + " " + p.LastName
                        //                                             });
                    }

                    var liaisons1 = liaisons.ToList();
                    var item = new SelectListItem();
                    item.Text = "No Assigned";
                    item.Value = "-1";
                    liaisons1.Insert(0, item);
                    ViewBag.Liaisons = new SelectList(liaisons1.OrderBy(l => l.Text), "Value", "Text", user.Role == "Liaison" ? user.CCMid.Value.ToString() : "");
                    var translator1 = translator.ToList();
                    translator1.Insert(0, item);
                    ViewBag.translator = new SelectList(translator1, "Value", "Text").OrderBy("Text");
                    var physicians1 = physicians.ToList();
                    physicians1.Insert(0, item);
                    ViewBag.Physicians = new SelectList(physicians1.OrderBy(p => p.Text), "Value", "Text");
                    var physiciansGroups1 = physiciansGroups.ToList();
                    physiciansGroups1.Insert(0, item);
                    ViewBag.physiciansGroups = new SelectList(physiciansGroups1.OrderBy(p => p.Text), "Value", "Text");


                    string Billingcategoryname = _db.BillingCategories.Where(p => p.BillingCategoryId == BillingcategoryId).Select(p => p.Name).FirstOrDefault();
                    ViewBag.BillingCategory = Billingcategoryname.ToUpperInvariant();
                    ViewBag.BillingCategoryId = BillingcategoryId;
                    Session["category"] = BillingcategoryId;
                    return View("Index1");
                    //return View(user.Role == "Liaison"
                    //           ? await patients.Where(p => p.LiaisonId == user.CCMid).ToListAsync()
                    //           : user.Role == "Physician"
                    //           ? await patients.Where(p => p.PhysicianId == user.CCMid).ToListAsync()
                    //           : user.Role == "PhysiciansGroup"
                    //           ? await patients.Where(p => p.Physician.MainPhoneNumber == user.PhoneNumber).ToListAsync()
                    //           : await patients.ToListAsync());
                }
                catch (Exception ex)
                {

                    log.Error(Environment.NewLine + User.Identity.GetUserName() + "-------" + User.Identity.GetUserId() + Environment.NewLine + ex.Message + "-----" + ex.StackTrace);
                    return View();
                    /*return ex.Message + "------------------" + ex.StackTrace;*/
                }

            }
            else
            {
                try
                {


                    ViewBag.EnrollmentStauses = _db.EnrollmentStatuss.AsNoTracking().ToList();
                    ViewBag.EnrollmentSubStatuses = _db.EnrollmentSubStatuss.AsNoTracking().ToList();
                    //var patients = _db.Patients.AsNoTracking().Where(p => p.EnrollmentStatus == "Enrolled" && p.CcmStatus == status);
                    var user = string.IsNullOrEmpty(userId)
                                   ? _db.Users.Find(User.Identity.GetUserId())
                                   : _db.Users.Find(userId);

                    ViewBag.UserId = userId;
                    ViewBag.fromDashBaord = fromDashBaord;
                    ViewBag.forTranslator = forTranslator;
                    ViewBag.ForShareCarePlan = forcareplan;
                    ViewBag.Status = string.IsNullOrEmpty(status) ? "Unknown" : status;
                    ViewBag.SubStatus = substatus == null ? "" : substatus;
                    ViewBag.Owner = user.Role == "Liaison" || user.Role == "PhysiciansGroup" ? user.FirstName
                                   : user.Role == "Physician" ? "Dr. " + user.LastName
                                   : "Admin";
                    ViewBag.UserRole = user.Role;
                    ViewBag.Message = Message;
                    var liaisons = liasionsList.Where(x => x.IsTranslator == false).Select(p => new SelectListItem
                    {
                        Value = p.Id.ToString(),
                        Text = p.FirstName + " " + p.LastName
                    });
                    var translator = liasionsList.Where(x => x.IsTranslator == true).Select(p => new SelectListItem
                    {
                        Value = p.Id.ToString(),
                        Text = p.FirstName + " " + p.LastName
                    });

                    var physicians = PhysiciansList.Select(p => new SelectListItem
                    {
                        Value = p.Id.ToString(),
                        Text = p.FirstName + " " + p.LastName
                    });

                    var physiciansGroups = PhysiciansGroupList.Select(p => new SelectListItem
                    {
                        Value = p.Id.ToString(),
                        Text = p.GroupName
                    });
                    if (User.IsInRole("PhysiciansGroup"))
                    {


                        List<int> physicianids = new List<int>();
                        physicianids = _db.physicianGroup_Physician_Mappings.AsNoTracking().Where(x => x.PhysiciansGroupId == user.CCMid).Select(x => x.PhysicianId).ToList();
                        var group = _db.Users.Find(User.Identity.GetUserId());
                        physicians = PhysiciansList.Where(p => physicianids.Contains(p.Id))
                                                                     .Select(p => new SelectListItem
                                                                     {
                                                                         Value = p.Id.ToString(),
                                                                         Text = p.FirstName + " " + p.LastName
                                                                     });


                        var liasionids = _db.Patients.AsNoTracking().Where(x => physicianids.Contains(x.PhysicianId.Value) && x.LiaisonId != null).Select(x => x.LiaisonId).Distinct().ToList();
                        liaisons = liasionsList.Where(p => liasionids.Contains(p.Id) && p.IsTranslator == false).Select(p => new SelectListItem
                        {
                            Value = p.Id.ToString(),
                            Text = p.FirstName + " " + p.LastName
                        });

                        var translatorids = _db.Patients.AsNoTracking().Where(x => physicianids.Contains(x.PhysicianId.Value) && x.TranslatorId != null).Select(x => x.TranslatorId).Distinct().ToList();
                        translator = liasionsList.Where(p => translatorids.Contains(p.Id) && p.IsTranslator == true).Select(p => new SelectListItem
                        {
                            Value = p.Id.ToString(),
                            Text = p.FirstName + " " + p.LastName
                        });
                        physiciansGroups = PhysiciansGroupList.Where(x => x.Id == user.CCMid).Select(p => new SelectListItem
                        {
                            Value = p.Id.ToString(),
                            Text = p.GroupName

                        });


                        //var liasionids = _db.Patients.AsNoTracking().Where(x => physicianids.Contains(x.PhysicianId.Value) && x.LiaisonId != null).Select(x => x.LiaisonId).Distinct().ToList();
                        //liaisons = _db.Liaisons.AsNoTracking().Where(p => liasionids.Contains(p.Id)).Select(p => new SelectListItem
                        //{
                        //    Value = p.Id.ToString(),
                        //    Text = p.FirstName + " " + p.LastName
                        //});

                    }
                    if (User.IsInRole("LiaisonGroup"))
                    {
                        List<int> physicianids = new List<int>();
                        var liasionids = _db.LiaisonGroup_Liaison_Mappings.AsNoTracking().Where(x => x.LiaisonGroupId == user.CCMid).Select(x => x.LiaisonId).ToList();
                        var group = _db.Users.Find(User.Identity.GetUserId());

                        liaisons = liasionsList.Where(p => liasionids.Contains(p.Id) && p.IsTranslator == false).Select(p => new SelectListItem
                        {
                            Value = p.Id.ToString(),
                            Text = p.FirstName + " " + p.LastName
                        });
                        translator = liasionsList.Where(p => liasionids.Contains(p.Id) && p.IsTranslator == true).Select(p => new SelectListItem
                        {
                            Value = p.Id.ToString(),
                            Text = p.FirstName + " " + p.LastName
                        });
                        physicianids = _db.Patients.AsNoTracking().Where(x => liasionids.Contains(x.LiaisonId.Value) && x.LiaisonId != null).Select(x => x.PhysicianId.Value).Distinct().ToList();
                        physicians = PhysiciansList.Where(p => physicianids.Contains(p.Id))
                        .Select(p => new SelectListItem
                        {
                            Value = p.Id.ToString(),
                            Text = p.FirstName + " " + p.LastName
                        });


                        //liaisons = _db.Liaisons.AsNoTracking().Where(p => liasionids.Contains(p.Id)).Select(p => new SelectListItem
                        //{
                        //    Value = p.Id.ToString(),
                        //    Text = p.FirstName + " " + p.LastName
                        //});
                        //physicianids = _db.Patients.AsNoTracking().Where(x => liasionids.Contains(x.LiaisonId.Value) && x.LiaisonId != null).Select(x => x.PhysicianId.Value).Distinct().ToList();
                        //physicians = _db.Physicians.AsNoTracking().Where(p => physicianids.Contains(p.Id))
                        //                                             .Select(p => new SelectListItem
                        //                                             {
                        //                                                 Value = p.Id.ToString(),
                        //                                                 Text = p.FirstName + " " + p.LastName
                        //                                             });
                    }

                    var liaisons1 = liaisons.ToList();
                    var item = new SelectListItem();
                    item.Text = "No Assigned";
                    item.Value = "-1";
                    liaisons1.Insert(0, item);
                    ViewBag.Liaisons = new SelectList(liaisons1.OrderBy(l => l.Text), "Value", "Text", user.Role == "Liaison" ? user.CCMid.Value.ToString() : "");
                    var translator1 = translator.ToList();
                    translator1.Insert(0, item);
                    ViewBag.translator = new SelectList(translator1, "Value", "Text").OrderBy("Text");
                    var physicians1 = physicians.ToList();
                    physicians1.Insert(0, item);
                    ViewBag.Physicians = new SelectList(physicians1.OrderBy(p => p.Text), "Value", "Text");
                    var physiciansGroups1 = physiciansGroups.ToList();
                    physiciansGroups1.Insert(0, item);
                    ViewBag.physiciansGroups = new SelectList(physiciansGroups1.OrderBy(p => p.Text), "Value", "Text");
                    return View("Index1");
                    //return View(user.Role == "Liaison"
                    //           ? await patients.Where(p => p.LiaisonId == user.CCMid).ToListAsync()
                    //           : user.Role == "Physician"
                    //           ? await patients.Where(p => p.PhysicianId == user.CCMid).ToListAsync()
                    //           : user.Role == "PhysiciansGroup"
                    //           ? await patients.Where(p => p.Physician.MainPhoneNumber == user.PhoneNumber).ToListAsync()
                    //           : await patients.ToListAsync());
                }
                catch (Exception ex)
                {

                    log.Error(Environment.NewLine + User.Identity.GetUserName() + "-------" + User.Identity.GetUserId() + Environment.NewLine + ex.Message + "-----" + ex.StackTrace);
                    return View();
                    /*return ex.Message + "------------------" + ex.StackTrace;*/
                }
            }
        }

        public string AppointmentDate = "";
        [HttpPost]
        public ActionResult LoadDrugData(string status, string userId, DateTime? DateFrom, DateTime? DateTo, string SearchCol = "", int LiaisonId = 0, int PhysicianID = 0, int PhysicianGroupID = 0, string forcareplan = "", int TranslatorID = 0, string forTranslator = "", string Languages = "", int? BillingcategoryId = null)
        {

            if (BillingcategoryId == null || BillingcategoryId == 0)
            {
                BillingcategoryId = BillingCodeHelper.cmmBillingCatagoryid;
            }
            if (DateFrom == null)
            {
                DateTime now = DateTime.Now;
                DateFrom =  new DateTime(now.Year, now.Month, 1);
            }
            if (DateTo == null)
            {
                DateTo = DateTime.Now;
            }
            var draw = Request.Form.GetValues("draw")?.FirstOrDefault();
            var start = Request.Form.GetValues("start")?.FirstOrDefault();
            var length = Request.Form.GetValues("length")?.FirstOrDefault();
            var sortColumn = Request.Form.GetValues("columns[" + Request.Form.GetValues("order[0][column]")?.FirstOrDefault() + "][name]")?.FirstOrDefault();
            var sortColumnDir = Request.Form.GetValues("order[0][dir]")?.FirstOrDefault();
            string searchValue = Request.Form.GetValues("search[value]")?.FirstOrDefault();

            if (status == "Enrolled")
            {
                status = "Enrolled";
            }

            //Paging Size (10,20,50,100)    
            int pageSize = length != null ? Convert.ToInt32(length) : 0;
            int skip = start != null ? Convert.ToInt32(start) : 0;
            int recordsTotal = 0;
            var user = string.IsNullOrEmpty(userId)
                           ? _db.Users.Find(User.Identity.GetUserId())
                           : _db.Users.Find(userId);
            var lstPatients = new List<PatientViewModel>();
            string CurrentUserId = User.Identity.GetUserId();
            string CurrentUserRole = user.Role.ToString();
            var isTranslater = -1;
            var Liaisonid = 0;
            var Lisaion = _db.Liaisons.Where(x => x.Id == user.CCMid).FirstOrDefault();
            bool isTranslatorParam = forTranslator == "True" ? true : false;
            if (Lisaion != null)
            {
                Liaisonid = Lisaion.Id;
                isTranslater = Convert.ToInt32(Lisaion.IsTranslator);
                isTranslatorParam = Convert.ToBoolean(isTranslater);
            }

            ////using (var context = new ApplicationdbContect())
            ////{

                var statusP = new SqlParameter("Status", status);
                var UserRole = new SqlParameter("UserRole", user.Role.ToString());

                var dataView = new List<PatientsforQues>();

                if (BillingcategoryId == BillingCodeHelper.cmmBillingCatagoryid && status == "Clinical Sign-Off")
                {

                    dataView = _db.Database
                          .SqlQuery<PatientsforQues>("GetPatientDataMultipleForClinical @Status,@UserRole,@BillingcategoryId,@DateFrom,@DateTo", new SqlParameter("Status", status),
        new SqlParameter("UserRole", user.Role.ToString()),
        new SqlParameter("BillingcategoryId", BillingcategoryId),
           new SqlParameter("DateFrom", DateFrom?.ToString("yyyy-MM-dd")),
     new SqlParameter("DateTo", DateTo?.ToString("yyyy-MM-dd")))
                          .ToList();
                }
                else if (status == "Clinical Sign-Off")
                {


                    dataView = _db.Database
                          .SqlQuery<PatientsforQues>("GetPatientDataMultipleForClinicalForOther @Status,@UserRole,@LiaisonId,@BillingcategoryId,@DateFrom,@DateTo", new SqlParameter("Status", status),
        new SqlParameter("UserRole", user.Role.ToString()),
        new SqlParameter("LiaisonId", Liaisonid),
        new SqlParameter("BillingcategoryId", BillingcategoryId),

                         new SqlParameter("DateFrom", DateFrom?.ToString("yyyy-MM-dd")),
     new SqlParameter("DateTo", DateTo?.ToString("yyyy-MM-dd")))
                          .ToList();


                }
                if (status == "Enrolled")
                {//ccm
                    if (BillingcategoryId == BillingCodeHelper.cmmBillingCatagoryid)
                    {

                        dataView = _db.Database
                              .SqlQuery<PatientsforQues>("GetPatientDataMultipleForActiveWorkQue @Status,@UserRole,@BillingcategoryId,@DateFrom,@DateTo", new SqlParameter("Status", status),
            new SqlParameter("UserRole", user.Role.ToString()),
          
            new SqlParameter("BillingcategoryId", BillingcategoryId),
             
                             new SqlParameter("DateFrom", DateFrom?.ToString("yyyy-MM-dd")),
     new SqlParameter("DateTo", DateTo?.ToString("yyyy-MM-dd")))
                              .ToList();
                   

                }
                    else
                    {//go506
                        try
                        {
                            dataView = _db.Database
                                     .SqlQuery<PatientsforQues>("GetPatientDataMultipleForActiveWorkQueForOthers @Status,@UserRole,@LiaisonId,@BillingcategoryId,@IsTranslator,@DateFrom,@DateTo", new SqlParameter("Status", status),new SqlParameter("UserRole", user.Role.ToString()),
                                     new SqlParameter("LiaisonId", Liaisonid)
                       , new SqlParameter("BillingcategoryId", BillingcategoryId),
                                      new SqlParameter("IsTranslator", isTranslatorParam), new SqlParameter("DateFrom", DateFrom?.ToString("yyyy-MM-dd")), new SqlParameter("DateTo", DateTo?.ToString("yyyy-MM-dd"))).ToList();
                    }
                        catch (Exception e)
                        {

                            throw;
                        }

                    }


                
            }
                if (status == "Claims Submission")
                {

                    dataView = _db.Database
                          .SqlQuery<PatientsforQues>("GetPatientDataMultipleforClaimSubmission @Status,@UserRole,@BillingcategoryId,@DateFrom,@DateTo", new SqlParameter("Status", status),
        new SqlParameter("UserRole", user.Role.ToString()),
        new SqlParameter("BillingcategoryId", BillingcategoryId),
                 new SqlParameter("DateFrom", DateFrom?.ToString("yyyy-MM-dd")),
     new SqlParameter("DateTo", DateTo?.ToString("yyyy-MM-dd")))
                          .ToList();

                    try
                    {
                        int BillingCategoryId = 0;
                        var dataViewForClaim = _db.Database
                                .SqlQuery<PatientsforQues>("GetPatientDataMultipleforClaimSubmissionForOther @Status,@UserRole,@LiaisonId,@BillingCategoryId,@DateFrom,@DateTo", new SqlParameter("Status", status),
              new SqlParameter("UserRole", user.Role.ToString()),
              new SqlParameter("LiaisonId", Liaisonid),
               new SqlParameter("BillingCategoryId", BillingCategoryId),
                  new SqlParameter("DateFrom", DateFrom?.ToString("yyyy-MM-dd")),
              new SqlParameter("DateTo", DateTo?.ToString("yyyy-MM-dd")))
                                .ToList();
                        int billId = BillingCodeHelper.cmmBillingCatagoryid;
                        dataView.AddRange(dataViewForClaim.Where(p => p.BillingCategoryId != billId && p.BillingCategoryId != null));

                    }
                    catch (SqlException e)
                    {

                        throw;
                    }





                }
                if (status == "Ready for Clinical Sign-Off")
                {

                    if (BillingcategoryId == BillingCodeHelper.cmmBillingCatagoryid)
                    {
                        
                     dataView = _db.Database
                    .SqlQuery<PatientsforQues>("GetPatientDataMultipleForReadyForCouncelorReview @Status,@UserRole,@BillingcategoryId,@DateFrom,@DateTo", new SqlParameter("Status", status),
  new SqlParameter("UserRole", user.Role.ToString()),
  new SqlParameter("BillingCategoryId", BillingcategoryId),
    new SqlParameter("DateFrom", DateFrom?.ToString("yyyy-MM-dd")),
     new SqlParameter("DateTo", DateTo?.ToString("yyyy-MM-dd"))

     )
                       .ToList();

                        //dataView = dataView.Where(p => p.BillingCategoryId == BillingCodeHelper.cmmBillingCatagoryid).ToList();
                    }
                    else if (BillingcategoryId != BillingCodeHelper.cmmBillingCatagoryid)
                    {

                        var dataViewForClaim = _db.Database
                           .SqlQuery<PatientsforQues>("GetPatientDataMultipleForReadyForCouncelorReviewForOthers @Status,@UserRole,@LiaisonId,@BillingCategoryId,@DateFrom,@DateTo "
                           , new SqlParameter("Status", status)
                           , new SqlParameter("UserRole", user.Role.ToString())
                           , new SqlParameter("LiaisonId", Liaisonid.ToString())
                           , new SqlParameter("BillingCategoryId", BillingcategoryId.ToString())
                            , new SqlParameter("DateFrom", DateFrom?.ToString("yyyy-MM-dd")),
                             new SqlParameter("DateTo", DateTo?.ToString("yyyy-MM-dd"))
                           ).ToList();
                        int billId = BillingCodeHelper.cmmBillingCatagoryid;
                        dataView.AddRange(dataViewForClaim.ToList());
                           // dataView = dataView.Where(p => p.BillingCategoryId == BillingCodeHelper.cmmBillingCatagoryid).ToList();
                        }
                   


                }




                List<int> physicianids = new List<int>();
            if (user.Role == "Sales")
            {
                var physiciangrpids = _db.physicianGroup_SalesStaff_Mappings.AsNoTracking().Where(x => x.SaleStaffId == user.CCMid).Select(x => x.PhysiciansGroupId).ToList();
                physicianids = _db.physicianGroup_Physician_Mappings.AsNoTracking().Where(x => physiciangrpids.Contains(x.PhysiciansGroupId)).Select(x => x.PhysicianId).ToList();
            }
            if (user.Role == "PhysiciansGroup")
                {
                    physicianids = _db.physicianGroup_Physician_Mappings.AsNoTracking().Where(x => x.PhysiciansGroupId == user.CCMid).Select(x => x.PhysicianId).ToList();
                }
                List<int> liasionids = new List<int>();
                if (user.Role == "LiaisonGroup")
                {
                    liasionids = _db.LiaisonGroup_Liaison_Mappings.AsNoTracking().Where(x => x.LiaisonGroupId == user.CCMid).Select(x => x.LiaisonId).ToList();
                }
                dataView = user.Role == "Liaison" && HelperExtensions.isTranslator(user.Id) == false
                           ? dataView.Where(p => p.LiaisonId == user.CCMid).ToList()
                           : user.Role == "Liaison" && HelperExtensions.isTranslator(user.Id) == true
                           ? dataView.Where(p => p.TranslatorId == user.CCMid).ToList()
                           : user.Role == "Physician"
                           ? dataView.Where(p => p.PhysicianId == user.CCMid).ToList()
                           : user.Role == "PhysiciansGroup" || user.Role == "Sales"
                           ? dataView.Where(p => physicianids.Contains(p.PhysicianId)).ToList()
                           : user.Role == "QAQC"
                           ? dataView.Where(p => p.CcmStatus == "Clinical Sign-Off").ToList()
                           : user.Role == "LiaisonGroup"
                          ? dataView.Where(p => liasionids.Contains(p.LiaisonId)).ToList()
                           : dataView;


            //if (user.Role == "Liaison" && HelperExtensions.isTranslator(user.Id) == false)
            //{
            //    if (status == "Enrolled" && forTranslator == false)
            //    {
            //        dataView = dataView.Where(p => p.TranslatorId == null).ToList();
            //    }

            //}

            var istranslator = HelperExtensions.isTranslator(user.Id);
            if (istranslator == true)
            {
                forTranslator = "True";
            }
            if (status == "Enrolled" && (forTranslator == "True" || HelperExtensions.isTranslator(user.Id) == true))
            {
                dataView = dataView.Where(p => p.TranslatorId != null).ToList();
            }
            if (status == "Enrolled" && (forTranslator == "False" || forTranslator == ""))
            {
                dataView = dataView.Where(p => p.TranslatorId == null).ToList();
            }

            if (LiaisonId > 0)
                    {
                        dataView = dataView.Where(p => p.LiaisonId == LiaisonId).ToList();
                    }
                    if (LiaisonId == -1)
                    {
                        dataView = dataView.Where(p => p.LiaisonId == 0).ToList();
                    }
                    if (PhysicianID > 0)
                    {
                        dataView = dataView.Where(p => p.PhysicianId == PhysicianID).ToList();
                    }
                    if (PhysicianID == -1)
                    {
                        dataView = dataView.Where(p => p.PhysicianId == 0).ToList();
                    }
                    if (PhysicianGroupID > 0)
                    {
                        physicianids = _db.physicianGroup_Physician_Mappings.AsNoTracking().Where(x => x.PhysiciansGroupId == PhysicianGroupID).Select(x => x.PhysicianId).ToList();
                        dataView = dataView.Where(p => physicianids.Contains(p.PhysicianId)).ToList();

                    }
                    if (PhysicianGroupID == -1)
                    {
                        physicianids = _db.physicianGroup_Physician_Mappings.AsNoTracking().Select(x => x.PhysicianId).ToList();
                        dataView = dataView.Where(p => !physicianids.Contains(p.PhysicianId)).ToList();

                    }
            if (TranslatorID > 0)
            {
                dataView = dataView.Where(p => p.TranslatorId == TranslatorID).ToList();
            }
            if (TranslatorID == -1)
            {
                dataView = dataView.Where(p => p.TranslatorId == 0).ToList();
            }
            if (Languages != "")
                    {
                        dataView = dataView.Where(p => p.PreferredLanguage == Languages).ToList();
                    }

                    //Search  
                    try
                    {
                        if (!string.IsNullOrEmpty(searchValue) && !string.IsNullOrWhiteSpace(searchValue))
                        {
                            bool isDateTimeSearch = false;
                            if (searchValue == "Rejected")
                            {
                                isDateTimeSearch = true;
                                dataView = dataView.Where(item => (item.ccmcyclenotes != null && item.ccmcyclenotes != "") || (item.IsRejectedByLiaison == true)).ToList();
                            }
                            else
                            {
                                if (searchValue == "No Work Done")
                                {
                                    isDateTimeSearch = true;
                                    dataView = dataView.Where(item => item.ReviewTime?.TotalMinutes == 0).ToList();
                                }
                                else
                                {
                                    if (searchValue == "More Work Require")
                                    {
                                        isDateTimeSearch = true;
                                        dataView = dataView.Where(item => item.ReviewTime?.TotalMinutes > 0 && item.ReviewTime?.TotalMinutes < 15).ToList();
                                    }
                                    else
                                    {
                                        if (searchValue == "Minimum Requirements Meet")
                                        {
                                            isDateTimeSearch = true;
                                            dataView = dataView.Where(item => item.ReviewTime?.TotalMinutes > 15 && (item.ccmcyclenotes == null || item.ccmcyclenotes == "")).ToList();
                                        }

                                    }
                                }

                            }
                            try
                            {
                                var searcvaluedate = Convert.ToDateTime(searchValue);
                                if (searcvaluedate != null)
                                {
                                    isDateTimeSearch = true;
                                    dataView = dataView.Where(p => p.BirthDate?.ToString("MM-dd-yyyy") == searcvaluedate.ToString("MM-dd-yyyy") ||
                                     p?.AppointmentDate?.ToString("MM-dd-yyyy") == searcvaluedate.ToString("MM-dd-yyyy") ||
                                      p?.CCMEnrolledOn?.ToString("MM-dd-yyyy") == searcvaluedate.ToString("MM-dd-yyyy") ||
                                       p?.LiasionAssignedOn?.ToString("MM-dd-yyyy") == searcvaluedate.ToString("MM-dd-yyyy") ||
                                     p?.CcmClaimSubmissionDate?.ToString("MM-dd-yyyy") == searcvaluedate.ToString("MM-dd-yyyy") ||
                                     p?.CcmClinicalSignOffDate?.ToString("MM-dd-yyyy") == searcvaluedate.ToString("MM-dd-yyyy") ||
                                     p?.CcmReconciliationDate?.ToString("MM-dd-yyyy") == searcvaluedate.ToString("MM-dd-yyyy")





                                                        ).ToList();
                                }
                            }
                            catch (Exception)
                            {


                            }
                            if (searchValue.Contains("cpt"))
                            {
                                isDateTimeSearch = true;
                                var search = searchValue.Split(',');
                                dataView = dataView.Where(p => p.BillingCodeName == search[1]).ToList();
                            }
                            if (searchValue == "99487" || searchValue == "99490" || searchValue == "99491")
                            {
                                isDateTimeSearch = true;
                                dataView = dataView.Where(p => p.BillingCode1.ToString().ToLower().Contains(searchValue.ToLower()) && p.BillingCode2 == "").ToList();
                            }
                            else if (searchValue == "99489")
                            {
                                isDateTimeSearch = true;
                                dataView = dataView.Where(p => p.BillingCode2.ToString().ToLower().Contains(searchValue.ToLower())).ToList();
                            }
                            if (searchValue.Contains("searchsplit"))
                            {
                                try
                                {

                                    var searcharr = Regex.Split(searchValue, "searchsplit");
                                    //var currentyear = "-" + DateTime.Now.Year.ToString("yy");
                                    var searchmonth = Convert.ToInt32(searchValue[0].ToString());
                                    if (searcharr[1] == "Rejected")
                                    {
                                        isDateTimeSearch = true;
                                        dataView = dataView.Where(item => (item.ccmcyclenotes != null && item.ccmcyclenotes != "") || (item.IsRejectedByLiaison == true)).ToList().Where(item => item.CycleCreatedON?.Month == searchmonth && item.CycleCreatedON?.Year == DateTime.Now.Year).ToList();
                                    }
                                    else
                                    {
                                        if (searcharr[1] == "No Work Done")
                                        {
                                            isDateTimeSearch = true;
                                            dataView = dataView.Where(item => item.ReviewTime?.TotalMinutes == 0 && item.CycleCreatedON?.Month == searchmonth && item.CycleCreatedON?.Year == DateTime.Now.Year).ToList();
                                        }
                                        else
                                        {
                                            if (searcharr[1] == "More Work Require")
                                            {
                                                isDateTimeSearch = true;
                                                dataView = dataView.Where(item => item.ReviewTime?.TotalMinutes > 0 && item.ReviewTime?.TotalMinutes < 15 && item.CycleCreatedON?.Month == searchmonth && item.CycleCreatedON?.Year == DateTime.Now.Year).ToList();
                                            }
                                            else
                                            {
                                                if (searcharr[1] == "Minimum requirements meet")
                                                {
                                                    isDateTimeSearch = true;
                                                    dataView = dataView.Where(item => item.ReviewTime?.TotalMinutes > 15 && (item.ccmcyclenotes == null || item.ccmcyclenotes == "") && item.CycleCreatedON?.Month == searchmonth && item.CycleCreatedON?.Year == DateTime.Now.Year).ToList();
                                                }
                                                else
                                                {
                                                    if (searcharr[1] == "Total")
                                                    {
                                                        isDateTimeSearch = true;
                                                        dataView = dataView.Where(item => item.CycleCreatedON?.Month == searchmonth && item.CycleCreatedON?.Year == DateTime.Now.Year).ToList();
                                                    }
                                                }

                                            }
                                        }

                                    }
                                }
                                catch (Exception ex)
                                {
                                    throw ex;

                                }

                            }
                            // Apply search   
                            if (isDateTimeSearch == false)
                            {
                                dataView = dataView.Where(p => p.PatientName.ToString().ToLower().Contains(searchValue.ToLower()) ||

                                                                                     (p.EnrollmentStatus ?? "").ToLower() == (searchValue.ToLower()) ||
                                                                                     (p.DocFirstName ?? "").ToLower().Contains(searchValue.ToLower()) ||
                                                                                     (p.DocLastName ?? "").ToLower().Contains(searchValue.ToLower()) ||
                                                                                     (p.liaisonFirstName ?? "").ToLower().Contains(searchValue.ToLower()) ||
                                                                                       (p.liaisonLastName ?? "").ToLower().Contains(searchValue.ToLower()) ||

                                                                                     (p.Gender ?? "").ToLower() == (searchValue.ToLower()) ||
                                                                                     (p.Cycle).ToString().ToLower().Contains(searchValue.ToLower()) ||
                                                                                     (p.PreferredLanguage ?? "").ToLower().Contains(searchValue.ToLower()) ||




                                                                                     (p.CcmStatus ?? "").ToString().ToLower().Contains(searchValue.ToLower()) ||
                                                                                     (p.DaysinQue).ToString().Contains(searchValue.ToLower()) ||

                                                                                     (p.callingstatus ?? "").ToString().ToLower() == searchValue.ToLower() ||
                                                                                      (p.emrnumber ?? "").ToString().ToLower().Contains(searchValue.ToLower()) ||
                                                                                      (p.emrtype ?? "").ToString().ToLower().Contains(searchValue.ToLower()) ||
                                                                                        //p.medicaideligibility.ToString().ToLower().Contains(searchValue.ToLower()) ||
                                                                                        //p.medicareeligibility.ToString().ToLower().Contains(searchValue.ToLower()) ||

                                                                                        (p.insuranceid ?? "").ToString().ToLower().Contains(searchValue.ToLower()) ||
                                                                                           (p.insurancename ?? "").ToString().ToLower().Contains(searchValue.ToLower()) ||
                                                                                            (p.MedicareIdNumber ?? "").ToString().ToLower().Contains(searchValue.ToLower()) ||
                                                                                           (p.MedicaidIdNumber ?? "").ToString().ToLower().Contains(searchValue.ToLower()) ||
                                                                                             (p.OtherInsuranceIdNumber ?? "").ToString().ToLower().Contains(searchValue.ToLower()) ||

                                                                                     (p.ccmcyclenotes ?? "").ToLower().Contains(searchValue.ToLower()) ||
                                                                                     //(p.Activitytext).ToLower().Contains(searchValue.ToLower()) ||
                                                                                     (p.callingstatus ?? "").ToLower().Contains(searchValue.ToLower()) ||
                                                                                     (p.SubmittedBy ?? "").ToLower().Contains(searchValue.ToLower()) ||
                                                                                         (p.note ?? "").ToLower().Contains(searchValue.ToLower()) ||
                                                                                         (p.TranslatorName ?? "").ToLower().Contains(searchValue.ToLower()) ||
                                                                                         p.Id.ToString().Contains(searchValue.ToLower())




                                                      ).ToList();
                            }


                        }





                    }
                    catch (Exception ex)
                    {
                        throw ex;

                    }


                    try
                    {
                        if (SearchCol != "" && DateFrom != null && DateTo != null)
                        {
                            if (SearchCol == "Enrolled On")
                            {
                                dataView = dataView.Where(p => p.CCMEnrolledOn?.Date >= DateFrom.Value.Date && p.CCMEnrolledOn?.Date <= DateTo.Value.Date).ToList();
                            }

                            else
                            {
                                if (SearchCol == "Appointment")
                                {
                                    dataView = dataView.Where(p => p.AppointmentDate?.Date >= DateFrom.Value.Date && p.AppointmentDate?.Date <= DateTo.Value.Date).ToList();
                                }
                                else
                                {
                                    if (SearchCol == "Date of Birth")
                                    {
                                        dataView = dataView.Where(p => p.BirthDate?.Date >= DateFrom.Value.Date && p.BirthDate?.Date <= DateTo.Value.Date).ToList();
                                    }
                                    else
                                    {
                                        if (SearchCol == "Date Entered")
                                        {
                                            dataView = status == "Enrolled" ? dataView.Where(p => p.CycleCreatedON?.Date >= DateFrom.Value.Date && p.CycleCreatedON?.Date <= DateTo.Value.Date).ToList() :
                                                status == "Clinical Sign-Off" ? dataView.Where(p => p.CcmClinicalSignOffDate?.Date >= DateFrom.Value.Date && p.CcmClinicalSignOffDate?.Date <= DateTo.Value.Date).ToList()
                                                : status == "Claims Submission" ? dataView.Where(p => p.CcmClaimSubmissionDate?.Date >= DateFrom.Value.Date && p.CcmClaimSubmissionDate?.Date <= DateTo.Value.Date).ToList()
                                                : status == "Reconciliation" ? dataView.Where(p => p.CcmReconciliationDate?.Date >= DateFrom.Value.Date && p.CcmReconciliationDate?.Date <= DateTo.Value.Date).ToList()
                                               : status == "Ready for Clinical Sign-Off" ? dataView.Where(p => p.CcmReadyforClinicalSignOffDate?.Date >= DateFrom.Value.Date && p.CcmReadyforClinicalSignOffDate?.Date <= DateTo.Value.Date).ToList() :
                                                status == "Expired" ? dataView.Where(p => p.CycleCreatedON?.Date >= DateFrom.Value.Date && p.CycleCreatedON?.Date <= DateTo.Value.Date).ToList() :
                                                dataView.ToList();
                                        }
                                        else
                                        {
                                            if (SearchCol == "Date of Service")
                                            {
                                                dataView = dataView.Where(p => p.CcmClinicalSignOffDate?.Date >= DateFrom.Value.Date && p.CcmClinicalSignOffDate?.Date <= DateTo.Value.Date || (p.ClinicalSignOffDate?.Date >= DateFrom.Value.Date && p.ClinicalSignOffDate?.Date <= DateTo.Value.Date)).ToList();

                                            }
                                        }
                                    }
                                }

                            }
                        }
                    }
                    catch (Exception)
                    {


                    }
                    if (sortColumn == "")
                    {
                        sortColumn = "Id";
                    }
                    try
                    {
                        //SORT
                        if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
                        {
                            dataView = dataView.OrderBy("Id" + " " + sortColumnDir).ToList();
                        }
                    }
                    catch (Exception)
                    {


                    }

                    var dataview2 = new List<PatientsforQues>();
                    if (status == "Enrolled" || status == "Clinical Sign-Off")
                    {

                        dataview2 = dataView.ToList()
           .GroupBy(n => new { n.Id, n.Cycle })
           .Select(g => g.First())

           .ToList();
                    }
                    if (status == "Claims Submission" || status == "Ready for Clinical Sign-Off")
                    {

                        dataview2 = dataView.ToList()
               .GroupBy(n => new { n.Id, n.Cycle, n.BillingCategoryId,n.BillingCodeId })
               .Select(g => g.First())

               .ToList();




                    }



            //var pbc = _db.Patients_BillingCategories.AsQueryable();
            //var allliasions = _db.Liaisons.AsQueryable();
            //foreach (var item in dataview2)
            //{
            //    //var liaisonID= pbc.Where(p => p.PatientId == item.Id && p.BillingCategoryId == item.BillingCategoryId && p.IsTranslator == false && p.Status == true).Select(p => p.LiaisonId).FirstOrDefault();
            //    //var liason = allliasions.Where(p => p.Id == liaisonID).Select(p => p).FirstOrDefault();
            //    //if (liason != null)
            //    //{

            //    //    item.LiaisonId = (int)liaisonID;
            //    //    item.liaisonFirstName = liason.FirstName;
            //    //    item.liaisonLastName = liason.LastName;

            //    //}
            //    if (item.TranslatorId >1)
            //    {

            //        var TranslatorId = pbc.Where(p => p.PatientId == item.Id && p.BillingCategoryId == item.BillingCategoryId && p.IsTranslator == true && p.Status == true).Select(p => p.LiaisonId).FirstOrDefault();
            //        if (TranslatorId != null)
            //        {

            //            var translator =allliasions .Where(p => p.Id == TranslatorId).Select(p => p).FirstOrDefault();
            //            if (translator != null)
            //            {

            //                item.TranslatorId = (int)TranslatorId;
            //                item.TranslatorName = translator.FirstName + " " + translator.LastName;


            //            }

            //        }
            //        else
            //        {
            //            item.TranslatorId = 0;
            //            item.TranslatorName = "";
            //        }
            //    }
            //    else
            //    {
            //        item.TranslatorId = 0;
            //        item.TranslatorName = "";
            //    }

            //}
            
           
            var res = dataview2.GroupBy(x => x.BillingCodeName).Select(group => new CPTCODES
                    {
                        Name = group.Key,
                        Count = group.Count().ToString()
                    }).Where(x => x.Name != null).ToList();
            //again group for table rows
            if (status == "Claims Submission" || status == "Ready for Clinical Sign-Off")
            {

                dataview2 = dataView.ToList()
       .GroupBy(n => new { n.Id, n.Cycle, n.BillingCategoryId })
       .Select(g => g.First())

       .ToList();




            }
            //Total 
            List<CPTCODES> lstCarePlaneShared = new List<CPTCODES>();
                    if (forcareplan == "value")
                    {
                        var totaltobshaared = dataview2.Sum(x => x.TotalCountCarePlanTobeShared);
                        CPTCODES resTotalTobeShared = new CPTCODES
                        {
                            Count = totaltobshaared.ToString(),
                            Name = "Total Careplans to be shared"
                        };

                        lstCarePlaneShared.Add(resTotalTobeShared);
                        var TotalCareplanShared = dataview2.Sum(x => x.TotalCareplanShared);
                        CPTCODES resTotalShared = new CPTCODES
                        {
                            Count = TotalCareplanShared.ToString(),
                            Name = "Total Careplan Shared"
                        };

                        lstCarePlaneShared.Add(resTotalShared);
                        var TotalSharedRemining = totaltobshaared - TotalCareplanShared;
                        CPTCODES resTotalremaining = new CPTCODES
                        {
                            Count = TotalSharedRemining.ToString(),
                            Name = "Total Careplan Remaining"
                        };

                        lstCarePlaneShared.Add(resTotalremaining);
                    }
                    //
                    List<string> lstTypes = new List<string>() { "Red", "Yellow", "Green", "Blue" };
                    List<string> lstMonths = new List<string>() { "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec" };

                    List<ActiveWorkQueTotals> activeWorkQueTotals = new List<ActiveWorkQueTotals>();
                    ActiveWorkQueTotals activeWorkQueTotal12 = new ActiveWorkQueTotals();
                    activeWorkQueTotal12.CycleType = "";
                    activeWorkQueTotal12.colorname = "";
                    string year = DateTime.Now.Date.ToString("yy");
                    //DateTime currdatetime = new DateTime();
                    for (int jj = 1; jj <= 12; jj++)
                    {
                        MinuteandType minuteandType = new MinuteandType();
                        minuteandType.Type = lstMonths[jj - 1] + "-" + year;
                        minuteandType.Minute = 0;
                        activeWorkQueTotal12.MonthNames.Add(minuteandType);
                    }
                    activeWorkQueTotals.Add(activeWorkQueTotal12);
                    var icd10codesall = _db.Icd10Codes.AsNoTracking();

                    try
                    {


                        foreach (var item in lstTypes)
                        {
                            ActiveWorkQueTotals activeWorkQueTotal = new ActiveWorkQueTotals();
                            activeWorkQueTotal.colorname = item;
                            if (item == "Red")
                            {
                                activeWorkQueTotal.CycleType = "No Work Done";

                                var dataview3 = dataview2.Where(item1 => item1.ReviewTime?.TotalMinutes == 0).ToList();

                                for (int ii = 1; ii <= 12; ii++)
                                {
                                    MinuteandType minuteandType = new MinuteandType();
                                    var totcountformonth = dataview3.Where(item1 => item1.CycleCreatedON?.Month == ii && item1.CycleCreatedON?.Year == DateTime.Now.Year).ToList().Count();
                                    minuteandType.Minute = totcountformonth;
                                    minuteandType.Type = ii.ToString();
                                    activeWorkQueTotal.MonthNames.Add(minuteandType);
                                }
                            }
                            else if (item == "Yellow")
                            {
                                activeWorkQueTotal.CycleType = "More Work Require";

                                var dataview3 = dataview2.Where(item1 => item1.ReviewTime?.TotalMinutes > 0 && item1.ReviewTime?.TotalMinutes < 15).ToList();
                                for (int ii = 1; ii <= 12; ii++)
                                {
                                    MinuteandType minuteandType = new MinuteandType();
                                    var totcountformonth = dataview3.Where(item1 => item1.CycleCreatedON?.Month == ii && item1.CycleCreatedON?.Year == DateTime.Now.Year).ToList().Count();
                                    minuteandType.Minute = totcountformonth;
                                    minuteandType.Type = ii.ToString();
                                    activeWorkQueTotal.MonthNames.Add(minuteandType);
                                }
                            }
                            else if (item == "Green")
                            {
                                activeWorkQueTotal.CycleType = "Minimum requirements meet";
                                var dataview3 = dataview2.Where(item1 => item1.ReviewTime?.TotalMinutes > 15 && (item1.ccmcyclenotes == null || item1.ccmcyclenotes == "")).ToList();
                                for (int ii = 1; ii <= 12; ii++)
                                {
                                    MinuteandType minuteandType = new MinuteandType();
                                    var totcountformonth = dataview3.Where(item1 => item1.CycleCreatedON?.Month == ii && item1.CycleCreatedON?.Year == DateTime.Now.Year).ToList().Count();
                                    minuteandType.Minute = totcountformonth;
                                    minuteandType.Type = ii.ToString();
                                    activeWorkQueTotal.MonthNames.Add(minuteandType);
                                }
                            }
                            else if (item == "Blue")
                            {
                                activeWorkQueTotal.CycleType = "Rejected";

                                var dataview3 = dataview2.Where(item1 => (item1.ccmcyclenotes != null && item1.ccmcyclenotes != "") || (item1.IsRejectedByLiaison == true)).ToList();
                                for (int ii = 1; ii <= 12; ii++)
                                {
                                    MinuteandType minuteandType = new MinuteandType();
                                    var totcountformonth = dataview3.Where(item1 => item1.CycleCreatedON?.Month == ii && item1.CycleCreatedON?.Year == DateTime.Now.Year).ToList().Count();

                                    minuteandType.Minute = totcountformonth;
                                    minuteandType.Type = ii.ToString();
                                    activeWorkQueTotal.MonthNames.Add(minuteandType);
                                }
                            }
                            activeWorkQueTotals.Add(activeWorkQueTotal);
                        }
                        var lsttypes = activeWorkQueTotals.Select(x => x.CycleType).ToList();

                        var res123 = activeWorkQueTotals.Where(x => x.CycleType != "").SelectMany(a => a.MonthNames).ToList().GroupBy(x => x.Type).Select(group => new MinuteandType
                        {
                            Type = group.Key,
                            Minute = group.Sum(x => x.Minute)
                        }).ToList();
                        ActiveWorkQueTotals activeWorkQueTotal1 = new ActiveWorkQueTotals();
                        activeWorkQueTotal1.CycleType = "Total";
                        activeWorkQueTotal1.colorname = "All";
                        activeWorkQueTotal1.MonthNames = res123;
                        activeWorkQueTotals.Add(activeWorkQueTotal1);
                        foreach (var item in activeWorkQueTotals)
                        {
                            item.TotalCounts = item.MonthNames.Sum(x => x.Minute);
                        }
                    }
                    catch (Exception ex)
                    {
                        throw ex;

                    }
                    //total number of rows count     
                    recordsTotal = dataview2.Count();
                    //Paging     
                    if (pageSize == -1)
                    {
                        pageSize = recordsTotal;
                    }
                    var dataView1 = dataview2.Skip(skip).Take(pageSize).ToList();
                    int i = 0;
                    string activeworkquechart = ConvertViewToString("ActiveWorkQueChart", activeWorkQueTotals);
                    var patientidsnew = dataView1.Select(x1 => x1.Id).Distinct().ToList();
                    var patientsharedcareplans = _db.carePlanSharedHistories.AsNoTracking().Where(x => patientidsnew.Contains(x.PatientId)).AsQueryable();
                    foreach (var item in dataView1)
                    {

                        var color = "";
                        var dayLapsed = 0;
                        var dateColor = "";
                        var today = DateTime.Today.Date;

                        var dateEntered = item?.CcmStatus == "Enrolled" ? Convert.ToDateTime(item.CycleCreatedON)
                                        : item?.CcmStatus == "Clinical Sign-Off" ? Convert.ToDateTime(item.CcmClinicalSignOffDate)
                                          : item?.CcmStatus == "Ready for Clinical Sign-Off" ? Convert.ToDateTime(item.CcmReadyforClinicalSignOffDate)
                                         : item?.CcmStatus == "Expired" ? Convert.ToDateTime(item.CCMUpdatedOn)
                                        : Convert.ToDateTime(item?.CcmClaimSubmissionDate);

                        var yearDifference = today.Year - Convert.ToDateTime(dateEntered).Year > 0
                                           ? today.Year - Convert.ToDateTime(dateEntered).Year
                                           : 1;

                        if (item?.CcmReconciliationDate != null)
                        {
                            var reconciledDate = Convert.ToDateTime(item.CcmReconciliationDate).Date;
                            var dayOfYear = today.Year > Convert.ToDateTime(reconciledDate).Year
                                               ? today.DayOfYear + 365 * yearDifference : today.DayOfYear;
                            //if(item.CcmStatus=="Enrolled")
                            //{
                            //    dayLapsed = dayOfYear - Convert.ToDateTime(reconciledDate).DayOfYear;
                            //}
                            //else
                            //{
                            //    dayLapsed = Convert.ToInt32((today - reconciledDate).TotalHours);
                            //}
                            dayLapsed = Convert.ToInt32((DateTime.Now - reconciledDate).TotalHours);

                            color = dayLapsed > 0 && dayLapsed < 30 ? "lightskyblue"
                                      : dayLapsed >= 30 && dayLapsed <= 33 ? "lightgreen"
                                      : dayLapsed >= 34 && dayLapsed <= 45 ? "#ffff7f"
                                      : dayLapsed >= 45 ? "#F5A08E" : "";
                        }
                        else
                        {
                            //if (item.CcmStatus == "Enrolled")
                            //{
                            //    dayLapsed = (today.Year > Convert.ToDateTime(dateEntered).Year ? today.DayOfYear + 365 * yearDifference : today.DayOfYear)
                            //         - Convert.ToDateTime(dateEntered).DayOfYear;
                            //}
                            //else
                            //{
                            //    dayLapsed = Convert.ToInt32((today - dateEntered).TotalHours);
                            //}
                            dayLapsed = Convert.ToInt32((DateTime.Now - dateEntered).TotalHours);

                            dateColor = dayLapsed >= 30 && dayLapsed <= 45 ? "#ffff7f" : dayLapsed >= 45 ? "#F5A08E" : "";
                        }
                        PatientViewModel item1 = new PatientViewModel();
                        if (i == 0)
                        {
                            item1.cPTCODEs = res;
                            item1.ActiveWorkQueTotals = activeWorkQueTotals;

                        }
                        i = i + 1;
                        item1.TranslatorName = item.TranslatorName ?? "";
                        var reviewtimeminutes = item.ReviewTime;
                        item1.ccmnotes = item.ccmcyclenotes ?? "";
                        item1.SubmittedBy = item.SubmittedBy ?? "";
                        try
                        {
                            var icd10codes = icd10codesall.Where(x => x.PatientId == item.Id).Select(x => x.Code10).ToList();
                            item1.ICD10Codes = icd10codes.Count > 0 ? string.Join("\n", icd10codes.ToArray()) : "";
                            item1.Address = item.address ?? "";
                            if (forcareplan != "")
                            {
                                //var totalpeoplestobeshared= (_db.SecondaryDoctors.AsNoTracking().Where(x => x.PatientId == item.Id && x.Email != null && x.Email != "" && x.IsShareCarePlan == true).ToList().Count() + _db.PatientProfile_UrgencyContacts.AsNoTracking().Where(x => x.PatientId == item.Id && x.PrimaryIsShareCarePlan==true).ToList().Count());
                                //var patient = _db.Patients.Find(item.Id);
                                //var doctor = _db.Physicians.Find(patient.PhysicianId);
                                //if (!string.IsNullOrEmpty(doctor.Email))
                                //{
                                //    totalpeoplestobeshared += 1;
                                //}
                                //if (!string.IsNullOrEmpty(patient.Email))
                                //{
                                //    totalpeoplestobeshared += 1;
                                //}
                                item1.TotalCountCarePlanTobeShared = item.TotalCountCarePlanTobeShared;
                                item1.TotalSharedRemining = item.TotalCountCarePlanTobeShared - item.TotalCareplanShared;
                                item1.TotalCareplanShared = item.TotalCareplanShared;
                            }
                        }
                        catch /*(Exception ex)*/
                        {
                            item1.ICD10Codes = "";
                            item1.Address = "";

                        }

                        item1.Cyclestr = "Cycle " + item.Cycle.ToString() + " (" + item.CycleCreatedON?.Date.ToString("MMM") + "-" + item.CycleCreatedON?.Date.ToString("yy") + ")";
                        if (item1.ccmnotes != "")
                        {
                            item1.Activitytext = "Rejected";
                        }
                        else
                        {
                            if (reviewtimeminutes?.TotalMinutes == 0)
                            {
                                item1.Activitytext = "No Work Done";
                            }
                            else
                            {
                                if (reviewtimeminutes?.TotalMinutes > 0 && reviewtimeminutes?.TotalMinutes < 15)
                                {
                                    item1.Activitytext = "More Work Require";
                                }
                                else
                                {
                                    item1.Activitytext = "Minimum Requirements Meet";
                                }
                            }
                        }
                        try
                        {
                            if (item1.Activitytext == "Rejected" || item?.CcmStatus.ToLower() == "claims submission")
                            {
                                item1.QAQCName = CCM.HelperExtensions.GetUserNamebyID(item.CCMUpdatedBy);
                                item1.CCMUpdatedOn = item?.CCMUpdatedOn?.ToString("MM/dd/yyyy");
                                if (!string.IsNullOrEmpty(item.ApprovedBy))
                                {
                                    item1.approvedby = HelperExtensions.GetUserNamebyID(item.ApprovedBy);

                                }
                                if (!string.IsNullOrEmpty(item.RejectedBy))
                                {
                                    item1.rejectedby = HelperExtensions.GetUserNamebyID(item.RejectedBy);

                                }
                                else
                                {
                                    if (item.IsRejectedByLiaison == true)
                                    {
                                        item1.rejectedby = item.RejectedbyLiaison;
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {

                            throw ex;
                        }

                        item1.ReviewTime = reviewtimeminutes?.ToString(@"hh\:mm\:ss");
                        item1.Color = color;
                        item1.dateColor = dateColor;
                        item1.DaysinQue = dayLapsed;
                        item1.CcmEnrolledOn = item?.CCMEnrolledOn.ToString();
                        item1.LiasionAssignedOn = item?.LiasionAssignedOn.ToString();
                        item1.CallingStatus = item?.callingstatus;
                        item1.emrtype = item?.emrtype;
                        item1.emrnumber = item?.emrnumber;

                        item1.insuranceid = item.insuranceid ?? "";
                        item1.insurancename = item.insurancename ?? "";
                        item1.medicaidid = item.MedicaidIdNumber ?? "";
                        item1.medicareid = item.MedicareIdNumber ?? "";
                        item1.otherinsuranceid = item.OtherInsuranceIdNumber ?? "";
                        //item1.medicaideligibility = item?.medicaideligibility;
                        //item1.medicareeligibility = item?.medicareeligibility;
                        item1.capitated = item?.capitated;
                        item1.LastUpdatedDate = item.lastupdatedate == null ? "" : item.lastupdatedate.ToString();

                        item1.capitatedfrom = item.capitatedfrom == null ? "" : item.capitatedfrom?.ToString("MM/dd/yyyy");
                        item1.capitatedto = item.capitatedto == null ? "" : item.capitatedto?.ToString("MM/dd/yyyy");
                        if (item?.CcmStatus.ToLower() == "claims submission")
                        {


                            if (Convert.ToDateTime(item.CcmClinicalSignOffDate).Year != 0001)
                            {
                                item1.CcmClinicalSignOffDate = Convert.ToDateTime(item.CcmClinicalSignOffDate).ToShortDateString();
                            }

                            //  var CcmBillingCodes = _db.BillingCycles.AsNoTracking().Where(x => x.PatientId == item.Id && x.Cycle == item.Cycle).FirstOrDefault();

                           
                            string billingcode = "";
                            if (item.BillingCycleId != null)
                            {
                                var billingCycle = _db.BillingCycles.AsQueryable().Where(p => p.Id == item.BillingCycleId).Include(x => x.BillingCodes).Select(p => p).FirstOrDefault();
                                if (billingCycle != null)
                                {
                                    if (billingCycle.BillingCodes != null)
                                    {
                                        var BillingCodes = billingCycle.BillingCodes.Where(p => p.BillingCategoryId == item.BillingCategoryId).ToList();
                                        foreach (var billingcodes in BillingCodes)
                                        {
                                    if (billingcode == "")
                                    {
                                        billingcode =billingcodes.BillingCategory.Name + "|" + billingcodes.Name;
                                    }
                                    else
                                    {
                                        billingcode = billingcode + System.Environment.NewLine + billingcodes.BillingCategory.Name + "|" + billingcodes.Name;
                                    }
                                        

                                        }
                                        item1.CcmBillingCode = billingcode;
                                    }

                                }

                            }
                            //var billingcode = HelperExtensions.GetPatientBillingCodes(item.BillingCycleId,item.BillingCategoryId);
                            //item1.CcmBillingCode = billingcode;

                        }
                        item1.DateEntered = dateEntered.ToShortDateString();
                        item1.Cycle = item.Cycle;
                        //if (User.IsInRole("Admin"))
                        //{
                        //    item1.liaisonName = item.liaisonFirstName + " " + item?.liaisonLastName;


                        //}
                        item1.liaisonName = item.liaisonFirstName + " " + item?.liaisonLastName;
                        item1.PatientName = item.PatientName;

                        item1.Gender = item.Gender;

                        var age = (int.Parse(DateTime.Today.ToString("yyyyMMdd")) - int.Parse(item.BirthDate?.ToString("yyyyMMdd"))) / 10000;
                        item1.BirthDate = item.BirthDate?.ToString("MM/dd/yyyy") + " " + age.ToString() + " Years";


                        if (status != "Claims Submission")
                        {
                            item1.AppointmentDate = item.AppointmentDate == null ? "" : item.AppointmentDate.Value.ToString("MM/dd/yyyy hh:mm tt");
                        }
                        item1.DocFirstName = item.DocFirstName + " " + item?.DocLastName;
                        item1.note = item.note == null ? "" : item.note;
                        item1.Id = item.Id;
                        item1.CcmStatus = item.CcmStatus;
                        item1.UserRole = item.UserRole;
                        item1.PhysicianGroup = item.PhysicianGroupName;
                        //if (User.IsInRole("Liaison") || User.IsInRole("Admin"))
                        //{
                        //    if (item.EnrollmentStatus == "Enrolled" && item.CcmStatus == "Enrolled")
                        //    {
                        //        item1.CMMReviewLink = "/Patient/Details?Id=" + item.Id;

                        //    }
                        //    else if (item.EnrollmentStatus == "Enrolled" && item.CcmStatus == "Clinical Sign-Off" && User.IsInRole("Liaison"))
                        //    {
                        //        //item1.CMMReviewLink = "/CcmStatus/ReviewTimeV1?patientId=" + item.Id + "&cycle=" + item1.Cycle;
                        //        item1.CMMReviewLink = "/Patient/Details?Id=" + item.Id;

                        //    }
                        //    else if (User.IsInRole("Admin") && item.CcmStatus == "Clinical Sign-Off")
                        //    {
                        //        item1.CMMReviewLink = "/CcmStatus/ReviewTimeV1?patientId=" + item.Id + "&cycle=" + item1.Cycle;
                        //        // item1.CMMReviewLink = "/CcmStatus/ReviewClaimSubmission?patientId=" + item.Id;

                        //        //item1.CMMReviewLink = "/PatientProfile/Create?patientId=" + item.Id;

                        //    }
                        //    else if (User.IsInRole("Admin"))
                        //    {
                        //        item1.CMMReviewLink = "/Patient/Details?Id=" + item.Id;
                        //    }
                        //}
                        //else if (User.IsInRole("Physician") || User.IsInRole("PhysiciansGroup"))
                        //{
                        //    //  item1.CMMReviewLink = "/CcmStatus/ReviewTimeV1?patientId=" + item.Id;

                        //    item1.CMMReviewLink = "/Patient/Details?Id=" + item.Id;

                        //}
                        //else if (User.IsInRole("QAQC") && item.CcmStatus == "Clinical Sign-Off")
                        //{
                        //    item1.CMMReviewLink = "/CcmStatus/ReviewTimeV1?patientId=" + item.Id + "&cycle=" + item1.Cycle;
                        //}

                        item1.CMMReviewLink = "/Patient/Details?Id=" + item.Id;
                        //var laisonID = _db.patientAppointments.Where(p => p.LiaisonID.Value != null).ToList().Select(X => X.LiaisonID);
                        var laisonID = _db.patientAppointments.AsQueryable().Select(X => X.LiaisonID);




                        var translatorIds = _db.Liaisons.AsQueryable().Where(p => (laisonID.Contains(p.Id) && p.IsTranslator == true)).Select(p => p.Id);


                        var laisonsIds = _db.Liaisons.AsQueryable().Where(p => (laisonID.Contains(p.Id) && p.IsTranslator == false)).Select(p => p.Id);

                        //var enrollerID = _db.patientAppointments.Where(p => p.SaleStaffID.Value != null).ToList().Select(X => X.SaleStaffID);
                        var enrollerID = _db.patientAppointments.AsQueryable().Select(X => X.SaleStaffID);

                        int? Id = item1.Id;
                        var laisonspatientAppointmentids = _db.patientAppointments.AsQueryable().Where(p => p.PatientID == Id && p.LiaisonID != null).Select(p => p.ID);

                        var lpatientAppointmentid = _db.patientAppointments.AsQueryable().Where(p => p.PatientID == Id && p.LiaisonID != null).Select(p => p.ID).FirstOrDefault();

                        int? transid = _db.Patients.AsQueryable().Where(p => p.Id.Equals(item1.Id) && p.TranslatorId != null).Select(p => p.TranslatorId).FirstOrDefault();

                        int? Liasonid = _db.Patients.AsQueryable().Where(p => p.Id.Equals(item1.Id) && p.LiaisonId != null).Select(p => p.LiaisonId).FirstOrDefault();

                        // Adding Different Appointments
                        if (status != "Claims Submission")
                        {
                            if (Liasonid != null)
                            {
                                var lappoint = _db.patientAppointments.AsQueryable().Where(p => p.LiaisonID == Liasonid && p.PatientID == item1.Id).Select(p => p.LiaisonID);


                                if (lappoint != null)
                                {
                                    //DateTime app = new DateTime();
                                    foreach (var item2 in lappoint)
                                    {
                                        List<DateTime> appointments = _db.patientAppointments.AsQueryable().Where(p => p.LiaisonID == item2.Value && p.PatientID == item1.Id).Select(p => p.StartTime).ToList();
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
                                            pastapplist.Sort((a, b) => b.CompareTo(a));
                                            pastapp = string.Join(System.Environment.NewLine, pastapplist);
                                            pastapp = "Past Appointments:" + System.Environment.NewLine + pastapp + System.Environment.NewLine;


                                        }
                                        else
                                        {
                                            pastapp = "";
                                        }
                                        item1.CouncelorAppointmentDate = pastapp + System.Environment.NewLine + upcomings;

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

                                var tappoint = _db.patientAppointments.AsQueryable().Where(p => p.LiaisonID == transid && p.PatientID == item1.Id).Select(p => p.LiaisonID);

                                if (tappoint != null)
                                {
                                    //DateTime app = new DateTime();
                                    foreach (var item2 in tappoint)
                                    {
                                        List<DateTime> appointments = _db.patientAppointments.AsQueryable().Where(p => p.LiaisonID == item2.Value).Select(p => p.StartTime).ToList();
                                        appointments.Sort((a, b) => a.CompareTo(b));
                                        List<DateTime> upcomingapplist = appointments.Where(p => p >= DateTime.Now).Select(p => p).Take(2).ToList();
                                        string upcomings;
                                        if (upcomingapplist != null && upcomingapplist.Count() != 0)
                                        {
                                            upcomings = string.Join(System.Environment.NewLine, upcomingapplist);
                                            upcomings = "Upcoming :" + System.Environment.NewLine + upcomings + System.Environment.NewLine;

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
                                            pastapplist.Sort((a, b) => b.CompareTo(a));
                                            pastapp = string.Join(System.Environment.NewLine, pastapplist);
                                            pastapp = "Past Appointments:" + System.Environment.NewLine + pastapp + System.Environment.NewLine;


                                        }
                                        else
                                        {
                                            pastapp = "";
                                        }
                                        item1.TranslatorAppointmentDate = pastapp + System.Environment.NewLine + upcomings;

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

                            var enrollerpatientsapp = _db.patientAppointments.AsQueryable().Where(p => p.PatientID == Id && p.SaleStaffID != null).Select(p => p.ID).FirstOrDefault();

                            var eappoint = _db.patientAppointments.AsQueryable().Where(p => p.PatientID == item.Id && p.SaleStaffID != null).Select(p => p.SaleStaffID).ToList();

                            if (eappoint != null)
                            {
                                //DateTime app = new DateTime();
                                foreach (var item2 in eappoint)
                                {
                                    List<DateTime> appointments = _db.patientAppointments.AsQueryable().Where(p => p.SaleStaffID == item2.Value && p.PatientID == item.Id).Select(p => p.StartTime).ToList();
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
                                        pastapplist.Sort((a, b) => b.CompareTo(a));
                                        pastapp = string.Join(System.Environment.NewLine, pastapplist);
                                        pastapp = "Past Appointments:" + System.Environment.NewLine + pastapp + System.Environment.NewLine;


                                    }
                                    else
                                    {
                                        pastapp = "";
                                    }
                                    item1.EnrollerAppointmentDate = pastapp + System.Environment.NewLine + upcomings;

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
                            //int? id = lId;
                            //        if (id != null)
                            //        {
                            //            int Id2 = (int)id;
                            //            if (translatorIds.Contains(Id2))
                            //            {
                            //                item1.TranslatorAppointmentDate = item.AppointmentDate == null ? "" : item.AppointmentDate.Value.ToString("MM/dd/yyyy hh:mm tt"); ;
                            //            }
                            //        }
                            //    }
                            //    //  translator



                            //    if (lId != null)
                            //    {
                            //        int? id = lId;
                            //        if (id != null)
                            //        {
                            //            int Id2 = (int)id;
                            //            if (laisonsIds.Contains(Id2))
                            //            {
                            //                item1.CouncelorAppointmentDate = item.AppointmentDate == null ? "" : item.AppointmentDate.Value.ToString("MM/dd/yyyy hh:mm tt"); ;
                            //            }
                            //        }
                            //    }
                            //var enrollerpatientsapp = _db.patientAppointments.Where(p => p.PatientID == Id && p.SaleStaffID != null).Select(p => p.ID).FirstOrDefault();

                            //if (enrollerpatientsapp != null || enrollerpatientsapp!=0)
                            //{
                            //    var enrollerId = _db.patientAppointments.Where(p => p.ID.Equals(enrollerpatientsapp)).Select(p => p.SaleStaffID).FirstOrDefault();

                            //    int? id = enrollerId;
                            //        if (id != null)
                            //        {
                            //            int Id2 = (int)id;
                            //            if (enrollerID.Contains(Id2))
                            //            {
                            //            DateTime app = _db.patientAppointments.Where(p => p.ID.Equals(enrollerpatientsapp)).Select(p => p.StartTime).FirstOrDefault();
                            //            item1.EnrollerAppointmentDate = Convert.ToString(app);
                            //                }
                            //        }




                            //    }
                            //}



                            //  item1.CMMReviewLink = "/CcmStatus/ReviewTimeV1?patientId=" + item.Id + "&cycle=" + item1.Cycle;



                        }
                        if (status == "Claims Submission")
                        {
                            item1.Category = item.CategoryName;


                        }
                        if (status == "Ready for Clinical Sign-Off")
                        {
                            if (item.BillingCategoryId != null)
                            {
                                item1.Category = _db.BillingCategories.AsQueryable().Where(p => p.BillingCategoryId == item.BillingCategoryId).Select(p => p.Name).FirstOrDefault();
                            }
                        }

                        //Adding  Category  in Claim Submission
                        //if (status == "Claims Submission")
                        //{
                        //    var patientcategories = _db.Patients_BillingCategories.Where(p => p.PatientId == item1.Id).Select(p => p).ToList();
                        //    foreach (var patientcat in patientcategories)
                        //    {

                        //        item1.Category = item1.Category + System.Environment.NewLine + patientcat.BillingCategory.Name + System.Environment.NewLine + "Enrolled On:" + "" + patientcat.EnrolledOn;
                        //        if (patientcat.DeEnrolledOn != null)
                        //        {
                        //            item1.Category = item1.Category + System.Environment.NewLine + "DeEnrolled On:" + " " + patientcat.DeEnrolledOn;
                        //        }

                        //    }

                        //}

                        lstPatients.Add(item1);
                    }


                    //SORT
                    if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
                    {
                        lstPatients = lstPatients.OrderBy(sortColumn + " " + sortColumnDir).ToList();
                    }
                    //            var lstPatients1 = lstPatients
                    //.GroupBy(n => new { n.Id, n.Cycle })
                    //.Select(g => g.First())

                    //.ToList();


                    var jsonResult = Json(new { cptcodestotal = res, activeworkquechart = activeworkquechart, draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = lstPatients, Totaltobeshared = lstCarePlaneShared, JsonRequestBehavior.AllowGet });
                    jsonResult.MaxJsonLength = int.MaxValue;
                    //Returning Json Data    
                    return jsonResult;
                



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
                catch /*(Exception ed)*/
                {
                    return "";

                }
            }
            //public string RenderViewToString(ControllerContext context,
            //                            string viewPath,
            //                            object model = null,
            //                            bool partial = false)
            //{
            //    // first find the ViewEngine for this view
            //    ViewEngineResult viewEngineResult = null;
            //    if (partial)
            //        viewEngineResult = ViewEngines.Engines.FindPartialView(context, viewPath);
            //    else
            //        viewEngineResult = ViewEngines.Engines.FindView(context, viewPath, null);

            //    if (viewEngineResult == null)
            //        throw new FileNotFoundException("View cannot be found.");

            //    // get the view and attach the model to view data
            //    var view = viewEngineResult.View;
            //    context.Controller.ViewData.Model = model;

            //    string result = null;

            //    using (var sw = new StringWriter())
            //    {
            //        var ctx = new ViewContext(context, view,
            //                                    context.Controller.ViewData,
            //                                    context.Controller.TempData,
            //                                    sw);
            //        view.Render(ctx, sw);
            //        result = sw.ToString();
            //    }

            //    return result;
            //}
            //public static string RenderPartialToString(string controlName, object viewData)
            //{
            //    ViewPage viewPage = new ViewPage() { ViewContext = new ViewContext() };

            //    viewPage.ViewData = new ViewDataDictionary(viewData);
            //    viewPage.Controls.Add(viewPage.LoadControl(controlName));

            //    StringBuilder sb = new StringBuilder();
            //    using (StringWriter sw = new StringWriter(sb))
            //    {
            //        using (HtmlTextWriter tw = new HtmlTextWriter(sw))
            //        {
            //            viewPage.RenderControl(tw);
            //        }
            //    }

            //    return sb.ToString();
            //}
            public PartialViewResult ActiveWorkQueChart(List<ActiveWorkQueTotals> activeWorkQueTotals)
            {
                return PartialView(activeWorkQueTotals);
            }
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> ReviewClaimSubmission(int? patientId, string userId)
        {
            ViewBag.UserId = userId;

            var patient = await _db.Patients.FindAsync(patientId);
            if (patient == null)
            {
                ViewBag.Message = "Patient Not Found!";
                return View("Error");
            }

            return View(patient);
        }


        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Reconciliation()
        {
            return View(await _db.Patients.Where(p => p.EnrollmentStatus == "Enrolled" && p.CcmReconciliationDate != null).ToListAsync());
        }


        public async Task<ActionResult> ReviewTime(int patientId, string cycle)
        {
            var patient = await _db.Patients.FindAsync(patientId);
            var phoneNumbers = new List<string>();

            if (!string.IsNullOrEmpty(patient?.MobilePhoneNumber))
                phoneNumbers.Add(patient.MobilePhoneNumber);
            if (!string.IsNullOrEmpty(patient?.HomePhoneNumber))
                phoneNumbers.Add(patient.HomePhoneNumber);
            if (!string.IsNullOrEmpty(patient?.WorkPhoneNumber))
                phoneNumbers.Add(patient.WorkPhoneNumber);
            if (!string.IsNullOrEmpty(patient?.EmergencyNumber))
                phoneNumbers.Add(patient.EmergencyNumber);

            var callHistories = new List<CallHistory>();
            var accountSid = ConfigurationManager.AppSettings["TwilioAccountSid"];
            var authToken = ConfigurationManager.AppSettings["TwilioAuthToken"];

            TwilioClient.Init(accountSid, authToken);

            foreach (var number in phoneNumbers)
            {
                var to = number.Length == 11 && number.Substring(0) == "1"
                       ? "+" + number : number.Length == 10
                       ? "+1" + number : number;

                var calls = await CallResource.ReadAsync(to: new PhoneNumber(to));
                if (!calls.Any())
                    continue;

                callHistories.AddRange(calls.Select(call => new CallHistory
                {
                    Duration = call?.StartTime != null && call.EndTime != null
                                                                         ? (TimeSpan)(call.EndTime - call.StartTime)
                                                                         : TimeSpan.Zero,
                })
                );
            }

            var totalReview = _db.ReviewTimeCcms.Where(r => r.PatientId == patient.Id).AsNoTracking().ToList();
            var reviewCycle1 = totalReview.Where(r => r.StartTime <= patient?.CcmClinicalSignOffDate).ToList();
            var reviewCycle2 = totalReview.Where(r => r.StartTime > patient?.CcmClinicalSignOffDate).ToList();
            var reviews = string.IsNullOrEmpty(cycle) || cycle == "0" ? totalReview : cycle == "1" ? reviewCycle1 : reviewCycle2;

            ViewBag.CurrentPage = string.IsNullOrEmpty(cycle) || cycle == "0" ? "Total" : cycle == "1" ? "Cycle1" : "Cycle2";
            ViewBag.TotalTimeSpent = reviews.Any()
                                   ? reviews.Aggregate(TimeSpan.Zero, (sum, review) => sum + review.ReviewTime)
                                   : TimeSpan.Zero;

            ViewBag.TotalCallTime = callHistories.Any() ? callHistories.Aggregate(TimeSpan.Zero, (sum, review) => sum + review.Duration) : TimeSpan.Zero;
            ViewBag.PatientName = patient?.FirstName + " " + patient?.LastName;
            ViewBag.PatientId = patientId;

            return View(new ReviewTimeViewModel
            {
                Cycle = patient?.Cycle,
                ReviewTimeCcm = reviews,
                PatientId = patientId,
                BillingCode1 = patient?.CcmBillingCode,
                BillingCode2 = patient?.CcmBillingCode2,
                TotalReviewTime = totalReview.Any() ? totalReview.Aggregate(TimeSpan.Zero, (sum, review) => sum + review.ReviewTime) : TimeSpan.Zero,
                ReviewTimeCycle1 = reviewCycle1.Any() ? reviewCycle1.Aggregate(TimeSpan.Zero, (sum, review) => sum + review.ReviewTime) : TimeSpan.Zero,
                ReviewTimeCycle2 = reviewCycle2.Any() ? reviewCycle2.Aggregate(TimeSpan.Zero, (sum, review) => sum + review.ReviewTime) : TimeSpan.Zero
            });
        }

        [HttpPost]
        [Authorize(Roles = "Admin, QAQC")]
        public async Task<ActionResult> ReviewTime(ReviewTimeViewModel model)
        {
            var patient = await _db.Patients.FindAsync(model.PatientId);
            if (patient != null)
            {
                patient.CcmBillingCode = model.BillingCode1;
                patient.CcmBillingCode2 = model.BillingCode2;
            }

            _db.Entry(patient).State = EntityState.Modified;
            await _db.SaveChangesAsync();

            return RedirectToAction("Create", "PatientProfile", new { patientId = model.PatientId });
        }


        [Authorize(Roles = "Liaison, Physician, PhysiciansGroup, Admin, QAQC, LiaisonGroup")]
        public async Task<ActionResult> ReviewTimeV1(int patientId, int cycle = 0)
        {



            var patient = await _db.Patients.FindAsync(patientId);
            //var phoneNumbers = new List<string>();

            //if (!string.IsNullOrEmpty(patient?.MobilePhoneNumber))
            //    phoneNumbers.Add(patient.MobilePhoneNumber);
            //if (!string.IsNullOrEmpty(patient?.HomePhoneNumber))
            //    phoneNumbers.Add(patient.HomePhoneNumber);
            //if (!string.IsNullOrEmpty(patient?.WorkPhoneNumber))
            //    phoneNumbers.Add(patient.WorkPhoneNumber);
            //if (!string.IsNullOrEmpty(patient?.EmergencyNumber))
            //    phoneNumbers.Add(patient.EmergencyNumber);

            var callHistories = new List<CallHistory>();
            var accountSid = ConfigurationManager.AppSettings["TwilioAccountSid"];
            var authToken = ConfigurationManager.AppSettings["TwilioAuthToken"];


            try
            {
                callHistories = _db.CallHistories.Where(x => x.PatientID == patient.Id).AsNoTracking().ToList().OrderByDescending(x => x.StartTime).ToList();
                //TwilioClient.Init(accountSid, authToken);
                //foreach (var number in phoneNumbers)
                //{
                //    var to = number.Length == 11 && number.Substring(0) == "1"
                //           ? "+" + number : number.Length == 10
                //           ? "+1" + number : number;

                //    var calls = await CallResource.ReadAsync(to: new PhoneNumber(to));
                //    if (!calls.Any())
                //        continue;

                //    callHistories.AddRange(calls.Select(call => new CallHistory
                //    {
                //        Duration = call?.StartTime != null && call.EndTime != null
                //                                                             ? (TimeSpan)(call.EndTime - call.StartTime)
                //                                                             : TimeSpan.Zero,
                //    })
                //    );


                //}
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(0);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                ViewBag.ErrorLine = line.ToString();
                if (ex.InnerException != null)
                {

                    ViewBag.Message = ex.InnerException.ToString();
                }



            }


            var totalReview = await _db.ReviewTimeCcms.Where(r => r.PatientId == patient.Id && r.Activity != null && r.EndTime != null).AsNoTracking().ToListAsync();

            List<ReviewTimeCcm> copyTotalReviewTimeCcms = new List<ReviewTimeCcm>(totalReview);

            //int cycleId = 1;
            List<ReviewTimeCcm> cycleReviewTimeCcms = null;
            List<ReviewTimeCycle> reviewTimeCycles = new List<ReviewTimeCycle>();
            List<int> cycles = totalReview.Select(x => x.Cycle).Distinct().OrderBy(x => x).ToList();

            if (copyTotalReviewTimeCcms.Any())
            {
                DateTime cycleStartDate = copyTotalReviewTimeCcms.FirstOrDefault()?.StartTime ?? DateTime.MinValue;
                DateTime cycleEndDate = DateTime.MinValue;
                ReviewTimeCycle reviewTimeCycle = null;
                //int cycleIndex = 1;
                foreach (var cycleitemin in cycles)
                {
                    reviewTimeCycle = new ReviewTimeCycle();
                    reviewTimeCycle.CycleId = cycleitemin;
                    //reviewTimeCycle.CycleId = cycleId++;
                    reviewTimeCycle.StartDate = copyTotalReviewTimeCcms.Where(x => x.Cycle == cycleitemin).FirstOrDefault().StartTime;
                    reviewTimeCycle.EndDate = copyTotalReviewTimeCcms.Where(x => x.Cycle == cycleitemin).LastOrDefault().EndTime.Value;

                    cycleReviewTimeCcms = copyTotalReviewTimeCcms.FindAll(rv =>
                        rv.Cycle == cycleitemin);

                    reviewTimeCycle.ReviewTimeCycleCcms = cycleReviewTimeCcms;
                    reviewTimeCycle.ReviewTimes = new List<TimeSpan>(cycleReviewTimeCcms.Select(rt => rt.ReviewTime));
                    reviewTimeCycles.Add(reviewTimeCycle);
                }
                //if (cycleStartDate != DateTime.MinValue)
                //{
                //    do
                //    {
                //        reviewTimeCycle = new ReviewTimeCycle();
                //        reviewTimeCycle.CycleId = cycle;
                //        //reviewTimeCycle.CycleId = cycleId++;
                //        reviewTimeCycle.StartDate = cycleStartDate;
                //        reviewTimeCycle.EndDate = cycleStartDate.AddDays(30);

                //        cycleReviewTimeCcms = copyTotalReviewTimeCcms.FindAll(rv =>
                //            rv.StartTime.Date.Ticks >= reviewTimeCycle.StartDate.Date.Ticks && rv.StartTime.Date.Ticks <= reviewTimeCycle.EndDate.Date.Ticks);

                //        reviewTimeCycle.ReviewTimeCycleCcms = cycleReviewTimeCcms;
                //        reviewTimeCycle.ReviewTimes = new List<TimeSpan>(cycleReviewTimeCcms.Select(rt => rt.ReviewTime));
                //        reviewTimeCycles.Add(reviewTimeCycle);

                //        cycleStartDate = cycleStartDate.Date.AddDays(31);

                //    }
                //    while (cycleIndex++ <= 11) ;


                //}

                //var firstReviewTime = copyTotalReviewTimeCcms.FirstOrDefault()?.StartTime;
                //DateTime startReviewDateTime = firstReviewTime.HasValue ? firstReviewTime.Value : DateTime.MinValue;
                //if (startReviewDateTime != DateTime.MinValue)
                //{
                //    cycleReviewTimeCcms = copyTotalReviewTimeCcms.FindAll(rv =>
                //        rv.StartTime.Date.Ticks >= startReviewDateTime.Date.Ticks && rv.StartTime.Date.Ticks <= startReviewDateTime.Date.AddDays(30).Ticks);
                //    if (cycleReviewTimeCcms.Any())
                //    {


                //        int removeItems = copyTotalReviewTimeCcms.RemoveAll(rt => cycleReviewTimeCcms.Contains(rt));
                //    }


                //}

            }
            try
            {
                ViewBag.Version = _db.FinalCarePlanNotes.Where(x => x.PatientId == patient.Id && x.Cycle == cycle).FirstOrDefault().Version;
            }
            catch (Exception)
            {


            }


            var reviewCycle1 = totalReview.Where(r => r.StartTime <= patient?.CcmClinicalSignOffDate).ToList();
            var reviewCycle2 = totalReview.Where(r => r.StartTime > patient?.CcmClinicalSignOffDate).ToList();
            // var reviews = cycle == 0 ? totalReview : reviewTimeCycles.Where(r => r.CycleId == cycle).Select(x=> new { x.ReviewTimeCycleCcms });
            var reviews = reviewTimeCycles.Count > 0 && reviewTimeCycles.Where(x => x.CycleId == cycle).Count() > 0 ? reviewTimeCycles.Find(r => r.CycleId == cycle).ReviewTimeCycleCcms.OrderByDescending(x => x.Id).ToList() : totalReview;

            ViewBag.Reasons = new SelectList(_db.Reasons.ToList(), "ReasonText", "ReasonText");
            ViewBag.CurrentPage = cycle;
            ViewBag.TotalTimeSpent = reviews.Any()
                                   ? reviews.Aggregate(TimeSpan.Zero, (sum, review) => sum + review.ReviewTime)
                                   : TimeSpan.Zero;

            ViewBag.TotalCallTime = callHistories.Any() ? callHistories.Aggregate(TimeSpan.Zero, (sum, review) => sum + review.Duration) : TimeSpan.Zero;
            ViewBag.PatientName = patient?.FirstName + " " + patient?.LastName;
            ViewBag.PatientId = patientId;
            ViewBag.isLocked = false;
            var alreadyaddedbilling = _db.BillingCycles.Where(x => x.PatientId == patient.Id && x.Cycle == cycle).FirstOrDefault();
            List<BillingCycleDetails> billingCycleDetails = new List<BillingCycleDetails>();
            List<BillingCycleComments> billingCycleComments = new List<BillingCycleComments>();
            if (alreadyaddedbilling != null)
            {
                ViewBag.isLocked = true;
                ViewBag.BillingCycleID = alreadyaddedbilling.Id;
                billingCycleDetails = _db.BillingCycleDetails.Where(x => x.BillingCycleId == alreadyaddedbilling.Id).ToList();
                billingCycleComments = _db.BillingCycleComments.Where(x => x.BillingCycleId == alreadyaddedbilling.Id).OrderByDescending(x => x.Id).ToList();
                if (billingCycleComments.Count > 0)
                {
                    foreach (var item in billingCycleComments)
                    {
                        var user = _db.Users.Where(x => x.Id == item.CreatedBy).FirstOrDefault();
                        if (user != null)
                        {
                            item.CreatedBy = user.FirstName + " " + user.LastName;

                        }

                    }
                }
            }

            return View(new ReviewTimeViewModel
            {
                Cycle = patient?.Cycle,
                ReviewTimeCcm = reviews,
                PatientId = patientId,
                BillingCode1 = patient?.CcmBillingCode,
                BillingCode2 = patient?.CcmBillingCode2,
                TotalReviewTime = totalReview.Any() ? totalReview.Aggregate(TimeSpan.Zero, (sum, review) => sum + review.ReviewTime) : TimeSpan.Zero,
                ReviewTimeCycle1 = reviewCycle1.Any() ? reviewCycle1.Aggregate(TimeSpan.Zero, (sum, review) => sum + review.ReviewTime) : TimeSpan.Zero,
                ReviewTimeCycle2 = reviewCycle2.Any() ? reviewCycle2.Aggregate(TimeSpan.Zero, (sum, review) => sum + review.ReviewTime) : TimeSpan.Zero,
                ReviewTimeCycles = reviewTimeCycles,
                BillingCycleDetails = billingCycleDetails,
                BillingCycleComments = billingCycleComments,
                billingCycles = _db.BillingCycles.Where(item => item.PatientId == patientId).ToList()

            });
        }


        [Authorize(Roles = "Liaison, Physician, PhysiciansGroup, Admin, QAQC, LiaisonGroup,Sales")]
        public async Task<PartialViewResult> _ReviewTimeV1(int patientId, int cycle = 0, int? BillingCategoryId = 0)
        {
            var BillingCatCheck = _db.BillingCycles.Where(p => p.PatientId == patientId && p.Cycle == cycle).Select(p => p.BillingCodes.Select(x => x.BillingCategoryId).ToList());
            List<int?> BillingCategoryCheck = new List<int?>();
            foreach (var BillingCatCheck2 in BillingCatCheck)
            {

                BillingCategoryCheck.AddRange(BillingCatCheck2);



            }
            ViewBag.CategoryCheck = BillingCategoryCheck;
            var patient = await _db.Patients.FindAsync(patientId);
            ViewBag.EnrolledCcmCategories = _db.Patients_BillingCategories.Where(x => x.PatientId == patient.Id).Select(p => p.BillingCategory).Distinct().ToList();
            //var phoneNumbers = new List<string>();

            //if (!string.IsNullOrEmpty(patient?.MobilePhoneNumber))
            //    phoneNumbers.Add(patient.MobilePhoneNumber);
            //if (!string.IsNullOrEmpty(patient?.HomePhoneNumber))
            //    phoneNumbers.Add(patient.HomePhoneNumber);
            //if (!string.IsNullOrEmpty(patient?.WorkPhoneNumber))
            //    phoneNumbers.Add(patient.WorkPhoneNumber);
            //if (!string.IsNullOrEmpty(patient?.EmergencyNumber))
            //    phoneNumbers.Add(patient.EmergencyNumber);

            var callHistories = new List<CallHistory>();
            var accountSid = ConfigurationManager.AppSettings["TwilioAccountSid"];
            var authToken = ConfigurationManager.AppSettings["TwilioAuthToken"];


            try
            {
                callHistories = _db.CallHistories.Where(x => x.PatientID == patient.Id).AsNoTracking().ToList().OrderByDescending(x => x.StartTime).ToList();
                //TwilioClient.Init(accountSid, authToken);
                //foreach (var number in phoneNumbers)
                //{
                //    var to = number.Length == 11 && number.Substring(0) == "1"
                //           ? "+" + number : number.Length == 10
                //           ? "+1" + number : number;

                //    var calls = await CallResource.ReadAsync(to: new PhoneNumber(to));
                //    if (!calls.Any())
                //        continue;

                //    callHistories.AddRange(calls.Select(call => new CallHistory
                //    {
                //        Duration = call?.StartTime != null && call.EndTime != null
                //                                                             ? (TimeSpan)(call.EndTime - call.StartTime)
                //                                                             : TimeSpan.Zero,
                //    })
                //    );


                //}
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(0);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                ViewBag.ErrorLine = line.ToString();
                if (ex.InnerException != null)
                {

                    ViewBag.Message = ex.InnerException.ToString();
                }



            }


            var totalReview = await _db.ReviewTimeCcms.Where(r => r.PatientId == patient.Id && r.Activity != null && r.EndTime != null).AsNoTracking().ToListAsync();

            List<ReviewTimeCcm> copyTotalReviewTimeCcms = new List<ReviewTimeCcm>(totalReview);

            //int cycleId = 1;
            List<ReviewTimeCcm> cycleReviewTimeCcms = null;
            List<ReviewTimeCycle> reviewTimeCycles = new List<ReviewTimeCycle>();
            List<int> cycles = totalReview.Select(x => x.Cycle).Distinct().OrderBy(x => x).ToList();

            if (copyTotalReviewTimeCcms.Any())
            {
                DateTime cycleStartDate = copyTotalReviewTimeCcms.FirstOrDefault()?.StartTime ?? DateTime.MinValue;
                DateTime cycleEndDate = DateTime.MinValue;
                ReviewTimeCycle reviewTimeCycle = null;
                //int cycleIndex = 1;
                foreach (var cycleitemin in cycles)
                {
                    reviewTimeCycle = new ReviewTimeCycle();
                    reviewTimeCycle.CycleId = cycleitemin;
                    //reviewTimeCycle.CycleId = cycleId++;
                    reviewTimeCycle.StartDate = copyTotalReviewTimeCcms.Where(x => x.Cycle == cycleitemin).FirstOrDefault().StartTime;
                    reviewTimeCycle.EndDate = copyTotalReviewTimeCcms.Where(x => x.Cycle == cycleitemin).LastOrDefault().EndTime.Value;

                    cycleReviewTimeCcms = copyTotalReviewTimeCcms.FindAll(rv =>
                        rv.Cycle == cycleitemin);

                    reviewTimeCycle.ReviewTimeCycleCcms = cycleReviewTimeCcms;
                    reviewTimeCycle.ReviewTimes = new List<TimeSpan>(cycleReviewTimeCcms.Select(rt => rt.ReviewTime));
                    reviewTimeCycles.Add(reviewTimeCycle);
                }
                //if (cycleStartDate != DateTime.MinValue)
                //{
                //    do
                //    {
                //        reviewTimeCycle = new ReviewTimeCycle();
                //        reviewTimeCycle.CycleId = cycle;
                //        //reviewTimeCycle.CycleId = cycleId++;
                //        reviewTimeCycle.StartDate = cycleStartDate;
                //        reviewTimeCycle.EndDate = cycleStartDate.AddDays(30);

                //        cycleReviewTimeCcms = copyTotalReviewTimeCcms.FindAll(rv =>
                //            rv.StartTime.Date.Ticks >= reviewTimeCycle.StartDate.Date.Ticks && rv.StartTime.Date.Ticks <= reviewTimeCycle.EndDate.Date.Ticks);

                //        reviewTimeCycle.ReviewTimeCycleCcms = cycleReviewTimeCcms;
                //        reviewTimeCycle.ReviewTimes = new List<TimeSpan>(cycleReviewTimeCcms.Select(rt => rt.ReviewTime));
                //        reviewTimeCycles.Add(reviewTimeCycle);

                //        cycleStartDate = cycleStartDate.Date.AddDays(31);

                //    }
                //    while (cycleIndex++ <= 11) ;


                //}

                //var firstReviewTime = copyTotalReviewTimeCcms.FirstOrDefault()?.StartTime;
                //DateTime startReviewDateTime = firstReviewTime.HasValue ? firstReviewTime.Value : DateTime.MinValue;
                //if (startReviewDateTime != DateTime.MinValue)
                //{
                //    cycleReviewTimeCcms = copyTotalReviewTimeCcms.FindAll(rv =>
                //        rv.StartTime.Date.Ticks >= startReviewDateTime.Date.Ticks && rv.StartTime.Date.Ticks <= startReviewDateTime.Date.AddDays(30).Ticks);
                //    if (cycleReviewTimeCcms.Any())
                //    {


                //        int removeItems = copyTotalReviewTimeCcms.RemoveAll(rt => cycleReviewTimeCcms.Contains(rt));
                //    }


                //}

            }
            try
            {
                ViewBag.Version = _db.FinalCarePlanNotes.Where(x => x.PatientId == patient.Id && x.Cycle == cycle).FirstOrDefault().Version;
            }
            catch /*(Exception e)*/
            {


            }


            var reviewCycle1 = totalReview.Where(r => r.StartTime <= patient?.CcmClinicalSignOffDate).ToList();
            var reviewCycle2 = totalReview.Where(r => r.StartTime > patient?.CcmClinicalSignOffDate).ToList();
            // var reviews = cycle == 0 ? totalReview : reviewTimeCycles.Where(r => r.CycleId == cycle).Select(x=> new { x.ReviewTimeCycleCcms });
            var reviews = reviewTimeCycles.Count > 0 && reviewTimeCycles.Where(x => x.CycleId == cycle).Count() > 0 ? reviewTimeCycles.Find(r => r.CycleId == cycle).ReviewTimeCycleCcms.OrderByDescending(x => x.Id).ToList() : totalReview;
            //var reviews =new  List<ReviewTimeCcm>();
            //foreach (var review in reviews2)
            //{
            //    var userrole = HelperExtensions.GetUserRolebyID(review.UserId);
            //    if (userrole == "Counselor" ||userrole== "Translator")
            //    {
            //        reviews.Add(review);

            //    }



            //}

            ViewBag.Reasons = new SelectList(_db.Reasons.ToList(), "ReasonText", "ReasonText");
            ViewBag.CurrentPage = cycle;
            ViewBag.TotalTimeSpent = reviews.Any()
                                   ? reviews.Aggregate(TimeSpan.Zero, (sum, review) => sum + review.ReviewTime)
                                   : TimeSpan.Zero;





            ViewBag.TotalTimeSpent = reviews.Select(x => new ReviewTime_TimeviewModal
            {
                BillingCategoryId = x.BillingcategoryId,
                Time = reviews.Where(y => y.BillingcategoryId == x.BillingcategoryId).Any()
                                   ? reviews.Aggregate(TimeSpan.Zero, (sum, review) => sum + review.ReviewTime)
                                   : TimeSpan.Zero
            }).ToList();

            ViewBag.TotalCallTime = callHistories.Any() ? callHistories.Aggregate(TimeSpan.Zero, (sum, review) => sum + review.Duration) : TimeSpan.Zero;
            ViewBag.PatientName = patient?.FirstName + " " + patient?.LastName;
            ViewBag.PatientId = patientId;
            ViewBag.isLocked = false;
            ViewBag.CurrentCycle = CategoryCycleStatusHelper.GetPatientNewOrOldCycleByCategory(patientId, BillingCodeHelper.cmmBillingCatagoryid);
            ViewBag.SelectedBillingCategoryId = 0;
            ViewBag.BillingCodesList = _db.BillingCodes.ToList();
            //if (BillingCategoryId == 0)
            //{
            ViewBag.Billingcategories = _db.BillingCategories.ToList();
            //}
            //else
            //{
            //    var Billingcategories = _db.BillingCategories.Where(x => x.BillingCategoryId == BillingCategoryId).ToList();
            //    ViewBag.Billingcategories = Billingcategories;
            //    ViewBag.SelectedBillingCategoryId = Billingcategories.FirstOrDefault().BillingCategoryId;
            //}

            var alreadyaddedbilling = _db.BillingCycles.Where(x => x.PatientId == patient.Id && x.Cycle == cycle).FirstOrDefault();
            List<BillingCycleDetails> billingCycleDetails = new List<BillingCycleDetails>();
            List<BillingCycleComments> billingCycleComments = new List<BillingCycleComments>();
            if (alreadyaddedbilling != null)
            {
                ViewBag.isLocked = true;
                ViewBag.BillingCycleID = alreadyaddedbilling.Id;
                billingCycleDetails = _db.BillingCycleDetails.Where(x => x.BillingCycleId == alreadyaddedbilling.Id).ToList();
                billingCycleComments = _db.BillingCycleComments.Where(x => x.BillingCycleId == alreadyaddedbilling.Id).OrderByDescending(x => x.Id).ToList();
                if (billingCycleComments.Count > 0)
                {
                    foreach (var item in billingCycleComments)
                    {
                        var user = _db.Users.Where(x => x.Id == item.CreatedBy).FirstOrDefault();
                        if (user != null)
                        {
                            item.CreatedBy = user.FirstName + " " + user.LastName;

                        }

                    }
                }
            }

            //var patientcategory=_db.Patients_BillingCategories.Where(p=>p.PatientId==patientId).Select(p=>p.)
            //= _db.BillingCategories.Where(p=>p.BillingCategoryId!=BillingCodeHelper.cmmBillingCatagoryid).ToList();
            ViewBag.BillCat = (from c in _db.BillingCategories
                               where c.BillingCategoryId != BillingCodeHelper.cmmBillingCatagoryid
                               && (from p in _db.Patients_BillingCategories where p.PatientId == patientId select p.BillingCategoryId).Contains(c.BillingCategoryId)
                               select c).ToList();




            return PartialView(new ReviewTimeViewModel
            {

                Cycle = patient?.Cycle,
                ReviewTimeCcm = reviews,
                PatientId = patientId,
                BillingCode1 = patient?.CcmBillingCode,
                BillingCode2 = patient?.CcmBillingCode2,
                TotalReviewTime = totalReview.Any() ? totalReview.Aggregate(TimeSpan.Zero, (sum, review) => sum + review.ReviewTime) : TimeSpan.Zero,
                ReviewTimeCycle1 = reviewCycle1.Any() ? reviewCycle1.Aggregate(TimeSpan.Zero, (sum, review) => sum + review.ReviewTime) : TimeSpan.Zero,
                ReviewTimeCycle2 = reviewCycle2.Any() ? reviewCycle2.Aggregate(TimeSpan.Zero, (sum, review) => sum + review.ReviewTime) : TimeSpan.Zero,
                ReviewTimeCycles = reviewTimeCycles,
                BillingCycleDetails = billingCycleDetails,
                BillingCycleComments = billingCycleComments,
                billingCycles = _db.BillingCycles.Where(item => item.PatientId == patientId).ToList()

            });
        }


        [Authorize(Roles = "Liaison, Physician, PhysiciansGroup, Admin, QAQC, LiaisonGroup,Sales")]
        public string TestFun(int patientId, int cycle = 0, int? BillingCategoryId = 0)
        {






            List<string> usertimes = new List<string>();
            int Billingcategory = BillingCodeHelper.cmmBillingCatagoryid;
            var reviewtime = _db.ReviewTimeCcms.Where(p => p.PatientId == patientId && p.Cycle == cycle && p.BillingcategoryId == Billingcategory).Select(p => p).ToList();
            if (reviewtime.Count() > 0)
            {

                var userids = reviewtime.Select(p => p.UserId).Distinct().ToList();
                foreach (var item in userids)
                {
                    var reviews = reviewtime.Where(p => p.UserId == item).ToList();
                    var time = reviews.Any()
                   ? reviews.Aggregate(TimeSpan.Zero, (sum, review) => sum + review.ReviewTime)
                   : TimeSpan.Zero;
                    // var time = reviewtime.Where(p => p.UserId == item).Aggregate((p => p.ReviewTime.Minutes);

                    string userrole = HelperExtensions.GetUserRolebyID(item);
                    if (userrole == "Admin")
                    {
                        continue;
                    }
                    if (userrole == "Sales")
                    {
                        userrole = "Enroller";
                    }
                    string fullname = userrole + "=" + time.ToString(@"hh\:mm\:ss");
                    usertimes.Add(fullname);

                }
            }
            Session["ReviewTimeViewModel"] = usertimes;

            return "";

        }




        public string GetUserNamebyID(string userid)
        {
            return _db.Users.Find(User.Identity.GetUserId()).FirstName + " " + _db.Users.Find(User.Identity.GetUserId()).LastName;
        }
        public string GetTotalTimeforSelectedReviews(int[] timeSpans)
        {
            if (timeSpans == null)
            {
                return "";
            }
            var reviewtimeccms = _db.ReviewTimeCcms.AsNoTracking().Where(x => timeSpans.Contains(x.Id)).Select(x => x.ReviewTime).ToList();
            //List<TimeSpan> newtimespans = new List<TimeSpan>();

            //foreach (var item in timeSpans)
            //{
            //    DateTime dt;
            //    if (!DateTime.TryParseExact(item, "hh:mm:ss", CultureInfo.InvariantCulture,
            //                                                  DateTimeStyles.None, out dt))
            //    {
            //        // handle validation error
            //    }
            //    TimeSpan time = dt.TimeOfDay;
            //    newtimespans.Add(time);
            //}
            var totalSpan = reviewtimeccms.Aggregate
               (TimeSpan.Zero,
               (sumSoFar, nextMyObject) => sumSoFar + nextMyObject);
            return totalSpan.ToString(@"hh\:mm\:ss");
        }
        [Authorize(Roles = "Admin, QAQC")]
        public async Task<ActionResult> ReviewTimeComparison(int patientId, int[] cyclesforreivew, int cycle = 0)
        {
            var patient = await _db.Patients.FindAsync(patientId);
            var phoneNumbers = new List<string>();

            if (!string.IsNullOrEmpty(patient?.MobilePhoneNumber))
                phoneNumbers.Add(patient.MobilePhoneNumber);
            if (!string.IsNullOrEmpty(patient?.HomePhoneNumber))
                phoneNumbers.Add(patient.HomePhoneNumber);
            if (!string.IsNullOrEmpty(patient?.WorkPhoneNumber))
                phoneNumbers.Add(patient.WorkPhoneNumber);
            if (!string.IsNullOrEmpty(patient?.EmergencyNumber))
                phoneNumbers.Add(patient.EmergencyNumber);

            var callHistories = new List<CallHistory>();
            var accountSid = ConfigurationManager.AppSettings["TwilioAccountSid"];
            var authToken = ConfigurationManager.AppSettings["TwilioAuthToken"];

            TwilioClient.Init(accountSid, authToken);

            foreach (var number in phoneNumbers)
            {
                var to = number.Length == 11 && number.Substring(0) == "1"
                       ? "+" + number : number.Length == 10
                       ? "+1" + number : number;

                var calls = await CallResource.ReadAsync(to: new PhoneNumber(to));
                if (!calls.Any())
                    continue;

                callHistories.AddRange(calls.Select(call => new CallHistory
                {
                    Duration = call?.StartTime != null && call.EndTime != null
                                                                         ? (TimeSpan)(call.EndTime - call.StartTime)
                                                                         : TimeSpan.Zero,
                })
                );


            }

            var totalReview = await _db.ReviewTimeCcms.AsNoTracking().Where(r => r.PatientId == patient.Id && r.Cycle > 0 && r.Activity != null && r.EndTime != null).ToListAsync();

            List<ReviewTimeCcm> copyTotalReviewTimeCcms = new List<ReviewTimeCcm>(totalReview);

            //int cycleId = 1;
            List<ReviewTimeCcm> cycleReviewTimeCcms = null;
            List<ReviewTimeCycle> reviewTimeCycles = new List<ReviewTimeCycle>();
            List<int> cycles = totalReview.Select(x => x.Cycle).Distinct().OrderBy(x => x).ToList();

            if (copyTotalReviewTimeCcms.Any())
            {
                DateTime cycleStartDate = copyTotalReviewTimeCcms.FirstOrDefault()?.StartTime ?? DateTime.MinValue;
                DateTime cycleEndDate = DateTime.MinValue;
                ReviewTimeCycle reviewTimeCycle = null;
                //int cycleIndex = 1;
                foreach (var cycleitemin in cycles)
                {
                    reviewTimeCycle = new ReviewTimeCycle();
                    reviewTimeCycle.CycleId = cycleitemin;
                    //reviewTimeCycle.CycleId = cycleId++;
                    reviewTimeCycle.StartDate = copyTotalReviewTimeCcms.Where(x => x.Cycle == cycleitemin).FirstOrDefault().StartTime;
                    reviewTimeCycle.EndDate = copyTotalReviewTimeCcms.Where(x => x.Cycle == cycleitemin).LastOrDefault().EndTime.Value;

                    cycleReviewTimeCcms = copyTotalReviewTimeCcms.FindAll(rv =>
                        rv.Cycle == cycleitemin);

                    reviewTimeCycle.ReviewTimeCycleCcms = cycleReviewTimeCcms;
                    reviewTimeCycle.ReviewTimes = new List<TimeSpan>(cycleReviewTimeCcms.Select(rt => rt.ReviewTime));
                    reviewTimeCycles.Add(reviewTimeCycle);
                }
                //if (cycleStartDate != DateTime.MinValue)
                //{
                //    do
                //    {
                //        reviewTimeCycle = new ReviewTimeCycle();
                //        reviewTimeCycle.CycleId = cycleId++;
                //        reviewTimeCycle.StartDate = cycleStartDate;
                //        reviewTimeCycle.EndDate = cycleStartDate.AddDays(30);

                //        cycleReviewTimeCcms = copyTotalReviewTimeCcms.FindAll(rv =>
                //            rv.StartTime.Date.Ticks >= reviewTimeCycle.StartDate.Date.Ticks && rv.StartTime.Date.Ticks <= reviewTimeCycle.EndDate.Date.Ticks);

                //        reviewTimeCycle.ReviewTimeCycleCcms = cycleReviewTimeCcms;
                //        reviewTimeCycle.ReviewTimes = new List<TimeSpan>(cycleReviewTimeCcms.Select(rt => rt.ReviewTime));
                //        reviewTimeCycles.Add(reviewTimeCycle);

                //        cycleStartDate = cycleStartDate.Date.AddDays(31);

                //    } while (cycleIndex++ <= 11);
                //}

                //var firstReviewTime = copyTotalReviewTimeCcms.FirstOrDefault()?.StartTime;
                //DateTime startReviewDateTime = firstReviewTime.HasValue ? firstReviewTime.Value : DateTime.MinValue;
                //if (startReviewDateTime != DateTime.MinValue)
                //{
                //    cycleReviewTimeCcms = copyTotalReviewTimeCcms.FindAll(rv =>
                //        rv.StartTime.Date.Ticks >= startReviewDateTime.Date.Ticks && rv.StartTime.Date.Ticks <= startReviewDateTime.Date.AddDays(30).Ticks);
                //    if (cycleReviewTimeCcms.Any())
                //    {


                //        int removeItems = copyTotalReviewTimeCcms.RemoveAll(rt => cycleReviewTimeCcms.Contains(rt));
                //    }


                //}

            }


            var reviewCycle1 = totalReview.Where(r => r.StartTime <= patient?.CcmClinicalSignOffDate).ToList();
            var reviewCycle2 = totalReview.Where(r => r.StartTime > patient?.CcmClinicalSignOffDate).ToList();
            //var reviews = cycle == 0 ? totalReview : reviewTimeCycles.Find(r => r.CycleId == cycle).ReviewTimeCycleCcms;
            var reviews = reviewTimeCycles.Count > 0 && cycle > 0 && reviewTimeCycles.Where(x => x.CycleId == cycle).Count() > 0 ? reviewTimeCycles.Find(r => r.CycleId == cycle).ReviewTimeCycleCcms.ToList() : totalReview;

            ViewBag.CurrentPage = cycle;
            ViewBag.TotalTimeSpent = reviews.Any()
                                   ? reviews.Aggregate(TimeSpan.Zero, (sum, review) => sum + review.ReviewTime)
                                   : TimeSpan.Zero;

            ViewBag.TotalCallTime = callHistories.Any() ? callHistories.Aggregate(TimeSpan.Zero, (sum, review) => sum + review.Duration) : TimeSpan.Zero;
            ViewBag.PatientName = patient?.FirstName + " " + patient?.LastName;
            ViewBag.PatientId = patientId;
            ViewBag.isLocked = false;
            //var alreadyaddedbilling = from bc in _db.BillingCycles
            //                          join bcd in _db.BillingCycleDetails on bc.Id equals bcd.BillingCycleId into x 
            //                          from y in x.DefaultIfEmpty()
            //                          join bcc in _db.BillingCycleComments on bc.Id equals bcc.BillingCycleId into x1
            //                          from y1 in x1.DefaultIfEmpty()
            //                          where cyclesforreivew.Contains(bc.Cycle)
            //                          select new { BillingCycle = bc, BillingCycleDetails = y, BillingCycleComments = y1 } ;
            List<BillingCycle> alreadyaddedbilling = new List<BillingCycle>();
            try
            {
                alreadyaddedbilling = _db.BillingCycles.Include("BillingCycleDetails").Include("BillingCycleComments").Where(x => x.PatientId == patient.Id && cyclesforreivew.Contains(x.Cycle)).ToList();
            }
            catch (Exception ex)
            {
                throw ex;

            }
            ReviewTimeViewModel resultsnew;
            try
            {




                resultsnew = new ReviewTimeViewModel
                {
                    Cycle = patient?.Cycle,
                    ReviewTimeCcm = reviews,
                    PatientId = patientId,
                    BillingCode1 = patient?.CcmBillingCode,
                    BillingCode2 = patient?.CcmBillingCode2,
                    TotalReviewTime = totalReview.Any() ? totalReview.Aggregate(TimeSpan.Zero, (sum, review) => sum + review.ReviewTime) : TimeSpan.Zero,
                    ReviewTimeCycle1 = reviewCycle1.Any() ? reviewCycle1.Aggregate(TimeSpan.Zero, (sum, review) => sum + review.ReviewTime) : TimeSpan.Zero,
                    ReviewTimeCycle2 = reviewCycle2.Any() ? reviewCycle2.Aggregate(TimeSpan.Zero, (sum, review) => sum + review.ReviewTime) : TimeSpan.Zero,
                    ReviewTimeCycles = reviewTimeCycles,
                    billingCycles = alreadyaddedbilling


                };
                resultsnew.ReviewTimeCycles = resultsnew.ReviewTimeCycles.Where(x => cyclesforreivew.Contains(x.CycleId)).ToList();
                return PartialView("CycleComparison", resultsnew);
            }
            catch /*(Exception ex)*/
            {

                return PartialView("CycleComparison", new ReviewTimeViewModel());
            }


        }
        public string Addcommentsforbilling(string comments, int BillingCycleID)
        {
            BillingCycleComments billingCycleComments = new BillingCycleComments();
            billingCycleComments.Status = "Pending";
            billingCycleComments.Comments = comments;
            billingCycleComments.BillingCycleId = BillingCycleID;
            billingCycleComments.CreatedBy = User.Identity.GetUserId();
            billingCycleComments.CreatedOn = DateTime.Now;
            _db.BillingCycleComments.Add(billingCycleComments);
            _db.SaveChanges();
            return "True";

        }

        [Authorize(Roles = "Admin, QAQC")]
        public string Addreviewtimesforbilling(int patientId, int cycle, int[] RecordingIDs, string BillingCode1, string BillingCode2, bool isUpdate, string[] timeSpans, string FeedBack, string[] BillingCodes, int BillingCategoryId)
        {
            try
            {
                var patient = _db.Patients.Where(item => item.Id == patientId).FirstOrDefault();

                var check = _db.BillingCodes.Where(p => p.BillingCategoryId == BillingCategoryId).SelectMany(p => p.BillingCycles.Where(x => x.PatientId == patientId && x.Cycle == cycle)).ToList();
                //var check2 = _db.BillingCycles.Where(p=>p.PatientId==patientId).SelectMany(
                //    p => p.BillingCodes, (p, c) => new { p, c })
                //    .Where(f => f.c.BillingCategoryId == BillingCategoryId)
                //    .Select(cd => new
                //    {
                //        billingcycle = cd.p
                //    }

                //    );




                if (patient.LiaisonId == null)
                {
                    return "Liaison is not Assigned to this patient.";
                }
                var reviewtimeccms = _db.ReviewTimeCcms.AsNoTracking().Where(x => RecordingIDs.Contains(x.Id)).Select(x => x.ReviewTime).ToList();
                //List<TimeSpan> newtimespans = new List<TimeSpan>();

                //foreach (var item in timeSpans)
                //{
                //    DateTime dt;
                //    if (!DateTime.TryParseExact(item, "hh:mm:ss", CultureInfo.InvariantCulture,
                //                                                  DateTimeStyles.None, out dt))
                //    {
                //        // handle validation error
                //    }
                //    TimeSpan time = dt.TimeOfDay;
                //    newtimespans.Add(time);
                //}
                var totalSpan = reviewtimeccms.Aggregate
                   (TimeSpan.Zero,
                   (sumSoFar, nextMyObject) => sumSoFar + nextMyObject);
                //if (totalSpan.Hours == 0 && totalSpan.Minutes < 20)
                //{
                //    return "Total Time for selected review times is  less than 20 minutes therefore it is not eligibile for billing.";
                //}




                //if (BillingCode1 == "CPT99490" && totalSpan.Hours == 0 && totalSpan.Minutes >= 30)
                //{
                //    return "CPT Code is not valid for total time.It should be greater or equal to 20 min and less than 30 min.";
                //}
                //if (BillingCode1 == "CPT99491" && totalSpan.Hours == 0 && totalSpan.Minutes < 30)
                //{
                //    return "CPT Code is not valid for total time.It should be greater or equal to 30 min and less than 60 min";
                //}
                //if (BillingCode1 == "CPT99491" && totalSpan.Hours >= 1)
                //{
                //    return "CPT Code is not valid for total time.It should be greater or equal to 60 min.";
                //}

                //var LiaisonCPTRatesexists = _db.Liaison_CPTRates.Where(x => x.LiaisonId == patient.LiaisonId).ToList();
                //if (LiaisonCPTRatesexists.Count == 0)
                //{
                //    return "Counsler salary rates are not defined. Please update the salary rates first.";
                //}
                if (BillingCategoryId != BillingCodeHelper.G0506BillingCatagoryid)
                {
                    var finalcareplan = _db.FinalCarePlanNotes.Where(x => x.PatientId == patientId && x.Cycle == cycle).FirstOrDefault();
                    if (finalcareplan == null)
                    {
                        return "Cannot submit this cycle for billing until final care plan not generated.";
                    }

                }
                var alreadyexistsdata = new BillingCycle();
                if (check.Count > 0)
                {
                    alreadyexistsdata = _db.BillingCycles.Where(x => x.PatientId == patientId && x.Cycle == cycle).FirstOrDefault();
                }
                else
                {
                    alreadyexistsdata = null;
                }


                //var LiaisonCPTRates = _db.Liaison_CPTRates.Where(x => x.LiaisonId == patient.LiaisonId).ToList();

                decimal BillingCode1Rate = 0;
                decimal BillingCode2Rate = 0;

                decimal BillingCode1PhyInvoice = 0;
                decimal BillingCode2PhyInvoice = 0;

                decimal BillingCode1PhyBill = 0;
                decimal BillingCode2PhyBill = 0;
                //if (LiaisonCPTRates.Count > 0)
                //{
                //    BillingCode1Rate = LiaisonCPTRates.Where(x => x.BillingCode == BillingCode1).FirstOrDefault().SalaryRate.Value;


                //    if (BillingCode2 != "")
                //    {
                //        BillingCode2Rate = LiaisonCPTRates.Where(x => x.BillingCode == BillingCode2).FirstOrDefault().SalaryRate.Value;

                //    }
                //}

                if (isUpdate == false && alreadyexistsdata == null)
                {
                    BillingCycle billingCycle = new BillingCycle();
                    billingCycle.PatientId = patientId;
                    billingCycle.Cycle = cycle;
                    billingCycle.BillingCode1 = BillingCode1;
                    billingCycle.BillingCode2 = BillingCode2;
                    billingCycle.BillingCode1Rate = BillingCode1Rate;
                    billingCycle.BillingCode2Rate = BillingCode2Rate;
                    //physicianrates
                    billingCycle.BillingCode1PhysicianRateBilling = BillingCode1PhyBill;
                    billingCycle.BillingCode1PhysicianRateInvoice = BillingCode1PhyInvoice;
                    billingCycle.BillingCode2PhysicianRateBilling = BillingCode2PhyBill;
                    billingCycle.BillingCode2PhysicianRateInvoice = BillingCode2PhyInvoice;
                    //
                    billingCycle.CreatedOn = DateTime.Now;
                    billingCycle.CreatedBy = User.Identity.GetUserId();
                    billingCycle.BillingCodes = new List<Models.CCMBILLINGS.BillingCodes>();
                    billingCycle.Liaison_CPTRates = new List<Liaison_CPTRates>();
                    billingCycle.Physician_CPTRates = new List<Physician_CPTRates>();
                    foreach (var item in BillingCodes)
                    {
                        int BillingId = Convert.ToInt32(item);
                        var billingcode = _db.BillingCodes.Where(p => p.Id == BillingId).Select(p => p).FirstOrDefault();

                        billingCycle.BillingCodes.Add(billingcode);

                        var laisonCPT = _db.Liaison_CPTRates.Where(p => p.LiaisonId == patient.LiaisonId && p.BillingCodeId == BillingId).Select(p => p).FirstOrDefault();
                        if (laisonCPT == null)
                        {
                            return "Counsler salary rates are not defined. Please update the salary rates first.";
                        }
                        if (laisonCPT.SalaryRate == null)
                        {
                            return "Counsler salary rates are not defined. Please update the salary rates first.";
                        }
                        billingCycle.Liaison_CPTRates.Add(laisonCPT);

                        var PhysicianCPT = _db.Physician_CPTRates.Where(p => p.PhysicianId == patient.PhysicianId && p.BillingCodeId == BillingId).Select(p => p).FirstOrDefault();
                        if (PhysicianCPT == null)
                        {
                            return "Physician salary rates are not defined. Please update the salary rates first.";
                        }
                        if (PhysicianCPT.BillingRate == null)
                        {
                            return "Physician salary rates are not defined. Please update the salary rates first.";
                        }
                        if (PhysicianCPT.InvoiceRate == null)
                        {
                            return "Physician salary rates are not defined. Please update the salary rates first.";
                        }
                        billingCycle.Physician_CPTRates.Add(PhysicianCPT);


                    }
                    _db.BillingCycles.Add(billingCycle);
                    _db.SaveChanges();

                    foreach (var recordingid in RecordingIDs)
                    {
                        var review = _db.ReviewTimeCcms.Where(p => p.Id == recordingid).FirstOrDefault();
                        review.IsLocked = true;
                        _db.Entry(review).State = EntityState.Modified;
                        _db.SaveChanges();
                        BillingCycleDetails billingCycleDetails = new BillingCycleDetails();
                        billingCycleDetails.BillingCycleId = billingCycle.Id;
                        billingCycleDetails.RecordingID = recordingid;
                        billingCycleDetails.isDeleted = false;
                        billingCycleDetails.CreatedBy = User.Identity.GetUserId();
                        billingCycleDetails.CreatedOn = DateTime.Now;
                        _db.BillingCycleDetails.Add(billingCycleDetails);

                    }
                    _db.SaveChanges();
                    if (BillingCategoryId == BillingCodeHelper.cmmBillingCatagoryid)
                    {

                        HelperExtensions.UpdateCCMCycleStatus(patientId, cycle, "Claims Submission", User.Identity.GetUserId(), FeedBack);
                    }
                    else
                    {
                        HelperExtensions.UpdatecategoryStatus(BillingCategoryId, patientId, cycle, "Claims Submission", User.Identity.GetUserId(), FeedBack);

                    }

                    return "True";
                }
                else
                {

                    alreadyexistsdata.PatientId = patientId;
                    alreadyexistsdata.Cycle = cycle;
                    alreadyexistsdata.BillingCode1 = BillingCode1;
                    alreadyexistsdata.BillingCode2 = BillingCode2;
                    alreadyexistsdata.BillingCode1Rate = BillingCode1Rate;
                    alreadyexistsdata.BillingCode2Rate = BillingCode2Rate;
                    //
                    alreadyexistsdata.BillingCode1PhysicianRateBilling = BillingCode1PhyBill;
                    alreadyexistsdata.BillingCode1PhysicianRateInvoice = BillingCode1PhyInvoice;
                    alreadyexistsdata.BillingCode2PhysicianRateBilling = BillingCode2PhyBill;
                    alreadyexistsdata.BillingCode2PhysicianRateInvoice = BillingCode2PhyInvoice;
                    //
                    alreadyexistsdata.UpdatedOn = DateTime.Now;
                    alreadyexistsdata.UpdatedBy = User.Identity.GetUserId();
                    foreach (var item in BillingCodes)
                    {
                        int BillingId = Convert.ToInt32(item);
                        var billingcode = _db.BillingCodes.Where(p => p.Id == BillingId).Select(p => p).FirstOrDefault();

                        alreadyexistsdata.BillingCodes.Add(billingcode);

                        var laisonCPT = _db.Liaison_CPTRates.Where(p => p.LiaisonId == patient.LiaisonId && p.BillingCodeId == BillingId).Select(p => p).FirstOrDefault();
                        if (laisonCPT == null)
                        {
                            return "Counsler salary rates are not defined. Please update the salary rates first.";
                        }
                        if (laisonCPT.SalaryRate == null)
                        {
                            return "Counsler salary rates are not defined. Please update the salary rates first.";
                        }
                        alreadyexistsdata.Liaison_CPTRates.Add(laisonCPT);



                        var PhysicianCPT = _db.Physician_CPTRates.Where(p => p.PhysicianId == patient.PhysicianId && p.BillingCodeId == BillingId).Select(p => p).FirstOrDefault();
                        if (PhysicianCPT == null)
                        {
                            return "Physician salary rates are not defined. Please update the salary rates first.";
                        }
                        if (PhysicianCPT.BillingRate == null)
                        {
                            return "Physician salary rates are not defined. Please update the salary rates first.";
                        }
                        if (PhysicianCPT.InvoiceRate == null)
                        {
                            return "Physician salary rates are not defined. Please update the salary rates first.";
                        }



                        alreadyexistsdata.Physician_CPTRates.Add(PhysicianCPT);


                    }
                    _db.Entry(alreadyexistsdata).State = EntityState.Modified;
                    _db.SaveChanges();
                    var alreadybillingcycledetails = _db.BillingCycleDetails.Where(x => x.BillingCycleId == alreadyexistsdata.Id).ToList();
                    foreach (var billingcycledetail in alreadybillingcycledetails)
                    {
                        billingcycledetail.isDeleted = true;
                        _db.Entry(billingcycledetail).State = EntityState.Modified;
                    }
                    foreach (var recordingid in RecordingIDs)
                    {
                        var review = _db.ReviewTimeCcms.Where(p => p.Id == recordingid).FirstOrDefault();
                        review.IsLocked = true;
                        _db.Entry(review).State = EntityState.Modified;
                        _db.SaveChanges();
                        BillingCycleDetails billingCycleDetails = new BillingCycleDetails();
                        billingCycleDetails.BillingCycleId = alreadyexistsdata.Id;
                        billingCycleDetails.RecordingID = recordingid;
                        billingCycleDetails.isDeleted = false;
                        billingCycleDetails.CreatedBy = User.Identity.GetUserId();
                        billingCycleDetails.CreatedOn = DateTime.Now;
                        _db.BillingCycleDetails.Add(billingCycleDetails);

                    }
                    _db.SaveChanges();

                    if (BillingCategoryId == BillingCodeHelper.cmmBillingCatagoryid)
                    {

                        HelperExtensions.UpdateCCMCycleStatus(patientId, cycle, "Claims Submission", User.Identity.GetUserId(), FeedBack);
                    }
                    else
                    {
                        HelperExtensions.UpdatecategoryStatus(BillingCategoryId, patientId, cycle, "Claims Submission", User.Identity.GetUserId(), FeedBack);

                    }
                    return "True";
                }

            }
            catch (Exception ex)
            {
                return "False" + ex.Message;

            }

        }
        public PartialViewResult _CcmActivityLinks(int? patientId, int cycleId = 0)
        {
            ViewBag.CycleId = cycleId;
            return PartialView(_db.Patients.Find(patientId));
        }


        public JsonResult ReviewActivities(string patientId, string cycle, int BillingCategoryId)
        {
            //List<int> reviews = new List<int>();
            try
            {
                int? pId = Convert.ToInt32(patientId);
                int? rcycle = Convert.ToInt32(cycle);
                if (pId != null && rcycle != null)
                {

                    var reviews = _db.ReviewTimeCcms.Where(p => p.PatientId == pId && p.Cycle == rcycle && p.Activity != null & p.IsLocked == true).Select(p => new { id = p.Id, billingid = p.BillingcategoryId }).ToList();
                    return Json(reviews, JsonRequestBehavior.AllowGet);
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }


            return Json(null, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ReviewTimeRollUp(string patientId, string cycle, int[] RecordingIDs2, string Billingcategoryid, string rollupfrom)
        {
            try
            {


                int? pId = Convert.ToInt32(patientId);
                int? cycleid = Convert.ToInt32(cycle);
                int? billingcategoryid = Convert.ToInt32(Billingcategoryid);
                string BillingCatname = _db.BillingCategories.Where(p => p.BillingCategoryId == billingcategoryid).Select(p => p.Name).FirstOrDefault();
                foreach (var item in RecordingIDs2)
                {

                    var reviewrollup = _db.ReviewTimeCcms.Where(p => p.Id == item).Select(p => p).FirstOrDefault();
                    if (billingcategoryid != null)
                    {
                        reviewrollup.BillingcategoryId = billingcategoryid;
                        reviewrollup.RollupFrom = rollupfrom;
                        reviewrollup.RolledupTo = BillingCatname;
                        _db.Entry(reviewrollup).State = EntityState.Modified;
                        _db.SaveChanges();

                    }

                }


                return Json("success", JsonRequestBehavior.AllowGet);
            }
            catch/*(Exception ex)*/
            {
                return Json("error", JsonRequestBehavior.AllowGet);
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