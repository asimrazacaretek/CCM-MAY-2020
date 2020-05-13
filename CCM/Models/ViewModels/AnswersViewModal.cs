using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CCM.Models.ViewModels
{
    public class AnswersViewModal
    {
        public string AnswerId { get; set; }
        public string Answer { get; set; }
        public string QuestionGUID { get; set; }
        public string type { get; set; }
        public string haveDateTime { get; set; }

        public string Date { get; set; }
        public  string  IsAnswer { get; set; }
        public string isChecked { get; set; }

    }
}