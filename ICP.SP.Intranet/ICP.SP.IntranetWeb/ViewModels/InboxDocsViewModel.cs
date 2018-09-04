using ICP.SP.Intranet.Models.Common;
using ICP.SP.IntranetWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ICP.SP.IntranetWeb.ViewModels
{
    public class InboxDocsViewModel
    {
        public string Message { get; set; }
        public List<InboxDocumentModel> MailboxDocumentList { get; set; }
        public System.Globalization.CultureInfo CurrentCulture { get; set; }
        public OutDocumentFilterModel SearchFilter { get; set; }
        public InboxDocsViewModel()
        {
            MailboxDocumentList = new List<InboxDocumentModel>();
        }
    }
}