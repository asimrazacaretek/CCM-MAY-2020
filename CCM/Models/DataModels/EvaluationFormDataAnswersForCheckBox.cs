using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace CCM.Models.DataModels
{
    public class EvaluationFormDataAnswersForCheckBox
    {

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int Id { get; set; }
 
        public string AnswerGuid { get; set; }
        public string Answer { get; set; }
        public bool Status { get; set; }
        public bool HasDate { get; set; }

        public DateTime? Date { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }

        [ForeignKey("EvaluationFormData")]
        public int? EvaluationFormDataId { get; set; }
        public virtual EvaluationFormData EvaluationFormData { get; set; }

    }
}