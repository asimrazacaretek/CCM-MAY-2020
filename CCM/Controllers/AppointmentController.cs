using CCM.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace CCM.Controllers
{

    public class AppointmentController : BaseController
    {
        //private readonly ApplicationdbContect _db = new ApplicationdbContect();
        
        // log4net declaration
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);


        private ApplicationUserManager _userManager;
        public AppointmentController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            //_db.Configuration.ProxyCreationEnabled = false;
            UserManager = userManager;
        }
        public AppointmentController()
        {

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

        public async Task<ActionResult> Index()
        {
            try
            {  


                List<Liaison> Physician = _db.Liaisons.AsNoTracking().ToList();
                var query = (from a in Physician
                             select new Liaison
                             {
                                 FirstName = a.FirstName + " " + a.LastName + (a.IsTranslator == true ? " (Translator)" : " (Counselor)"),
                                 Id = a.Id
                             }).OrderBy(x => x.FirstName).ToList();
                List<SaleStaff> enroller = _db.saleStaffs.AsNoTracking().ToList();
                var query1 = (from a in enroller
                              select new Liaison
                              {
                                  FirstName = a.FirstName + " " + a.LastName + (" (Enroller)"),
                                  Id = a.Id
                              }).ToList();

                var user = _db.Users.Find(User.Identity.GetUserId());
                if (user.Role == "Liaison")
                {
                    query = query.Where(x => x.Id == user.CCMid).ToList();
                    ViewBag.DoctorId = user.CCMid;
                }
                if (User.IsInRole("Sales"))
                {
                    query = query1.Where(x => x.Id == user.CCMid).ToList();
                    ViewBag.DoctorId = user.CCMid;
                }
                if (User.IsInRole("PhysiciansGroup"))
                {
                    List<int> physicianids = new List<int>();
                    physicianids = _db.physicianGroup_Physician_Mappings.AsNoTracking().Where(x => x.PhysiciansGroupId == user.CCMid).Select(x => x.PhysicianId).ToList();
                    var liasionids = _db.Patients.AsNoTracking().Where(x => physicianids.Contains(x.PhysicianId.Value) && x.LiaisonId != null).Select(x => x.LiaisonId).Distinct().ToList();
                    query = query.Where(p => liasionids.Contains(p.Id)).ToList();
                }
                if (User.IsInRole("LiaisonGroup"))
                {

                    var liasionids = _db.LiaisonGroup_Liaison_Mappings.AsNoTracking().Where(x => x.LiaisonGroupId == user.CCMid).Select(x => x.LiaisonId).ToList();

                    query = query.Where(p => liasionids.Contains(p.Id)).ToList();
                }

                if (User.IsInRole("Admin"))
                {
                    query.AddRange(query1);
                }

                List<LiaisonDropdown> liaisonDropdowns = new List<LiaisonDropdown>();
                foreach (var item_L in query)
                {
                    if (item_L.FirstName.Contains("Enroller"))
                        liaisonDropdowns.Add(new LiaisonDropdown { SId = Convert.ToString("E" + item_L.Id), SName = item_L.FirstName });
                    else
                        liaisonDropdowns.Add(new LiaisonDropdown { SId = Convert.ToString("L" + item_L.Id), SName = item_L.FirstName });
                }


                ViewBag.AllDoctors = liaisonDropdowns.OrderBy(p => p.SName).ToList();

                //ViewBag.AllDoctors = query;
                ViewBag.DoctorId = 0;


                //ViewBag.AllDoctors = query;
                return View();
            }
            catch (Exception ex)
            {
                ViewBag.AllDoctors = new List<LiaisonDropdown>();
                ViewBag.DoctorId = 0;
                log.Error(Environment.NewLine + User.Identity.GetUserName() + "-------" + User.Identity.GetUserId() + Environment.NewLine + ex.Message + "-----" + ex.StackTrace);
                return View();
                /*return ex.Message + "------------------" + ex.StackTrace;*/
            }
        }

        public async Task<ActionResult> ManageSchedules(string ID)
        {
            try
            {
                List<Liaison> Physician = _db.Liaisons.AsNoTracking().ToList();
                var query = (from a in Physician
                             select new Liaison
                             {
                                 FirstName = a.FirstName + " " + a.LastName + (a.IsTranslator == true ? " (Translator)" : " (Counselor)"),
                                 Id = a.Id
                             }).ToList();

                List<SaleStaff> enroller = _db.saleStaffs.AsNoTracking().ToList();
                var query1 = (from a in enroller
                              select new Liaison
                              {
                                  FirstName = a.FirstName + " " + a.LastName + (" (Enroller)"),
                                  Id = a.Id
                              }).ToList();

                var user = _db.Users.Find(User.Identity.GetUserId());
                if (user.Role == "Liaison")
                {
                    query = query.Where(x => x.Id == user.CCMid).ToList();
                }
                if (User.IsInRole("Sales"))
                {
                    query = query1.Where(x => x.Id == user.CCMid).ToList();
                }
                if (User.IsInRole("PhysiciansGroup"))
                {
                    List<int> physicianids = new List<int>();
                    physicianids = _db.physicianGroup_Physician_Mappings.AsNoTracking().Where(x => x.PhysiciansGroupId == user.CCMid).Select(x => x.PhysicianId).ToList();
                    var liasionids = _db.Patients.AsNoTracking().Where(x => physicianids.Contains(x.PhysicianId.Value) && x.LiaisonId != null).Select(x => x.LiaisonId).Distinct().ToList();
                    query = query.Where(p => liasionids.Contains(p.Id)).ToList();
                }
                if (User.IsInRole("LiaisonGroup"))
                {

                    var liasionids = _db.LiaisonGroup_Liaison_Mappings.AsNoTracking().Where(x => x.LiaisonGroupId == user.CCMid).Select(x => x.LiaisonId).ToList();

                    query = query.Where(p => liasionids.Contains(p.Id)).ToList();
                }


                if (User.IsInRole("Admin"))
                {
                    query.AddRange(query1);
                }


                List<LiaisonDropdown> liaisonDropdowns = new List<LiaisonDropdown>();
                foreach (var item_L in query)
                {
                    if (item_L.FirstName.Contains("Enroller"))
                        liaisonDropdowns.Add(new LiaisonDropdown { SId = Convert.ToString("E" + item_L.Id), SName = item_L.FirstName });
                    else
                        liaisonDropdowns.Add(new LiaisonDropdown { SId = Convert.ToString("L" + item_L.Id), SName = item_L.FirstName });
                }


                ViewBag.SelectedLiaison = ID;
                ViewBag.AllDoctors = liaisonDropdowns.OrderBy(p => p.SName).ToList();
                //ViewBag.AllDoctors = query.OrderBy(p => p.FirstName).ToList();
                List<Clinic> clinics = new List<Clinic>();
                ViewBag.AllClinics = clinics;
                return View();
            }
            catch (Exception ex)
            {
                ViewBag.SelectedLiaison = 0;
                ViewBag.AllDoctors= new List<LiaisonDropdown>();


                log.Error(Environment.NewLine + User.Identity.GetUserName() + "-------" + User.Identity.GetUserId() + Environment.NewLine + ex.Message + "-----" + ex.StackTrace);
                return View();
                /*return ex.Message + "------------------" + ex.StackTrace;*/
            }
        }

        public async Task<ActionResult> Schedules()
        {
                var response = _db.doctorSchedules.AsNoTracking().ToList();
                var result = response;
            try
            {





                var user = _db.Users.Find(User.Identity.GetUserId());
                if (user.Role == "Liaison")
                {
                    result = result.Where(x => x.LiaisonID == user.CCMid).ToList();
                }
                if (user.Role == "Sales")
                {
                    result = result.Where(x => x.SaleStaffID == user.CCMid).ToList();
                }
                if (User.IsInRole("PhysiciansGroup"))
                {
                    List<int> physicianids = new List<int>();
                    physicianids = _db.physicianGroup_Physician_Mappings.AsNoTracking().Where(x => x.PhysiciansGroupId == user.CCMid).Select(x => x.PhysicianId).ToList();
                    var liasionids = _db.Patients.AsNoTracking().Where(x => physicianids.Contains(x.PhysicianId.Value) && x.LiaisonId != null).Select(x => x.LiaisonId).Distinct().ToList();
                    result = result.Where(p => liasionids.Contains(p.LiaisonID)).ToList();
                }
                if (User.IsInRole("LiaisonGroup"))
                {

                    var liasionids = _db.LiaisonGroup_Liaison_Mappings.AsNoTracking().Where(x => x.LiaisonGroupId == user.CCMid).Select(x => x.LiaisonId).ToList();

                    result = result.Where(p => liasionids.Contains(p.LiaisonID.Value)).ToList();
                }

                return View(result);
            }
            catch (Exception ex)
            {

                log.Error(Environment.NewLine + User.Identity.GetUserName() + "-------" + User.Identity.GetUserId() + Environment.NewLine + ex.Message + "-----" + ex.StackTrace);
                return View(result);
                /*return ex.Message + "------------------" + ex.StackTrace;*/
            }
        }

        public async Task<ActionResult> BookAppointment(int? OrderId, int PatientID = 0)
        {
            try
            {



                //GetPObyClinicID
                var response2 = _db.Patients.AsNoTracking().ToList().Select(x => new PatientsViewModelAppointment
                {
                    Description = x.Id.ToString() + " | " + x.FirstName + " " + x.LastName + " | DOB:" + x.BirthDate.ToString("MM/dd/yyyy"),
                    Id = x.Id
                }).Distinct().ToList();

                ViewBag.POByClinic = response2;


                List<Liaison> Physician = _db.Liaisons.AsNoTracking().ToList();
                var query = (from a in Physician
                             select new Liaison
                             {
                                 FirstName = a.FirstName + " " + a.LastName,
                                 Id = a.Id
                             }).ToList();
                var user = _db.Users.Find(User.Identity.GetUserId());
                if (user.Role == "Liaison")
                {
                    query = query.Where(x => x.Id == user.CCMid).ToList();
                    ViewBag.DoctorId = user.CCMid;
                }
                else
                {
                    ViewBag.DoctorId = query.Count > 0 ? query.FirstOrDefault().Id : 0;
                }

                ViewBag.AllDoctors = query;
                ViewBag.Hours = new List<int> { 1, 2, 3, 4, 5, 6, 7 };

                return View();
            }
            catch (Exception ex)
            {
                ViewBag.AllDoctors = _db.Liaisons.AsNoTracking().ToList();
                ViewBag.Hours = new List<int> { 1, 2, 3, 4, 5, 6, 7 };
                log.Error(Environment.NewLine + User.Identity.GetUserName() + "-------" + User.Identity.GetUserId() + Environment.NewLine + ex.Message + "-----" + ex.StackTrace);
                return View();
                /*return ex.Message + "------------------" + ex.StackTrace;*/
            }
        }
        [HttpPost]
        public PartialViewResult _BookAppointment(int? OrderId, int PatientID = 0,string ForEnroller="")
        {
            try
            {
                //GetPObyClinicID
                var response2 = _db.Patients.AsNoTracking().Where(x => x.Id == PatientID).ToList().Select(x => new PatientsViewModelAppointment
                {
                    Description = x.Id.ToString() + " | " + x.FirstName + " " + x.LastName + " | " + x.BirthDate.ToString("MM/dd/yyyy"),
                    Id = x.Id
                }).ToList();

                ViewBag.POByClinic = response2;


                List<Liaison> Physician = _db.Liaisons.AsNoTracking().ToList();
                var query = (from a in Physician

                             select new Liaison
                             {
                                 FirstName = a.FirstName + " " + a.LastName + (a.IsTranslator == true ? " (Translator)" : " (Counselor)"),
                                 Id = a.Id
                             }).ToList();
                var patient = _db.Patients.AsNoTracking().Where(x => x.Id == PatientID).FirstOrDefault();
                var query1 = new List<Liaison>();
                if (patient?.LiaisonId != null)
                {
                    query1 = query.Where(x => x.Id == patient.LiaisonId).ToList();
                }
                if (patient?.TranslatorId != null)
                {
                    query1.AddRange(query.Where(x => x.Id == patient.TranslatorId).ToList());
                }


                List<SaleStaff> enroller = _db.saleStaffs.AsNoTracking().ToList();
                var query2 = (from a in enroller
                              select new Liaison
                              {
                                  FirstName = a.FirstName + " " + a.LastName + (" (Enroller)"),
                                  Id = a.Id
                              }).ToList();

                if (User.IsInRole("Admin"))
                {
                    query1.AddRange(query2);
                }
                var user = _db.Users.Find(User.Identity.GetUserId());
                if (User.IsInRole("Sales"))
                {
                    query1.AddRange(query2.Where(x => x.Id == user.CCMid));
                }
                List<LiaisonDropdown> liaisonDropdowns = new List<LiaisonDropdown>();
                foreach (var item_L in query1)
                {
                    if (item_L.FirstName.Contains("Enroller"))
                        liaisonDropdowns.Add(new LiaisonDropdown { SId = Convert.ToString("E" + item_L.Id), SName = item_L.FirstName });
                    else
                        liaisonDropdowns.Add(new LiaisonDropdown { SId = Convert.ToString("L" + item_L.Id), SName = item_L.FirstName });
                }



                if (user.Role == "Sales")
                {
                    string useridS = "E" + user.CCMid.ToString();
                    liaisonDropdowns = liaisonDropdowns.Where(x => x.SId == useridS).ToList();
                    ViewBag.DoctorId = useridS;
                }
                else
                {
                    ViewBag.DoctorId = query.Count > 0 ? liaisonDropdowns.FirstOrDefault().SId : "0";
                }

                //ViewBag.AllDoctors = query1;
                ViewBag.AllDoctors = liaisonDropdowns.OrderBy(p => p.SName).ToList();
                ViewBag.Hours = new List<int> { 1, 2, 3, 4, 5, 6, 7 };
                ViewBag.ForEnrollers = ForEnroller;

                return PartialView();
            }
            catch (Exception ex)
            {
                ViewBag.AllDoctors = new List<LiaisonDropdown>();
                ViewBag.Hours = new List<int> { 1, 2, 3, 4, 5, 6, 7 };
                ViewBag.ForEnrollers = ForEnroller;
                log.Error(Environment.NewLine + User.Identity.GetUserName() + "-------" + User.Identity.GetUserId() + Environment.NewLine + ex.Message + "-----" + ex.StackTrace);
                return PartialView();
                /*return ex.Message + "------------------" + ex.StackTrace;*/
            }
        }
        public async Task<ActionResult> AddUpdateSchedule(string PhysicianID)
        {
            EditViewModel model = new EditViewModel() { list = new List<AddUpdateViewModel>() };
            try
            {
                bool isEnroller = PhysicianID.Contains("E") ? true : false;
                int id = PhysicianID.Contains("E") ? Convert.ToInt32(PhysicianID.Replace("E", "").Trim()) : Convert.ToInt32(PhysicianID.Replace("L", "").Trim());
                List<DoctorTiming> doctortiming;
                var weekdays = new List<string> { "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday" };
                ViewBag.DayList = weekdays;

                if (isEnroller)
                    doctortiming = _db.doctorTimings.AsNoTracking().Where(x => x.SaleStaffID == id).ToList();
                else
                    doctortiming = _db.doctorTimings.AsNoTracking().Where(x => x.LiaisonID == id).ToList();
                List<ClinicTimingViewModel> list = new List<ClinicTimingViewModel>();
               
                foreach (var item in doctortiming)
                {
                    ClinicTimingViewModel vm = new ClinicTimingViewModel();
                    vm.ID = item.ID;
                    vm.StartTime = item.StartTime.ToString("hh:mm tt");
                    vm.EndTime = item.EndTime.ToString("hh:mm tt");
                    vm.WeekDayName = item.WeekDayName;
                    list.Add(vm);
                }
                foreach (var item in list)
                {
                    AddUpdateViewModel vm = new AddUpdateViewModel();
                    vm.StartTime = item.StartTime;
                    vm.EndTime = item.EndTime;
                    vm.WeekDayName = item.WeekDayName;
                    vm.IsHollyDay = false;
                    //vm.ScheduleID = shedule.ID;
                    //vm.SheduleDetailID = item.ID;
                    vm.DetailId = item.ID;
                    model.list.Add(vm);
                }
                var holydays = weekdays.Except(list.Select(x => x.WeekDayName).ToList());
                foreach (var item in holydays.ToList())
                {
                    AddUpdateViewModel vm = new AddUpdateViewModel();
                    vm.StartTime = "9:00 AM";
                    vm.EndTime = "05:00 PM";
                    vm.WeekDayName = item;
                    vm.IsHollyDay = true;
                    model.list.Add(vm);
                }
                var user = _db.Users.Find(User.Identity.GetUserId());
                var query = (from a in _db.Liaisons
                             select new Doctorlist
                             {
                                 FullName = a.FirstName + " " + a.LastName + (a.IsTranslator == true ? " (Translator)" : " (Counselor)"),
                                 Value = a.Id
                             }).ToList();

                var query1 = (from a in _db.saleStaffs
                              select new Doctorlist
                              {
                                  FullName = a.FirstName + " " + a.LastName + " (Enroller)",
                                  Value = a.Id
                              }).ToList();
                if (user.Role == "Liaison")
                {

                }
                if (isEnroller)
                    query = query1.Where(x => x.Value == id).ToList();
                else
                    query = query.Where(x => x.Value == id).ToList();


                List<LiaisonDropdown> liaisonDropdowns = new List<LiaisonDropdown>();
                foreach (var item_L in query)
                {
                    if (isEnroller)
                        liaisonDropdowns.Add(new LiaisonDropdown { SId = Convert.ToString("E" + item_L.Value), SName = item_L.FullName });
                    else
                        liaisonDropdowns.Add(new LiaisonDropdown { SId = Convert.ToString("L" + item_L.Value), SName = item_L.FullName });
                }

                List<LiaisonDropdown> doctorlists = liaisonDropdowns;
                ViewBag.Doctor = new SelectList(doctorlists.ToList(), "SId", "SName", PhysicianID);

                return View(model);
            }
            catch (Exception ex)
            {
                List<LiaisonDropdown> doctorlists=new List<LiaisonDropdown>();
                ViewBag.Doctor = new SelectList(doctorlists.ToList(), "SId", "SName", PhysicianID);
                log.Error(Environment.NewLine + User.Identity.GetUserName() + "-------" + User.Identity.GetUserId() + Environment.NewLine + ex.Message + "-----" + ex.StackTrace);
                return View(model);
                /*return ex.Message + "------------------" + ex.StackTrace;*/
            }
        }

        [HttpPost]
        public async Task<JsonResult> AddUpdateSchedule(List<AddUpdateViewModel> list, string ScheduleValidTill, string ScheduleValidFrom, bool IsTillDate, string PhysicianIDNew)
        {
          

            
            int PhysicianID = PhysicianIDNew.Contains("E") ? Convert.ToInt32(PhysicianIDNew.Replace("E", "").Trim()) : Convert.ToInt32(PhysicianIDNew.Replace("L", "").Trim());

            bool isEnroller = PhysicianIDNew.Contains("E") ? true : false;
            
            DoctorSchedule schedule = new DoctorSchedule();
            try
            {
                if (!IsTillDate)
                {
                    ScheduleValidTill = Convert.ToDateTime(ScheduleValidFrom).AddDays(7).ToString();
                }
                DoctorSchedule shedule = null;
                //var shedule = _context.DoctorSchedules.Where(x => x.ScheduleValidTill > DateTime.Now).FirstOrDefault();
                if (Convert.ToDateTime(ScheduleValidTill) < Convert.ToDateTime(ScheduleValidFrom))
                {
                    return Json("Invalid Date");
                }
                if (shedule == null)
                {
                    schedule = new DoctorSchedule()
                    {
                        ID = 0,
                        ClinicID = 1,
                        CreateDate = DateTime.Now,
                        ScheduleValidFrom = Convert.ToDateTime(ScheduleValidFrom),
                        ScheduleValidTill = Convert.ToDateTime(ScheduleValidTill),

                        CreatedBy = User.Identity.GetUserId(),
                        LiaisonID = isEnroller? (int ?)null :PhysicianID,
                        SaleStaffID=isEnroller?PhysicianID : (int?)null,//TODO: Make it dynamic
                        IsDeleted = false,
                    };

                }
                else
                {
                    schedule = shedule;
                }
                _db.doctorSchedules.Add(schedule);
                _db.SaveChanges();
                List<DoctorScheduleDeatil> dsdlist = new List<DoctorScheduleDeatil>();
                foreach (var item in list)
                {
                    DoctorScheduleDeatil detail = new DoctorScheduleDeatil()
                    {
                        ID = 0,
                        StartTime = Convert.ToDateTime(DateTime.Now.ToString("MM/dd/yyyy") + " " + item.StartTime),
                        EndTime = Convert.ToDateTime(DateTime.Now.ToString("MM/dd/yyyy") + " " + item.EndTime),
                        ScheduleID = schedule.ID,
                        WeekDayName = item.WeekDayName
                    };
                    if (!item.IsHollyDay)
                    {
                        dsdlist.Add(detail);
                        _db.doctorScheduleDeatils.Add(detail);
                        _db.SaveChanges();
                    }

                }
                //schedule.DoctorScheduleDeatils = dsdlist;
                //_db.doctorSchedules.Add(schedule);
                //_db.SaveChanges();

            }
            catch (Exception ex)
            {

                log.Error(Environment.NewLine + User.Identity.GetUserName() + "-------" + User.Identity.GetUserId() + Environment.NewLine + ex.Message + "-----" + ex.StackTrace);

               
            }
            return Json(true);
           
        }

        public async Task<ActionResult> EditSchedule(string PhysicianID)
        {
            EditViewModel model = new EditViewModel() { list = new List<AddUpdateViewModel>() };
            try
            {



                int Physicianid = PhysicianID.Contains("E") ? Convert.ToInt32(PhysicianID.Replace("E", "").Trim()) : Convert.ToInt32(PhysicianID.Replace("L", "").Trim());

                bool isEnroller = PhysicianID.Contains("E") ? true : false;

                DoctorSchedule shedule;
                if (isEnroller)
                    shedule = _db.doctorSchedules.AsNoTracking().ToList().Where(x => x.ScheduleValidTill.Date > DateTime.Now.Date && x.SaleStaffID == Physicianid).FirstOrDefault();
                else
                    shedule = _db.doctorSchedules.AsNoTracking().ToList().Where(x => x.ScheduleValidTill.Date > DateTime.Now.Date && x.LiaisonID == Physicianid).FirstOrDefault();

                //var shedule = _context.DoctorSchedules.Where(x => x.DoctorID == 11000 && x.ScheduleValidTill > DateTime.Now).FirstOrDefault();//TODO: Make it dynamic

                var detail = _db.doctorScheduleDeatils.AsNoTracking().Where(x => x.ScheduleID == shedule.ID).ToList();
                //var detail = _context.DoctorScheduleDeatil.Where(x => x.ScheduleID == shedule.ID).ToList();
               
                model.ScheduleValidFrom = shedule.ScheduleValidFrom.ToString("yyyy-MM-dd");
                model.ScheduleValidTill = shedule.ScheduleValidTill.ToString("yyyy-MM-dd");
                var weekdays = new List<string> { "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday" };
                model.SheduleId = shedule.ID;
                TimeZoneInfo easternZone = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
                foreach (var item in detail)
                {
                    AddUpdateViewModel vm = new AddUpdateViewModel();
                    var combine = item.StartTime;
                    //combine = DateTime.SpecifyKind(combine, DateTimeKind.Unspecified);
                    //DateTime easternTime = TimeZoneInfo.ConvertTimeFromUtc(combine, easternZone);

                    vm.StartTime = combine.ToString("hh:mm tt");
                    combine = item.EndTime;
                    //combine = DateTime.SpecifyKind(combine, DateTimeKind.Unspecified);
                    //easternTime = TimeZoneInfo.ConvertTimeFromUtc(combine, easternZone);
                    vm.EndTime = combine.ToString("hh:mm tt");
                    vm.WeekDayName = item.WeekDayName;
                    vm.IsHollyDay = false;
                    //vm.ScheduleID = shedule.ID;
                    //vm.SheduleDetailID = item.ID;
                    vm.DetailId = item.ID;
                    model.list.Add(vm);
                }
                var holydays = weekdays.Except(detail.Select(x => x.WeekDayName).ToList());
                foreach (var item in holydays.ToList())
                {
                    AddUpdateViewModel vm = new AddUpdateViewModel();
                    vm.StartTime = "9:00 AM";
                    vm.EndTime = "5:00 PM";
                    vm.WeekDayName = item;
                    vm.IsHollyDay = true;
                    model.list.Add(vm);
                }
                List<Liaison> Physician = _db.Liaisons.ToList();
                var query = (from a in _db.Liaisons
                             select new Doctorlist
                             {
                                 FullName = a.FirstName + " " + a.LastName + (a.IsTranslator == true ? " (Translator)" : " (Counselor)"),
                                 Value = a.Id
                             }).ToList();

                var query1 = (from a in _db.saleStaffs
                              select new Doctorlist
                              {
                                  FullName = a.FirstName + " " + a.LastName + " (Enroller)",
                                  Value = a.Id
                              }).ToList();
                //var query1 = (from a in _db.saleStaffs
                //              select new Doctorlist
                //             {
                //                 FullName = a.FirstName + " " + a.LastName,
                //                 Value = a.Id
                //             }).ToList();


                var user = _db.Users.Find(User.Identity.GetUserId());

                if (isEnroller)
                    query = query1.Where(x => x.Value == Physicianid).ToList();
                else
                    query = query.Where(x => x.Value == Physicianid).ToList();

                List<LiaisonDropdown> liaisonDropdowns = new List<LiaisonDropdown>();
                foreach (var item_L in query)
                {
                    if (isEnroller)
                        liaisonDropdowns.Add(new LiaisonDropdown { SId = Convert.ToString("E" + item_L.Value), SName = item_L.FullName });
                    else
                        liaisonDropdowns.Add(new LiaisonDropdown { SId = Convert.ToString("L" + item_L.Value), SName = item_L.FullName });
                }

                List<LiaisonDropdown> doctorlists = liaisonDropdowns;
                ViewBag.Doctor = new SelectList(doctorlists.ToList(), "SId", "SName", PhysicianID);



                //ViewBag.Doctor = new SelectList(query.ToList(), "Value", "FullName", PhysicianID);
                //ViewBag.AllDoctors = query;
                return View(model);
            }
            catch (Exception ex)
            {
                List<LiaisonDropdown> liaisonDropdowns = new List<LiaisonDropdown>();
                ViewBag.Doctor = new SelectList(liaisonDropdowns.ToList(), "SId", "SName", PhysicianID);
                log.Error(Environment.NewLine + User.Identity.GetUserName() + "-------" + User.Identity.GetUserId() + Environment.NewLine + ex.Message + "-----" + ex.StackTrace);
                return View(model);
                /*return ex.Message + "------------------" + ex.StackTrace;*/
            }
        }
        [HttpPost]
        public async Task<JsonResult> EditSchedule(List<AddUpdateViewModel> list, string ScheduleValidTill, string ScheduleValidFrom, bool IsTillDate, int SheduleId, int[] iDs, string PhysicianIDNew)
        {
            try
            {


                int PhysicianID = PhysicianIDNew.Contains("E") ? Convert.ToInt32(PhysicianIDNew.Replace("E", "").Trim()) : Convert.ToInt32(PhysicianIDNew.Replace("L", "").Trim());

                bool isEnroller = PhysicianIDNew.Contains("E") ? true : false;


                //int PhysicianID = PhysicianIDNew;
                DoctorSchedule schedule = new DoctorSchedule();
                if (!IsTillDate)
                {
                    ScheduleValidTill = Convert.ToDateTime(ScheduleValidFrom).AddDays(7).ToString();
                }
                DoctorSchedule shedule;
                if (isEnroller)
                    shedule = _db.doctorSchedules.ToList().Where(x => x.ScheduleValidTill.Date > DateTime.Now.Date && x.SaleStaffID == PhysicianID).FirstOrDefault();
                else
                    shedule = _db.doctorSchedules.ToList().Where(x => x.ScheduleValidTill.Date > DateTime.Now.Date && x.LiaisonID == PhysicianID).FirstOrDefault();


                if (Convert.ToDateTime(ScheduleValidTill) < Convert.ToDateTime(ScheduleValidFrom))
                {
                    return Json("Invalid Date");
                }

                //schedule = new DoctorSchedule()
                //{
                shedule.ID = SheduleId;
                shedule.ClinicID = 1;
                shedule.CreateDate = shedule.CreateDate;
                shedule.ScheduleValidFrom = Convert.ToDateTime(ScheduleValidFrom);
                shedule.ScheduleValidTill = Convert.ToDateTime(ScheduleValidTill);
                shedule.UpdatedDate = DateTime.Now;
                shedule.CreatedBy = shedule.CreatedBy;
                shedule.UpdatedBy = User.Identity.GetUserId();
                shedule.LiaisonID = isEnroller ? (int?)null : PhysicianID;
                shedule.SaleStaffID = isEnroller ? PhysicianID : (int?)null;
                shedule.IsDeleted = false;
                var lstalready = _db.doctorScheduleDeatils.Where(x => x.ScheduleID == SheduleId).ToList();
                _db.doctorScheduleDeatils.RemoveRange(lstalready);
                _db.SaveChanges();
                List<DoctorScheduleDeatil> dsdlist = new List<DoctorScheduleDeatil>();
                int count = 0;
                foreach (var item in list)
                {
                    DoctorScheduleDeatil detail = new DoctorScheduleDeatil()
                    {
                        //ID = item.DetailId,
                        StartTime = Convert.ToDateTime(DateTime.Now.ToString("MM/dd/yyyy") + " " + item.StartTime),
                        EndTime = Convert.ToDateTime(DateTime.Now.ToString("MM/dd/yyyy") + " " + item.EndTime),
                        ScheduleID = shedule.ID,
                        WeekDayName = item.WeekDayName,

                    };
                    if (!item.IsHollyDay)
                    {

                        dsdlist.Add(detail);
                        _db.doctorScheduleDeatils.Add(detail);
                        _db.SaveChanges();
                    }

                }


                _db.Entry(shedule).State = System.Data.Entity.EntityState.Modified;
                _db.SaveChanges();
                return Json(true);
            }
            catch (Exception ex)
            {

                log.Error(Environment.NewLine + User.Identity.GetUserName() + "-------" + User.Identity.GetUserId() + Environment.NewLine + ex.Message + "-----" + ex.StackTrace);
                return Json(true);
                /*return ex.Message + "------------------" + ex.StackTrace;*/
            }
        }
        [HttpGet]
        public async Task<JsonResult> GetEventsbyID(string PhysicianID)
        {
           

                bool isEnroller = PhysicianID.Contains("E") ? true : false;
                int id = PhysicianID.Contains("E") ? Convert.ToInt32(PhysicianID.Replace("E", "").Trim()) : Convert.ToInt32(PhysicianID.Replace("L", "").Trim());
                DoctorSchedule shedule = new DoctorSchedule();
                if (isEnroller)
                    shedule = _db.doctorSchedules.AsNoTracking().ToList().Where(x => x.SaleStaffID == id && (x.ScheduleValidTill.Date) > DateTime.Now.Date).FirstOrDefault();
                else
                    shedule = _db.doctorSchedules.AsNoTracking().ToList().Where(x => x.LiaisonID == id && (x.ScheduleValidTill.Date) > DateTime.Now.Date).FirstOrDefault();


                List<DateTime> list = new List<DateTime>();

                List<DoctorScheduleDeatil> listOfDetail = new List<DoctorScheduleDeatil>();
                //var scheduledetail = _context.DoctorSchedules.Where(x => x.DoctorID == 11000 && DateTime.ParseExact(x.ScheduleValidTill.ToString("dd/MM/yyyy"), "dd/MM/yyyy", null) >= DateTime.ParseExact(DateTime.Now.ToString("dd/MM/yyyy"), "dd/MM/yyyy", null)).FirstOrDefault();//TODO: Make it dynamic
                if (shedule != null)
                {
                    if (DateTime.ParseExact(shedule.ScheduleValidTill.ToString("dd/MM/yyyy"), "dd/MM/yyyy", null) >= DateTime.ParseExact(DateTime.Now.ToString("dd/MM/yyyy"), "dd/MM/yyyy", null))
                    {

                        var result = _db.doctorScheduleDeatils.Where(x => x.ScheduleID == shedule.ID).ToList();
                        //var result = _context.DoctorScheduleDeatil.Where(x => x.ScheduleID == scheduledetail.ID).ToList();

                        int numberofdays = Convert.ToInt32((shedule.ScheduleValidTill - DateTime.Now).TotalDays);

                        //TimeZoneInfo easternZone = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");

                        for (int i = 0; i <= numberofdays; i++)
                        {
                            try
                            {



                                string Day = Convert.ToDateTime(DateTime.Now.AddDays(i)).ToString("dddd");
                                foreach (var item in result.Where(x => x.WeekDayName == Day).ToList())
                                {
                                    DoctorScheduleDeatil dsd = new DoctorScheduleDeatil();
                                    dsd.ID = item.ID;
                                    dsd.ScheduleID = item.ScheduleID;

                                    TimeSpan time = new TimeSpan(item.StartTime.Hour, item.StartTime.Minute, 0);
                                    var combine = Convert.ToDateTime(DateTime.Now.AddDays(i).Date) + time;
                                    //combine = DateTime.SpecifyKind(combine, DateTimeKind.Unspecified);
                                    //DateTime easternTime = TimeZoneInfo.ConvertTimeFromUtc(combine, easternZone);
                                    dsd.StartTime = combine;
                                    dsd.WeekDayName = item.WeekDayName;
                                    time = new TimeSpan(item.EndTime.Hour, item.EndTime.Minute, 0);
                                    //if (time.ToString() != "00:00:00")
                                    //{
                                    //    combine = Convert.ToDateTime(DateTime.UtcNow.AddDays(i).Date) + time;
                                    //    combine = DateTime.SpecifyKind(combine, DateTimeKind.Unspecified);
                                    //}
                                    //else
                                    //{
                                    //    combine = Convert.ToDateTime(DateTime.UtcNow.AddDays(i + 1).Date) + time;
                                    //    combine = DateTime.SpecifyKind(combine, DateTimeKind.Unspecified);
                                    //}

                                    //easternTime = TimeZoneInfo.ConvertTimeFromUtc(combine, easternZone);
                                    combine = Convert.ToDateTime(DateTime.Now.AddDays(i).Date) + time;
                                    dsd.EndTime = combine;
                                    listOfDetail.Add(dsd);

                                }
                            }
                            catch (Exception ex)
                            {
                            log.Error(Environment.NewLine + User.Identity.GetUserName() + "-------" + User.Identity.GetUserId() + Environment.NewLine + ex.Message + "-----" + ex.StackTrace);


                        }
                    }
                        //Holidays




                        var physicianHolidays = _db.PhysicianHolidays.AsNoTracking().Where(x => x.LiaisonID == id).ToList();
                        //
                        try
                        {
                            var listOfDetailnot = physicianHolidays.Select(x => new DoctorScheduleDeatil
                            {
                                StartTime = x.HDate.Value,

                            }).ToList();
                            listOfDetail = listOfDetail.Where(x => !listOfDetailnot.Any(y => y.StartTime.Date == x.StartTime.Date)).ToList();
                        }
                        catch (Exception ex)
                        {


                        }

                        return Json(listOfDetail, JsonRequestBehavior.AllowGet);
                    }
                }
                return Json(false, JsonRequestBehavior.AllowGet);
           
            

        }
        [HttpGet]
        //public async Task<JsonResult> GetEvents()
        //{
        //    var response = _db.doctorSchedules.Where(x => x.LiaisonID == DoctorID && x.ScheduleValidTill > DateTime.Now.Date).FirstOrDefault();
        //    var shedule = JsonConvert.DeserializeObject<DoctorSchedule>(response);

        //    List<DateTime> list = new List<DateTime>();

        //    List<DoctorScheduleDeatil> listOfDetail = new List<DoctorScheduleDeatil>();
        //    //var scheduledetail = _context.DoctorSchedules.Where(x => x.DoctorID == 11000 && DateTime.ParseExact(x.ScheduleValidTill.ToString("dd/MM/yyyy"), "dd/MM/yyyy", null) >= DateTime.ParseExact(DateTime.Now.ToString("dd/MM/yyyy"), "dd/MM/yyyy", null)).FirstOrDefault();//TODO: Make it dynamic
        //    if (shedule != null)
        //    {


        //        if (DateTime.ParseExact(shedule.ScheduleValidTill.ToString("dd/MM/yyyy"), "dd/MM/yyyy", null) >= DateTime.ParseExact(DateTime.Now.ToString("dd/MM/yyyy"), "dd/MM/yyyy", null))
        //        {
        //            response = await client.GetStringAsync(Constants.Api + "api/RSAppointment/GetScheduleDetailByScheduleID/" + shedule.ID);
        //            var result = JsonConvert.DeserializeObject<List<DoctorScheduleDeatil>>(response);
        //            //var result = _context.DoctorScheduleDeatil.Where(x => x.ScheduleID == scheduledetail.ID).ToList();

        //            int numberofdays = Convert.ToInt32((shedule.ScheduleValidTill - DateTime.Now).TotalDays);


        //            for (int i = 0; i <= numberofdays; i++)
        //            {
        //                string Day = Convert.ToDateTime(DateTime.Now.AddDays(i)).ToString("dddd");
        //                foreach (var item in result.Where(x => x.WeekDayName == Day).ToList())
        //                {
        //                    DoctorScheduleDeatil dsd = new DoctorScheduleDeatil();
        //                    dsd.ID = item.ID;
        //                    dsd.ScheduleID = item.ScheduleID;

        //                    TimeSpan time = new TimeSpan(item.StartTime.Hour, item.StartTime.Minute, 0);
        //                    var combine = Convert.ToDateTime(DateTime.Now.AddDays(i).Date) + time;
        //                    dsd.StartTime = combine;
        //                    dsd.WeekDayName = item.WeekDayName;
        //                    time = new TimeSpan(item.EndTime.Hour, item.EndTime.Minute, 0);
        //                    combine = Convert.ToDateTime(DateTime.Now.AddDays(i).Date) + time;
        //                    dsd.EndTime = combine;
        //                    listOfDetail.Add(dsd);

        //                }
        //            }
        //            //Holidays

        //            client = new HttpClient();
        //            token = await HttpContext.GetTokenAsync("access_token");
        //            client.SetBearerToken(token);

        //            response = await client.GetStringAsync(Constants.Api + "api/PhysicianHolidays/GetAllPhysicianHoliday/" + PhysicianID + "/" + cid);
        //            var physicianHolidays = JsonConvert.DeserializeObject<List<PhysicianHolidays>>(response);
        //            //
        //            try
        //            {
        //                var listOfDetailnot = physicianHolidays.Select(x => new DoctorScheduleDeatil
        //                {
        //                    StartTime = x.HDate.Value,

        //                }).ToList();
        //                listOfDetail = listOfDetail.Where(x => !listOfDetailnot.Any(y => y.StartTime.Date == x.StartTime.Date)).ToList();
        //            }
        //            catch (Exception ex)
        //            {


        //            }
        //            return Json(listOfDetail);
        //        }
        //    }
        //    return Json(false);

        //}
        public async Task<ActionResult> BookAppointmentEnroller()
        {
            return View();
        }
        public async Task<JsonResult> GetClinincs()
        {

            List<Clinic> clinics = new List<Clinic>();
            //var result = _context.Clinics;
            return Json(clinics, JsonRequestBehavior.AllowGet);
        }
        public async Task<JsonResult> GetPatientAppointment(string Id)
        {
            try
            {

           
            bool isEnroller = Id.Contains("E") ? true : false;
            int id = Id.Contains("E") ? Convert.ToInt32(Id.Replace("E", "").Trim()) : Convert.ToInt32(Id.Replace("L", "").Trim());

            List<PatientAppointment> result;
            if(isEnroller)
                result = _db.patientAppointments.AsNoTracking().AsQueryable().Where(x => x.SaleStaffID == id).ToList();
            else
                result = _db.patientAppointments.AsNoTracking().AsQueryable().Where(x => x.LiaisonID == id).ToList();

            return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                List<PatientAppointment> result = new List<PatientAppointment>();
                log.Error(Environment.NewLine + User.Identity.GetUserName() + "-------" + User.Identity.GetUserId() + Environment.NewLine + ex.Message + "-----" + ex.StackTrace);
                return Json(result, JsonRequestBehavior.AllowGet);
                /*return ex.Message + "------------------" + ex.StackTrace;*/
            }
        }
        [HttpPost]
        public async Task<JsonResult> SavePatientAppointment(string DoctorID, string Subject, string StartTime, string EndTime, string Date, int AppointmentId, string PatientName)
        {
            try
            {


                bool isEnroller = DoctorID.Contains("E") ? true : false;
                int DoctorId = DoctorID.Contains("E") ? Convert.ToInt32(DoctorID.Replace("E", "").Trim()) : Convert.ToInt32(DoctorID.Replace("L", "").Trim());


                var user = _db.Users.Find(User.Identity.GetUserId());

                DoctorSchedule schedule;
                if (isEnroller)
                    schedule = _db.doctorSchedules.ToList().Where(x => x.SaleStaffID == DoctorId && x.ScheduleValidTill > DateTime.Now.Date).FirstOrDefault();
                else
                    schedule = _db.doctorSchedules.ToList().Where(x => x.LiaisonID == DoctorId && x.ScheduleValidTill > DateTime.Now.Date).FirstOrDefault();

                string day = Convert.ToDateTime(Date).ToString("dddd");
                //var schedule = _context.DoctorSchedules.Where(x => x.DoctorID == DoctorID && x.ScheduleValidTill > DateTime.Now).FirstOrDefault();

                //List<DoctoTimeTable> timetable = new List<DoctoTimeTable>();

                List<int> DocHours = new List<int>();
                List<int> DocMint = new List<int>();
                var Day = Convert.ToDateTime(Date).ToString("dddd");
                _db.doctorScheduleDeatils.Where(x => x.ScheduleID == schedule.ID).ToList();
                var doctortimeing = _db.doctorScheduleDeatils.Where(x => x.ScheduleID == schedule.ID).ToList();
                int hour, mint;
                hour = int.Parse(StartTime.Split(':')[0]);
                mint = int.Parse(StartTime.Split(':')[1].Split(' ')[0]);

                TimeSpan time = new TimeSpan(hour, mint, 0);
                var combine = Convert.ToDateTime(Convert.ToDateTime(Date).ToString("MM/dd/yyyy")) + time;
                // System.Globalization.CultureInfo MyCultureInfo = new System.Globalization.CultureInfo("de-US");
                string MyString = StartTime;
                DateTime startDate = combine;

                hour = int.Parse(EndTime.Split(':')[0]);
                mint = int.Parse(EndTime.Split(':')[1].Split(' ')[0]);
                time = new TimeSpan(hour, mint, 0);
                //  System.Globalization.CultureInfo MyCultureInfo2 = new System.Globalization.CultureInfo("de-US");
                string date = EndTime;
                DateTime endDate = Convert.ToDateTime(Convert.ToDateTime(Date).ToString("MM/dd/yyyy")) + time;
                hour = int.Parse(endDate.ToString("HH"));
                string min = endDate.ToString("mm");
                PatientAppointment model = new PatientAppointment();
                model.LiaisonID = isEnroller ? (int?)null : DoctorId;
                model.SaleStaffID = isEnroller ? DoctorId : (int?)null;
                model.Subject = PatientName;
                model.StartTime = startDate;
                model.EndTime = endDate;
                if (Subject != "0")
                {
                    model.PatientID = Convert.ToInt32(Subject);
                }

                model.CreatedBy = user.CCMid == null ? 1 : user.CCMid.Value;
                model.CreatedOn = DateTime.Now;
                model.ClinicID = 1;
                model.AptStatus = "Pending";
                try
                {
                    var patient = _db.Patients.Where(x => x.Id == model.PatientID).FirstOrDefault();
                    patient.AppointmentDate = startDate;
                    _db.Entry(patient).State = System.Data.Entity.EntityState.Modified;
                    _db.SaveChanges();
                }
                catch (Exception ex)
                {


                }

                if (AppointmentId == 0)
                {
                    _db.patientAppointments.Add(model);
                    _db.SaveChanges();
                    //_context.PatientAppointments.Add(model);
                    //_context.SaveChanges();
                }
                else
                {

                    //time = new TimeSpan(hour, mint, 0);
                    //combine = Convert.ToDateTime(StartTime).Date + time;
                    var result = _db.patientAppointments.Where(x => x.ID == AppointmentId).FirstOrDefault();

                    //var result = _context.PatientAppointments.Where(x => x.ID == AppointmentId).FirstOrDefault();
                    result.LiaisonID = model.LiaisonID;
                    result.SaleStaffID = model.SaleStaffID;
                    hour = int.Parse(StartTime.Split(':')[0]);
                    mint = int.Parse(StartTime.Split(':')[1].Split(' ')[0]);

                    time = new TimeSpan(hour, mint, 0);
                    combine = Convert.ToDateTime(Convert.ToDateTime(Date).ToString("MM/dd/yyyy")) + time;



                    result.Subject = model.Subject;
                    result.StartTime = combine;

                    //hour = int.Parse(endDate.ToString("HH"));
                    // min = endDate.ToString("mm");

                    hour = int.Parse(EndTime.Split(':')[0]);
                    mint = int.Parse(EndTime.Split(':')[1].Split(' ')[0]);

                    time = new TimeSpan(hour, mint, 0);
                    combine = Convert.ToDateTime(Convert.ToDateTime(Date).ToString("MM/dd/yyyy")) + time;



                    result.EndTime = combine;
                    result.PatientID = model.PatientID;
                    result.CreatedBy = model.CreatedBy;
                    result.ClinicID = 1;
                    result.UpdateOn = DateTime.Now;
                    result.AptStatus = "Pending";
                    _db.Entry(result).State = System.Data.Entity.EntityState.Modified;
                    _db.SaveChanges();
                    //_context.SaveChanges();
                }

                return Json(true);
            }
            catch (Exception ex)
            {

                log.Error(Environment.NewLine + User.Identity.GetUserName() + "-------" + User.Identity.GetUserId() + Environment.NewLine + ex.Message + "-----" + ex.StackTrace);
                return Json(true);
                /*return ex.Message + "------------------" + ex.StackTrace;*/
            }
        }
        [HttpGet]
        public async Task<JsonResult> DoctorTime(string DoctorID, string Date, string Time)
        {
            try
            {


                bool isEnroller = DoctorID.Contains("E") ? true : false;
                int DoctorId = DoctorID.Contains("E") ? Convert.ToInt32(DoctorID.Replace("E", "").Trim()) : Convert.ToInt32(DoctorID.Replace("L", "").Trim());


                if (Date == "Invalid date")
                {
                    return Json(true);
                }
                var date = Convert.ToDateTime(Date);

                List<PatientAppointment> response;
                if (isEnroller)
                    response = _db.patientAppointments.AsNoTracking().ToList().Where(x => x.StartTime.Date == date.Date && x.SaleStaffID == DoctorId).ToList();
                else
                    response = _db.patientAppointments.AsNoTracking().ToList().Where(x => x.StartTime.Date == date.Date && x.LiaisonID == DoctorId).ToList();
                var patient = (response);
                //var patient = _context.GetPatientAppointmentByDate.Where(x => x.StartTime.ToString("dd-MM-yyyy") == date).ToList();

                return Json(patient, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                List<PatientAppointment> response = new List<PatientAppointment>();
                log.Error(Environment.NewLine + User.Identity.GetUserName() + "-------" + User.Identity.GetUserId() + Environment.NewLine + ex.Message + "-----" + ex.StackTrace);
                return Json(response, JsonRequestBehavior.AllowGet);
                /*return ex.Message + "------------------" + ex.StackTrace;*/
            }

        }
        [HttpPost]
        public async Task<JsonResult> ShowDocotrAppointment(string Id)
        {
            try
            {
                bool isEnroller = Id.Contains("E") ? true : false;
                int id = Id.Contains("E") ? Convert.ToInt32(Id.Replace("E", "").Trim()) : Convert.ToInt32(Id.Replace("L", "").Trim());

                List<PatientAppointment> response;
                if (isEnroller)
                    response = _db.patientAppointments.AsNoTracking().Where(x => x.SaleStaffID == id).ToList();
                else
                    response = _db.patientAppointments.AsNoTracking().Where(x => x.LiaisonID == id).ToList();


                var PatientAppointments = response;
                //response = await client.GetStringAsync(Constants.Api + "api/Patients/"+cid);
                //List<Patient> Patients = JsonConvert.DeserializeObject<List<Patient>>(response);

                var query = (from pa in PatientAppointments//TODO: Make it dynamic
                                                           //join pe1 in _db.Patients on pa.PatientID equals pe1.Id into pe1
                                                           //from pe in pe1.DefaultIfEmpty()                                       //join p in Patients on pa.PatientID equals p.ID
                             select new PatientAppointmentViewModel
                             {
                                 ID = pa.ID,
                                 StartTime = pa.StartTime,
                                 EndTime = pa.EndTime,

                                 Subject = pa.Subject,
                                 AptStatus = pa.AptStatus,
                                 PatientID = pa.PatientID,
                                 isMigrated = pa.UpdateOn != null ? true : false

                             }).ToList();

                return Json(query);
            }
            catch (Exception ex)
            {
                List<PatientAppointment> response = new List<PatientAppointment>();
                log.Error(Environment.NewLine + User.Identity.GetUserName() + "-------" + User.Identity.GetUserId() + Environment.NewLine + ex.Message + "-----" + ex.StackTrace);
                return Json(response);
                /*return ex.Message + "------------------" + ex.StackTrace;*/
            }
        }

        public async Task<JsonResult> GetDoctorByID(string ID)
        {
            try
            {


                bool isEnroller = ID.Contains("E") ? true : false;
                int Id = ID.Contains("E") ? Convert.ToInt32(ID.Replace("E", "").Trim()) : Convert.ToInt32(ID.Replace("L", "").Trim());


                List<DateTime> list = new List<DateTime>();

                List<DoctorScheduleDeatil> listOfDetail = new List<DoctorScheduleDeatil>();

                DoctorSchedule scheduledetail;
                if (isEnroller)
                    scheduledetail = _db.doctorSchedules.AsNoTracking().ToList().Where(x => x.ScheduleValidTill.Date > DateTime.Now && x.SaleStaffID == Id).FirstOrDefault();
                else
                    scheduledetail = _db.doctorSchedules.AsNoTracking().ToList().Where(x => x.ScheduleValidTill.Date > DateTime.Now && x.LiaisonID == Id).FirstOrDefault();


                //var scheduledetail = _context.DoctorSchedules.Where(x => x.DoctorID == 11000 && DateTime.ParseExact(x.ScheduleValidTill.ToString("dd/MM/yyyy"), "dd/MM/yyyy", null) >= DateTime.ParseExact(DateTime.Now.ToString("dd/MM/yyyy"), "dd/MM/yyyy", null)).FirstOrDefault();//TODO: Make it dynamic
                if (scheduledetail != null)
                {


                    if (DateTime.ParseExact(scheduledetail.ScheduleValidTill.ToString("dd/MM/yyyy"), "dd/MM/yyyy", null) >= DateTime.ParseExact(DateTime.Now.ToString("dd/MM/yyyy"), "dd/MM/yyyy", null))
                    {
                        //var result = _context.DoctorScheduleDeatil.Where(x => x.ScheduleID == scheduledetail.ID).ToList();

                        var result = _db.doctorScheduleDeatils.AsNoTracking().Where(x => x.ScheduleID == scheduledetail.ID).ToList();
                        int startdate = Convert.ToInt32((scheduledetail.ScheduleValidFrom - DateTime.Now).TotalDays);

                        int numberofdays = Convert.ToInt32((scheduledetail.ScheduleValidTill - DateTime.Now).TotalDays);
                        if (startdate > 0)
                        {
                            numberofdays = Convert.ToInt32((scheduledetail.ScheduleValidTill - scheduledetail.ScheduleValidFrom).TotalDays);
                        }

                        for (int i = 0; i <= numberofdays; i++)
                        {
                            string Day = "";
                            if (startdate > 0)
                            {
                                Day = scheduledetail.ScheduleValidFrom.AddDays(i).ToString("dddd");
                            }
                            else
                            {
                                Day = Convert.ToDateTime(DateTime.Now.AddDays(i)).ToString("dddd");
                            }
                            foreach (var item in result.Where(x => x.WeekDayName == Day).ToList())
                            {
                                DoctorScheduleDeatil dsd = new DoctorScheduleDeatil();
                                dsd.ID = item.ID;
                                dsd.ScheduleID = item.ScheduleID;

                                TimeSpan time = new TimeSpan(item.StartTime.Hour, item.StartTime.Minute, 0);
                                var combine = Convert.ToDateTime(DateTime.Now.AddDays(i).Date) + time;
                                if (startdate > 0)
                                {
                                    combine = scheduledetail.ScheduleValidFrom.AddDays(i) + time;
                                    //Day = scheduledetail.ScheduleValidFrom.AddDays(i).ToString("dddd");
                                }

                                dsd.StartTime = combine;
                                dsd.WeekDayName = item.WeekDayName;
                                time = new TimeSpan(item.EndTime.Hour, item.EndTime.Minute, 0);
                                combine = Convert.ToDateTime(DateTime.Now.AddDays(i).Date) + time;
                                if (startdate > 0)
                                {
                                    combine = scheduledetail.ScheduleValidFrom.AddDays(i) + time;
                                    //Day = scheduledetail.ScheduleValidFrom.AddDays(i).ToString("dddd");
                                }
                                dsd.EndTime = combine;
                                listOfDetail.Add(dsd);

                            }
                        }
                        //Holidays
                        List<PhysicianHolidays> physicianHolidays;
                        if (isEnroller)
                            physicianHolidays = _db.PhysicianHolidays.AsNoTracking().Where(x => x.SaleStaffID == Id).ToList();
                        else
                            physicianHolidays = _db.PhysicianHolidays.AsNoTracking().Where(x => x.LiaisonID == Id).ToList();
                        //
                        try
                        {
                            var listOfDetailnot = physicianHolidays.Select(x => new DoctorScheduleDeatil
                            {
                                StartTime = x.HDate.Value,

                            }).ToList();
                            listOfDetail = listOfDetail.Where(x => !listOfDetailnot.Any(y => y.StartTime.Date == x.StartTime.Date)).ToList();
                        }
                        catch (Exception ex)
                        {


                        }

                        // listOfDetail = listOfDetail.Except(listOfDetailnot).ToList();
                        return Json(listOfDetail, JsonRequestBehavior.AllowGet);
                    }
                }
                return Json(false, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {

                log.Error(Environment.NewLine + User.Identity.GetUserName() + "-------" + User.Identity.GetUserId() + Environment.NewLine + ex.Message + "-----" + ex.StackTrace);
                return Json(false, JsonRequestBehavior.AllowGet);
                /*return ex.Message + "------------------" + ex.StackTrace;*/
            }
        }
        public async Task<PartialViewResult> GetAllHolidays(string PhysicianID)
        {
            try
            {
                int Physician = PhysicianID.Contains("E") ? Convert.ToInt32(PhysicianID.Replace("E", "").Trim()) : Convert.ToInt32(PhysicianID.Replace("L", "").Trim());

                bool isEnroller = PhysicianID.Contains("E") ? true : false;

                List<PhysicianHolidays> response;
                if(isEnroller)
                    response = _db.PhysicianHolidays.AsNoTracking().Where(x => x.SaleStaffID == Physician).ToList();
                else
                    response = _db.PhysicianHolidays.AsNoTracking().Where(x => x.LiaisonID == Physician).ToList();

                var physicianHolidays = response;
                return PartialView("Holidays", physicianHolidays);
            }
            catch (Exception ex)
            {

                log.Error(Environment.NewLine + User.Identity.GetUserName() + "-------" + User.Identity.GetUserId() + Environment.NewLine + ex.Message + "-----" + ex.StackTrace);

                return PartialView("Holidays", new List<PhysicianHolidays>());


            }

        }
        public async Task<bool> AddHolidays(DateTime FromDate, DateTime ToDate, bool isRepeated, string PhysicianID, string remarksH)
        {
            try
            {
                int Physician = PhysicianID.Contains("E") ? Convert.ToInt32(PhysicianID.Replace("E", "").Trim()) : Convert.ToInt32(PhysicianID.Replace("L", "").Trim());

                bool isEnroller = PhysicianID.Contains("E") ? true : false;

                ////var response = await client.GetStringAsync(Constants.Api + "api/Physicians/GetPhysicianByUserID/" + sub + "/" + cid);
                ////int PhysicianID = JsonConvert.DeserializeObject<Int32>(response);
                List<PhysicianHolidays> physicianHolidays = new List<PhysicianHolidays>();
                if (isRepeated == true)
                {
                    int numberofdays = Convert.ToInt32((ToDate - FromDate).TotalDays) + 1;
                    for (int i = 0; i < numberofdays; i++)
                    {
                        PhysicianHolidays physicianHoliday = new PhysicianHolidays();
                        physicianHoliday.ClinicID = 1;
                        physicianHoliday.HDate = FromDate.AddDays(i);
                        physicianHoliday.LiaisonID = isEnroller ? (int?)null : Physician;
                        physicianHoliday.SaleStaffID = isEnroller ? Physician : (int?)null;
                        physicianHoliday.Remarks = remarksH;
                        physicianHolidays.Add(physicianHoliday);
                    }
                }
                else
                {
                    PhysicianHolidays physicianHoliday = new PhysicianHolidays();
                    physicianHoliday.ClinicID = 1;
                    physicianHoliday.HDate = FromDate;
                    physicianHoliday.LiaisonID = isEnroller ? (int?)null : Physician;
                    physicianHoliday.SaleStaffID = isEnroller ? Physician : (int?)null;
                    physicianHoliday.Remarks = remarksH;
                    physicianHolidays.Add(physicianHoliday);
                }
                _db.PhysicianHolidays.AddRange(physicianHolidays);
                _db.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {

                log.Error(Environment.NewLine + User.Identity.GetUserName() + "-------" + User.Identity.GetUserId() + Environment.NewLine + ex.Message + "-----" + ex.StackTrace);
                return true;
                /*return ex.Message + "------------------" + ex.StackTrace;*/
            }

        }
        public async Task<JsonResult> DeletePhysicianHoliday(int ID)
        {
            try
            {


                var result = _db.PhysicianHolidays.Where(x => x.ID == ID).FirstOrDefault();
                _db.PhysicianHolidays.Remove(result);
                _db.SaveChanges();
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {

                log.Error(Environment.NewLine + User.Identity.GetUserName() + "-------" + User.Identity.GetUserId() + Environment.NewLine + ex.Message + "-----" + ex.StackTrace);
                return Json(true, JsonRequestBehavior.AllowGet);
                /*return ex.Message + "------------------" + ex.StackTrace;*/
            }
        }
        public async Task<JsonResult> DeletePatientAppointment(int eventID)
        {
            try
            {
            var result = _db.patientAppointments.Where(x => x.ID == eventID).FirstOrDefault();
            var patient = _db.Patients.Where(x => x.Id == result.PatientID).FirstOrDefault();
            _db.patientAppointments.Remove(result);

            patient.AppointmentDate = null;
            _db.Entry(patient).State = System.Data.Entity.EntityState.Modified;

            _db.SaveChanges();
            return Json(true, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {

                log.Error(Environment.NewLine + User.Identity.GetUserName() + "-------" + User.Identity.GetUserId() + Environment.NewLine + ex.Message + "-----" + ex.StackTrace);
                return Json(true, JsonRequestBehavior.AllowGet);
                /*return ex.Message + "------------------" + ex.StackTrace;*/
            }

        }
        public async Task<bool> UpdateAppointmentStatus(int ID, string Status)
        {
            try
            {
            var results = _db.patientAppointments.Where(x => x.ID == ID).FirstOrDefault();
            results.AptStatus = Status;
            _db.Entry(results).State = System.Data.Entity.EntityState.Modified;
            _db.SaveChanges();

            return true;

            }
            catch (Exception ex)
            {

                log.Error(Environment.NewLine + User.Identity.GetUserName() + "-------" + User.Identity.GetUserId() + Environment.NewLine + ex.Message + "-----" + ex.StackTrace);
                return true;
                /*return ex.Message + "------------------" + ex.StackTrace;*/
            }
        }

    }
}