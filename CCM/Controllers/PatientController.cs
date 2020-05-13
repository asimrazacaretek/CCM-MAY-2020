using CCM.Helpers;
using CCM.Models;
using CCM.Models.BulkChanges;
using CCM.Models.Caching;
using CCM.Models.CCMBILLINGS;
using CCM.Models.CCMBILLINGS.ViewModels;
using CCM.Models.DataModels;
using CCM.Models.ViewModels;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Linq.Dynamic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using System.Web.Caching;
using System.Web.Mvc;

namespace CCM.Controllers
{
    [Authorize(Roles = "Liaison, Physician, PhysiciansGroup, Admin, QAQC, LiaisonGroup, Sales")]
    public class PatientController : BaseController
    {
        //private readonly ApplicationdbContect _db = new ApplicationdbContect();


        private ApplicationUserManager _userManager;

        public PatientController()
        {

        }

        public PatientController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            _db.Configuration.ProxyCreationEnabled = false;
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

        [HttpPost]
        public ActionResult PatientNo(int? patientId)
        {
            Session["PatientNo"] = patientId;

            return RedirectToAction("AssignLiaison", "EnrollmentStatus", new { patientId = Session["PatientNo"] });
        }


        public async Task<ActionResult> Index(string userId)
        {
            return RedirectToAction("TotalPatients", new { userId = userId });
            //ViewBag.EnrollmentStauses = _db.EnrollmentStatuss.ToList();
            //ViewBag.EnrollmentSubStatuses = _db.EnrollmentSubStatuss.ToList();
            //var user = string.IsNullOrEmpty(userId)
            //                 ? _db.Users.Find(GetUserId())
            //                 : _db.Users.Find(userId);
            //ViewBag.UserId = user.Id;
            //ViewBag.Status = "Total Patients";
            //ViewBag.Owner = user.Role == "Liaison" || user.Role == "PhysiciansGroup" ? user.FirstName
            //                 : user.Role == "Physician" ? "Dr. " + user.LastName
            //                 : "Admin";
            //ViewBag.Liaisons = new SelectList(_db.Liaisons.Select(l => new SelectListItem
            //{
            //    Value = l.Id.ToString(),
            //    Text = l.FirstName + " " + l.LastName
            //}).OrderBy(l => l.Text), "Value", "Text");


            //return View(user.Role == "Liaison"
            //           ? await _db.Patients.Where(p => p.LiaisonId == user.CCMid).Include(p => p.Liaison).Include(p => p.Physician).Select(PatientListItem.Projection).ToListAsync()
            //           : user.Role == "Physician"
            //           ? await _db.Patients.Where(p => p.PhysicianId == user.CCMid).Include(p => p.Liaison).Include(p => p.Physician).Select(PatientListItem.Projection).ToListAsync()
            //           : user.Role == "PhysiciansGroup"
            //           ? await _db.Patients.Where(p => p.Physician.MainPhoneNumber == user.PhoneNumber).Include(p => p.Liaison).Include(p => p.Physician).Select(PatientListItem.Projection).ToListAsync()
            //           : await _db.Patients.Include(p => p.Liaison).Include(p => p.Physician).Select(PatientListItem.Projection).ToListAsync());

        }
        public ActionResult PatientSurveys(string userId, string status = "", string substatus = "", DateTime? date = null)
        {
            //


            //


            var user = string.IsNullOrEmpty(userId)
                ? _db.Users.Find(GetUserId())
                : _db.Users.Find(userId);
            var user1 =
               _db.Users.Find(GetUserId());

            //ViewBag.UserId = user.Id;
            ViewBag.Status = "Total Patients";
            ViewBag.Owner = user1.Role == "Liaison" || user1.Role == "PhysiciansGroup" ? user1.FirstName
                : user1.Role == "Physician" ? "Dr. " + user1.LastName
                : "Admin";


            ViewBag.UserId = userId;
            ViewBag.Status = date != null ? "Calls Due (" + (date == DateTime.Today ? "Today" : "Tomorrow") + ")"
                           : string.IsNullOrEmpty(status) ? "Unknown"
                           : status;
            ViewBag.StatusStr = status;
            ViewBag.SubStatus = substatus == null ? "" : substatus;
            ViewBag.DateStr = date;

            ViewBag.CCMID = user.Role == "Liaison" || user.Role == "PhysiciansGroup" ? user.CCMid.Value
                          : user.Role == "Physician" ? user.CCMid.Value
                          : 0;
            ViewBag.UserRole = user.Role;
            ViewBag.Liaisons = new SelectList(_db.Liaisons.Select(l => new SelectListItem
            {
                Value = l.Id.ToString(),
                Text = l.FirstName + " " + l.LastName
            }).OrderBy(l => l.Text), "Value", "Text");
            var liaisons = _db.Liaisons.AsNoTracking().Select(p => new SelectListItem
            {
                Value = p.Id.ToString(),
                Text = p.FirstName + " " + p.LastName
            });

            var physicians = _db.Physicians.AsNoTracking().Select(p => new SelectListItem
            {
                Value = p.Id.ToString(),
                Text = p.FirstName + " " + p.LastName
            });

            var physiciansGroups = _db.PhysiciansGroup.AsNoTracking().Select(p => new SelectListItem
            {
                Value = p.Id.ToString(),
                Text = p.GroupName

            });
            if (User.IsInRole("PhysiciansGroup"))
            {


                List<int> physicianids = new List<int>();
                physicianids = _db.physicianGroup_Physician_Mappings.AsNoTracking().Where(x => x.PhysiciansGroupId == user1.CCMid).Select(x => x.PhysicianId).ToList();
                var group = _db.Users.Find(GetUserId());
                physicians = _db.Physicians.AsNoTracking().Where(p => physicianids.Contains(p.Id))
                                                             .Select(p => new SelectListItem
                                                             {
                                                                 Value = p.Id.ToString(),
                                                                 Text = p.FirstName + " " + p.LastName
                                                             });
                var liasionids = _db.Patients.AsNoTracking().Where(x => physicianids.Contains(x.PhysicianId.Value) && x.LiaisonId != null).Select(x => x.LiaisonId).Distinct().ToList();
                liaisons = _db.Liaisons.AsNoTracking().Where(p => liasionids.Contains(p.Id)).Select(p => new SelectListItem
                {
                    Value = p.Id.ToString(),
                    Text = p.FirstName + " " + p.LastName
                });

            }
            if (User.IsInRole("LiaisonGroup"))
            {


                List<int> physicianids = new List<int>();
                var liasionids = _db.LiaisonGroup_Liaison_Mappings.AsNoTracking().Where(x => x.LiaisonGroupId == user1.CCMid).Select(x => x.LiaisonId).ToList();
                var group = _db.Users.Find(GetUserId());
                liaisons = _db.Liaisons.AsNoTracking().Where(p => liasionids.Contains(p.Id)).Select(p => new SelectListItem
                {
                    Value = p.Id.ToString(),
                    Text = p.FirstName + " " + p.LastName
                });
                physicianids = _db.Patients.AsNoTracking().Where(x => liasionids.Contains(x.LiaisonId.Value) && x.LiaisonId != null).Select(x => x.PhysicianId.Value).Distinct().ToList();
                physicians = _db.Physicians.AsNoTracking().Where(p => physicianids.Contains(p.Id))
                                                             .Select(p => new SelectListItem
                                                             {
                                                                 Value = p.Id.ToString(),
                                                                 Text = p.FirstName + " " + p.LastName
                                                             });


                //ViewBag.Liaisons = new SelectList(liaisons.OrderBy(l => l.Text), "Value", "Text");
                //ViewBag.Physicians = new SelectList(physicians, "Value", "Text");
                //return View();
            }
            try
            {
                ViewBag.Liaisons = new SelectList(liaisons.OrderBy(l => l.Text), "Value", "Text", user.Role == "Liaison" ? user.CCMid.Value.ToString() : "");
                ViewBag.Physicians = new SelectList(physicians.OrderBy(p => p.Text), "Value", "Text", user.Role == "Physician" ? user.CCMid.Value.ToString() : "");
                ViewBag.physiciansGroups = new SelectList(physiciansGroups.OrderBy(p => p.Text), "Value", "Text", user.Role == "PhysiciansGroup" ? user.CCMid.Value.ToString() : "");
            }
            catch /*(Exception ex)*/
            {
                ViewBag.Liaisons = new SelectList(liaisons.OrderBy(l => l.Text), "Value", "Text");
                ViewBag.Physicians = new SelectList(physicians.OrderBy(p => p.Text), "Value", "Text");
                ViewBag.physiciansGroups = new SelectList(physiciansGroups.OrderBy(p => p.Text), "Value", "Text");

            }

            return View();
        }

       

        public ActionResult TotalPatients(string userId, string status = "", string substatus = "", DateTime? date = null, string HideDataforCallReceive = "No")
        {

            ViewBag.CurrentUserId = GetUserId();
            int dataexpiretime = 5; //in minuts
            ViewBag.HideDataForCallReceive = HideDataforCallReceive;
            ViewBag.userID_translator = GetUserId();

            var enrolmentstatus = HttpContextCaching.GetCachData("enrolmentstatus") as List<EnrollmentStatus>;
            if (enrolmentstatus == null)
            {
                enrolmentstatus = _db.EnrollmentStatuss.ToList();
                HttpContextCaching.AddDataToCache("enrolmentstatus", enrolmentstatus, dataexpiretime, Cache.NoSlidingExpiration);
            }
            ViewBag.EnrollmentStatus = new SelectList(enrolmentstatus, "Id", "Name");
            var EnrollmentSubStatuss = HttpContextCaching.GetCachData("EnrollmentSubStatuss") as List<EnrollmentSubStatus>;
            if (EnrollmentSubStatuss == null)
            {
                EnrollmentSubStatuss = _db.EnrollmentSubStatuss.ToList();
                HttpContextCaching.AddDataToCache("EnrollmentSubStatuss", EnrollmentSubStatuss, dataexpiretime, Cache.NoSlidingExpiration);
            }
            ViewBag.EnrollmentSubStatus = new SelectList(EnrollmentSubStatuss, "EnrollmentStatusID", "Name");

            var EnrollmentSubstatusReasons = HttpContextCaching.GetCachData("EnrollmentSubstatusReasons") as List<EnrollmentSubstatusReason>;
            if (EnrollmentSubstatusReasons == null)
            {
                EnrollmentSubstatusReasons = _db.EnrollmentSubstatusReasons.ToList();
                HttpContextCaching.AddDataToCache("EnrollmentSubstatusReasons", EnrollmentSubstatusReasons, dataexpiretime, Cache.NoSlidingExpiration);
            }
            ViewBag.EnrollemntStatusReson = new SelectList(EnrollmentSubstatusReasons, "Name", "Name");


            ViewBag.EnrollmentStauses = enrolmentstatus;

            ViewBag.EnrollmentSubStatuses = EnrollmentSubStatuss;

            var user = string.IsNullOrEmpty(userId) ? _db.Users.Find(GetUserId()) : _db.Users.Find(userId);
            var user1 = _db.Users.Find(GetUserId());

            //ViewBag.UserId = user.Id;
            ViewBag.Status = "Total Patients";
            ViewBag.Owner = user1.Role == "Liaison" || user1.Role == "PhysiciansGroup" || user1.Role == "Sales" ? user1.FirstName
                : user1.Role == "Physician" ? "Dr. " + user1.LastName
                : "Admin";


            ViewBag.UserId = userId;
            ViewBag.Status = date != null ? "Calls Due (" + (date == DateTime.Today ? "Today" : "Tomorrow") + ")"
                           : string.IsNullOrEmpty(status) ? "Unknown"
                           : status;
            ViewBag.StatusStr = status;
            ViewBag.SubStatus = substatus == null ? "" : substatus;
            ViewBag.DateStr = date;

            ViewBag.CCMID = user.Role == "Liaison" || user.Role == "PhysiciansGroup" || user.Role == "Sales" ? user.CCMid.Value
                          : user.Role == "Physician" ? user.CCMid.Value
                          : 0;
            ViewBag.UserIDfortranslator = user.Id;
            ViewBag.UserRole = user.Role;
            //ViewBag.Liaisons = new SelectList(_db.Liaisons.Where(x => x.isTranslator == false).Select(l => new SelectListItem
            //{
            //    Value = l.Id.ToString(),
            //    Text = l.FirstName + " " + l.LastName
            //}).OrderBy(l => l.Text), "Value", "Text");
            //var liaisons = _db.Liaisons.AsNoTracking().Select(p => new SelectListItem
            //{
            //    Value = p.Id.ToString(),
            //    Text = p.FirstName + " " + p.LastName
            //});
            List<SelectListItem> preliaisons = new List<SelectListItem>();

            preliaisons.Add(new SelectListItem() { Text = "Do Nothing", Value = "0" });
            preliaisons.Add(new SelectListItem() { Text = "Remove Counselor", Value = null });
            preliaisons.AddRange(_db.Liaisons.Where(x => x.IsTranslator == false && x.isActive == true).Select(p => new SelectListItem
            {
                Value = p.Id.ToString(),
                Text = p.FirstName + " " + p.LastName
            }).OrderBy(n => n.Text).ToList());

            List<SelectListItem> pretranslator = new List<SelectListItem>();
            pretranslator.Add(new SelectListItem() { Text = "Do Nothing", Value = "0" });
            pretranslator.Add(new SelectListItem() { Text = "Remove Translator", Value = null });
            pretranslator.AddRange(_db.Liaisons.Where(x => x.IsTranslator == true && x.isActive == true).Select(p => new SelectListItem
            {
                Value = p.Id.ToString(),
                Text = p.FirstName + " " + p.LastName
            }).OrderBy(n => n.Text).ToList());

            ViewBag.preliaisons = new SelectList(preliaisons, "Value", "Text");
            ViewBag.pretranslator = new SelectList(pretranslator, "Value", "Text");
            var liaisons = _db.Liaisons.Where(x => x.IsTranslator == false && x.isActive == true).Select(p => new SelectListItem
            {
                Value = p.Id.ToString(),
                Text = p.FirstName + " " + p.LastName
            });

            var translator = _db.Liaisons.Where(x => x.IsTranslator == true && x.isActive == true).Select(p => new SelectListItem
            {
                Value = p.Id.ToString(),
                Text = p.FirstName + " " + p.LastName
            });

            ViewBag.liaisonsAssign = new SelectList(liaisons, "Value", "Text").OrderBy("Text");
            ViewBag.translatorAssign = new SelectList(translator, "Value", "Text").OrderBy("Text");

            var physicians = _db.Physicians.AsNoTracking().Select(p => new SelectListItem
            {
                Value = p.Id.ToString(),
                Text = p.FirstName + " " + p.LastName
            });

            var physiciansGroups = _db.PhysiciansGroup.AsNoTracking().Select(p => new SelectListItem
            {
                Value = p.Id.ToString(),
                Text = p.GroupName

            });
            if (User.IsInRole("PhysiciansGroup"))
            {


                List<int> physicianids = new List<int>();
                physicianids = _db.physicianGroup_Physician_Mappings.AsNoTracking().Where(x => x.PhysiciansGroupId == user1.CCMid).Select(x => x.PhysicianId).ToList();
                var group = _db.Users.Find(GetUserId());
                physicians = _db.Physicians.AsNoTracking().Where(p => physicianids.Contains(p.Id))
                                                             .Select(p => new SelectListItem
                                                             {
                                                                 Value = p.Id.ToString(),
                                                                 Text = p.FirstName + " " + p.LastName
                                                             });
                var liasionids = _db.Patients.AsNoTracking().Where(x => physicianids.Contains(x.PhysicianId.Value) && x.LiaisonId != null).Select(x => x.LiaisonId).Distinct().ToList();
                liaisons = _db.Liaisons.AsNoTracking().Where(p => liasionids.Contains(p.Id) && p.IsTranslator == false).Select(p => new SelectListItem
                {
                    Value = p.Id.ToString(),
                    Text = p.FirstName + " " + p.LastName
                });

                var translatorids = _db.Patients.AsNoTracking().Where(x => physicianids.Contains(x.PhysicianId.Value) && x.TranslatorId != null).Select(x => x.TranslatorId).Distinct().ToList();
                translator = _db.Liaisons.AsNoTracking().Where(p => translatorids.Contains(p.Id) && p.IsTranslator == true).Select(p => new SelectListItem
                {
                    Value = p.Id.ToString(),
                    Text = p.FirstName + " " + p.LastName
                });
                physiciansGroups = _db.PhysiciansGroup.AsNoTracking().Where(x => x.Id == user.CCMid).Select(p => new SelectListItem
                {
                    Value = p.Id.ToString(),
                    Text = p.GroupName

                });

            }
            if (User.IsInRole("Sales"))
            {


                List<int> physicianids = new List<int>();
                var physiciangrpids = _db.physicianGroup_SalesStaff_Mappings.AsNoTracking().Where(x => x.SaleStaffId == user.CCMid).Select(x => x.PhysiciansGroupId).ToList();
                physicianids = _db.physicianGroup_Physician_Mappings.AsNoTracking().Where(x => physiciangrpids.Contains(x.PhysiciansGroupId)).Select(x => x.PhysicianId).ToList();
                var group = _db.Users.Find(GetUserId());
                physicians = _db.Physicians.AsNoTracking().Where(p => physicianids.Contains(p.Id))
                                                             .Select(p => new SelectListItem
                                                             {
                                                                 Value = p.Id.ToString(),
                                                                 Text = p.FirstName + " " + p.LastName
                                                             });
                //var liasionids = _db.Patients.AsNoTracking().Where(x => physicianids.Contains(x.PhysicianId.Value) && x.LiaisonId != null).Select(x => x.LiaisonId).Distinct().ToList();
                //liaisons = _db.Liaisons.AsNoTracking().Where(p => liasionids.Contains(p.Id) && p.IsTranslator == false).Select(p => new SelectListItem
                //{
                //    Value = p.Id.ToString(),
                //    Text = p.FirstName + " " + p.LastName
                //});

                //  var translatorids = _db.Patients.AsNoTracking().Where(x => physicianids.Contains(x.PhysicianId.Value) && x.TranslatorId != null).Select(x => x.TranslatorId).Distinct().ToList();
                //translator = _db.Liaisons.AsNoTracking().Where(p => translatorids.Contains(p.Id) && p.IsTranslator == true).Select(p => new SelectListItem
                //  {
                //      Value = p.Id.ToString(),
                //      Text = p.FirstName + " " + p.LastName
                //  });
                physiciansGroups = _db.PhysiciansGroup.AsNoTracking().Where(x => physiciangrpids.Contains(x.Id)).Select(p => new SelectListItem
                {
                    Value = p.Id.ToString(),
                    Text = p.GroupName

                });


            }
            if (User.IsInRole("LiaisonGroup"))
            {
                List<int> physicianids = new List<int>();
                var liasionids = _db.LiaisonGroup_Liaison_Mappings.AsNoTracking().Where(x => x.LiaisonGroupId == user1.CCMid).Select(x => x.LiaisonId).ToList();
                var group = _db.Users.Find(GetUserId());
                liaisons = _db.Liaisons.AsNoTracking().Where(p => liasionids.Contains(p.Id) && p.IsTranslator == false).Select(p => new SelectListItem
                {
                    Value = p.Id.ToString(),
                    Text = p.FirstName + " " + p.LastName
                });
                translator = _db.Liaisons.AsNoTracking().Where(p => liasionids.Contains(p.Id) && p.IsTranslator == true).Select(p => new SelectListItem
                {
                    Value = p.Id.ToString(),
                    Text = p.FirstName + " " + p.LastName
                });
                physicianids = _db.Patients.AsNoTracking().Where(x => liasionids.Contains(x.LiaisonId.Value) && x.LiaisonId != null).Select(x => x.PhysicianId.Value).Distinct().ToList();
                physicians = _db.Physicians.AsNoTracking().Where(p => physicianids.Contains(p.Id))
                .Select(p => new SelectListItem
                {
                    Value = p.Id.ToString(),
                    Text = p.FirstName + " " + p.LastName
                });

                //ViewBag.Liaisons = new SelectList(liaisons.OrderBy(l => l.Text), "Value", "Text");
                //ViewBag.Physicians = new SelectList(physicians, "Value", "Text");
                //return View();
            }
            try
            {
                var liasions1 = liaisons.ToList();
                var item = new SelectListItem();
                item.Text = "No Assigned";
                item.Value = "-1";

                liasions1.Insert(0, item);
                ViewBag.Liaisons = new SelectList(liasions1.OrderBy(l => l.Text), "Value", "Text", user.Role == "Liaison" ? user.CCMid.Value.ToString() : "");
                //Add Item in translator for No Assigned
                var translator1 = translator.ToList();
                translator1.Insert(0, item);
                ViewBag.translator = new SelectList(translator1, "Value", "Text").OrderBy("Text");
                //Add Item in physicians for No Assigned
                var physicians1 = physicians.ToList();
                physicians1.Insert(0, item);
                ViewBag.Physicians = new SelectList(physicians1.OrderBy(p => p.Text), "Value", "Text", user.Role == "Physician" ? user.CCMid.Value.ToString() : "");
                //Add Item in physiciansGroups for No Assigned
                var physiciansGroups1 = physiciansGroups.ToList();
                physiciansGroups1.Insert(0, item);
                ViewBag.physiciansGroups = new SelectList(physiciansGroups1.OrderBy(p => p.Text), "Value", "Text", user.Role == "PhysiciansGroup" ? user.CCMid.Value.ToString() : "");
            }
            catch /*(Exception ex)*/
            {
                ViewBag.Liaisons = new SelectList(liaisons.OrderBy(l => l.Text), "Value", "Text");
                ViewBag.translator = new SelectList(translator, "Value", "Text").OrderBy("Text");
                ViewBag.Physicians = new SelectList(physicians.OrderBy(p => p.Text), "Value", "Text");
                ViewBag.physiciansGroups = new SelectList(physiciansGroups.OrderBy(p => p.Text), "Value", "Text");

            }
            ViewBag.BillingCategories = _db.BillingCategories.OrderBy(n => n.BillingCategoryId).ToList();
            ViewBag.Laisons = _db.Liaisons.Where(p => p.IsTranslator == false).Include(p => p.Liaisons_BillingCategories).Select(p => p).ToList();
            ViewBag.Translators = _db.Liaisons.Where(p => p.IsTranslator == true).Include(p => p.Liaisons_BillingCategories).Select(p => p).ToList();
            ViewBag.EnrollmentReasons = _db.EnrollmentSubstatusReasons.ToList();

            return View();
        }
        public ActionResult TotalPatientsStaff(string userId, string status = "", string substatus = "", DateTime? date = null)
        {
            //


            //

            ViewBag.EnrollmentStatus = new SelectList(_db.EnrollmentStatuss.ToList(), "Id", "Name");
            ViewBag.EnrollmentSubStatus = new SelectList(_db.EnrollmentSubStatuss.ToList(), "EnrollmentStatusID", "Name");
            ViewBag.EnrollemntStatusReson = new SelectList(_db.EnrollmentSubstatusReasons.ToList(), "Name", "Name");
            ViewBag.EnrollmentStauses = _db.EnrollmentStatuss.ToList();
            ViewBag.EnrollmentSubStatuses = _db.EnrollmentSubStatuss.ToList();
            var user = string.IsNullOrEmpty(userId)
                ? _db.Users.Find(GetUserId())
                : _db.Users.Find(userId);
            var user1 =
               _db.Users.Find(GetUserId());

            //ViewBag.UserId = user.Id;
            ViewBag.Status = "Total Patients";
            ViewBag.Owner = user1.Role == "Liaison" || user1.Role == "PhysiciansGroup" ? user1.FirstName
                : user1.Role == "Physician" ? "Dr. " + user1.LastName
                : "Admin";


            ViewBag.UserId = userId;
            ViewBag.Status = date != null ? "Calls Due (" + (date == DateTime.Today ? "Today" : "Tomorrow") + ")"
                           : string.IsNullOrEmpty(status) ? "Unknown"
                           : status;
            ViewBag.StatusStr = status;
            ViewBag.SubStatus = substatus == null ? "" : substatus;
            ViewBag.DateStr = date;

            ViewBag.CCMID = user.Role == "Liaison" || user.Role == "PhysiciansGroup" ? user.CCMid.Value
                          : user.Role == "Physician" ? user.CCMid.Value
                          : 0;
            ViewBag.UserRole = user.Role;


            var physicians = _db.Physicians.AsNoTracking().Select(p => new SelectListItem
            {
                Value = p.Id.ToString(),
                Text = p.FirstName + " " + p.LastName
            });

            var physiciansGroups = _db.PhysiciansGroup.AsNoTracking().Select(p => new SelectListItem
            {
                Value = p.Id.ToString(),
                Text = p.GroupName

            });
            if (User.IsInRole("Sales"))
            {


                List<int> physicianids = new List<int>();
                var physicianidgroupids = _db.physicianGroup_SalesStaff_Mappings.AsNoTracking().Where(x => x.SaleStaffId == user1.CCMid).Select(x => x.PhysiciansGroupId).ToList();
                physicianids = _db.physicianGroup_Physician_Mappings.Where(x => physicianidgroupids.Contains(x.PhysiciansGroupId)).Select(x => x.PhysicianId).ToList();

                var group = _db.Users.Find(GetUserId());
                physicians = _db.Physicians.AsNoTracking().Where(p => physicianids.Contains(p.Id))
                                                             .Select(p => new SelectListItem
                                                             {
                                                                 Value = p.Id.ToString(),
                                                                 Text = p.FirstName + " " + p.LastName
                                                             });

                physiciansGroups = _db.PhysiciansGroup.AsNoTracking().Where(p => physicianidgroupids.Contains(p.Id)).Select(p => new SelectListItem
                {
                    Value = p.Id.ToString(),
                    Text = p.GroupName

                });

            }

            try
            {

                ViewBag.Physicians = new SelectList(physicians.OrderBy(p => p.Text), "Value", "Text", user.Role == "Physician" ? user.CCMid.Value.ToString() : "");
                ViewBag.physiciansGroups = new SelectList(physiciansGroups.OrderBy(p => p.Text), "Value", "Text", user.Role == "PhysiciansGroup" ? user.CCMid.Value.ToString() : "");
            }
            catch /*(Exception ex)*/
            {

                ViewBag.Physicians = new SelectList(physicians.OrderBy(p => p.Text), "Value", "Text");
                ViewBag.physiciansGroups = new SelectList(physiciansGroups.OrderBy(p => p.Text), "Value", "Text");

            }

            return View("SaleStaffPatient");
        }


        [HttpPost]
        public ActionResult LoadDrugData(string status, string userId, string date1, string substatus, DateTime? DateFrom, DateTime? DateTo, string SearchCol = "", int LiaisonId = 0, int PhysicianID = 0, int PhysicianGroupID = 0, int TranslatorID = 0, string Languages = "", string EnrollStatus = "", string EnrollSubStatus = "", string CallingStatus = "", int PostLiaisonId = 0, int PostTranslatorID = 0, int PreLiaisonId = 0, int PreTranslaterId = 0)
        {
            DateTime? date = null;
            if (date1 != "")
            {
                try
                {
                    date = Convert.ToDateTime(date1);
                }
                catch (Exception)
                {


                }


            }
            var draw = Request.Form.GetValues("draw")?.FirstOrDefault();
            var start = Request.Form.GetValues("start")?.FirstOrDefault();
            var length = Request.Form.GetValues("length")?.FirstOrDefault();
            var sortColumn = Request.Form.GetValues("columns[" + Request.Form.GetValues("order[0][column]")?.FirstOrDefault() + "][name]")?.FirstOrDefault();
            var sortColumnDir = Request.Form.GetValues("order[0][dir]")?.FirstOrDefault();
            string searchValue = Request.Form.GetValues("search[value]")?.FirstOrDefault();


            //Paging Size (10,20,50,100)    
            int pageSize = length != null ? Convert.ToInt32(length) : 0;
            //pageSize *= 3;
            int skip = start != null ? Convert.ToInt32(start) : 0;
            var pageSize2 = (pageSize * 300) + skip;
            if (pageSize == 10)
            {
                pageSize2 = (pageSize * 300) + skip;

            }
            if (pageSize == 20)
            {
                pageSize2 = (pageSize * 200) + skip;

            }
            if (pageSize == 50)
            {
                pageSize2 = (pageSize * 125) + skip;

            }
            if (pageSize == 100)
            {
                pageSize2 = (pageSize * 100) + skip;

            }
            int recordsTotal = 0;

            var user = _db.Users.Find(GetUserId());
            //var user = string.IsNullOrEmpty(userId)
            //    ? _db.Users.Find(GetUserId())
            //    : _db.Users.Find(userId);
            var today = DateTime.Today;
            using (ApplicationdbContect _db = new ApplicationdbContect())
            {

                //var dataView3=new List< TotalPatientsViewModel>();
                //if (date == null)
                //{

                //var  dataView3 = _db.Database
                //   .SqlQuery<TotalPatientsViewModel>("Totalpatientsgeneral ")
                //   .ToList();

                //}




                _db.Configuration.ProxyCreationEnabled = false;
                _db.Database.CommandTimeout = 180;

                var dataView = new List<TotalPatientsViewModel>();
                if (date == null)
                {
                    var d = DBNull.Value;
                    dataView = _db.Database
                   .SqlQuery<TotalPatientsViewModel>("TotalPatients @date,@today,@callingstatus,@substatus,@status", new SqlParameter("date", d), new
                   SqlParameter("today", today), new SqlParameter("callingstatus", CallingStatus), new SqlParameter("substatus", substatus), new SqlParameter("status", status)).ToList();
                }
                else
                {
                    dataView = _db.Database
                  .SqlQuery<TotalPatientsViewModel>("TotalPatients @date,@today,@callingstatus,@substatus", new SqlParameter("date", date), new
                  SqlParameter("today", today), new SqlParameter("callingstatus", CallingStatus), new SqlParameter("substatus", substatus)).ToList();
                }
                dataView = dataView.OrderBy(n => n.Id).ThenBy(n => n.BillingCategoryId).ToList();

                var tempcheck = dataView.Where(x => x.Id == 28362).ToList();


                //var patients = date != null && date >= today ? _db.Patients.AsNoTracking().AsQueryable().Where(p => DbFunctions.TruncateTime(p.AppointmentDate) == date)
                //           : date != null && date < today ? _db.Patients.AsNoTracking().AsQueryable().Where(p => DbFunctions.TruncateTime(p.AppointmentDate) < today)
                //           : !string.IsNullOrEmpty(CallingStatus) && CallingStatus == "Left Voice Message"
                //           ? _db.Patients.AsNoTracking().AsQueryable().Where(p => (p.CallingStatus == "Left Voice Message 1" ||
                //                                                     p.CallingStatus == "Left Voice Message 2" ||
                //                                                     p.CallingStatus == "Left Voice Message 3"))
                //           : !string.IsNullOrEmpty(CallingStatus) && CallingStatus == "Left Voice Message 1"
                //           ? _db.Patients.AsNoTracking().AsQueryable().Where(p => (p.CallingStatus == "Left Voice Message 1"))
                //           : !string.IsNullOrEmpty(CallingStatus) && CallingStatus == "Left Voice Message 2"
                //           ? _db.Patients.AsNoTracking().AsQueryable().Where(p => (p.CallingStatus == "Left Voice Message 2"))
                //           : !string.IsNullOrEmpty(CallingStatus) && CallingStatus == "Left Voice Message 3"
                //           ? _db.Patients.AsNoTracking().AsQueryable().Where(p => (p.CallingStatus == "Left Voice Message 3"))
                //      : !string.IsNullOrEmpty(substatus) ? _db.Patients.AsNoTracking().Where(p => (p.EnrollmentSubStatus == substatus)).AsQueryable()
                //                                                     : !string.IsNullOrEmpty(status) ? _db.Patients.AsNoTracking().Where(p => p.EnrollmentStatus == status).AsQueryable() :
                //                                                     _db.Patients.AsNoTracking().AsQueryable();
                //This is getting the Data
                //from p in _db.Patients
                //join pe in _db.PatientMeidcareMedicaidEligibilities on p.Id equals pe.PatientId into ps
                //from pe in ps.DefaultIfEmpty()
                //var c = patients.Count();
                //var liaisons = _db.Liaisons.ToList();

                //var dataView = (from p in patients.AsNoTracking()
                //                join L in _db.Liaisons.AsNoTracking() on p.TranslatorId equals L.Id into T
                //                join Pre in _db.Patients_PreLiaisons.AsNoTracking() on p.Patients_PreLiaisonsId equals Pre.Id into Pre
                //                join patientsCat in _db.Patients_BillingCategories.AsNoTracking() on p.Id equals patientsCat.PatientId into patientsCat
                //                from patientsCat1 in patientsCat.DefaultIfEmpty()
                //                from Pre1 in Pre.DefaultIfEmpty()
                //                from L1 in T.DefaultIfEmpty()

                //                select new CCM.Models.ViewModels.TotalPatientsViewModel 
                //                {
                //                    FirstName = p.FirstName + " " + p.LastName,
                //                    LastName= p.LastName,
                //                    Id= p.Id,
                //                    Cycle=  p.Cycle,
                //                    BirthDate= p.BirthDate,

                //                    Gender = p.Gender ?? "",
                //                    PreferredLanguage = p.PreferredLanguage ?? "",
                //                    AppointmentDate = p.AppointmentDate,
                //                    AppointmentDateStr = p.AppointmentDate.Value == null ? "" : p.AppointmentDate.Value.ToString(),
                //                    EnrollmentStatus = p.EnrollmentStatus ?? "",
                //                    LiaisonId = patientsCat1.IsTranslator == false ? patientsCat1.LiaisonId : 0,
                //                    liaisonFirstName = patientsCat1.IsTranslator == false ? patientsCat1.LiaisonId!=null?_db.Liaisons.Where(p=>p.Id== patientsCat1.LiaisonId).Select(p=>p.FirstName+p.LastName).FirstOrDefault():"": "",
                //                    liaisonLastName = p.Liaison.LastName ?? "",
                //                    liaisonassignedon = p.LiasionAssignedOn,

                //                    enrolledon = p.CCMEnrolledOn,

                //                    DocFirstName = p.Physician.FirstName + " " + p.Physician.LastName,
                //                    DocLastName = p.Physician.LastName ?? "",
                //                    enrollmentsubstatus = p.EnrollmentSubStatus ?? "",
                //                    callingstatus = p.CallingStatus == null ? "" : p.CallingStatus,
                //                    emrnumber = p.EMRNumber == null ? "" : p.EMRNumber,
                //                    emrtype = p.EMRType == null ? "" : p.EMRType,
                //                    picassochecked = p.PicassoChecked == null ? "" : p.PicassoChecked,
                //                    picssodate = p.PicassoCheckedOn,
                //                    //medicareeligibility = pe.MedicareEligibilty == null ? "" : pe.MedicareEligibilty,
                //                    //medicaideligibility = pe.MedicaidEligibilty == null ? "" : pe.MedicaidEligibilty,
                //                    capitated = p.CapitatedPatient == null ? "" : p.CapitatedPatient,
                //                    capitatedfrom = p.CapitatedFrom,
                //                    capitatedto = p.CapitatedTo  ,
                //                    PhysicianId = p.PhysicianId ?? 0,
                //                    MainPhoneNumber=p.Physician.MainPhoneNumber,
                //                    note = p.EnrollmentNotes == null ? p.Notes : p.EnrollmentNotes,
                //                    insuranceid = p.Insurance.PrimaryIdNumber,
                //                    insurancename = p.Insurance.PrimaryName,
                //                    TranslatorId = patientsCat1.IsTranslator==true ? patientsCat1.LiaisonId: 0,
                //                    Translator = L1.FirstName + " " + L1.LastName,
                //                    TranslatorAssignedOn=p.TranslatorAssignedOn,
                //                    Patients_PreLiaisonId = p.Patients_PreLiaisonsId,
                //                    preliaisonId = Pre1.LiaisonId ?? 0,
                //                    pretranslatorId = Pre1.TranslatorId ?? 0,
                //                    prestatus = p.Patients_PreLiaisonsId == null ? false : Pre1.Status,
                //                    PreLiaisonName = _db.Liaisons.Where(p => p.Id == (Pre1.LiaisonId == null ? 0 : Pre1.Status == false ?0 :Pre1.LiaisonId)).Select(p => p.FirstName+p.LastName).FirstOrDefault(),
                //                    PreTranslatorName = _db.Liaisons.Where(p => p.Id == (Pre1.LiaisonId == null ? 0 : Pre1.Status == false?0: Pre1.TranslatorId)).Select(p => p.FirstName + p.LastName).FirstOrDefault(),
                //                    BillingCategoryId= patientsCat1.Status!=false? patientsCat1.BillingCategoryId:0,








                //                }).ToList();
                List<int> physicianids = new List<int>();

                //foreach (var item in dataView)
                //{
                //    physicianids.Add(item.Id);
                //    foreach (var item in collection)
                //    {

                //    }



                //}

                //foreach (var item in dataView)
                //{

                //    if (item.preliaison != 0)
                //    {
                //        string Lname = liaisons.Where(p => p.Id == item.preliaison).Select(p => p.FirstName + p.LastName).FirstOrDefault();

                //    }


                //}





                if (user.Role == "PhysiciansGroup")
                {
                    physicianids = _db.physicianGroup_Physician_Mappings.AsNoTracking().Where(x => x.PhysiciansGroupId == user.CCMid).Select(x => x.PhysicianId).ToList();
                }
                if (user.Role == "Sales")
                {
                    var physiciangrpids = _db.physicianGroup_SalesStaff_Mappings.AsNoTracking().Where(x => x.SaleStaffId == user.CCMid).Select(x => x.PhysiciansGroupId).ToList();
                    physicianids = _db.physicianGroup_Physician_Mappings.AsNoTracking().Where(x => physiciangrpids.Contains(x.PhysiciansGroupId)).Select(x => x.PhysicianId).ToList();
                }
                List<int> liasionids = new List<int>();
                if (user.Role == "LiaisonGroup")
                {
                    liasionids = _db.LiaisonGroup_Liaison_Mappings.Where(x => x.LiaisonGroupId == user.CCMid).Select(x => x.LiaisonId).ToList();
                }


                var pIds2 = new List<int?>();

                pIds2 = user.Role == "Liaison" && HelperExtensions.isTranslator(user.Id) == false
                           ? dataView.Where(p => p.LiaisonId == user.CCMid || (p.preliaisonId == user.CCMid && p.prestatus == true)).Select(p => p.Id).ToList()
                           : user.Role == "Liaison" && HelperExtensions.isTranslator(user.Id) == true
                           ? dataView.Where(p => p.TranslatorId == user.CCMid || (p.pretranslatorId == user.CCMid && p.prestatus == true)).Select(p => p.Id).ToList()
                       : user.Role == "Physician"
                       ? dataView.Where(p => p.PhysicianId == user.CCMid).Select(p => p.Id).ToList()
                       : user.Role == "PhysiciansGroup" || user.Role == "Sales"
                       ? dataView.Where(p => physicianids.Contains(p.PhysicianId)).Select(p => p.Id).ToList() //.Value
                        : user.Role == "LiaisonGroup"
                          ? dataView.Where(p => liasionids.Contains(p.LiaisonId ?? 0)).Select(p => p.Id).ToList()
                       : dataView.Select(p => p.Id).ToList();


                dataView = (from d in dataView
                            where pIds2.Contains(d.Id)
                            select d).ToList();


                if (LiaisonId > 0)
                {
                    dataView = dataView.Where(p => p.PostLiaisonId == LiaisonId || (p.preliaisonId == user.CCMid && p.prestatus == true)).ToList();
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
                    dataView = dataView.Where(p => physicianids.Contains(p.PhysicianId)).ToList(); //.Value

                }
                if (PhysicianGroupID == -1)
                {
                    physicianids = _db.physicianGroup_Physician_Mappings.AsNoTracking().Select(x => x.PhysicianId).ToList();
                    dataView = dataView.Where(p => !physicianids.Contains(p.PhysicianId)).ToList(); //.Value

                }
                if (TranslatorID > 0)
                {
                    dataView = dataView.Where(p => p.TranslatorId == TranslatorID || p.pretranslatorId == TranslatorID).ToList();
                }
                if (TranslatorID == -1)
                {
                    dataView = dataView.Where(p => p.TranslatorId == 0).ToList();
                }
                if (Languages != "")
                {
                    dataView = dataView.Where(p => p.PreferredLanguage == Languages).ToList();
                }

                if (PostLiaisonId > 0)
                {
                    var pIds = dataView.Where(p => p.PostLiaisonId == PostLiaisonId).Select(p => p.Id).ToList();
                    dataView = (from d in dataView
                                where pIds.Contains(d.Id)
                                select d).ToList();

                }
                if (PostTranslatorID > 0)
                {
                    var pIds = dataView.Where(p => p.TranslatorId == PostTranslatorID).Select(p => p.Id).ToList();
                    dataView = (from d in dataView
                                where pIds.Contains(d.Id)
                                select d).ToList();
                }
                if (PreLiaisonId > 0)
                {
                    dataView = dataView.Where(p => p.preliaisonId == PreLiaisonId && p.prestatus == true).ToList();
                }
                if (PreTranslaterId > 0)
                {
                    dataView = dataView.Where(p => p.pretranslatorId == PreTranslaterId && p.prestatus == true).ToList();
                }








                //if(EnrollStatus != "")
                //{
                //    dataView = dataView.Where(p => p.EnrollmentStatus == EnrollStatus).ToList();
                //}
                //if(EnrollSubStatus != "")
                //{
                //    dataView = dataView.Where(p => p.enrollmentsubstatus == EnrollSubStatus).ToList();
                //}
                //if(CallingStatus != "")
                //{
                //    if (CallingStatus == "")
                //    {

                //    }
                //    else
                //    {

                //    }
                //}
                try
                {
                    if (SearchCol != "" && DateFrom != null && DateTo != null)
                    {
                        if (SearchCol == "Enrolled On")
                        {
                            dataView = dataView.Where(p => p.enrolledon?.Date >= DateFrom && p.enrolledon?.Date <= DateTo).ToList();
                        }

                        else
                        {
                            if (SearchCol == "Appointment")
                            {
                                dataView = dataView.Where(p => p.AppointmentDate?.Date >= DateFrom && p.AppointmentDate?.Date <= DateTo).ToList();
                            }
                            else
                            {
                                if (SearchCol == "Date of Birth")
                                {
                                    dataView = dataView.Where(p => p.BirthDate.Date >= DateFrom && p.BirthDate.Date <= DateTo).ToList();
                                }
                                else
                                {
                                    if (SearchCol == "Assigned on")
                                    {
                                        dataView = dataView.Where(p => p.liaisonassignedon?.Date >= DateFrom && p.liaisonassignedon?.Date <= DateTo).ToList();

                                    }
                                    else
                                    {
                                        if (SearchCol == "Capitated")
                                        {
                                            dataView = dataView.Where(p => p.capitatedfrom?.Date >= DateFrom && p.capitatedto?.Date <= DateTo).ToList();

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
                //SORT
                if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
                {
                    dataView = dataView.OrderBy(sortColumn + " " + sortColumnDir).ToList();
                }

                //Search  
                try
                {
                    if (!string.IsNullOrEmpty(searchValue) && !string.IsNullOrWhiteSpace(searchValue))
                    {
                        bool isDateTimeSearch = false;
                        try
                        {
                            var searcvaluedate = Convert.ToDateTime(searchValue);
                            if (searcvaluedate != null)
                            {
                                isDateTimeSearch = true;
                                dataView = dataView.Where(p => p.BirthDate.ToString("MM-dd-yyyy") == searcvaluedate.ToString("MM-dd-yyyy") ||
                                 p?.AppointmentDate?.ToString("MM-dd-yyyy") == searcvaluedate.ToString("MM-dd-yyyy") ||
                                  p?.enrolledon?.ToString("MM-dd-yyyy") == searcvaluedate.ToString("MM-dd-yyyy") ||
                                   p?.liaisonassignedon?.ToString("MM-dd-yyyy") == searcvaluedate.ToString("MM-dd-yyyy") ||
                                 p?.picssodate?.ToString("MM-dd-yyyy") == searcvaluedate.ToString("MM-dd-yyyy") ||
                                 p?.capitatedfrom?.ToString("MM-dd-yyyy") == searcvaluedate.ToString("MM-dd-yyyy") ||
                                 p?.capitatedto?.ToString("MM-dd-yyyy") == searcvaluedate.ToString("MM-dd-yyyy")




                                                    ).ToList();
                            }
                        }
                        catch /*(Exception ex)*/
                        {


                        }
                        // Apply search   
                        if (isDateTimeSearch == false)
                        {
                            dataView = dataView.Where(p => p.FirstName.ToString().ToLower().Contains(searchValue.ToLower()) ||
                                                                               //p.LastName.ToLower().Contains(searchValue.ToLower()) ||
                                                                               p.EnrollmentStatus.ToLower() == (searchValue.ToLower()) ||
                                                                               p.DocFirstName.ToLower().Contains(searchValue.ToLower()) ||
                                                                               //p.DocLastName.ToLower().Contains(searchValue.ToLower()) ||
                                                                               p.liaisonFirstName.ToLower().Contains(searchValue.ToLower()) ||
                                                                               p.liaisonLastName.ToLower().Contains(searchValue.ToLower()) ||
                                                                               p.Gender.ToLower() == (searchValue.ToLower()) ||
                                                                               p.Cycle.ToString().ToLower().Contains(searchValue.ToLower()) ||
                                                                               p.PreferredLanguage.ToLower().Contains(searchValue.ToLower()) ||

                                                                                p.enrollmentsubstatus.ToLower() == searchValue.ToLower() ||
                                                                                p.callingstatus.ToLower().Contains(searchValue.ToLower()) ||
                                                                                p.emrnumber.ToLower().Contains(searchValue.ToLower()) ||
                                                                                p.emrtype.ToLower().Contains(searchValue.ToLower()) ||
                                                                                p.Id.ToString().ToLower().Contains(searchValue.ToLower()) ||
                                                                                p.picassochecked.ToLower().Contains(searchValue.ToLower()) ||
                                                                                p.capitated.ToLower().Contains(searchValue.ToLower()) ||
                                                                                (p.note ?? "").ToLower().Contains(searchValue.ToLower()) ||
                                                                                (p.insuranceid ?? "").ToLower().Contains(searchValue.ToLower()) ||
                                                                                (p.insurancename ?? "").ToLower().Contains(searchValue.ToLower())
                                                //p.medicaideligibility.ToLower().Contains(searchValue.ToLower()) ||
                                                //p.medicareeligibility.ToLower().Contains(searchValue.ToLower())





                                                ).ToList();
                        }


                    }


                }
                catch (Exception)
                {


                }
                var physicangroupnames = _db.PhysiciansGroup.AsNoTracking().ToList();
                var phygroupmappings = _db.physicianGroup_Physician_Mappings.AsNoTracking().AsQueryable();


                List<TotalPatientsViewModel> TotalPatients = new List<TotalPatientsViewModel>();
                TotalPatientsViewModel totalPatients = new TotalPatientsViewModel();
                var BillingCategories = _db.BillingCategories.ToList();
                int index2 = -1;

                recordsTotal = dataView.ToList()
  .GroupBy(n => new { n.Id })
  .Select(g => g.First()).Count();

                dataView = dataView.Take(pageSize2).ToList();
                var count = dataView.Count();

                foreach (var item in dataView)
                {


                    //item.PhyGroupName = physicangroupnames.Where(x => x.Id == (phygroupmappings.Where(x1 => x1.PhysicianId == item.PhysicianId).FirstOrDefault()?.PhysiciansGroupId)).FirstOrDefault()?.GroupName;
                    item.AppointmentDateStr = item.AppointmentDate == null ? "" : item.AppointmentDate.Value.ToString("MM/dd/yyyy hh:mm tt");
                    item.BirthDatestr = item.BirthDate == null ? "" : item.BirthDate.ToString("MM/dd/yyyy hh:mm tt");
                    item.enrolledonstr = item.enrolledon == null ? "" : item.enrolledon.Value.ToString("MM/dd/yyyy hh:mm tt");
                    item.TranslatorAssignedOnstr = item.TranslatorAssignedOn == null ? "" : item.TranslatorAssignedOn.Value.ToString("MM/dd/yyyy hh:mm tt");



                    var index = dataView.IndexOf(item);
                    if (item.enrollmentsubstatus != "Active Enrolled")
                    {


                        totalPatients = item;
                        foreach (var Bill in BillingCategories)
                        {
                            if (totalPatients.TotalPatientsCategoryViewModel.Where(x => x.CategoryId == Bill.BillingCategoryId).FirstOrDefault() == null)
                            {
                                var obj = new TotalPatientsCategoryViewModel()
                                {

                                    CategoryId = Bill.BillingCategoryId,
                                    CategoryName = Bill.Name,
                                    LiaisonId = null,
                                    LiaisonName = "",
                                    TranslatorId = null,
                                    TranslatorName = "",

                                };
                                totalPatients.TotalPatientsCategoryViewModel.Add(obj);
                            }
                        }

                        totalPatients.TotalPatientsCategoryViewModel = totalPatients.TotalPatientsCategoryViewModel.OrderBy(x => x.CategoryId).ToList();
                        TotalPatients.Add(totalPatients);
                        index = index + 1;
                        index2 = index;

                        continue;
                    }




                    if (index < index2)
                    {
                        continue;
                    }


                    totalPatients = item;


                    var same = true;

                    while (same != false)
                    {
                        if (index < count - 1)
                        {

                            if (item.Id == dataView[index].Id)
                            {

                                if (dataView[index].Id == dataView[index + 1].Id)
                                {
                                    if (dataView[index].BillingCategoryId == dataView[index + 1].BillingCategoryId && index != count - 1)
                                    {
                                        TotalPatientsCategoryViewModel totalPatientsCategory = new TotalPatientsCategoryViewModel();

                                        totalPatientsCategory.LiaisonId = dataView[index].LiaisonId != 0 ? dataView[index].LiaisonId : dataView[index + 1].LiaisonId;
                                        totalPatientsCategory.TranslatorId = dataView[index].TranslatorId != 0 ? dataView[index].TranslatorId : dataView[index + 1].TranslatorId;
                                        totalPatientsCategory.LiaisonName = dataView[index].LiaisonName != null ? dataView[index].LiaisonName : dataView[index + 1].LiaisonName;
                                        totalPatientsCategory.TranslatorName = dataView[index].Translator != null ? dataView[index].Translator : dataView[index + 1].Translator;
                                        totalPatientsCategory.CategoryId = dataView[index].BillingCategoryId;
                                        int? categoryId = dataView[index].BillingCategoryId;
                                        totalPatientsCategory.CategoryName = dataView[index].BillingCategoryId != 0 ? _db.BillingCategories.Where(p => p.BillingCategoryId == categoryId).Select(p => p.Name).FirstOrDefault() : "";
                                        if (totalPatients.enrollmentsubstatus != "Active Enrolled")
                                        {
                                            totalPatientsCategory.LiaisonName = "";
                                            totalPatientsCategory.TranslatorName = "";
                                        }
                                        else
                                        {
                                            totalPatients.PreLiaisonName = "";
                                            totalPatients.PreTranslatorName = "";
                                        }

                                        totalPatients.TotalPatientsCategoryViewModel.Add(totalPatientsCategory);

                                        same = true;
                                        index = index + 1;
                                        index2 = index;
                                    }
                                    else
                                    {
                                        TotalPatientsCategoryViewModel totalPatientsCategory2 = new TotalPatientsCategoryViewModel();

                                        totalPatientsCategory2.LiaisonId = dataView[index].LiaisonId;
                                        totalPatientsCategory2.TranslatorId = dataView[index].TranslatorId;
                                        totalPatientsCategory2.LiaisonName = dataView[index].LiaisonName;
                                        totalPatientsCategory2.TranslatorName = dataView[index].Translator;
                                        totalPatientsCategory2.CategoryId = dataView[index].BillingCategoryId;
                                        int? categoryId = dataView[index].BillingCategoryId;
                                        totalPatientsCategory2.CategoryName = dataView[index].BillingCategoryId != 0 ? _db.BillingCategories.Where(p => p.BillingCategoryId == categoryId).Select(p => p.Name).FirstOrDefault() : "";

                                        if (totalPatients.enrollmentsubstatus != "Active Enrolled")
                                        {
                                            totalPatientsCategory2.LiaisonName = "";
                                            totalPatientsCategory2.TranslatorName = "";
                                        }
                                        else
                                        {
                                            totalPatients.PreLiaisonName = "";
                                            totalPatients.PreTranslatorName = "";
                                        }
                                        totalPatients.TotalPatientsCategoryViewModel.Add(totalPatientsCategory2);


                                    }

                                }

                                else
                                {
                                    TotalPatientsCategoryViewModel totalPatientsCategory = new TotalPatientsCategoryViewModel();

                                    totalPatientsCategory.LiaisonId = dataView[index].LiaisonId;
                                    totalPatientsCategory.TranslatorId = dataView[index].TranslatorId;
                                    totalPatientsCategory.LiaisonName = dataView[index].LiaisonName;
                                    totalPatientsCategory.TranslatorName = dataView[index].Translator;
                                    totalPatientsCategory.CategoryId = dataView[index].BillingCategoryId;
                                    int? categoryId = dataView[index].BillingCategoryId;
                                    totalPatientsCategory.CategoryName = dataView[index].BillingCategoryId != 0 ? _db.BillingCategories.Where(p => p.BillingCategoryId == categoryId).Select(p => p.Name).FirstOrDefault() : "";

                                    if (totalPatients.enrollmentsubstatus != "Active Enrolled")
                                    {
                                        totalPatientsCategory.LiaisonName = "";
                                        totalPatientsCategory.TranslatorName = "";
                                    }
                                    else
                                    {
                                        totalPatients.PreLiaisonName = "";
                                        totalPatients.PreTranslatorName = "";
                                    }
                                    totalPatients.TotalPatientsCategoryViewModel.Add(totalPatientsCategory);


                                }










                                same = true;
                                index = index + 1;
                                index2 = index;

                            }
                            else
                            {

                                same = false;
                            }




                        }
                        else
                        {
                            if (index == count - 1)
                            {


                                TotalPatientsCategoryViewModel totalPatientsCategory = new TotalPatientsCategoryViewModel();

                                totalPatientsCategory.LiaisonId = dataView[index].LiaisonId;
                                totalPatientsCategory.TranslatorId = dataView[index].TranslatorId;
                                totalPatientsCategory.LiaisonName = dataView[index].LiaisonName;
                                totalPatientsCategory.TranslatorName = dataView[index].Translator;
                                totalPatientsCategory.CategoryId = dataView[index].BillingCategoryId;
                                int? categoryId = dataView[index].BillingCategoryId;
                                totalPatientsCategory.CategoryName = dataView[index].BillingCategoryId != 0 ? _db.BillingCategories.Where(p => p.BillingCategoryId == categoryId).Select(p => p.Name).FirstOrDefault() : "";
                                if (totalPatients.enrollmentsubstatus != "Active Enrolled")
                                {
                                    totalPatientsCategory.LiaisonName = "";
                                    totalPatientsCategory.TranslatorName = "";
                                }
                                else
                                {
                                    totalPatients.PreLiaisonName = "";
                                    totalPatients.PreTranslatorName = "";
                                }
                                totalPatients.TotalPatientsCategoryViewModel.Add(totalPatientsCategory);
                                index = index + 1;
                                index2 = index;
                            }

                            same = false;
                        }
                    }
                    foreach (var Bill in BillingCategories)
                    {
                        if (totalPatients.TotalPatientsCategoryViewModel.Where(x => x.CategoryId == Bill.BillingCategoryId).FirstOrDefault() == null)
                        {
                            var obj = new TotalPatientsCategoryViewModel()
                            {

                                CategoryId = Bill.BillingCategoryId,
                                CategoryName = Bill.Name,
                                LiaisonId = null,
                                LiaisonName = "",
                                TranslatorId = null,
                                TranslatorName = "",

                            };
                            totalPatients.TotalPatientsCategoryViewModel.Add(obj);
                        }
                    }

                    totalPatients.TotalPatientsCategoryViewModel = totalPatients.TotalPatientsCategoryViewModel.OrderBy(x => x.CategoryId).ToList();
                    TotalPatients.Add(totalPatients);

                }

                //total number of rows count     

                //Paging     
                if (pageSize < 0)
                {
                    pageSize = recordsTotal;
                }

                var data = TotalPatients.Skip(skip).Take(pageSize).ToList();




                //var data = dataView.Skip(skip).Take(pageSize).ToList().Select(p => new TotalPatientsViewModel2
                //{
                //    FirstName = p.FirstName,

                //    Id = p.Id,
                //    Cycle = p.Cycle,

                //    BirthDate = p.BirthDate == null ? "" : p.BirthDate.ToString("MM/dd/yyyy"),
                //    Gender = p.Gender,
                //    PreferredLanguage = p.PreferredLanguage,

                //    AppointmentDate = p.AppointmentDate == null ? "" : p.AppointmentDate.Value.ToString("MM/dd/yyyy hh:mm tt"),
                //    EnrollmentStatus = p.EnrollmentStatus ?? "",
                //    LiaisonId = p.LiaisonId,
                //    liaisonFirstName = p.LiaisonName,
                //    liaisonLastName = p.liaisonLastName ?? "",
                //    liaisonassignedon = p.liaisonassignedon == null ? "" : p.liaisonassignedon.Value.ToString("MM/dd/yyyy hh:mm tt"),

                //    enrolledon = p.enrolledon == null ? "" : p.enrolledon.Value.ToString("MM/dd/yyyy hh:mm tt"),

                //    DocFirstName = p.DocFirstName,

                //    enrollmentsubstatus = p.enrollmentsubstatus ?? "",
                //    callingstatus = p.callingstatus == null ? "" : p.callingstatus,
                //    emrnumber = p.emrnumber == null ? "" : p.emrnumber,
                //    emrtype = p.emrtype == null ? "" : p.emrtype,
                //    picassochecked = p.picassochecked == null ? "" : p.picassochecked,
                //    picssodate = p.picssodate,
                //    //medicareeligibility = pe.MedicareEligibilty == null ? "" : pe.MedicareEligibilty,
                //    //medicaideligibility = pe.MedicaidEligibilty == null ? "" : pe.MedicaidEligibilty,
                //    capitated = p.capitated == null ? "" : p.capitated,
                //    capitatedfrom = p.capitatedfrom,
                //    capitatedto = p.capitatedto,
                //    PhysicianId = p.PhysicianId,

                //    note = p.note == null ? "" : p.note,
                //    insuranceid = p.insuranceid,
                //    insurancename = p.insurancename,
                //    TranslatorId = p.TranslatorId,
                //    Translator = _db.Liaisons.Where(x => x.Id == p.TranslatorId).Select(x => x.FirstName).FirstOrDefault(),
                //    PreLiaisonName = p.PreLiaisonName,
                //    PreTranslatorName = p.PreTranslatorName,
                //    TranslatorAssignedOn = p.TranslatorAssignedOn == null ? "" : p.TranslatorAssignedOn.Value.ToString("MM/dd/yyyy hh:mm tt"),
                //    PhyGroupName = physicangroupnames.Where(x => x.Id == (phygroupmappings.Where(x1 => x1.PhysicianId == p.PhysicianId).FirstOrDefault()?.PhysiciansGroupId)).FirstOrDefault()?.GroupName,
                //    BillingCategoryId = p.BillingCategoryId

                //}).OrderBy(n => n.Id).ThenBy(p => p.BillingCategoryId).ToList();






                //Returning Json Data    
                var jsonResult = Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data, JsonRequestBehavior.AllowGet });
                jsonResult.MaxJsonLength = int.MaxValue;
                return jsonResult;
            }
        }
        [HttpPost]
        public ActionResult LoadPatientDataSurvey(string status, string userId, string date1, string substatus, DateTime? DateFrom, DateTime? DateTo, string SearchCol = "", int LiaisonId = 0, int PhysicianID = 0, int PhysicianGroupID = 0)
        {
            DateTime? date = null;
            if (date1 != "")
            {
                try
                {
                    date = Convert.ToDateTime(date1);
                }
                catch (Exception)
                {

                }
            }
            var draw = Request.Form.GetValues("draw")?.FirstOrDefault();
            var start = Request.Form.GetValues("start")?.FirstOrDefault();
            var length = Request.Form.GetValues("length")?.FirstOrDefault();
            var sortColumn = Request.Form.GetValues("columns[" + Request.Form.GetValues("order[0][column]")?.FirstOrDefault() + "][name]")?.FirstOrDefault();
            var sortColumnDir = Request.Form.GetValues("order[0][dir]")?.FirstOrDefault();
            string searchValue = Request.Form.GetValues("search[value]")?.FirstOrDefault();


            //Paging Size (10,20,50,100)    
            int pageSize = length != null ? Convert.ToInt32(length) : 0;
            int skip = start != null ? Convert.ToInt32(start) : 0;
            int recordsTotal = 0;

            var user = _db.Users.Find(GetUserId());
            //var user = string.IsNullOrEmpty(userId)
            //    ? _db.Users.Find(GetUserId())
            //    : _db.Users.Find(userId);
            var today = DateTime.Today;
            using (ApplicationdbContect _db = new ApplicationdbContect())
            {

                _db.Configuration.ProxyCreationEnabled = false;
                _db.Database.CommandTimeout = 180;
                var patients = date != null && date >= today ? _db.Patients.AsNoTracking().AsQueryable().Where(p => DbFunctions.TruncateTime(p.AppointmentDate) == date)
                           : date != null && date < today ? _db.Patients.AsNoTracking().AsQueryable().Where(p => DbFunctions.TruncateTime(p.AppointmentDate) < today)
                           : !string.IsNullOrEmpty(status) && status == "Left Voice Message"
                           ? _db.Patients.AsNoTracking().AsQueryable().Where(p => (p.CallingStatus == "Left Voice Message 1" ||
                                                                     p.CallingStatus == "Left Voice Message 2" ||
                                                                     p.CallingStatus == "Left Voice Message 3"))
                           : !string.IsNullOrEmpty(status) && status == "Left Voice Message 1"
                           ? _db.Patients.AsNoTracking().AsQueryable().Where(p => (p.CallingStatus == "Left Voice Message 1"))
                           : !string.IsNullOrEmpty(status) && status == "Left Voice Message 2"
                           ? _db.Patients.AsNoTracking().AsQueryable().Where(p => (p.CallingStatus == "Left Voice Message 2"))
                           : !string.IsNullOrEmpty(status) && status == "Left Voice Message 3"
                           ? _db.Patients.AsNoTracking().AsQueryable().Where(p => (p.CallingStatus == "Left Voice Message 3"))
                      : !string.IsNullOrEmpty(substatus) ? _db.Patients.AsNoTracking().Where(p => (p.EnrollmentSubStatus == substatus)).AsQueryable()
                                                                     : !string.IsNullOrEmpty(status) ? _db.Patients.AsNoTracking().Where(p => p.EnrollmentStatus == status).AsQueryable() :
                                                                     _db.Patients.AsNoTracking().AsQueryable();
                //This is getting the Data
                //from p in _db.Patients
                //join pe in _db.PatientMeidcareMedicaidEligibilities on p.Id equals pe.PatientId into ps
                //from pe in ps.DefaultIfEmpty()
                var dataView = (from p in patients.AsNoTracking()
                                where p.EnrollmentSubStatus == "Active Enrolled"

                                select new
                                {
                                    FirstName = p.FirstName + " " + p.LastName,
                                    p.LastName,
                                    p.Id,
                                    p.Cycle,
                                    p.BirthDate,

                                    Gender = p.Gender ?? "",
                                    PreferredLanguage = p.PreferredLanguage ?? "",

                                    EnrollmentStatus = p.EnrollmentStatus ?? "",
                                    LiaisonId = p.LiaisonId ?? 0,
                                    liaisonFirstName = p.Liaison.FirstName ?? "",
                                    liaisonLastName = p.Liaison.LastName ?? "",


                                    DocFirstName = p.Physician.FirstName + " " + p.Physician.LastName,

                                    enrollmentsubstatus = p.EnrollmentSubStatus ?? "",

                                    p.PhysicianId,
                                    p.Physician.MainPhoneNumber

                                }).ToList();
                List<int> physicianids = new List<int>();

                if (user.Role == "PhysiciansGroup")
                {
                    physicianids = _db.physicianGroup_Physician_Mappings.AsNoTracking().Where(x => x.PhysiciansGroupId == user.CCMid).Select(x => x.PhysicianId).ToList();
                }
                List<int> liasionids = new List<int>();
                if (user.Role == "LiaisonGroup")
                {
                    liasionids = _db.LiaisonGroup_Liaison_Mappings.Where(x => x.LiaisonGroupId == user.CCMid).Select(x => x.LiaisonId).ToList();
                }
                dataView = user.Role == "Liaison"
                       ? dataView.Where(p => p.LiaisonId == user.CCMid).ToList()
                       : user.Role == "Physician"
                       ? dataView.Where(p => p.PhysicianId == user.CCMid).ToList()
                       : user.Role == "PhysiciansGroup"
                       ? dataView.Where(p => physicianids.Contains(p.PhysicianId.Value)).ToList()
                        : user.Role == "LiaisonGroup"
                          ? dataView.Where(p => liasionids.Contains(p.LiaisonId)).ToList()
                       : dataView.ToList();

                if (LiaisonId > 0)
                {
                    dataView = dataView.Where(p => p.LiaisonId == LiaisonId).ToList();
                }
                if (PhysicianID > 0)
                {
                    dataView = dataView.Where(p => p.PhysicianId == PhysicianID).ToList();
                }
                if (PhysicianGroupID > 0)
                {
                    physicianids = _db.physicianGroup_Physician_Mappings.AsNoTracking().Where(x => x.PhysiciansGroupId == PhysicianGroupID).Select(x => x.PhysicianId).ToList();
                    dataView = dataView.Where(p => physicianids.Contains(p.PhysicianId.Value)).ToList();

                }
                try
                {

                    if (SearchCol != "" && DateFrom != null && DateTo != null)
                    {
                        if (SearchCol == "Date of Birth")
                        {
                            dataView = dataView.Where(p => p.BirthDate.Date >= DateFrom && p.BirthDate.Date <= DateTo).ToList();
                        }
                        //if (SearchCol == "Enrolled On")
                        //{
                        //    dataView = dataView.Where(p => p.enrolledon?.Date >= DateFrom && p.enrolledon?.Date <= DateTo).ToList();
                        //}

                        //else
                        //{
                        //    if (SearchCol == "Appointment")
                        //    {
                        //        dataView = dataView.Where(p => p.AppointmentDate?.Date >= DateFrom && p.AppointmentDate?.Date <= DateTo).ToList();
                        //    }
                        //    else
                        //    {


                        //        if (SearchCol == "Assigned on")
                        //        {
                        //            dataView = dataView.Where(p => p.liaisonassignedon?.Date >= DateFrom && p.liaisonassignedon?.Date <= DateTo).ToList();

                        //        }
                        //        else
                        //        {
                        //            if (SearchCol == "Capitated")
                        //            {
                        //                dataView = dataView.Where(p => p.capitatedfrom?.Date >= DateFrom && p.capitatedto?.Date <= DateTo).ToList();

                        //            }
                        //        }

                        //    }


                        //}
                    }
                }
                catch (Exception)
                {


                }
                if (sortColumn == "")
                {
                    sortColumn = "Id";
                }
                //SORT
                if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
                {
                    dataView = dataView.OrderBy(sortColumn + " " + sortColumnDir).ToList();
                }

                //Search  
                try
                {
                    if (!string.IsNullOrEmpty(searchValue) && !string.IsNullOrWhiteSpace(searchValue))
                    {
                        bool isDateTimeSearch = false;
                        try
                        {
                            var searcvaluedate = Convert.ToDateTime(searchValue);
                            if (searcvaluedate != null)
                            {
                                isDateTimeSearch = true;
                                dataView = dataView.Where(p => p.BirthDate.ToString("MM-dd-yyyy") == searcvaluedate.ToString("MM-dd-yyyy")





                                                    ).ToList();
                            }
                        }
                        catch (Exception)
                        {


                        }
                        // Apply search   
                        if (isDateTimeSearch == false)
                        {
                            dataView = dataView.Where(p => p.FirstName.ToString().ToLower().Contains(searchValue.ToLower()) ||
                                                                               //p.LastName.ToLower().Contains(searchValue.ToLower()) ||
                                                                               p.EnrollmentStatus.ToLower() == (searchValue.ToLower()) ||
                                                                               p.DocFirstName.ToLower().Contains(searchValue.ToLower()) ||
                                                                               //p.DocLastName.ToLower().Contains(searchValue.ToLower()) ||
                                                                               p.liaisonFirstName.ToLower().Contains(searchValue.ToLower()) ||
                                                                               p.liaisonLastName.ToLower().Contains(searchValue.ToLower()) ||
                                                                               p.Gender.ToLower() == (searchValue.ToLower()) ||
                                                                               p.Cycle.ToString().ToLower().Contains(searchValue.ToLower()) ||
                                                                               p.PreferredLanguage.ToLower().Contains(searchValue.ToLower()) ||

                                                                                p.enrollmentsubstatus.ToLower() == searchValue.ToLower() ||




                                                                                p.Id.ToString().ToLower().Contains(searchValue.ToLower())





                                                //p.medicaideligibility.ToLower().Contains(searchValue.ToLower()) ||
                                                //p.medicareeligibility.ToLower().Contains(searchValue.ToLower())





                                                ).ToList();
                        }


                    }
                }
                catch (Exception)
                {


                }



                //total number of rows count     
                recordsTotal = dataView.Count();
                //Paging     
                if (pageSize == -1)
                {
                    pageSize = recordsTotal;
                }
                var data = dataView.Skip(skip).Take(pageSize).ToList().Select(p => new
                {
                    p.FirstName,

                    p.Id,
                    p.Cycle,

                    BirthDate = p.BirthDate == null ? "" : p.BirthDate.ToString("MM/dd/yyyy"),
                    p.Gender,
                    p.PreferredLanguage,


                    EnrollmentStatus = p.EnrollmentStatus ?? "",
                    p.LiaisonId,
                    liaisonFirstName = p.liaisonFirstName ?? "",
                    liaisonLastName = p.liaisonLastName ?? "",




                    DocFirstName = p.DocFirstName,

                    enrollmentsubstatus = p.enrollmentsubstatus ?? "",


                    p.PhysicianId,



                }).ToList();

                //Returning Json Data    
                var jsonResult = Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data, JsonRequestBehavior.AllowGet });
                jsonResult.MaxJsonLength = int.MaxValue;
                return jsonResult;
            }
        }
        public async Task<PartialViewResult> SurveyResultAsyncID(int Id, int PatientId)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(HelperExtensions.apiurl);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                //Get List of Questions According to Survey Id
                string apiParameters = "";

                apiParameters = "Id=" + Id;
                HttpResponseMessage GetSurveyByPatientId = await client.GetAsync(HelperExtensions.PatientSurvey + "?" + apiParameters);
                PatientSurveryViewModel patientSurvey = new PatientSurveryViewModel();

                if (GetSurveyByPatientId.IsSuccessStatusCode)
                {
                    var results = GetSurveyByPatientId.Content.ReadAsStringAsync().Result;
                    patientSurvey = JsonConvert.DeserializeObject<PatientSurveryViewModel>(results);
                }


                //HttpResponseMessage GetSurveyAnswerById = await client.GetAsync(HelperExtensions.GetSurveyAnswerById);
                //List<SurveyAnswer> surveyAnswerLst = new List<SurveyAnswer>();
                //if (GetSurveyAnswerById.IsSuccessStatusCode)
                //{
                //    var results = GetSurveyAnswerById.Content.ReadAsStringAsync().Result;
                //    surveyAnswerLst = JsonConvert.DeserializeObject<List<SurveyAnswer>>(results);
                //}

                //foreach(var item1  in patientSurvey.patientSurveryQA)
                //    {
                //        var givenansid = item1.AnswerGivenId.Split(',');
                //        var a = surveyAnswerLst.Where(x => x.Id.ToString() == item1.AnswerGivenId).Select(x => x.AnswerText).FirstOrDefault();
                //        item1.AnswerGivenId = a;
                //    }

                var patient = _db.Patients.Where(x => x.Id == PatientId).FirstOrDefault();
                patientSurvey.PatientName = patient.FirstName + " " + patient.LastName;
                patientSurvey.DOB = patient.BirthDate.ToString("d");
                return PartialView("SurveyResult", patientSurvey);
            }

            // return PartialView("SurveyResult", new PatientSurveryViewModel());
        }

        //public async Task<PartialViewResult> SurveyResultAsync(int PatientId, int? SurveryId, int? SurveySectionId, int? SurveyTypeId)
        //{


        //    using (var client = new HttpClient())
        //    {
        //        client.BaseAddress = new Uri(HelperExtensions.apiurl);
        //        client.DefaultRequestHeaders.Accept.Clear();
        //        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        //        //Get List of Questions According to Survey Id
        //        string apiParameters = "";
        //        if (PatientId > 0)
        //        {
        //            apiParameters = "patientId="+PatientId+"&surveyId="+(SurveryId ??0)+"&surveySectionId="+(SurveySectionId ??0)+"&surveyTypeId="+(SurveyTypeId ??0);
        //            HttpResponseMessage GetSurveyByPatientId = await client.GetAsync(HelperExtensions.GetSurveyByPatientId + "?" + apiParameters);
        //            PatientSurveryViewModel patientSurvey = new PatientSurveryViewModel();
        //            if (GetSurveyByPatientId.IsSuccessStatusCode)
        //            {
        //                var results = GetSurveyByPatientId.Content.ReadAsStringAsync().Result;
        //                patientSurvey = JsonConvert.DeserializeObject< PatientSurveryViewModel > (results);
        //            }
        //            var patient = _db.Patients.Where(x => x.Id == PatientId).FirstOrDefault();
        //            patientSurvey.PatientName = patient.FirstName + " " + patient.LastName;
        //            patientSurvey.DOB = patient.BirthDate.ToString("d");
        //            return PartialView("SurveyResult", patientSurvey);


        //        }

        //    }

        //    return PartialView("SurveyResult",new PatientSurveryViewModel());
        //}
        [HttpPost]
        public ActionResult LoadDrugDataSales(string status, string userId, string date1, string substatus, DateTime? DateFrom, DateTime? DateTo, string SearchCol = "", int LiaisonId = 0, int PhysicianID = 0, int PhysicianGroupID = 0)
        {
            DateTime? date = null;
            if (date1 != "")
            {
                try
                {
                    date = Convert.ToDateTime(date1);
                }
                catch (Exception)
                {


                }


            }
            var draw = Request.Form.GetValues("draw")?.FirstOrDefault();
            var start = Request.Form.GetValues("start")?.FirstOrDefault();
            var length = Request.Form.GetValues("length")?.FirstOrDefault();
            var sortColumn = Request.Form.GetValues("columns[" + Request.Form.GetValues("order[0][column]")?.FirstOrDefault() + "][name]")?.FirstOrDefault();
            var sortColumnDir = Request.Form.GetValues("order[0][dir]")?.FirstOrDefault();
            string searchValue = Request.Form.GetValues("search[value]")?.FirstOrDefault();


            //Paging Size (10,20,50,100)    
            int pageSize = length != null ? Convert.ToInt32(length) : 0;
            int skip = start != null ? Convert.ToInt32(start) : 0;
            int recordsTotal = 0;

            var user = _db.Users.Find(GetUserId());
            //var user = string.IsNullOrEmpty(userId)
            //    ? _db.Users.Find(GetUserId())
            //    : _db.Users.Find(userId);
            var today = DateTime.Today;
            using (ApplicationdbContect _db = new ApplicationdbContect())
            {

                _db.Configuration.ProxyCreationEnabled = false;
                _db.Database.CommandTimeout = 180;
                List<int> physicianids = new List<int>();

                if (user.Role == "Sales")
                {
                    var physicianidgroupids = _db.physicianGroup_SalesStaff_Mappings.AsNoTracking().Where(x => x.SaleStaffId == user.CCMid).Select(x => x.PhysiciansGroupId).ToList();
                    physicianids = _db.physicianGroup_Physician_Mappings.Where(x => physicianidgroupids.Contains(x.PhysiciansGroupId)).Select(x => x.PhysicianId).ToList();

                }

                var patients = _db.Patients.AsNoTracking().Where(x => x.EnrollmentStatus != "Active Enrolled" && x.PhysicianId != null && physicianids.Contains(x.PhysicianId.Value)).AsQueryable();
                var dataView = (from p in patients.AsNoTracking()


                                select new
                                {
                                    FirstName = p.FirstName + " " + p.LastName,
                                    p.LastName,
                                    p.Id,
                                    p.Cycle,
                                    p.BirthDate,

                                    Gender = p.Gender ?? "",
                                    PreferredLanguage = p.PreferredLanguage ?? "",

                                    EnrollmentStatus = p.EnrollmentStatus ?? "",


                                    DocFirstName = p.Physician.FirstName + " " + p.Physician.LastName,

                                    enrollmentsubstatus = p.EnrollmentSubStatus ?? "",

                                    p.PhysicianId,
                                    p.Physician.MainPhoneNumber,

                                }).ToList();

                // dataView = dataView.Where(p => physicianids.Contains(p.PhysicianId.Value)).ToList();



                if (PhysicianID > 0)
                {
                    dataView = dataView.Where(p => p.PhysicianId == PhysicianID).ToList();
                }
                if (PhysicianGroupID > 0)
                {
                    physicianids = _db.physicianGroup_Physician_Mappings.AsNoTracking().Where(x => x.PhysiciansGroupId == PhysicianGroupID).Select(x => x.PhysicianId).ToList();
                    dataView = dataView.Where(p => physicianids.Contains(p.PhysicianId.Value)).ToList();

                }

                if (sortColumn == "")
                {
                    sortColumn = "Id";
                }
                //SORT
                if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
                {
                    dataView = dataView.OrderBy(sortColumn + " " + sortColumnDir).ToList();
                }

                //Search  
                try
                {
                    if (!string.IsNullOrEmpty(searchValue) && !string.IsNullOrWhiteSpace(searchValue))
                    {
                        bool isDateTimeSearch = false;
                        try
                        {
                            var searcvaluedate = Convert.ToDateTime(searchValue);
                            if (searcvaluedate != null)
                            {
                                isDateTimeSearch = true;
                                dataView = dataView.Where(p => p.BirthDate.ToString("MM-dd-yyyy") == searcvaluedate.ToString("MM-dd-yyyy")).ToList();
                            }
                        }
                        catch (Exception)
                        {


                        }
                        // Apply search   
                        if (isDateTimeSearch == false)
                        {
                            dataView = dataView.Where(p => p.FirstName.ToString().ToLower().Contains(searchValue.ToLower()) ||
                                                                               //p.LastName.ToLower().Contains(searchValue.ToLower()) ||
                                                                               p.EnrollmentStatus.ToLower() == (searchValue.ToLower()) ||
                                                                               p.DocFirstName.ToLower().Contains(searchValue.ToLower()) ||
                                                                               //p.DocLastName.ToLower().Contains(searchValue.ToLower()) ||

                                                                               p.Gender.ToLower() == (searchValue.ToLower()) ||
                                                                               p.Cycle.ToString().ToLower().Contains(searchValue.ToLower()) ||
                                                                               p.PreferredLanguage.ToLower().Contains(searchValue.ToLower()) ||

                                                                                p.enrollmentsubstatus.ToLower() == searchValue.ToLower() ||

                                                                                p.Id.ToString().ToLower().Contains(searchValue.ToLower())
                                       ).ToList();
                        }


                    }


                }
                catch (Exception)
                {


                }



                //total number of rows count     
                recordsTotal = dataView.Count();
                //Paging     
                if (pageSize == -1)
                {
                    pageSize = recordsTotal;
                }
                var data = dataView.Skip(skip).Take(pageSize).ToList().Select(p => new
                {
                    p.FirstName,

                    p.Id,
                    p.Cycle,

                    BirthDate = p.BirthDate == null ? "" : p.BirthDate.ToString("MM/dd/yyyy"),
                    p.Gender,
                    p.PreferredLanguage,


                    EnrollmentStatus = p.EnrollmentStatus ?? "",



                    DocFirstName = p.DocFirstName,

                    enrollmentsubstatus = p.enrollmentsubstatus ?? "",




                    p.PhysicianId



                }).ToList();

                //Returning Json Data    
                var jsonResult = Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data, JsonRequestBehavior.AllowGet });
                jsonResult.MaxJsonLength = int.MaxValue;
                return jsonResult;
            }



        }

        public PartialViewResult PatientNotes(int patientId)
        {



            return PartialView(_db.PatientNotes.AsNoTracking().Where(x => x.PatientId == patientId).OrderByDescending(x => x.CreatedOn).ToList());
        }
        [HttpPost]
        public PartialViewResult GetImportantNotes(int PatientID)
        {
            var patientnotes = _db.PatientNotes.AsNoTracking().Where(x => x.PatientId == PatientID && x.isToShowinPopup == true).OrderByDescending(x => x.CreatedOn).ToList();
            return PartialView(patientnotes);
        }

        [HttpPost]
        public bool SetImportantNotes(int[] notesIds, string[] checkboxvalues)
        {
            for (int i = 0; i <= notesIds.Count() - 1; i++)
            {
                var id = notesIds[i];
                var patientnote = _db.PatientNotes.Where(x => x.Id == id).FirstOrDefault();
                patientnote.isToShowinPopup = HelperExtensions.ToBoolean(checkboxvalues[i]);
                _db.Entry(patientnote).State = EntityState.Modified;


            }
            _db.SaveChanges();
            return true;
        }

        [HttpPost]
        public bool AddNotes(int patientId, string Notes, string Module)
        {
            PatientNotes patientNotes = new PatientNotes();
            patientNotes.CreatedBy = GetUserId();
            patientNotes.CreatedOn = DateTime.Now;
            patientNotes.Notes = Notes;
            patientNotes.PatientId = patientId;
            patientNotes.Module = Module;
            _db.PatientNotes.Add(patientNotes);
            _db.SaveChanges();
            return true;
        }
        public List<List<T>> GetAllCombos<T>(List<T> list)
        {
            int comboCount = (int)Math.Pow(2, list.Count) - 1;
            List<List<T>> result = new List<List<T>>();
            for (int i = 1; i < comboCount + 1; i++)
            {
                // make each combo here
                result.Add(new List<T>());
                for (int j = 0; j < list.Count; j++)
                {
                    if ((i >> j) % 2 != 0)
                        result.Last().Add(list[j]);
                }
            }
            return result;
        }
        [Authorize(Roles = "Liaison, Admin, QAQC, PhysiciansGroup, LiaisonGroup,Sales")]
        public async Task<PartialViewResult> _ShareCarePlanViewAll(int id)
        {
            var cycles = (from p in _db.Patients
                          join bc in _db.BillingCycles on p.Id equals bc.PatientId

                          join fcp in _db.FinalCarePlanNotes on p.Id equals fcp.PatientId


                          where p.Id == id

                          select new CarePlanforButtons
                          {
                              Cycle = fcp.Cycle,
                              CreatedMonth = fcp.CarePlanCreatedOn



                          }).Distinct().ToList();
            ViewBag.Cycles = cycles;
            var results = _db.carePlanSharedHistories.AsNoTracking().Where(x => x.PatientId == id).OrderByDescending(x => x.CreatedOn).ToList().Select(x => new ShareCarePlanViewAll
            {
                Cycle = x.Cycle,
                EmailAddress = x.EmailSentTo,
                Name = x.ReceiverName,
                sharedby = x.CreatedBy,
                shareddate = x.CreatedOn,
                PatientId = x.PatientId

            }).ToList().OrderByDescending(x => x.shareddate).ToList();
            return PartialView(results);
        }

        [Authorize(Roles = "Liaison, Admin, QAQC, PhysiciansGroup, LiaisonGroup,Sales")]
        public async Task<PartialViewResult> _ShareCarePlanView(int id, int cycle = 0)
        {
            var cycles = (from p in _db.Patients
                          join bc in _db.BillingCycles on p.Id equals bc.PatientId

                          join fcp in _db.FinalCarePlanNotes on p.Id equals fcp.PatientId


                          where p.Id == id

                          select new CarePlanforButtons
                          {
                              Cycle = fcp.Cycle,
                              CreatedMonth = fcp.CarePlanCreatedOn



                          }).Distinct().OrderByDescending(x => x.Cycle).ToList();
            ViewBag.Cycles = cycles;
            List<ShareCarePlanView> shareCarePlanViews = new List<ShareCarePlanView>();
            if (cycle == 0)
            {
                foreach (var cycleitem in cycles)
                {

                    var alreadyshared = _db.carePlanSharedHistories.AsNoTracking().Where(x => x.PatientId == id && x.Cycle == cycleitem.Cycle).ToList().OrderByDescending(x => x.CreatedOn).ToList();

                    var patient = _db.Patients.Find(id);
                    if (!string.IsNullOrEmpty(patient.Email))
                    {
                        var alreadysent = false;
                        var alreadyshareditem = alreadyshared.Where(x => x.EmailSentTo == patient.Email).FirstOrDefault();
                        if (alreadyshareditem != null)
                        {
                            alreadysent = true;
                        }
                        shareCarePlanViews.Add(new ShareCarePlanView
                        {
                            EmailAddress = patient.Email,
                            Name = patient.FirstName + " " + patient.LastName,
                            Relation = "",
                            AlreadySentEmail = alreadysent,
                            Cycle = cycleitem.Cycle,
                            PatientId = id,
                            ReceiverId = id,
                            ReceiverType = "Patient",
                            sharingHistories = alreadyshared.Where(x => x.EmailSentTo == patient.Email && x.ReceiverId == id && x.ReceiverType == "Patient").ToList().Select(x => new SharingHistory { sharedby = x.CreatedBy, shareddate = x.CreatedOn, EmailAddress = x.EmailSentTo }).ToList(),
                            SectionName = "Patient"

                        });
                    }
                    var doctor = _db.Physicians.Find(patient.PhysicianId);
                    if (!string.IsNullOrEmpty(doctor.Email))
                    {
                        var alreadysent = false;
                        var alreadyshareditem = alreadyshared.Where(x => x.EmailSentTo == doctor.Email).FirstOrDefault();
                        if (alreadyshareditem != null)
                        {
                            alreadysent = true;
                        }
                        shareCarePlanViews.Add(new ShareCarePlanView
                        {
                            EmailAddress = doctor.Email,
                            Name = doctor.FirstName + " " + doctor.LastName,
                            Relation = "",
                            AlreadySentEmail = alreadysent,
                            Cycle = cycleitem.Cycle,
                            sharingHistories = alreadyshared.Where(x => x.EmailSentTo == doctor.Email && x.ReceiverId == doctor.Id && x.ReceiverType == "Physician").ToList().Select(x => new SharingHistory { sharedby = x.CreatedBy, shareddate = x.CreatedOn, EmailAddress = x.EmailSentTo }).ToList(),
                            PatientId = id,
                            ReceiverId = doctor.Id,
                            ReceiverType = "Physician",
                            SectionName = "Physician"

                        });
                    }


                    var additionalproviders = _db.SecondaryDoctors.AsNoTracking().Where(x => x.PatientId == id && x.Email != null && x.Email != "" && x.IsShareCarePlan == true).ToList();
                    foreach (var item in additionalproviders)
                    {
                        var alreadysent = false;
                        var alreadyshareditem = alreadyshared.Where(x => x.EmailSentTo == item.Email).FirstOrDefault();
                        if (alreadyshareditem != null)
                        {
                            alreadysent = true;
                        }
                        shareCarePlanViews.Add(new ShareCarePlanView
                        {
                            EmailAddress = item.Email,
                            Name = item.FullName,
                            Relation = "",
                            AlreadySentEmail = alreadysent,
                            Cycle = cycleitem.Cycle,
                            PatientId = id,
                            ReceiverId = item.Id,
                            ReceiverType = "Additional Provider",
                            sharingHistories = alreadyshared.Where(x => x.EmailSentTo == item.Email && x.ReceiverId == item.Id && x.ReceiverType == "Additional Provider").ToList().Select(x => new SharingHistory { sharedby = x.CreatedBy, shareddate = x.CreatedOn, EmailAddress = x.EmailSentTo }).ToList(),
                            SectionName = "Additional Provider(s)"

                        });
                    }
                    var urgencycontacts = _db.PatientProfile_UrgencyContacts.AsNoTracking().Where(x => x.PatientId == id).ToList();
                    if (urgencycontacts.Count > 0)
                    {
                        foreach (var urgencycontact in urgencycontacts)
                        {
                            if (!string.IsNullOrEmpty(urgencycontact.PrimaryEmail))
                            {
                                if (urgencycontact.PrimaryIsShareCarePlan == true)
                                {
                                    var alreadysent = false;
                                    var alreadyshareditem = alreadyshared.Where(x => x.EmailSentTo == urgencycontact.PrimaryEmail).FirstOrDefault();
                                    if (alreadyshareditem != null)
                                    {
                                        alreadysent = true;
                                    }
                                    shareCarePlanViews.Add(new ShareCarePlanView
                                    {
                                        EmailAddress = urgencycontact.PrimaryEmail,
                                        Name = urgencycontact.PrimaryName + " (" + urgencycontact.PrimaryRelationship + ")",
                                        Relation = "",
                                        AlreadySentEmail = alreadysent,
                                        Cycle = cycleitem.Cycle,
                                        PatientId = id,
                                        ReceiverId = urgencycontact.Id,
                                        ReceiverType = "Urgency Contact",
                                        sharingHistories = alreadyshared.Where(x => x.EmailSentTo == urgencycontact.PrimaryEmail && x.ReceiverId == urgencycontact.Id && x.ReceiverType == "Urgency Contact").ToList().Select(x => new SharingHistory { sharedby = x.CreatedBy, shareddate = x.CreatedOn, EmailAddress = x.EmailSentTo }).ToList(),
                                        SectionName = "Urgency Contact(s)"

                                    });
                                }
                            }
                        }

                    }

                }
                return PartialView("_ShareCarePlanViewAll", shareCarePlanViews);
            }
            else
            {
                var alreadyshared = _db.carePlanSharedHistories.AsNoTracking().Where(x => x.PatientId == id && x.Cycle == cycle).ToList().OrderByDescending(x => x.CreatedOn).ToList();

                var patient = _db.Patients.Find(id);
                if (!string.IsNullOrEmpty(patient.Email))
                {
                    var alreadysent = false;
                    var alreadyshareditem = alreadyshared.Where(x => x.EmailSentTo == patient.Email).FirstOrDefault();
                    if (alreadyshareditem != null)
                    {
                        alreadysent = true;
                    }
                    shareCarePlanViews.Add(new ShareCarePlanView
                    {
                        EmailAddress = patient.Email,
                        Name = patient.FirstName + " " + patient.LastName,
                        Relation = "",
                        AlreadySentEmail = alreadysent,
                        Cycle = cycle,
                        PatientId = id,
                        ReceiverId = id,
                        ReceiverType = "Patient",
                        sharingHistories = alreadyshared.Where(x => x.EmailSentTo == patient.Email && x.ReceiverId == id && x.ReceiverType == "Patient").ToList().Select(x => new SharingHistory { sharedby = x.CreatedBy, shareddate = x.CreatedOn, EmailAddress = x.EmailSentTo }).ToList(),
                        SectionName = "Patient"

                    });
                }
                var doctor = _db.Physicians.Find(patient.PhysicianId);
                if (!string.IsNullOrEmpty(doctor.Email))
                {
                    var alreadysent = false;
                    var alreadyshareditem = alreadyshared.Where(x => x.EmailSentTo == doctor.Email).FirstOrDefault();
                    if (alreadyshareditem != null)
                    {
                        alreadysent = true;
                    }
                    shareCarePlanViews.Add(new ShareCarePlanView
                    {
                        EmailAddress = doctor.Email,
                        Name = doctor.FirstName + " " + doctor.LastName,
                        Relation = "",
                        AlreadySentEmail = alreadysent,
                        Cycle = cycle,
                        sharingHistories = alreadyshared.Where(x => x.EmailSentTo == doctor.Email && x.ReceiverId == doctor.Id && x.ReceiverType == "Physician").ToList().Select(x => new SharingHistory { sharedby = x.CreatedBy, shareddate = x.CreatedOn, EmailAddress = x.EmailSentTo }).ToList(),
                        PatientId = id,
                        ReceiverId = doctor.Id,
                        ReceiverType = "Physician",
                        SectionName = "Physician"

                    });
                }


                var additionalproviders = _db.SecondaryDoctors.AsNoTracking().Where(x => x.PatientId == id && x.Email != null && x.Email != "" && x.IsShareCarePlan == true).ToList();
                foreach (var item in additionalproviders)
                {
                    var alreadysent = false;
                    var alreadyshareditem = alreadyshared.Where(x => x.EmailSentTo == item.Email).FirstOrDefault();
                    if (alreadyshareditem != null)
                    {
                        alreadysent = true;
                    }
                    shareCarePlanViews.Add(new ShareCarePlanView
                    {
                        EmailAddress = item.Email,
                        Name = item.FullName,
                        Relation = "",
                        AlreadySentEmail = alreadysent,
                        Cycle = cycle,
                        PatientId = id,
                        ReceiverId = item.Id,
                        ReceiverType = "Additional Provider",
                        sharingHistories = alreadyshared.Where(x => x.EmailSentTo == item.Email && x.ReceiverId == item.Id && x.ReceiverType == "Additional Provider").ToList().Select(x => new SharingHistory { sharedby = x.CreatedBy, shareddate = x.CreatedOn, EmailAddress = x.EmailSentTo }).ToList(),
                        SectionName = "Additional Provider(s)"

                    });
                }
                var urgencycontacts = _db.PatientProfile_UrgencyContacts.AsNoTracking().Where(x => x.PatientId == id).ToList();
                if (urgencycontacts.Count > 0)
                {
                    foreach (var urgencycontact in urgencycontacts)
                    {
                        if (!string.IsNullOrEmpty(urgencycontact.PrimaryEmail))
                        {
                            if (urgencycontact.PrimaryIsShareCarePlan == true)
                            {
                                var alreadysent = false;
                                var alreadyshareditem = alreadyshared.Where(x => x.EmailSentTo == urgencycontact.PrimaryEmail).FirstOrDefault();
                                if (alreadyshareditem != null)
                                {
                                    alreadysent = true;
                                }
                                shareCarePlanViews.Add(new ShareCarePlanView
                                {
                                    EmailAddress = urgencycontact.PrimaryEmail,
                                    Name = urgencycontact.PrimaryName + " (" + urgencycontact.PrimaryRelationship + ")",
                                    Relation = "",
                                    AlreadySentEmail = alreadysent,
                                    Cycle = cycle,
                                    PatientId = id,
                                    ReceiverId = urgencycontact.Id,
                                    ReceiverType = "Urgency Contact",
                                    sharingHistories = alreadyshared.Where(x => x.EmailSentTo == urgencycontact.PrimaryEmail && x.ReceiverId == urgencycontact.Id && x.ReceiverType == "Urgency Contact").ToList().Select(x => new SharingHistory { sharedby = x.CreatedBy, shareddate = x.CreatedOn, EmailAddress = x.EmailSentTo }).ToList(),
                                    SectionName = "Urgency Contact(s)"

                                });
                            }
                        }
                    }

                }
                return PartialView(shareCarePlanViews);
            }



        }
        [HttpGet]
        [Authorize(Roles = "Liaison, Admin, QAQC, PhysiciansGroup, LiaisonGroup,Sales")]
        public async Task<ActionResult> Details(int? id, string status = "", bool forcareplan = false, int cycle = 0, string HideDataforCallReceive = "No")
        {
            ViewBag.HideDataForCallReceive = HideDataforCallReceive;
            List<List<SurveyAnswer>> combos = GetAllCombos(new SurveyAnswer[] {
                new SurveyAnswer{ AnswerText="Answer 1",Id=1},new SurveyAnswer{ AnswerText="Answer 2",Id=2},new SurveyAnswer{ AnswerText="Answer 3",Id=3}
                 }.ToList());
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var patient = _db.Patients.Find(id);

            if (patient == null)
            {
                return HttpNotFound();
            }
            //if (patient.LiaisonId == null && User.IsInRole("Liaison"))
            //{
            //    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            //}
            //else
            //{
            //    if(patient.LifeStressId !=User.)
            //}


            var physician = await _db.Physicians.FindAsync(patient.PhysicianId);
            var physicians = _db.Physicians.Select(p => new SelectListItem { Value = p.Id.ToString(), Text = p.FirstName + " " + p.LastName });
            var chroniccondition1 = _db.Patient_ChronicCondition1s?.Select(p => new SelectListItem { Value = p.Id.ToString(), Text = p.ChronicCondition1Type });
            var chroniccondition2 = _db.Patient_ChronicCondition2s?.Select(p => new SelectListItem { Value = p.Id.ToString(), Text = p.ChronicCondition2Type });
            ViewBag.Msg = status;
            ViewBag.PhysicianName = physician?.FirstName + " " + physician?.LastName;
            ViewBag.Physicians = new SelectList(physicians.OrderBy(p => p.Text), "Value", "Text", patient.PhysicianId);
            ViewBag.PatientChronicCondition1 = new SelectList(chroniccondition1?.OrderBy(cc => cc.Text), "Value", "Text", patient.PatientChronicCondition1Id);
            ViewBag.PatientChronicCondition2 = new SelectList(chroniccondition2?.OrderByDescending(cc => cc.Text), "Value", "Text", patient.PatientChronicCondition2Id);
            ViewBag.EnrolledCcmCategories = _db.Patients_BillingCategories.Where(x => x.PatientId == patient.Id && x.Status == true).Select(p => p.BillingCategory).Distinct().ToList();

            ViewBag.PatientName = patient.FirstName + " " + patient.LastName;
            ViewBag.PatientId = patient.Id;
            CategoryCycleStatusHelper.User = User;
            var ccmcyclestatus = CategoryCycleStatusHelper.GetPatientNewOrOldCycleStatusbyCategory(patient.Id, BillingCodeHelper.cmmBillingCatagoryid,null);
            ViewBag.ccmcyclestatus = ccmcyclestatus;
            ViewBag.CcmStatus = patient.CcmStatus;
            ViewBag.EnrollmentStatus = new SelectList(_db.EnrollmentStatuss.ToList(), "Id", "Name", patient.EnrollmentStatus);
            ViewBag.EnrollmentSubStatus = new SelectList(_db.EnrollmentSubStatuss.ToList(), "EnrollmentStatusID", "Name");
            ViewBag.EnrollemntStatusReson = new SelectList(_db.EnrollmentSubstatusReasons.ToList(), "Name", "Name");
            ViewBag.ForCareplan = forcareplan;
            ViewBag.CycleforCarePlan = cycle;
            int? PreliasonId = 0;
            int? PreTranslaterId = 0;
            try
            {
                PreliasonId = patient.Patients_PreLiaisons.LiaisonId;
            }
            catch { }
            try
            {
                PreTranslaterId = patient.Patients_PreLiaisons.TranslatorId;
            }
            catch { }

            var PreLiaison = _db.Liaisons.Where(x => x.Id == PreliasonId).FirstOrDefault();
            var PreTranslater = _db.Liaisons.Where(x => x.Id == PreliasonId).FirstOrDefault();
            ViewBag.PreLiaison = PreLiaison;
            ViewBag.PreTranslater = PreTranslater;
            var liasion = _db.Liaisons.AsNoTracking().Where(x => x.Id == patient.LiaisonId).FirstOrDefault();
            if (liasion != null)
            {
                ViewBag.CallerId = liasion.TwilioCallerId ?? "";
                ViewBag.LiasionName = liasion.FirstName + liasion.LastName;
            }
            //if (User.IsInRole("Liaison"))
            ViewBag.ReviewId = HelperExtensions.ReviewTimeGet("Patient Details", patient.Id, GetUserId());
            ViewBag.ReviewIdCCM = ViewBag.ReviewId;
            /*
            string userEmail = "vgrullon@aol.com";
            string uid = "d7926557-3642-4454-8d61-f7caa790e9bf";
            string newPassword = "mega*76";
            //var userManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();


            var user = await UserManager.FindByNameAsync(userEmail);
            if (user.Id == "Liaison")
            {
                int nn = 123;
            }
               
            if (user.PasswordHash != null)
            {
                UserManager.RemovePassword(user.Id);
            }

            UserManager.AddPassword(user.Id, newPassword);
            */

            //ApplicationUser au = await userManager.FindByNameAsync(userName); 


            /*
            var code = await userManager.GeneratePasswordResetTokenAsync(userName);
            var result2 = await userManager.ResetPasswordAsync(userName, code, newPassword);
            if (!result2.Succeeded)
            {
                //password does not meet standards
                int nnn = 2;
            }
            int nn = 1;
            */

            //UserManager<IdentityUser> userManager = new UserManager<IdentityUser>(new UserStore<IdentityUser>());

            //userManager.RemovePassword(userName);
            //userManager.AddPassword(userName, newPassword);
            //ApplicationUser user =  UserManager.FindById("029995b5-c7f2-4262-b2bd-36574d4402e1");
            //if (user == null)
            //{

            //}
            //user.PasswordHash = UserManager.PasswordHasher.HashPassword("Bbc&123");
            //var result =  UserManager.Update(user);
            //if (!result.Succeeded)
            //{
            //    //throw exception......
            //}
            //old Code
            //var CounsolersList = (from l in _db.Liaisons
            //                      join Pc in _db.Patients_BillingCategories on l.Id equals Pc.LiaisonId
            //                      join B in _db.BillingCategories on Pc.BillingCategoryId equals B.BillingCategoryId
            //                      where Pc.PatientId == id && Pc.IsTranslator == false && Pc.Status == true
            //                      select B.Name + "-" + l.FirstName).ToList();
            //new Code
            var CounsolersList = (from l in _db.Liaisons
                                  join Pc in _db.Patients_BillingCategories on l.Id equals Pc.LiaisonId
                                  join B in _db.BillingCategories on Pc.BillingCategoryId equals B.BillingCategoryId
                                  where Pc.PatientId == id && Pc.LiaisonId != null && Pc.Status == true
                                  select B.Name + "-" + l.FirstName).ToList();
            ViewBag.Counselors = CounsolersList;
            if (CounsolersList.Count == 0 & PreLiaison != null)
            {
                if (PreLiaison != null)
                {

                    ViewBag.PreCounsolerName = PreLiaison.FirstName + " " + PreLiaison.LastName;
                }
                else
                {
                    ViewBag.PreCounsolerName = "Not Assigned";
                }
            }


            return View(patient);
        }

        [HttpPost, ValidateInput(false)]
        [Authorize(Roles = "Liaison, Admin")]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> Details([Bind(Exclude = "Photo,PhotoEmrRecords,PhotoEmrRecords2,PhotoEmrRecords3,PhotoEmrRecords4,PhotoEmrRecords5,PhotoEmrRecords6,PhotoEmrRecords7,PhotoEmrRecords8,PhotoEmrRecords9,PhotoEmrRecords10,PhotoEmrRecords11,PhotoEmrRecords12,PhotoEmrRecords13,PhotoEmrRecords14,PhotoEmrRecords15,EnrollmentStatus,EnrollmentSubStatus,EnrollmentSubStatusReason")] Patient patient, string EnrollmentStatushiden, string EnrollmentSubStatushiden, string EnrollmentSubStatusReasonhiden)
        {
            //if (HelperExtensions.isAllowedforEditingorAdd(patient.Id, HelperExtensions.GetCCMCycle(patient.Id), GetUserId()) == false)
            //{
            //    return RedirectToAction("Index", "CcmStatus", new { status = HelperExtensions.GetStatusRedirectionbyUser(GetUserId()), Message = "Cycle is locked." });

            //}
            if (ModelState.IsValid)
            {

                patient.EnrollmentStatus = EnrollmentStatushiden == "" ? null : EnrollmentStatushiden;
                patient.EnrollmentSubStatus = EnrollmentSubStatushiden == "" ? null : EnrollmentSubStatushiden;
                if (patient?.EnrollmentSubStatus == "In-Active Enrolled")
                {
                    patient.EnrollmentSubStatusReason = EnrollmentSubStatusReasonhiden;
                }
                else
                {
                    patient.EnrollmentSubStatusReason = "";
                }

                var postedImageFile = Request.Files["Photo"];
                var postedEmrImage = Request.Files["PhotoEmrRecords"];
                var postedEmrImage2 = Request.Files["PhotoEmrRecords2"];
                var postedEmrImage3 = Request.Files["PhotoEmrRecords3"];
                var postedEmrImage4 = Request.Files["PhotoEmrRecords4"];
                var postedEmrImage5 = Request.Files["PhotoEmrRecords5"];
                var postedEmrImage6 = Request.Files["PhotoEmrRecords6"];
                var postedEmrImage7 = Request.Files["PhotoEmrRecords7"];
                var postedEmrImage8 = Request.Files["PhotoEmrRecords8"];
                var postedEmrImage9 = Request.Files["PhotoEmrRecords9"];
                var postedEmrImage10 = Request.Files["PhotoEmrRecords10"];
                var postedEmrImage11 = Request.Files["PhotoEmrRecords11"];
                var postedEmrImage12 = Request.Files["PhotoEmrRecords12"];
                var postedEmrImage13 = Request.Files["PhotoEmrRecords13"];
                var postedEmrImage14 = Request.Files["PhotoEmrRecords14"];
                var postedEmrImage15 = Request.Files["PhotoEmrRecords15"];

                if (postedImageFile?.ContentLength != 0 && postedImageFile?.InputStream != null)
                    using (var binary = new BinaryReader(postedImageFile.InputStream))
                    {
                        var imageData = binary.ReadBytes(postedImageFile.ContentLength);
                        if (imageData.Length > 0)
                            patient.Photo = imageData;
                    }

                else
                {
                    var patientCopy = await _db.Patients.AsNoTracking().FirstOrDefaultAsync(p => p.Id == patient.Id);
                    if (patientCopy?.Photo != null)
                        patient.Photo = patientCopy.Photo;
                }


                if (postedEmrImage?.ContentLength != 0 && postedEmrImage?.InputStream != null)
                    using (var binaryEmr = new BinaryReader(postedEmrImage.InputStream))
                    {
                        var emrImageData = binaryEmr.ReadBytes(postedEmrImage.ContentLength);
                        if (emrImageData.Length > 0)
                            patient.PhotoEmrRecords = emrImageData;
                    }
                else
                {
                    var patientCopy = await _db.Patients.AsNoTracking().FirstOrDefaultAsync(p => p.Id == patient.Id);
                    if (patientCopy?.PhotoEmrRecords != null)
                        patient.PhotoEmrRecords = patientCopy.PhotoEmrRecords;
                }


                if (postedEmrImage2?.ContentLength != 0 && postedEmrImage2?.InputStream != null)
                    using (var binaryEmr2 = new BinaryReader(postedEmrImage2.InputStream))
                    {
                        var emrImageData2 = binaryEmr2.ReadBytes(postedEmrImage2.ContentLength);
                        if (emrImageData2.Length > 0)
                            patient.PhotoEmrRecords2 = emrImageData2;
                    }
                else
                {
                    var patientCopy = await _db.Patients.AsNoTracking().FirstOrDefaultAsync(p => p.Id == patient.Id);
                    if (patientCopy.PhotoEmrRecords2 != null)
                        patient.PhotoEmrRecords2 = patientCopy.PhotoEmrRecords2;
                }

                if (postedEmrImage3?.ContentLength != 0 && postedEmrImage3?.InputStream != null)
                    using (var binaryEmr3 = new BinaryReader(postedEmrImage3.InputStream))
                    {
                        var emrImageData3 = binaryEmr3.ReadBytes(postedEmrImage3.ContentLength);
                        if (emrImageData3.Length > 0)
                            patient.PhotoEmrRecords3 = emrImageData3;
                    }
                else
                {
                    var patientCopy = await _db.Patients.AsNoTracking().FirstOrDefaultAsync(p => p.Id == patient.Id);
                    if (patientCopy.PhotoEmrRecords3 != null)
                        patient.PhotoEmrRecords3 = patientCopy.PhotoEmrRecords3;
                }

                if (postedEmrImage4?.ContentLength != 0 && postedEmrImage4?.InputStream != null)
                    using (var binaryEmr4 = new BinaryReader(postedEmrImage4.InputStream))
                    {
                        var emrImageData4 = binaryEmr4.ReadBytes(postedEmrImage4.ContentLength);
                        if (emrImageData4.Length > 0)
                            patient.PhotoEmrRecords4 = emrImageData4;
                    }
                else
                {
                    var patientCopy = await _db.Patients.AsNoTracking().FirstOrDefaultAsync(p => p.Id == patient.Id);
                    if (patientCopy.PhotoEmrRecords4 != null)
                        patient.PhotoEmrRecords4 = patientCopy.PhotoEmrRecords4;
                }

                if (postedEmrImage5?.ContentLength != 0 && postedEmrImage5?.InputStream != null)
                    using (var binaryEmr5 = new BinaryReader(postedEmrImage5.InputStream))
                    {
                        var emrImageData5 = binaryEmr5.ReadBytes(postedEmrImage5.ContentLength);
                        if (emrImageData5.Length > 0)
                            patient.PhotoEmrRecords5 = emrImageData5;
                    }
                else
                {
                    var patientCopy = await _db.Patients.AsNoTracking().FirstOrDefaultAsync(p => p.Id == patient.Id);
                    if (patientCopy.PhotoEmrRecords5 != null)
                        patient.PhotoEmrRecords5 = patientCopy.PhotoEmrRecords5;
                }

                if (postedEmrImage6?.ContentLength != 0 && postedEmrImage6?.InputStream != null)
                    using (var binaryEmr6 = new BinaryReader(postedEmrImage6.InputStream))
                    {
                        var emrImageData6 = binaryEmr6.ReadBytes(postedEmrImage6.ContentLength);
                        if (emrImageData6.Length > 0)
                            patient.PhotoEmrRecords6 = emrImageData6;
                    }
                else
                {
                    var patientCopy = await _db.Patients.AsNoTracking().FirstOrDefaultAsync(p => p.Id == patient.Id);
                    if (patientCopy.PhotoEmrRecords6 != null)
                        patient.PhotoEmrRecords6 = patientCopy.PhotoEmrRecords6;
                }
                //NewOne
                if (postedEmrImage7?.ContentLength != 0 && postedEmrImage7?.InputStream != null)
                    using (var binaryEmr7 = new BinaryReader(postedEmrImage7.InputStream))
                    {
                        var emrImageData7 = binaryEmr7.ReadBytes(postedEmrImage7.ContentLength);
                        if (emrImageData7.Length > 0)
                            patient.PhotoEmrRecords7 = emrImageData7;
                    }
                else
                {
                    var patientCopy = await _db.Patients.AsNoTracking().FirstOrDefaultAsync(p => p.Id == patient.Id);
                    if (patientCopy.PhotoEmrRecords7 != null)
                        patient.PhotoEmrRecords7 = patientCopy.PhotoEmrRecords7;
                }
                if (postedEmrImage8?.ContentLength != 0 && postedEmrImage8?.InputStream != null)
                    using (var binaryEmr8 = new BinaryReader(postedEmrImage8.InputStream))
                    {
                        var emrImageData8 = binaryEmr8.ReadBytes(postedEmrImage8.ContentLength);
                        if (emrImageData8.Length > 0)
                            patient.PhotoEmrRecords8 = emrImageData8;
                    }
                else
                {
                    var patientCopy = await _db.Patients.AsNoTracking().FirstOrDefaultAsync(p => p.Id == patient.Id);
                    if (patientCopy.PhotoEmrRecords8 != null)
                        patient.PhotoEmrRecords8 = patientCopy.PhotoEmrRecords8;
                }
                if (postedEmrImage9?.ContentLength != 0 && postedEmrImage9?.InputStream != null)
                    using (var binaryEmr9 = new BinaryReader(postedEmrImage9.InputStream))
                    {
                        var emrImageData9 = binaryEmr9.ReadBytes(postedEmrImage9.ContentLength);
                        if (emrImageData9.Length > 0)
                            patient.PhotoEmrRecords9 = emrImageData9;
                    }
                else
                {
                    var patientCopy = await _db.Patients.AsNoTracking().FirstOrDefaultAsync(p => p.Id == patient.Id);
                    if (patientCopy.PhotoEmrRecords9 != null)
                        patient.PhotoEmrRecords9 = patientCopy.PhotoEmrRecords9;
                }
                if (postedEmrImage10?.ContentLength != 0 && postedEmrImage10?.InputStream != null)
                    using (var binaryEmr10 = new BinaryReader(postedEmrImage10.InputStream))
                    {
                        var emrImageData10 = binaryEmr10.ReadBytes(postedEmrImage10.ContentLength);
                        if (emrImageData10.Length > 0)
                            patient.PhotoEmrRecords10 = emrImageData10;
                    }
                else
                {
                    var patientCopy = await _db.Patients.AsNoTracking().FirstOrDefaultAsync(p => p.Id == patient.Id);
                    if (patientCopy.PhotoEmrRecords10 != null)
                        patient.PhotoEmrRecords10 = patientCopy.PhotoEmrRecords10;
                }
                if (postedEmrImage11?.ContentLength != 0 && postedEmrImage11?.InputStream != null)
                    using (var binaryEmr11 = new BinaryReader(postedEmrImage11.InputStream))
                    {
                        var emrImageData11 = binaryEmr11.ReadBytes(postedEmrImage11.ContentLength);
                        if (emrImageData11.Length > 0)
                            patient.PhotoEmrRecords11 = emrImageData11;
                    }
                else
                {
                    var patientCopy = await _db.Patients.AsNoTracking().FirstOrDefaultAsync(p => p.Id == patient.Id);
                    if (patientCopy.PhotoEmrRecords11 != null)
                        patient.PhotoEmrRecords11 = patientCopy.PhotoEmrRecords11;
                }
                if (postedEmrImage12?.ContentLength != 0 && postedEmrImage12?.InputStream != null)
                    using (var binaryEmr12 = new BinaryReader(postedEmrImage12.InputStream))
                    {
                        var emrImageData12 = binaryEmr12.ReadBytes(postedEmrImage12.ContentLength);
                        if (emrImageData12.Length > 0)
                            patient.PhotoEmrRecords12 = emrImageData12;
                    }
                else
                {
                    var patientCopy = await _db.Patients.AsNoTracking().FirstOrDefaultAsync(p => p.Id == patient.Id);
                    if (patientCopy.PhotoEmrRecords12 != null)
                        patient.PhotoEmrRecords12 = patientCopy.PhotoEmrRecords12;
                }
                if (postedEmrImage13?.ContentLength != 0 && postedEmrImage13?.InputStream != null)
                    using (var binaryEmr13 = new BinaryReader(postedEmrImage13.InputStream))
                    {
                        var emrImageData13 = binaryEmr13.ReadBytes(postedEmrImage13.ContentLength);
                        if (emrImageData13.Length > 0)
                            patient.PhotoEmrRecords13 = emrImageData13;
                    }
                else
                {
                    var patientCopy = await _db.Patients.AsNoTracking().FirstOrDefaultAsync(p => p.Id == patient.Id);
                    if (patientCopy.PhotoEmrRecords13 != null)
                        patient.PhotoEmrRecords13 = patientCopy.PhotoEmrRecords13;
                }
                if (postedEmrImage14?.ContentLength != 0 && postedEmrImage14?.InputStream != null)
                    using (var binaryEmr14 = new BinaryReader(postedEmrImage14.InputStream))
                    {
                        var emrImageData14 = binaryEmr14.ReadBytes(postedEmrImage14.ContentLength);
                        if (emrImageData14.Length > 0)
                            patient.PhotoEmrRecords14 = emrImageData14;
                    }
                else
                {
                    var patientCopy = await _db.Patients.AsNoTracking().FirstOrDefaultAsync(p => p.Id == patient.Id);
                    if (patientCopy.PhotoEmrRecords14 != null)
                        patient.PhotoEmrRecords14 = patientCopy.PhotoEmrRecords14;
                }
                if (postedEmrImage15?.ContentLength != 0 && postedEmrImage15?.InputStream != null)
                    using (var binaryEmr15 = new BinaryReader(postedEmrImage15.InputStream))
                    {
                        var emrImageData15 = binaryEmr15.ReadBytes(postedEmrImage15.ContentLength);
                        if (emrImageData15.Length > 0)
                            patient.PhotoEmrRecords15 = emrImageData15;
                    }
                else
                {
                    var patientCopy = await _db.Patients.AsNoTracking().FirstOrDefaultAsync(p => p.Id == patient.Id);
                    if (patientCopy.PhotoEmrRecords15 != null)
                        patient.PhotoEmrRecords15 = patientCopy.PhotoEmrRecords15;
                }
                var userId = GetUserId();
                var liaison = await _db.Liaisons.FirstOrDefaultAsync(l => l.UserId == userId);
                var physician = await _db.Physicians.FindAsync(patient.PhysicianId);
                var physicians = _db.Physicians.Select(p => new SelectListItem { Value = p.Id.ToString(), Text = p.FirstName + " " + p.LastName });
                var chroniccondition2 = _db.Patient_ChronicCondition2s?.Select(p => new SelectListItem { Value = p.Id.ToString(), Text = p.ChronicCondition2Type });
                var chroniccondition1 = _db.Patient_ChronicCondition1s?.Select(p => new SelectListItem { Value = p.Id.ToString(), Text = p.ChronicCondition1Type });

                ViewBag.Physicians = new SelectList(physicians.OrderBy(p => p.Text), "Value", "Text", patient.PhysicianId);
                ViewBag.PhysicianName = physician?.FirstName + " " + physician?.LastName;
                ViewBag.PatientChronicCondition1 = new SelectList(chroniccondition1.OrderBy(cc => cc.Text), "Value", "Text", patient.PatientChronicCondition1Id);
                ViewBag.PatientChronicCondition2 = new SelectList(chroniccondition2.OrderByDescending(cc => cc.Text), "Value", "Text", patient.PatientChronicCondition2Id);

                ViewBag.PatientName = patient.FirstName + " " + patient.LastName;
                ViewBag.PatientId = patient.Id;
                ViewBag.CcmStatus = patient.CcmStatus;
                ViewBag.EnrollmentStatus = new SelectList(_db.EnrollmentStatuss.ToList(), "Id", "Name", patient.EnrollmentStatus);
                ViewBag.EnrollmentSubStatus = new SelectList(_db.EnrollmentSubStatuss.ToList(), "EnrollmentStatusID", "Name");
                ViewBag.EnrollemntStatusReson = new SelectList(_db.EnrollmentSubstatusReasons.ToList(), "Name", "Name");
                patient.UpdatedOn = DateTime.Now;
                patient.UpdatedBy = GetUserId();
                try
                {
                    _db.Entry(patient).State = EntityState.Modified;
                    await _db.SaveChangesAsync();
                }
                catch /*(Exception ex)*/
                {


                }


                if (patient.EnrollmentSubStatus == "Active Enrolled")
                {
                    if (patient.CCMEnrolledOn == null)
                    {
                        patient.CcmStatus = "Enrolled";
                        patient.CCMEnrolledOn = DateTime.Now;
                        patient.CCMEnrolledBy = GetUserId();
                        //HelperExtensions.UpdateCurrentMonthActivityfromCycleZeroToOne(patient.Id);
                        try
                        {


                            var reviewtimeccms = _db.ReviewTimeCcms.Where(x => x.PatientId == patient.Id && x.Cycle == 0).ToList().Where(x => x.StartTime.Date.Month == DateTime.Now.Month).ToList();
                            foreach (var reviewtimeccmitem in reviewtimeccms)
                            {
                                reviewtimeccmitem.Cycle = 1;
                                _db.Entry(reviewtimeccmitem).State = EntityState.Modified;
                                _db.SaveChanges();
                            }
                        }
                        catch /*(Exception ex)*/
                        {


                        }

                    }


                    patient.LiaisonId = liaison?.Id ?? patient?.LiaisonId;
                    patient.Liaison = liaison != null ? await _db.Liaisons.FindAsync(liaison.Id) : patient.Liaison;

                    _db.Entry(patient).State = EntityState.Modified;
                    await _db.SaveChangesAsync();
                    if (!string.IsNullOrEmpty(patient.Email))
                    {
                        var existingUser = await UserManager.FindByNameAsync(patient.Email);
                        if (existingUser == null)
                        {
                            var password = patient.LastName.ToLower() + "#PA1013"; // + patient.Id;
                            var user = new ApplicationUser { UserName = patient.Email, Email = patient.Email };
                            var result = await UserManager.CreateAsync(user, password);

                            if (result.Succeeded)
                            {
                                user.Role = "Patient";
                                user.CCMid = patient.Id;
                                user.FirstName = patient.FirstName;
                                user.LastName = patient.LastName;
                                user.PhoneNumber = patient.MobilePhoneNumber;
                                await UserManager.AddToRoleAsync(user.Id, "Patient");
                                if (patient.CCMEnrolledOn == null)
                                {
                                    patient.CcmStatus = "Enrolled";
                                    patient.CCMEnrolledOn = DateTime.Now;
                                    patient.CCMEnrolledBy = GetUserId();
                                    // HelperExtensions.UpdateCurrentMonthActivityfromCycleZeroToOne(patient.Id);
                                    try
                                    {


                                        var reviewtimeccms = _db.ReviewTimeCcms.Where(x => x.PatientId == patient.Id && x.Cycle == 0).ToList().Where(x => x.StartTime.Date.Month == DateTime.Now.Month).ToList();
                                        foreach (var reviewtimeccmitem in reviewtimeccms)
                                        {
                                            reviewtimeccmitem.Cycle = 1;
                                            _db.Entry(reviewtimeccmitem).State = EntityState.Modified;
                                            _db.SaveChanges();
                                        }
                                    }
                                    catch /*(Exception ex)*/
                                    {


                                    }
                                }
                                patient.LiaisonId = liaison?.Id ?? patient?.LiaisonId;
                                patient.Liaison = liaison != null ? await _db.Liaisons.FindAsync(liaison.Id) : patient.Liaison;
                                _db.Entry(patient).State = EntityState.Modified;
                                _db.Entry(user).State = EntityState.Modified;
                                await _db.SaveChangesAsync();

                                ViewBag.Message = "CCM Enrolled. Patient Portal Created.";
                                ViewBag.Username = user.Email;
                                ViewBag.Password = password;

                                return RedirectToAction("Index", "CcmStatus", new { status = "Enrolled" });
                            }

                            TempData["PatientPhoneNumber"] = patient.MobilePhoneNumber;
                            ViewBag.Message = "Error: CCM Not Enrolled & Patient Portal Not Created!. Please, Try again.";
                            return RedirectToAction("Details", "Patient", new { id = patient.Id });
                            //return View(patient);
                        }
                        else
                        {
                            TempData["PatientPhoneNumber"] = patient.MobilePhoneNumber;
                            ViewBag.Message = "Patient-Email Already Exists! CCM Not Enrolled & Patient Portal Not Created!.";
                            return RedirectToAction("Details", "Patient", new { id = patient.Id });
                            //return View(patient);

                        }

                    }



                    //return RedirectToAction("Index", "CcmStatus", new { status = "Enrolled" });
                }
            }
            return RedirectToAction("Details", "Patient", new { id = patient.Id });
            //return View(patient);
        }

        [HttpGet]
        public async Task<PartialViewResult> _Details(int? id, string status = "")
        {
            if (id == null)
            {
                return PartialView("~/Views/Shared/_BadRequest");
                //return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ViewBag.Laisons = _db.Liaisons.Where(p => p.IsTranslator == false).Include(p => p.Liaisons_BillingCategories).Select(p => p).ToList();
            ViewBag.Translators = _db.Liaisons.Where(p => p.IsTranslator == true).Include(p => p.Liaisons_BillingCategories).Select(p => p).ToList();

            var patient = _db.Patients.Find(id);

            if (patient == null)
            {
                return PartialView("~/Views/Shared/_NotFound");
                //return HttpNotFound();
            }
            //if (patient.LiaisonId == null && User.IsInRole("Liaison"))
            //{
            //    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            //}
            //else
            //{
            //    if(patient.LifeStressId !=User.)
            //}
            List<EnrollmentListViewModel> EnrollmentList = new List<EnrollmentListViewModel>();
            EnrollmentListViewModel obj1 = new EnrollmentListViewModel();
            EnrollmentListViewModel obj2 = new EnrollmentListViewModel();
            EnrollmentListViewModel obj3 = new EnrollmentListViewModel();
            obj1.BillingcatagoryId = 9;
            obj1.EnrollmentStatus = "Enrolled";
            obj1.EnrollmentSubStatus = "Active Enrolled";
            obj1.EnrollemntStatusReson = "";
            obj2.BillingcatagoryId = 11;
            obj2.EnrollmentStatus = "De-Enrolled";
            obj2.EnrollmentSubStatus = "Refused";
            obj2.EnrollemntStatusReson = "";
            obj3.BillingcatagoryId = 13;
            obj3.EnrollmentStatus = "Enrolled";
            obj3.EnrollmentSubStatus = "In-Active Enrolled";
            obj3.EnrollemntStatusReson = "Waiting on Physician Orders";
            EnrollmentList.Add(obj1);
            EnrollmentList.Add(obj2);
            EnrollmentList.Add(obj3);

            ViewBag.EnrollmentList = EnrollmentList;
            ViewBag.EnrollementIdList = new string[] { "9", "11", "13" };

            ViewBag.BillingCategories = _db.BillingCategories.ToList();
            ViewBag.BillingCategoriesJson = _db.BillingCategories.Select(x => x.BillingCategoryId).ToList();
            //var sd=_db.Liaisons_BillingCategories.Where(x=?)
            var physician = await _db.Physicians.FindAsync(patient.PhysicianId);
            var physicians = _db.Physicians.Select(p => new SelectListItem { Value = p.Id.ToString(), Text = p.FirstName + " " + p.LastName });
            var chroniccondition1 = _db.Patient_ChronicCondition1s?.Select(p => new SelectListItem { Value = p.Id.ToString(), Text = p.ChronicCondition1Type });
            var chroniccondition2 = _db.Patient_ChronicCondition2s?.Select(p => new SelectListItem { Value = p.Id.ToString(), Text = p.ChronicCondition2Type });
            ViewBag.Msg = status;
            ViewBag.PhysicianName = physician?.FirstName + " " + physician?.LastName;
            ViewBag.Physicians = new SelectList(physicians.OrderBy(p => p.Text), "Value", "Text", patient.PhysicianId);
            ViewBag.PatientChronicCondition1 = new SelectList(chroniccondition1?.OrderBy(cc => cc.Text), "Value", "Text", patient.PatientChronicCondition1Id);
            ViewBag.PatientChronicCondition2 = new SelectList(chroniccondition2?.OrderByDescending(cc => cc.Text), "Value", "Text", patient.PatientChronicCondition2Id);

            ViewBag.PatientName = patient.FirstName + " " + patient.LastName;
            ViewBag.PatientId = patient.Id;
            //patient.Cycle = CategoryCycleStatusHelper.GetPatientNewOrOldCycleByCategory(patient.Id);

            ViewBag.CcmStatus = patient.CcmStatus;
            ViewBag.EnrollmentStatus = new SelectList(_db.EnrollmentStatuss.ToList(), "Id", "Name", patient.EnrollmentStatus);
            ViewBag.EnrollmentSubStatus = new SelectList(_db.EnrollmentSubStatuss.ToList(), "EnrollmentStatusID", "Name");
            ViewBag.EnrollemntStatusReson = new SelectList(_db.EnrollmentSubstatusReasons.ToList(), "Name", "Name");

            ViewBag.patientOldSubStatus = patient.EnrollmentSubStatus;
            int? PreliasonId = 0;
            int? PreTranslaterId = 0;
            try
            {
                PreliasonId = patient.Patients_PreLiaisons.LiaisonId;
            }
            catch { }
            try
            {
                PreTranslaterId = patient.Patients_PreLiaisons.TranslatorId;
            }
            catch { }

            var PreLiaison = _db.Liaisons.Where(x => x.Id == PreliasonId).FirstOrDefault();
            var PreTranslater = _db.Liaisons.Where(x => x.Id == PreliasonId).FirstOrDefault();
            if (PreLiaison != null)
            {
                ViewBag.PreLiaisonId = PreLiaison.Id;
            }
            else
            {
                ViewBag.PreLiaisonId = 0;
            }
            if (PreTranslater != null)
            {
                ViewBag.PreTranslaterId = PreTranslater.Id;
            }
            else
            {
                ViewBag.PreTranslaterId = 0;
            }


            var translator = _db.Liaisons.Where(x => x.IsTranslator == true).Select(p => new SelectListItem
            {
                Value = p.Id.ToString(),
                Text = p.FirstName + " " + p.LastName
            }).OrderBy(p => p.Text);
            var liason = _db.Liaisons.Where(x => x.IsTranslator == false).Select(p => new SelectListItem
            {
                Value = p.Id.ToString(),
                Text = p.FirstName + " " + p.LastName
            }).OrderBy(p => p.Text);

            ViewBag.Translator = new SelectList(translator, "Value", "Text");
            ViewBag.Liason = new SelectList(liason, "Value", "Text");


            ViewBag.EnrollmentReasons = _db.EnrollmentSubstatusReasons.ToList();


            ViewBag.RpmServices = _db.RPMServices.Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.ServiceName }).ToList();
            return PartialView(patient);
        }


        [HttpPost, ValidateInput(false)]
        [Authorize(Roles = "Liaison, Admin,Sales,LiaisonGroup")]
        //[ValidateAntiForgeryToken]
        //////////////////////////////////////////////////revert to changeset 513
        public async Task<string> _Details([Bind(Exclude = "Photo,PhotoEmrRecords,PhotoEmrRecords2,PhotoEmrRecords3,PhotoEmrRecords4,PhotoEmrRecords5,PhotoEmrRecords6,PhotoEmrRecords7,PhotoEmrRecords8,PhotoEmrRecords9,PhotoEmrRecords10,PhotoEmrRecords11,PhotoEmrRecords12,PhotoEmrRecords13,PhotoEmrRecords14,PhotoEmrRecords15,EnrollmentStatus,EnrollmentSubStatus,EnrollmentSubStatusReason")]
                Patient patient, string EnrollmentStatushiden, string EnrollmentSubStatushiden,
                     string EnrollmentSubStatusReasonhiden, string[] billingCategory, /*string EnrollmentList,*/
                     string PatientBillingCategories, string Prelaison, string Pretranslator, int[] RpmServices)
        {

            List<int> RpmServiceList = RpmServices != null ? RpmServices.ToList() : new List<int>();
            List<PatientBillingCategoryViewModelEdit> patientCategoryList = JsonConvert.DeserializeObject<List<PatientBillingCategoryViewModelEdit>>(PatientBillingCategories);


            string Msg = "";

            patient.EnrollmentStatus = EnrollmentStatushiden == "" ? null : EnrollmentStatushiden;
            patient.EnrollmentSubStatus = EnrollmentSubStatushiden == "" ? null : EnrollmentSubStatushiden;
            if (patient?.EnrollmentSubStatus == "In-Active Enrolled")
            {
                patient.EnrollmentSubStatusReason = EnrollmentSubStatusReasonhiden;
            }
            else
            {
                patient.EnrollmentSubStatusReason = null;
            }

            if (!User.IsInRole("Admin") && !User.IsInRole("LiaisonGroup"))
            {
                var OLDpatientdata = _db.Patients.AsNoTracking().Where(x => x.Id == patient.Id).FirstOrDefault();
                if ((OLDpatientdata.EnrollmentStatus == "Enrolled" && OLDpatientdata.EnrollmentSubStatus == "Active Enrolled") && (patient.EnrollmentStatus != "Enrolled" && patient.EnrollmentSubStatus != "Active Enrolled"))
                {
                    return Msg = "Not allowed to change the enrollment status";
                }
            }

            var patientpreliasion = _db.Patients.Where(p => p.Id == patient.Id).Select(p => p.Patients_PreLiaisons).FirstOrDefault();

            #region Saving Patient Pre Liaison Data
            if (Prelaison != "" || Pretranslator != "")
            {

                if (patientpreliasion != null)
                {

                    if (Prelaison != "")
                    {
                        int? prelaison = Convert.ToInt32(Prelaison);
                        patientpreliasion.LiaisonId = prelaison;
                    }
                    if (Pretranslator != "")
                    {
                        int? pretranslator = Convert.ToInt32(Pretranslator);
                        patientpreliasion.TranslatorId = pretranslator;
                    }
                    else
                    {
                        patientpreliasion.TranslatorId = null;
                    }
                    patientpreliasion.Status = true;
                    _db.Entry(patientpreliasion).State = EntityState.Modified;
                    //_db.SaveChanges();
                    patient.Patients_PreLiaisons = patientpreliasion;
                    patient.Patients_PreLiaisonsId = patientpreliasion.Id;
                }
                else
                {

                    Patients_PreLiaisons patients_Pre = new Patients_PreLiaisons();
                    patients_Pre.CreatedBy = GetUserId();
                    patients_Pre.CreatedOn = DateTime.Now;
                    if (Prelaison != "")
                    {
                        int? prelaison = Convert.ToInt32(Prelaison);
                        patients_Pre.LiaisonId = prelaison;
                    }
                    if (Pretranslator != "")
                    {
                        int? pretranslator = Convert.ToInt32(Pretranslator);
                        patients_Pre.TranslatorId = pretranslator;
                    }

                    patients_Pre.Status = true;
                    _db.Patients_PreLiaisons.Add(patients_Pre);
                    //_db.SaveChanges();
                    patient.Patients_PreLiaisons = patients_Pre;
                    patient.Patients_PreLiaisonsId = patients_Pre.Id;

                }
                if (patient.LiaisonId != null)
                {
                    patient.LiaisonId = null;
                    patient.TranslatorId = null;
                    //_db.Entry(patient).State = EntityState.Modified;
                    //await _db.SaveChangesAsync();
                }
            }
            #endregion
            patient.UpdatedOn = DateTime.Now;
            patient.UpdatedBy = GetUserId();
            _db.Entry(patient).State = EntityState.Modified;
            await _db.SaveChangesAsync();

            #region if EnrollmentSubStatus == "Active Enrolled"
            if (patient.EnrollmentSubStatus == "Active Enrolled")
            {

                //disabling Patient Pre Liaison
                if (patient.Patients_PreLiaisonsId != null)
                {
                    if (patientpreliasion != null)
                    {
                        patientpreliasion.Status = false;
                        _db.Entry(patientpreliasion).State = EntityState.Modified;
                        _db.SaveChanges();

                    }

                }

                var ccmbillingCatgoryID = 0;
                if (patient.CCMEnrolledOn == null)
                {
                    ccmbillingCatgoryID = BillingCodeHelper.cmmBillingCatagoryid;

                    if (billingCategory != null)
                    {

                        if (billingCategory.Contains(ccmbillingCatgoryID.ToString()))
                        {

                            patient.CcmStatus = "Enrolled";
                            patient.CCMEnrolledOn = DateTime.Now;
                            patient.CCMEnrolledBy = GetUserId();
                            _db.Entry(patient).State = EntityState.Modified;
                            _db.SaveChanges();
                            CategoryCycleStatusHelper.User = User;
                            patient.Cycle = CategoryCycleStatusHelper.GetPatientNewOrOldCycleByCategory(patient.Id, BillingCodeHelper.cmmBillingCatagoryid);
                            _db.Entry(patient).State = EntityState.Modified;
                            await _db.SaveChangesAsync();
                        }

                    }


                }
                try
                {
                    //var existingBillingCategories = _db.Patients_BillingCategories.Where(x => x.PatientId == patient.Id && x.Status == true).ToList();


                    foreach (var item in patientCategoryList)
                    {
                        if (string.IsNullOrEmpty(item.DeEnrollmentReason))
                        {
                            //mean Patient is enrolled 
                            int BillingCategoryId = Convert.ToInt32(item.BillingcategoryId);

                            #region Saving RPM Service
                            if (BillingCategoryId == BillingCodeHelper.RPMBillingCatagoryid)
                            {
                                try
                                {
                                    var patient_service = _db.Patients_Services.Where(x => x.PatientId == patient.Id && x.IsActive == (int)IsActiveStatus.Active).ToList();
                                    if (patient_service.Count() > 0)
                                    {
                                        patient_service.ForEach(x =>
                                        {

                                            if (!RpmServiceList.Contains(x.RPMServiceId.GetInteger()))
                                            {
                                                x.IsActive = (int)IsActiveStatus.DeActive;
                                                x.UpdatedOn = DateTime.Now;
                                                x.UpdatedBy = GetUserId();
                                                _db.Entry(x).State = EntityState.Modified;
                                                _db.SaveChanges();
                                            }
                                            else
                                            {
                                                RpmServiceList.Remove(x.RPMServiceId.GetInteger());
                                            }
                                        });
                                        RpmServiceList.ForEach(x =>
                                        {
                                            var rpmservice = x.GetRPMServivceById();
                                            var PatientRpmSevice = new Patients_Services();
                                            PatientRpmSevice.IsActive = (int)IsActiveStatus.Active;
                                            PatientRpmSevice.PatientId = patient.Id;
                                            PatientRpmSevice.Patient = patient;
                                            //PatientRpmSevice.RPMService = rpmservice;
                                            PatientRpmSevice.RPMServiceId = rpmservice.Id;
                                            PatientRpmSevice.CreatedBy = GetUserId();
                                            PatientRpmSevice.CreatedOn = DateTime.Now;
                                            _db.Patients_Services.Add(PatientRpmSevice);
                                            _db.SaveChanges();
                                        });

                                    }
                                    else
                                    {
                                        RpmServiceList.ForEach(x =>
                                        {
                                            var rpmservice = x.GetRPMServivceById();
                                            var PatientRpmSevice = new Patients_Services();
                                            PatientRpmSevice.IsActive = (int)IsActiveStatus.Active;
                                            PatientRpmSevice.PatientId = patient.Id;
                                            PatientRpmSevice.Patient = patient;
                                            // PatientRpmSevice.RPMService = rpmservice;
                                            PatientRpmSevice.RPMServiceId = rpmservice.Id;
                                            PatientRpmSevice.CreatedBy = GetUserId();
                                            PatientRpmSevice.CreatedOn = DateTime.Now;
                                            _db.Patients_Services.Add(PatientRpmSevice);
                                            _db.SaveChanges();

                                        });
                                    }

                                }
                                catch (Exception e)
                                {
                                    // throw e;
                                }
                            }
                            #endregion

                            //adding liason and translator for ccm in patient table
                            if (BillingCategoryId == BillingCodeHelper.cmmBillingCatagoryid)
                            {
                                int liaisonccm = Convert.ToInt32(item.LiaisonId);
                                patient.LiaisonId = liaisonccm;
                                patient.TranslatorId = item.TranslatorId.GetNullableInteger() != null ? item.TranslatorId.GetNullableInteger() : null;

                            }
                            CategoryCycleStatusHelper.User = User;
                            CategoryCycleStatusHelper.GetPatientNewOrOldCycleStatusbyCategory(patient.Id, BillingCategoryId,null);

                            //conselor
                            int? liasonId = item.LiaisonId.GetNullableInteger();
                           
                            #region changed code assigning counsler and Translator
                            var exitingLiaison = _db.Patients_BillingCategories.Where(x => x.BillingCategoryId == BillingCategoryId && x.PatientId == patient.Id && x.Status == true).FirstOrDefault();
                            if (exitingLiaison != null )
                            {

                                exitingLiaison.Status = false;
                                exitingLiaison.UpdatedBy = GetUserId();
                                exitingLiaison.UpdatedOn = DateTime.Now;
                                exitingLiaison.DeEnrolledOn = DateTime.Now;
                                exitingLiaison.DeEnrollmentReason = item.DeEnrollmentReason;
                                _db.Entry(exitingLiaison).State = EntityState.Modified;
                                _db.SaveChanges();

                                var PatientBilling = new Patients_BillingCategories();
                                PatientBilling.PatientId = patient.Id;
                                PatientBilling.BillingCategoryId = BillingCategoryId;
                                PatientBilling.CreatedOn = DateTime.Now;
                                PatientBilling.CreatedBy = GetUserId();
                                PatientBilling.EnrolledOn = DateTime.Now;
                                PatientBilling.Status = true;
                                PatientBilling.LiaisonId = liasonId;
                                PatientBilling.IsTranslator = false;
                                _db.Patients_BillingCategories.Add(PatientBilling);
                                _db.SaveChanges();
                                Msg = "True";
                            }
                            else
                            {
                                var PatientBilling = new Patients_BillingCategories();
                                PatientBilling.PatientId = patient.Id;
                                PatientBilling.BillingCategoryId = BillingCategoryId;
                                PatientBilling.CreatedOn = DateTime.Now;
                                PatientBilling.CreatedBy = GetUserId();
                                PatientBilling.EnrolledOn = DateTime.Now;
                                PatientBilling.Status = true;
                                PatientBilling.LiaisonId = liasonId;
                                PatientBilling.IsTranslator = false;
                                _db.Patients_BillingCategories.Add(PatientBilling);
                                _db.SaveChanges();
                                Msg = "True";
                            }
                            if (!string.IsNullOrEmpty(item.TranslatorId))
                            {
                                int? translatorId = item.TranslatorId.GetNullableInteger();
                                var exitingTranslator = _db.Patients_BillingCategories.Where(x => x.BillingCategoryId == BillingCategoryId && x.PatientId == patient.Id && x.Status == true).FirstOrDefault();
                                if (exitingTranslator != null )
                                {
                                    exitingTranslator.TranslatorId = translatorId;
                                    exitingTranslator.IsTranslator = true;
                                    exitingTranslator.UpdatedBy = GetUserId();
                                    exitingTranslator.UpdatedOn = DateTime.Now;
                                    exitingTranslator.DeEnrolledOn = DateTime.Now;
                                    //exitingTranslator.DeEnrollmentReason = item.DeEnrollmentReason;
                                    _db.Entry(exitingTranslator).State = EntityState.Modified;
                                    _db.SaveChanges();
                                    Msg = "True";
                                    //var PatientBilling = new Patients_BillingCategories();
                                    //PatientBilling.PatientId = patient.Id;
                                    //PatientBilling.BillingCategoryId = BillingCategoryId;
                                    //PatientBilling.CreatedOn = DateTime.Now;
                                    //PatientBilling.CreatedBy = GetUserId();
                                    //PatientBilling.EnrolledOn = DateTime.Now;
                                    //PatientBilling.Status = true;
                                    //PatientBilling.LiaisonId = translatorId;
                                    //PatientBilling.IsTranslator = true;
                                    //_db.Patients_BillingCategories.Add(PatientBilling);
                                    //_db.SaveChanges();

                                }
                                //else
                                //{
                                //    var PatientBilling = new Patients_BillingCategories();
                                //    PatientBilling.PatientId = patient.Id;
                                //    PatientBilling.BillingCategoryId = BillingCategoryId;
                                //    PatientBilling.CreatedOn = DateTime.Now;
                                //    PatientBilling.CreatedBy = GetUserId();
                                //    PatientBilling.EnrolledOn = DateTime.Now;
                                //    PatientBilling.Status = true;
                                //    PatientBilling.LiaisonId = translatorId;
                                //    PatientBilling.IsTranslator = true;
                                //    _db.Patients_BillingCategories.Add(PatientBilling);
                                //    _db.SaveChanges();
                                //    Msg = "True";
                                //}
                            }
                            #endregion
                            #region old code assigning counsler and Translator
                            //var exitingLiaison = _db.Patients_BillingCategories.Where(x => x.BillingCategoryId == BillingCategoryId && x.PatientId == patient.Id && x.IsTranslator == false && x.Status == true).FirstOrDefault();
                            //if (exitingLiaison != null)
                            //{

                            //    exitingLiaison.Status = false;
                            //    exitingLiaison.UpdatedBy = GetUserId();
                            //    exitingLiaison.UpdatedOn = DateTime.Now;
                            //    exitingLiaison.DeEnrolledOn = DateTime.Now;
                            //    exitingLiaison.DeEnrollmentReason = item.DeEnrollmentReason;
                            //    _db.Entry(exitingLiaison).State = EntityState.Modified;
                            //    _db.SaveChanges();

                            //    var PatientBilling = new Patients_BillingCategories();
                            //    PatientBilling.PatientId = patient.Id;
                            //    PatientBilling.BillingCategoryId = BillingCategoryId;
                            //    PatientBilling.CreatedOn = DateTime.Now;
                            //    PatientBilling.CreatedBy = GetUserId();
                            //    PatientBilling.EnrolledOn = DateTime.Now;
                            //    PatientBilling.Status = true;
                            //    PatientBilling.LiaisonId = liasonId;
                            //    PatientBilling.IsTranslator = false;
                            //    _db.Patients_BillingCategories.Add(PatientBilling);
                            //    _db.SaveChanges();
                            //    Msg = "True";
                            //}
                            //else
                            //{
                            //    var PatientBilling = new Patients_BillingCategories();
                            //    PatientBilling.PatientId = patient.Id;
                            //    PatientBilling.BillingCategoryId = BillingCategoryId;
                            //    PatientBilling.CreatedOn = DateTime.Now;
                            //    PatientBilling.CreatedBy = GetUserId();
                            //    PatientBilling.EnrolledOn = DateTime.Now;
                            //    PatientBilling.Status = true;
                            //    PatientBilling.LiaisonId = liasonId;
                            //    PatientBilling.IsTranslator = false;
                            //    _db.Patients_BillingCategories.Add(PatientBilling);
                            //    _db.SaveChanges();
                            //    Msg = "True";
                            //}
                            //if (!string.IsNullOrEmpty(item.TranslatorId))
                            //{
                            //    int? translatorId = item.TranslatorId.GetNullableInteger();
                            //    var exitingTranslator = _db.Patients_BillingCategories.Where(x => x.BillingCategoryId == BillingCategoryId && x.PatientId == patient.Id && x.IsTranslator == true && x.Status == true).FirstOrDefault();
                            //    if (exitingTranslator != null)
                            //    {
                            //        exitingTranslator.Status = false;
                            //        exitingTranslator.UpdatedBy = GetUserId();
                            //        exitingTranslator.UpdatedOn = DateTime.Now;
                            //        exitingTranslator.DeEnrolledOn = DateTime.Now;
                            //        exitingTranslator.DeEnrollmentReason = item.DeEnrollmentReason;
                            //        _db.Entry(exitingTranslator).State = EntityState.Modified;
                            //        _db.SaveChanges();

                            //        var PatientBilling = new Patients_BillingCategories();
                            //        PatientBilling.PatientId = patient.Id;
                            //        PatientBilling.BillingCategoryId = BillingCategoryId;
                            //        PatientBilling.CreatedOn = DateTime.Now;
                            //        PatientBilling.CreatedBy = GetUserId();
                            //        PatientBilling.EnrolledOn = DateTime.Now;
                            //        PatientBilling.Status = true;
                            //        PatientBilling.LiaisonId = translatorId;
                            //        PatientBilling.IsTranslator = true;
                            //        _db.Patients_BillingCategories.Add(PatientBilling);
                            //        _db.SaveChanges();
                            //        Msg = "True";
                            //    }
                            //    else
                            //    {
                            //        var PatientBilling = new Patients_BillingCategories();
                            //        PatientBilling.PatientId = patient.Id;
                            //        PatientBilling.BillingCategoryId = BillingCategoryId;
                            //        PatientBilling.CreatedOn = DateTime.Now;
                            //        PatientBilling.CreatedBy = GetUserId();
                            //        PatientBilling.EnrolledOn = DateTime.Now;
                            //        PatientBilling.Status = true;
                            //        PatientBilling.LiaisonId = translatorId;
                            //        PatientBilling.IsTranslator = true;
                            //        _db.Patients_BillingCategories.Add(PatientBilling);
                            //        _db.SaveChanges();
                            //        Msg = "True";
                            //    }
                            //}
                            #endregion
                        }
                        else
                        {
                            int? BillingCategoryId = item.BillingcategoryId.GetNullableInteger();
                            #region Disabling RPM Service
                            if (BillingCategoryId == BillingCodeHelper.RPMBillingCatagoryid)
                            {
                                try
                                {
                                    var patient_service = _db.Patients_Services.Where(x => x.PatientId == patient.Id && x.IsActive == (int)IsActiveStatus.Active).ToList();
                                    if (patient_service.Count() > 0)
                                    {

                                        patient_service.ForEach(x =>
                                        {
                                            x.IsActive = (int)IsActiveStatus.DeActive;
                                            x.UpdatedOn = DateTime.Now;
                                            x.UpdatedBy = GetUserId();
                                            _db.Entry(x).State = EntityState.Modified;
                                            _db.SaveChanges();
                                        });
                                    }
                                   

                                }
                                catch (Exception e)
                                {
                                    // throw e;
                                }
                            }
                            #endregion
                            var existpatientBillingcategory = _db.Patients_BillingCategories.Where(p => p.BillingCategoryId == BillingCategoryId && p.PatientId == patient.Id && p.Status == true).ToList();
                            existpatientBillingcategory.ForEach(x =>
                            {
                                x.Status = false;
                                x.UpdatedBy = GetUserId();
                                x.UpdatedOn = DateTime.Now;
                                x.DeEnrolledOn = DateTime.Now;
                                x.DeEnrollmentReason = item.DeEnrollmentReason;
                                _db.Entry(x).State = EntityState.Modified;
                                _db.SaveChanges();
                            });

                            if (existpatientBillingcategory.Where(x => x.BillingCategoryId == ccmbillingCatgoryID).FirstOrDefault() != null)
                            {
                                patient.EnrollmentStatus = "De-Enrolled";
                                patient.CcmStatus = null;
                                patient.CCMEnrolledOn = null;
                                patient.CCMEnrolledBy = null;


                            }
                        }
                    }


                }
                catch (Exception ex)
                {
                    throw ex;
                }


                _db.Entry(patient).State = EntityState.Modified;
                await _db.SaveChangesAsync();

                #region Patient Portal
                if (!string.IsNullOrEmpty(patient.Email))
                {
                    var existingUser = await UserManager.FindByNameAsync(patient.Email);
                    if (existingUser == null)
                    {
                        var password = patient.LastName.ToLower() + "#PA1013"; // + patient.Id;
                        var user = new ApplicationUser { UserName = patient.Email, Email = patient.Email };
                        var result = await UserManager.CreateAsync(user, password);

                        if (result.Succeeded)
                        {
                            user.Role = "Patient";
                            user.CCMid = patient.Id;
                            user.FirstName = patient.FirstName;
                            user.LastName = patient.LastName;
                            user.PhoneNumber = patient.MobilePhoneNumber;
                            await UserManager.AddToRoleAsync(user.Id, "Patient");
                            if (patient.CCMEnrolledOn == null && billingCategory.Contains(ccmbillingCatgoryID.ToString()))
                            {
                                patient.CcmStatus = "Enrolled";
                                patient.CCMEnrolledOn = DateTime.Now;
                                patient.CCMEnrolledBy = GetUserId();
                                // HelperExtensions.UpdateCurrentMonthActivityfromCycleZeroToOne(patient.Id);
                                try
                                {


                                    var reviewtimeccms = _db.ReviewTimeCcms.Where(x => x.PatientId == patient.Id && x.Cycle == 0).ToList().Where(x => x.StartTime.Date.Month == DateTime.Now.Month).ToList();
                                    foreach (var reviewtimeccmitem in reviewtimeccms)
                                    {
                                        reviewtimeccmitem.Cycle = 1;
                                        _db.Entry(reviewtimeccmitem).State = EntityState.Modified;
                                        _db.SaveChanges();
                                        Msg = "True";
                                    }
                                }
                                catch/* (Exception ex)*/
                                {


                                }
                            }
                            //  patient.LiaisonId = liaison?.Id ?? patient?.LiaisonId;
                            // patient.Liaison = liaison != null ? await _db.Liaisons.FindAsync(liaison.Id) : patient.Liaison;
                            _db.Entry(patient).State = EntityState.Modified;
                            _db.Entry(user).State = EntityState.Modified;
                            await _db.SaveChangesAsync();
                            Msg = "True";

                            ViewBag.Message = "CCM Enrolled. Patient Portal Created.";
                            ViewBag.Username = user.Email;
                            ViewBag.Password = password;

                            return "CCM Enrolled. Patient Portal Created.";
                            //return RedirectToAction("Index", "CcmStatus", new { status = "Enrolled" });
                        }

                        TempData["PatientPhoneNumber"] = patient.MobilePhoneNumber;
                        //ViewBag.Message = "Error: CCM Not Enrolled & Patient Portal Not Created!. Please, Try again.";
                        Msg = "Error: CCM Not Enrolled & Patient Portal Not Created!. Please, Try again.";
                        //return RedirectToAction("Details", "Patient", new { id = patient.Id });
                        //return View(patient);
                    }

                }
                #endregion
            }
            #endregion

            #region EmrRecords
            var postedImageFile = Request.Files["Photo"];
            var postedEmrImage = Request.Files["PhotoEmrRecords"];
            var postedEmrImage2 = Request.Files["PhotoEmrRecords2"];
            var postedEmrImage3 = Request.Files["PhotoEmrRecords3"];
            var postedEmrImage4 = Request.Files["PhotoEmrRecords4"];
            var postedEmrImage5 = Request.Files["PhotoEmrRecords5"];
            var postedEmrImage6 = Request.Files["PhotoEmrRecords6"];
            var postedEmrImage7 = Request.Files["PhotoEmrRecords7"];
            var postedEmrImage8 = Request.Files["PhotoEmrRecords8"];
            var postedEmrImage9 = Request.Files["PhotoEmrRecords9"];
            var postedEmrImage10 = Request.Files["PhotoEmrRecords10"];
            var postedEmrImage11 = Request.Files["PhotoEmrRecords11"];
            var postedEmrImage12 = Request.Files["PhotoEmrRecords12"];
            var postedEmrImage13 = Request.Files["PhotoEmrRecords13"];
            var postedEmrImage14 = Request.Files["PhotoEmrRecords14"];
            var postedEmrImage15 = Request.Files["PhotoEmrRecords15"];

            if (postedImageFile?.ContentLength != 0 && postedImageFile?.InputStream != null)
                using (var binary = new BinaryReader(postedImageFile.InputStream))
                {
                    var imageData = binary.ReadBytes(postedImageFile.ContentLength);
                    if (imageData.Length > 0)
                        patient.Photo = imageData;
                }

            else
            {
                var patientCopy = await _db.Patients.AsNoTracking().FirstOrDefaultAsync(p => p.Id == patient.Id);
                if (patientCopy?.Photo != null)
                    patient.Photo = patientCopy.Photo;
            }
            if (postedEmrImage?.ContentLength != 0 && postedEmrImage?.InputStream != null)
                using (var binaryEmr = new BinaryReader(postedEmrImage.InputStream))
                {
                    var emrImageData = binaryEmr.ReadBytes(postedEmrImage.ContentLength);
                    if (emrImageData.Length > 0)
                        patient.PhotoEmrRecords = emrImageData;
                }
            else
            {
                var patientCopy = await _db.Patients.AsNoTracking().FirstOrDefaultAsync(p => p.Id == patient.Id);
                if (patientCopy?.PhotoEmrRecords != null)
                    patient.PhotoEmrRecords = patientCopy.PhotoEmrRecords;
            }
            if (postedEmrImage2?.ContentLength != 0 && postedEmrImage2?.InputStream != null)
                using (var binaryEmr2 = new BinaryReader(postedEmrImage2.InputStream))
                {
                    var emrImageData2 = binaryEmr2.ReadBytes(postedEmrImage2.ContentLength);
                    if (emrImageData2.Length > 0)
                        patient.PhotoEmrRecords2 = emrImageData2;
                }
            else
            {
                var patientCopy = await _db.Patients.AsNoTracking().FirstOrDefaultAsync(p => p.Id == patient.Id);
                if (patientCopy.PhotoEmrRecords2 != null)
                    patient.PhotoEmrRecords2 = patientCopy.PhotoEmrRecords2;
            }
            if (postedEmrImage3?.ContentLength != 0 && postedEmrImage3?.InputStream != null)
                using (var binaryEmr3 = new BinaryReader(postedEmrImage3.InputStream))
                {
                    var emrImageData3 = binaryEmr3.ReadBytes(postedEmrImage3.ContentLength);
                    if (emrImageData3.Length > 0)
                        patient.PhotoEmrRecords3 = emrImageData3;
                }
            else
            {
                var patientCopy = await _db.Patients.AsNoTracking().FirstOrDefaultAsync(p => p.Id == patient.Id);
                if (patientCopy.PhotoEmrRecords3 != null)
                    patient.PhotoEmrRecords3 = patientCopy.PhotoEmrRecords3;
            }

            if (postedEmrImage4?.ContentLength != 0 && postedEmrImage4?.InputStream != null)
                using (var binaryEmr4 = new BinaryReader(postedEmrImage4.InputStream))
                {
                    var emrImageData4 = binaryEmr4.ReadBytes(postedEmrImage4.ContentLength);
                    if (emrImageData4.Length > 0)
                        patient.PhotoEmrRecords4 = emrImageData4;
                }
            else
            {
                var patientCopy = await _db.Patients.AsNoTracking().FirstOrDefaultAsync(p => p.Id == patient.Id);
                if (patientCopy.PhotoEmrRecords4 != null)
                    patient.PhotoEmrRecords4 = patientCopy.PhotoEmrRecords4;
            }

            if (postedEmrImage5?.ContentLength != 0 && postedEmrImage5?.InputStream != null)
                using (var binaryEmr5 = new BinaryReader(postedEmrImage5.InputStream))
                {
                    var emrImageData5 = binaryEmr5.ReadBytes(postedEmrImage5.ContentLength);
                    if (emrImageData5.Length > 0)
                        patient.PhotoEmrRecords5 = emrImageData5;
                }
            else
            {
                var patientCopy = await _db.Patients.AsNoTracking().FirstOrDefaultAsync(p => p.Id == patient.Id);
                if (patientCopy.PhotoEmrRecords5 != null)
                    patient.PhotoEmrRecords5 = patientCopy.PhotoEmrRecords5;
            }

            if (postedEmrImage6?.ContentLength != 0 && postedEmrImage6?.InputStream != null)
                using (var binaryEmr6 = new BinaryReader(postedEmrImage6.InputStream))
                {
                    var emrImageData6 = binaryEmr6.ReadBytes(postedEmrImage6.ContentLength);
                    if (emrImageData6.Length > 0)
                        patient.PhotoEmrRecords6 = emrImageData6;
                }
            else
            {
                var patientCopy = await _db.Patients.AsNoTracking().FirstOrDefaultAsync(p => p.Id == patient.Id);
                if (patientCopy.PhotoEmrRecords6 != null)
                    patient.PhotoEmrRecords6 = patientCopy.PhotoEmrRecords6;
            }
            //NewOne
            if (postedEmrImage7?.ContentLength != 0 && postedEmrImage7?.InputStream != null)
                using (var binaryEmr7 = new BinaryReader(postedEmrImage7.InputStream))
                {
                    var emrImageData7 = binaryEmr7.ReadBytes(postedEmrImage7.ContentLength);
                    if (emrImageData7.Length > 0)
                        patient.PhotoEmrRecords7 = emrImageData7;
                }
            else
            {
                var patientCopy = await _db.Patients.AsNoTracking().FirstOrDefaultAsync(p => p.Id == patient.Id);
                if (patientCopy.PhotoEmrRecords7 != null)
                    patient.PhotoEmrRecords7 = patientCopy.PhotoEmrRecords7;
            }
            if (postedEmrImage8?.ContentLength != 0 && postedEmrImage8?.InputStream != null)
                using (var binaryEmr8 = new BinaryReader(postedEmrImage8.InputStream))
                {
                    var emrImageData8 = binaryEmr8.ReadBytes(postedEmrImage8.ContentLength);
                    if (emrImageData8.Length > 0)
                        patient.PhotoEmrRecords8 = emrImageData8;
                }
            else
            {
                var patientCopy = await _db.Patients.AsNoTracking().FirstOrDefaultAsync(p => p.Id == patient.Id);
                if (patientCopy.PhotoEmrRecords8 != null)
                    patient.PhotoEmrRecords8 = patientCopy.PhotoEmrRecords8;
            }
            if (postedEmrImage9?.ContentLength != 0 && postedEmrImage9?.InputStream != null)
                using (var binaryEmr9 = new BinaryReader(postedEmrImage9.InputStream))
                {
                    var emrImageData9 = binaryEmr9.ReadBytes(postedEmrImage9.ContentLength);
                    if (emrImageData9.Length > 0)
                        patient.PhotoEmrRecords9 = emrImageData9;
                }
            else
            {
                var patientCopy = await _db.Patients.AsNoTracking().FirstOrDefaultAsync(p => p.Id == patient.Id);
                if (patientCopy.PhotoEmrRecords9 != null)
                    patient.PhotoEmrRecords9 = patientCopy.PhotoEmrRecords9;
            }
            if (postedEmrImage10?.ContentLength != 0 && postedEmrImage10?.InputStream != null)
                using (var binaryEmr10 = new BinaryReader(postedEmrImage10.InputStream))
                {
                    var emrImageData10 = binaryEmr10.ReadBytes(postedEmrImage10.ContentLength);
                    if (emrImageData10.Length > 0)
                        patient.PhotoEmrRecords10 = emrImageData10;
                }
            else
            {
                var patientCopy = await _db.Patients.AsNoTracking().FirstOrDefaultAsync(p => p.Id == patient.Id);
                if (patientCopy.PhotoEmrRecords10 != null)
                    patient.PhotoEmrRecords10 = patientCopy.PhotoEmrRecords10;
            }
            if (postedEmrImage11?.ContentLength != 0 && postedEmrImage11?.InputStream != null)
                using (var binaryEmr11 = new BinaryReader(postedEmrImage11.InputStream))
                {
                    var emrImageData11 = binaryEmr11.ReadBytes(postedEmrImage11.ContentLength);
                    if (emrImageData11.Length > 0)
                        patient.PhotoEmrRecords11 = emrImageData11;
                }
            else
            {
                var patientCopy = await _db.Patients.AsNoTracking().FirstOrDefaultAsync(p => p.Id == patient.Id);
                if (patientCopy.PhotoEmrRecords11 != null)
                    patient.PhotoEmrRecords11 = patientCopy.PhotoEmrRecords11;
            }
            if (postedEmrImage12?.ContentLength != 0 && postedEmrImage12?.InputStream != null)
                using (var binaryEmr12 = new BinaryReader(postedEmrImage12.InputStream))
                {
                    var emrImageData12 = binaryEmr12.ReadBytes(postedEmrImage12.ContentLength);
                    if (emrImageData12.Length > 0)
                        patient.PhotoEmrRecords12 = emrImageData12;
                }
            else
            {
                var patientCopy = await _db.Patients.AsNoTracking().FirstOrDefaultAsync(p => p.Id == patient.Id);
                if (patientCopy.PhotoEmrRecords12 != null)
                    patient.PhotoEmrRecords12 = patientCopy.PhotoEmrRecords12;
            }
            if (postedEmrImage13?.ContentLength != 0 && postedEmrImage13?.InputStream != null)
                using (var binaryEmr13 = new BinaryReader(postedEmrImage13.InputStream))
                {
                    var emrImageData13 = binaryEmr13.ReadBytes(postedEmrImage13.ContentLength);
                    if (emrImageData13.Length > 0)
                        patient.PhotoEmrRecords13 = emrImageData13;
                }
            else
            {
                var patientCopy = await _db.Patients.AsNoTracking().FirstOrDefaultAsync(p => p.Id == patient.Id);
                if (patientCopy.PhotoEmrRecords13 != null)
                    patient.PhotoEmrRecords13 = patientCopy.PhotoEmrRecords13;
            }
            if (postedEmrImage14?.ContentLength != 0 && postedEmrImage14?.InputStream != null)
                using (var binaryEmr14 = new BinaryReader(postedEmrImage14.InputStream))
                {
                    var emrImageData14 = binaryEmr14.ReadBytes(postedEmrImage14.ContentLength);
                    if (emrImageData14.Length > 0)
                        patient.PhotoEmrRecords14 = emrImageData14;
                }
            else
            {
                var patientCopy = await _db.Patients.AsNoTracking().FirstOrDefaultAsync(p => p.Id == patient.Id);
                if (patientCopy.PhotoEmrRecords14 != null)
                    patient.PhotoEmrRecords14 = patientCopy.PhotoEmrRecords14;
            }
            if (postedEmrImage15?.ContentLength != 0 && postedEmrImage15?.InputStream != null)
                using (var binaryEmr15 = new BinaryReader(postedEmrImage15.InputStream))
                {
                    var emrImageData15 = binaryEmr15.ReadBytes(postedEmrImage15.ContentLength);
                    if (emrImageData15.Length > 0)
                        patient.PhotoEmrRecords15 = emrImageData15;
                }
            else
            {
                var patientCopy = await _db.Patients.AsNoTracking().FirstOrDefaultAsync(p => p.Id == patient.Id);
                if (patientCopy.PhotoEmrRecords15 != null)
                    patient.PhotoEmrRecords15 = patientCopy.PhotoEmrRecords15;
            }
            patient.UpdatedOn = DateTime.Now;
            patient.UpdatedBy = GetUserId();
            _db.Entry(patient).State = EntityState.Modified;
            await _db.SaveChangesAsync();
            #endregion



            var userId = GetUserId();
            var liaison = await _db.Liaisons.FirstOrDefaultAsync(l => l.UserId == userId);
            var physician = await _db.Physicians.FindAsync(patient.PhysicianId);
            var physicians = _db.Physicians.Select(p => new SelectListItem { Value = p.Id.ToString(), Text = p.FirstName + " " + p.LastName });
            var chroniccondition2 = _db.Patient_ChronicCondition2s?.Select(p => new SelectListItem { Value = p.Id.ToString(), Text = p.ChronicCondition2Type });
            var chroniccondition1 = _db.Patient_ChronicCondition1s?.Select(p => new SelectListItem { Value = p.Id.ToString(), Text = p.ChronicCondition1Type });

            ViewBag.Physicians = new SelectList(physicians.OrderBy(p => p.Text), "Value", "Text", patient.PhysicianId);
            ViewBag.PhysicianName = physician?.FirstName + " " + physician?.LastName;
            ViewBag.PatientChronicCondition1 = new SelectList(chroniccondition1.OrderBy(cc => cc.Text), "Value", "Text", patient.PatientChronicCondition1Id);
            ViewBag.PatientChronicCondition2 = new SelectList(chroniccondition2.OrderByDescending(cc => cc.Text), "Value", "Text", patient.PatientChronicCondition2Id);

            ViewBag.PatientName = patient.FirstName + " " + patient.LastName;
            ViewBag.PatientId = patient.Id;
            ViewBag.CcmStatus = patient.CcmStatus;
            ViewBag.EnrollmentStatus = new SelectList(_db.EnrollmentStatuss.ToList(), "Id", "Name", patient.EnrollmentStatus);
            ViewBag.EnrollmentSubStatus = new SelectList(_db.EnrollmentSubStatuss.ToList(), "EnrollmentStatusID", "Name");
            ViewBag.EnrollemntStatusReson = new SelectList(_db.EnrollmentSubstatusReasons.ToList(), "Name", "Name");
            return Msg;

            //return RedirectToAction("Details", "Patient", new { id = patient.Id });
            //return View(patient);
        }









        [HttpPost, ValidateInput(false)]
        [Authorize(Roles = "Liaison, Admin,Sales,LiaisonGroup")]
        //[ValidateAntiForgeryToken]
        //////////////////////////////////////////////////revert to chnegeset 513
        public async Task<string> UplaodEmr([Bind(Exclude = "Photo,PhotoEmrRecords,PhotoEmrRecords2,PhotoEmrRecords3,PhotoEmrRecords4,PhotoEmrRecords5,PhotoEmrRecords6,PhotoEmrRecords7,PhotoEmrRecords8,PhotoEmrRecords9,PhotoEmrRecords10,PhotoEmrRecords11,PhotoEmrRecords12,PhotoEmrRecords13,PhotoEmrRecords14,PhotoEmrRecords15,EnrollmentStatus,EnrollmentSubStatus,EnrollmentSubStatusReason")] Patient patient)
        {
            string Msg = "";
            var postedImageFile = Request.Files["Photo"];
            var postedEmrImage = Request.Files["PhotoEmrRecords"];
            var postedEmrImage2 = Request.Files["PhotoEmrRecords2"];
            var postedEmrImage3 = Request.Files["PhotoEmrRecords3"];
            var postedEmrImage4 = Request.Files["PhotoEmrRecords4"];
            var postedEmrImage5 = Request.Files["PhotoEmrRecords5"];
            var postedEmrImage6 = Request.Files["PhotoEmrRecords6"];
            var postedEmrImage7 = Request.Files["PhotoEmrRecords7"];
            var postedEmrImage8 = Request.Files["PhotoEmrRecords8"];
            var postedEmrImage9 = Request.Files["PhotoEmrRecords9"];
            var postedEmrImage10 = Request.Files["PhotoEmrRecords10"];
            var postedEmrImage11 = Request.Files["PhotoEmrRecords11"];
            var postedEmrImage12 = Request.Files["PhotoEmrRecords12"];
            var postedEmrImage13 = Request.Files["PhotoEmrRecords13"];
            var postedEmrImage14 = Request.Files["PhotoEmrRecords14"];
            var postedEmrImage15 = Request.Files["PhotoEmrRecords15"];

            if (postedImageFile?.ContentLength != 0 && postedImageFile?.InputStream != null)
                using (var binary = new BinaryReader(postedImageFile.InputStream))
                {
                    var imageData = binary.ReadBytes(postedImageFile.ContentLength);
                    if (imageData.Length > 0)
                        patient.Photo = imageData;
                }

            else
            {
                var patientCopy = await _db.Patients.AsNoTracking().FirstOrDefaultAsync(p => p.Id == patient.Id);
                if (patientCopy?.Photo != null)
                    patient.Photo = patientCopy.Photo;
            }


            if (postedEmrImage?.ContentLength != 0 && postedEmrImage?.InputStream != null)
                using (var binaryEmr = new BinaryReader(postedEmrImage.InputStream))
                {
                    var emrImageData = binaryEmr.ReadBytes(postedEmrImage.ContentLength);
                    if (emrImageData.Length > 0)
                        patient.PhotoEmrRecords = emrImageData;
                }
            else
            {
                var patientCopy = await _db.Patients.AsNoTracking().FirstOrDefaultAsync(p => p.Id == patient.Id);
                if (patientCopy?.PhotoEmrRecords != null)
                    patient.PhotoEmrRecords = patientCopy.PhotoEmrRecords;
            }


            if (postedEmrImage2?.ContentLength != 0 && postedEmrImage2?.InputStream != null)
                using (var binaryEmr2 = new BinaryReader(postedEmrImage2.InputStream))
                {
                    var emrImageData2 = binaryEmr2.ReadBytes(postedEmrImage2.ContentLength);
                    if (emrImageData2.Length > 0)
                        patient.PhotoEmrRecords2 = emrImageData2;
                }
            else
            {
                var patientCopy = await _db.Patients.AsNoTracking().FirstOrDefaultAsync(p => p.Id == patient.Id);
                if (patientCopy.PhotoEmrRecords2 != null)
                    patient.PhotoEmrRecords2 = patientCopy.PhotoEmrRecords2;
            }

            if (postedEmrImage3?.ContentLength != 0 && postedEmrImage3?.InputStream != null)
                using (var binaryEmr3 = new BinaryReader(postedEmrImage3.InputStream))
                {
                    var emrImageData3 = binaryEmr3.ReadBytes(postedEmrImage3.ContentLength);
                    if (emrImageData3.Length > 0)
                        patient.PhotoEmrRecords3 = emrImageData3;
                }
            else
            {
                var patientCopy = await _db.Patients.AsNoTracking().FirstOrDefaultAsync(p => p.Id == patient.Id);
                if (patientCopy.PhotoEmrRecords3 != null)
                    patient.PhotoEmrRecords3 = patientCopy.PhotoEmrRecords3;
            }

            if (postedEmrImage4?.ContentLength != 0 && postedEmrImage4?.InputStream != null)
                using (var binaryEmr4 = new BinaryReader(postedEmrImage4.InputStream))
                {
                    var emrImageData4 = binaryEmr4.ReadBytes(postedEmrImage4.ContentLength);
                    if (emrImageData4.Length > 0)
                        patient.PhotoEmrRecords4 = emrImageData4;
                }
            else
            {
                var patientCopy = await _db.Patients.AsNoTracking().FirstOrDefaultAsync(p => p.Id == patient.Id);
                if (patientCopy.PhotoEmrRecords4 != null)
                    patient.PhotoEmrRecords4 = patientCopy.PhotoEmrRecords4;
            }

            if (postedEmrImage5?.ContentLength != 0 && postedEmrImage5?.InputStream != null)
                using (var binaryEmr5 = new BinaryReader(postedEmrImage5.InputStream))
                {
                    var emrImageData5 = binaryEmr5.ReadBytes(postedEmrImage5.ContentLength);
                    if (emrImageData5.Length > 0)
                        patient.PhotoEmrRecords5 = emrImageData5;
                }
            else
            {
                var patientCopy = await _db.Patients.AsNoTracking().FirstOrDefaultAsync(p => p.Id == patient.Id);
                if (patientCopy.PhotoEmrRecords5 != null)
                    patient.PhotoEmrRecords5 = patientCopy.PhotoEmrRecords5;
            }

            if (postedEmrImage6?.ContentLength != 0 && postedEmrImage6?.InputStream != null)
                using (var binaryEmr6 = new BinaryReader(postedEmrImage6.InputStream))
                {
                    var emrImageData6 = binaryEmr6.ReadBytes(postedEmrImage6.ContentLength);
                    if (emrImageData6.Length > 0)
                        patient.PhotoEmrRecords6 = emrImageData6;
                }
            else
            {
                var patientCopy = await _db.Patients.AsNoTracking().FirstOrDefaultAsync(p => p.Id == patient.Id);
                if (patientCopy.PhotoEmrRecords6 != null)
                    patient.PhotoEmrRecords6 = patientCopy.PhotoEmrRecords6;
            }
            //NewOne
            if (postedEmrImage7?.ContentLength != 0 && postedEmrImage7?.InputStream != null)
                using (var binaryEmr7 = new BinaryReader(postedEmrImage7.InputStream))
                {
                    var emrImageData7 = binaryEmr7.ReadBytes(postedEmrImage7.ContentLength);
                    if (emrImageData7.Length > 0)
                        patient.PhotoEmrRecords7 = emrImageData7;
                }
            else
            {
                var patientCopy = await _db.Patients.AsNoTracking().FirstOrDefaultAsync(p => p.Id == patient.Id);
                if (patientCopy.PhotoEmrRecords7 != null)
                    patient.PhotoEmrRecords7 = patientCopy.PhotoEmrRecords7;
            }
            if (postedEmrImage8?.ContentLength != 0 && postedEmrImage8?.InputStream != null)
                using (var binaryEmr8 = new BinaryReader(postedEmrImage8.InputStream))
                {
                    var emrImageData8 = binaryEmr8.ReadBytes(postedEmrImage8.ContentLength);
                    if (emrImageData8.Length > 0)
                        patient.PhotoEmrRecords8 = emrImageData8;
                }
            else
            {
                var patientCopy = await _db.Patients.AsNoTracking().FirstOrDefaultAsync(p => p.Id == patient.Id);
                if (patientCopy.PhotoEmrRecords8 != null)
                    patient.PhotoEmrRecords8 = patientCopy.PhotoEmrRecords8;
            }
            if (postedEmrImage9?.ContentLength != 0 && postedEmrImage9?.InputStream != null)
                using (var binaryEmr9 = new BinaryReader(postedEmrImage9.InputStream))
                {
                    var emrImageData9 = binaryEmr9.ReadBytes(postedEmrImage9.ContentLength);
                    if (emrImageData9.Length > 0)
                        patient.PhotoEmrRecords9 = emrImageData9;
                }
            else
            {
                var patientCopy = await _db.Patients.AsNoTracking().FirstOrDefaultAsync(p => p.Id == patient.Id);
                if (patientCopy.PhotoEmrRecords9 != null)
                    patient.PhotoEmrRecords9 = patientCopy.PhotoEmrRecords9;
            }
            if (postedEmrImage10?.ContentLength != 0 && postedEmrImage10?.InputStream != null)
                using (var binaryEmr10 = new BinaryReader(postedEmrImage10.InputStream))
                {
                    var emrImageData10 = binaryEmr10.ReadBytes(postedEmrImage10.ContentLength);
                    if (emrImageData10.Length > 0)
                        patient.PhotoEmrRecords10 = emrImageData10;
                }
            else
            {
                var patientCopy = await _db.Patients.AsNoTracking().FirstOrDefaultAsync(p => p.Id == patient.Id);
                if (patientCopy.PhotoEmrRecords10 != null)
                    patient.PhotoEmrRecords10 = patientCopy.PhotoEmrRecords10;
            }
            if (postedEmrImage11?.ContentLength != 0 && postedEmrImage11?.InputStream != null)
                using (var binaryEmr11 = new BinaryReader(postedEmrImage11.InputStream))
                {
                    var emrImageData11 = binaryEmr11.ReadBytes(postedEmrImage11.ContentLength);
                    if (emrImageData11.Length > 0)
                        patient.PhotoEmrRecords11 = emrImageData11;
                }
            else
            {
                var patientCopy = await _db.Patients.AsNoTracking().FirstOrDefaultAsync(p => p.Id == patient.Id);
                if (patientCopy.PhotoEmrRecords11 != null)
                    patient.PhotoEmrRecords11 = patientCopy.PhotoEmrRecords11;
            }
            if (postedEmrImage12?.ContentLength != 0 && postedEmrImage12?.InputStream != null)
                using (var binaryEmr12 = new BinaryReader(postedEmrImage12.InputStream))
                {
                    var emrImageData12 = binaryEmr12.ReadBytes(postedEmrImage12.ContentLength);
                    if (emrImageData12.Length > 0)
                        patient.PhotoEmrRecords12 = emrImageData12;
                }
            else
            {
                var patientCopy = await _db.Patients.AsNoTracking().FirstOrDefaultAsync(p => p.Id == patient.Id);
                if (patientCopy.PhotoEmrRecords12 != null)
                    patient.PhotoEmrRecords12 = patientCopy.PhotoEmrRecords12;
            }
            if (postedEmrImage13?.ContentLength != 0 && postedEmrImage13?.InputStream != null)
                using (var binaryEmr13 = new BinaryReader(postedEmrImage13.InputStream))
                {
                    var emrImageData13 = binaryEmr13.ReadBytes(postedEmrImage13.ContentLength);
                    if (emrImageData13.Length > 0)
                        patient.PhotoEmrRecords13 = emrImageData13;
                }
            else
            {
                var patientCopy = await _db.Patients.AsNoTracking().FirstOrDefaultAsync(p => p.Id == patient.Id);
                if (patientCopy.PhotoEmrRecords13 != null)
                    patient.PhotoEmrRecords13 = patientCopy.PhotoEmrRecords13;
            }
            if (postedEmrImage14?.ContentLength != 0 && postedEmrImage14?.InputStream != null)
                using (var binaryEmr14 = new BinaryReader(postedEmrImage14.InputStream))
                {
                    var emrImageData14 = binaryEmr14.ReadBytes(postedEmrImage14.ContentLength);
                    if (emrImageData14.Length > 0)
                        patient.PhotoEmrRecords14 = emrImageData14;
                }
            else
            {
                var patientCopy = await _db.Patients.AsNoTracking().FirstOrDefaultAsync(p => p.Id == patient.Id);
                if (patientCopy.PhotoEmrRecords14 != null)
                    patient.PhotoEmrRecords14 = patientCopy.PhotoEmrRecords14;
            }
            if (postedEmrImage15?.ContentLength != 0 && postedEmrImage15?.InputStream != null)
                using (var binaryEmr15 = new BinaryReader(postedEmrImage15.InputStream))
                {
                    var emrImageData15 = binaryEmr15.ReadBytes(postedEmrImage15.ContentLength);
                    if (emrImageData15.Length > 0)
                        patient.PhotoEmrRecords15 = emrImageData15;
                }
            else
            {
                var patientCopy = await _db.Patients.AsNoTracking().FirstOrDefaultAsync(p => p.Id == patient.Id);
                if (patientCopy.PhotoEmrRecords15 != null)
                    patient.PhotoEmrRecords15 = patientCopy.PhotoEmrRecords15;
            }

            var userId = GetUserId();
            var liaison = await _db.Liaisons.FirstOrDefaultAsync(l => l.UserId == userId);
            var physician = await _db.Physicians.FindAsync(patient.PhysicianId);
            var physicians = _db.Physicians.Select(p => new SelectListItem { Value = p.Id.ToString(), Text = p.FirstName + " " + p.LastName });
            var chroniccondition2 = _db.Patient_ChronicCondition2s?.Select(p => new SelectListItem { Value = p.Id.ToString(), Text = p.ChronicCondition2Type });
            var chroniccondition1 = _db.Patient_ChronicCondition1s?.Select(p => new SelectListItem { Value = p.Id.ToString(), Text = p.ChronicCondition1Type });

            ViewBag.Physicians = new SelectList(physicians.OrderBy(p => p.Text), "Value", "Text", patient.PhysicianId);
            ViewBag.PhysicianName = physician?.FirstName + " " + physician?.LastName;
            ViewBag.PatientChronicCondition1 = new SelectList(chroniccondition1.OrderBy(cc => cc.Text), "Value", "Text", patient.PatientChronicCondition1Id);
            ViewBag.PatientChronicCondition2 = new SelectList(chroniccondition2.OrderByDescending(cc => cc.Text), "Value", "Text", patient.PatientChronicCondition2Id);

            ViewBag.PatientName = patient.FirstName + " " + patient.LastName;
            ViewBag.PatientId = patient.Id;
            ViewBag.CcmStatus = patient.CcmStatus;
            ViewBag.EnrollmentStatus = new SelectList(_db.EnrollmentStatuss.ToList(), "Id", "Name", patient.EnrollmentStatus);
            ViewBag.EnrollmentSubStatus = new SelectList(_db.EnrollmentSubStatuss.ToList(), "EnrollmentStatusID", "Name");
            ViewBag.EnrollemntStatusReson = new SelectList(_db.EnrollmentSubstatusReasons.ToList(), "Name", "Name");
            patient.UpdatedOn = DateTime.Now;
            patient.UpdatedBy = GetUserId();
            try
            {
                _db.Entry(patient).State = EntityState.Modified;
                await _db.SaveChangesAsync();
                Msg = "True";
            }
            catch /*(Exception ex)*/
            {


            }

            if (patient.EnrollmentSubStatus == "Active Enrolled")
            {
                if (patient.CCMEnrolledOn == null)
                {
                    patient.CcmStatus = "Enrolled";
                    patient.CCMEnrolledOn = DateTime.Now;
                    patient.CCMEnrolledBy = GetUserId();
                    //HelperExtensions.UpdateCurrentMonthActivityfromCycleZeroToOne(patient.Id);
                    try
                    {


                        var reviewtimeccms = _db.ReviewTimeCcms.Where(x => x.PatientId == patient.Id && x.Cycle == 0).ToList().Where(x => x.StartTime.Date.Month == DateTime.Now.Month).ToList();
                        foreach (var reviewtimeccmitem in reviewtimeccms)
                        {
                            reviewtimeccmitem.Cycle = 1;
                            _db.Entry(reviewtimeccmitem).State = EntityState.Modified;
                            _db.SaveChanges();
                            Msg = "True";
                        }
                    }
                    catch /*(Exception ex)*/
                    {


                    }

                }


                // patient.LiaisonId = liaison?.Id ?? patient?.LiaisonId;
                // patient.Liaison = liaison != null ? await _db.Liaisons.FindAsync(liaison.Id) : patient.Liaison;

                _db.Entry(patient).State = EntityState.Modified;
                await _db.SaveChangesAsync();
                if (!string.IsNullOrEmpty(patient.Email))
                {
                    var existingUser = await UserManager.FindByNameAsync(patient.Email);
                    if (existingUser == null)
                    {
                        var password = patient.LastName.ToLower() + "#PA1013"; // + patient.Id;
                        var user = new ApplicationUser { UserName = patient.Email, Email = patient.Email };
                        var result = await UserManager.CreateAsync(user, password);

                        if (result.Succeeded)
                        {
                            user.Role = "Patient";
                            user.CCMid = patient.Id;
                            user.FirstName = patient.FirstName;
                            user.LastName = patient.LastName;
                            user.PhoneNumber = patient.MobilePhoneNumber;
                            await UserManager.AddToRoleAsync(user.Id, "Patient");
                            if (patient.CCMEnrolledOn == null)
                            {
                                patient.CcmStatus = "Enrolled";
                                patient.CCMEnrolledOn = DateTime.Now;
                                patient.CCMEnrolledBy = GetUserId();
                                //HelperExtensions.UpdateCurrentMonthActivityfromCycleZeroToOne(patient.Id);
                                try
                                {


                                    var reviewtimeccms = _db.ReviewTimeCcms.Where(x => x.PatientId == patient.Id && x.Cycle == 0).ToList().Where(x => x.StartTime.Date.Month == DateTime.Now.Month).ToList();
                                    foreach (var reviewtimeccmitem in reviewtimeccms)
                                    {
                                        reviewtimeccmitem.Cycle = 1;
                                        _db.Entry(reviewtimeccmitem).State = EntityState.Modified;
                                        _db.SaveChanges();
                                        Msg = "True";
                                    }
                                }
                                catch /*(Exception ex)*/
                                {


                                }
                            }
                            //  patient.LiaisonId = liaison?.Id ?? patient?.LiaisonId;
                            // patient.Liaison = liaison != null ? await _db.Liaisons.FindAsync(liaison.Id) : patient.Liaison;
                            _db.Entry(patient).State = EntityState.Modified;
                            _db.Entry(user).State = EntityState.Modified;
                            await _db.SaveChangesAsync();
                            Msg = "True";

                            ViewBag.Message = "CCM Enrolled. Patient Portal Created.";
                            ViewBag.Username = user.Email;
                            ViewBag.Password = password;

                            return "CCM Enrolled. Patient Portal Created.";
                            //return RedirectToAction("Index", "CcmStatus", new { status = "Enrolled" });
                        }

                        TempData["PatientPhoneNumber"] = patient.MobilePhoneNumber;
                        //ViewBag.Message = "Error: CCM Not Enrolled & Patient Portal Not Created!. Please, Try again.";
                        Msg = "Error: CCM Not Enrolled & Patient Portal Not Created!. Please, Try again.";
                        //return RedirectToAction("Details", "Patient", new { id = patient.Id });
                        //return View(patient);
                    }
                    //else
                    //{
                    //    TempData["PatientPhoneNumber"] = patient.MobilePhoneNumber;
                    //    ViewBag.Message = "Patient-Email Already Exists! CCM Not Enrolled & Patient Portal Not Created!.";
                    //    Msg= "Patient-Email Already Exists! CCM Not Enrolled & Patient Portal Not Created!.";
                    //    //return RedirectToAction("Details", "Patient", new { id = patient.Id });
                    //    //return View(patient);

                    //}

                }
                //return RedirectToAction("Index", "CcmStatus", new { status = "Enrolled" });
            }
            return Msg;
        }











        [Authorize(Roles = "Liaison, Admin")]
        public ActionResult UploadNewPatient()
        {
            var physician = _db.Physicians.Select(p => new SelectListItem
            {
                Value = p.Id.ToString(),
                Text = p.FirstName + " " + p.LastName
            }
                                                 );
            ViewBag.Physicians = new SelectList(physician, "Value", "Text");

            return View();
        }

        [HttpPost]
        public async Task<ActionResult> AddPatient(CcdaPatientViewModel ccdaPatient)
        {
            if (ccdaPatient.PhysicianId == null)
                return Content("Physician Not Found. Patient Not Added.");

            var physician = await _db.Physicians.FindAsync(ccdaPatient.PhysicianId);
            if (physician == null)
                return Content("Physician Not Found. Patient Not Added");

            var newPatient = new Patient
            {
                Prefix = ccdaPatient.Name.Prefix,
                FirstName = ccdaPatient.Name.Given[0],
                LastName = ccdaPatient.Name.Family,
                BirthDate = Convert.ToDateTime(ccdaPatient.Dob),
                Gender = ccdaPatient.Gender,
                PreferredLanguage = ccdaPatient.Language,

                MobilePhoneNumber = ccdaPatient.Phone.Mobile,
                HomePhoneNumber = ccdaPatient.Phone.Home,
                WorkPhoneNumber = ccdaPatient.Phone.Work,
                Email = ccdaPatient.Email,

                Address1 = ccdaPatient.Address.Street[0],
                Address2 = ccdaPatient.Address.Street[1],
                City = ccdaPatient.Address.City,
                State = ccdaPatient.Address.State,
                Zipcode = ccdaPatient.Address.Zip,

                EnrollmentStatus = "Not Enrolled",
                EnrollmentSubStatus = "Not Assigned Yet",
                CreatedOn = DateTime.Now,
                CreatedBy = GetUserId(),

                Physician = physician,
                PhysicianId = physician.Id
            };

            _db.Patients.Add(newPatient);
            await _db.SaveChangesAsync();

            newPatient.ProfileId = await NewProfileAsync(newPatient.Id, ccdaPatient);
            newPatient.ContactId = await NewContactAsync(newPatient.Id, ccdaPatient);
            newPatient.AddressId = await NewAddressAsync(newPatient.Id, ccdaPatient);
            newPatient.DietAndHabitId = await NewDietAndHabitAsync(newPatient.Id, ccdaPatient.SmokingStatus);

            if (ccdaPatient.LabResults != null)
                AddLabResults(newPatient.Id, ccdaPatient.LabResults);

            if (ccdaPatient.Allergies != null)
                AddAllergies(newPatient.Id, ccdaPatient.Allergies);

            if (ccdaPatient.Problems != null)
                AddProblems(newPatient.Id, ccdaPatient.Problems);

            if (ccdaPatient.Procedures != null)
                AddProcedures(newPatient.Id, ccdaPatient.Procedures);

            if (ccdaPatient.Medications != null)
                AddMedications(newPatient.Id, ccdaPatient.Medications);

            await _db.SaveChangesAsync();
            return Content("Success: Patient " + newPatient.FirstName + ' ' + newPatient.LastName + " Uploaded.");
        }

        private async Task<int> NewProfileAsync(int patientId, CcdaPatientViewModel ccdaPatient)
        {
            var profile = new PatientProfile
            {
                PatientId = patientId,
                Prefix = ccdaPatient.Name.Prefix,
                FirstName = ccdaPatient.Name.Given[0],
                LastName = ccdaPatient.Name.Family,
                BirthDate = Convert.ToDateTime(ccdaPatient.Dob),
                Gender = ccdaPatient.Gender,
                PreferredLanguage = ccdaPatient.Language
            };

            _db.PatientProfiles.Add(profile);
            await _db.SaveChangesAsync();

            return profile.Id;
        }

        private async Task<int> NewContactAsync(int patientId, CcdaPatientViewModel ccdaPatient)
        {
            var contact = new PatientProfile_Contact
            {
                PatientId = patientId,
                CellPhoneNumber = ccdaPatient.Phone.Mobile,
                HomePhoneNumber = ccdaPatient.Phone.Home,
                WorkPhoneNumber = ccdaPatient.Phone.Work,
                Email = ccdaPatient.Email,
                CellPhonePermission = true,
                EmailPermission = true
            };

            _db.PatientProfile_Contact.Add(contact);
            await _db.SaveChangesAsync();

            return contact.Id;
        }

        private async Task<int> NewAddressAsync(int patientId, CcdaPatientViewModel ccdaPatient)
        {
            var address = new PatientProfile_Address
            {
                PatientId = patientId,
                Address1 = ccdaPatient.Address.Street[0],
                Address2 = ccdaPatient.Address.Street[1],
                City = ccdaPatient.Address.City,
                State = ccdaPatient.Address.State,
                Zip = ccdaPatient.Address.Zip
            };

            _db.PatientProfile_Addresses.Add(address);
            await _db.SaveChangesAsync();

            return address.Id;
        }

        private async Task<int> NewDietAndHabitAsync(int patientId, string smokingStatus)
        {
            var dietAndHabit = new PatientLifestyle_DietAndHabit
            {
                PatientId = patientId,
                TobaccoHowOften = smokingStatus
            };

            _db.PatientLifestyle_DietAndHabits.Add(dietAndHabit);
            await _db.SaveChangesAsync();

            return dietAndHabit.Id;
        }

        private void AddLabResults(int patientId, IEnumerable<LabResult> labResults)
        {
            foreach (var result in labResults)
            {
                if (!string.IsNullOrEmpty(result.Name) &&
                    !string.IsNullOrEmpty(result.TestValue))
                    _db.CcdaLabResults.Add(new CcdaLabResult
                    {
                        PatientId = patientId,
                        Name = result.Name,
                        TestValue = result.TestValue,
                        Date = result.Date
                    });
            }
            _db.SaveChanges();
        }

        private void AddAllergies(int patientId, IEnumerable<Allergy> allergies)
        {
            foreach (var allergy in allergies)
            {
                if (!string.IsNullOrEmpty(allergy.Name) &&
                    !string.IsNullOrEmpty(allergy.Severity))
                    _db.CcdaAllergies.Add(new CcdaAllergy
                    {
                        PatientId = patientId,
                        Name = allergy.Name,
                        Severity = allergy.Severity,
                        StartDate = Convert.ToDateTime(allergy.StartDate).ToShortDateString()
                    });
            }
            _db.SaveChanges();
        }

        private void AddProblems(int patientId, IEnumerable<Problem> problems)
        {
            foreach (var problem in problems)
            {
                if (!string.IsNullOrEmpty(problem.Name) &&
                    !string.IsNullOrEmpty(problem.Status))
                    _db.CcdaProblems.Add(new CcdaProblem
                    {
                        PatientId = patientId,
                        Name = problem.Name,
                        Status = problem.Status
                    });
            }
            _db.SaveChanges();
        }

        private void AddProcedures(int patientId, IEnumerable<Procedure> procedures)
        {
            foreach (var procedure in procedures)
            {
                if (!string.IsNullOrEmpty(procedure.Name))
                    _db.CcdaProcedures.Add(new CcdaProcedure
                    {
                        PatientId = patientId,
                        Name = procedure.Name,
                        Date = Convert.ToDateTime(procedure.Date).ToShortDateString()
                    });
            }
            _db.SaveChanges();
        }

        private void AddMedications(int patientId, IEnumerable<Medication> medications)
        {
            foreach (var medication in medications)
            {
                if (!string.IsNullOrEmpty(medication.Name))
                    _db.PatientMedicalHistory_MedicationRxes.Add(new PatientMedicalHistory_MedicationRx()
                    {
                        PatientId = patientId,
                        DrugName = medication.Name,
                        StartDate = (DateTime)medication.StartDate,
                        DailyDose = medication.DoseValue + ' ' + medication.DoseUnit,
                        RxCuis = medication.RxCui,
                        RateQuantity = medication.RateValue + ' ' + medication.RateUnit,
                        Route = medication.Route,
                        UseReason = medication.Reason,
                        EndDate = (DateTime)medication.EndDate,
                        DoseRepetitionTime = medication.DoseRepetitionTime,
                        PrescribeBy = medication.PrescribeBy,
                        ForHowLong = medication.ForHowLong
                    });
            }
            _db.SaveChanges();
        }


        [Authorize(Roles = "Liaison, Admin,Sales")]
        public async Task<ActionResult> Create(string rda)
        {
            var user1 =
               _db.Users.Find(GetUserId());
            var physician = _db.Physicians.AsNoTracking().Select(p => new SelectListItem
            {
                Value = p.Id.ToString(),
                Text = p.FirstName + " " + p.LastName
            });
            if (User.IsInRole("Sales"))
            {
                var physiciangrpids = _db.physicianGroup_SalesStaff_Mappings.AsNoTracking().Where(x => x.SaleStaffId == user1.CCMid).Select(x => x.PhysiciansGroupId).ToList();
                var physicianids = _db.physicianGroup_Physician_Mappings.AsNoTracking().Where(x => physiciangrpids.Contains(x.PhysiciansGroupId)).Select(x => x.PhysicianId).ToList();
                physician = _db.Physicians.AsNoTracking().Where(x => physicianids.Contains(x.Id)).Select(p => new SelectListItem
                {
                    Value = p.Id.ToString(),
                    Text = p.FirstName + " " + p.LastName
                });
            }


            var condition1List = await _db.Patient_ChronicCondition1s.OrderBy(cc => cc.ChronicCondition1Type).ToListAsync();
            var condition2List = await _db.Patient_ChronicCondition2s.OrderByDescending(cc => cc.ChronicCondition2Type).ToListAsync();
            var BillingCategories = _db.BillingCategories.ToList();
            ViewBag.BillingCategories = BillingCategories;
            ViewBag.BillingCategoriesJson = _db.BillingCategories.Select(x => x.BillingCategoryId).ToList();
            var translator = _db.Liaisons.Where(x => x.IsTranslator == true).Select(p => new SelectListItem
            {
                Value = p.Id.ToString(),
                Text = p.FirstName + " " + p.LastName
            });
            var liason = _db.Liaisons.Where(x => x.IsTranslator == false).Select(p => new SelectListItem
            {
                Value = p.Id.ToString(),
                Text = p.FirstName + " " + p.LastName
            });

            ViewBag.Translator = new SelectList(translator, "Value", "Text").OrderBy(p => p.Text);
            ViewBag.Liason = new SelectList(liason, "Value", "Text").OrderBy(p => p.Text);
            ViewBag.RpmServices = _db.RPMServices.Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.ServiceName }).ToList();

            ViewBag.Physicians = new SelectList(physician.OrderBy(p => p.Text), "Value", "Text").OrderBy(p => p.Text);
            ViewBag.PatientChronicCondition1Id = new SelectList(condition1List, "Id", "ChronicCondition1Type");
            ViewBag.PatientChronicCondition2Id = new SelectList(condition2List, "Id", "ChronicCondition2Type");
            ViewBag.EnrollmentStatus = new SelectList(_db.EnrollmentStatuss.ToList(), "Id", "Name", "Eligibility");
            ViewBag.EnrollmentSubStatus = new SelectList(_db.EnrollmentSubStatuss.ToList(), "EnrollmentStatusID", "Name");
            ViewBag.EnrollemntStatusReson = new SelectList(_db.EnrollmentSubstatusReasons.ToList(), "Name", "Name");
            if (User.IsInRole("Sales"))
                ViewBag.ReviewId = HelperExtensions.ReviewTimeGetForSales("Adding Patient", GetUserId());

            ViewBag.Laisons = _db.Liaisons.Where(p => p.IsTranslator == false).Include(p => p.Liaisons_BillingCategories).Select(p => p).ToList();
            ViewBag.Translators = _db.Liaisons.Where(p => p.IsTranslator == true).Include(p => p.Liaisons_BillingCategories).Select(p => p).ToList();
            if (!string.IsNullOrEmpty(rda)) TempData["prda"] = "TotalPatients";

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Exclude = "Photo, PhotoEmrRecords,PhotoEmrRecords2,PhotoEmrRecords3,PhotoEmrRecords4,PhotoEmrRecords5,PhotoEmrRecords6,PhotoEmrRecords7,PhotoEmrRecords8,PhotoEmrRecords9,PhotoEmrRecords10,PhotoEmrRecords11,PhotoEmrRecords12,PhotoEmrRecords13,PhotoEmrRecords14,PhotoEmrRecords15")]
                        Patient patient, FormCollection form, string ReviewID, string EnrollmentStatushiden,
                            string EnrollmentSubStatushiden, string EnrollmentSubStatusReasonhiden, /*string EnroledCatagories,*/
                                                                                                    /*string[] billingCategory,*/ string EnrollmentList, int[] RpmServices)
        {

            List<int> RpmServiceList = RpmServices != null ? RpmServices.ToList() : new List<int>();
            List<PatientBillingCategoryViewModel> newList = JsonConvert.DeserializeObject<List<PatientBillingCategoryViewModel>>(EnrollmentList);
            List<EnrollmentListViewModel> BillingCatagoriesList = new List<EnrollmentListViewModel>();


            if (ModelState.IsValid)
            {

                patient.EnrollmentStatus = EnrollmentStatushiden == "" ? null : EnrollmentStatushiden;
                patient.EnrollmentSubStatus = EnrollmentSubStatushiden == "" ? null : EnrollmentSubStatushiden;
                if (patient?.EnrollmentSubStatus == "In-Active Enrolled")
                {
                    patient.EnrollmentSubStatusReason = EnrollmentSubStatusReasonhiden;
                }
                else
                {
                    patient.EnrollmentSubStatusReason = "";
                }

                #region imageuploadCode
                var postedImageFile = Request.Files["Photo"];
                var postedEmrImage = Request.Files["PhotoEmrRecords"];
                var postedEmrImage2 = Request.Files["PhotoEmrRecords2"];
                var postedEmrImage3 = Request.Files["PhotoEmrRecords3"];
                var postedEmrImage4 = Request.Files["PhotoEmrRecords4"];
                var postedEmrImage5 = Request.Files["PhotoEmrRecords5"];
                var postedEmrImage6 = Request.Files["PhotoEmrRecords6"];
                var postedEmrImage7 = Request.Files["PhotoEmrRecords7"];
                var postedEmrImage8 = Request.Files["PhotoEmrRecords8"];
                var postedEmrImage9 = Request.Files["PhotoEmrRecords9"];
                var postedEmrImage10 = Request.Files["PhotoEmrRecords10"];
                var postedEmrImage11 = Request.Files["PhotoEmrRecords11"];
                var postedEmrImage12 = Request.Files["PhotoEmrRecords12"];
                var postedEmrImage13 = Request.Files["PhotoEmrRecords13"];
                var postedEmrImage14 = Request.Files["PhotoEmrRecords14"];
                var postedEmrImage15 = Request.Files["PhotoEmrRecords15"];

                if (postedImageFile?.ContentLength != 0 && postedImageFile?.InputStream != null)
                    using (var binary = new BinaryReader(postedImageFile.InputStream))
                    {
                        var imageData = binary.ReadBytes(postedImageFile.ContentLength);
                        if (imageData.Length > 0)
                            patient.Photo = imageData;
                    }

                if (postedEmrImage?.ContentLength != 0 && postedEmrImage?.InputStream != null)
                    using (var binaryEmr = new BinaryReader(postedEmrImage.InputStream))
                    {
                        var emrImageData = binaryEmr.ReadBytes(postedEmrImage.ContentLength);
                        if (emrImageData.Length > 0)
                            patient.PhotoEmrRecords = emrImageData;
                    }

                if (postedEmrImage2?.ContentLength != 0 && postedEmrImage2?.InputStream != null)
                    using (var binaryEmr2 = new BinaryReader(postedEmrImage2.InputStream))
                    {
                        var emrImageData2 = binaryEmr2.ReadBytes(postedEmrImage2.ContentLength);
                        if (emrImageData2.Length > 0)
                            patient.PhotoEmrRecords2 = emrImageData2;
                    }

                if (postedEmrImage3?.ContentLength != 0 && postedEmrImage3?.InputStream != null)
                    using (var binaryEmr3 = new BinaryReader(postedEmrImage3.InputStream))
                    {
                        var emrImageData3 = binaryEmr3.ReadBytes(postedEmrImage3.ContentLength);
                        if (emrImageData3.Length > 0)
                            patient.PhotoEmrRecords3 = emrImageData3;
                    }

                if (postedEmrImage4?.ContentLength != 0 && postedEmrImage4?.InputStream != null)
                    using (var binaryEmr4 = new BinaryReader(postedEmrImage4.InputStream))
                    {
                        var emrImageData4 = binaryEmr4.ReadBytes(postedEmrImage4.ContentLength);
                        if (emrImageData4.Length > 0)
                            patient.PhotoEmrRecords4 = emrImageData4;
                    }

                if (postedEmrImage5?.ContentLength != 0 && postedEmrImage5?.InputStream != null)
                    using (var binaryEmr5 = new BinaryReader(postedEmrImage5.InputStream))
                    {
                        var emrImageData5 = binaryEmr5.ReadBytes(postedEmrImage5.ContentLength);
                        if (emrImageData5.Length > 0)
                            patient.PhotoEmrRecords5 = emrImageData5;
                    }

                if (postedEmrImage6?.ContentLength != 0 && postedEmrImage6?.InputStream != null)
                    using (var binaryEmr6 = new BinaryReader(postedEmrImage6.InputStream))
                    {
                        var emrImageData6 = binaryEmr6.ReadBytes(postedEmrImage6.ContentLength);
                        if (emrImageData6.Length > 0)
                            patient.PhotoEmrRecords6 = emrImageData6;
                    }
                //NewOne
                if (postedEmrImage7?.ContentLength != 0 && postedEmrImage7?.InputStream != null)
                    using (var binaryEmr7 = new BinaryReader(postedEmrImage7.InputStream))
                    {
                        var emrImageData7 = binaryEmr7.ReadBytes(postedEmrImage7.ContentLength);
                        if (emrImageData7.Length > 0)
                            patient.PhotoEmrRecords7 = emrImageData7;
                    }
                if (postedEmrImage8?.ContentLength != 0 && postedEmrImage8?.InputStream != null)
                    using (var binaryEmr8 = new BinaryReader(postedEmrImage8.InputStream))
                    {
                        var emrImageData8 = binaryEmr8.ReadBytes(postedEmrImage8.ContentLength);
                        if (emrImageData8.Length > 0)
                            patient.PhotoEmrRecords8 = emrImageData8;
                    }
                if (postedEmrImage9?.ContentLength != 0 && postedEmrImage9?.InputStream != null)
                    using (var binaryEmr9 = new BinaryReader(postedEmrImage9.InputStream))
                    {
                        var emrImageData9 = binaryEmr9.ReadBytes(postedEmrImage9.ContentLength);
                        if (emrImageData9.Length > 0)
                            patient.PhotoEmrRecords9 = emrImageData9;
                    }
                if (postedEmrImage10?.ContentLength != 0 && postedEmrImage10?.InputStream != null)
                    using (var binaryEmr10 = new BinaryReader(postedEmrImage10.InputStream))
                    {
                        var emrImageData10 = binaryEmr10.ReadBytes(postedEmrImage10.ContentLength);
                        if (emrImageData10.Length > 0)
                            patient.PhotoEmrRecords10 = emrImageData10;
                    }
                if (postedEmrImage11?.ContentLength != 0 && postedEmrImage11?.InputStream != null)
                    using (var binaryEmr11 = new BinaryReader(postedEmrImage11.InputStream))
                    {
                        var emrImageData11 = binaryEmr11.ReadBytes(postedEmrImage11.ContentLength);
                        if (emrImageData11.Length > 0)
                            patient.PhotoEmrRecords11 = emrImageData11;
                    }
                if (postedEmrImage12?.ContentLength != 0 && postedEmrImage12?.InputStream != null)
                    using (var binaryEmr12 = new BinaryReader(postedEmrImage12.InputStream))
                    {
                        var emrImageData12 = binaryEmr12.ReadBytes(postedEmrImage12.ContentLength);
                        if (emrImageData12.Length > 0)
                            patient.PhotoEmrRecords12 = emrImageData12;
                    }
                if (postedEmrImage13?.ContentLength != 0 && postedEmrImage13?.InputStream != null)
                    using (var binaryEmr13 = new BinaryReader(postedEmrImage13.InputStream))
                    {
                        var emrImageData13 = binaryEmr13.ReadBytes(postedEmrImage13.ContentLength);
                        if (emrImageData13.Length > 0)
                            patient.PhotoEmrRecords13 = emrImageData13;
                    }
                if (postedEmrImage14?.ContentLength != 0 && postedEmrImage14?.InputStream != null)
                    using (var binaryEmr14 = new BinaryReader(postedEmrImage14.InputStream))
                    {
                        var emrImageData14 = binaryEmr14.ReadBytes(postedEmrImage14.ContentLength);
                        if (emrImageData14.Length > 0)
                            patient.PhotoEmrRecords14 = emrImageData14;
                    }
                if (postedEmrImage15?.ContentLength != 0 && postedEmrImage15?.InputStream != null)
                    using (var binaryEmr15 = new BinaryReader(postedEmrImage15.InputStream))
                    {
                        var emrImageData15 = binaryEmr15.ReadBytes(postedEmrImage15.ContentLength);
                        if (emrImageData15.Length > 0)
                            patient.PhotoEmrRecords15 = emrImageData15;
                    }
                //
                #endregion

                var address = patient.Address1 + " " + form["street"];
                patient.Address1 = address;

                patient.CreatedOn = DateTime.Now;


                if (!string.IsNullOrEmpty(patient.LiaisonId.ToString()))
                {
                    patient.LiasionAssignedBy = GetUserId();
                    patient.LiasionAssignedOn = DateTime.Now;
                }
                if (!string.IsNullOrEmpty(patient.TranslatorId.ToString()))
                {
                    patient.TranslatorAssignedBy = GetUserId();
                    patient.TranslatorAssignedOn = DateTime.Now;
                }

                patient.CreatedBy = GetUserId();
                patient.UpdatedOn = DateTime.Now;


                patient.Physician = await _db.Physicians.FindAsync(patient.PhysicianId);
                if (patient.EnrollmentSubStatus == "Active Enrolled")
                {
                    if (patient.CCMEnrolledOn == null)
                    {
                        patient.CcmStatus = "Enrolled";
                        patient.CCMEnrolledOn = DateTime.Now;
                        patient.CCMEnrolledBy = GetUserId();

                    }
                }

                try
                {
                    var Patients_BillingCategoriesList = new List<Patients_BillingCategories>();


                    var ccmbillingCatgoryID = BillingCodeHelper.cmmBillingCatagoryid;

                    foreach (var patientcategory in newList)
                    {
                        var PatientBillingcatagory = new Patients_BillingCategories();
                        int BillingCategoryId = patientcategory.BillingcategoryId.GetInteger();

                        if (BillingCategoryId == ccmbillingCatgoryID)
                        {

                            patient.LiaisonId = patientcategory.LiaisonId.GetInteger();
                            if (!string.IsNullOrEmpty(patientcategory.TranslatorId))
                            {
                                patient.TranslatorId = patientcategory.TranslatorId.GetNullableInteger();

                            }
                        }
                        PatientBillingcatagory.BillingCategoryId = BillingCategoryId;
                        PatientBillingcatagory.CreatedOn = DateTime.Now;
                        PatientBillingcatagory.CreatedBy = GetUserId();
                        PatientBillingcatagory.EnrolledOn = DateTime.Now;
                        PatientBillingcatagory.Status = true;
                        PatientBillingcatagory.LiaisonId = patientcategory.LiaisonId.GetInteger();
                        if (!string.IsNullOrEmpty(patientcategory.TranslatorId))
                        {
                            PatientBillingcatagory.TranslatorId = patientcategory.TranslatorId.GetInteger();
                            PatientBillingcatagory.IsTranslator = true;
                        }
                        Patients_BillingCategoriesList.Add(PatientBillingcatagory);

                        #region old code for assigning translater
                        //if (!string.IsNullOrEmpty(patientcategory.TranslatorId))
                        //{
                        //    var PatientBillingcatagorytranslator = new Patients_BillingCategories();
                        //    PatientBillingcatagorytranslator.BillingCategoryId = patientcategory.BillingcategoryId.GetInteger();
                        //    PatientBillingcatagorytranslator.CreatedOn = DateTime.Now;
                        //    PatientBillingcatagorytranslator.CreatedBy = GetUserId();
                        //    PatientBillingcatagorytranslator.EnrolledOn = DateTime.Now;
                        //    PatientBillingcatagorytranslator.Status = true;
                        //    PatientBillingcatagorytranslator.IsTranslator = true;
                        //    if (!string.IsNullOrEmpty(patientcategory.TranslatorId))
                        //    {
                        //        PatientBillingcatagorytranslator.LiaisonId = patientcategory.TranslatorId.GetInteger();
                        //    }

                        //    Patients_BillingCategoriesList.Add(PatientBillingcatagorytranslator);
                        //}
                        #endregion

                    }

                    patient.Patients_BillingCategories = Patients_BillingCategoriesList;

                    _db.Patients.Add(patient);

                    _db.SaveChanges();
                    //patient.Cycle = CategoryCycleStatusHelper.GetPatientNewOrOldCycleByCategory(patient.Id);
                    CategoryCycleStatusHelper.User = User;
                    patient.Cycle = CategoryCycleStatusHelper.GetPatientNewOrOldCycleByCategory(patient.Id, BillingCodeHelper.cmmBillingCatagoryid);
                    _db.Entry(patient).State = EntityState.Modified;
                    _db.SaveChanges();
                    var billingcategory = _db.BillingCategories.Where(x => x.BillingPeriodsId == BillingCodeHelper.MonthalyPeriodId).ToList();
                    billingcategory.ForEach(b =>
                    {
                        CategoryCycleStatusHelper.User = User;
                        var cyclestatus = CategoryCycleStatusHelper.GetPatientNewOrOldCycleStatusbyCategory(patient.Id, b.BillingCategoryId,null);


                    });

                    var categorylist = newList.Where(x => x.BillingcategoryId.GetInteger() != BillingCodeHelper.cmmBillingCatagoryid).ToList();
                    foreach (var patientcategory in categorylist)
                    {
                        int BillingCategoryId = patientcategory.BillingcategoryId.GetInteger();
                        var bcategory = _db.BillingCategories.Where(x => x.BillingCategoryId == BillingCategoryId).FirstOrDefault();
                        #region Saving RPM Service
                        if (BillingCategoryId == BillingCodeHelper.RPMBillingCatagoryid)
                        {
                            try
                            {
                                RpmServiceList.ForEach(x =>
                                {
                                    var rpmservice = x.GetRPMServivceById();
                                    var PatientRpmSevice = new Patients_Services();
                                    PatientRpmSevice.IsActive = (int)IsActiveStatus.Active;
                                    PatientRpmSevice.Patient = patient;
                                    PatientRpmSevice.PatientId = patient.Id;
                                    //PatientRpmSevice.RPMService = rpmservice;
                                    PatientRpmSevice.RPMServiceId = rpmservice.Id;
                                    PatientRpmSevice.CreatedBy = GetUserId();
                                    PatientRpmSevice.CreatedOn = DateTime.Now;
                                    _db.Patients_Services.Add(PatientRpmSevice);
                                    _db.SaveChanges();

                                });
                            }
                            catch (Exception e)
                            {
                                throw e;
                            }

                        }
                        #endregion
                        if (bcategory != null && bcategory.BillingPeriodsId != BillingCodeHelper.MonthalyPeriodId)
                        {
                            CategoryCycleStatusHelper.User = User;
                            CategoryCycleStatusHelper.GetPatientNewOrOldCycleStatusbyCategory(patient.Id, BillingCategoryId,null);
                        }

                    }

                    if (patient.EnrollmentSubStatus != "Active Enrolled")
                    {
                        try
                        {
                            var Patients_PreLiaison = new Patients_PreLiaisons();
                            Patients_PreLiaison.LiaisonId = patient.LiaisonId;
                            Patients_PreLiaison.TranslatorId = patient.TranslatorId;
                            Patients_PreLiaison.Status = true;
                            Patients_PreLiaison.CreatedOn = DateTime.Now.Date;
                            Patients_PreLiaison.CreatedBy = GetUserId();
                            _db.Patients_PreLiaisons.Add(Patients_PreLiaison);
                            _db.SaveChanges();
                            patient.Patients_PreLiaisonsId = Patients_PreLiaison.Id;
                            patient.TranslatorId = null;
                            patient.LiaisonId = null;
                            _db.Entry(patient).State = EntityState.Modified;
                            _db.SaveChanges();
                        }
                        catch (Exception e)
                        {
                            throw e;
                        }

                    }
                    if (!string.IsNullOrEmpty(ReviewID))
                    {
                        var review = _db.ReviewTimeCcms.Find(Convert.ToInt32(ReviewID));
                        review.PatientId = patient.Id;
                        review.Cycle = patient.Cycle;
                        _db.Entry(review).State = EntityState.Modified;
                        _db.SaveChanges();
                    }

                    //  transaction.Commit();

                    if (!string.IsNullOrEmpty(TempData["prda"]?.ToString() ?? string.Empty))
                        return RedirectToAction(nameof(TotalPatients));

                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    // transaction.Rollback();
                    throw ex;
                }
                //    }
                //}   
            }
            else
            {
                var error = ModalStateValidator.ValidateModelState(ModelState);
            }


            var physician = _db.Physicians.Select(p => new SelectListItem
            {
                Value = p.Id.ToString(),
                Text = p.FirstName + " " + p.LastName
            });
            var condition1List = await _db.Patient_ChronicCondition1s.OrderBy(cc => cc.ChronicCondition1Type).ToListAsync();
            var condition2List = await _db.Patient_ChronicCondition2s.OrderByDescending(cc => cc.ChronicCondition2Type).ToListAsync();

            ViewBag.Physicians = new SelectList(physician.OrderBy(p => p.Text), "Value", "Text");
            ViewBag.PatientChronicCondition1Id = new SelectList(condition1List, "Id", "ChronicCondition1Type");
            ViewBag.PatientChronicCondition2Id = new SelectList(condition2List, "Id", "ChronicCondition2Type");

            return View(patient);
        }


        [Authorize(Roles = "Liaison, Admin")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var patient = _db.Patients.Find(id);
            if (patient == null)
            {
                return HttpNotFound();
            }

            ViewBag.PatientChronicCondition1Id = new SelectList(_db.Patient_ChronicCondition1s, "Id", "ChronicCondition1Type", patient.PatientChronicCondition1Id);
            ViewBag.PatientChronicCondition2Id = new SelectList(_db.Patient_ChronicCondition2s, "Id", "ChronicCondition2Type", patient.PatientChronicCondition2Id);
            ViewBag.EnrollmentStatus = new SelectList(_db.EnrollmentStatuss.ToList(), "Id", "Name", patient.EnrollmentStatus);
            ViewBag.EnrollmentSubStatus = new SelectList(_db.EnrollmentSubStatuss.ToList(), "EnrollmentStatusID", "Name");
            ViewBag.EnrollemntStatusReson = new SelectList(_db.EnrollmentSubstatusReasons.ToList(), "Name", "Name");
            return View(patient);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Patient patient)
        {
            if (ModelState.IsValid)
            {
                _db.Entry(patient).State = EntityState.Modified;
                _db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.PatientChronicCondition1Id = new SelectList(_db.Patient_ChronicCondition1s, "Id", "ChronicCondition1Type", patient.PatientChronicCondition1Id);
            ViewBag.PatientChronicCondition2Id = new SelectList(_db.Patient_ChronicCondition2s, "Id", "ChronicCondition2Type", patient.PatientChronicCondition2Id);

            return View(patient);
        }


        [Authorize(Roles = "Admin")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var patient = _db.Patients.Find(id);
            if (patient == null)
                return HttpNotFound();

            return View(patient);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var patient = _db.Patients.Find(id);
            if (patient != null)
            {
                var patientdata = _db.Patients_BillingCategories.Where(p => p.PatientId == id).ToList();
                foreach (var item in patientdata)
                {
                    _db.Patients_BillingCategories.Remove(item);
                    _db.SaveChanges();

                }
                _db.Patients.Remove(patient);
                _db.SaveChanges();
            }

            return RedirectToAction("TotalPatients");
        }


        public async Task<ActionResult> DisplayImage(int patientId, string imageName)
        {
            var patient = await _db.Patients.FindAsync(patientId);

            return File(!string.IsNullOrEmpty(imageName) &&
                         imageName == "photo" ? patient?.Photo
                       : imageName == "record1" ? patient?.PhotoEmrRecords
                       : imageName == "record2" ? patient?.PhotoEmrRecords2
                       : imageName == "record3" ? patient?.PhotoEmrRecords3
                       : imageName == "record4" ? patient?.PhotoEmrRecords4
                       : imageName == "record5" ? patient?.PhotoEmrRecords5
                        : imageName == "record7" ? patient?.PhotoEmrRecords7
                         : imageName == "record8" ? patient?.PhotoEmrRecords8
                          : imageName == "record9" ? patient?.PhotoEmrRecords9
                           : imageName == "record10" ? patient?.PhotoEmrRecords10
                            : imageName == "record11" ? patient?.PhotoEmrRecords11
                             : imageName == "record12" ? patient?.PhotoEmrRecords12
                              : imageName == "record13" ? patient?.PhotoEmrRecords13
                               : imageName == "record14" ? patient?.PhotoEmrRecords14
                                : imageName == "record15" ? patient?.PhotoEmrRecords15
                       : null, "image/jpeg");
        }


        public ActionResult PhotoEmrRecords6(int patientId)
        {
            ActionResult result = null;
            var pdf = _db.Patients.Find(patientId)?.PhotoEmrRecords6;
            var ms = pdf != null ? new MemoryStream(pdf) : null;
            if (ms != null)
                result = new FileStreamResult(ms, "application/pdf");
            return result;
        }

        public async Task<PartialViewResult> PatientInsuranceInfo(int patientId)
        {
            var patient = _db.Patients.Find(patientId);
            var insurance = patient?.InsuranceId != null
                          ? await _db.PatientProfile_Insurance.FindAsync(patient.InsuranceId)
                          : new PatientProfile_Insurance { PatientId = patientId };
            return PartialView(insurance);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _db.Dispose();
            }
            base.Dispose(disposing);
        }

        public string ValidateBulkChanges(int[] Patients)
        {
            List<int?> EnrolledCategories = new List<int?>();
            bool isActiveEnroll = false;
            bool isOther = false;
            bool Check = false;
            bool CheckSameBillingCategories = false;
            int count = -1;
            if (Patients.Length > 0)
            {
                foreach (var item in Patients)
                {
                    var patient = _db.Patients.Where(x => x.Id == item).FirstOrDefault();
                    if (patient.EnrollmentSubStatus == "Active Enrolled")
                    {
                        isActiveEnroll = true;
                        if (EnrolledCategories.Count == 0)
                        {
                            patient.Patients_BillingCategories.Where(x => x.Status == true).Distinct()
                            //.GroupBy(x =>x.BillingCategoryId)
                            //.FirstOrDefault()
                            .ToList().ForEach(x =>
                            {
                                EnrolledCategories.Add(x.BillingCategoryId);
                            });

                        }
                        if (count >= 0)
                        {
                            List<int?> Match = patient.Patients_BillingCategories.Where(x => x.Status == true).Select(x => x.BillingCategoryId).ToList().OrderBy(x => x.Value).ToList();
                            var check1 = EnrolledCategories.Except(Match).ToList();
                            var check2 = Match.Except(EnrolledCategories).ToList();
                            if (check1.Count != 0 || check2.Count != 0)
                            {
                                CheckSameBillingCategories = true;
                                break;
                            }
                        }
                        count++;
                        if (isOther == true)
                        {
                            Check = true;
                            break;
                        }
                    }
                    else
                    {
                        isOther = true;
                        if (isActiveEnroll == true)
                        {
                            Check = true;
                            break;
                        }
                    }
                }
                if (CheckSameBillingCategories == true)
                {
                    return "NotSameCategoreis";
                }
                if (Check == true)
                {
                    return "false";
                }
                if (isActiveEnroll == true && isOther == false)
                {
                    var JsonResult = JsonConvert.SerializeObject(EnrolledCategories.Distinct());
                    return JsonResult;
                }
                else if (isActiveEnroll == false && isOther == true)
                {
                    return "true";
                }
                else
                {
                    return "false";
                }
            }

            return "";

        }
        /// <summary>  
        /// Override the JSON Result with Max integer JSON lenght  
        /// </summary>  
        /// <param name="data">Data</param>  
        /// <param name="contentType">Content Type</param>  
        /// <param name="contentEncoding">Content Encoding</param>  
        /// <param name="behavior">Behavior</param>  
        /// <returns>As JsonResult</returns>  
        //protected override JsonResult Json(object data, string contentType,
        //    Encoding contentEncoding, JsonRequestBehavior behavior)
        //{
        //    return new JsonResult()
        //    {
        //        Data = data,
        //        ContentType = contentType,
        //        ContentEncoding = contentEncoding,
        //        JsonRequestBehavior = behavior,
        //        MaxJsonLength = Int32.MaxValue
        //    };
        //}



        public JsonResult PatientBulkChanges(int[] Patients, string ChangeStatus, string EnrollinList, string EnrollmentStatus, string EnollmentSubStatus, string EnrollemntStatusReson, int? PreLiaisonId, int? PreTranslaterId, string MigrateAppointment, int[] MigrateAppointmentIn, string postConsolerTranslaterList, string DeEnrollmentReasonsList)
        {

            TempBulkChangeViewModel bulkChangesLogs = new TempBulkChangeViewModel();

            BulkChangeViewModel bulkChangeViewModel = new BulkChangeViewModel();
            bulkChangeViewModel.PatientsList = Patients.ToList();
            bulkChangeViewModel.ChangeStatus = ChangeStatus;
            bulkChangeViewModel.EnrollmentStatus = EnrollmentStatus;
            bulkChangeViewModel.EnollmentSubStatus = EnollmentSubStatus;
            bulkChangeViewModel.EnrollemntStatusReson = EnrollemntStatusReson;
            bulkChangeViewModel.PreLiaisonId = PreLiaisonId;
            bulkChangeViewModel.PreTranslaterId = PreTranslaterId;
            bulkChangeViewModel.MigrateAppointment = MigrateAppointment;
            bulkChangeViewModel.MigrateAppointmentIn_Id = MigrateAppointmentIn.ToList().Count() > 0 ? MigrateAppointmentIn.ToList() : null;
            bulkChangeViewModel.EnrollementList = new List<PatientBillingCategoryViewModel>();
            List<PatientBillingCategoryViewModel> Erolledlist = JsonConvert.DeserializeObject<List<PatientBillingCategoryViewModel>>(EnrollinList);
            Erolledlist.ForEach(x =>
            {
                if (bulkChangeViewModel.EnrollementList.Where(c => c.BillingcategoryId == x.BillingcategoryId).FirstOrDefault() == null)
                {
                    bulkChangeViewModel.EnrollementList.Add(x);
                }
            });

            bulkChangeViewModel.PostCounselorTranslatorList = new List<PatientBillingCategoryViewModel>();
            List<PatientBillingCategoryViewModel> PostList = JsonConvert.DeserializeObject<List<PatientBillingCategoryViewModel>>(postConsolerTranslaterList);
            PostList.ForEach(x =>
            {
                if (bulkChangeViewModel.PostCounselorTranslatorList.Where(c => c.BillingcategoryId == x.BillingcategoryId).FirstOrDefault() == null)
                {
                    bulkChangeViewModel.PostCounselorTranslatorList.Add(x);
                }
            });
            bulkChangeViewModel.DeEnrollmentReason = JsonConvert.DeserializeObject<List<DeEnrollmentReasonViewModel>>(DeEnrollmentReasonsList);
            BulkChangesModel bulkChanges = new BulkChangesModel();
            bulkChanges.User = User;

            if (ChangeStatus == "Yes")
            {
                bulkChangesLogs = bulkChanges.ChangeStatusOnly(bulkChangeViewModel);
                //if (EnrollmentStatus == "Enrolled" && EnollmentSubStatus == "Active Enrolled")
                //{
                //bulkChangesLogs = bulkChanges.EnrollPatients(bulkChangeViewModel);
                //}
                //else
                //{
                //    //EnrollmentStatus != "Enrolled" && EnollmentSubStatus != "Active Enrolled"

                //    if (bulkChangeViewModel.DeEnrollmentReason.Count() > 0)
                //    {
                //        //bulkChangesLogs = bulkChanges.DeEnrollPatients(bulkChangeViewModel);
                //    }
                //    else
                //    {
                //        //bulkChangesLogs = bulkChanges.ChangeStatusOnly(bulkChangeViewModel);
                //    }

                //}
            }
            else
            {
                if (bulkChangeViewModel.PostCounselorTranslatorList.Count() > 0)
                {
                    //Selected Patients are Enrolled 
                    bulkChangesLogs = bulkChanges.ChangePostCounselorForEnrolledPatients(bulkChangeViewModel);
                }
                else
                {
                    //selected Patients are not active Enrolled 
                    bulkChangesLogs = bulkChanges.ChnagePreCounselorOfNotEnrolledPatients(bulkChangeViewModel);
                }

            }


            string result = "";
            if (bulkChangesLogs.BulkChangesLogList.Count() != 0)
            {
                int id = bulkChanges.SaveBulkChangeLog(bulkChangesLogs);
                result = "Operation Performed " + bulkChangesLogs.Title + "\n " +
               "Total Selected Patients : " + bulkChangeViewModel.PatientsList.Count() + "\n " +
               "Successful : " + bulkChangesLogs.BulkChangesLogList.Where(x => x.Status == (int)BulkChangesStatus.success).GroupBy(x => x.PatientId).ToList().Count() + "\n " +
               "Failed : " + bulkChangesLogs.BulkChangesLogList.Where(x => x.Status == (int)BulkChangesStatus.Failed).GroupBy(x => x.PatientId).ToList().Count() + "\n " +
               "See Bulk Changes Logs (Id=" + id + ") for more Details";
                return Json(result, JsonRequestBehavior.AllowGet);

            }
            else
            {
                result = "Saving Failed";
                return Json(result, JsonRequestBehavior.AllowGet);
            }

        }



        private void ShowConsoleMassage(string text)
        {
            Debug.WriteLine(text);
        }
        [Authorize(Roles = "Admin")]
        public ActionResult Bulkchanges()
        {


            var BulkChangesdata = _db.BulkChangeses.ToList();
            var BulkChangesDataList = new List<BulkChangesViewModel>();

            foreach (var item in BulkChangesdata)
            {
                BulkChangesViewModel Model = new BulkChangesViewModel();
                Model.CreatedOn = item.CreatedOn.ToString("MM/dd/yyyy");
                Model.CreatedBy = HelperExtensions.GetUserNamebyID(item.CreatedBy);
                Model.id = item.id;
                Model.Title = item.Title;
                BulkChangesDataList.Add(Model);
            }
            BulkChangesDataList.OrderByDescending(x => x.id);
            return View(BulkChangesDataList);
        }

        [Authorize(Roles = "Admin")]
        public ActionResult GetBulkchangesData(string to, string from, int? CounsolerId = 0, int TranslaterId = 0)
        {
            var toDate = new DateTime();
            var fromDate = new DateTime();
            if (!string.IsNullOrEmpty(to))
            {
                toDate = Convert.ToDateTime(to);

            }
            if (!string.IsNullOrEmpty(from))
            {
                fromDate = Convert.ToDateTime(from);
            }

            var BulkChangesdata = new List<BulkChange>();
            if (!string.IsNullOrEmpty(to) && string.IsNullOrEmpty(from))
            {
                BulkChangesdata = _db.BulkChangeses.Where(x => x.CreatedOn <= toDate).ToList();
            }
            else if (string.IsNullOrEmpty(to) && !string.IsNullOrEmpty(from))
            {
                BulkChangesdata = _db.BulkChangeses.Where(x => x.CreatedOn >= fromDate).ToList();
            }
            else if (!string.IsNullOrEmpty(to) && !string.IsNullOrEmpty(from))
            {
                BulkChangesdata = _db.BulkChangeses.Where(x => x.CreatedOn <= toDate && x.CreatedOn >= fromDate).ToList();
            }
            else
            {
                BulkChangesdata = _db.BulkChangeses.ToList();

            }

            var BulkChangesDataList = new List<BulkChangesViewModel>();

            foreach (var item in BulkChangesdata)
            {
                BulkChangesViewModel Model = new BulkChangesViewModel();
                Model.CreatedOn = item.CreatedOn.ToString("MM/dd/yyyy");
                Model.CreatedBy = HelperExtensions.GetUserNamebyID(item.CreatedBy);
                Model.id = item.id;
                Model.Title = item.Title;
                BulkChangesDataList.Add(Model);
            }
            BulkChangesDataList.OrderByDescending(x => x.id);
            return View(BulkChangesDataList);

        }

        [Authorize(Roles = "Admin")]
        public ActionResult BulkchangesLogs(int Id)
        {

            var BulkChangingLogData = _db.BulkChangesLogs.Where(x => x.BulkChangeId == Id).ToList();
            var BulkChangesLogDataList = new List<BulkChangesLogViewModel>();
            foreach (var item in BulkChangingLogData)
            {
                BulkChangesLogViewModel Model = new BulkChangesLogViewModel();
                Model.PatientId = item.PatientId;
                Model.ResultMassage = item.ResultMessage;
                //Model.Patient = item.PatientId
                Model.Status = item.Status;
                Model.Createdby = HelperExtensions.GetUserNamebyID(item.Createdby);
                Model.CreatedOn = item.CreatedOn.ToString("MM/dd/yyyy");
                BulkChangesLogDataList.Add(Model);
            }
            BulkChangesLogDataList.OrderByDescending(x => x.PatientId);
            ViewBag.BulkChangeTitle = _db.BulkChangeses.Where(x => x.id == Id).Select(x => x.Title).FirstOrDefault();
            return View(BulkChangesLogDataList);
        }
    }
    public class ErrorResult
    {
        public string ErrorMessage { get; set; }
        public string Field { get; set; }
    }

}
