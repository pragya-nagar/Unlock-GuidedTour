using System;
using System.Collections.Generic;
using System.Text;

namespace GuidedTour.ViewModel.Request
{
    public class MailRequest
    {
        public string MailTo { get; set; }
        public string MailFrom { get; set; }
        public string Bcc { get; set; }
        public string CC { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }

    }
}
