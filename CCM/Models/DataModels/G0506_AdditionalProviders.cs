using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace CCM.Models.DataModels
{
    public class G0506_AdditionalProviders
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int Id { get; set; }

        [Required]
        public string FullName { get; set; }
        public string Speciality { get; set; }

        [Display(Name = "Last Visited (mm/dd/yyyy)")]
        public string LastVisit { get; set; }

        [Display(Name = "Next Appointment (mm/dd/yyyy)")]
        public string NextAppointment { get; set; }

        public string DoctorType { get; set; }
        [Display(Name = "CCM Provider")]
        public bool isCCMProvider { get; set; }
        [Display(Name = "Phone Number")]

        public string MobilePhoneNumber { get; set; }

        [Display(Name = "Email")]
        public string Email { get; set; }

        [Display(Name = "Share Care Plan")]
        public bool? IsShareCarePlan { get; set; }

        [Display(Name = "NPI")]
        public int? NPI { get; set; }

        [Required]
        [Display(Name = "Main Phone Number")]
        public string MainPhoneNumber { get; set; }

        [Display(Name = "Address Line 1")]
        [Column(TypeName = "VARCHAR")]
        [StringLength(100)]
        public string Address1 { get; set; }

        [Display(Name = "Address Line 2")]
        [Column(TypeName = "VARCHAR")]
        [StringLength(100)]
        public string Address2 { get; set; }
        public bool IsPrimaryCareProvider { get; set; }

        [ForeignKey("G0506_PatientsInfo")]
        public int? G0506_PatientsInfoId { get; set; }
        public virtual G0506_PatientsInfo G0506_PatientsInfo { get; set; }



    }
}