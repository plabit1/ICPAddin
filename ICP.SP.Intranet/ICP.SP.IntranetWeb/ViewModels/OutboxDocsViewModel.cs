using ICP.SP.Intranet.Models.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ICP.SP.IntranetWeb.ViewModels
{
    public class OutboxDocsViewModel
    {
        public string Message { get; set; }
        public List<OutDocumentModel> MailboxDocumentList { get; set; }
        public System.Globalization.CultureInfo CurrentCulture { get; set; }
        public OutDocumentFilterModel SearchFilter { get; set; }

        public OutboxDocsViewModel()
        {
            MailboxDocumentList = new List<OutDocumentModel>();
        }
    }
}