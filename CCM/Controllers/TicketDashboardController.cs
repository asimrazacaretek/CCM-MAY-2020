using CCM.Helpers;
using CCM.Models;
using CCM.Models.ENUMS_;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Runtime.Serialization;
using CCM.Models.ENUMS_;
using Microsoft.AspNet.Identity;
using CCM.Models.ViewModels;

namespace CCM.Controllers
{
    [Authorize(Roles = "Liaison, Physician, PhysiciansGroup, Admin, QAQC, LiaisonGroup, Sales")]
    public class TicketDashboardController : BaseController
    {
        //private readonly ApplicationdbContect _db = new ApplicationdbContect();

       
        public class TicketsCharts
        {
            public TicketsCharts()
            {
                this.Tikets = new List<TicketByWeek>();
            }
            public int totalTickets { get; set; }
            public string StartDate { get; set; }
            public string EndDate { get; set; }
            public List<TicketByWeek> Tikets { get; set; }
        }
        public class TicketByWeek
        {
            public int Week { get; set; }
            public int TotalTicket { get; set; }
        }

        // GET: TicketDashboard
        [HttpGet]
        public ActionResult Index()
        {
            try
            {
                List<UserTicketGeneration> userTicketGeneration = new List<UserTicketGeneration>();
                List<AssigneeTicket> assigneeTicket = new List<AssigneeTicket>();
                userTicketGeneration = _db.UserTicketGeneration.ToList();

            assigneeTicket = (from u in userTicketGeneration
                              join at in _db.AssigneeTicket.AsNoTracking()
                              on u.Id equals at.UserTicketGenerationId
                              select at).ToList();
           

            var ticketsGrid = (from u in userTicketGeneration
                               join at in _db.AssigneeTicket.AsNoTracking()
                               on u.Id equals at.UserTicketGenerationId
                               select new
                               {
                                   ticketId = u.Id,
                                   ticketSubject = at.ticketSubject,
                                   ticketType = at.ticketType,
                                   ticketAssignee = HelperExtensions.GetUserNamebyID(u.UserId),
                                   ticketStatus = at.Status?.statusName,
                                   createdDate = u.createdDate,
                                   openTime = DateTimeExtension.ConvertDateIntoString(u.createdDate),
                                   resolveTime = DateTimeExtension.ConvertDateIntoString(at.closesCreatedDate),
                                   inProgressTime = DateTimeExtension.ConvertDateIntoString(at.inProgressCreatedDate),
                                   priority = at.ticketPriority,
                                   createdBy = HelperExtensions.GetUserNamebyID(u.createdBy)
                               }.ToExpando());
                var nonactivetickets = _db.UserTicketGeneration.Where(p => p.Status.statusName == ETicketStatus.OPEN).Select(p => p).ToList();
                ViewBag.pendingactive = nonactivetickets;
                // Dough Chart
                ViewBag.OpenTickets = userTicketGeneration.Where(x => x.Status?.statusName == ETicketStatus.OPEN).Count();
            ViewBag.totalInProgressTickets = userTicketGeneration.Where(x => x.Status?.statusName == ETicketStatus.IN_PROGRESS).Count();
            ViewBag.totalResolveTickets = userTicketGeneration.Where(x => x.Status?.statusName == ETicketStatus.RESOLVED).Count();
            ViewBag.totalUnResovleTickets = userTicketGeneration.Where(x => x.Status?.statusName == ETicketStatus.UNRESOLVED).Count();

            // All Tickets
            ViewBag.ticketGrid = ticketsGrid;

            ViewBag.UserTicketGeneration = userTicketGeneration;
            ViewBag.totalUserTicketGeneration = userTicketGeneration.Count();

            ViewBag.assigneeTicket = assigneeTicket;
            ViewBag.totalAssigneeTicket = assigneeTicket.Count();

            // Charts Data
            ViewBag.totalgenratedticketsbyweek = GetTotalgeneratedTickets(userTicketGeneration);
            ViewBag.totalResoloveticketsbyweek = GetTotalResoloveTickets(userTicketGeneration);
            ViewBag.totalOverDueticketsbyweek = GetTotalOverDueTickets(userTicketGeneration);
                ViewBag.TotalOpenTickets = GetTotalOpenTickets(_db.UserTicketGeneration.ToList());
                ViewBag.TotalActiveTickets = GetTotalActiveTickets(userTicketGeneration);



                //Tickets
                ViewBag.CardOpenTickets = userTicketGeneration.Where(x => x.Status?.statusName == ETicketStatus.OPEN).Select(p => p);
            ViewBag.CardInProgressTickets = userTicketGeneration.Where(x => x.Status?.statusName == ETicketStatus.IN_PROGRESS).Select(p => p);
            ViewBag.CardResolveTickets = userTicketGeneration.Where(x => x.Status?.statusName == ETicketStatus.RESOLVED).Select(p => p);


            //Highest time to resolve ticket
            var resolvetimeDouble = _db.AssigneeTicket.ToList().Count() > 0   ? _db.AssigneeTicket.ToList().Select(p => p.closeResoloutionTime): null;
            var highest = resolvetimeDouble != null ? resolvetimeDouble.Max() : 0;
            TimeSpan highestTime = TimeSpan.FromMinutes(highest);

            int? hdd = highestTime != null ? highestTime.Days : (int?)null;
            int? hhh = highestTime != null ? highestTime.Hours : (int?)null;
            int? hmm = highestTime != null ? highestTime.Minutes : (int?)null;
            int? hss = highestTime != null ? highestTime.Seconds : (int?)null;
            ViewBag.hdays = hdd;
            ViewBag.hhours = hhh;
            ViewBag.hminutes = hmm;
            ViewBag.hseconds = hss; 

            //Average Time to resolve a ticket

            var avgminutesdouble = resolvetimeDouble != null ? resolvetimeDouble.Average(p => p):0;
            TimeSpan averagetime = TimeSpan.FromMinutes(avgminutesdouble);
            int? avgdd = averagetime != null ? averagetime.Days : (int?)null; 
            int? avghh = averagetime != null ? averagetime.Hours : (int?)null;
            int? avgmm = averagetime != null ? averagetime.Minutes : (int?)null;
            int? avgss = averagetime != null ? averagetime.Seconds : (int?)null;
            ViewBag.avgdays = avgdd;
            ViewBag.avghours = avghh;
            ViewBag.avgminutes = avgmm;
            ViewBag.avgseconds = avgss;

              

                //Average time in Progress
                var Progress = _db.AssigneeTicket.ToList().Count() > 0 ? _db.AssigneeTicket.ToList().Select(p => p.inProgressWaitTime):null;
            var AvginProgressdoubles = Progress != null ? Progress.Average(p => p):0;
            TimeSpan averageinprogresstime = TimeSpan.FromMinutes(AvginProgressdoubles);
            int? pavgdd = averageinprogresstime != null ? averageinprogresstime.Days : (int?)null;
            int? pavghh = averageinprogresstime != null ? averageinprogresstime.Hours : (int?)null;
            int? pavgmm = averageinprogresstime != null ? averageinprogresstime.Minutes : (int?)null;
            int? pavgss = averageinprogresstime != null ? averageinprogresstime.Seconds : (int?)null;
            ViewBag.waitingavgdays = pavgdd;
            ViewBag.waitingavghours = pavghh;
            ViewBag.waitingavgminutes = pavgmm;
            ViewBag.waitingavgseconds = pavgss;




            //highest time in Progress
            var HighProgressdoubles = Progress != null ? Progress.Max():0;
            TimeSpan Highinprogresstime = TimeSpan.FromMinutes(HighProgressdoubles);
            int? hwdd = Highinprogresstime != null ? Highinprogresstime.Days : (int?)null;
            int? hwhh = Highinprogresstime != null ? Highinprogresstime.Hours : (int?)null;
            int? hwmm = Highinprogresstime != null ? Highinprogresstime.Minutes : (int?)null;
            int? hwss = Highinprogresstime != null ? Highinprogresstime.Seconds : (int?)null;
            ViewBag.waitinghdays = hwdd;
            ViewBag.waitinghhours = hwhh;
            ViewBag.waitinghminutes = hwmm;
            ViewBag.waitinghseconds = hwss;

                //  all tickets generated with Subject
                var subjectlabels = _db.TicketGeneration.Select(p => p).ToList();
        
                List<SubjectsViewModel> s = new List<SubjectsViewModel>();
                

                foreach (var item in subjectlabels)
                {

                    var res = _db.UserTicketGeneration.Where(p => p.TicketGeneration.subjectName == item.subjectName).Count();

                    SubjectsViewModel c = new SubjectsViewModel();
                   c.SubjectName = item.subjectName;
                    c.Count = res;
                    s.Add(c);
                        

                }


                ViewBag.subjects = s;
                List<SubjectsViewModel> s1 = new List<SubjectsViewModel>();

                //  all tickets generated with Resolution
                var resolutionlabels = _db.ticketResolution.Select(p => p).ToList();
                foreach (var item in resolutionlabels)
                {
                    var res = _db.AssigneeTicket.Where(p => p.TicketResolutionId == item.id).Count();
                    SubjectsViewModel c = new SubjectsViewModel();
                    c.SubjectName = item.resolutionName;
                    c.Count = res;
                    s1.Add(c);

                }
                ViewBag.resolution = s1;

            }
            catch (Exception ex)
            {
                TempData.Remove("AlertMessage");
                TempData.Add("AlertMessage", new AlertModel(ex.Message + "---------" + ex.StackTrace, AlertType.Error));
                AlertModel ar = (AlertModel)TempData["AlertMessage"];
                Session.Add("Alert", ar);
                HelperExtensions.LogError(User.Identity.GetUserName(), User.Identity.GetUserId(), ex.Message, ex.StackTrace);
                return RedirectToAction("exception", "Error");
            }
            return View();
        }

        private dynamic GetTotalOverDueTickets(List<UserTicketGeneration> userTicketGeneration)
        {
            var startdate = DateTime.Now.AddDays(-60);
            var enddate = DateTime.Today;

            if (userTicketGeneration.Count() != 0)
            {

                List<UserTicketGeneration> totalGenratedTickets = userTicketGeneration.Where(x => x.createdDate >= startdate && x.Status?.statusName == ETicketStatus.UNRESOLVED).ToList();

                TicketsCharts GenratedTickets = new TicketsCharts();
                GenratedTickets.totalTickets = totalGenratedTickets.Count();
                GenratedTickets.StartDate = String.Format("{0:G}", startdate.ToShortDateString());
                GenratedTickets.EndDate = String.Format("{0:G}", enddate.ToShortDateString());
                var startweekofyear = CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(startdate, CalendarWeekRule.FirstDay, DayOfWeek.Monday);
                var endweekofyear = CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(enddate, CalendarWeekRule.FirstDay, DayOfWeek.Monday);
                var ticketbyweeks = totalGenratedTickets.GroupBy(i => CultureInfo.CurrentCulture.Calendar.GetWeekOfYear((DateTime)i.createdDate, CalendarWeekRule.FirstDay, DayOfWeek.Monday)).Select(n => new
                {
                    weekofyear = n.Key,
                    totalCount = n.Count()
                }
                ).ToList();

                if (ticketbyweeks.Count() != 0)
                {
                    while (startweekofyear < ticketbyweeks[0].weekofyear)
                    {
                        GenratedTickets.Tikets.Add(new TicketByWeek()
                        {
                            Week = startweekofyear,
                            TotalTicket = 0
                        });
                        startweekofyear++;
                    }
                    foreach (var i in ticketbyweeks)
                    {
                        GenratedTickets.Tikets.Add(new TicketByWeek()
                        {
                            Week = i.weekofyear,
                            TotalTicket = i.totalCount
                        });
                    }
                    while (endweekofyear > ticketbyweeks[(ticketbyweeks.Count() - 1)].weekofyear)
                    {
                        GenratedTickets.Tikets.Add(new TicketByWeek()
                        {
                            Week = startweekofyear,
                            TotalTicket = 0
                        });
                        endweekofyear--;
                    }
                }
                return GenratedTickets;
            }
            return new TicketsCharts();
        }

        private dynamic GetTotalResoloveTickets(List<UserTicketGeneration> userTicketGeneration)
        {
            var startdate = DateTime.Now.AddDays(-60);
            var enddate = DateTime.Today;

            if (userTicketGeneration.Count() != 0)
            {

                List<UserTicketGeneration> totalGenratedTickets = userTicketGeneration.Count() > 0 ? userTicketGeneration.Where(x => x.createdDate >= startdate && x.Status?.statusName == ETicketStatus.RESOLVED).ToList() : new List<UserTicketGeneration>();

                TicketsCharts GenratedTickets = new TicketsCharts();
                GenratedTickets.totalTickets = totalGenratedTickets.Count();
                GenratedTickets.StartDate = String.Format("{0:G}", startdate.ToShortDateString());
                GenratedTickets.EndDate = String.Format("{0:G}", enddate.ToShortDateString());
                var startweekofyear = CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(startdate, CalendarWeekRule.FirstDay, DayOfWeek.Monday);
                var endweekofyear = CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(enddate, CalendarWeekRule.FirstDay, DayOfWeek.Monday);
                var ticketbyweeks = totalGenratedTickets.GroupBy(i => CultureInfo.CurrentCulture.Calendar.GetWeekOfYear((DateTime)i.createdDate, CalendarWeekRule.FirstDay, DayOfWeek.Monday)).Select(n => new
                {
                    weekofyear = n.Key,
                    totalCount = n.Count()
                }
                ).ToList();

                if (ticketbyweeks.Count() != 0) { 
                    while (startweekofyear < ticketbyweeks[0].weekofyear)
                    {
                        GenratedTickets.Tikets.Add(new TicketByWeek()
                        {
                            Week = startweekofyear,
                            TotalTicket = 0
                        });
                        startweekofyear++;
                    }
                foreach (var i in ticketbyweeks)
                {
                    GenratedTickets.Tikets.Add(new TicketByWeek()
                    {
                        Week = i.weekofyear,
                        TotalTicket = i.totalCount
                    });
                }
                while (endweekofyear > ticketbyweeks[(ticketbyweeks.Count() - 1)].weekofyear)
                {
                    GenratedTickets.Tikets.Add(new TicketByWeek()
                    {
                        Week = startweekofyear,
                        TotalTicket = 0
                    });
                    endweekofyear--;
                }
            }
                return GenratedTickets;
            }
            return new TicketsCharts();
        }

        private dynamic GetTotalgeneratedTickets(List<UserTicketGeneration> userTicketGeneration)
        {
            var startdate = DateTime.Now.AddDays(-60);
            var enddate = DateTime.Today;


            if (userTicketGeneration.Count() != 0)
            {


                List<UserTicketGeneration> totalGenratedTickets = userTicketGeneration.Count() > 0 ? userTicketGeneration.Where(x => x.createdDate >= startdate ).ToList() : new List<UserTicketGeneration>();

                TicketsCharts GenratedTickets = new TicketsCharts();
                GenratedTickets.totalTickets = totalGenratedTickets.Count();
                GenratedTickets.StartDate = String.Format("{0:G}", startdate.ToShortDateString());
                GenratedTickets.EndDate = String.Format("{0:G}", enddate.ToShortDateString());
                var startweekofyear = CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(startdate, CalendarWeekRule.FirstDay, DayOfWeek.Monday);
                var endweekofyear = CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(enddate, CalendarWeekRule.FirstDay, DayOfWeek.Monday);
                var ticketbyweeks = totalGenratedTickets.GroupBy(i => CultureInfo.CurrentCulture.Calendar.GetWeekOfYear((DateTime)i.createdDate, CalendarWeekRule.FirstDay, DayOfWeek.Monday)).Select(n => new
                {
                    weekofyear = n.Key,
                    totalCount = n.Count()
                }
                ).ToList();
                if (ticketbyweeks.Count() != 0) { 
                while (startweekofyear < ticketbyweeks[0].weekofyear)
                {
                    GenratedTickets.Tikets.Add(new TicketByWeek()
                    {
                        Week = startweekofyear,
                        TotalTicket = 0
                    });
                    startweekofyear++;
                }
                foreach (var i in ticketbyweeks)
                {
                    GenratedTickets.Tikets.Add(new TicketByWeek()
                    {
                        Week = i.weekofyear,
                        TotalTicket = i.totalCount
                    });
                }
                while (endweekofyear > ticketbyweeks[(ticketbyweeks.Count() - 1)].weekofyear)
                {
                    GenratedTickets.Tikets.Add(new TicketByWeek()
                    {
                        Week = startweekofyear,
                        TotalTicket = 0
                    });
                    endweekofyear--;
                }
                }
                return GenratedTickets;

            }
            return new TicketsCharts();
        }
        private dynamic GetTotalOpenTickets(List<UserTicketGeneration> userTicketGeneration)
        {
            var startdate = DateTime.Now.AddDays(-60);
            var enddate = DateTime.Today;


            if (userTicketGeneration.Count() != 0)
            {


                List<UserTicketGeneration> totalGenratedTickets = userTicketGeneration.Count() > 0 ? userTicketGeneration.Where(x => x.createdDate >= startdate && x.Status?.statusName == ETicketStatus.OPEN).ToList() : new List<UserTicketGeneration>();

                TicketsCharts GenratedTickets = new TicketsCharts();
                GenratedTickets.totalTickets = totalGenratedTickets.Count();
                GenratedTickets.StartDate = String.Format("{0:G}", startdate.ToShortDateString());
                GenratedTickets.EndDate = String.Format("{0:G}", enddate.ToShortDateString());
                var startweekofyear = CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(startdate, CalendarWeekRule.FirstDay, DayOfWeek.Monday);
                var endweekofyear = CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(enddate, CalendarWeekRule.FirstDay, DayOfWeek.Monday);
                var ticketbyweeks = totalGenratedTickets.GroupBy(i => CultureInfo.CurrentCulture.Calendar.GetWeekOfYear((DateTime)i.createdDate, CalendarWeekRule.FirstDay, DayOfWeek.Monday)).Select(n => new
                {
                    weekofyear = n.Key,
                    totalCount = n.Count()
                }
                ).ToList();
                if (ticketbyweeks.Count() != 0)
                {
                    while (startweekofyear < ticketbyweeks[0].weekofyear)
                    {
                        GenratedTickets.Tikets.Add(new TicketByWeek()
                        {
                            Week = startweekofyear,
                            TotalTicket = 0
                        });
                        startweekofyear++;
                    }
                    foreach (var i in ticketbyweeks)
                    {
                        GenratedTickets.Tikets.Add(new TicketByWeek()
                        {
                            Week = i.weekofyear,
                            TotalTicket = i.totalCount
                        });
                    }
                    while (endweekofyear > ticketbyweeks[(ticketbyweeks.Count() - 1)].weekofyear)
                    {
                        GenratedTickets.Tikets.Add(new TicketByWeek()
                        {
                            Week = startweekofyear,
                            TotalTicket = 0
                        });
                        endweekofyear--;
                    }
                }
                return GenratedTickets;

            }
            return new TicketsCharts();
        }
        private dynamic GetTotalActiveTickets(List<UserTicketGeneration> userTicketGeneration)
        {
            var startdate = DateTime.Now.AddDays(-60);
            var enddate = DateTime.Today;


            if (userTicketGeneration.Count() != 0)
            {


                List<UserTicketGeneration> totalGenratedTickets = userTicketGeneration.Count() > 0 ? userTicketGeneration.Where(x => x.createdDate >= startdate && x.Status?.statusName == ETicketStatus.IN_PROGRESS).ToList() : new List<UserTicketGeneration>();

                TicketsCharts GenratedTickets = new TicketsCharts();
                GenratedTickets.totalTickets = totalGenratedTickets.Count();
                GenratedTickets.StartDate = String.Format("{0:G}", startdate.ToShortDateString());
                GenratedTickets.EndDate = String.Format("{0:G}", enddate.ToShortDateString());
                var startweekofyear = CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(startdate, CalendarWeekRule.FirstDay, DayOfWeek.Monday);
                var endweekofyear = CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(enddate, CalendarWeekRule.FirstDay, DayOfWeek.Monday);
                var ticketbyweeks = totalGenratedTickets.GroupBy(i => CultureInfo.CurrentCulture.Calendar.GetWeekOfYear((DateTime)i.createdDate, CalendarWeekRule.FirstDay, DayOfWeek.Monday)).Select(n => new
                {
                    weekofyear = n.Key,
                    totalCount = n.Count()
                }
                ).ToList();
                if (ticketbyweeks.Count() != 0)
                {
                    while (startweekofyear < ticketbyweeks[0].weekofyear)
                    {
                        GenratedTickets.Tikets.Add(new TicketByWeek()
                        {
                            Week = startweekofyear,
                            TotalTicket = 0
                        });
                        startweekofyear++;
                    }
                    foreach (var i in ticketbyweeks)
                    {
                        GenratedTickets.Tikets.Add(new TicketByWeek()
                        {
                            Week = i.weekofyear,
                            TotalTicket = i.totalCount
                        });
                    }
                    while (endweekofyear > ticketbyweeks[(ticketbyweeks.Count() - 1)].weekofyear)
                    {
                        GenratedTickets.Tikets.Add(new TicketByWeek()
                        {
                            Week = startweekofyear,
                            TotalTicket = 0
                        });
                        endweekofyear--;
                    }
                }
                return GenratedTickets;

            }
            return new TicketsCharts();
        }

        [HttpPost]
        public ActionResult Index(TicketDashboardViewModel obj)
        {
            List<UserTicketGeneration> userTicketGeneration = new List<UserTicketGeneration>();
            List<AssigneeTicket> assigneeTicket = new List<AssigneeTicket>();

            try
            {
                if (obj.EndDate == null && obj.StartDate == null && obj.TicketAssignee == null && obj.Tickettype == null && obj.ticketSubjectDashboard == null && obj.status == null && obj.createdBy == null)
                {
                    userTicketGeneration = _db.UserTicketGeneration.ToList();
                    assigneeTicket = (from u in userTicketGeneration
                                      join at in _db.AssigneeTicket.AsNoTracking()
                                      on u.Id equals at.UserTicketGenerationId
                                      select at).ToList();

                    var ticketsGrid = (from u in userTicketGeneration
                                       join at in _db.AssigneeTicket.AsNoTracking()
                                       on u.Id equals at.UserTicketGenerationId
                                       select new
                                       {
                                           ticketId = u.Id,
                                           ticketSubject = at.ticketSubject,
                                           ticketType = at.ticketType,
                                           ticketAssignee = HelperExtensions.GetUserNamebyID(u.UserId),
                                           ticketStatus = at.Status?.statusName,
                                           createdDate = u.createdDate,
                                           openTime = DateTimeExtension.ConvertDateIntoString(u.createdDate),
                                           resolveTime = DateTimeExtension.ConvertDateIntoString(at.closesCreatedDate),
                                           inProgressTime = DateTimeExtension.ConvertDateIntoString(at.inProgressCreatedDate),
                                           priority = at.ticketPriority,
                                           createdBy = HelperExtensions.GetUserNamebyID(u.createdBy)
                                       }.ToExpando());


                    var nonactivetickets = _db.UserTicketGeneration.Where(p => p.Status.statusName == ETicketStatus.OPEN).Select(p => p).ToList();
                    ViewBag.pendingactive = nonactivetickets;
                    // Dough Chart
                    ViewBag.OpenTickets = userTicketGeneration.Where(x => x.Status?.statusName == ETicketStatus.OPEN).Count();
                    ViewBag.totalInProgressTickets = userTicketGeneration.Where(x => x.Status?.statusName == ETicketStatus.IN_PROGRESS).Count();
                    ViewBag.totalResolveTickets = userTicketGeneration.Where(x => x.Status?.statusName == ETicketStatus.RESOLVED).Count();
                    ViewBag.totalUnResovleTickets = userTicketGeneration.Where(x => x.Status?.statusName == ETicketStatus.UNRESOLVED).Count();

                    // All Tickets
                    ViewBag.ticketGrid = ticketsGrid;
                    ViewBag.UserTicketGeneration = userTicketGeneration;
                    ViewBag.totalUserTicketGeneration = userTicketGeneration.Count();


                    // Charts Data
                    ViewBag.totalgenratedticketsbyweek = GetTotalgeneratedTickets(userTicketGeneration);
                    ViewBag.totalResoloveticketsbyweek = GetTotalResoloveTickets(userTicketGeneration);
                    ViewBag.totalOverDueticketsbyweek = GetTotalOverDueTickets(userTicketGeneration);
                    ViewBag.TotalOpenTickets = GetTotalOpenTickets(_db.UserTicketGeneration.ToList());
                    ViewBag.TotalActiveTickets = GetTotalActiveTickets(userTicketGeneration);

                    ViewBag.assigneeTicket = assigneeTicket;
                    ViewBag.totalAssigneeTicket = assigneeTicket.Count();


                    //Tickets
                    ViewBag.CardOpenTickets = userTicketGeneration.Where(x => x.Status?.statusName == ETicketStatus.OPEN).Select(p => p);
                    ViewBag.CardInProgressTickets = userTicketGeneration.Where(x => x.Status?.statusName == ETicketStatus.IN_PROGRESS).Select(p => p);
                    ViewBag.CardResolveTickets = userTicketGeneration.Where(x => x.Status?.statusName == ETicketStatus.RESOLVED).Select(p => p);


                    //Highest time to resolve ticket
                    var resolvetimeDouble = _db.AssigneeTicket.ToList().Count() > 0 ? _db.AssigneeTicket.ToList().Select(p => p.closeResoloutionTime) : null;
                    var highest = resolvetimeDouble != null ? resolvetimeDouble.Max() : 0;
                    TimeSpan highestTime = TimeSpan.FromMinutes(highest);

                    int? hdd = highestTime != null ? highestTime.Days : (int?)null;
                    int? hhh = highestTime != null ? highestTime.Hours : (int?)null;
                    int? hmm = highestTime != null ? highestTime.Minutes : (int?)null;
                    int? hss = highestTime != null ? highestTime.Seconds : (int?)null;
                    ViewBag.hdays = hdd;
                    ViewBag.hhours = hhh;
                    ViewBag.hminutes = hmm;
                    ViewBag.hseconds = hss;

                    //Average Time to resolve a ticket

                    var avgminutesdouble = resolvetimeDouble != null ? resolvetimeDouble.Average(p => p) : 0;
                    TimeSpan averagetime = TimeSpan.FromMinutes(avgminutesdouble);
                    int? avgdd = averagetime != null ? averagetime.Days : (int?)null;
                    int? avghh = averagetime != null ? averagetime.Hours : (int?)null;
                    int? avgmm = averagetime != null ? averagetime.Minutes : (int?)null;
                    int? avgss = averagetime != null ? averagetime.Seconds : (int?)null;
                    ViewBag.avgdays = avgdd;
                    ViewBag.avghours = avghh;
                    ViewBag.avgminutes = avgmm;
                    ViewBag.avgseconds = avgss;



                    //Average time in Progress
                    var Progress = _db.AssigneeTicket.ToList().Count() > 0 ? _db.AssigneeTicket.ToList().Select(p => p.inProgressWaitTime) : null;
                    var AvginProgressdoubles = Progress != null ? Progress.Average(p => p) : 0;
                    TimeSpan averageinprogresstime = TimeSpan.FromMinutes(AvginProgressdoubles);
                    int? pavgdd = averageinprogresstime != null ? averageinprogresstime.Days : (int?)null;
                    int? pavghh = averageinprogresstime != null ? averageinprogresstime.Hours : (int?)null;
                    int? pavgmm = averageinprogresstime != null ? averageinprogresstime.Minutes : (int?)null;
                    int? pavgss = averageinprogresstime != null ? averageinprogresstime.Seconds : (int?)null;
                    ViewBag.waitingavgdays = pavgdd;
                    ViewBag.waitingavghours = pavghh;
                    ViewBag.waitingavgminutes = pavgmm;
                    ViewBag.waitingavgseconds = pavgss;




                    //highest time in Progress
                    var HighProgressdoubles = Progress != null ? Progress.Max() : 0;
                    TimeSpan Highinprogresstime = TimeSpan.FromMinutes(HighProgressdoubles);
                    int? hwdd = Highinprogresstime != null ? Highinprogresstime.Days : (int?)null;
                    int? hwhh = Highinprogresstime != null ? Highinprogresstime.Hours : (int?)null;
                    int? hwmm = Highinprogresstime != null ? Highinprogresstime.Minutes : (int?)null;
                    int? hwss = Highinprogresstime != null ? Highinprogresstime.Seconds : (int?)null;
                    ViewBag.waitinghdays = hwdd;
                    ViewBag.waitinghhours = hwhh;
                    ViewBag.waitinghminutes = hwmm;
                    ViewBag.waitinghseconds = hwss;

                    //  all tickets generated with Subject
                    var subjectlabels = _db.TicketGeneration.Select(p => p).ToList();

                    List<SubjectsViewModel> s = new List<SubjectsViewModel>();


                    foreach (var item in subjectlabels)
                    {

                        var res = _db.UserTicketGeneration.Where(p => p.TicketGeneration.subjectName == item.subjectName).Count();

                        SubjectsViewModel c = new SubjectsViewModel();
                        c.SubjectName = item.subjectName;
                        c.Count = res;
                        s.Add(c);


                    }


                    ViewBag.subjects = s;
                    List<SubjectsViewModel> s1 = new List<SubjectsViewModel>();

                    //  all tickets generated with Resolution
                    var resolutionlabels = _db.ticketResolution.Select(p => p).ToList();
                    foreach (var item in resolutionlabels)
                    {
                        var res = _db.AssigneeTicket.Where(p => p.TicketResolutionId == item.id).Count();
                        SubjectsViewModel c = new SubjectsViewModel();
                        c.SubjectName = item.resolutionName;
                        c.Count = res;
                        s1.Add(c);

                    }
                    ViewBag.resolution = s1;


                }
                else
                {
                    userTicketGeneration = _db.UserTicketGeneration.ToList();

                    if (obj.StartDate != null)
                    {
                        userTicketGeneration = userTicketGeneration.Where(x => x.createdDate.Value.Date >= obj.StartDate.Value.Date).AsEnumerable().ToList();
                    }
                    if (obj.EndDate != null)
                    {
                        userTicketGeneration = userTicketGeneration.Where(x => x.createdDate.Value.Date <= obj.EndDate.Value.Date).AsEnumerable().ToList();
                    }
                    if (obj.Tickettype != null)
                    {
                        var ticketTypeId = Convert.ToInt32(obj.Tickettype);
                        userTicketGeneration = userTicketGeneration.Where(x => x.TypeId == ticketTypeId).ToList();
                    }
                    if (obj.ticketSubjectDashboard != null)
                    {
                        var ticketSubjectDashboard = Convert.ToInt32(obj.ticketSubjectDashboard);
                        userTicketGeneration = userTicketGeneration.Where(x => x.TicketGenerationId == ticketSubjectDashboard).ToList();
                    }
                    if (obj.TicketAssignee != null)
                    {
                        var ticketAssignee = obj.TicketAssignee;
                        userTicketGeneration = userTicketGeneration.Where(x => x.UserId == obj.TicketAssignee).ToList();
                    }
                    if (obj.createdBy != null)
                    {
                        userTicketGeneration = userTicketGeneration.Where(x => x.createdBy == obj.createdBy).ToList();
                    }
                    if (obj.status != null)
                    {
                        int? statussId = Convert.ToInt32(obj.status);
                        userTicketGeneration = userTicketGeneration.Where(x => x.StatusId == statussId).ToList();
                    }




                    assigneeTicket = (from u in userTicketGeneration
                                      join at in _db.AssigneeTicket.AsNoTracking()
                                      on u.Id equals at.UserTicketGenerationId
                                      select at).ToList();

                    var ticketsGrid = (from u in userTicketGeneration
                                       join at in _db.AssigneeTicket.AsNoTracking()
                                       on u.Id equals at.UserTicketGenerationId
                                       select new
                                       {
                                           ticketId = u.Id,
                                           ticketSubject = at.ticketSubject,
                                           ticketType = at.ticketType,
                                           ticketAssignee = HelperExtensions.GetUserNamebyID(u.UserId),
                                           ticketStatus = at.Status?.statusName,
                                           createdDate = u.createdDate,
                                           openTime = DateTimeExtension.ConvertDateIntoString(u.createdDate),
                                           resolveTime = DateTimeExtension.ConvertDateIntoString(at.closesCreatedDate),
                                           inProgressTime = DateTimeExtension.ConvertDateIntoString(at.inProgressCreatedDate),
                                           priority = at.ticketPriority,
                                           createdBy = HelperExtensions.GetUserNamebyID(u.createdBy)
                                       }.ToExpando());

                    var nonactivetickets = _db.UserTicketGeneration.Where(p => p.Status.statusName == ETicketStatus.OPEN).Select(p => p).ToList();
                    ViewBag.pendingactive = nonactivetickets;

                    // Dough Chart
                    ViewBag.OpenTickets = userTicketGeneration.Where(x => x.Status?.statusName == ETicketStatus.OPEN).Count();
                    ViewBag.totalInProgressTickets = userTicketGeneration.Where(x => x.Status?.statusName == ETicketStatus.IN_PROGRESS).Count();
                    ViewBag.totalResolveTickets = userTicketGeneration.Where(x => x.Status?.statusName == ETicketStatus.RESOLVED).Count();
                    ViewBag.totalUnResovleTickets = userTicketGeneration.Where(x => x.Status?.statusName == ETicketStatus.UNRESOLVED).Count();

                    // All Tickets
                    ViewBag.ticketGrid = ticketsGrid;

                    ViewBag.UserTicketGeneration = userTicketGeneration;
                    ViewBag.totalUserTicketGeneration = userTicketGeneration.Count();

                    // Charts Data
                    ViewBag.totalgenratedticketsbyweek = GetTotalgeneratedTickets(userTicketGeneration);
                    ViewBag.totalResoloveticketsbyweek = GetTotalResoloveTickets(userTicketGeneration);
                    ViewBag.totalOverDueticketsbyweek = GetTotalOverDueTickets(userTicketGeneration);
                    ViewBag.TotalOpenTickets = GetTotalOpenTickets(_db.UserTicketGeneration.ToList());
                    ViewBag.TotalActiveTickets = GetTotalActiveTickets(userTicketGeneration);


                    ViewBag.assigneeTicket = assigneeTicket;
                    ViewBag.totalAssigneeTicket = assigneeTicket.Count();


                    //Tickets
                    ViewBag.CardOpenTickets = userTicketGeneration.Where(x => x.Status?.statusName == ETicketStatus.OPEN).Select(p => p);
                    ViewBag.CardInProgressTickets = userTicketGeneration.Where(x => x.Status?.statusName == ETicketStatus.IN_PROGRESS).Select(p => p);
                    ViewBag.CardResolveTickets = userTicketGeneration.Where(x => x.Status?.statusName == ETicketStatus.RESOLVED).Select(p => p);


                    //Highest time to resolve ticket
                    var resolvetimeDouble = _db.AssigneeTicket.ToList().Count() > 0 ? _db.AssigneeTicket.ToList().Select(p => p.closeResoloutionTime) : null;
                    var highest = resolvetimeDouble != null ? resolvetimeDouble.Max() : 0;
                    TimeSpan highestTime = TimeSpan.FromMinutes(highest);

                    int? hdd = highestTime != null ? highestTime.Days : (int?)null;
                    int? hhh = highestTime != null ? highestTime.Hours : (int?)null;
                    int? hmm = highestTime != null ? highestTime.Minutes : (int?)null;
                    int? hss = highestTime != null ? highestTime.Seconds : (int?)null;
                    ViewBag.hdays = hdd;
                    ViewBag.hhours = hhh;
                    ViewBag.hminutes = hmm;
                    ViewBag.hseconds = hss;

                    //Average Time to resolve a ticket

                    var avgminutesdouble = resolvetimeDouble != null ? resolvetimeDouble.Average(p => p) : 0;
                    TimeSpan averagetime = TimeSpan.FromMinutes(avgminutesdouble);
                    int? avgdd = averagetime != null ? averagetime.Days : (int?)null;
                    int? avghh = averagetime != null ? averagetime.Hours : (int?)null;
                    int? avgmm = averagetime != null ? averagetime.Minutes : (int?)null;
                    int? avgss = averagetime != null ? averagetime.Seconds : (int?)null;
                    ViewBag.avgdays = avgdd;
                    ViewBag.avghours = avghh;
                    ViewBag.avgminutes = avgmm;
                    ViewBag.avgseconds = avgss;



                    //Average time in Progress
                    var Progress = _db.AssigneeTicket.ToList().Count() > 0 ? _db.AssigneeTicket.ToList().Select(p => p.inProgressWaitTime) : null;
                    var AvginProgressdoubles = Progress != null ? Progress.Average(p => p) : 0;
                    TimeSpan averageinprogresstime = TimeSpan.FromMinutes(AvginProgressdoubles);
                    int? pavgdd = averageinprogresstime != null ? averageinprogresstime.Days : (int?)null;
                    int? pavghh = averageinprogresstime != null ? averageinprogresstime.Hours : (int?)null;
                    int? pavgmm = averageinprogresstime != null ? averageinprogresstime.Minutes : (int?)null;
                    int? pavgss = averageinprogresstime != null ? averageinprogresstime.Seconds : (int?)null;
                    ViewBag.waitingavgdays = pavgdd;
                    ViewBag.waitingavghours = pavghh;
                    ViewBag.waitingavgminutes = pavgmm;
                    ViewBag.waitingavgseconds = pavgss;




                    //highest time in Progress
                    var HighProgressdoubles = Progress != null ? Progress.Max() : 0;
                    TimeSpan Highinprogresstime = TimeSpan.FromMinutes(HighProgressdoubles);
                    int? hwdd = Highinprogresstime != null ? Highinprogresstime.Days : (int?)null;
                    int? hwhh = Highinprogresstime != null ? Highinprogresstime.Hours : (int?)null;
                    int? hwmm = Highinprogresstime != null ? Highinprogresstime.Minutes : (int?)null;
                    int? hwss = Highinprogresstime != null ? Highinprogresstime.Seconds : (int?)null;
                    ViewBag.waitinghdays = hwdd;
                    ViewBag.waitinghhours = hwhh;
                    ViewBag.waitinghminutes = hwmm;
                    ViewBag.waitinghseconds = hwss;
                    //  all tickets generated with Subject
                    var subjectlabels = _db.TicketGeneration.Select(p => p).ToList();

                    List<SubjectsViewModel> s = new List<SubjectsViewModel>();


                    foreach (var item in subjectlabels)
                    {

                        var res = _db.UserTicketGeneration.Where(p => p.TicketGeneration.subjectName == item.subjectName).Count();

                        SubjectsViewModel c = new SubjectsViewModel();
                        c.SubjectName = item.subjectName;
                        c.Count = res;
                        s.Add(c);


                    }


                    ViewBag.subjects = s;
                    List<SubjectsViewModel> s1 = new List<SubjectsViewModel>();

                    //  all tickets generated with Resolution
                    var resolutionlabels = _db.ticketResolution.Select(p => p).ToList();
                    foreach (var item in resolutionlabels)
                    {
                        var res = _db.AssigneeTicket.Where(p => p.TicketResolutionId == item.id).Count();
                        SubjectsViewModel c = new SubjectsViewModel();
                        c.SubjectName = item.resolutionName;
                        c.Count = res;
                        s1.Add(c);

                    }
                    ViewBag.resolution = s1;


                }

            }
            catch (Exception ex)
            {
                TempData.Remove("AlertMessage");
                TempData.Add("AlertMessage", new AlertModel(ex.Message + "---------" + ex.StackTrace, AlertType.Error));
                AlertModel ar = (AlertModel)TempData["AlertMessage"];
                Session.Add("Alert", ar);
                HelperExtensions.LogError(User.Identity.GetUserName(), User.Identity.GetUserId(), ex.Message, ex.StackTrace);
                return RedirectToAction("exception", "Error");
                //return "false";

            }
            return View();
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