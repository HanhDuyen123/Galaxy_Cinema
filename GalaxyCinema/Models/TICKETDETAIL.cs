//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace GalaxyCinema.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class TICKETDETAIL
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public TICKETDETAIL()
        {
            this.TICKETSEATs = new HashSet<TICKETSEAT>();
        }
    
        public long TICKETDETAILID { get; set; }
        public int TICKETID { get; set; }
        public int TICKETPRICEID { get; set; }
        public int QUANTITY { get; set; }
        public decimal TOTALPRICE { get; set; }
    
        public virtual TICKET TICKET { get; set; }
        public virtual TICKETPRICE TICKETPRICE { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TICKETSEAT> TICKETSEATs { get; set; }
    }
}
