using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CCM.Models.ViewModels
{
    public class MainQuestionViewModal
    {
        public MainQuestionViewModal()
        {
            this.MainAnswer = new List<MainAnswerViewModal>();
            this.subQuestions = new List<SubQuestionsViewModal>();
        }

        public string QuestionId { get; set; }
        public string MainQuestion { get; set; }
        public string haveSubQuestion { get; set; }
        public string QuestionGUID { get; set; }
        public string CurrentAnswer { get; set; }
        public List<SubQuestionsViewModal> subQuestions { get; set; }
        public string AnswerType { get; set; }
        public string sortIndex { get; set; }
        public string haveDateTime { get; set; }
        public string Date { get; set; }
        public List<MainAnswerViewModal> MainAnswer { get; set; }
    }
}