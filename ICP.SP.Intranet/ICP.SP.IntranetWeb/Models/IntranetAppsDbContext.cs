using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;

namespace ICP.SP.IntranetWeb.Models
{
    public class DocumentAssignment
    {
        public int AssignmentId { get; set; }
        public string SiteUrl { get; set; }
        public string DocLibrary { get; set; }
        public int ListItemId { get; set; }
        public string Title { get; set; }
        public string AssignedTo { get; set; }
        public string AssignedToName { get; set; }
        public DateTime DueTo { get; set; }
        public int FirstReminderDays { get; set; }
        public int SecondReminderDays { get; set; }
        public string AssignedBy { get; set; }
        public string AssignedByName { get; set; }
        public DateTime AssignmentDate { get; set; }
        public string DocumentURL { get; set; }
    }

    public class MailboxDocument
    {
        public int MailboxDocumentId { get; set; }
        public string SiteUrl { get; set; }
        public string DocLibrary { get; set; }
        public int ListItemId { get; set; }
        public string Title { get; set; }
        public string AssignedTo { get; set; }
        public string AssignedToName { get; set; }
        public string AssignedToAccount { get; set; }
        public string AssignedToCC { get; set; }
        public string AssignedToCCName { get; set; }
        public DateTime DocumentDate { get; set; }
        public string DocumentTitle { get; set; }
        public string DocumentSummary { get; set; }
        public string AssignedBy { get; set; }
        public string AssignedByName { get; set; }
        public DateTime AssignmentDate { get; set; }
        public string DocumentURL { get; set; }
        public string DocumentStatus { get; set; }
    }

    public class MailboxDetails
    {
        public int MailboxDetailId { get; set; }
        public int MailboxDocumentId { get; set; }
        public string AssignedTo { get; set; }
        public string AssignedToName { get; set; }
        public string AssignedBy { get; set; }
        public string AssignedByName { get; set; }
        public DateTime AssignmentDate { get; set; }
        public string ActionName { get; set; }
        public string Annotations { get; set; }
    }
}