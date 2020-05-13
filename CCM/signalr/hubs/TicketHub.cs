using CCM.Models;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.Identity;
using CCM.Controllers;
using System.ComponentModel;

namespace CCM.signalr.hubs
{
    public class TicketHub : Hub
    {
        private ApplicationdbContect db = new ApplicationdbContect();

        public object JSON { get; private set; }

        public void ticketnotify(string toUserId, string senderId ,string Type="")
        {
            var NotoficationList = new List<NotificationViewMidal>();
            string userId = toUserId;
            // Get fr count
            var uncheckTicketsList = db.TicketNotificationHistory.Where(x => x.UserId == userId && x.notify == false).ToList();
            uncheckTicketsList = uncheckTicketsList.OrderByDescending(x=>x.Id).ToList();
            var uncheckTickets = uncheckTicketsList.Where(x => x.UserId == userId && x.notify == false).FirstOrDefault();
            //var res = uncheckTickets.Count > 0 ? Convert.ToString(uncheckTickets.Count) : "";
            //foreach (var item in uncheckTickets)
            //{

            var item = uncheckTickets;
                var Notification = new NotificationViewMidal();
                Notification.AssigneeName = HelperExtensions.GetUserNamebyID(item.createdBy);
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

                Notification.patientId = item.PatientId;
              

                Notification.Priority = item.Priority!=null? item.Priority.priorityLevel:"";
               
                
                Notification.TicketType = item.Type!=null? item.Type.typeName:"";
                Notification.CreatorNotes = item.creatorNotes;
                Notification.TicketId = item.UserTicketGenerationId;
                Notification.Status = item.Status!=null? item.Status.statusName:"";
            Notification.Id = item.Id;
            if (Type == "Comment")
            {
                userId = item.NotificationCreatedBy;
                Notification.ChangingType = "Comment";
                Notification.AssigneeName = HelperExtensions.GetUserNamebyID(userId);


                if (item.isTicketHonor != true)
                {
                    if (item.createdBy == userId)
                    {
                        userId = item.UserId;
                    }
                    else
                    {
                        userId = item.createdBy;
                    }
                }
                else
                {
                    if (item.UserId == userId)
                    {
                        userId = item.ticketHonorId;
                    }
                }
            }else if (Type=="Status")
            {
                userId = item.NotificationCreatedBy;
                Notification.ChangingType = "Status";
                Notification.AssigneeName = HelperExtensions.GetUserNamebyID(userId);

                Notification.Status = item.Status.statusName;
             

                if (item.isTicketHonor != true)
                {
                    if (item.createdBy == userId)
                    {
                        userId = item.UserId;
                    }
                    else
                    {
                        userId = item.createdBy;
                    }
                }
                else
                {
                    if (item.ticketHonorId == userId)
                    {
                        userId = item.UserId;
                    }
                    else
                    {
                        userId = item.ticketHonorId;
                    }
                }
            }
            else
            {
                Notification.ChangingType = "Create";

            }
        
            NotoficationList.Add(Notification);
            //}
            //NotoficationList=NotoficationList.OrderByDescending(x => x.TicketId).ToList();
            // Set clients
            Clients.User(userId).frnotify2(userId, new NotificationViewMidal { AssigneeName = new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(NotoficationList) });

            // Call js function

        }



        public override Task OnConnected()
        {

            string clientId = Context.ConnectionId;
            string data = clientId;
        
            return base.OnConnected();
        }

        public override Task OnReconnected()
        {
            return base.OnReconnected();
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            string count = "NA";

            string clientId = Context.ConnectionId;
            string[] Exceptional = new string[1];
            Exceptional[0] = clientId;
            Clients.AllExcept(Exceptional).receiveMessage(clientId, "NewConnection", clientId + " leave", count, "");

            return base.OnDisconnected(stopCalled);
        }
    }
}