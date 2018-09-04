using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICP.SP.Intranet.Models.Common
{
    public class InboxDocumentModel
    {
        public int IncomingMailboxId { get; set; }
        public string SiteUrl { get; set; }
        public string DocumentCode { get; set; }
        public DateTime DocumentDate { get; set; }
        public string DocumentSubject { get; set; }
        public string CompanyName { get; set; }
        public string DepartmentName { get; set; }
        public string AssignedByLogin { get; set; }
        public string AssignedByName { get; set; }
        public string AssignedByControl { get; set; }
        public DateTime AssignmentDate { get; set; }
        public string DocumentStatus { get; set; }
        public string AssignedToLogin { get; set; }
        public string AssignedToName { get; set; }
        public string AssignedToControl { get; set; }
        public string AssignedToCCLogin { get; set; }
        public string AssignedToCCName { get; set; }
        public string AssignedToCCControl { get; set; }
        public string FromCompany { get; set; }
        public string FromContact { get; set; }
        public DateTime ResponseDate { get; set; }
        public string Annotations { get; set; }
        public string Indicator { get; set; }
        public DateTime FirstReminderDate { get; set; }
        public DateTime SecondReminderDate { get; set; }
        public string Attachment1Url { get; set; }
        public string Attachment2Url { get; set; }
        public string Attachment3Url { get; set; }
    }
}
