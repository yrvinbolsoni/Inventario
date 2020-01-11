namespace IntraGriegHomolog.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class CAD_ITEM_TYPE
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public CAD_ITEM_TYPE()
        {
            CAD_ITEM = new HashSet<CAD_ITEM>();
            IN_HISTORY = new HashSet<IN_HISTORY>();
            NFE = new HashSet<NFE>();
        }

        public int id { get; set; }

        [Required]
        [StringLength(60)]
        [DisplayName("Tipo")]
        public string descs { get; set; }

        [Required]
        [StringLength(2)]
        public string ITEM_TP { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CAD_ITEM> CAD_ITEM { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<IN_HISTORY> IN_HISTORY { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<NFE> NFE { get; set; }
    }
}
