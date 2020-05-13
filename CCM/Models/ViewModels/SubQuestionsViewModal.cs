using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CCM.Models.ViewModels
{
    public class SubQuestionsViewModal
    {
        public SubQuestionsViewModal()
        {
            this.answers = new List<AnswersViewModal>();
        }
        public string QuestionId { get; set; }
        public string Question { get; set; }
        public string AnswerType { get; set; }
        public string haveDateTime { get; set; }
        public string Date { get; set; }
        public string QuestionGUID { get; set; }
        public string CurrentAnswer { get; set; }

        public List<AnswersViewModal> answers { get; set; }

    }
}