using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace CCM.Models.DataModels
{
    public class G0506_PatientsInfo
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int Id { get; set; }
        [ForeignKey("Patients")]
        public int? PatientId { get; set; }
        public Patient Patients { get; set; }
        public string FullName { get; set; }
        public DateTime? BirthDate { get; set; }
        public Nullable< bool> IsCurrentlyActiveinCCM { get; set; }
        public Nullable<bool> IsCCMConsentcompleted { get; set; }
        public DateTime? DateConsentcompleted { get; set; }

        public string DesignatedCCMContact { get; set; }
        [ForeignKey("G0506_PrimaryInsurance")]
        public int? G0506_PrimaryInsuranceId { get; set; }
        public virtual G0506_PrimaryInsurance G0506_PrimaryInsurance { get; set; }
        [ForeignKey("G0506_SecondaryInsurance")]
        public int? G0506_SecondaryInsuranceId { get; set; }
       

        public virtual G0506_SecondaryInsurance G0506_SecondaryInsurance { get; set; }
        public virtual List<G0506_AdditionalProviders> AdditionalProvidersList { get; set; }

        public bool Status { get; set; }

        public string CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }





    }
}