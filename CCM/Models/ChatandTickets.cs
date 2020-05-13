using CCM.Models.DataModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CCM.Models
{
    public class ChatandTickets
    {

    }

    public class PrivateChat
    {
        public int id { get; set; }
        public string From { get; set; }
        public string SenderName { get; set; }
        public string To { get; set; }
        public string ReceiverName { get; set; }
        public string Message { get; set; }
        public DateTime DateSent { get; set; }
        public bool Read { get; set; }
        public bool Attachment { get; set; }
        public string FilePath { get; set; }
    }
    public class PrivateChatViewModel
    {
        public PrivateChat PrivateChat { get; set; }
        public string FromUser { get; set; }
        public string ToUser { get; set; }
    }

    public class TicketDashboardViewModel {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public string ticketSubjectDashboard { get; set; }
        public string Tickettype { get; set; }
        public string TicketAssignee { get; set; }

        public string status { get; set; }
        public string createdBy { get; set; }
    }

    public class GroupNotifylistViewModel
    {
        public string groupid { get; set; }
        public string user { get; set; }
        public int unreadAllMsg { get; set; }
        public int UnreadGroupMsg { get; set; }
    }
    public class NotifyList
    {
        public string user { get; set; }
        public int total { get; set; }
    }
    public class GroupNewParticipents
    {
        public string user { get; set; }
        public int totalgroup { get; set; }
    }
    
    public class OnlineUser
    {
        public int id { get; set; }
        public int UserId { get; set; }
        public bool Acitve { get; set; }
        public DateTime? loginDate { get; set; }
    }


    /////////////////////////////////////////////////////////////////Group ChatClass///////////////////////////////////////////////////////////////
    public class GroupChats
    {
        public int Id { get; set; }
        public string ChatName { get; set; }
        [Display(Name = "Date Created")]
        [DataType(DataType.Date)]
        public DateTime CreatedOn { get; set; }

        public string CreatedBy { get; set; }

        [Display(Name = "Date Updated")]
        [DataType(DataType.Date)]
        public DateTime? UpdatedOn { get; set; }

        public string UpdatedBy { get; set; }

        public string GroupStatus { get; set; }
    }
    public class GroupChatsDetails
    {
        public int Id { get; set; }
        public int GroupChatId { get; set; }

        public string Message { get; set; }

        [Display(Name = "Date Created")]
        [DataType(DataType.Date)]
        public DateTime? CreatedOn { get; set; }
        public string CreatedBy { get; set; }
    }
    public class GroupChatsParticipants
    {
        public int Id { get; set; }
        public int GroupChatId { get; set; }

        public string UserId { get; set; }

        public bool isCreater { get; set; }

        public bool Notify { get; set; }
    }
    public class GroupChatMessages
    {
        public int Id { get; set; }
        public int GroupChatDetailsId { get; set; }

        public int MessageId { get; set; }

        public string ReceivedBy { get; set; }
        public DateTime ReceivedOn { get; set; }
        public bool isNew { get; set; }
        public bool Attachment { get; set; }
        public string FilePath { get; set; }
    }
    public class GroupchatsdetailsViewModel
    {
        public string Message { get; set; }
        public DateTime timesent
        {
            get; set;
        }
        public string sender { get; set; }

        public string senderclass { get; set; }
        public int LastID { get; set; }
        public int TotalMessages { get; set; }
        public bool isNew { get; set; }

        public string userid { get; set; }
        public bool attachment { get; set; }
        public int msgid { get; set; }
        public string filePath { get; set; }
    
    }
    public class GroupChatViewModel
    {
        public string Message { get; set; }
        public int GID { get; set; }
        public bool Attachment { get; set; }
        public string filename { get; set; }
        public string filepath { get; set; }
        public HttpPostedFileBase file { get; set; }
    }
    public class GroupChatsTotalCounts
    {
        public int GroupChatId { get; set; }
        public int TotalCount { get; set; }
    }
    public class GroupchatsViewModel
    {
        public GroupChats GroupChat { get; set; }
        public List<UsersViewModel> usersViewModels { get; set; }

        public int TotalCount { get; set; }
    }
    public class UsersViewModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public bool isActive { get; set; }
    }

    public class Priority
    {
        public int Id { get; set; }

        public string priorityLevel { get; set; }

        public double priorityMinute { get; set; }

    }

    public class Status
    {
        public int Id { get; set; }

        public string statusName { get; set; }

    }

    public class Type
    {
        public int Id { get; set; }
        public string typeName { get; set; }
    }
    public class TicketGeneration
    {
        public int Id { get; set; }

        public string subjectName { get; set; }

        public DateTime createdDate { get; set; } = DateTime.Now;

        public string createdBy { get; set; }

        public DateTime? updatedDate { get; set; }
        public string updateBy { get; set; }

        public Boolean? isDeleted { get; set; }

        public DateTime? deletedDate { get; set; }

        public string deletedBy { get; set; }
        public int? TypeId { get; set; }

        public virtual Type Type { get; set; }
        public int? PriorityId { get; set; }

        public virtual Priority Priority { get; set; }

        public int? StatusId { get; set; }

        public virtual Status Status { get; set; }


    }

    public class UserTicketGeneration
    {
        public int Id { get; set; }

        public int? TicketGenerationId { get; set; }
        public virtual TicketGeneration TicketGeneration { get; set; }

        //public int? TicketResolutionId { get; set; }
        //public virtual TicketResolution TicketResolution { get; set; }

        public int? TypeId { get; set; }

        public virtual Type Type { get; set; }
        public int? PriorityId { get; set; }

        public virtual Priority Priority { get; set; }

        public int? StatusId { get; set; }

        public virtual Status Status { get; set; }

        public string creatorNotes { get; set; }
        public string UserId { get; set; }

        public int? PatientId { get; set; }
        public virtual Patient Patient { get; set; }

        public string createdBy { get; set; }

        public DateTime? createdDate { get; set; }

        public string createdWeekDay { get; set; }

        public Boolean notify { get; set; }

        public string ticketHonorId { get; set; }
        public bool isTicketHonor { get; set; }


        public List<TicketNotificationHistory> TicketNotificationHistory { get; set; }

    }

    public class TicketAttachment
    {
        public int Id { get; set; }
        public string userTicketId { get; set; }

        public int assigneeTicketId { get; set; }

        public string filePath { get; set; }

        public string fileName { get; set; }

        public string createdBy { get; set; }
        public DateTime? createdDate { get; set; }

        public string updatedBy { get; set; }

        public DateTime? updatedDate { get; set; }
    }
    

    public class TicketListViewModel {

        public string subjectName { get; set; }
        public string createdBy { get; set; }
        public string tickettype { get; set; }
        public string ticketassignee { get; set; }

        public DateTime? startDate { get; set; }
        public DateTime? enddate { get; set; }
        public string AssignTo { get; set; }

        public string statusName { get; set; }
        public string Priority { get; set; }
        public string status { get; set; }
        public int TicketResolutionId { get; set; }
        public List<ListViewTicket> ticketlistView { get; set; }
        public List<TicketsCreatedViewModel> ticketCreated { get; set; }

    }
    public class ListViewTicket
    {
        public int UserTicketId { get; set; }
        public string subjectName { get; set; }
        public string createdBy { get; set; }
        public string PatientName { get; set; }
        public int PatientId { get; set; }
        public string Pstatus { get; set; }
        public string Psubstatus { get; set; }

        public string TypeName { get; set; }
        public Nullable<double> timestamp { get; set; }
        public string createdDate { get; set; }
        public string AssignTo { get; set; }

        public string statusName { get; set; }
        public string TicketResolution { get; set; }
        public bool notify { get; set; }

        public virtual Priority Priority { get; set; }
        public virtual TicketGeneration TicketGeneration { get; set; }
    }
    public class AssigneeTicket
    {
        public int id { get; set; }
        public string ticketSubject { get; set; }
        public string ticketType { get; set; }
        public string ticketPriority { get; set; }

        public int? StatusId { get; set; }
        public virtual Status Status { get; set; }
        public string ticketTat { get; set; }
        public string CreatorNotes { get; set; }

        public string createdBy { get; set; }

        public DateTime? inProgressCreatedDate { get; set; }
        public string inProgressWeekDay { get; set; }

        public double inProgressWaitTime { get; set; }

        public DateTime? closesCreatedDate { get; set; }
        public string closesWeekDay { get; set; }

        public double closeResoloutionTime { get; set; }

        public DateTime? pendingCreatedDate { get; set; }
        public string pendingWeekDay { get; set; }

        public double pendingTime { get; set; }


        public int? UserTicketGenerationId { get; set; }
        public virtual UserTicketGeneration UserTicketGeneration { get; set; }


        public int? TicketResolutionId { get; set; }
        public virtual TicketResolution TicketResolution { get; set; }

        public string AssigneeNotes { get; set; }

    }

    public class AssigneeTicketViewModel
    {
        public int id { get; set; }
        public string ticketSubject { get; set; }
        public string ticketType { get; set; }
        public string ticketPriority { get; set; }

        public int? StatusId { get; set; }
        public virtual Status Status { get; set; }
        public string ticketTat { get; set; }
        public string CreatorNotes { get; set; }

        public int patientId { get; set; }
        public string CreaterId { get; set; }
        public virtual UserTicketGeneration UserTicketGeneration { get; set; }
        public int? UserTicketGenerationId { get; set; }


        public int? TicketResolutionId { get; set; }
        public virtual TicketResolution TicketResolution { get; set; }

        public string ticketStatus { get; set; }
        public string AssigneeNotes { get; set; }

        public List<TicketAttachment> TicketAttachmentList { get; set; }

    }
    public class TicketComment
    {
        public int Id { get; set; }
        public virtual UserTicketGeneration UserTicketGeneration { get; set; }
        public int? UserTicketGenerationId { get; set; }

        public string noteText { get; set; }

        public DateTime createdDate { get; set; } = DateTime.Now;

        public string createdBy{ get; set; }

        public virtual AssigneeTicket AssigneeTicket { get; set; }
        public int? AssigneeTicketId { get; set; }

        public string ticketActionName { get; set; }

        public string updatedBy { get; set; }

        public DateTime? updatedDate { get; set; }

        public Boolean isDeleted { get; set; }

        public DateTime? deletedDate { get; set; }
    }
    public class CommentBoxViewModel
    {
        public int Id { get; set; }
        public virtual UserTicketGeneration UserTicketGeneration { get; set; }
        public int? UserTicketGenerationId { get; set; }
        public string CommentBox { get; set; }
    }
    public class TicketsCreatedViewModel
    {

        public int id { get; set; }

        public string AssigneeName { get; set; }
        public string subjectName { get; set; }

        public string Status { get; set; }
        public string PatientName { get; set; }
        public int PatientId { get; set; }
        public string TypeName { get; set; }
        public string CreatorName { get; set; }
        public string CreatorId { get; set; }
        public DateTime CreatedDate { get; set; }
        public string Ishonour { get; set; }
        public string ticketHonourId { get; set; }
        public string Pstatus { get; set; }
        public string Psubstatus { get; set; }
        public string TicketResolution { get; set; }


        public virtual UserTicketGeneration UserTicketGeneration { get; set; }
        public int? UserTicketGenerationId { get; set; }
    }
    public class TicketResolution
    {
        public int id { get; set; }
        public string resolutionName { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public string updatedBy { get; set; }

        public DateTime? updatedDate { get; set; }

        public Boolean isDeleted { get; set; }

        public DateTime? deletedDate { get; set; }
    }
    /////////////////////////////////////////////////////////////////Group ChatClass///////////////////////////////////////////////////////////////
}