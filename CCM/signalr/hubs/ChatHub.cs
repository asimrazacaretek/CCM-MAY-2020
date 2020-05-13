using System;
using Microsoft.AspNet.Identity;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using CCM.Models;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;

namespace CCM.signalr.hubs
{
    public class ChatHub : Hub
    {
        private ApplicationdbContect db = new ApplicationdbContect();
        public void Send(string sender, string reciverId,string name, string message, Boolean attachment, string messageId)
        {
            Clients.User(reciverId).addNewMessageToPage(name,sender,message, attachment, messageId);
        }

        public void Notify(string friend,string sender)
        {
            // Get fr count
            var UnReadAllMsg = db.privateChats.Count(x => x.To == friend && x.Read == false);
            var UnreadGroupMsg = db.GroupChatMessages.Count(x => x.isNew == true && x.ReceivedBy == friend);
            var totalUnreadMsg = Convert.ToInt32(UnreadGroupMsg) + Convert.ToInt32(UnReadAllMsg);

            var UnreadMsgForSender = db.privateChats.Count(x => x.To == friend && x.From == sender && x.Read==false);
            var clients = Clients.Others;
            // Call js function
            clients.frnotify(friend, totalUnreadMsg, UnreadMsgForSender,sender);
        }
        public void GroupNotify(int groupid, string senderid)
        {
            List<GroupNotifylistViewModel> notifylist = new List<GroupNotifylistViewModel>();
            // Get fr count
            var Participantsuser = db.GroupChatsParticipants.Where(x => (x.GroupChatId == groupid && x.UserId != senderid)).ToList();
            for (int i = 0; i < Participantsuser.Count; i++)
            {
                var participant = Participantsuser[i].UserId.ToString();var group = groupid;
                var UnReadPrivateMsg = db.privateChats.Count(y => y.Read == false && y.To == participant.ToString());
                var UnreadGroupMsg = db.GroupChatMessages.Count(x => (x.isNew == true && x.ReceivedBy == participant));
                var totalUnreadMsg = Convert.ToInt32(UnreadGroupMsg) + Convert.ToInt32(UnReadPrivateMsg);
                var Unreadgroup = db.GroupChatMessages.Where(x => (x.GroupChatDetailsId == group && x.isNew == true && x.ReceivedBy== participant)).ToList();
                notifylist.Add(new GroupNotifylistViewModel
                {
                    groupid = groupid.ToString(),
                    user = participant,
                    unreadAllMsg = totalUnreadMsg,
                    UnreadGroupMsg = Unreadgroup.Count
                });
            }
            //var UnReadAllMsg= db.GroupChatMessages.Count(x => x.isNew == true && x.ReceivedBy == senderid);
            //var UnReadGroup= db.GroupChatMessages.Count(x => x.GroupChatDetailsId== groupid && x.isNew == true && x.ReceivedBy== senderid);
            var clients = Clients.Others;
            // Call js function
            clients.groupchatnotify(notifylist);
        }

        public void AddnewgroupNotify(string senderid , string groupid)
        {
            List<GroupNewParticipents> participents = new List<GroupNewParticipents>();
            // Get fr count
            int idofgroup = Convert.ToInt32(groupid);
            var Participantsuser = db.GroupChatsParticipants.Where(x=>  (x.UserId != senderid && x.GroupChatId == idofgroup)).ToList();
            for (int i = 0; i < Participantsuser.Count; i++)
            {
                var user = Participantsuser[i].UserId;
                var newgroups = db.GroupChatsParticipants.Where(x=> (x.Notify==true && x.UserId==user)).ToList();
                participents.Add(new GroupNewParticipents
                {
                    user = user,
                    totalgroup = newgroups.Count
                });
            }
            var clients = Clients.Others;
            // Call js function
            clients.NewgroupNotify(participents);
        }
        //// ************************************** Group Chat**************************************//
        //public void AddToRoom(string roomName)
        //{
        //    //Retrieve room.
        //    var room = db.Rooms.Find(roomName);

        //    if (room != null)
        //    {
        //        var user = new User() { UserName = Context.User.Identity.Name };
        //        db.Users.Attach(user);

        //        room.Users.Add(user);
        //        db.SaveChanges();
        //        Groups.Add(Context.ConnectionId, roomName);
        //    }
        //}

        //public void RemoveFromRoom(string roomName)
        //{
        //    // Retrieve room.
        //    //var room = db.Rooms.Find(roomName);
        //    //if (room != null)
        //    //{
        //    //    var user = new User() { UserName = Context.User.Identity.Name };
        //    //    db.Users.Attach(user);

        //    //    room.Users.Remove(user);
        //    //    db.SaveChanges();

        //    //    Groups.Remove(Context.ConnectionId, roomName);
        //    //}
        //}

        public async Task JoinRoom(string roomName)
        {
            await Groups.Add(Context.ConnectionId, roomName);
            Clients.Group(roomName).addChatMessage(Context.User.Identity.Name + " joined.");
        }


        //Sent message
        public void BroadCastMessage(string msgfromid,String msgFrom, String msg, String GroupName,string groupid, bool Attachment, string msgid)
        {
            var id = Context.ConnectionId;
            Clients.Group(GroupName).receiveMessage(msgFrom, msg, msgfromid,groupid, Attachment, msgid);
            //Clients.All.receiveMessage(msgFrom, msg, "");
            /*string[] Exceptional = new string[1];
            Exceptional[0] = id;       
            Clients.AllExcept(Exceptional).receiveMessage(msgFrom, msg);*/
        }
        /* 
         * Following custom method is written to connect current user to a particular group. 
         * The groupname was entered by user only.
         * **/
        [HubMethodName("groupconnect")]
        public void Get_Connect(String userid)
        {
            var clientid= Context.ConnectionId;
            var results = (from gc in db.GroupsChats.AsNoTracking()
                           join gcp in db.GroupChatsParticipants.AsNoTracking() on gc.Id equals gcp.GroupChatId
                           where gcp.UserId == userid
                           select gc).OrderByDescending(x => x.CreatedOn).ToList();
            foreach (var item in results)
            {
                Groups.Add(clientid, item.ChatName);
            }
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
            Clients.AllExcept(Exceptional).receiveMessage(clientId,"NewConnection", clientId + " leave", count,"");

            return base.OnDisconnected(stopCalled);
        }

        //// ************************************** Group Chat**************************************//

    }
}