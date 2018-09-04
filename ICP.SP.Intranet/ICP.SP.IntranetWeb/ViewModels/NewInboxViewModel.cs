using ICP.SP.Intranet.Models.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ICP.SP.IntranetWeb.ViewModels
{
    public class NewInboxViewModel
    {
        public InboxDocumentModel DBModel { get; set; }
        public string ErrorMessage { get; set; }
        public string WarningMessage { get; set; }
        public string ConfirmMessage { get; set; }
        public IEnumerable<HttpPostedFileBase> Attachments { get; set; }
        

        public NewInboxViewModel()
        {
            DBModel = new InboxDocumentModel();
        }
    }
}