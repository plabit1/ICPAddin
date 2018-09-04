using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICP.SP.Intranet.Models.Common
{
    public class MailboxDocumentDetailsModel
    {
        public int MailboxDocumentId { get; set; }
        public int MailboxDocumentDetailId { get; set; }
        public string DocumentTitle { get; set; }
        public string AssignedByLogin { get; set; }
        public string AssignedByName { get; set; }
        public DateTime AssignmentDate { get; set; }
        public string AssignmentStatus { get; set; }
        public string AssignedToLogin { get; set; }
        public string AssignedToName { get; set; }
        public string AssignedToCCLogin { get; set; }
        public string AssignedToCCName { get; set; }
        public string Annotations { get; set; }
        public DateTime DueToDate { get; set; }
    }
}
