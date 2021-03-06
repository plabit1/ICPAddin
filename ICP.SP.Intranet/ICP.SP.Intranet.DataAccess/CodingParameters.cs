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
    
    public partial class CodingParameters
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public CodingParameters()
        {
            this.CodingHistory = new HashSet<CodingHistory>();
        }
    
        public int CodingId { get; set; }
        public string Company { get; set; }
        public string Department { get; set; }
        public Nullable<int> Year { get; set; }
        public string Prefix { get; set; }
        public Nullable<int> Code { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CodingHistory> CodingHistory { get; set; }
    }
}
