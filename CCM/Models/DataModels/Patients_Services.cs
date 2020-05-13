using System;
using System.ComponentModel.DataAnnotations;

namespace CCM.Models.DataModels
{
    public class Patients_Services
    {
        [Key]
        public int Id { get; set; }

        public int? DeviceId { get; set; }
        public virtual Device Device { get; set; }
        public int? PatientId { get; set; }
        public virtual Patient Patient { get; set; }
        public int? RPMServiceId { get; set; }
        public virtual RPMService RPMService { get; set; }
        public int IsAssigned { get; set; }
        public string CommentsOnAssigement { get; set; }
        public DateTime? AssignedDate { get; set; }
        public int IsActive { get; set; }
        public string ReasonForDeactivate { get; set; }
        public DateTime? CreatedOn { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }


    }
}