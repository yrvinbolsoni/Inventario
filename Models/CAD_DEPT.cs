namespace IntraGriegHomolog.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class CAD_DEPT
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public CAD_DEPT()
        {
            CAD_COLABORADOR = new HashSet<CAD_COLABORADOR>();
            IN_PRINTER = new HashSet<IN_PRINTER>();
        }

        public int id { get; set; }

        [Required]
        [StringLength(255)]
        [DisplayName("Departamento")]
        public string descs { get; set; }

        public int? emp { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CAD_COLABORADOR> CAD_COLABORADOR { get; set; }

        public virtual CAD_EMP CAD_EMP { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<IN_PRINTER> IN_PRINTER { get; set; }
    }
}
