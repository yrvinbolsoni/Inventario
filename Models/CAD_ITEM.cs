namespace IntraGriegHomolog.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class CAD_ITEM
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public CAD_ITEM()
        {
            IN_PRINTER = new HashSet<IN_PRINTER>();
            IN_DESKTOP = new HashSet<IN_DESKTOP>();
            IN_DESKTOP1 = new HashSet<IN_DESKTOP>();
            IN_DESKTOP2 = new HashSet<IN_DESKTOP>();
            IN_DESKTOP3 = new HashSet<IN_DESKTOP>();
            IN_DESKTOP4 = new HashSet<IN_DESKTOP>();
            IN_DESKTOP5 = new HashSet<IN_DESKTOP>();
            IN_SMARTPHONE = new HashSet<IN_SMARTPHONE>();
            IN_VOIP = new HashSet<IN_VOIP>();
        }

        public int id { get; set; }

        public int? typeID { get; set; }

        [Required]
        [StringLength(60)]
        [DisplayName("Descrição")]
        public string descs { get; set; }

        public virtual CAD_ITEM_TYPE CAD_ITEM_TYPE { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<IN_PRINTER> IN_PRINTER { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<IN_DESKTOP> IN_DESKTOP { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<IN_DESKTOP> IN_DESKTOP1 { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<IN_DESKTOP> IN_DESKTOP2 { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<IN_DESKTOP> IN_DESKTOP3 { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<IN_DESKTOP> IN_DESKTOP4 { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<IN_DESKTOP> IN_DESKTOP5 { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<IN_SMARTPHONE> IN_SMARTPHONE { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<IN_VOIP> IN_VOIP { get; set; }
    }
}
