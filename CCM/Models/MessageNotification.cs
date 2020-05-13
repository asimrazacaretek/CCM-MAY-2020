using System;
using System.ComponentModel.DataAnnotations;


namespace CCM.Models
{
    public class MessageNotification
    {
        public int Id { get; set; }

        public DateTime? SendDateTime { get; set; }
        public string MessageBody { get; set; }

        public bool? IsMessageViewed { get; set; }
        public DateTime? MessageViewedDateTime { get; set; }
        public string Response { get; set; }
        public DateTime? ResponseDateTime { get; set; }

        public int? PatientId { get; set; }
        public int? LiaisonId { get; set; }

        public virtual Patient Patient { get; set; }
        public virtual Liaison Liaison { get; set; }
    }


    public class NewMessageViewModel
    {
        public int PatientId { get; set; }

        [Required]
        [Display(Name = "Message")]
        public string MessageBody { get; set; }
    }
}