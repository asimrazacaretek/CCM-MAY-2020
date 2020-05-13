using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace CCM.Models.DataModels
{
    public class Evaluation_SubQuestions
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        [ForeignKey("QuestionBank")]
        public int? QuestionId { get; set; }
        public virtual QuestionBank QuestionBank { get; set; }
        public string QuestionGUID { get; set; }

        public bool HasDate { get; set; }
        public bool Status { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public string Type { get; set; }

        public DateTime? UpdatedOn { get; set; }
        public string UpdatedBy { get; set; }

        [ForeignKey("Evaluation_Questions")]
        public int? Evaluation_QuestionsId { get; set; }
        public virtual Evaluation_Questions Evaluation_Questions { get; set; }
        public virtual List<Evaluation_SubQuestionAnswer> Evaluation_SubQuestionAnswer { get; set; }


    }
}