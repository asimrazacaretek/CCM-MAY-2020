using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using CCM.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;

namespace CCM.Controllers
{
    [Authorize(Roles = "Liaison, Physician, PhysiciansGroup, Admin, QAQC, LiaisonGroup, Sales")]
    public class EnrollerController : BaseController
    {
        //private Application_dbContect _db = new Application_dbContect();
        private ApplicationUserManager _userManager;
        public EnrollerController()
        {

        }
        public EnrollerController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
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
        // GET: SaleStaffs
        public async Task<ActionResult> Index()
        {
            return View(await _db.saleStaffs.ToListAsync());
        }

        // GET: SaleStaffs/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SaleStaff saleStaff = await _db.saleStaffs.FindAsync(id);
            if (saleStaff == null)
            {
                return HttpNotFound();
            }
            return View(saleStaff);
        }

        // GET: SaleStaffs/Create
        public ActionResult Create()
        {
            List<ClinicTiming> list = new List<ClinicTiming>();
            var days = new List<string> { "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday" };
            foreach (var item in days)
            {
                ClinicTiming ct = new ClinicTiming()
                {
                    WeekDayName = item,
                    StartTime = Convert.ToDateTime(DateTime.Now.ToString("MM/dd/yyyy") + " " + "9:00 AM"),
                    EndTime = Convert.ToDateTime(DateTime.Now.ToString("MM/dd/yyyy") + " " + "5:00 PM")
                };
                list.Add(ct);
            }
            ViewBag.DayList = list;
            return View();
        }

        // POST: SaleStaffs/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,UserId,CreatedOn,CreatedBy,UpdatedOn,UpdatedBy,FirstName,LastName,Gender,MobilePhoneNumber,Email,Address,City")] SaleStaff saleStaff, string[] StartTime, string[] EndTime, string[] WeekDays, bool[] isHoliday)
        {
            if (ModelState.IsValid)
            {
                var existingUser = await UserManager.FindByNameAsync(saleStaff.Email);
                if (existingUser != null)
                {
                    string sExistingId = existingUser.Id;
                    ViewBag.Message = "Email Already Exists: " + saleStaff.Email + "! SaleStaff Not Created!.";
                    return View(saleStaff);
                  //  await UserManager.DeleteAsync(existingUser);

                }

                ApplicationUser user = new ApplicationUser { UserName = saleStaff.Email, Email = saleStaff.Email, FirstName = saleStaff.FirstName, LastName = saleStaff.LastName, Role = "Sales" };
                string password = saleStaff.LastName.ToLower() + "#SS1013"; // + liaisonId;
                var result = await UserManager.CreateAsync(user, password);





                saleStaff.UserId = user.Id;
                saleStaff.CreatedOn = DateTime.Now;
                saleStaff.CreatedBy = User.Identity.GetUserId();
                _db.saleStaffs.Add(saleStaff);
                _db.SaveChanges();
                if (result.Succeeded)
                {
                    user.Role = "Sales";
                    user.CCMid = saleStaff.Id;
                    user.FirstName = saleStaff.FirstName;
                    user.LastName = saleStaff.LastName;
                    user.PhoneNumber = saleStaff.MobilePhoneNumber;

                    await UserManager.AddToRoleAsync(user.Id, "Sales");

                    //LiasionTimings
                    List<DoctorTiming> list = new List<DoctorTiming>();
                    for (int i = 0; i < StartTime.Count(); i++)
                    {
                        if (isHoliday[i] == false)
                        {
                            DoctorTiming ct = new DoctorTiming();
                            string one = DateTime.Now.ToString("MM/dd/yyyy");
                            ct.StartTime = Convert.ToDateTime(one + " " + StartTime[i]);
                            ct.EndTime = Convert.ToDateTime(one + " " + EndTime[i]);
                            ct.ClinicID = 1;
                            ct.IsDeleted = false;
                            ct.WeekDayName = WeekDays[i];
                            ct.LiaisonID =null;
                            ct.SaleStaffID = saleStaff.Id;
                            list.Add(ct);

                        }
                    }
                    _db.doctorTimings.AddRange(list);
                    _db.SaveChanges();

                }
                return RedirectToAction("Index");
            }

            return View(saleStaff);
        }

        // GET: SaleStaffs/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SaleStaff saleStaff = await _db.saleStaffs.FindAsync(id);
            if (saleStaff == null)
            {
                return HttpNotFound();
            }
            var doctortiming = _db.doctorTimings.Where(x => x.SaleStaffID == saleStaff.Id).ToList();
            List<ClinicTimingViewModel> list = new List<ClinicTimingViewModel>();
            foreach (var item in doctortiming)
            {
                ClinicTimingViewModel vm = new ClinicTimingViewModel();
                vm.ID = item.ID;
                vm.StartTime = item.StartTime.ToString("HH:mm tt");
                vm.EndTime = item.EndTime.ToString("HH:mm tt");
                vm.WeekDayName = item.WeekDayName;
                vm.ClinicTimingStr = item.StartTime.ToString("hh:mm tt") + "-" + item.EndTime.ToString("hh:mm tt");

                list.Add(vm);
            }
            List<string> weekdays = new List<string> { "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday" };
            var lstnotinlist = weekdays.Where(p => !doctortiming.Any(p2 => p2.WeekDayName == p)).ToList();
            foreach (var item in lstnotinlist)
            {
                ClinicTimingViewModel vm = new ClinicTimingViewModel();
                vm.ID = 0;
                string one = DateTime.Now.ToString("MM/dd/yyyy");
                vm.StartTime = Convert.ToDateTime(one + " " + "09:00 AM").ToString("HH:mm tt");
                vm.EndTime = Convert.ToDateTime(one + " " + "05:00 PM").ToString("HH:mm tt");
                vm.isHoliday = true;
                vm.WeekDayName = item;
                list.Add(vm);
            }
            ViewBag.ClinicTimining = list;

            return View(saleStaff);
        }

        // POST: SaleStaffs/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,UserId,CreatedOn,CreatedBy,UpdatedOn,UpdatedBy,FirstName,LastName,Gender,MobilePhoneNumber,Email,Address,City")] SaleStaff saleStaff, string[] StartTime, string[] EndTime, string[] WeekDays, bool[] isHoliday)
        {
            if (ModelState.IsValid)
            {
                saleStaff.UpdatedBy = User.Identity.GetUserId();
                saleStaff.UpdatedOn = DateTime.Now;
                _db.Entry(saleStaff).State = EntityState.Modified;
                await _db.SaveChangesAsync();
                try
                {
                    //LiaisonTimings
                    List<DoctorTiming> list = new List<DoctorTiming>();
                    var doctortimings = _db.doctorTimings.Where(x => x.SaleStaffID == saleStaff.Id).ToList();
                    _db.doctorTimings.RemoveRange(doctortimings);
                    for (int i = 0; i < WeekDays.Count(); i++)
                    {
                        if (isHoliday[i] == false)
                        {



                            DoctorTiming ct = new DoctorTiming();

                            string one = DateTime.Now.ToString("MM/dd/yyyy");
                            try
                            {

                                ct.StartTime = Convert.ToDateTime(one + " " + StartTime[i]);
                                ct.EndTime = Convert.ToDateTime(one + " " + EndTime[i]);
                                ct.ClinicID = 1;

                                ct.IsDeleted = false;
                                ct.WeekDayName = WeekDays[i];
                                ct.LiaisonID = null;
                                ct.SaleStaffID = saleStaff.Id;
                                list.Add(ct);

                            }
                            catch (Exception)
                            {
                                ct.ClinicID = 1;
                                ct.StartTime = Convert.ToDateTime(one + " " + "9:00 AM");
                                ct.IsDeleted = false;
                                ct.WeekDayName = WeekDays[i];
                                ct.EndTime = Convert.ToDateTime(one + " " + "5:00 PM");

                                ct.LiaisonID = null;
                                ct.SaleStaffID = saleStaff.Id;
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
            return View(saleStaff);
        }

        // GET: SaleStaffs/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SaleStaff saleStaff = await _db.saleStaffs.FindAsync(id);
            if (saleStaff == null)
            {
                return HttpNotFound();
            }
            return View(saleStaff);
        }

        // POST: SaleStaffs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            SaleStaff saleStaff = await _db.saleStaffs.FindAsync(id);
          
            var existingUser = await UserManager.FindByNameAsync(saleStaff.Email);
            if (existingUser != null)
            {
                //string sExistingId = existingUser.Id;
                //ViewBag.Message = "Email Already Exists: " + sExistingId + "! Liaison Portal Not Created!.";
                await UserManager.DeleteAsync(existingUser);

            }
            _db.saleStaffs.Remove(saleStaff);
            await _db.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        public ActionResult EnrollerWorkLoad()
        {
            return View();
        }
        public PartialViewResult ScheduleList(DateTime date)
        {

            var results1 = _db.saleStaffs.AsNoTracking().Select(x => new { LiaisonID = x.Id, FirstName = x.FirstName, LastName = x.LastName }).ToList();
            if (User.IsInRole("Sales"))
            {
                var user = _db.Users.Find(User.Identity.GetUserId());
                results1 = results1.Where(x => x.LiaisonID == user.CCMid).ToList();
            }
            var results = _db.patientAppointments.AsNoTracking().ToList().Where(x => x.StartTime.Date == date.Date).OrderBy(x=>x.StartTime);
           
            var newresults = results1.Select(x1 => new LiaisonSchedulelist
            {
                LiaisonName = x1.FirstName + " " + x1?.LastName + " (Enroller)",
                Appointmentlst = string.Join("\n", results.Where(x => x.SaleStaffID == x1.LiaisonID).Select(x =>"<p class='pworkload'>"+ x.Subject + "</p> booked From: <p class='pworkload'>" + x.StartTime.ToString("hh:mm tt") + " To: " + x.EndTime.ToString("hh:mm tt")+"</p>").ToList())

            }).Distinct().ToList();
            newresults = newresults.GroupBy(i => i.LiaisonName).Select(i => i.FirstOrDefault()).ToList();
            return PartialView(newresults);
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
