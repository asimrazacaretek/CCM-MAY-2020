using CCM.Helpers;
using CCM.Models;
using CCM.Models.DataModels;
using CCM.Models.ENUMS_;
using CCM.Models.ViewModels;
using Microsoft.Ajax.Utilities;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;


namespace CCM.Controllers
{
    [Authorize(Roles = "Liaison, Physician, PhysiciansGroup, Admin, QAQC, LiaisonGroup, Sales")]
    public class ChatsController : BaseController
    {
       // private Application_dbContect _db = new Application_dbContect();
        // GET: Chats
        public ActionResult Index()
        {
            var user1 = _db.Users.Find(User.Identity.GetUserId());
            var allusers = _db.Users.Where(x => x.Role != "Patient" && x.Id != user1.Id).Select(x2 => new UsersViewModel { Id = x2.Id, Name = x2.FirstName + " " + x2.LastName + " (" + x2.Role + ")" }).ToList();
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
            return View();
        }
        public PartialViewResult _SingleChat()
        {
            var CurrentUserId = User.Identity.GetUserId();
            ViewBag.Users = _db.Users.Where(x => x.Role != "Patient").Select(x2 => new UsersViewModel { Id = x2.Id, Name = x2.FirstName + " " + x2.LastName + " (" + x2.Role + ")" }).ToList();
            var TicketHistory = _db.TicketNotificationHistory.Where(x => ( x.UserId == CurrentUserId && x.NotificationCreatedBy != CurrentUserId ) ||( x.createdBy==CurrentUserId &&x.NotificationCreatedBy!=CurrentUserId && x.NotificationCreatedBy != null) ||( x.ticketHonorId==CurrentUserId &&x.NotificationCreatedBy!=CurrentUserId && x.NotificationCreatedBy != null) ).ToList();
            var NotificationHistoryList = new List<NotificationViewMidal>();
            foreach (var item in TicketHistory)
            {
                var Notification = new NotificationViewMidal();
                if (item.isTicketHonor == true)
                {
                    Notification.AssigneeName = HelperExtensions.GetUserNamebyID(item.createdBy);
                }
                else
                {
                    Notification.AssigneeName = HelperExtensions.GetUserNamebyID(item.createdBy);
                }
         
                DateTime time = Convert.ToDateTime(item.createdDate);
                var totalTime = DateTime.Now.Subtract(time).TotalMinutes;

                int timeToCheck = Convert.ToInt32(totalTime);
                if (timeToCheck <= 59)
                {
                    Notification.Time = timeToCheck.ToString() + " min ago";
                }
                else if (timeToCheck >= 60 && timeToCheck <= 1439)
                {

                    decimal hours = timeToCheck / 60;
                    hours = Math.Floor(hours);
                    Notification.Time = hours.ToString() + " hr ago";
                }
                else if (timeToCheck >= 1440 && timeToCheck <= 43799)
                {
                    decimal days = timeToCheck / 1440;
                    days = Math.Floor(days);
                    Notification.Time = days.ToString() + " day ago";
                }
                else if (timeToCheck >= 43800 && timeToCheck <= 525599)
                {
                    decimal months = timeToCheck / 43800;
                    months = Math.Floor(months);
                    Notification.Time = months.ToString() + " Month ago";
                }
                else
                {
                    decimal year = timeToCheck / 525600;
                    year = Math.Floor(year);
                    Notification.Time = year.ToString() + " year ago";
                }
                Notification.createdDate = item.createdDate;
                Notification.patientId = item.PatientId;
                Notification.notify = item.notify;

                Notification.Priority = item.Priority != null ? item.Priority.priorityLevel : "";

                Notification.Id =item.Id;
                Notification.TicketType = item.Type != null ? item.Type.typeName : "";
                Notification.CreatorNotes = item.creatorNotes;
                Notification.TicketId = item.UserTicketGenerationId;
                Notification.Status = item.Status != null ? item.Status.statusName : "";
                Notification.ChangingType = item.ChangingType;
                NotificationHistoryList.Add(Notification);
            }

            ViewBag.TicketHistory = NotificationHistoryList.OrderByDescending(x=>x.Id).ToList();


            //return View(new List<GroupchatViewModel>());
            if (User.IsInRole("Admin"))
            {
                var results = (from gc in _db.GroupsChats.AsNoTracking()
                                   //join pe in __db.PatientMeidcareMedicaidEligibilities on p.Id equals pe.PatientId into ps
                                   //from pe in ps.DefaultIfEmpty()
                               join gcp1 in _db.GroupChatsParticipants.AsNoTracking() on gc.Id equals gcp1.GroupChatId into gcp1

                               from gcp in gcp1.DefaultIfEmpty()
                               select new GroupchatsViewModel
                               {
                                   GroupChat = gc,
                                   usersViewModels = _db.Users.Where(x1 => gcp1.Where(x => x.GroupChatId == gc.Id).Select(x => x.UserId).ToList().Contains(x1.Id)).Select(x2 => new UsersViewModel { Id = x2.Id, Name = x2.FirstName + " " + x2.LastName + " (" + x2.Role + ")" }).ToList(),
                                   TotalCount = _db.GroupChatsDetails.Where(x1 => x1.GroupChatId == gc.Id).Count()
                               }
                             ).GroupBy(n => new { n.GroupChat.Id })
   .Select(g => g.FirstOrDefault())

   .ToList().OrderByDescending(x => x.GroupChat.CreatedOn).ToList();
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

                var user1 =
                _db.Users.Find(User.Identity.GetUserId());
                var allusers = _db.Users.Where(x => x.Role != "Patient" && x.Id != user1.Id).Select(x2 => new UsersViewModel { Id = x2.Id, Name = x2.FirstName + " " + x2.LastName + " (" + x2.Role + ")" }).ToList();
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
                ViewBag.AllUserCount = allusers.Count();
                return PartialView(results);
            }
            else
            {
                var user1 =
                  _db.Users.Find(User.Identity.GetUserId());
                var results = (from gc in _db.GroupsChats.AsNoTracking()
                                   //join pe in __db.PatientMeidcareMedicaidEligibilities on p.Id equals pe.PatientId into ps
                                   //from pe in ps.DefaultIfEmpty()
                               join gcp1 in _db.GroupChatsParticipants.AsNoTracking() on gc.Id equals gcp1.GroupChatId into gcp1

                               from gcp in gcp1.DefaultIfEmpty()
                               where gc.CreatedBy == user1.Id || gcp.UserId == user1.Id
                               select new GroupchatsViewModel
                               {
                                   GroupChat = gc,
                                   usersViewModels = _db.Users.Where(x1 => gcp1.Where(x => x.GroupChatId == gc.Id).Select(x => x.UserId).ToList().Contains(x1.Id)).Select(x2 => new UsersViewModel { Id = x2.Id, Name = x2.FirstName + " " + x2.LastName + " (" + x2.Role + ")" }).ToList(),
                                   TotalCount = _db.GroupChatsDetails.Where(x1 => x1.GroupChatId == gc.Id).Count()
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
                ViewBag.AllUserCount = allusers.Count();
                return PartialView(results);
            }
        }

        public ActionResult GroupChat()
        {
            return PartialView("GroupChat");
        }
        // POST: Profile/AddFriend
        //[HttpGet]
        //public ActionResult SendPrivateMessage(string from,string SenderName,string to,string ReceiverName,string message)
        //{
        //    PrivateChat pchat = new PrivateChat();
        //    pchat.From = from;
        //    pchat.SenderName = SenderName;
        //    pchat.To = to;
        //    pchat.ReceiverName = ReceiverName;
        //    pchat.Message = message;
        //    pchat.Read = false;
        //    pchat.DateSent = DateTime.Now;
        //    // Init _db

        //    _db.privateChats.Add(pchat);
        //    _db.SaveChanges();
        //    return Json("1", JsonRequestBehavior.AllowGet);
        //}
        [HttpPost]
        public ActionResult SendPrivateMessage(PrivateMessageViewModel pvm, FormCollection fc)
        {
            try
            {
                HttpPostedFileBase file = pvm.file;
                PrivateChat pchat = new PrivateChat();

                var filename = pvm.filename;
                if (!string.IsNullOrWhiteSpace(filename))
                {
                    string url = pvm.filepath;
                    string path = Request.MapPath(url);
                    file.SaveAs(path);
                }




                pchat.Attachment = pvm.Attachment == true ? true : false;
                pchat.FilePath = pvm.filepath;

                pchat.From = pvm.from;
                pchat.SenderName = pvm.SenderName;
                pchat.To = pvm.to;
                pchat.ReceiverName = pvm.ReceiverName;
                pchat.Message = pvm.message;
                pchat.Read = false;
                pchat.DateSent = DateTime.Now;

                _db.privateChats.Add(pchat);
                _db.SaveChanges();
                var id = Convert.ToString(pchat.id);
                return Json(id, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }
        public JsonResult AddnewChatmessage(GroupChatViewModel gvm, FormCollection GroupForm)
        {
            var chatstatus = _db.GroupsChats.AsNoTracking().Where(x => x.Id == gvm.GID).FirstOrDefault().GroupStatus;
            var id = 0;

            try
            {
                if (chatstatus == "Open")
                {
                    HttpPostedFileBase groupfile = gvm.file;
                    if (gvm.filename != "null" && gvm.filename != "")
                    {
                        string url = gvm.filepath;
                        string path = Request.MapPath(url);
                        gvm.file.SaveAs(path);
                    }
                    GroupChatsDetails groupChatDetails = new GroupChatsDetails
                    {
                        CreatedBy = User.Identity.GetUserId(),
                        CreatedOn = DateTime.Now,
                        GroupChatId = gvm.GID,
                        Message = gvm.Message,
                    };
                    var userid = User.Identity.GetUserId();
                    var chatrecepitatns = _db.GroupChatsParticipants.Where(x => x.GroupChatId == gvm.GID && x.UserId != userid).Select(x => x.UserId).ToList();
                    _db.GroupChatsDetails.Add(groupChatDetails);
                    _db.SaveChanges();

                    foreach (var item in chatrecepitatns)
                    {
                        GroupChatMessages gcnm = new GroupChatMessages
                        {
                            GroupChatDetailsId = gvm.GID,
                            isNew = true,
                            MessageId = groupChatDetails.Id,
                            ReceivedBy = item,
                            ReceivedOn = DateTime.Now,
                            Attachment = gvm.Attachment,
                            FilePath = gvm.filepath
                        };
                        _db.GroupChatMessages.Add(gcnm);
                        _db.SaveChanges();
                        id = gcnm.Id;
                    }
                }
                return Json(id, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }

        public async Task<PartialViewResult> Details(int id = 0, int LastID = 0)
        {
            var user1 = _db.Users.Find(User.Identity.GetUserId()).Id;
            if (1 == 2)
            {
                var results = await (from gcd in _db.GroupChatsDetails.AsNoTracking()
                                     join users in _db.Users.AsNoTracking() on gcd.CreatedBy equals users.Id
                                     where gcd.GroupChatId == id && gcd.Id > LastID
                                     select new GroupchatsdetailsViewModel
                                     {

                                         Message = gcd.Message,
                                         sender = gcd.CreatedBy != user1 ? users.FirstName + " " + users.LastName + " (" +  users.Role + ")" : "You",
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
                var groupchatdetails = _db.GroupChatMessages.Where(x => x.GroupChatDetailsId == id && x.isNew == true && x.ReceivedBy == user1).ToList();
                foreach (var item in groupchatdetails)
                {
                    item.isNew = false;
                    _db.Entry(item).State = EntityState.Modified;
                }
                if (groupchatdetails.Count > 0)
                {
                    _db.SaveChanges();
                }
                var newgroupParticipents = _db.GroupChatsParticipants.Where(x => x.GroupChatId == id && x.UserId == user1 && x.Notify == true).ToList();
                foreach (var item in newgroupParticipents)
                {
                    item.Notify = false;
                    _db.Entry(item).State = EntityState.Modified;
                }
                if (newgroupParticipents.Count > 0)
                    _db.SaveChanges();
              
               

                var results = await (from gcd in _db.GroupChatsDetails.AsNoTracking()
                                     join users in _db.Users.AsNoTracking() on gcd.CreatedBy equals users.Id
                                     join gcm in _db.GroupChatMessages on gcd.Id equals gcm.MessageId
                                     where gcd.GroupChatId == id
                                     select new GroupchatsdetailsViewModel
                                     {    
                                           
                                         Message = gcd.Message,
                                         sender = gcd.CreatedBy != user1 ? users.FirstName + " " + users.LastName + "(" + users.Role + ")": "You",
                                         timesent = gcd.CreatedOn.Value,
                                         senderclass = gcd.CreatedBy == user1 ? "Sender" : "Receiver",
                                         LastID = gcd.Id,
                                         userid = gcd.CreatedBy,
                                         attachment = gcm.Attachment,
                                         filePath = gcm.FilePath,
                                         msgid = gcm.Id,

                                     }
                                       ).GroupBy(x => x.LastID).Select(x => x.FirstOrDefault()).ToListAsync();




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
                    }
                    else
                    {
                        user.sender = user.sender.Replace("(Sales)", "(Enroller)");
                    }
                    
                }
                return PartialView(results);
            }

        }

        // POST: Profile/DisplayUnreadMessages
        [HttpPost]
        public JsonResult DisplayUnreadMessages(string userId)
        {
            // Init _db
            // Create a list of unread messages
            List<PrivateChat> list = _db.privateChats.Where(x => x.To == userId && x.Read == false).ToList();
            foreach (var item in list)
            {
                item.Read = true;
                _db.Entry(item).State = EntityState.Modified;
            }


            //_db.privateChats.Where(x => x.To == userId && x.Read == false).ToList().ForEach(x => x.Read = true);
            //_db.privateChats.Where(x => x.To == userId && x.Read == false).ToList().ForEach(a =>
            //{
            //    a.Read = true;
            //}); 
            _db.SaveChanges();
            // Return json
            return Json(list);
        }


        public JsonResult DisplayAllMessage(string senderId, string recevierId)
        {
            List<PrivateChat> list = _db.privateChats.Where(x => (x.From == senderId && x.To == recevierId) || (x.From == recevierId && x.To == senderId)).OrderBy(x => x.DateSent).ToList();
            List<PrivateChat> UnreadChat = list.Where(x => x.To == senderId && x.Read == false).ToList();
            int count = _db.privateChats.Where(x => (x.From == senderId && x.To == recevierId) || (x.From == recevierId && x.To == senderId)).Count();
            foreach (var item in UnreadChat)
            {
                item.Read = true;
                _db.Entry(item).State = EntityState.Modified;
            }
            _db.SaveChanges();
            var objectofcountandlist = new { list, count };
            //foreach (var item in list)
            //{
            //}
            return Json(objectofcountandlist);
        }

        public ActionResult DownloadDoc(int? id)
        {
            var messageObj = _db.privateChats.Where(x => x.id == id).FirstOrDefault();
            if (messageObj != null)
            {

                var absolutePath = Request.MapPath(messageObj.FilePath);
                if (absolutePath != null)
                {
                    byte[] fileBytes = System.IO.File.ReadAllBytes(absolutePath);
                    string fileName = Path.GetFileName(absolutePath);
                    return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
                }
            }
            return null;
        }
        public ActionResult DwonloadGroupdoc(int? id)
        {
            var messageObj = _db.GroupChatMessages.Where(x => x.Id == id).FirstOrDefault();
            if (messageObj != null)
            {

                var absolutePath = Request.MapPath(messageObj.FilePath);
                if (absolutePath != null)
                {
                    byte[] fileBytes = System.IO.File.ReadAllBytes(absolutePath);
                    string fileName = Path.GetFileName(absolutePath);
                    return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
                }
            }
            return null;
        }
        ////////////////////////////////////////////////////////////////////////////////////////
        ///
        [HttpPost]

        public JsonResult GetChatParticipent(int id)
        {
            var chat = _db.GroupsChats.Where(x => x.Id == id).FirstOrDefault().CreatedBy;
            if (User.Identity.GetUserId() == chat || User.IsInRole("Admin"))
            {
                var results = _db.GroupChatsParticipants.Where(x => x.GroupChatId == id).Select(x => x.UserId).ToList();
                return Json(results);
            }
            else
            {
                return Json(false);
            }

        }
        [HttpPost]
        public JsonResult AddnewChat(string chatName, string[] Participents, int chatId, string status = "", bool isTicket = false, string TicketTitle = "", string Department = "")
        {
            GroupChats groupChatObj = new GroupChats();
            try
            {
                // string[] groupchatparticpents = Participents.Split(',');

                if (chatId > 0)
                {
                    var groupChat = _db.GroupsChats.Where(x => x.Id == chatId).FirstOrDefault();
                    if (groupChat.CreatedBy != User.Identity.GetUserId())
                    {
                        return Json("false", JsonRequestBehavior.AllowGet);
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
                        var alreadyparticipents = _db.GroupChatsParticipants.Where(x => x.GroupChatId == chatId).ToList();
                        _db.GroupChatsParticipants.RemoveRange(alreadyparticipents);
                        _db.SaveChanges();
                        foreach (var item in Participents)
                        {
                            GroupChatsParticipants groupChatParticipants = new GroupChatsParticipants
                            {
                                GroupChatId = groupChat.Id,
                                UserId = item,
                                isCreater = false,
                                Notify = true

                            };
                            _db.GroupChatsParticipants.Add(groupChatParticipants);

                        }
                        GroupChatsParticipants groupChatParticipants1 = new GroupChatsParticipants
                        {
                            GroupChatId = groupChat.Id,
                            UserId = User.Identity.GetUserId(),
                            isCreater = true,
                            Notify = false
                        };
                        _db.GroupChatsParticipants.Add(groupChatParticipants1);
                        _db.SaveChanges();
                    }
                }
                else
                {


                    groupChatObj.ChatName = chatName;
                    groupChatObj.GroupStatus = "Open";
                    groupChatObj.CreatedBy = User.Identity.GetUserId();
                    groupChatObj.CreatedOn = DateTime.Now;


                    _db.GroupsChats.Add(groupChatObj);
                    _db.SaveChanges();
                    if (isTicket == true)
                    {
                        var groupchat = _db.groupChats.Where(x => x.Id == groupChatObj.Id).FirstOrDefault();
                        groupchat.isTicket = true;
                        groupchat.Department = Department;
                        groupchat.TicketTitle = TicketTitle;
                        groupchat.TicketNum = "T-" + DateTime.Now.ToString("MMddyyyyhhmm") + "-" + groupChatObj.Id.ToString();
                        _db.Entry(groupChatObj).State = EntityState.Modified;
                        _db.SaveChanges();

                    }
                    if (Participents.Count() > 0)
                    {
                        foreach (var item in Participents)
                        {
                            GroupChatsParticipants groupChatParticipants = new GroupChatsParticipants
                            {
                                GroupChatId = groupChatObj.Id,
                                UserId = item,
                                isCreater = false,
                                Notify = true
                            };
                            _db.GroupChatsParticipants.Add(groupChatParticipants);

                        }
                        GroupChatsParticipants groupChatParticipants1 = new GroupChatsParticipants
                        {
                            GroupChatId = groupChatObj.Id,
                            UserId = User.Identity.GetUserId(),
                            isCreater = true,
                            Notify = false
                        };
                        _db.GroupChatsParticipants.Add(groupChatParticipants1);
                        _db.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {

                return Json(ex.Message, JsonRequestBehavior.AllowGet); ;
            }
            return Json(groupChatObj.Id, JsonRequestBehavior.AllowGet);
        }
        /// GET ALL UNRead Notificaitons
        public JsonResult GetNotificationsDetails(string id)
        {
            List<GroupNotifylistViewModel> notifylist = new List<GroupNotifylistViewModel>();
            // Get fr count
            var Participantsuser = _db.GroupChatsParticipants.Where(x => x.UserId == id).Select(x => x.GroupChatId).ToList();
            for (int i = 0; i < Participantsuser.Count; i++)
            {
                var participant = id; var group = Participantsuser[i];
                var UnReadPrivateMsg = _db.privateChats.Count(y => y.Read == false && y.To == participant.ToString());
                var UnreadGroupMsg = _db.GroupChatMessages.Count(x => x.isNew == true && x.ReceivedBy == participant);
                var totalUnreadMsg = Convert.ToInt32(UnreadGroupMsg) + Convert.ToInt32(UnReadPrivateMsg);
                var Unreadgroup = _db.GroupChatMessages.Where(x => x.GroupChatDetailsId == group && x.isNew == true && x.ReceivedBy == participant).ToList();
                notifylist.Add(new GroupNotifylistViewModel
                {
                    groupid = group.ToString(),
                    user = participant,
                    unreadAllMsg = totalUnreadMsg,
                    UnreadGroupMsg = Unreadgroup.Count
                });
            }


            return Json(notifylist);
        }

        public JsonResult GetPrivateChatDetails(string id)
        {
            List<NotifyList> notifylist = new List<NotifyList>();
            var singleChat = (from c in _db.privateChats
                              where c.To == id
                              group c by new { c.From, c.Read } into grouping
                              select new
                              {
                                  UserId = grouping.Key.From,
                                  Read = grouping.Key.Read,
                                  Count = grouping.Count()
                              }).ToList();
            var chatlist = singleChat.Where(x => x.Read == false).ToList();
            for (int i = 0; i < chatlist.Count; i++)
            {
                notifylist.Add(new NotifyList
                {
                    user = chatlist[i].UserId,
                    total = chatlist[i].Count
                });
            }

            return Json(notifylist);
        }

        public JsonResult GetNewGroupList(string id)
        {
            List<GroupNewParticipents> participents = new List<GroupNewParticipents>();
            // Get fr count
            var Participantsuser = _db.GroupChatsParticipants.Where(x => x.UserId == id && x.Notify == true).ToList();
            int newgroupcount = 0;
            for (int i = 0; i < Participantsuser.Count; i++)
            {
                var groupNew = Participantsuser[i].GroupChatId;
                newgroupcount += 1;
                participents.Add(new GroupNewParticipents
                {
                    //User Represented GroupID
                    user = groupNew.ToString(),
                    totalgroup = newgroupcount
                });
            }

            return Json(participents);
            //var clients = Clients.Others;
            //// Call js function
            //clients.NewgroupNotify(participents);

        }

        // Ticket Module

        public ActionResult AdminTicketGeneration()
        {
            TicketGeneration obj = new TicketGeneration();
            ViewBag.ticketSubjectList = _db.TicketGeneration.Where(x => x.isDeleted == null).ToList();
            return View();
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<string> AdminTicketGeneration(TicketGeneration obj)
        {
            try
            {
                if (obj != null)
                {
                    if (obj.Id > 0)
                    {
                        obj.updateBy = User.Identity.GetUserId();
                        obj.updatedDate = DateTime.Now;
                        _db.Entry(obj).State = EntityState.Modified;
                        await _db.SaveChangesAsync();
                        return "Edit";
                    }
                    else
                    {
                        obj.createdBy = User.Identity.GetUserId();
                        obj.createdDate = DateTime.Now;
                        _db.TicketGeneration.Add(obj);
                        await _db.SaveChangesAsync();
                        return "True";
                    }
                }
                else
                {
                    var errorList = ModelState.Values.SelectMany(m => m.Errors)
                                     .Select(e => e.ErrorMessage)
                                     .ToList();
                    var errorstr = string.Join(",", errorList);
                    return errorstr;
                }

                //return RedirectToAction("AdminTicketGeneration");
            }
            catch (Exception ex)
            {

                return ex.Message;
            }
            return "";
            //return View(obj);
        }

        [HttpGet]
        public ActionResult DeleteAdminTicket(int id)
        {
            var obj = _db.TicketGeneration.Where(x => x.Id == id).FirstOrDefault();
            if (obj != null)
            {
                obj.deletedBy = User.Identity.GetUserId();
                obj.deletedDate = DateTime.Now;
                obj.isDeleted = true;
                _db.Entry(obj).State = EntityState.Modified;
                _db.SaveChangesAsync();

            }
            return RedirectToAction("AdminTicketGeneration");
        }

        public ActionResult _AdminTicketGenerationPartialView(int? id)
        {
            try
            {


                if (id > 0)
                {
                    var adminObj = _db.TicketGeneration.Where(x => x.Id == id).FirstOrDefault();
                    return PartialView("~/Views/Chats/_AdminTicketGeneration.cshtml", adminObj);
                }
                return PartialView("~/Views/Chats/_AdminTicketGeneration.cshtml");

            }
            catch (Exception ex)
            {
                return PartialView("~/Views/Chats/_AdminTicketGeneration.cshtml");


            }
        }

        public ActionResult autosuggestion()
        {
            var subjectNames = _db.TicketGeneration.ToList().Select(x => x.subjectName);
            return Json(subjectNames, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult getPriorityTime(int id)
        {
            var obj = _db.Priority.Where(x => x.Id == id).FirstOrDefault();
            if (obj != null)
            {
                return Json(obj.priorityMinute, JsonRequestBehavior.AllowGet);
            }
            return Json("", JsonRequestBehavior.AllowGet);
        }

        public ActionResult isSubjectExist(string value)
        {
            if (_db.TicketGeneration.Where(x => x.subjectName == value).FirstOrDefault() != null)
            {
                return Json("yes", JsonRequestBehavior.AllowGet);
            }
            return Json("no", JsonRequestBehavior.AllowGet);
        }


        public ActionResult UserTicketGeneration()
        {
            return View();
        }
        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<string> UserTicketGeneration(UserTicketGeneration obj, string uploadFiles, FormCollection fc)
        {
            try
            {
                if (obj != null && ModelState.IsValid)
                {
                    obj.createdBy = User.Identity.GetUserId();
                    obj.createdDate = DateTime.Now;
                    obj.createdWeekDay = System.DateTime.Now.DayOfWeek.ToString();
                    obj.notify = false;
                    if (obj.ticketHonorId != null)
                    {
                        obj.isTicketHonor = true;
                        obj.createdBy = obj.ticketHonorId;
                    }
                    _db.UserTicketGeneration.Add(obj);
                    await _db.SaveChangesAsync();

                    var History = new TicketNotificationHistory();
                    History.createdBy = obj.createdBy;
                    History.createdDate = obj.createdDate;
                    History.createdWeekDay = obj.createdWeekDay;
                    History.creatorNotes = obj.creatorNotes;
                    History.isTicketHonor = obj.isTicketHonor;
                    History.notify = obj.notify;
                    History.Patient = obj.Patient;
                    History.PatientId = obj.PatientId;
                    History.Priority = obj.Priority;
                    History.PriorityId = obj.PriorityId;
                    History.Status = obj.Status;
                    History.StatusId = obj.StatusId;
                    History.TicketGeneration = obj.TicketGeneration;
                    History.UserTicketGenerationId = obj.Id;
                    History.ticketHonorId = obj.ticketHonorId;
                    History.Type = obj.Type;
                    History.TypeId = obj.TypeId;
                    History.UserId = obj.UserId;
                    History.TicketGenerationId = obj.TicketGenerationId;
                    History.ChangingType = "Create";

                    _db.TicketNotificationHistory.Add(History);
                    _db.SaveChanges();
                    //////////************************************************For Images Saving*************************////////////////
                    string[] arr = null;
                    if (!string.IsNullOrEmpty(uploadFiles))
                        arr = uploadFiles.Split('|');
                    if (arr != null)
                    {
                        Patient_Images laboratoryResult = new Patient_Images();
                        for (int i = 0; i <= arr.Length - 1; i++)
                        {
                            string fileName = Guid.NewGuid() + "_UDT_" + DateTime.Now.ToString("MMddyyyy") + "_" + obj.Id;

                            string filepath = HelperExtensions.fileDirectory() + fileName;
                            string base64StringData = arr[i];
                            string cleandata = "";
                            string mimtype = "";
                            if (base64StringData.Contains("data:image/jpeg") || base64StringData.Contains("data:image/jpg"))
                            {
                                cleandata = base64StringData.Replace("data:image/jpeg;base64,", "");
                                mimtype = "image/jpeg";
                            }
                            else if (base64StringData.Contains("data:image/png"))
                            {
                                cleandata = base64StringData.Replace("data:image/png;base64,", "");
                                mimtype = "image/png";
                            }
                            else
                                cleandata = base64StringData;

                            TicketAttachment t_Obj = new TicketAttachment();
                            t_Obj.userTicketId = Convert.ToString(obj.Id);
                            t_Obj.fileName = fileName;
                            t_Obj.filePath = HelperExtensions.Encrypt(filepath);
                            t_Obj.createdBy = User.Identity.GetUserId();
                            t_Obj.createdDate = DateTime.Now;


                            _db.TicketAttachment.Add(t_Obj);
                            byte[] data = System.Convert.FromBase64String(cleandata);

                            System.IO.File.WriteAllBytes(filepath, data);

                        }
                        await _db.SaveChangesAsync();
                    }



                    return "True";
                }
                else
                {
                    var errorList = ModelState.Values.SelectMany(m => m.Errors)
                                     .Select(e => e.ErrorMessage)
                                     .ToList();
                    var errorstr = string.Join(",", errorList);
                    return errorstr;
                }

            }
            catch (Exception ex)
            {

                TempData.Remove("AlertMessage");
                TempData.Add("AlertMessage", new AlertModel(ex.Message + "---------" + ex.StackTrace, AlertType.Error));
                AlertModel ar = (AlertModel)TempData["AlertMessage"];
                Session.Add("Alert", ar);
                HelperExtensions.LogError(User.Identity.GetUserName(), User.Identity.GetUserId(), ex.Message, ex.StackTrace);
                return ex.Message;
            }
            return "";
        }
        [HttpPost]
        public string SubmitCommentTicket(TicketComment obj)
        {
            var UserId = "";
            if (!String.IsNullOrEmpty(obj.noteText))
            {
             

                //var UserName = HelperExtensions.GetUserNamebyID(User.Identity.GetUserId());
                obj.createdBy = Convert.ToString(User.Identity.GetUserId());
                obj.createdDate = DateTime.Now;
                obj.UserTicketGenerationId = obj.UserTicketGenerationId;
                _db.TicketComment.Add(obj);
                _db.SaveChanges();
                var ticket = _db.UserTicketGeneration.Where(x => x.Id == obj.UserTicketGenerationId).FirstOrDefault();
                UserId = ticket.UserId;
                var UserTicket = _db.UserTicketGeneration.Where(x => x.Id == obj.UserTicketGenerationId).FirstOrDefault();
                var History = new TicketNotificationHistory();
                History.createdBy = UserTicket.createdBy;
                History.createdDate = DateTime.Now;
                History.createdWeekDay =DateTime.Now.DayOfWeek.ToString();
                History.creatorNotes = UserTicket.creatorNotes;
                History.isTicketHonor = UserTicket.isTicketHonor;
                History.notify = false;
                History.Patient = UserTicket.Patient;
                History.PatientId = UserTicket.PatientId;
                History.Priority = UserTicket.Priority;
                History.PriorityId = UserTicket.PriorityId;
                History.Status = UserTicket.Status;
                History.StatusId = UserTicket.StatusId;
                History.TicketGeneration = UserTicket.TicketGeneration;
                History.UserTicketGenerationId = UserTicket.Id;
                History.ticketHonorId = UserTicket.ticketHonorId;
                History.Type = UserTicket.Type;
                History.TypeId = UserTicket.TypeId;
                History.UserId = UserTicket.UserId;
                History.TicketGenerationId = UserTicket.TicketGenerationId;
                History.ChangingType = "Comment";
                History.NotificationCreatedBy = User.Identity.GetUserId();
                _db.TicketNotificationHistory.Add(History);
                _db.SaveChanges();

                //UserId=user.
            }
            return UserId;
        }
        public ActionResult AllAssociatedComments(int? userId)
        {
            List<TicketComment> CommentList = _db.TicketComment.Where(x => x.UserTicketGenerationId == userId).ToList();
            if (CommentList.Count == 0)
            {
                ViewBag.UserTicketGenId = _db.UserTicketGeneration.Where(x => x.Id == userId).FirstOrDefault();
            }
            return PartialView(CommentList);
        }

        public ActionResult getPatient(string val)
        {
            string userId = User.Identity.GetUserId();
            if (userId != "")
            {
                using (_db)
                {

                    _db.Configuration.ProxyCreationEnabled = false;
                    _db.Database.CommandTimeout = 180;

                    var user = _db.Users.Find(userId);
                    var patients = _db.Patients.AsNoTracking().AsQueryable();

                    var dataView = (from p in patients.AsNoTracking()
                                    join L in _db.Liaisons.AsNoTracking() on p.TranslatorId equals L.Id into T
                                    from L1 in T.DefaultIfEmpty()
                                    select new
                                    {
                                        FirstName = p.FirstName + " " + p.LastName,
                                        p.LastName,
                                        p.Id,
                                        p.Cycle,
                                        p.BirthDate,

                                        Gender = p.Gender ?? "",
                                        PreferredLanguage = p.PreferredLanguage ?? "",
                                        AppointmentDate = p.AppointmentDate,
                                        AppointmentDateStr = p.AppointmentDate.Value == null ? "" : p.AppointmentDate.Value.ToString(),
                                        EnrollmentStatus = p.EnrollmentStatus ?? "",
                                        LiaisonId = p.LiaisonId ?? 0,
                                        liaisonFirstName = p.Liaison.FirstName ?? "",
                                        liaisonLastName = p.Liaison.LastName ?? "",
                                        liaisonassignedon = p.LiasionAssignedOn,

                                        enrolledon = p.CCMEnrolledOn,

                                        DocFirstName = p.Physician.FirstName + " " + p.Physician.LastName,
                                        DocLastName = p.Physician.LastName ?? "",
                                        enrollmentsubstatus = p.EnrollmentSubStatus ?? "",
                                        callingstatus = p.CallingStatus == null ? "" : p.CallingStatus,
                                        emrnumber = p.EMRNumber == null ? "" : p.EMRNumber,
                                        emrtype = p.EMRType == null ? "" : p.EMRType,
                                        picassochecked = p.PicassoChecked == null ? "" : p.PicassoChecked,
                                        picssodate = p.PicassoCheckedOn,
                                        capitated = p.CapitatedPatient == null ? "" : p.CapitatedPatient,
                                        capitatedfrom = p.CapitatedFrom,
                                        capitatedto = p.CapitatedTo,
                                        PhysicianId = p.PhysicianId ?? 0,
                                        p.Physician.MainPhoneNumber,
                                        note = p.EnrollmentNotes == null ? p.Notes : p.EnrollmentNotes,
                                        insuranceid = p.Insurance.PrimaryIdNumber,
                                        insurancename = p.Insurance.PrimaryName,
                                        TranslatorId = p.TranslatorId ?? 0,
                                        Translator = L1.FirstName + " " + L1.LastName,
                                        p.TranslatorAssignedOn


                                    }).ToList();

                    List<int> physicianids = new List<int>();

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
                    dataView = user.Role == "Liaison" && HelperExtensions.isTranslator(user.Id) == false
                               ? dataView.Where(p => p.LiaisonId == user.CCMid).ToList()
                               : user.Role == "Liaison" && HelperExtensions.isTranslator(user.Id) == true
                               ? dataView.Where(p => p.TranslatorId == user.CCMid).ToList()
                           : user.Role == "Physician"
                           ? dataView.Where(p => p.PhysicianId == user.CCMid).ToList()
                           : user.Role == "PhysiciansGroup" || user.Role == "Sales"
                           ? dataView.Where(p => physicianids.Contains(p.PhysicianId)).ToList() //.Value
                            : user.Role == "LiaisonGroup"
                              ? dataView.Where(p => liasionids.Contains(p.LiaisonId)).ToList()
                           : dataView.ToList();
                    var patientObj = dataView.Select(x => new Patient() { Id = x.Id, FirstName = x.FirstName, LastName = x.LastName, BirthDate = x.BirthDate });
                    var selectListItem = patientObj.Select(x => new SelectListItem()
                    {
                        Text = x.Id.ToString() + " | " + x.FirstName + " | " + x.BirthDate.ToString("dd-MM-yyyy"),
                        Value = x.Id.ToString()
                    }).ToList();

                    var jsonResultReturn = Json(selectListItem, JsonRequestBehavior.AllowGet);
                    jsonResultReturn.MaxJsonLength = int.MaxValue;
                    return jsonResultReturn;
                    //if (selectListItem != null)
                    //{
                    //    return PartialView("~/Views/Chats/_PatientDropdown.cshtml", selectListItem);
                    //}
                    //return Json("error", JsonRequestBehavior.AllowGet);
                }
            }
            return null;
        }



        public ActionResult AdminListViewTicket()

        {
            string currentuserId = User.Identity.GetUserId();
            var liaisons = _db.Liaisons.AsNoTracking().Select(p => new SelectListItem
            {
                Value = p.UserId,
                Text = p.FirstName + " " + p.LastName + (p.IsTranslator == true ? " (Translator)" : " (Counsler)")
            });

            ViewBag.LiaisonsForTicket = new SelectList(liaisons.OrderBy(l => l.Text), "Value", "Text");
            List<ListViewTicket> listView = new List<ListViewTicket>();

            List<UserTicketGeneration> abc = _db.UserTicketGeneration.ToList();
            foreach (var item in abc)
            {
                TicketGeneration ctd = _db.TicketGeneration.Find(item.TicketGenerationId);
                if (ctd != null)
                {
                    listView.Add(new ListViewTicket
                    {
                        statusName = CommonFunctions.GetStatusName(item.StatusId),
                        UserTicketId = item.Id,
                        subjectName = ctd.subjectName,
                        Priority = item.Priority,
                        createdBy = HelperExtensions.GetUserNamebyID(item.createdBy),
                        AssignTo = HelperExtensions.GetUserNamebyID(item.UserId),
                        createdDate = String.Format("{0:G}", item.createdDate.Value),

                    });
                }

            }
            return View(listView);
        }
        public ActionResult TicketsCreated()
        {
            string currentuserId = User.Identity.GetUserId();
            TicketListViewModel model = new TicketListViewModel();
            model.ticketCreated = new List<TicketsCreatedViewModel>();
            List<UserTicketGeneration> WholeTickets = _db.UserTicketGeneration.Where(x => x.ticketHonorId == currentuserId || x.createdBy == currentuserId).ToList();
            foreach (var item in WholeTickets)
            {
                TicketGeneration SprcificTickets = _db.TicketGeneration.Find(item.TicketGenerationId);
                if (SprcificTickets != null)
                {
                    TicketsCreatedViewModel s = new TicketsCreatedViewModel();
                    s.subjectName = SprcificTickets.subjectName;
                    s.Status = CommonFunctions.GetStatusName(item.StatusId);
                    s.UserTicketGeneration = item;
                    s.UserTicketGenerationId = item.Id;
                    s.CreatorName = HelperExtensions.GetUserNamebyID(item.createdBy);
                    s.CreatorId = item.createdBy;
                    s.CreatedDate = item.createdDate.Value;
                    s.ticketHonourId = HelperExtensions.GetUserNamebyID(item.ticketHonorId);
                    if (item.Type != null)
                    {
                        s.TypeName = item.Type.typeName;
                    }

                    s.TicketResolution = _db.AssigneeTicket.Where(x => x.UserTicketGenerationId == item.Id).Select(x => x.TicketResolution.resolutionName).FirstOrDefault();
                    if (item.Patient != null)
                    {
                        s.PatientName = (item.Patient.FirstName == null ? "" : item.Patient.FirstName) + " " + (item.Patient.MiddleName == null ? "" : item.Patient.MiddleName) + " " + (item.Patient.LastName == null ? "" : item.Patient.LastName);
                        s.PatientId = (int)item.PatientId;
                        s.Pstatus = item.Patient.EnrollmentStatus;
                        s.Psubstatus = item.Patient.EnrollmentSubStatus;
                    }
                    else
                    {

                        s.PatientName = "No Patient Assigned";

                        s.Pstatus = "No Patient Assigned";
                        s.Psubstatus = "No Patient Assigned";
                    }
                    model.ticketCreated.Add(s);
                }
            }
            int? TicketId = Convert.ToInt32(Session["TicketForOpenId"]);
            TicketId = TicketId == 0 ? null : TicketId;
            if (TicketId != null)
            {
                var ticket = _db.UserTicketGeneration.Where(p => p.Id == TicketId).FirstOrDefault();
                ViewBag.TicketId = ticket.Id;
                ViewBag.TicketStatus = ticket.Status.statusName;

                Session["TicketForOpenId"] = null;
            }
            return View(model);
        }

        [HttpPost]
        public PartialViewResult _TicketCreated(TicketListViewModel obj)
        {

            string currentuserId = User.Identity.GetUserId();

            obj.ticketCreated = new List<TicketsCreatedViewModel>();
            var userTicketGeneration = _db.UserTicketGeneration.Where(x => x.ticketHonorId == currentuserId || x.createdBy == currentuserId).ToList();
            List<TicketGenerationViewModel> tc = new List<TicketGenerationViewModel>();
            foreach (var item in userTicketGeneration)
            {
                TicketGenerationViewModel t = new TicketGenerationViewModel();
                t.Id = item.Id;
                t.PriorityId = item.PriorityId;
                t.StatusId = item.StatusId;
                t.createdDate = item.createdDate;
                t.Priority = item.Priority;
                t.Type = item.Type;
                t.TicketGenerationId = item.TicketGenerationId;
                t.UserId = item.UserId;
                t.createdBy = item.createdBy;
                t.TypeId = item.TypeId;
                var AssigneeTicket = _db.AssigneeTicket.Where(p => p.UserTicketGenerationId == item.Id).FirstOrDefault();
                if (AssigneeTicket != null && AssigneeTicket.TicketResolution != null)
                {
                    t.TicketResolutionid = AssigneeTicket.TicketResolution.id;
                    t.TicketResolution = AssigneeTicket.TicketResolution.resolutionName;
                }
                tc.Add(t);

            }


            if (tc.Count() > 0)
            {
                List<TicketGenerationViewModel> ticket = new List<TicketGenerationViewModel>();
                if (obj.startDate != null && obj.enddate != null)
                {
                    ticket = tc.Where(x =>
                    x.PriorityId == Convert.ToInt32(obj.Priority) ||
                    x.StatusId == Convert.ToInt32(obj.status) ||
                       (x.createdDate.Value.Date >= obj.startDate.Value.Date &&
                   x.createdDate.Value.Date <= obj.enddate.Value.Date) ||
                    x.TicketGenerationId == Convert.ToInt32(obj.subjectName) ||
                    x.TypeId == Convert.ToInt32(obj.tickettype) ||
                     x.TicketResolutionid == obj.TicketResolutionId
                    ).AsEnumerable().ToList();
                }
                if (obj.startDate != null && obj.enddate == null)
                {
                    ticket = tc.Where(x =>
                     x.PriorityId == Convert.ToInt32(obj.Priority) ||
                     x.StatusId == Convert.ToInt32(obj.status) ||
                        x.createdDate.Value.Date >= obj.startDate.Value.Date ||
                     x.TicketGenerationId == Convert.ToInt32(obj.subjectName) ||
                     x.TypeId == Convert.ToInt32(obj.tickettype) ||
                      x.TicketResolutionid == obj.TicketResolutionId
                     ).AsEnumerable().ToList();
                   
                
                }
                if (obj.startDate== null && obj.enddate != null)
                {
                    ticket = tc.Where(x =>
                    x.PriorityId == Convert.ToInt32(obj.Priority) ||
                    x.StatusId == Convert.ToInt32(obj.status) ||
                      
                   x.createdDate.Value.Date <= obj.enddate.Value.Date||
                    x.TicketGenerationId == Convert.ToInt32(obj.subjectName) ||
                    x.TypeId == Convert.ToInt32(obj.tickettype) ||
                     x.TicketResolutionid == obj.TicketResolutionId
                    ).AsEnumerable().ToList();
                }
                if (obj.startDate == null && obj.enddate == null)
                {
                    ticket = tc.Where(x =>
                    x.PriorityId == Convert.ToInt32(obj.Priority) ||
                    x.StatusId == Convert.ToInt32(obj.status) ||
                     
                    x.TicketGenerationId == Convert.ToInt32(obj.subjectName) ||
                    x.TypeId == Convert.ToInt32(obj.tickettype) ||
                     x.TicketResolutionid == obj.TicketResolutionId
                    ).AsEnumerable().ToList();
                }

                if (ticket.Count() > 0)
                {
                    foreach (var item in ticket)
                    {
                        var SprcificTickets = _db.UserTicketGeneration.Find(item.Id);
                        if (SprcificTickets != null)
                        {
                            TicketsCreatedViewModel s = new TicketsCreatedViewModel();
                            s.subjectName = SprcificTickets.TicketGeneration.subjectName;
                            s.Status = CommonFunctions.GetStatusName(SprcificTickets.StatusId);
                            //s.UserTicketGeneration = item;
                            s.UserTicketGenerationId = item.Id;
                            s.CreatorName = HelperExtensions.GetUserNamebyID(item.createdBy);
                            s.CreatorId = SprcificTickets.createdBy;
                            s.CreatedDate = SprcificTickets.createdDate.Value;
                            s.ticketHonourId = HelperExtensions.GetUserNamebyID(SprcificTickets.ticketHonorId);
                            s.TypeName = SprcificTickets.Type.typeName;
                            s.UserTicketGeneration = SprcificTickets;
                            s.TicketResolution = _db.AssigneeTicket.Where(p => p.UserTicketGenerationId == item.Id).Select(p => p.TicketResolution.resolutionName).FirstOrDefault();
                            if (SprcificTickets.Patient != null)
                            {
                                s.PatientName = (SprcificTickets.Patient.FirstName == null ? "" : SprcificTickets.Patient.FirstName) + " " + (SprcificTickets.Patient.MiddleName == null ? "" : SprcificTickets.Patient.MiddleName) + " " + (SprcificTickets.Patient.LastName == null ? "" : SprcificTickets.Patient.LastName);
                                s.PatientId = (int)SprcificTickets.PatientId;
                                s.Pstatus = SprcificTickets.Patient.EnrollmentStatus;
                                s.Psubstatus = SprcificTickets.Patient.EnrollmentSubStatus;
                            }
                            else
                            {
                                s.PatientName = "No Patient Assigned";
                                s.Pstatus = "No Patient Assigned";
                                s.Psubstatus = "No Patient Assigned";
                            }
                            obj.ticketCreated.Add(s);
                        }
                    }
                }
            }


            return PartialView(obj.ticketCreated);
        }
        public ActionResult ListViewTicket( )
        {
            
            string currentuserId = User.Identity.GetUserId();
            TicketListViewModel model = new TicketListViewModel();

            model.ticketlistView = new List<ListViewTicket>();
            //List<ListViewTicket> listView = new List<ListViewTicket>();
            List<UserTicketGeneration> abc = _db.UserTicketGeneration.Where(x => x.UserId == currentuserId).ToList();
            foreach (var item in abc)
            {
                TicketGeneration ctd = _db.TicketGeneration.Find(item.TicketGenerationId);
                if (ctd != null)
                {

                    var s = new ListViewTicket();
                    s.createdDate = String.Format("{0:G}", item.createdDate.Value);
                    s.statusName = CommonFunctions.GetStatusName(item.StatusId);
                    s.UserTicketId = item.Id;

                    s.subjectName = ctd.subjectName;
                    s.Priority = item.Priority;
                    s.createdBy = HelperExtensions.GetUserNamebyID(item.createdBy);
                    if (item.Type != null)
                    {
                        s.TypeName = item.Type.typeName;
                    }
                    s.TicketResolution = _db.AssigneeTicket.Where(p => p.UserTicketGenerationId == item.Id).Select(p => p.TicketResolution.resolutionName).FirstOrDefault();
                    s.notify = item.notify;
                    if (item.Patient != null)
                    {
                        s.PatientName = (item.Patient.FirstName == null ? "" : item.Patient.FirstName) + " " + (item.Patient.MiddleName == null ? "" : item.Patient.MiddleName) + " " + (item.Patient.LastName == null ? "" : item.Patient.LastName);
                        s.PatientId = (int)item.PatientId;
                        s.Pstatus = item.Patient.EnrollmentStatus;
                        s.Psubstatus = item.Patient.EnrollmentSubStatus;
                    }
                    else
                    {
                        s.PatientName = "No Patient Assigned";

                        s.Pstatus = "No Patient Assigned";
                        s.Psubstatus = "No Patient Assigned";
                    

                    }
                    model.ticketlistView.Add(s);
                    //model.tickets.Add(new ListViewTicket
                    //{
                    //    //timestamp = DateTimeExtension.ConvertToUnixTimestamp(item.createdDate.Value),
                    //    createdDate = String.Format("{0:G}", item.createdDate.Value),  // "3/9/2008 4:05:07 PM
                    //    statusName = CommonFunctions.GetStatusName(item.StatusId),
                    //    UserTicketId = item.Id,
                    //    subjectName = ctd.subjectName,
                    //    Priority = item.Priority,
                    //    createdBy = HelperExtensions.GetUserNamebyID(item.createdBy),
                    //    TypeName = item.Type.typeName,

                    //    PatientName = item.Patient.FirstName + " " + item.Patient.MiddleName + " " + item.Patient.LastName


                    //});



                }
            }
            if (User.IsInRole("LiaisonGroup"))
            {
                var email = _db.Users.Where(x => x.Id == currentuserId).FirstOrDefault();
                var liaisonManager = _db.liaisonGroups.Where(x => x.Email == email.Email).FirstOrDefault();
                var LiaisonGroup_LiaisonMapping = _db.LiaisonGroup_Liaison_Mappings.Where(x => x.LiaisonGroupId == liaisonManager.Id).ToList();
                var newlistof = LiaisonGroup_LiaisonMapping.Select(x => x.Liaison).Select(x => new SelectListItem()
                {
                    Text = x.FirstName,
                    Value = _db.Users.Where(item => item.Email == x.Email).Select(item => item.Id).First().ToString(),
                    Selected = x.isActive
                }).ToList();
                ViewBag.ListLiasonManagersPersons = newlistof;
            }
            else if (User.IsInRole("Admin"))
            {
                var email = _db.Users.Where(x => x.Id == currentuserId).FirstOrDefault();

                var PhyscianManager = _db.PhysiciansGroup.Where(x => x.Email == email.Email).FirstOrDefault();
                //PhysicianGroup_Physician_Mapping manager = new PhysicianGroup_Physician_Mapping();
                //var physicianGroup_SalesStaff_Mappings = _db.physicianGroup_SalesStaff_Mappings.Include(p => p.SaleStaff).Include(p => p.PhysiciansGroup).ToList();
                //var physiciangroups = physicianGroup_SalesStaff_Mappings.Select(x => x.PhysiciansGroup).Where(x => x.Email == PhyscianManager.Email).FirstOrDefault();
                //var NewList = physicianGroup_SalesStaff_Mappings.Where(x => x.PhysiciansGroupId == physiciangroups.Id).ToList();
                var listof = PhyscianManager != null ? _db.physicianGroup_Physician_Mappings.Where(x => x.PhysiciansGroupId == PhyscianManager.Id).ToList() : new List<PhysicianGroup_Physician_Mapping>();


                var newlistof = listof.Select(x => x.Physician).Select(x => new SelectListItem()
                {
                    Text = x.FirstName,
                    Value = _db.Users.Where(item => item.Email == x.Email).Select(item => item.Id).First().ToString(),
                     Selected = x.isActive
                }).ToList();

                ViewBag.ListPhysicanManagerPerson = newlistof;
            }
            int? TicketId = Convert.ToInt32(Session["TicketForOpenId"]);
            TicketId = TicketId == 0 ? null : TicketId;
            if (TicketId != null)
            {
                var ticket = _db.UserTicketGeneration.Where(p => p.Id == TicketId).FirstOrDefault();
                ViewBag.TicketId = ticket.Id;
                ViewBag.TicketStatus = ticket.Status==null?"": ticket.Status.statusName;

                Session["TicketForOpenId"] = null;
            }
            return View(model);
        }
        [HttpPost]
        public PartialViewResult _TicketListView(TicketListViewModel obj)
        {

            string currentuserId = User.Identity.GetUserId();

            obj.ticketlistView = new List<ListViewTicket>();
            var userTicketGeneration = _db.UserTicketGeneration.Where(x => x.UserId == currentuserId).ToList();
            List<TicketGenerationViewModel> tc = new List<TicketGenerationViewModel>();
            foreach (var item in userTicketGeneration)
            {
                TicketGenerationViewModel t = new TicketGenerationViewModel();
                t.Id = item.Id;
                t.PriorityId = item.PriorityId;
                t.StatusId = item.StatusId;
                t.createdDate = item.createdDate;
                t.Priority = item.Priority;
                t.Type = item.Type;
                t.TicketGenerationId = item.TicketGenerationId;
                t.UserId = item.UserId;
                t.createdBy = item.createdBy;
                t.TypeId = item.TypeId;
                var AssigneeTicket = _db.AssigneeTicket.Where(p => p.UserTicketGenerationId == item.Id).FirstOrDefault();
                if (AssigneeTicket != null && AssigneeTicket.TicketResolution != null)
                {
                    t.TicketResolutionid = AssigneeTicket.TicketResolution.id;
                    t.TicketResolution = AssigneeTicket.TicketResolution.resolutionName;
                }
                tc.Add(t);

            }
            if (tc.Count() > 0)
            {
                List<TicketGenerationViewModel> ticket = new List<TicketGenerationViewModel>();
                if (obj.startDate != null && obj.enddate != null )
                {
                    ticket = tc.Where(x =>
                    x.PriorityId == Convert.ToInt32(obj.Priority) ||
                    x.StatusId == Convert.ToInt32(obj.status) ||
                       (x.createdDate.Value.Date >= obj.startDate.Value.Date &&
                   x.createdDate.Value.Date <= obj.enddate.Value.Date) ||
                    x.TicketGenerationId == Convert.ToInt32(obj.subjectName) ||
                    x.TypeId == Convert.ToInt32(obj.tickettype) ||
                     x.TicketResolutionid == obj.TicketResolutionId
                    ).AsEnumerable().ToList();
                }
                if (obj.startDate != null && obj.enddate == null)
                {
                    ticket = tc.Where(x =>
                     x.PriorityId == Convert.ToInt32(obj.Priority) ||
                     x.StatusId == Convert.ToInt32(obj.status) ||
                        x.createdDate.Value.Date >= obj.startDate.Value.Date ||
                     x.TicketGenerationId == Convert.ToInt32(obj.subjectName) ||
                     x.TypeId == Convert.ToInt32(obj.tickettype) ||
                      x.TicketResolutionid == obj.TicketResolutionId
                     ).AsEnumerable().ToList();


                }
                if (obj.startDate == null && obj.enddate != null)
                {
                    ticket = tc.Where(x =>
                    x.PriorityId == Convert.ToInt32(obj.Priority) ||
                    x.StatusId == Convert.ToInt32(obj.status) ||

                   x.createdDate.Value.Date <= obj.enddate.Value.Date ||
                    x.TicketGenerationId == Convert.ToInt32(obj.subjectName) ||
                    x.TypeId == Convert.ToInt32(obj.tickettype) ||
                     x.TicketResolutionid == obj.TicketResolutionId
                    ).AsEnumerable().ToList();
                }
                if (obj.startDate == null && obj.enddate == null)
                {
                    ticket = tc.Where(x =>
                    x.PriorityId == Convert.ToInt32(obj.Priority) ||
                    x.StatusId == Convert.ToInt32(obj.status) ||

                    x.TicketGenerationId == Convert.ToInt32(obj.subjectName) ||
                    x.TypeId == Convert.ToInt32(obj.tickettype) ||
                     x.TicketResolutionid == obj.TicketResolutionId
                    ).AsEnumerable().ToList();
                }



                if (ticket.Count() > 0)
                {
                    foreach (var item in ticket)
                    {
                        var ctd = _db.UserTicketGeneration.Find(item.Id);
                        if (ctd != null)
                        {

                            ListViewTicket s = new ListViewTicket();
                            s.timestamp = DateTimeExtension.ConvertToUnixTimestamp(ctd.createdDate.Value);
                            s.createdDate = String.Format("{0:G}", ctd.createdDate.Value);  // "3/9/2008 4:05:07 PM
                            s.statusName = CommonFunctions.GetStatusName(ctd.StatusId);
                            s.UserTicketId = item.Id;
                            s.subjectName = ctd.TicketGeneration.subjectName;
                            s.Priority = ctd.Priority;
                            s.createdBy = HelperExtensions.GetUserNamebyID(ctd.createdBy);
                            s.TypeName = ctd.Type.typeName;
                            s.TicketResolution = _db.AssigneeTicket.Where(p => p.UserTicketGenerationId == item.Id).Select(p => p.TicketResolution.resolutionName).FirstOrDefault();

                            if (ctd.Patient != null)
                            {
                                s.PatientName = (ctd.Patient.FirstName == null ? "" : ctd.Patient.FirstName) + " " + (ctd.Patient.MiddleName == null ? "" : ctd.Patient.MiddleName) + " " + (ctd.Patient.LastName == null ? "" : ctd.Patient.LastName);
                                s.PatientId = (int)ctd.PatientId;
                                s.Pstatus = ctd.Patient.EnrollmentStatus;
                                s.Psubstatus = ctd.Patient.EnrollmentSubStatus;
                            }
                            else
                            {
                                s.PatientName = "No Patient Assigned";
                                s.Pstatus = "No Patient Assigned";
                                s.Psubstatus = "No Patient Assigned";
                            }

                            obj.ticketlistView.Add(s);



                        }
                    }
                }
            }


            //if (obj.startDate != null)
            //{
            //    userTicketGeneration = userTicketGeneration.Where(x => x.createdDate.Value.Date >= obj.startDate).AsEnumerable().ToList();
            //}
            //if (obj.enddate != null)
            //{
            //    userTicketGeneration = userTicketGeneration.Where(x => x.createdDate.Value.Date <= obj.enddate).AsEnumerable().ToList();
            //}
            //if (obj.tickettype != null)
            //{
            //    var ticketTypeId = Convert.ToInt32(obj.tickettype);
            //    userTicketGeneration = userTicketGeneration.Where(x => x.TypeId == ticketTypeId).ToList();
            //}
            //if (obj.subjectName != null)
            //{
            //    var ticketSubjectDashboard = Convert.ToInt32(obj.subjectName);
            //    userTicketGeneration = userTicketGeneration.Where(x => x.TicketGenerationId == ticketSubjectDashboard).ToList();
            //}
            //if (obj.ticketassignee != null)
            //{
            //    var ticketAssignee = Convert.ToInt32(obj.ticketassignee);
            //    userTicketGeneration = userTicketGeneration.Where(x => x.UserId == obj.ticketassignee).ToList();
            //}
            //if (obj.createdBy != null)
            //{
            //    userTicketGeneration = userTicketGeneration.Where(x => x.createdBy == obj.createdBy).ToList();
            //}
            //if (obj.status != null)
            //{
            //    int? statussId = Convert.ToInt32(obj.status);
            //    userTicketGeneration = userTicketGeneration.Where(x => x.StatusId == statussId).ToList();
            //}


            //List<UserTicketGeneration> abc = _db.UserTicketGeneration.Where(x => x.UserId == currentuserId).ToList();

            if (User.IsInRole("LiaisonGroup"))
            {
                var email = _db.Users.Where(x => x.Id == currentuserId).FirstOrDefault();
                var liaisonManager = _db.liaisonGroups.Where(x => x.Email == email.Email).FirstOrDefault();
                var LiaisonGroup_LiaisonMapping = _db.LiaisonGroup_Liaison_Mappings.Where(x => x.LiaisonGroupId == liaisonManager.Id).ToList();
                var newlistof = LiaisonGroup_LiaisonMapping.Select(x => x.Liaison).Select(x => new SelectListItem()
                {
                    Text = x.FirstName,
                    Value = _db.Users.Where(item => item.Email == x.Email).Select(item => item.Id).First().ToString(),
                    Selected=x.isActive
                }).ToList();
                ViewBag.ListLiasonManagersPersons = newlistof;
            }
            else if (User.IsInRole("Admin"))
            {
                var email = _db.Users.Where(x => x.Id == currentuserId).FirstOrDefault();
                var PhyscianManager = _db.PhysiciansGroup.Where(x => x.Email == email.Email).FirstOrDefault();
                //PhysicianGroup_Physician_Mapping manager = new PhysicianGroup_Physician_Mapping();
                //var physicianGroup_SalesStaff_Mappings = _db.physicianGroup_SalesStaff_Mappings.Include(p => p.SaleStaff).Include(p => p.PhysiciansGroup).ToList();
                //var physiciangroups = physicianGroup_SalesStaff_Mappings.Select(x => x.PhysiciansGroup).Where(x => x.Email == PhyscianManager.Email).FirstOrDefault();
                //var NewList = physicianGroup_SalesStaff_Mappings.Where(x => x.PhysiciansGroupId == physiciangroups.Id).ToList();
                var listof = PhyscianManager != null ? _db.physicianGroup_Physician_Mappings.Where(x => x.PhysiciansGroupId == PhyscianManager.Id).ToList() : new List<PhysicianGroup_Physician_Mapping>();


                var newlistof = listof.Select(x => x.Physician).Select(x => new SelectListItem()
                {
                    Text = x.FirstName,
                    Value = _db.Users.Where(item => item.Email == x.Email).Select(item => item.Id).First().ToString(),
                    Selected=x.isActive
                }).ToList();

                ViewBag.ListPhysicanManagerPerson = newlistof;
            }

            return PartialView(obj.ticketlistView);
        }
        public ActionResult AssigneeTicket()
        {
            return PartialView("_AssigneeTicket");
        }

        public ActionResult clearTicketNotification(int? TicktId,string Type="")
        {
            string url = "";
            Session["TicketForOpenId"] = TicktId;
            if (TicktId != null)
            {
                var currentTicket = _db.UserTicketGeneration.Where(x => x.Id == TicktId).FirstOrDefault();
                string CurrentUserId = User.Identity.GetUserId();

                if (currentTicket.UserId == CurrentUserId)
                {
                    url = "listviewticket";

                }
                else if(currentTicket.createdBy==CurrentUserId){
                    url = "TicketsCreated";
                }
                else if(currentTicket.ticketHonorId==CurrentUserId){
                    url = "TicketsCreated";
                }


            }



            try
            {
                string userId = User.Identity.GetUserId();
                foreach (var item in _db.UserTicketGeneration.Where(x => (x.UserId == userId && x.notify == false && x.Id == TicktId)).ToList())
                {
                    item.notify = true;
                    _db.Entry(item).State = EntityState.Modified;
                }

                _db.SaveChanges();
                foreach (var item in _db.TicketNotificationHistory.Where(x => ((x.UserId == userId && x.NotificationCreatedBy != userId) || (x.createdBy == userId && x.NotificationCreatedBy != userId && x.NotificationCreatedBy != null))&& x.UserTicketGenerationId== TicktId).ToList())
                {
                    item.notify = true;
                    _db.Entry(item).State = EntityState.Modified;
                }
                _db.SaveChanges();

                if (Type == "single")
                {
                    return Json("True", JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(url, JsonRequestBehavior.AllowGet);
                }
                }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }

         

        }

        public ActionResult getTicketNotification()
        {
            var NotoficationList = new List<NotificationViewMidal>();
            string userId = User.Identity.GetUserId();
            //var uncheckTickets = _db.UserTicketGeneration.Count(x => x.UserId == userId && x.notify == false);
            var uncheckTickets = _db.TicketNotificationHistory.Where(x => ((x.UserId == userId && x.NotificationCreatedBy != userId) || (x.createdBy == userId && x.NotificationCreatedBy != userId && x.NotificationCreatedBy != null)|| (x.ticketHonorId == userId && x.NotificationCreatedBy != userId && x.NotificationCreatedBy != null)) && x.notify == false).ToList();
            var res = uncheckTickets.Count > 0 ? Convert.ToString(uncheckTickets.Count) : "";
            //foreach (var item in uncheckTickets)
            //{
            //    var Notification = new NotificationViewMidal();
            //    Notification.AssigneeName = HelperExtensions.GetUserNamebyID(item.createdBy);
            //    DateTime time = Convert.ToDateTime(item.createdDate);
            //    var  totalTime =  DateTime.Now.Subtract(time).TotalMinutes;
            //   int timeToCheck= Convert.ToInt32(totalTime);
            //    if (timeToCheck <= 59)
            //    {
            //        Notification.Time = timeToCheck.ToString() + " min ago";
            //    }else if (timeToCheck>=60 && timeToCheck <= 1439)
            //    {

            //        decimal hours = timeToCheck / 60;
            //        hours = Math.Floor(hours);
            //        Notification.Time = hours.ToString() + " hr ago";
            //    }
            //    else if (timeToCheck >= 1440 && timeToCheck<= 43799)
            //    {
            //        decimal days = timeToCheck / 1440;
            //        days = Math.Floor(days);
            //        Notification.Time = days.ToString() + " day ago";
            //    }
            //    else if(timeToCheck >= 43800 && timeToCheck <= 525599)
            //    {
            //        decimal months = timeToCheck / 43800;
            //        months = Math.Floor(months);
            //        Notification.Time = months.ToString() + " Month ago";
            //    }
            //    else
            //    {
            //        decimal year = timeToCheck / 525600;
            //        year = Math.Floor(year);
            //        Notification.Time = year.ToString() + " year ago";
            //    }
          
            //    Notification.patientId = item.PatientId;
            //    Notification.Priority = item.Priority==null?"": item.Priority.priorityLevel;
            //    Notification.TicketType = item.Type==null?"": item.Type.typeName;
            //    Notification.CreatorNotes = item.creatorNotes;
            //    Notification.TicketId = item.Id;
            //    Notification.Status = item.Status.statusName;


            //    NotoficationList.Add(Notification);
            //}
            //NotoficationList = NotoficationList.OrderByDescending(x => x.TicketId).ToList();
            return Json(res, JsonRequestBehavior.AllowGet);
        }
        public string getPatientNameById(int id)
        {

            try
            {
                using (_db)
                {
                    return _db.Patients.Where(x => x.Id == id).Select(x => x.FirstName).FirstOrDefault();
                }
            }
            catch (Exception ex)
            {

                return null;
            }
        }
        public ActionResult getAdminAssignee(int userTicketId, string Ticket_status)
        {
            var status = _db.Status.Where(x => x.statusName.Equals(Ticket_status, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();
            AssigneeTicketViewModel obj = new AssigneeTicketViewModel();
            obj.TicketAttachmentList = new List<TicketAttachment>();

            var userticketgenerationObj = _db.UserTicketGeneration.Where(x => x.Id == userTicketId).FirstOrDefault();
            if (userticketgenerationObj != null)
            {
                var assigneeObj = _db.AssigneeTicket.Where(x => x.UserTicketGenerationId == userTicketId).FirstOrDefault();
                if (assigneeObj != null)
                {
                    obj.id = assigneeObj.id;
                }
                obj.UserTicketGenerationId = userticketgenerationObj.Id;
                obj.ticketSubject = userticketgenerationObj.TicketGeneration?.subjectName;
                obj.ticketType = userticketgenerationObj.Type?.typeName;
                obj.ticketPriority = userticketgenerationObj.Priority?.priorityLevel;
                obj.ticketTat = userticketgenerationObj?.Priority != null ? Convert.ToString(userticketgenerationObj?.Priority?.priorityMinute) : "";
                obj.CreatorNotes = userticketgenerationObj.creatorNotes;
                obj.patientId = userticketgenerationObj.PatientId != null ? userticketgenerationObj.PatientId.Value : 0;
                obj.StatusId = status.Id;
                obj.ticketStatus = Ticket_status;

                var ticketresid = _db.AssigneeTicket.Where(x => x.UserTicketGenerationId == userticketgenerationObj.Id).Select(x => x.TicketResolution).FirstOrDefault();

                if (ticketresid != null)
                {

                    obj.TicketResolutionId = _db.AssigneeTicket.Where(x => x.UserTicketGenerationId == userticketgenerationObj.Id).Select(x => x.TicketResolution.id).FirstOrDefault();

                }




                // update user ticket status and Assignee Ticket
                //obj.id = createDemoAssigneeTicket(obj, userTicketId, status, userticketgenerationObj);
            }
            var utidInString = Convert.ToString(userTicketId);

            // Get Linked Attachment 
            var list = _db.TicketAttachment.Where(x => x.userTicketId == utidInString).ToList();

            // convert files in it origanl path
            foreach (var item in list)
            {
                TicketAttachment taObj = new TicketAttachment();
                string baseval = HelperExtensions.Decrypt(item.filePath);
                string files = "data:image/png;base64," + HelperExtensions.convertbase64(baseval);
                taObj.filePath = files;
                obj.TicketAttachmentList.Add(taObj);
            }

            return PartialView(obj);
        }
        public ActionResult getAssigneeTicketPartialView(int userTicketId, string Ticket_status, string edit)
        {
            try
            {
                if (Ticket_status == ETicketStatus.OPEN)
                {
                    if (userTicketId != null)
                    {
                        var status = _db.Status.Where(x => x.statusName.Equals("In Progress", StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();
                        AssigneeTicketViewModel obj = new AssigneeTicketViewModel();
                        obj.TicketAttachmentList = new List<TicketAttachment>();

                        var userticketgenerationObj = _db.UserTicketGeneration.Where(x => x.Id == userTicketId).FirstOrDefault();
                        if (userticketgenerationObj != null)
                        {
                            obj.UserTicketGenerationId = userticketgenerationObj.Id;
                            obj.ticketSubject = userticketgenerationObj.TicketGeneration?.subjectName;
                            obj.ticketType = userticketgenerationObj.Type?.typeName;
                            obj.ticketPriority = userticketgenerationObj.Priority?.priorityLevel;
                            obj.ticketTat = userticketgenerationObj?.Priority != null ? Convert.ToString(userticketgenerationObj?.Priority?.priorityMinute) : "";
                            obj.CreatorNotes = userticketgenerationObj.creatorNotes;
                            obj.patientId = userticketgenerationObj.PatientId != null ? userticketgenerationObj.PatientId.Value : 0;
                            obj.StatusId = status.Id;
                            obj.ticketStatus = Ticket_status;



                            // update user ticket status and Assignee Ticket
                            obj.id = createDemoAssigneeTicket(obj, userTicketId, status, userticketgenerationObj);
                        }
                        var utidInString = Convert.ToString(userTicketId);

                        // Get Linked Attachment 
                        var list = _db.TicketAttachment.Where(x => x.userTicketId == utidInString).ToList();

                        // convert files in it origanl path
                        foreach (var item in list)
                        {
                            TicketAttachment taObj = new TicketAttachment();
                            string baseval = HelperExtensions.Decrypt(item.filePath);
                            string files = "data:image/png;base64," + HelperExtensions.convertbase64(baseval);
                            taObj.filePath = files;
                            obj.TicketAttachmentList.Add(taObj);
                        }

                        return PartialView("~/Views/Chats/_AssigneeTicket.cshtml", obj);
                    }

                }

                else if (Ticket_status == ETicketStatus.IN_PROGRESS)
                {

                    if (userTicketId != null)
                    {
                        var status = _db.Status.Where(x => x.statusName.Equals(Ticket_status, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();
                        AssigneeTicketViewModel obj = new AssigneeTicketViewModel();
                        obj.TicketAttachmentList = new List<TicketAttachment>();

                        var userticketgenerationObj = _db.UserTicketGeneration.Where(x => x.Id == userTicketId).FirstOrDefault();
                        if (userticketgenerationObj != null)
                        {
                            var assigneeObj = _db.AssigneeTicket.Where(x => x.UserTicketGenerationId == userTicketId).FirstOrDefault();

                            obj.id = assigneeObj.id;
                            obj.UserTicketGenerationId = userticketgenerationObj.Id;
                            obj.ticketSubject = userticketgenerationObj.TicketGeneration?.subjectName;
                            obj.ticketType = userticketgenerationObj.Type?.typeName;
                            obj.ticketPriority = userticketgenerationObj.Priority?.priorityLevel;
                            obj.ticketTat = userticketgenerationObj?.Priority != null ? Convert.ToString(userticketgenerationObj?.Priority?.priorityMinute) : "";
                            obj.CreatorNotes = userticketgenerationObj.creatorNotes;
                            obj.patientId = userticketgenerationObj.PatientId != null ? userticketgenerationObj.PatientId.Value : 0;
                            obj.StatusId = status.Id;
                            obj.ticketStatus = Ticket_status;



                            // update user ticket status and Assignee Ticket
                            //obj.id = createDemoAssigneeTicket(obj, userTicketId, status, userticketgenerationObj);
                        }
                        var utidInString = Convert.ToString(userTicketId);

                        // Get Linked Attachment 
                        var list = _db.TicketAttachment.Where(x => x.userTicketId == utidInString).ToList();

                        // convert files in it origanl path
                        foreach (var item in list)
                        {
                            TicketAttachment taObj = new TicketAttachment();
                            string baseval = HelperExtensions.Decrypt(item.filePath);
                            string files = "data:image/png;base64," + HelperExtensions.convertbase64(baseval);
                            taObj.filePath = files;
                            obj.TicketAttachmentList.Add(taObj);
                        }

                        return PartialView("~/Views/Chats/_AssigneeTicket.cshtml", obj);
                    }

                }

                else if (Ticket_status == ETicketStatus.PENDING)
                {
                    if (userTicketId != null)
                    {
                        var status = _db.Status.Where(x => x.statusName.Equals(Ticket_status, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();
                        AssigneeTicketViewModel obj = new AssigneeTicketViewModel();
                        obj.TicketAttachmentList = new List<TicketAttachment>();

                        var userticketgenerationObj = _db.UserTicketGeneration.Where(x => x.Id == userTicketId).FirstOrDefault();
                        if (userticketgenerationObj != null)
                        {
                            obj.UserTicketGenerationId = userticketgenerationObj.Id;
                            obj.ticketSubject = userticketgenerationObj.TicketGeneration?.subjectName;
                            obj.ticketType = userticketgenerationObj.Type?.typeName;
                            obj.ticketPriority = userticketgenerationObj.Priority?.priorityLevel;
                            obj.ticketTat = userticketgenerationObj?.Priority != null ? Convert.ToString(userticketgenerationObj?.Priority?.priorityMinute) : "";
                            obj.CreatorNotes = userticketgenerationObj.creatorNotes;
                            obj.patientId = userticketgenerationObj.PatientId != null ? userticketgenerationObj.PatientId.Value : 0;
                            obj.StatusId = status.Id;
                            obj.ticketStatus = Ticket_status;

                            // update user ticket status and Assignee Ticket
                            //obj.id = createDemoAssigneeTicket(obj, userTicketId, status, userticketgenerationObj);
                        }
                        var utidInString = Convert.ToString(userTicketId);

                        // Get Linked Attachment 
                        var list = _db.TicketAttachment.Where(x => x.userTicketId == utidInString).ToList();

                        // convert files in it origanl path
                        foreach (var item in list)
                        {
                            TicketAttachment taObj = new TicketAttachment();
                            string baseval = HelperExtensions.Decrypt(item.filePath);
                            string files = "data:image/png;base64," + HelperExtensions.convertbase64(baseval);
                            taObj.filePath = files;
                            obj.TicketAttachmentList.Add(taObj);
                        }

                        return PartialView("~/Views/Chats/_AssigneeTicket.cshtml", obj);
                    }

                }

                else if (Ticket_status == ETicketStatus.RESOLVED || Ticket_status == ETicketStatus.UNRESOLVED)
                {
                    if (userTicketId != null)
                    {
                        var status = _db.Status.Where(x => x.statusName.Equals(Ticket_status, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();
                        AssigneeTicketViewModel obj = new AssigneeTicketViewModel();
                        obj.TicketAttachmentList = new List<TicketAttachment>();

                        var userticketgenerationObj = _db.UserTicketGeneration.Where(x => x.Id == userTicketId).FirstOrDefault();
                        if (userticketgenerationObj != null)
                        {
                            var assigneeObj = _db.AssigneeTicket.Where(x => x.UserTicketGenerationId == userTicketId).FirstOrDefault();

                            obj.UserTicketGenerationId = userticketgenerationObj.Id;
                            obj.ticketSubject = userticketgenerationObj.TicketGeneration?.subjectName;
                            obj.ticketType = userticketgenerationObj.Type?.typeName;
                            obj.ticketPriority = userticketgenerationObj.Priority?.priorityLevel;
                            obj.ticketTat = userticketgenerationObj?.Priority != null ? Convert.ToString(userticketgenerationObj?.Priority?.priorityMinute) : "";
                            obj.CreatorNotes = userticketgenerationObj.creatorNotes;
                            obj.patientId = userticketgenerationObj.PatientId != null ? userticketgenerationObj.PatientId.Value : 0;
                            obj.StatusId = status.Id;
                            obj.ticketStatus = Ticket_status;
                            obj.TicketResolutionId = assigneeObj.TicketResolutionId;

                            // update user ticket status and Assignee Ticket
                            //obj.id = createDemoAssigneeTicket(obj, userTicketId, status, userticketgenerationObj);
                        }

                        var utidInString = Convert.ToString(userTicketId);
                        // Get Linked Attachment with Ticket 
                        var list = _db.TicketAttachment.Where(x => x.userTicketId == utidInString).ToList();

                        // convert files in it origanl path
                        foreach (var item in list)
                        {
                            TicketAttachment taObj = new TicketAttachment();
                            string baseval = HelperExtensions.Decrypt(item.filePath);
                            string files = "data:image/png;base64," + HelperExtensions.convertbase64(baseval);
                            taObj.filePath = files;
                            obj.TicketAttachmentList.Add(taObj);
                        }

                        return PartialView("~/Views/Chats/_AssigneeTicket.cshtml", obj);
                    }
                }

                else if (Ticket_status == ETicketStatus.JustWatching || Ticket_status == ETicketStatus.JustWatchingPending)
                {
                    if (userTicketId != null)
                    {
                        Status status = new Status();
                        if (Ticket_status == "watchPending")
                        {
                            status = _db.Status.Where(x => x.statusName.Equals("Pending", StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();
                        }
                        else
                        {
                            status = _db.Status.Where(x => x.statusName.Equals("Open", StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();
                        }
                        AssigneeTicketViewModel obj = new AssigneeTicketViewModel();
                        obj.TicketAttachmentList = new List<TicketAttachment>();

                        var userticketgenerationObj = _db.UserTicketGeneration.Where(x => x.Id == userTicketId).FirstOrDefault();
                        if (userticketgenerationObj != null)
                        {
                            //var assigneeObj = _db.AssigneeTicket.Where(x => x.UserTicketGenerationId == userTicketId).FirstOrDefault();

                            obj.UserTicketGenerationId = userticketgenerationObj.Id;
                            obj.ticketSubject = userticketgenerationObj.TicketGeneration?.subjectName;
                            obj.ticketType = userticketgenerationObj.Type?.typeName;
                            obj.ticketPriority = userticketgenerationObj.Priority?.priorityLevel;
                            obj.ticketTat = userticketgenerationObj?.Priority != null ? Convert.ToString(userticketgenerationObj?.Priority?.priorityMinute) : "";
                            obj.CreatorNotes = userticketgenerationObj.creatorNotes;
                            obj.patientId = userticketgenerationObj.PatientId != null ? userticketgenerationObj.PatientId.Value : 0;
                            obj.StatusId = status.Id;
                            obj.ticketStatus = ETicketStatus.JustWatching;
                            //obj.TicketResolutionId = assigneeObj.TicketResolutionId;

                            // update user ticket status and Assignee Ticket
                            //obj.id = createDemoAssigneeTicket(obj, userTicketId, status, userticketgenerationObj);
                        }

                        var utidInString = Convert.ToString(userTicketId);
                        // Get Linked Attachment with Ticket 
                        var list = _db.TicketAttachment.Where(x => x.userTicketId == utidInString).ToList();

                        // convert files in it origanl path
                        foreach (var item in list)
                        {
                            TicketAttachment taObj = new TicketAttachment();
                            string baseval = HelperExtensions.Decrypt(item.filePath);
                            string files = "data:image/png;base64," + HelperExtensions.convertbase64(baseval);
                            taObj.filePath = files;
                            obj.TicketAttachmentList.Add(taObj);
                        }

                        return PartialView("~/Views/Chats/_AssigneeTicket.cshtml", obj);
                    }
                }



                return Json("noticketId", JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(ex.Message + " error_error", JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult getMyTicketPartialView(int userTicketId, string Ticket_status)
        {
            try
            {
                if (userTicketId != null)
                {
                    Status status = new Status();

                    status = _db.Status.Where(x => x.statusName.Equals(Ticket_status, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();

                    AssigneeTicketViewModel obj = new AssigneeTicketViewModel();
                    obj.TicketAttachmentList = new List<TicketAttachment>();

                    var userticketgenerationObj = _db.UserTicketGeneration.Where(x => x.Id == userTicketId).FirstOrDefault();
                    if (userticketgenerationObj != null)
                    {
                        //var assigneeObj = _db.AssigneeTicket.Where(x => x.UserTicketGenerationId == userTicketId).FirstOrDefault();
                        //int resolutionid = 0;
                        //var resid=  _db.AssigneeTicket.Where(p => p.UserTicketGenerationId == obj.id).Select(p => p.TicketResolutionId).FirstOrDefault();
                        //if (resid != null)
                        //{
                        //    resolutionid = (int)resid;
                        //}

                        obj.UserTicketGenerationId = userticketgenerationObj.Id;
                        obj.ticketSubject = userticketgenerationObj.TicketGeneration?.subjectName;
                        obj.ticketType = userticketgenerationObj.Type?.typeName;
                        obj.ticketPriority = userticketgenerationObj.Priority?.priorityLevel;
                        obj.ticketTat = userticketgenerationObj?.Priority != null ? Convert.ToString(userticketgenerationObj?.Priority?.priorityMinute) : "";
                        obj.CreatorNotes = userticketgenerationObj.creatorNotes;
                        obj.patientId = userticketgenerationObj.PatientId != null ? userticketgenerationObj.PatientId.Value : 0;
                        obj.TicketResolutionId = _db.AssigneeTicket.Where(p => p.UserTicketGenerationId == userTicketId).Select(p => p.TicketResolutionId).FirstOrDefault();
                        obj.StatusId = status.Id;
                        obj.ticketStatus = Ticket_status;
                        obj.CreaterId = userticketgenerationObj.createdBy;
                        //obj.TicketResolutionId = assigneeObj.TicketResolutionId;

                        // update user ticket status and Assignee Ticket
                        //obj.id = createDemoAssigneeTicket(obj, userTicketId, status, userticketgenerationObj);
                    }

                    var utidInString = Convert.ToString(userTicketId);
                    // Get Linked Attachment with Ticket 
                    var list = _db.TicketAttachment.Where(x => x.userTicketId == utidInString).ToList();

                    // convert files in it origanl path
                    foreach (var item in list)
                    {
                        TicketAttachment taObj = new TicketAttachment();
                        string baseval = HelperExtensions.Decrypt(item.filePath);
                        string files = "data:image/png;base64," + HelperExtensions.convertbase64(baseval);
                        taObj.filePath = files;
                        obj.TicketAttachmentList.Add(taObj);
                    }

                    return PartialView("~/Views/Chats/_MyTicket.cshtml", obj);
                }

                return Json("noticketId", JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(ex.Message + " error_error", JsonRequestBehavior.AllowGet);
            }
        }


        // Function use in getAssigneeTicketPartialView (WaitTime remaining check (InProgress Wait time) )
        public int createDemoAssigneeTicket(AssigneeTicketViewModel _obj, int? userTicketId, Status status, UserTicketGeneration userticketgenerationObj)
        {
            var assigneeObj = _db.AssigneeTicket.Where(x => x.UserTicketGenerationId == userTicketId).FirstOrDefault();

            if (assigneeObj == null)
            {

                AssigneeTicket obj = new AssigneeTicket();

                obj.ticketSubject = _obj.ticketSubject;
                obj.ticketType = _obj.ticketType;
                obj.ticketPriority = _obj.ticketPriority;
                obj.ticketPriority = _obj.ticketPriority;
                obj.ticketTat = _obj.ticketTat;
                obj.CreatorNotes = _obj.CreatorNotes;
                obj.createdBy = User.Identity.GetUserId();
                obj.UserTicketGenerationId = userTicketId;
                obj.StatusId = status != null ? status.Id : 0;


                // remaining waitTime
                obj.inProgressWeekDay = DateTime.Now.DayOfWeek.ToString();
                obj.inProgressCreatedDate = DateTime.Now;


                // check on get wait time on different scenarios (Set In-Progress Wait Time)

                if ((obj.inProgressCreatedDate - userticketgenerationObj.createdDate).Value.TotalDays >= 1)
                {
                    var totalTodayMinutes = DateTimeExtension.timedifferenceInMinutes(userticketgenerationObj.createdDate.Value.Date.Add(new TimeSpan(17, 0, 0)), userticketgenerationObj.createdDate);

                    var totalDaysMinute = DateTimeExtension.TotalDaysLeftInMinutes(userticketgenerationObj.createdDate, DateTime.Now, true);

                    var totalEndDayMinute = DateTimeExtension.timedifferenceInMinutes(DateTime.Now, obj.inProgressCreatedDate.Value.Date.Add(new TimeSpan(9, 0, 0)));

                    obj.inProgressWaitTime = totalTodayMinutes + totalDaysMinute + totalEndDayMinute;

                }
                else
                {
                    obj.inProgressWaitTime = (obj.inProgressCreatedDate - userticketgenerationObj.createdDate).Value.TotalMinutes;
                }

                _db.AssigneeTicket.Add(obj);

                // Update Status in UserTicketGeneration
                userticketgenerationObj.StatusId = status.Id;
                _db.Entry(userticketgenerationObj).State = EntityState.Modified;

                _db.SaveChanges();

                return obj.id;
            }
            return assigneeObj.id;
        }

        //(WaitTime remaining check(close || pending Wait time)
        public async Task<string> SubmitAssigneeTicket(AssigneeTicketViewModel obj, string uploadFiles)
        {

            var assigneeTicket = _db.AssigneeTicket.Where(x => x.id == obj.id).FirstOrDefault();
            var userTicket = assigneeTicket != null ? _db.UserTicketGeneration.Where(x => x.Id == assigneeTicket.UserTicketGenerationId).FirstOrDefault() : null;
            var oldStatus = userTicket.StatusId;
            try
            {
                if (userTicket != null && assigneeTicket != null && ModelState.IsValid)
                {
                    assigneeTicket.AssigneeNotes = obj.AssigneeNotes;
                    assigneeTicket.StatusId = obj.StatusId;

                    var status = CommonFunctions.GetStatusName(obj.StatusId);
                    if (status.Equals("Resolved", StringComparison.CurrentCultureIgnoreCase) || status.Equals("UnResolved", StringComparison.CurrentCultureIgnoreCase))
                    {

                        //assigneeTicket.closeResoloutionTime = 0; // remaining

                        assigneeTicket.closesWeekDay = DateTime.Now.DayOfWeek.ToString();
                        assigneeTicket.closesCreatedDate = DateTime.Now;


                        // check on get wait time on different scenarios
                        if ((assigneeTicket.closesCreatedDate - assigneeTicket.inProgressCreatedDate).Value.TotalDays >= 1)
                        {
                            var totalTodayMinutes = DateTimeExtension.timedifferenceInMinutes(assigneeTicket.inProgressCreatedDate.Value.Date.Add(new TimeSpan(17, 0, 0)), assigneeTicket.inProgressCreatedDate);

                            var totalDaysMinute = DateTimeExtension.TotalDaysLeftInMinutes(assigneeTicket.inProgressCreatedDate, DateTime.Now, true);

                            var totalEndDayMinute = DateTimeExtension.timedifferenceInMinutes(DateTime.Now, assigneeTicket.closesCreatedDate.Value.Date.Add(new TimeSpan(9, 0, 0)));

                            assigneeTicket.closeResoloutionTime = totalTodayMinutes + totalDaysMinute + totalEndDayMinute;

                        }
                        else
                        {
                            assigneeTicket.closeResoloutionTime = (assigneeTicket.closesCreatedDate - assigneeTicket.inProgressCreatedDate).Value.TotalMinutes;
                        }

                    }

                    userTicket.StatusId = obj.StatusId;

                    // add ticket resolution
                    assigneeTicket.TicketResolutionId = obj.TicketResolutionId;

                    // update assignee ticket
                    _db.Entry(assigneeTicket).State = EntityState.Modified;


                    // update user ticket
                    _db.Entry(userTicket).State = EntityState.Modified;


                    await _db.SaveChangesAsync();
                    string UserId = "";
                    if (oldStatus != obj.StatusId)
                    {
                        var UserTicket = userTicket;
                        var History = new TicketNotificationHistory();
                        History.createdBy = UserTicket.createdBy;
                        History.createdDate = DateTime.Now;
                        UserId = userTicket.UserId;
                        History.createdWeekDay = DateTime.Now.DayOfWeek.ToString();
                        History.creatorNotes = UserTicket.creatorNotes;
                        History.isTicketHonor = UserTicket.isTicketHonor;
                        History.notify = false;
                        History.Patient = UserTicket.Patient;
                        History.PatientId = UserTicket.PatientId;
                        History.Priority = UserTicket.Priority;
                        History.PriorityId = UserTicket.PriorityId;
                        History.Status = UserTicket.Status;
                        History.StatusId = UserTicket.StatusId;
                        History.TicketGeneration = UserTicket.TicketGeneration;
                        History.UserTicketGenerationId = UserTicket.Id;
                        History.ticketHonorId = UserTicket.ticketHonorId;
                        History.Type = UserTicket.Type;
                        History.TypeId = UserTicket.TypeId;
                        History.UserId = UserTicket.UserId;
                        History.TicketGenerationId = UserTicket.TicketGenerationId;
                       
                        History.ChangingType = "Status";
                        History.NotificationCreatedBy = User.Identity.GetUserId();
                        _db.TicketNotificationHistory.Add(History);
                        _db.SaveChanges();
                    }




                    //////////************************************************For Images Saving*************************////////////////
                    string[] arr = null;
                    if (!string.IsNullOrEmpty(uploadFiles))
                        arr = uploadFiles.Split('|');
                    if (arr != null)
                    {
                        Patient_Images laboratoryResult = new Patient_Images();
                        for (int i = 0; i <= arr.Length - 1; i++)
                        {
                            string fileName = Guid.NewGuid() + "_UDT_" + DateTime.Now.ToString("MMddyyyy") + "_" + assigneeTicket.id;

                            string filepath = HelperExtensions.fileDirectory() + fileName;
                            string base64StringData = arr[i];
                            string cleandata = "";
                            string mimtype = "";
                            if (base64StringData.Contains("data:image/jpeg") || base64StringData.Contains("data:image/jpg"))
                            {
                                cleandata = base64StringData.Replace("data:image/jpeg;base64,", "");
                                mimtype = "image/jpeg";
                            }
                            else if (base64StringData.Contains("data:image/png"))
                            {
                                cleandata = base64StringData.Replace("data:image/png;base64,", "");
                                mimtype = "image/png";
                            }
                            else
                                cleandata = base64StringData;

                            TicketAttachment t_Obj = new TicketAttachment();
                            t_Obj.assigneeTicketId = assigneeTicket.id;
                            t_Obj.userTicketId = Convert.ToString(userTicket.Id);
                            t_Obj.fileName = fileName;
                            t_Obj.filePath = HelperExtensions.Encrypt(filepath);
                            t_Obj.createdBy = User.Identity.GetUserId();
                            t_Obj.createdDate = DateTime.Now;


                            _db.TicketAttachment.Add(t_Obj);
                            byte[] data = System.Convert.FromBase64String(cleandata);

                            System.IO.File.WriteAllBytes(filepath, data);

                        }
                        await _db.SaveChangesAsync();
                    }

                    return UserId;
                }
                else
                {
                    var errorList = ModelState.Values.SelectMany(m => m.Errors)
                                     .Select(e => e.ErrorMessage)
                                     .ToList();
                    var errorstr = string.Join(",", errorList);
                    return errorstr;
                }

            }
            catch (Exception ex)
            {

                return ex.Message;
            }
            return "";

        }
        public ActionResult Resolutions()
        {
            ViewBag.RsolutionList = _db.ticketResolution.Where(x => x.isDeleted == false).ToList();
            return View();
        }
        public JsonResult GetUpdate()
        {
            var data = _db.ticketResolution.Where(x => x.isDeleted == false).ToList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public ActionResult DeleteResolution(int id)
        {
            var userToDel = _db.ticketResolution.Find(id);
            userToDel.isDeleted = true;
            _db.Entry(userToDel).State = EntityState.Modified;
            _db.SaveChanges();

            return RedirectToAction("Resolutions");
        }
        [HttpPost]
        public JsonResult Resolutions(FormCollection abc)
        {
            if (abc != null)
            {
                string NameToEdit = Convert.ToString(abc[0]);
                var id = Convert.ToInt64(abc[1]);
                if (id > 0)
                {
                    var CurrentData = _db.ticketResolution.Where(x => x.resolutionName == NameToEdit).FirstOrDefault();
                    if (CurrentData == null)
                    {
                        var DatatoEdit = _db.ticketResolution.Where(x => x.id == id).FirstOrDefault();
                        DatatoEdit.resolutionName = NameToEdit;
                        DatatoEdit.updatedBy = User.Identity.GetUserId();
                        DatatoEdit.updatedDate = DateTime.Now;
                        _db.Entry(DatatoEdit).State = EntityState.Modified;
                        _db.SaveChanges();
                        return Json("Edited", JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json("Exist", JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    TicketResolution obj = new TicketResolution();
                    obj.resolutionName = NameToEdit;
                    obj.CreatedBy = User.Identity.GetUserId();
                    obj.CreatedDate = DateTime.Now;
                    _db.ticketResolution.Add(obj);
                    _db.SaveChanges();
                    return Json("Added", JsonRequestBehavior.AllowGet);
                }

            }
            else
            {
                return Json("false", JsonRequestBehavior.AllowGet);
            }

        }
        public JsonResult EditResolution(FormCollection fc)
        {
            string NameToEdit = Convert.ToString(fc[0]);
            long id = Convert.ToInt64(fc[1]);
            if (fc != null)
            {
                var CurrentData = _db.ticketResolution.Where(x => x.resolutionName == NameToEdit).FirstOrDefault();
                if (CurrentData != null)
                {
                    CurrentData.resolutionName = NameToEdit;
                    CurrentData.updatedBy = User.Identity.GetUserId();
                    CurrentData.updatedDate = DateTime.Now;
                    _db.Entry(CurrentData).State = EntityState.Modified;
                    _db.SaveChanges();


                }
                return Json("true", JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json("false", JsonRequestBehavior.AllowGet);
            }
        }


        public string TotalHours(DateTime? createdDate)
        {
            if (createdDate != null)
            {
                DateTime date = (DateTime)createdDate;
                TimeSpan timeSpan = DateTime.Now.Subtract(date);
                //int days = timeSpan.Days;
                //int hours = timeSpan.Hours;
                //int min = timeSpan.Minutes;
                //int sec = timeSpan.Seconds;
                double hh = Math.Round(timeSpan.TotalHours);
                return Convert.ToString(hh);
            }

            return "";
        }


        public ActionResult OpenNotification(int? Id)
        {
          var ticket=  _db.UserTicketGeneration.Where(p => p.Id == Id).FirstOrDefault();
            ViewBag.TicketId = ticket.Id;
            ViewBag.TicketStatus = ticket.Status;
            return View("ListViewTicket");
        }






    }
    public class NotificationViewMidal
    {
        public string AssigneeName { get; set; }
        public int? TicketId { get; set; }
        public string Time { get; set; }
        public int? patientId { get; set; }
        public string TicketType { get; set; }
        public string Priority { get; set; }
        public string CreatorNotes { get; set; }
        public string Status { get; set; }
        public string ChangingType { get; set; }
        public bool notify { get; set; }
        public int? Id { get; internal set; }
        public DateTime? createdDate { get; internal set; }
    }
}

















//                    else if (status.Equals("Pending", StringComparison.CurrentCultureIgnoreCase)) {
//                        //assigneeTicket.pendingTime = 0; // remaining
//                        assigneeTicket.pendingWeekDay = DateTime.Now.DayOfWeek.ToString();
//                        assigneeTicket.pendingCreatedDate = DateTime.Now;

//                        // check on get wait time on different scenarios
//                        if ((assigneeTicket.pendingCreatedDate - assigneeTicket.inProgressCreatedDate).Value.TotalDays > 0)
//                        {
//                            var totalTodayMinutes = DateTimeExtension.timedifferenceInMinutes(assigneeTicket.inProgressCreatedDate.Value.Date.Add(new TimeSpan(17, 0, 0)), assigneeTicket.inProgressCreatedDate);

//var totalDaysMinute = DateTimeExtension.TotalDaysLeftInMinutes(assigneeTicket.inProgressCreatedDate, DateTime.Now, true);

//var totalEndDayMinute = DateTimeExtension.timedifferenceInMinutes(DateTime.Now, assigneeTicket.pendingCreatedDate.Value.Date.Add(new TimeSpan(9, 0, 0)));

//assigneeTicket.pendingTime = totalTodayMinutes + totalDaysMinute + totalEndDayMinute;

//                        }
//                        else
//                        {
//                            assigneeTicket.pendingTime = (assigneeTicket.pendingCreatedDate - assigneeTicket.inProgressCreatedDate).Value.TotalMinutes;
//                        }

//                    }
