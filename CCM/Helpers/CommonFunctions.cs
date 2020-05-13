using CCM.Models;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CCM.Helpers
{
    public class CommonFunctions
    {
        public readonly ApplicationdbContect Db = new ApplicationdbContect();
        //public static List<SelectListItem> GetStatus()
        //{
        //    IStatusRepository _IStatusRepository = new StatusRepository();

        //    var _List = _IStatusRepository.GetAll().Where(x => x.isDelete == null).Select(x =>
        //      new SelectListItem()
        //      {
        //          Text = x.Status1,
        //          Value = x.StatusID.ToString()
        //      });
        //    return _List.ToList();
        //}
      
        public static List<SelectListItem> GetTicketSubject()
        {
            try
            {
                using (ApplicationdbContect Db = new ApplicationdbContect())
                {
                    return Db.TicketGeneration.Where(x=>x.isDeleted == null).ToList().Select(x => new SelectListItem()
                    {
                        Text = x.subjectName,
                        Value = x.Id.ToString()
                    }).ToList();
                }
            }
            catch (Exception ex)
            {

                return null;
            }
        }

        public static List<SelectListItem> GetTicketType()
        {
            try
            {
                using (ApplicationdbContect Db = new ApplicationdbContect())
                {
                    return Db.Type.ToList().Select(x => new SelectListItem()
                    {
                        Text = x.typeName,
                        Value = x.Id.ToString()
                    }).ToList();
                }
            }
            catch (Exception ex)
            {

                return null;
            }
        }

        public static List<SelectListItem> GetTicketStatus()
        {
            try
            {
                using (ApplicationdbContect Db = new ApplicationdbContect())
                {
                    return Db.Status.Where(x => (x.statusName.Equals("Open", StringComparison.CurrentCultureIgnoreCase))).ToList().Select(x => new SelectListItem()
                    {
                        Text = x.statusName,
                        Value = x.Id.ToString()
                    }).ToList();
                }
            }
            catch (Exception ex)
            {

                return null;
            }
        }

        public static List<SelectListItem> GetTicketStatusAll()
        {
            try
            {
                using (ApplicationdbContect Db = new ApplicationdbContect())
                {
                    return Db.Status.Where(x => (x.statusName!="Pending")).ToList().Select(x => new SelectListItem()
                    {
                        Text = x.statusName,
                        Value = x.Id.ToString()
                    }).ToList();
                }
            }
            catch (Exception ex)
            {

                return null;
            }
        }

        public static List<SelectListItem> GetTicketStatusAllName()
        {
            try
            {
                using (ApplicationdbContect Db = new ApplicationdbContect())
                {
                    return Db.Status.Where(x => (x.statusName != "Pending")).ToList().Select(x => new SelectListItem()
                    {
                        Text = x.statusName,
                        Value = x.statusName.ToString()
                    }).ToList();
                }
            }
            catch (Exception ex)
            {

                return null;
            }
        }

        public static List<SelectListItem> GetTicketPriority()
        {
            try
            {
                using (ApplicationdbContect Db = new ApplicationdbContect())
                {
                    return Db.Priority.ToList().Select(x => new SelectListItem()
                    {
                        Text = x.priorityLevel,
                        Value = x.Id.ToString()
                    }).ToList();
                }
            }
            catch (Exception ex)
            {

                return null;
            }
        }

        public static List<SelectListItem> GetTicketStatusWithOutInProgressJustWatch()
        {
            try
            {
                using (ApplicationdbContect Db = new ApplicationdbContect())
                {
                    //.Where(x => (x.statusName.Equals("Pending", StringComparison.CurrentCultureIgnoreCase) || x.statusName.Equals("Resolved", StringComparison.CurrentCultureIgnoreCase)))
                    return Db.Status.ToList().Where(x => (x.statusName.Equals("In Progress", StringComparison.CurrentCultureIgnoreCase) || x.statusName.Equals("Resolved", StringComparison.CurrentCultureIgnoreCase) || x.statusName.Equals("UnResolved", StringComparison.CurrentCultureIgnoreCase) || x.statusName.Equals("Open", StringComparison.CurrentCultureIgnoreCase)))
                      .Select(x => new SelectListItem()
                      {
                          Text = x.statusName,
                          Value = x.Id.ToString()
                      }).OrderByDescending(x => x.Text).ToList();
                }
            }
            catch (Exception ex)
            {

                return null;
            }
        }

        public static List<SelectListItem> GetTicketStatusWithOutInProgress()
        {
            try
            {
                using (ApplicationdbContect Db = new ApplicationdbContect())
                {
                    //.Where(x => (x.statusName.Equals("Pending", StringComparison.CurrentCultureIgnoreCase) || x.statusName.Equals("Resolved", StringComparison.CurrentCultureIgnoreCase)))
                    return Db.Status.ToList().Where(x => (x.statusName.Equals("In Progress", StringComparison.CurrentCultureIgnoreCase) || x.statusName.Equals("Resolved", StringComparison.CurrentCultureIgnoreCase) || x.statusName.Equals("UnResolved", StringComparison.CurrentCultureIgnoreCase) || x.statusName.Equals("Open", StringComparison.CurrentCultureIgnoreCase)))
                      .Select(x => new SelectListItem()
                      {
                        Text = x.statusName,
                        Value = x.Id.ToString()
                    }).OrderByDescending(x=>x.Text).ToList();
                }
            }
            catch (Exception ex)
            {

                return null;
            }
        }
        public static List<SelectListItem> GetTicketStatusTicket()
        {
            try
            {
                using (ApplicationdbContect Db = new ApplicationdbContect())
                {
                    //.Where(x => (x.statusName.Equals("Pending", StringComparison.CurrentCultureIgnoreCase) || x.statusName.Equals("Resolved", StringComparison.CurrentCultureIgnoreCase)))
                    return Db.Status.ToList().Where(x => (x.statusName.Equals("In Progress", StringComparison.CurrentCultureIgnoreCase) || x.statusName.Equals("Resolved", StringComparison.CurrentCultureIgnoreCase) || x.statusName.Equals("Open", StringComparison.CurrentCultureIgnoreCase) || x.statusName.Equals("UnResolved", StringComparison.CurrentCultureIgnoreCase)))
                      .Select(x => new SelectListItem()
                      {
                          Text = x.statusName,
                          Value = x.Id.ToString()
                      }).OrderByDescending(x => x.Text).ToList();
                }
            }
            catch (Exception ex)
            {

                return null;
            }
        }

        public static List<SelectListItem> GetTicketStatusWithOutInProgressOrignal()
        {
            try
            {
                using (ApplicationdbContect Db = new ApplicationdbContect())
                {
                    //.Where(x => (x.statusName.Equals("Pending", StringComparison.CurrentCultureIgnoreCase) || x.statusName.Equals("Resolved", StringComparison.CurrentCultureIgnoreCase)))
                    return Db.Status.ToList().Where(x => (x.statusName.Equals("In Progress", StringComparison.CurrentCultureIgnoreCase) || x.statusName.Equals("Resolved", StringComparison.CurrentCultureIgnoreCase) || x.statusName.Equals("UnResolved", StringComparison.CurrentCultureIgnoreCase) || x.statusName.Equals("UnResolved", StringComparison.CurrentCultureIgnoreCase) || x.statusName.Equals("Open", StringComparison.CurrentCultureIgnoreCase)))
                        .Select(x => new SelectListItem()
                    {
                        Text = x.statusName,
                        Value = x.Id.ToString()
                    }).OrderByDescending(x => x.Text).ToList();
                }
            }
            catch (Exception ex)
            {

                return null;
            }
        }

        public static List<SelectListItem> GetTicketResolution()
        {
            try
            {
                using (ApplicationdbContect Db = new ApplicationdbContect())
                {
                    return Db.ticketResolution.ToList().Select(x => new SelectListItem()
                    {
                        Text = x.resolutionName,
                        Value = x.id.ToString()
                    }).OrderByDescending(x => x.Text).ToList();
                }
            }
            catch (Exception ex)
            {

                return null;
            }
        }


        public static string GetStatusName(int? id) {
            try
            {
                using (ApplicationdbContect db = new ApplicationdbContect())
                {
                   var statusObj = db.Status.Where(x => x.Id == id).FirstOrDefault();
                    return statusObj != null ? statusObj.statusName : "Not Found";
                }
            }
            catch (Exception ex)
            {
                return "Not Found";
            }
        }
        public static List<SelectListItem> GetAssigneeUsers(string userid)
        {

            try
            {
                using (ApplicationdbContect db = new ApplicationdbContect())
                {
                    
                    var user1 = db.Users.Find(userid);
                    var allusers = db.Users.Where(x => x.Role != "Patient" && x.Id != user1.Id
                    ).Select(x2 => new UsersViewModel { Id = x2.Id, Name = x2.FirstName + " " + x2.LastName + " (" + x2.Role + ")" }).ToList();
                    foreach (var user in allusers)
                    {
                        if (user.Name.Contains("Liaison"))
                        {
                            var istranslator = HelperExtensions.isTranslator(user.Id);
                            var currentUser = db.Liaisons.Where(x => x.UserId == user.Id).FirstOrDefault();
                            if (istranslator == true)
                            {
                                user.Name = user.Name.Replace("(Liaison)", "(Translator)");
                                user.isActive = currentUser==null?false: currentUser.isActive;
                            }
                            else
                            {
                                user.Name = user.Name.Replace("(Liaison)", "(Counselor)");
                                user.isActive = currentUser == null ? false : currentUser.isActive;
                            }
                        }
                        else
                        {
                            user.Name = user.Name.Replace("(Sales)", "(Enroller)");
                            user.isActive = true;
                        }
                    }

                    return allusers.OrderBy(m => m.Name).ToList().Select(x => new SelectListItem()
                    {
                        Text = x.Name,
                        Value = x.Id.ToString(),
                       Selected=x.isActive
                    }).ToList();
                  
                }
            }
            catch (Exception ex)
            {

                return null;
            }
        }

        public static List<SelectListItem> GetPatients()
        {
            try
            {
                using (ApplicationdbContect Db = new ApplicationdbContect())
                {
                    var liaisons = Db.Liaisons.Where(x => x.IsTranslator == false).Select(p => new SelectListItem
                    {
                        Value = p.Id.ToString(),
                        Text = p.FirstName + " " + p.LastName
                    }).ToList();
                    var translator = Db.Liaisons.Where(x => x.IsTranslator == true).Select(p => new SelectListItem
                    {
                        Value = p.Id.ToString(),
                        Text = p.FirstName + " " + p.LastName
                    }).ToList();
                    var physicians = Db.Physicians.AsNoTracking().Select(p => new SelectListItem
                    {
                        Value = p.Id.ToString(),
                        Text = p.FirstName + " " + p.LastName
                    }).ToList();
                    var physiciansGroups = Db.PhysiciansGroup.AsNoTracking().Select(p => new SelectListItem
                    {
                        Value = p.Id.ToString(),
                        Text = p.GroupName

                    }).ToList();
                    return Db.Priority.ToList().Select(x => new SelectListItem()
                    {
                        Text = x.priorityLevel,
                        Value = x.Id.ToString()
                    }).ToList();
                }
            }
            catch (Exception ex)
            {

                return null;
            }
        }


        public static List<SelectListItem> GetActiveDevicesTypes()
        {
            try
            {
                using (ApplicationdbContect Db = new ApplicationdbContect())
                {
                    return Db.RPMServices.Where(x => x.IsActive == 1).ToList().Select(x => new SelectListItem()
                    {
                        Text = x.ServiceName,
                        Value = x.Id.ToString()
                    }).ToList();
                }
            }
            catch (Exception ex)
            {

                return null;
            }
        }

    }
}