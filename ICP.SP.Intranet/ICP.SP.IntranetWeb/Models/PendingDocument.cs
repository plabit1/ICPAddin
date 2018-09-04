using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ICP.SP.IntranetWeb.Models
{
    public class PendingDocument
    {
        public string SiteUrl { get; set; }
        public string DocLibrary { get; set; }
        public int ListItemId { get; set; }
        public string Title { get; set; }
        public string AssignedTo { get; set; }
        public DateTime DueDate { get; set; }
        public int FirstReminderDays { get; set; }
        public string Status { get; set; }
        public string Indicator { get; set; }
        public string DocumentURL { get; set; }
    }
}