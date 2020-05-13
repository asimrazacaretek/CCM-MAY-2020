using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CCM.Models.ViewModels
{
    public class TicketGenerationViewModel
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
        public string TicketResolution { get; set; }
        public int? TicketResolutionid { get; set; }


        public string ticketHonorId { get; set; }
        public bool isTicketHonor { get; set; }

    }
}