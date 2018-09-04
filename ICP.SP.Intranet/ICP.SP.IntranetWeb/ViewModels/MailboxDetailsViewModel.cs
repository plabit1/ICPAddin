using ICP.SP.Intranet.Models.Common;
using ICP.SP.IntranetWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ICP.SP.IntranetWeb.ViewModels
{
    public class MailboxDetailsViewModel
    {
        public string ErrorMessage { get; set; }
        public List<MailboxDocumentDetailsModel> Details { get; set; }
        public int DocumentId { get; set; }
    }
}