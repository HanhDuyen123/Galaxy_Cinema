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
    
    public partial class SHOWTIME
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public SHOWTIME()
        {
            this.TICKETs = new HashSet<TICKET>();
        }
    
        public long SHOWTIMEID { get; set; }
        public long THEATERID { get; set; }
        public long MOVIEID { get; set; }
        public System.DateTime STARTTIME { get; set; }
        public System.DateTime ENDTIME { get; set; }
    
        public virtual MOVIE MOVIE { get; set; }
        public virtual THEATER THEATER { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TICKET> TICKETs { get; set; }
    }
}
