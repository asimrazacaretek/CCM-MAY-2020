using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CCM.Models.ViewModels
{
    public class TwilioNumberViewModel
    {
        public int Id { get; set; }
        public string FriendlyPhoneNumer { get; set; }


        [Display(Name = "Mobile Phone Number")]
        [RegularExpression(@"^([0-9]{10})$", ErrorMessage = "Invalid Phone Number.")]
        [Range(1000000000, 9999999999, ErrorMessage = "Invalid Phone Number")]
        public string MobilePhoneNumber { get; set; }


        public bool Status { get; set; }
        public string Assignedto { get; set; }


        public DateTime? CreatedOn { get; set; }
        public string CreatedBy { get; set; }

        public DateTime? UpdatedOn { get; set; }
        public string UpdatedBy { get; set; }
        

    }
}