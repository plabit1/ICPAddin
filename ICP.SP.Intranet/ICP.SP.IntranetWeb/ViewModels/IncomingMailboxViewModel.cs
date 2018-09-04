using ICP.SP.Intranet.Models.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ICP.SP.IntranetWeb.ViewModels
{
    public class IncomingMailboxViewModel
    {
        public MailboxDocumentModel DBModel { get; set; }
        public string ErrorMessage { get; set; }
        public string WarningMessage { get; set; }
        public string ConfirmMessage { get; set; }
        public string PreviousPage { get; set; }
        public string Operation { get; set; }

        public IncomingMailboxViewModel()
        {
            DBModel = new MailboxDocumentModel();
        }
    }
}