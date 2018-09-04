using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICP.SP.Intranet.Models.Common
{
    public class DocumentAssignmentModel
    {
        public int DocumentAssignmentId { get; set; }
        public string SiteUrl { get; set; }
        public string DocumentLibraryId { get; set; }
        public int ListItemId { get; set; }
        public string DocumentUrl { get; set; }
        public string DocumentTitle { get; set; }
        public string AssignedByLogin { get; set; }
        public string AssignedByName { get; set; }
        public DateTime AssignmentDate { get; set; }
        public string AssignmentStatus { get; set; }
        public string AssignedToLogin { get; set; }
        public string AssignedToName { get; set; }
        public string AssignedToControl { get; set; }
        public DateTime DueToDate { get; set; }
        public int FirstReminderDays { get; set; }
        public int SecondReminderDays { get; set; }
        public string Indicator { get; set; }
        public string DocumentSubject { get; set; }
        public string DocumentAnnotations { get; set; }
    }
}
