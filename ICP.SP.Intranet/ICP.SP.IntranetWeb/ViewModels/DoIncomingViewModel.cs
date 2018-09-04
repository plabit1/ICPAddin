using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ICP.SP.IntranetWeb.ViewModels
{
    public class DoIncomingViewModel
    {
        public string AssignedTo { get; set; }
        public string AssignedToCC { get; set; }
        public string SiteUrl { get; set; }
        public string DocLibrary { get; set; }
        public int ListItemId { get; set; }
        public string AssignedBy { get; set; }
        public string Title { get; set; }
        public DateTime AssignmentDate { get; set; }
        public int AssignmentId { get; set; }
        public string ErrorMessage { get; set; }
        public string WarningMessage { get; set; }
        public string ConfirmMessage { get; set; }
        public string WebTitle { get; set; }
        public string DocLibraryTitle { get; set; }
        public string PreviousPage { get; set; }
        public string DocumentURL { get; set; }
        public DateTime DocumentDate { get; set; }
        public string DocumentSubject { get; set; }
        public string DocumentSummary { get; set; }
        public bool IsModified { get; set; }
        public string DocumentStatus { get; set; }
        public string NewAnnotations { get; set; }
    }
}