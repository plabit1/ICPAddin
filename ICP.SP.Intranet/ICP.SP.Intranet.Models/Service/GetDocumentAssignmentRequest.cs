using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICP.SP.Intranet.Models.Service
{
    public class GetDocumentAssignmentRequest
    {
        public int DocumentAssignmentId { get; set; }
        public string SiteUrl { get; set; }
        public string DocumentLibraryId { get; set; }
        public int ListItemId { get; set; }
        public string DocumentStatus { get; set; }
    }
}
