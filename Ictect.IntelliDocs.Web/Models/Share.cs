//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Ictect.IntelliDocs.Web.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Share
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Share()
        {
            this.AspNetUsers = new HashSet<AspNetUser>();
        }
    
        public int shrId { get; set; }
        public System.DateTime shrCreatedDate { get; set; }
        public int Directory_nodeId { get; set; }
        public int Document_docId { get; set; }
    
        public virtual Document Document { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<AspNetUser> AspNetUsers { get; set; }
    }
}
