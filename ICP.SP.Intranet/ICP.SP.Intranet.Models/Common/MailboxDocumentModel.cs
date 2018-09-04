using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICP.SP.Intranet.Models.Common
{
    public class MailboxDocumentModel
    {
        public int MailboxDocumentId { get; set; }
        public string SiteUrl { get; set; }
        public string DocumentLibraryId { get; set; }
        public int ListItemId { get; set; }
        public string DocumentUrl { get; set; }
        public string DocumentTitle { get; set; }
        public string AssignedByLogin { get; set; }
        public string AssignedByName { get; set; }
        public string AssignedByControl { get; set; }
        public DateTime AssignmentDate { get; set; }
        public string AssignmentStatus { get; set; }
        public string AssignedToLogin { get; set; }
        public string AssignedToName { get; set; }
        public string AssignedToControl { get; set; }
        public string AssignedToCCLogin { get; set; }
        public string AssignedToCCName { get; set; }
        public string AssignedToCCControl { get; set; }
        public DateTime DocumentDate { get; set; }
        public string DocumentSubject { get; set; }
        public string DocumentSummary { get; set; }
        public string Annotations { get; set; }
        public DateTime ResponseDate { get; set; }
        public string Indicator { get; set; }
        public string DocumentFrom { get; set; }
        public DateTime FirstReminderDate { get; set; }
        public DateTime SecondReminderDate { get; set; }
    }
}
