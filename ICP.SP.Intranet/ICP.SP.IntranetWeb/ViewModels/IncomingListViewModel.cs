using ICP.SP.Intranet.Models.Common;
using ICP.SP.IntranetWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ICP.SP.IntranetWeb.ViewModels
{
    public class IncomingListViewModel
    {
        public string ErrorMessage { get; set; }
        public List<MailboxDocumentModel> MailboxDocumentList { get; set; }
        public System.Globalization.CultureInfo CurrentCulture { get; set; }
    }
}