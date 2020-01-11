namespace IntraGriegHomolog.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class cad_Situacao
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public cad_Situacao()
        {
            IN_DESKTOP = new HashSet<IN_DESKTOP>();
            IN_LINHA_MOVEL = new HashSet<IN_LINHA_MOVEL>();
            IN_PRINTER = new HashSet<IN_PRINTER>();
            IN_SMARTPHONE = new HashSet<IN_SMARTPHONE>();
            IN_VOIP = new HashSet<IN_VOIP>();
        }

        public int id { get; set; }

        [Required]
        [StringLength(60)]
        [DisplayName("Status")]
        public string descs { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<IN_DESKTOP> IN_DESKTOP { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<IN_LINHA_MOVEL> IN_LINHA_MOVEL { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<IN_PRINTER> IN_PRINTER { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<IN_SMARTPHONE> IN_SMARTPHONE { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<IN_VOIP> IN_VOIP { get; set; }
    }
}
