using ICP.SP.Intranet.Models.Common;
using ICP.SP.IntranetWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ICP.SP.IntranetWeb.ViewModels
{
    public class AssignedDocsViewModel
    {
        public string Message { get; set; }
        public List<DocumentAssignmentModel> PendDocsInfoList { get; set; }
        public System.Globalization.CultureInfo CurrentCulture { get; set; }
    }
}