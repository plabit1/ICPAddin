//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ICP.SP.Intranet.DataAccess
{
    using System;
    using System.Collections.Generic;
    
    public partial class InboxDocumentDetail
    {
        public int IncomingDocumentId { get; set; }
        public int IncomingDocumentDetailId { get; set; }
        public string AssignedByLogin { get; set; }
        public string AssignedByName { get; set; }
        public Nullable<System.DateTime> AssignmentDate { get; set; }
        public string AssignmentStatus { get; set; }
        public string AssignedToLogin { get; set; }
        public string AssignedToName { get; set; }
        public string AssignedToCCLogin { get; set; }
        public string AssignedToCCName { get; set; }
        public string DocumentAnnotation { get; set; }
        public Nullable<System.DateTime> AssignmentDueTo { get; set; }
    
        public virtual InboxDocument InboxDocument { get; set; }
    }
}