using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICP.SP.Intranet.Models.Common
{
    public class DocumentCodingModel
    {
        public string DocumentSubject { get; set; }
        public string SiteUrl { get; set; }
        public string DocumentLibraryId { get; set; }
        public int ListItemId { get; set; }
        public string RequestedByName { get; set; }
        public string RequestedByLogin { get; set; }
        public DateTime RequestedDate { get; set; }
        public string CompanyName { get; set; }
        public string DepartmentName { get; set; }
    }
}
