using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace CCM.Models.DataModels
{
    public class Evaluation_SubQuestionAnswer
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int Id { get; set; }
        [ForeignKey("QuestionBank")]
        public int? QuestionId { get; set; }
        public virtual QuestionBank QuestionBank { get; set; }
        public string Question { get; set; }
        public string QuestionGUID { get; set; }
        public string Type { get; set; }
        public bool HaveDateTime { get; set; }
        public bool Status { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string CreatedBy { get; set; }


        public DateTime? UpdatedOn { get; set; }
        public string UpdatedBy { get; set; }

        [ForeignKey("Evaluation_SubQuestions")]
        public int? SubQuestionId { get; set; }
        public  virtual Evaluation_SubQuestions Evaluation_SubQuestions { get; set; }
    }
}