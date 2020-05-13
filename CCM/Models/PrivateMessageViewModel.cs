using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CCM.Models
{
    public class PrivateMessageViewModel
    {

        public string from { get; set; }
        public string SenderName { get; set; }

        public string to { get; set; }

        public string ReceiverName { get; set; }

        public string message { get; set; }

        public string filepath { get; set; }

        public string filename { get; set; }

        public Boolean? Attachment { get; set; }
        public HttpPostedFileBase file { get; set; }



    }
}