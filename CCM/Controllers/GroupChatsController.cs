using CCM.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace CCM.Controllers
{
    [Authorize(Roles = "Liaison, Physician, PhysiciansGroup, Admin, QAQC, LiaisonGroup, Sales")]
    public class GroupChatsController : BaseController
    {
        //private Application_dbContect _db = new Application_dbContect();

        // GET: GroupChats
        public async Task<ActionResult> Index(string Id = "")
        {
            ViewBag.Users = _db.Users.Where(x => x.Role != "Patient").Select(x2 => new UsersViewModel { Id = x2.Id, Name = x2.FirstName + " " + x2.LastName + " (" + x2.Role + ")" }).ToList();
            //return View(new List<GroupchatViewModel>());
            if (User.IsInRole("Admin"))
            {
                var results = (from gc in _db.groupChats.AsNoTracking()
                                   //join pe in __db.PatientMeidcareMedicaidEligibilities on p.Id equals pe.PatientId into ps
                                   //from pe in ps.DefaultIfEmpty()
                               join gcp1 in _db.GroupChatParticipants.AsNoTracking() on gc.Id equals gcp1.GroupChatId into gcp1

                               from gcp in gcp1.DefaultIfEmpty()
                               select new GroupchatViewModel
                               {
                                   GroupChat = gc,
                                   usersViewModels = _db.Users.Where(x1 => gcp1.Where(x => x.GroupChatId == gc.Id).Select(x => x.UserId).ToList().Contains(x1.Id)).Select(x2 => new UsersViewModel { Id = x2.Id, Name = x2.FirstName + " " + x2.LastName + " (" + x2.Role + ")" }).ToList(),
                                   TotalCount = _db.GroupChatDetails.Where(x1 => x1.GroupChatId == gc.Id).Count()
                               }
                             ).GroupBy(n => new { n.GroupChat.Id })
   .Select(g => g.FirstOrDefault())

   .ToList().OrderByDescending(x=>x.GroupChat.CreatedOn).ToList();
                foreach(var item in results)
                {
                    foreach (var user in item.usersViewModels)
                    {
                        if (user.Name.Contains("Liaison"))
                        {
                            var istranslator = HelperExtensions.isTranslator(user.Id);
                            if (istranslator == true)
                            {
                                user.Name = user.Name.Replace("(Liaison)", "(Translator)");
                            }
                        }
                        else
                        {
                            user.Name = user.Name.Replace("(Sales)", "(Enroller)");

                        }
                    }
                }
                
                var user1 =
                _db.Users.Find(User.Identity.GetUserId());
                var allusers = _db.Users.Where(x => x.Role != "Patient" && x.Id !=user1.Id).Select(x2 => new UsersViewModel { Id = x2.Id, Name = x2.FirstName + " " + x2.LastName + " (" + x2.Role + ")" }).ToList();
                foreach (var user in allusers)
                {
                    if (user.Name.Contains("Liaison"))
                    {
                        var istranslator = HelperExtensions.isTranslator(user.Id);
                        if (istranslator == true)
                        {
                            user.Name = user.Name.Replace("(Liaison)", "(Translator)");
                        }
                    }
                    else
                    {
                        user.Name = user.Name.Replace("(Sales)", "(Enroller)");

                    }
                }
                ViewBag.Users = allusers;
                return View(results);
            }
            else
            {
                var user1 =
                  _db.Users.Find(User.Identity.GetUserId());
                var results = (from gc in _db.groupChats.AsNoTracking()
                                   //join pe in __db.PatientMeidcareMedicaidEligibilities on p.Id equals pe.PatientId into ps
                                   //from pe in ps.DefaultIfEmpty()
                               join gcp1 in _db.GroupChatParticipants.AsNoTracking() on gc.Id equals gcp1.GroupChatId into gcp1

                               from gcp in gcp1.DefaultIfEmpty()
                               where gc.CreatedBy == user1.Id || gcp.UserId == user1.Id
                               select new GroupchatViewModel
                               {
                                   GroupChat = gc,
                                   usersViewModels = _db.Users.Where(x1 => gcp1.Where(x => x.GroupChatId == gc.Id).Select(x => x.UserId).ToList().Contains(x1.Id)).Select(x2 => new UsersViewModel { Id = x2.Id, Name = x2.FirstName + " " + x2.LastName + " (" + x2.Role + ")" }).ToList(),
                                   TotalCount = _db.GroupChatDetails.Where(x1 => x1.GroupChatId == gc.Id).Count()
                               }
                             ).GroupBy(n => new { n.GroupChat.Id })
   .Select(g => g.FirstOrDefault())

   .ToList().OrderByDescending(x => x.GroupChat.CreatedOn).ToList(); ;
                foreach (var item in results)
                {
                    foreach (var user in item.usersViewModels)
                    {
                        if (user.Name.Contains("Liaison"))
                        {
                            var istranslator = HelperExtensions.isTranslator(user.Id);
                            if (istranslator == true)
                            {
                                user.Name = user.Name.Replace("(Liaison)", "(Translator)");
                            }
                        }
                        else
                        {
                            user.Name = user.Name.Replace("(Sales)", "(Enroller)");

                        }
                    }
                }
                var allusers = _db.Users.Where(x => x.Role != "Patient" && x.Id != user1.Id).Select(x2 => new UsersViewModel { Id = x2.Id, Name = x2.FirstName + " " + x2.LastName + " (" + x2.Role + ")" }).ToList();
                foreach(var user in allusers)
                {
                    if (user.Name.Contains("Liaison"))
                    {
                        var istranslator = HelperExtensions.isTranslator(user.Id);
                        if (istranslator == true)
                        {
                            user.Name = user.Name.Replace("(Liaison)", "(Translator)");
                        }
                    }
                    else
                    {
                        user.Name = user.Name.Replace("(Sales)", "(Enroller)");

                    }
                

                }
                ViewBag.Users = allusers;

                return View(results);
            }

        }
        [HttpPost]
        public bool AddnewChat(string chatName, string[] Participents,int chatId,string status="",bool isTicket=false,string TicketTitle="",string Department="")
        {
            try
            {
                // string[] groupchatparticpents = Participents.Split(',');

                if (chatId > 0)
                {
                    var groupChat = _db.GroupsChats.Where(x => x.Id == chatId).FirstOrDefault();
                    if (groupChat.CreatedBy != User.Identity.GetUserId())
                    {
                        return false;
                    }
                    groupChat.ChatName = chatName;
                    groupChat.GroupStatus = status;
                    //if (isTicket == true && groupChat.isTicket==true)
                    //{
                    //    groupChat.Department = Department;
                    //}
                    //else
                    //{
                    //    if (isTicket == true && groupChat.isTicket == false)
                    //    {
                    //        groupChat.isTicket = true;
                    //        groupChat.Department = Department;
                    //        groupChat.TicketTitle = TicketTitle;
                    //        groupChat.TicketNum = "T-" + DateTime.Now.ToString("MMddyyyyhhmm") + "-" + groupChat.Id.ToString();
                    //    }
                     
                    //}
                    groupChat.UpdatedBy = User.Identity.GetUserId();
                    groupChat.UpdatedOn = DateTime.Now;
                    _db.Entry(groupChat).State = EntityState.Modified;
                    _db.SaveChanges();
                    if (Participents.Count() > 0)
                    {
                        var alreadyparticipents = _db.GroupChatParticipants.Where(x => x.GroupChatId == chatId).ToList();
                        _db.GroupChatParticipants.RemoveRange(alreadyparticipents);
                        _db.SaveChanges();
                        foreach (var item in Participents)
                        {
                            GroupChatParticipants groupChatParticipants = new GroupChatParticipants
                            {
                                GroupChatId = groupChat.Id,
                                UserId = item,
                                isCreater=false

                            };
                            _db.GroupChatParticipants.Add(groupChatParticipants);

                        }
                        GroupChatParticipants groupChatParticipants1 = new GroupChatParticipants
                        {
                            GroupChatId = groupChat.Id,
                            UserId = User.Identity.GetUserId(),
                            isCreater = true

                        };
                        _db.GroupChatParticipants.Add(groupChatParticipants1);
                        _db.SaveChanges();
                    }
                }
                else
                {
                  
                    GroupChats groupChat = new GroupChats
                    {
                        ChatName = chatName,
                        GroupStatus = "Open",
                        CreatedBy = User.Identity.GetUserId(),
                        CreatedOn = DateTime.Now,
                       

                    };
                    _db.GroupsChats.Add(groupChat);
                    _db.SaveChanges();
                    if (isTicket == true)
                    {
                        var groupchat = _db.groupChats.Where(x => x.Id == groupChat.Id).FirstOrDefault();
                        groupchat.isTicket = true;
                        groupchat.Department = Department;
                        groupchat.TicketTitle = TicketTitle;
                        groupchat.TicketNum = "T-" + DateTime.Now.ToString("MMddyyyyhhmm")+"-"+groupChat.Id.ToString();
                        _db.Entry(groupChat).State = EntityState.Modified;
                        _db.SaveChanges();

                    }
                    if (Participents.Count() > 0)
                    {
                        foreach (var item in Participents)
                        {
                            GroupChatParticipants groupChatParticipants = new GroupChatParticipants
                            {
                                GroupChatId = groupChat.Id,
                                UserId = item,
                                isCreater = false

                            };
                            _db.GroupChatParticipants.Add(groupChatParticipants);

                        }
                        GroupChatParticipants groupChatParticipants1 = new GroupChatParticipants
                        {
                            GroupChatId = groupChat.Id,
                            UserId = User.Identity.GetUserId(),
                            isCreater = true

                        };
                        _db.GroupChatParticipants.Add(groupChatParticipants1);
                        _db.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {

                return false;
            }
            return true;
        }
        public void AddnewChatmessage(string Message, int GID)
        {
            var chatstatus = _db.groupChats.AsNoTracking().Where(x => x.Id == GID).FirstOrDefault().ChatStatus;
            if (chatstatus == "Open")
            {
                try
                {
                    GroupChatDetails groupChatDetails = new GroupChatDetails
                    {
                        CreatedBy = User.Identity.GetUserId(),
                        CreatedOn = DateTime.Now,
                        GroupChatId = GID,
                        Message = Message,

                    };
                    var userid = User.Identity.GetUserId();
                    var chatrecepitatns = _db.GroupChatParticipants.Where(x => x.GroupChatId == GID && x.UserId != userid).Select(x => x.UserId).ToList();
                    _db.GroupChatDetails.Add(groupChatDetails);
                    _db.SaveChanges();
                    foreach (var item in chatrecepitatns)
                    {
                        GroupChatNewMessage gcnm = new GroupChatNewMessage
                        {
                            GroupChatDetailsId = GID,
                            isNew = true,
                            MessageId = groupChatDetails.Id,
                            ReceivedBy = item,
                            ReceivedOn = DateTime.Now

                        };
                        _db.groupChatNewMessages.Add(gcnm);
                        _db.SaveChanges();
                    }
                }
                catch (Exception ex)
                {


                }

            }
          
        }
        public async Task<JsonResult> GetTotalGroups()
        {
            var user1 =
                      _db.Users.Find(User.Identity.GetUserId()).Id;
            var res1 = _db.GroupChatParticipants.Where(x => x.UserId == user1).Count();
            //var results = await (from gcd in _db.GroupChatParticipants.AsNoTracking()
                              
            //                     where gcd.UserId==user1 && gcd.isCreater==false
                                 
            //                     select new GroupChatTotalCounts { GroupChatId = gcd.GroupChatId, TotalCount =1 }
            //             ).ToListAsync();
            //var totgroups = results.Sum(x => x.TotalCount);
            return Json(res1, JsonRequestBehavior.AllowGet);
        }
        public async Task<JsonResult> GetGroupChatCountsAll()
        {
            var user1 =
                      _db.Users.Find(User.Identity.GetUserId()).Id;
            var results = await (from gcd in _db.GroupChatDetails.AsNoTracking()
                                 group gcd by gcd.GroupChatId into gcd1
                                 select new GroupChatTotalCounts { GroupChatId = gcd1.Key, TotalCount = _db.groupChatNewMessages.Where(x=>x.isNew==true && x.ReceivedBy==user1 && x.GroupChatDetailsId==gcd1.Key).Count() }
                         ).ToListAsync();
            return Json(results, JsonRequestBehavior.AllowGet);
        }
        // GET: GroupChats/Details/5
        public async Task<PartialViewResult> Details(int id = 0, int LastID = 0)
        {
            var user1 =
                    _db.Users.Find(User.Identity.GetUserId()).Id;
            if (1 == 2)
            {
                var results = await (from gcd in _db.GroupChatDetails.AsNoTracking()
                                     join users in _db.Users.AsNoTracking() on gcd.CreatedBy equals users.Id
                                     where gcd.GroupChatId == id && gcd.Id > LastID
                                     select new GroupchatdetailsViewModel
                                     {

                                         Message = gcd.Message,
                                         sender = gcd.CreatedBy != user1 ? users.FirstName + " " + users.LastName + " (" + users.Role + ")" : "You",
                                         timesent = gcd.CreatedOn.Value,
                                         senderclass = gcd.CreatedBy == user1 ? "Sender" : "Receiver",
                                         LastID = gcd.Id
                                     }
                       ).ToListAsync();
                ViewBag.LastID = results.LastOrDefault().LastID;

                return PartialView(results);
            }
            else
            {
                var groupchatdetails = _db.groupChatNewMessages.Where(x => x.GroupChatDetailsId == id && x.isNew == true && x.ReceivedBy==user1).ToList();
                foreach(var item in groupchatdetails)
                {
                    item.isNew = false;
                    _db.Entry(item).State = EntityState.Modified;
                }
                if (groupchatdetails.Count > 0)
                {
                    _db.SaveChanges();
                }

                var results = await (from gcd in _db.GroupChatDetails.AsNoTracking()
                                     join users in _db.Users.AsNoTracking() on gcd.CreatedBy equals users.Id
                                     where gcd.GroupChatId == id
                                     select new GroupchatdetailsViewModel
                                     {

                                         Message = gcd.Message,
                                         sender = gcd.CreatedBy != user1 ? users.FirstName + " " + users.LastName + " (" + users.Role + ")" : "You",
                                         timesent = gcd.CreatedOn.Value,
                                         senderclass = gcd.CreatedBy == user1 ? "Sender" : "Receiver",
                                         LastID = gcd.Id,
                                         userid=gcd.CreatedBy

                                     }
                                       ).ToListAsync();

                ViewBag.LastID = results.Count;
                foreach (var user in results)
                {
                    if (user.sender.Contains("Liaison"))
                    {
                        var istranslator = HelperExtensions.isTranslator(user.userid);
                        if (istranslator == true)
                        {
                            user.sender = user.sender.Replace("(Liaison)", "(Translator)");
                        }
                        user.sender = user.sender.Replace("(Sales)", "(Enroller)");
                    }
                    


                }
                return PartialView(results);
            }

        }
        [HttpPost]
      
        public JsonResult GetChatParticipent(int id)
        {
            var chat = _db.groupChats.Where(x => x.Id == id).FirstOrDefault().CreatedBy;
            if (User.Identity.GetUserId() == chat || User.IsInRole("Admin"))
            {
                var results = _db.GroupChatParticipants.Where(x => x.GroupChatId == id).Select(x => x.UserId).ToList();
                return Json(results);

            }
            else
            {
                return Json(false);
            }
            
        }

        // GET: GroupChats/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: GroupChats/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,ChatName,CreatedOn,CreatedBy,ChatStatus")] GroupChat groupChat)
        {
            if (ModelState.IsValid)
            {
                _db.groupChats.Add(groupChat);
                await _db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(groupChat);
        }

        // GET: GroupChats/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            GroupChat groupChat = await _db.groupChats.FindAsync(id);
            if (groupChat == null)
            {
                return HttpNotFound();
            }
            return View(groupChat);
        }

        // POST: GroupChats/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,ChatName,CreatedOn,CreatedBy,ChatStatus")] GroupChat groupChat)
        {
            if (ModelState.IsValid)
            {
                _db.Entry(groupChat).State = EntityState.Modified;
                await _db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(groupChat);
        }

        // GET: GroupChats/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            GroupChat groupChat = await _db.groupChats.FindAsync(id);
            if (groupChat == null)
            {
                return HttpNotFound();
            }
            return View(groupChat);
        }

        // POST: GroupChats/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            GroupChat groupChat = await _db.groupChats.FindAsync(id);
            _db.groupChats.Remove(groupChat);
            await _db.SaveChangesAsync();
            return RedirectToAction("Index");
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
